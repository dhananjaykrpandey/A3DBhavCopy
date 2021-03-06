﻿using A3DBhavCopy.CommonClasses;
using A3DBhavCopy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Export;
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

            RdDtpFrom.Value = DateTime.Now.AddDays(-1);
            RdDtpTo.Value = DateTime.Now.AddDays(-1);
            RdRbtDate.CheckState = CheckState.Checked;
            RdRbtSortByVal.CheckState = CheckState.Checked;
            //RdToolWinReportMenu.ToolCaptionButtons = ~ToolStripCaptionButtons.Close;
            //RdToolWinReportMenu.AllowedDockState = AllowedDockState.Docked | AllowedDockState.AutoHide;
            //RdToolWinReportMenu.ToolCaptionButtons &= ~ToolStripCaptionButtons.SystemMenu;
            //RdToolWinReportMenu.ToolCaptionButtons &= ~ToolStripCaptionButtons.Close;


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
        private void RdRbtDate_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            try
            {
                RadRadioButton _RadRadioButton = (RadRadioButton)sender;
                switch (_RadRadioButton.Name)
                {
                    case "RdRbtDate":
                        RdDtpFrom.CustomFormat = "dd-MMM-yyyy";
                        RdDtpTo.CustomFormat = "dd-MMM-yyyy";
                        break;
                    case "RdRbtMonth":
                        RdDtpFrom.CustomFormat = "MMM-yyyy";
                        RdDtpTo.CustomFormat = "MMM-yyyy";
                        break;
                    case "RdRbtYear":
                        RdDtpFrom.CustomFormat = "yyyy";
                        RdDtpTo.CustomFormat = "yyyy";
                        break;
                    default:
                        break;
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
                DcColBhavCopyData.Caption = "Company Name(Symbol)";
                DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                DcColBhavCopyData = new DataColumn("cSERIES", typeof(string));
                DcColBhavCopyData.DefaultValue = "";
                DcColBhavCopyData.Caption = "Company Series(Series)";
                DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                DcColBhavCopyData = new DataColumn("cSummary", typeof(double));
                DcColBhavCopyData.DefaultValue = 0;
                DcColBhavCopyData.Caption = "Total Summary";
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

                StrSqlQueryFilter = StrSqlQueryFilter + " Convert(DateTime,dTIMESTAMP,101) >=Convert(DateTime,'" + _DtpFromDate.ToString("yyyy/MM/dd") + "',101) AND Convert(DateTime,dTIMESTAMP,101) <=Convert(DateTime,'" + _DtpToDate.ToString("yyyy/MM/dd") + "',101)";
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
                gridViewColumnGroup.Rows[0].ColumnNames.Add("cSummary");
                gridViewColumnGroup.IsPinned = true;
                gridViewColumnGroup.PinPosition = PinnedColumnPosition.Left;
                
                view.ColumnGroups.Add(gridViewColumnGroup);




                for (int i = 0; i <= _IDaysInMonth; i++)
                {

                    DateTime _DtMonthDate = _DtpFromDate.AddDays(Convert.ToDouble(i));
                    if (_DtMonthDate.DayOfWeek.ToString() == "Sunday" || _DtMonthDate.DayOfWeek.ToString() == "Saturday") { continue; }

                    DcColBhavCopyData = new DataColumn("cPREVCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double))
                    {
                        DefaultValue = 0,
                        Caption = "Previous Day Closing Value"
                    };
                    DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                    DcColBhavCopyData = new DataColumn("cCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double))
                    {
                        DefaultValue = 0,
                        Caption = "Today Closing Value"
                    };
                    DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                    if (EnuReportType == enumReportType.SortByQty)
                    {
                        DcColBhavCopyData = new DataColumn("cTOTTRDQTY_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double))
                        {
                            DefaultValue = 0,
                            Caption = "Total Trading Quantity"
                        };
                        DtBhavCopyData.Columns.Add(DcColBhavCopyData);
                    }
                    else
                    {
                        DcColBhavCopyData = new DataColumn("cTOTTRDVAL_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double))
                        {
                            DefaultValue = 0,
                            Caption = "Total Trading Value"
                        };
                        DtBhavCopyData.Columns.Add(DcColBhavCopyData);
                    }
                    DcColBhavCopyData = new DataColumn("cPriceChange_" + _DtMonthDate.ToString("dd-MMM-yyyy"), typeof(double))
                    {
                        DefaultValue = 0,
                        Caption = "Price Change"
                    };
                    DtBhavCopyData.Columns.Add(DcColBhavCopyData);

                    gridViewColumnGroup = new GridViewColumnGroup(_DtMonthDate.ToString("dd-MMM-yyyy"));

                    gridViewColumnGroup.Rows.Add(new GridViewColumnGroupRow());
                    gridViewColumnGroup.Rows[0].ColumnNames.Add("cPREVCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    gridViewColumnGroup.Rows[0].ColumnNames.Add("cCLOSE_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    if (EnuReportType == enumReportType.SortByQty)
                    {
                        gridViewColumnGroup.Rows[0].ColumnNames.Add("cTOTTRDQTY_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    }
                    else
                    {
                        gridViewColumnGroup.Rows[0].ColumnNames.Add("cTOTTRDVAL_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    }
                    gridViewColumnGroup.Rows[0].ColumnNames.Add("cPriceChange_" + _DtMonthDate.ToString("dd-MMM-yyyy"));
                    view.ColumnGroups.Add(gridViewColumnGroup);
                }

                RdGrdReportResult.ViewDefinition = view;



                RdProgressBar.Minimum = 1;
                RdProgressBar.Maximum = DtBhavCopyData.DefaultView.Count;
                RdProgressBar.Value1 = 1;
                RdProgressBar.Text = "Filling Bhav-Copy Data";
                double dblSummary = 0;
                foreach (DataRowView DrvCompany in DtBhavCopyData.DefaultView)
                {
                    dblSummary = 0;
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
                        if (EnuReportType == enumReportType.SortByQty)
                        {
                            if (DtBhavCopyData.Columns.Contains("cTOTTRDQTY_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")))
                            {
                                DrvCompany["cTOTTRDQTY_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")] = DrvBhavCopySqlData["cTOTTRDQTY"];

                            }
                        }
                        else
                        {
                            if (DtBhavCopyData.Columns.Contains("cTOTTRDVAL_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")))
                            {
                                DrvCompany["cTOTTRDVAL_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")] = DrvBhavCopySqlData["cTOTTRDVAL"];

                            }
                        }
                        if (DtBhavCopyData.Columns.Contains("cPriceChange_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")))
                        {
                            DrvCompany["cPriceChange_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")] = Math.Round(Convert.ToDouble(DrvBhavCopySqlData["cCLOSE"]) - Convert.ToDouble(DrvBhavCopySqlData["cPREVCLOSE"]), 2);
                            dblSummary = dblSummary + Convert.ToDouble(DrvCompany["cPriceChange_" + Convert.ToDateTime(DrvBhavCopySqlData["dTIMESTAMP"]).ToString("dd-MMM-yyyy")]);
                        }

                    }
                    DrvCompany["cSummary"] = dblSummary;
                    DtBhavCopySqlData.DefaultView.RowFilter = "";
                    RdProgressBar.Value1 = RdProgressBar.Value1 < DvBhavCopyCompany.Count ? RdProgressBar.Value1 + 1 : DvBhavCopyCompany.Count;
                }
                RdGrdReportResult.DataSource = DtBhavCopyData;
                ConditionalFormattingObject _ConditionalFormattingObject = new ConditionalFormattingObject("MyCondition", ConditionTypes.Less, "0", "", false);
                //obj.CellBackColor = Color.SkyBlue;
                _ConditionalFormattingObject.CellForeColor = Color.Red;
                //obj.TextAlignment = ContentAlignment.MiddleRight;

                RdGrdReportResult.Columns["cSYMBOL"].PinPosition = PinnedColumnPosition.Left;
                RdGrdReportResult.Columns["cSYMBOL"].IsPinned = true;
                RdGrdReportResult.Columns["cSERIES"].PinPosition = PinnedColumnPosition.Left;
                RdGrdReportResult.Columns["cSERIES"].IsPinned = true;
                RdGrdReportResult.Columns["cSummary"].PinPosition = PinnedColumnPosition.Left;
                RdGrdReportResult.Columns["cSummary"].IsPinned = true;
                RdGrdReportResult.Columns["cSummary"].ConditionalFormattingObjectList.Add(_ConditionalFormattingObject);


                foreach (var gridViewDataColumn in RdGrdReportResult.Columns)
                {
                    if (gridViewDataColumn.Name.Contains("cPriceChange_"))
                    {
                        gridViewDataColumn.ConditionalFormattingObjectList.Add(_ConditionalFormattingObject);
                    }
                    
                }
                RdProgressBar.Value1 = 1;
                RdProgressBar.Text = "Done";
                ClsMessage._IClsMessage.showMessage("Done!!");
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
                    else if (value < 0)
                    {
                        e.Graphics.DrawImage(Properties.Resources.DownArrowRed9X16, new PointF(0, 0));
                    }
                    else if (value > 0)
                    {
                        e.Graphics.DrawImage(Properties.Resources.UpArrowGreen8X16, new PointF(0, 0));
                    }
                }
                if (e.Cell != null && e.Cell.RowInfo is GridViewDataRowInfo && e.Cell.ColumnInfo.Name.Contains("cSummary"))
                {
                    double value = Convert.ToDouble(e.Cell.Value);
                    if (value == 0)
                    {
                        return;
                    }
                    else if (value < 0)
                    {
                        e.Graphics.DrawImage(Properties.Resources.DownArrowRed9X16, new PointF(0, 0));
                    }
                    else if (value > 0)
                    {
                        e.Graphics.DrawImage(Properties.Resources.UpArrowGreen8X16, new PointF(0, 0));
                    }
                }
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
        string StrExcelFileName = "";
        private void radButton1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog _SaveFileDialog = new SaveFileDialog()
                {
                    Title = "A3D Bhav-Copy Analysis Report",
                    Filter = "Excel File(*.xlsx)|*.xlsx",
                    RestoreDirectory = true,
                    AddExtension = true,
                    FileName = "A3D Bhav-Copy Analysis Data " + DateTime.Now.ToString("dd-MMM-yyyy hh-mm-ss tt")
                };
                if (_SaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    GridViewSpreadExport spreadExporter = new GridViewSpreadExport(this.RdGrdReportResult);
                    spreadExporter.ExportChildRowsGrouped = true;
                    spreadExporter.ExportViewDefinition = true;
                    spreadExporter.ExportVisualSettings = true;

                    spreadExporter.FileExportMode = FileExportMode.CreateOrOverrideFile;
                    SpreadExportRenderer exportRenderer = new SpreadExportRenderer();

                    spreadExporter.RunExport(_SaveFileDialog.FileName, exportRenderer);
                    StrExcelFileName = _SaveFileDialog.FileName;
                    Cursor = Cursors.Default;
                    if (ClsMessage._IClsMessage.showQuestionMessage("Excel Export Completed." + Environment.NewLine + "Do You Want To Open File?") == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(StrExcelFileName);
                    }
                }

            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }

        private void SpreadExporter_AsyncExportCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (ClsMessage._IClsMessage.showQuestionMessage("Excel Export Completed." + Environment.NewLine + "Do You Want To Open File?") == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(StrExcelFileName);
            }
        }

        private void SpreadExporter_AsyncExportProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            RdProgressBar.Value1 = e.ProgressPercentage;
            RdProgressBar.Update();
            RdProgressBar.Refresh();
            Application.DoEvents();
        }

        private void RdBtnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (DtBhavCopyCompany != null && DtBhavCopyCompany.DefaultView.Count > 0)
                {
                    foreach (DataRowView item in DtBhavCopyCompany.DefaultView)
                    {
                        item.BeginEdit();
                        item["lSelect"] = ((RadButton)sender).Name == "RdBtnSelectAll" ? true : false;
                        item.EndEdit();
                    }
                }
                
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
            Cursor = Cursors.Default;
        }

        private void RdBtnClearSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DtBhavCopyCompany.DefaultView.RowFilter = "";
                RdTxtSearchCompany.Text = "";

            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }

        private void RdBtnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                if (!string.IsNullOrEmpty(RdTxtSearchCompany.Text.Trim()))
                {

                    if (DtBhavCopyCompany != null && DtBhavCopyCompany.Rows.Count > 0)
                    {
                        DtBhavCopyCompany.DefaultView.RowFilter = "cSYMBOL like'%" + RdTxtSearchCompany.Text.Trim() + "%'";
                    }
                }

            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }

        private void RdTxtSearchCompany_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == 13)
                {

                    if (!string.IsNullOrEmpty(RdTxtSearchCompany.Text.Trim()))
                    {

                        if (DtBhavCopyCompany != null && DtBhavCopyCompany.Rows.Count > 0)
                        {
                            DtBhavCopyCompany.DefaultView.RowFilter = "cSYMBOL like'%" + RdTxtSearchCompany.Text.Trim() + "%'";
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }

        private void RdRbdSortByVal_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            try
            {
                RadRadioButton _radRadioButton = ((RadRadioButton)sender);
                if (_radRadioButton.Tag != null && _radRadioButton.Tag.ToString().Trim() != "")
                {
                    switch (_radRadioButton.Tag.ToString().Trim().ToUpper())
                    {
                        case "SORTBYVAL":
                            EnuReportType = enumReportType.SortByValue;
                            RdGrdReportResult.DataSource = null;
                            break;
                        case "SORTBYQUANTITY":
                            EnuReportType = enumReportType.SortByQty;
                            RdGrdReportResult.DataSource = null;
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
    }
}
