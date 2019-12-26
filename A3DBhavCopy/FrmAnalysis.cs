using A3DBhavCopy.CommonClasses;
using A3DBhavCopy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace A3DBhavCopy
{
    public partial class FrmAnalysis : RadForm
    {
        DateTime _DtpFromDate = DateTime.Now;
        DateTime _DtpToDate = DateTime.Now;
        DataSet _DataSet = new DataSet();
        DataTable DtBhavCopyCompany = new DataTable();
        DataTable DtBhavCopyData = new DataTable();
        DataTable DtBhavCopySqlData = new DataTable();
        DataView DvBhavCopyCompany = new DataView();
        public FrmAnalysis()
        {
            InitializeComponent();

            RdDtpFrom.Value = DateTime.Now;
            RdDtpTo.Value = DateTime.Now;
            RdRbtDate.CheckState = CheckState.Checked;

            RdToolWinReportMenu.ToolCaptionButtons = ~ToolStripCaptionButtons.Close;
            RdToolWinReportMenu.AllowedDockState = AllowedDockState.Docked | AllowedDockState.AutoHide;
            RdToolWinReportMenu.ToolCaptionButtons &= ~ToolStripCaptionButtons.SystemMenu;
            RdToolWinReportMenu.ToolCaptionButtons &= ~ToolStripCaptionButtons.Close;


            RdToolWinSymbol.ToolCaptionButtons = ~ToolStripCaptionButtons.Close;
            RdToolWinSymbol.AllowedDockState = AllowedDockState.Docked | AllowedDockState.AutoHide;
            RdToolWinSymbol.ToolCaptionButtons &= ~ToolStripCaptionButtons.SystemMenu;
            RdToolWinSymbol.ToolCaptionButtons &= ~ToolStripCaptionButtons.Close;

            this.RdGrdCompanies.EnableFiltering = true;
            this.RdGrdCompanies.MasterTemplate.EnableFiltering = true;

            this.RdGrdReportResult.EnableFiltering = true;
            this.RdGrdReportResult.MasterTemplate.EnableFiltering = true;


            DataColumn DcCol = new DataColumn("lSelect", typeof(bool));
            DcCol.DefaultValue = true;
            DtBhavCopyCompany.Columns.Add(DcCol);
            DcCol = new DataColumn("cSYMBOL", typeof(string));
            DcCol.DefaultValue = "";
            DtBhavCopyCompany.Columns.Add(DcCol);
            DcCol = new DataColumn("cSERIES", typeof(string));
            DcCol.DefaultValue = "";
            DtBhavCopyCompany.Columns.Add(DcCol);

        }

        enum enumReportType
        {
            SortByValue,
            SortByQty
        }
        private enumReportType EnuReportType { get; set; } = enumReportType.SortByValue;
        private void FrmAnalysis_Load(object sender, EventArgs e)
        {
            GetCompaniesDetails();
        }
        private void GetCompaniesDetails()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                using (var DbContext = new A3DBhavCopyDataContext())
                {

                    List<MClsCompanies> LstmClsCompanies = DbContext._MClsBhavCopyDetails.Select(cmp => new MClsCompanies { cSYMBOL = cmp.cSYMBOL, cSERIES = cmp.cSERIES }).OrderBy(cmpor => cmpor.cSYMBOL).Distinct().ToList();
                    foreach (var vCompanies in LstmClsCompanies)
                    {
                        DataRow dataRow = DtBhavCopyCompany.NewRow();
                        dataRow["cSYMBOL"] = vCompanies.cSYMBOL;
                        dataRow["cSERIES"] = vCompanies.cSERIES;
                        DtBhavCopyCompany.Rows.Add(dataRow);
                    }
                    DtBhavCopyCompany.AcceptChanges();
                }
                RdGrdCompanies.DataSource = DtBhavCopyCompany.DefaultView;
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
        private void RdBtnReload_Click(object sender, EventArgs e)
        {
            GetCompaniesDetails();
        }
        private void RdTreeMenu_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Tag != null && e.Node.Tag.ToString().Trim() != "")
                {
                    switch (e.Node.Tag.ToString().Trim().ToUpper())
                    {
                        case "SORTBYVAL":
                            EnuReportType = enumReportType.SortByValue;
                            break;
                        case "SortByQuantity":
                            EnuReportType = enumReportType.SortByQty;
                            break;
                        default:
                            EnuReportType = enumReportType.SortByValue;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
        private bool LValidateFilter()
        {
            try
            {
                DvBhavCopyCompany = _DataSet.DefaultViewManager.CreateDataView(DtBhavCopyCompany);
                DvBhavCopyCompany.RowFilter = "Isnull(lSelect,0)=1";

                bool LResult = true;
                if (RdRbtMonth.CheckState != CheckState.Unchecked && RdRbtYear.CheckState != CheckState.Unchecked && RdRbtDate.CheckState != CheckState.Unchecked)
                {
                    ClsMessage._IClsMessage.ProjectExceptionMessage("Please Select Filter Type");
                    RdRbtMonth.Focus();
                    RdRbtMonth.CheckState = CheckState.Checked;
                    LResult = false;
                }
                else if (RdDtpFrom.Value > RdDtpTo.Value)
                {
                    ClsMessage._IClsMessage.ProjectExceptionMessage("From Date Cannot Be Gratter Than To Date");
                    RdDtpFrom.Focus();
                    LResult = false;
                }
                else if (DvBhavCopyCompany == null || DvBhavCopyCompany.Count <= 0)
                {
                    ClsMessage._IClsMessage.ProjectExceptionMessage("Please Select Company Name");
                    RdBtnReload.Focus();
                    LResult = false;
                }
                return LResult;
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
                return false;
            }
        }
        private void RdBtnDownloadAndSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (LValidateFilter() == false) { return; }

                if (RdRbtDate.IsChecked)
                {
                    _DtpFromDate = new DateTime(RdDtpFrom.Value.Year, RdDtpFrom.Value.Month, RdDtpFrom.Value.Day);
                    _DtpToDate = new DateTime(RdDtpTo.Value.Year, RdDtpTo.Value.Month, RdDtpTo.Value.Day);
                }
                else if (RdRbtMonth.IsChecked)
                {
                    _DtpFromDate = new DateTime(RdDtpFrom.Value.Year, RdDtpFrom.Value.Month, 1);
                    _DtpToDate = new DateTime(RdDtpTo.Value.Year, RdDtpTo.Value.Month, DateTime.DaysInMonth(RdDtpTo.Value.Year, RdDtpTo.Value.Month));
                }
                else if (RdRbtYear.IsChecked)
                {
                    _DtpFromDate = new DateTime(RdDtpFrom.Value.Year, 01, 01);
                    _DtpToDate = new DateTime(RdDtpTo.Value.Year, 12, 31);

                }
                Cursor = Cursors.WaitCursor;
                PopulateData();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);

            }
        }

        private void PopulateData()
        {
            try
            {
                DtBhavCopyData = new DataTable();
                DtBhavCopySqlData = new DataTable();
                RdGrdReportResult.DataSource = null;
                DataColumn DcColBhavCopyData = new DataColumn("cSYMBOL", typeof(string));
                DcColBhavCopyData.DefaultValue = "";
                DcColBhavCopyData.Caption = "Compnay Name(Symbol)";
                DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                DcColBhavCopyData = new DataColumn("cSERIES", typeof(string));
                DcColBhavCopyData.DefaultValue = "";
                DcColBhavCopyData.Caption = "Compnay Series(Series)";
                DtBhavCopyData.Columns.Add(DcColBhavCopyData);
                string StrSqlQuery = "";
                string StrSqlQueryFilter = "Where ";
                RdProgressBar.Minimum = 1;
                RdProgressBar.Maximum = DvBhavCopyCompany.Count;
                RdProgressBar.Value1 = 1;
                RdProgressBar.Text = "Filling Company Details";
                DataRow _DataRowBhavCopy;
                for (int i = 0; i < DvBhavCopyCompany.Count; i++)
                {
                    _DataRowBhavCopy = DtBhavCopyData.NewRow();
                    _DataRowBhavCopy["cSYMBOL"] = DvBhavCopyCompany[i]["cSYMBOL"];
                    _DataRowBhavCopy["cSERIES"] = DvBhavCopyCompany[i]["cSERIES"];
                    DtBhavCopyData.Rows.Add(_DataRowBhavCopy);

                    RdProgressBar.Value1 = RdProgressBar.Value1 < DvBhavCopyCompany.Count ? RdProgressBar.Value1 + 1 : DvBhavCopyCompany.Count;
                    RdProgressBar.Text = " Filling Company Details  - " + ((RdProgressBar.Value1 * 100) / DvBhavCopyCompany.Count).ToString() + " %";
                    RdProgressBar.Update();
                    RdProgressBar.Refresh();
                    Application.DoEvents();
                }

                RdProgressBar.Value1 = 1;
                RdProgressBar.Text = "Filling Company Completed";

                string StrCompnay = "";
                StrCompnay = string.Join("','", DvBhavCopyCompany.ToTable().AsEnumerable().Select(x => x.Field<string>("cSYMBOL")).ToArray());//+ "_" + x.Field<string>("cSERIES")

                StrSqlQueryFilter = StrSqlQueryFilter + " Convert(DateTime,dTIMESTAMP,101) >=Convert(DateTime,'" + _DtpFromDate + "',101) AND Convert(DateTime,dTIMESTAMP,101) <=Convert(DateTime,'" + _DtpToDate + "',101)";
                StrSqlQuery = @"select * from [A3DBhavCopyData].[dbo].[BhavCopyDetails]";
                StrSqlQuery = StrSqlQuery + Environment.NewLine + StrSqlQueryFilter + Environment.NewLine + " AND cSYMBOL  IN ('" + StrCompnay + "')";//+ '_' + cSERIES

                using (var DbContext = new A3DBhavCopyDataContext())
                {
                    var vAtmUpTime = DbContext.Database.SqlQuery<MClsBhavCopyDetails>(StrSqlQuery).ToList();
                    DtBhavCopySqlData = ClsUtility._IClsUtility.NewTable(string.Concat("BhavCopyData"), vAtmUpTime);
                }

                double _IDaysInMonth = (_DtpToDate - _DtpFromDate).TotalDays;



                ColumnGroupsViewDefinition view = new ColumnGroupsViewDefinition();
                GridViewColumnGroup gridViewColumnGroup;
                gridViewColumnGroup = new GridViewColumnGroup("Company Details");
                gridViewColumnGroup.Rows.Add(new GridViewColumnGroupRow());
                gridViewColumnGroup.Rows[0].ColumnNames.Add("cSYMBOL");
                gridViewColumnGroup.Rows[0].ColumnNames.Add("cSERIES");
                view.ColumnGroups.Add(gridViewColumnGroup);

                


                for (int i = 0; i <= _IDaysInMonth; i++)
                {

                    DateTime _DtMonthDate = _DtpFromDate.AddDays(Convert.ToDouble(i));
                    if (_DtMonthDate.DayOfWeek.ToString() == "Sunday" || _DtMonthDate.DayOfWeek.ToString() == "Saturday") { continue; }

                    DcColBhavCopyData = new DataColumn("cPREVCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double));
                    DcColBhavCopyData.DefaultValue = 0;
                    DcColBhavCopyData.Caption = "Previous Day Closing Value";
                    DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                    DcColBhavCopyData = new DataColumn("cCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double));
                    DcColBhavCopyData.DefaultValue = 0;
                    DcColBhavCopyData.Caption = "Today Closing Value";
                    DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                    DcColBhavCopyData = new DataColumn("cTOTTRDVAL_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double));
                    DcColBhavCopyData.DefaultValue = 0;
                    DcColBhavCopyData.Caption = "Total Trading Value";
                    DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                    DcColBhavCopyData = new DataColumn("cPriceChange_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double));
                    DcColBhavCopyData.DefaultValue = 0;
                    DcColBhavCopyData.Caption = "Price Change";
                    DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                    gridViewColumnGroup = new GridViewColumnGroup(_DtMonthDate.ToString("dd-MMM-yyyy"));

                    gridViewColumnGroup.Rows.Add(new GridViewColumnGroupRow());
                    gridViewColumnGroup.Rows[0].ColumnNames.Add("cPREVCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    gridViewColumnGroup.Rows[0].ColumnNames.Add("cCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    gridViewColumnGroup.Rows[0].ColumnNames.Add("cTOTTRDVAL_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    gridViewColumnGroup.Rows[0].ColumnNames.Add("cPriceChange_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    view.ColumnGroups.Add(gridViewColumnGroup);
                }

                RdGrdReportResult.ViewDefinition = view;

               

                RdProgressBar.Minimum = 1;
                RdProgressBar.Maximum = DtBhavCopyData.DefaultView.Count;
                RdProgressBar.Value1 = 1;
                RdProgressBar.Text = "Filling Bhav-Copy Data";

                foreach (DataRowView DrvCompany in DtBhavCopyData.DefaultView)
                {

                    RdProgressBar.Text = " Filling < " + DrvCompany["cSYMBOL"] + " > Company Data  - " + ((RdProgressBar.Value1 * 100) / DtBhavCopyData.DefaultView.Count).ToString() + " %";
                    RdProgressBar.Update();
                    RdProgressBar.Refresh();
                    Application.DoEvents();

                    DtBhavCopySqlData.DefaultView.RowFilter = "cSYMBOL='" + DrvCompany["cSYMBOL"] + "' AND cSERIES='" + DrvCompany["cSERIES"] + "' ";
                    foreach (DataRowView DrvBhavCopySqlData in DtBhavCopySqlData.DefaultView)
                    {
                        //DrvCompany["cPREVCLOSE"] = DrvBhavCopySqlData["cPREVCLOSE"];
                        //DrvCompany["cCLOSE"] = DrvBhavCopySqlData["cCLOSE"];
                        //DrvCompany["cTOTTRDVAL"] = DrvBhavCopySqlData["cTOTTRDVAL"];

                        if (DtBhavCopyData.Columns.Contains("cPREVCLOSE_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")))
                        {
                            DrvCompany["cPREVCLOSE_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")] = DrvBhavCopySqlData["cPREVCLOSE"];

                        }
                        if (DtBhavCopyData.Columns.Contains("cCLOSE_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")))
                        {
                            DrvCompany["cCLOSE_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")] = DrvBhavCopySqlData["cCLOSE"];

                        }
                        if (DtBhavCopyData.Columns.Contains("cTOTTRDVAL_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")))
                        {
                            DrvCompany["cTOTTRDVAL_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")] = DrvBhavCopySqlData["cTOTTRDVAL"];

                        }
                        if (DtBhavCopyData.Columns.Contains("cPriceChange_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")))
                        {
                            DrvCompany["cPriceChange_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")] = Math.Round(Convert.ToDouble(DrvBhavCopySqlData["cCLOSE"])-Convert.ToDouble(DrvBhavCopySqlData["cPREVCLOSE"]),2);

                        }
                        //DrvAtmID["cAverage"] = _TotalData / _IDaysInMonth;//DrvAtmDeteData["cTotalPerInService"];
                    }
                    DtBhavCopySqlData.DefaultView.RowFilter = "";
                    RdProgressBar.Value1 = RdProgressBar.Value1 < DvBhavCopyCompany.Count ? RdProgressBar.Value1 + 1 : DvBhavCopyCompany.Count;
                }
                RdGrdReportResult.DataSource = DtBhavCopyData;
                ConditionalFormattingObject _ConditionalFormattingObject = new ConditionalFormattingObject("MyCondition", ConditionTypes.Less, "0", "", false);
                //obj.CellBackColor = Color.SkyBlue;
                _ConditionalFormattingObject.CellForeColor = Color.Red;
                //obj.TextAlignment = ContentAlignment.MiddleRight;
                foreach (var gridViewDataColumn in RdGrdReportResult.Columns)
                {
                    if (gridViewDataColumn.Name.Contains("cPriceChange_"))
                    {
                        gridViewDataColumn.ConditionalFormattingObjectList.Add(_ConditionalFormattingObject);
                    }
                }
                RdProgressBar.Value1 = 1;
                RdProgressBar.Text = "Done";
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }

        private void RdGrdReportResult_CellPaint(object sender, GridViewCellPaintEventArgs e)
        {
            try
            {
                if (e.Cell != null && e.Cell.RowInfo is GridViewDataRowInfo && e.Cell.ColumnInfo.Name.Contains("cPriceChange_"))
                {
                    double value = Convert.ToDouble(e.Cell.Value);
                    if (value == 0)
                    {
                        return;
                    }
                    else if (value<0)
                    {
                        e.Graphics.DrawImage(A3DBhavCopy.Properties.Resources.DownArrowRed9X16,new PointF(0,0));
                    }
                    else if (value > 0)
                    {
                        e.Graphics.DrawImage(A3DBhavCopy.Properties.Resources.UpArrowGreen8X16, new PointF(0, 0));
                    }
                    //Brush brush = value < 0 ? Brushes.Red : Brushes.Green;
                    //using (Font font = new Font("Segoe UI", 17))
                    //{
                    //    e.Graphics.DrawString("*", font, brush, Point.Empty);
                    //}
                }
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
    }
}
