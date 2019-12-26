using A3DBhavCopy.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Data.Entity;
using A3DBhavCopy.Models;
using System.Net.NetworkInformation;
using System.Globalization;

namespace A3DBhavCopy
{
    public partial class FrmDownload : RadForm
    {
        //private RadProgressBarElement RdProgressBar;
        //private RadLabelElement RdLlbMessage;
        private RadLabelElement RdLlbDateRange;
        private RadStatusStrip RdStatusStrip;
        DateTime _DtpFromDate = DateTime.Now;
        DateTime _DtpToDate = DateTime.Now;
        DataSet _DataSet = new DataSet();
        DataTable DtBhavCopyFile = new DataTable();
        DataTable DtBhavCopyData = new DataTable();
        DataView DvBhavCopyFile = new DataView();
        public FrmDownload()
        {
            InitializeComponent();
            RdDtpFrom.Value = DateTime.Now;
            RdDtpTo.Value = DateTime.Now;
            RdRbtDate.CheckState = CheckState.Checked;

            DataColumn DcCol = new DataColumn("lSelect", typeof(bool));
            DcCol.DefaultValue = true;
            DtBhavCopyFile.Columns.Add(DcCol);
            DcCol = new DataColumn("iFileID", typeof(int));
            DcCol.DefaultValue = 0;
            DtBhavCopyFile.Columns.Add(DcCol);

            DcCol = new DataColumn("cFileName", typeof(string));
            DcCol.DefaultValue = "";
            DtBhavCopyFile.Columns.Add(DcCol);

            DcCol = new DataColumn("dFileDate", typeof(DateTime));
            DtBhavCopyFile.Columns.Add(DcCol);

            DcCol = new DataColumn("dFileUploadDate", typeof(DateTime));
            DtBhavCopyFile.Columns.Add(DcCol);

            DcCol = new DataColumn("cFileDownLoadStatus", typeof(string));
            DcCol.DefaultValue = "";
            DtBhavCopyFile.Columns.Add(DcCol);

            DcCol = new DataColumn("cFileUploadStatus", typeof(string));
            DcCol.DefaultValue = "";
            DtBhavCopyFile.Columns.Add(DcCol);

            DcCol = new DataColumn("cFileLoation", typeof(string));
            DcCol.DefaultValue = "";
            DtBhavCopyFile.Columns.Add(DcCol);

            DcCol = new DataColumn("lFileDownloaded", typeof(bool));
            DcCol.DefaultValue = true;
            DtBhavCopyFile.Columns.Add(DcCol);

            this.RdGrdBhavCopyData.EnableFiltering = true;
            this.RdGrdBhavCopyData.MasterTemplate.EnableFiltering = true;

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

        private void FrmDownload_Load(object sender, EventArgs e)
        {

        }

        private void GetMDIProgessBar()
        {
            try
            {
                FrmA3DBhavCopy _RadFormParent = (FrmA3DBhavCopy)this.Parent.Parent.Parent.Parent.Parent;
                RdStatusStrip = _RadFormParent.RdStatusStrip;
                //RdProgressBar = _RadFormParent.RdProgressBar;
                //RdLlbMessage = _RadFormParent.RdLlbMessage;
                RdLlbDateRange = _RadFormParent.RdLlbDateRange;
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
        private void DownLoadFiles(DateTime DtpFromDate, DateTime DtpToDate)
        {
            try
            {
                GetMDIProgessBar();
                RdLlbDateRange.Text = "Date Range Selected  <From " + DtpFromDate.ToString("dd-MMM-yyyy") + " To " + DtpToDate.ToString("dd-MMM-yyyy") + " >";

                if (ClsMessage._IClsMessage.showQuestionMessage(RdLlbDateRange.Text + Environment.NewLine + "Do You Want To Continue?") == DialogResult.No) { return; }

                double _IDaysInMonth = (DtpToDate - DtpFromDate).TotalDays;

                RdProgressBar.Minimum = 1;
                RdProgressBar.Maximum = Convert.ToInt32(_IDaysInMonth);
                RdProgressBar.Value1 = 1;
                RdLlbMessage.Text = "Starting Downloading!! Please Wait....";
                DtBhavCopyFile.Rows.Clear();
                for (int i = 0; i <= _IDaysInMonth; i++)
                {
                    //lSelect cFileName dFileDate cFileDownLoadStatus  // cFileLoation  lFileDownloaded
                    DataRow _DataBhavCopyFileRow = DtBhavCopyFile.NewRow();
                    DateTime _DtMonthDate = _DtpFromDate.AddDays(Convert.ToDouble(i));
                    string StrUrl = "";
                    StrUrl = @"/" + _DtMonthDate.Year + @"/" + _DtMonthDate.ToString("MMM").ToUpper() + @"/cm" + _DtMonthDate.ToString("ddMMMyyyy").ToUpper() + "bhav.csv.zip";

                    _DataBhavCopyFileRow["cFileName"] = "cm" + _DtMonthDate.ToString("ddMMMyyyy").ToUpper() + "bhav.csv.zip";
                    _DataBhavCopyFileRow["dFileDate"] = _DtMonthDate.ToShortDateString();
                    if (CheckForInternetConnection() == true)
                    {

                        RdLlbMessage.Text = "Checking InterNet Connection !! Please Wait....";
                        RdLlbMessage.Text = "Checking File " + "cm" + _DtMonthDate.ToString("ddMMMyyyy") + "bhav.csv.zip" + " Exists Or Not !! Please Wait....";
                        if (CheckFileExists("https://www.nseindia.com/content/historical/EQUITIES" + StrUrl) == false)
                        {
                            RdLlbMessage.Text = "File " + "cm" + _DtMonthDate.ToString("ddMMMyyyy") + "bhav.csv.zip" + " Not Found .";
                            _DataBhavCopyFileRow["cFileDownLoadStatus"] = RdLlbMessage.Text;
                            _DataBhavCopyFileRow["cFileLoation"] = "";
                            _DataBhavCopyFileRow["lFileDownloaded"] = false;
                            DtBhavCopyFile.Rows.Add(_DataBhavCopyFileRow);
                            continue;
                        }
                        string StrTempFolder = System.IO.Path.GetTempPath();

                        using (WebClient webClient = new WebClient())
                        {

                            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                            webClient.DownloadFileAsync(new Uri("https://www.nseindia.com/content/historical/EQUITIES" + StrUrl), StrTempFolder + "cm" + _DtMonthDate.ToString("ddMMMyyyy") + "bhav.csv.zip");
                            _DataBhavCopyFileRow["cFileDownLoadStatus"] = "Download Completed.";
                            _DataBhavCopyFileRow["cFileLoation"] = StrTempFolder + "cm" + _DtMonthDate.ToString("ddMMMyyyy") + "bhav.csv.zip";
                            _DataBhavCopyFileRow["lFileDownloaded"] = true;

                        }
                    }
                    else
                    {
                        _DataBhavCopyFileRow["cFileDownLoadStatus"] = "No InterNet Connection.";
                        _DataBhavCopyFileRow["cFileLoation"] = "";
                        _DataBhavCopyFileRow["lFileDownloaded"] = false;
                    }
                    DtBhavCopyFile.Rows.Add(_DataBhavCopyFileRow);
                    RdProgressBar.Value1 = RdProgressBar.Value1 >= RdProgressBar.Maximum ? RdProgressBar.Maximum : RdProgressBar.Value1 + 1;
                    RdStatusStrip.Refresh();
                }

                RdLlbMessage.Text = "Download Completed.";
                RdLlbMessage.Text = "";
                ClsMessage._IClsMessage.showMessage("Download completed!");
                RdProgressBar.Value1 = 1;
                RdGrdBhavCopyFile.DataSource = DtBhavCopyFile.DefaultView;
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //RdProgressBar.Value1 = e.ProgressPercentage;
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {

        }
        private bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("www.nseindia.com"))
                    return true;

            }
            catch
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        var reply = ping.Send("www.nseindia.com");
                        if (reply != null && reply.Status != IPStatus.Success)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        private bool CheckFileExists(string _url)
        {
            try
            {
                //-> Check if url is valid  
                WebRequest serverRequest = WebRequest.Create(_url);
                WebResponse serverResponse = null;
                try //Try to get response from server  
                {
                    serverResponse = serverRequest.GetResponse();
                    serverResponse.Close();
                    return true;
                }
                catch //If could not obtain any response  
                {
                    if (serverResponse != null) { serverResponse.Close(); }
                    return false;
                }


            }
            catch
            {
                return false;
            }
        }
        private bool LValidateFilter()
        {
            try
            {


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
                DownLoadFiles(_DtpFromDate, _DtpToDate);
                UploadData();
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
            Cursor = Cursors.Default;
        }
        private void UploadData()
        {
            try
            {
                RdLlbMessage.Text = "Uploading Data..";
                RdLlbMessage.Update();
                Application.DoEvents();
                DvBhavCopyFile = new DataView();
                DvBhavCopyFile = _DataSet.DefaultViewManager.CreateDataView(DtBhavCopyFile);
                DvBhavCopyFile.RowFilter = "isnull(lFileDownloaded,0)=1";
                if (DvBhavCopyFile.Count > 0)
                {
                    string StrCsvFileLoation = "";
                    StrCsvFileLoation = Application.StartupPath + @"\BhavCopyDataCsvFiles";
                    if (Directory.Exists(StrCsvFileLoation) == false)
                    {
                        Directory.CreateDirectory(StrCsvFileLoation);
                    }
                    foreach (DataRowView DrvFiles in DvBhavCopyFile)
                    {
                        string zipPath = DrvFiles["cFileLoation"].ToString();
                        string extractPath = StrCsvFileLoation;//+"\\"+ Path.GetFileName(zipPath);

                        using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                                {
                                    entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);
                                }
                            }
                        }

                    }
                    DtBhavCopyData = new DataTable();

                    RdProgressBar.Minimum = 1;
                    RdProgressBar.Maximum = DvBhavCopyFile.Count;
                    foreach (DataRowView DrvFiles in DvBhavCopyFile)
                    {
                        RdProgressBar.Value1 = RdProgressBar.Value1 < DvBhavCopyFile.Count ? RdProgressBar.Value1 + 1 : DvBhavCopyFile.Count;
                        RdProgressBar.Text = ((RdProgressBar.Value1 * 100) / DvBhavCopyFile.Count).ToString() + " %";
                        RdProgressBar.Update();
                        RdProgressBar.Refresh();
                        RdLlbMessage.Text = "Getting < " + DrvFiles["cFileName"].ToString().Replace(".zip", "") + " > File Data..";
                        RdLlbMessage.Update();
                        Application.DoEvents();


                        ConnectCSV(StrCsvFileLoation, DrvFiles["cFileName"].ToString().Replace(".zip", ""));

                    }
                    DataColumn DcCol = new DataColumn("dTIMESTAMP", typeof(DateTime));
                    DtBhavCopyData.Columns.Add(DcCol);

                    foreach (DataRowView DrvFilesData in DtBhavCopyData.DefaultView)
                    {
                        DrvFilesData.BeginEdit();
                        string[] StrDateFormat;
                        StrDateFormat = DrvFilesData["cTIMESTAMP"].ToString().Split('-');

                        DrvFilesData["dTIMESTAMP"] = StrDateFormat[1] + "/" + StrDateFormat[0] + "/" + StrDateFormat[2];
                        DrvFilesData.EndEdit();
                    }
                    RdProgressBar.ResetText();
                    RdProgressBar.Refresh();
                    RdProgressBar.Minimum = 0;
                    RdProgressBar.Maximum = DvBhavCopyFile.Count;
                    RdProgressBar.Value1 = 0;

                    DvBhavCopyFile.Sort = " dFileDate DESC";

                    using (A3DBhavCopyDataContext dbContxt = new A3DBhavCopyDataContext())
                    {
                        using (DbContextTransaction transaction = dbContxt.Database.BeginTransaction())
                        {
                            try
                            {
                                int iFileID = 0;
                                foreach (DataRowView DrvFiles in DvBhavCopyFile)
                                {
                                    string StrFileName = DrvFiles["cFileName"].ToString().Replace(".zip", "");

                                    RdLlbMessage.Text = "Reading < " + StrFileName + " > File Data..";
                                    RdLlbMessage.Update();
                                    Application.DoEvents();

                                   


                                    var vBhavCopyHead = dbContxt._MBhavCopyHead.Where(f => f.cFileName == StrFileName).Select(BCH => BCH.iFileID).ToList();
                                    if (vBhavCopyHead != null && vBhavCopyHead.Count>0)
                                    {

                                        RdLlbMessage.Text = "Deleting existing < " + StrFileName + " > File Data..";
                                        RdLlbMessage.Update();
                                        Application.DoEvents();

                                        foreach (var item in vBhavCopyHead)
                                        {
                                            IList<MClsBhavCopyHead> mClsBhavCopyHeads = dbContxt._MBhavCopyHead.Where(bch => bch.iFileID == item).ToList();
                                            if (mClsBhavCopyHeads != null && mClsBhavCopyHeads.Count > 0)
                                            {
                                                dbContxt._MBhavCopyHead.RemoveRange(mClsBhavCopyHeads);
                                                dbContxt.SaveChanges();
                                                //foreach (var vBhavCopyHeads in mClsBhavCopyHeads)
                                                //{
                                                //    dbContxt._MBhavCopyHead.Remove(vBhavCopyHeads);
                                                //    dbContxt.SaveChanges();
                                                //}
                                            }
                                            IList<MClsBhavCopyDetails> mClsBhavCopyDetails = dbContxt._MClsBhavCopyDetails.Where(bcd => bcd.iFileID == item).ToList();
                                            if (mClsBhavCopyDetails != null && mClsBhavCopyDetails.Count > 0)
                                            {
                                                dbContxt._MClsBhavCopyDetails.RemoveRange(mClsBhavCopyDetails);
                                                dbContxt.SaveChanges();
                                                //foreach (var vBhavCopyDetails in mClsBhavCopyDetails)
                                                //{
                                                //    dbContxt._MClsBhavCopyDetails.Remove(vBhavCopyDetails);
                                                //    dbContxt.SaveChanges();
                                                //}
                                            }

                                        }
                                    }
                                    /**********************************************************************************************/
                                    iFileID = 0;
                                    MClsBhavCopyHead _mClsBhavCopyHead = new MClsBhavCopyHead();
                                    _mClsBhavCopyHead.cFileName = DrvFiles["cFileName"].ToString().Replace(".zip", "");
                                    _mClsBhavCopyHead.dFileDate = Convert.ToDateTime(DrvFiles["dFileDate"]);
                                    _mClsBhavCopyHead.dFileUploadDate = DateTime.Now;
                                    dbContxt._MBhavCopyHead.Add(_mClsBhavCopyHead);
                                    dbContxt.SaveChanges();
                                    iFileID = _mClsBhavCopyHead.iFileID;
                                    /**********************************************************************************************/

                                    RdLlbMessage.Text = "Getting all data of < " + StrFileName + " > File ..";
                                    RdLlbMessage.Update();
                                    Application.DoEvents();

                                    DtBhavCopyData.DefaultView.RowFilter = "cFileName='" + DrvFiles["cFileName"].ToString().Replace(".zip", "") + "'";

                                    RdLlbMessage.Text = "Saving data of  < " + StrFileName + " > File. Total no of record < " + DtBhavCopyData.DefaultView.Count.ToString() + " >";
                                    RdLlbMessage.Update();
                                    Application.DoEvents();


                                    List<MClsBhavCopyDetails> _mClsBhavCopyDetails = new List<MClsBhavCopyDetails>();
                                    _mClsBhavCopyDetails = (from DataRowView DrvFilesData in DtBhavCopyData.DefaultView
                                                            select new MClsBhavCopyDetails()
                                                            {
                                                                iFileID = iFileID,
                                                                cSYMBOL = DrvFilesData["cSYMBOL"].ToString().Trim(),
                                                                cSERIES = DrvFilesData["cSERIES"].ToString().Trim(),
                                                                cOPEN = DrvFilesData["cSYMBOL"].ToString().Trim(),
                                                                cHIGH = DrvFilesData["cHIGH"].ToString().Trim(),
                                                                cLOW = DrvFilesData["cLOW"].ToString().Trim(),
                                                                cCLOSE = DrvFilesData["cCLOSE"].ToString().Trim(),
                                                                cLAST = DrvFilesData["cLAST"].ToString().Trim(),
                                                                cPREVCLOSE = DrvFilesData["cPREVCLOSE"].ToString().Trim(),
                                                                cTOTTRDQTY = DrvFilesData["cTOTTRDQTY"].ToString().Trim(),
                                                                cTOTTRDVAL = DrvFilesData["cTOTTRDVAL"].ToString().Trim(),
                                                                cTIMESTAMP = DrvFilesData["cTIMESTAMP"].ToString().Trim(),
                                                                cTOTALTRADES = DrvFilesData["cTOTALTRADES"].ToString().Trim(),
                                                                cISIN = DrvFilesData["cISIN"].ToString().Trim(),
                                                                dTIMESTAMP = Convert.ToDateTime(DrvFilesData["dTIMESTAMP"])
                                                            }).ToList();
                                    dbContxt._MClsBhavCopyDetails.AddRange(_mClsBhavCopyDetails);

                                    //foreach (DataRowView DrvFilesData in DtBhavCopyData.DefaultView)
                                    //{

                                    //    RdProgressBar.Value1 = RdProgressBar.Value1 < DtBhavCopyData.DefaultView.Count ? RdProgressBar.Value1 + 1 : DtBhavCopyData.DefaultView.Count;
                                    //    RdProgressBar.Text = ((RdProgressBar.Value1 * 100) / DtBhavCopyData.DefaultView.Count).ToString() + " %";
                                    //    RdProgressBar.Update();
                                    //    RdProgressBar.Refresh();
                                    //    Application.DoEvents();

                                    //    MClsBhavCopyDetails _mClsBhavCopyDetails = new MClsBhavCopyDetails();
                                    //    _mClsBhavCopyDetails.iFileID = iFileID;
                                    //    _mClsBhavCopyDetails.cSYMBOL = DrvFilesData["cSYMBOL"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cSERIES = DrvFilesData["cSERIES"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cOPEN = DrvFilesData["cSYMBOL"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cHIGH = DrvFilesData["cHIGH"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cLOW = DrvFilesData["cLOW"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cCLOSE = DrvFilesData["cCLOSE"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cLAST = DrvFilesData["cLAST"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cPREVCLOSE = DrvFilesData["cPREVCLOSE"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cTOTTRDQTY = DrvFilesData["cTOTTRDQTY"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cTOTTRDVAL = DrvFilesData["cTOTTRDVAL"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cTIMESTAMP = DrvFilesData["cTIMESTAMP"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cTOTALTRADES = DrvFilesData["cTOTALTRADES"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.cISIN = DrvFilesData["cISIN"].ToString().Trim();
                                    //    _mClsBhavCopyDetails.dTIMESTAMP = Convert.ToDateTime(DrvFilesData["dTIMESTAMP"]);
                                    //    dbContxt._MClsBhavCopyDetails.Add(_mClsBhavCopyDetails);
                                    //}
                                    DtBhavCopyData.DefaultView.RowFilter = "";

                                    RdProgressBar.Value1 = RdProgressBar.Value1 < DvBhavCopyFile.Count ? RdProgressBar.Value1 + 1 : DvBhavCopyFile.Count;
                                    RdProgressBar.Text = ((RdProgressBar.Value1 * 100) / DvBhavCopyFile.Count).ToString() + " %";
                                    RdProgressBar.Update();
                                    RdProgressBar.Refresh();
                                    Application.DoEvents();
                                }
                                dbContxt.SaveChanges();
                                transaction.Commit();
                            }

                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
                            }

                        }


                    }
                    DtBhavCopyData.AcceptChanges();
                    RdGrdBhavCopyData.DataSource = DtBhavCopyData;
                }
            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
        public void ConnectCSV(string StrFileLocation, string StrFileName)
        {

            try
            {
                string[] Lines = File.ReadAllLines(Path.Combine(StrFileLocation, StrFileName));
                string[] Fields;
                Fields = Lines[0].Split(new char[] { ',' });
                int Cols = Fields.GetLength(0);

                //1st row must be column names; force lower case to ensure matching later on.
                if (DtBhavCopyData == null || DtBhavCopyData.Columns.Count <= 0)
                {
                    for (int i = 0; i < Cols; i++)
                    {
                        DtBhavCopyData.Columns.Add("c" + Fields[i].ToLower(), typeof(string));
                    }
                    DtBhavCopyData.Columns.Add("cFileName", typeof(string));
                }
                DataRow Row;
                for (int i = 1; i < Lines.GetLength(0); i++)
                {
                    Fields = Lines[i].Split(new char[] { ',' });
                    Row = DtBhavCopyData.NewRow();
                    for (int f = 0; f < Cols; f++)
                    {
                        Row[f] = Fields[f];

                    }
                    Row["cFileName"] = StrFileName;
                    DtBhavCopyData.Rows.Add(Row);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void RdBtnSaveFileOnly_Click(object sender, EventArgs e)
        {
            try
            {
                DtBhavCopyFile.Rows.Clear();
                DtBhavCopyData.Rows.Clear();
                using (OpenFileDialog Opg = new OpenFileDialog())
                {
                    Opg.Filter = "";
                    Opg.Multiselect = true;
                    Opg.Title = "Select Bhav-Copy Files";
                    Opg.Filter = "CSV Zip files(*.csv.zip)|*.csv.zip";
                    Opg.RestoreDirectory = true;

                    if (Opg.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var item in Opg.FileNames)
                        {
                            FileInfo fileinfo = new FileInfo(item);
                            DataRow _DataBhavCopyFileRow = DtBhavCopyFile.NewRow();
                            DateTime _DtMonthDate = GetOffLineBhavCopyDate(fileinfo.Name);
                            //string StrUrl = "";
                            //StrUrl = @"/" + _DtMonthDate.Year + @"/" + _DtMonthDate.ToString("MMM").ToUpper() + @"/cm" + _DtMonthDate.ToString("ddMMMyyyy").ToUpper() + "bhav.csv.zip";

                            _DataBhavCopyFileRow["cFileName"] = fileinfo.Name;
                            _DataBhavCopyFileRow["dFileDate"] = _DtMonthDate.ToShortDateString();
                            _DataBhavCopyFileRow["cFileDownLoadStatus"] = "Download Completed.";
                            _DataBhavCopyFileRow["cFileLoation"] = fileinfo.FullName;
                            _DataBhavCopyFileRow["lFileDownloaded"] = true;
                            DtBhavCopyFile.Rows.Add(_DataBhavCopyFileRow);
                        }
                    }
                    RdGrdBhavCopyFile.DataSource = DtBhavCopyFile.DefaultView;
                    UploadData();
                    ClsMessage._IClsMessage.showMessage("Upload completed!");
                }

            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }
        private DateTime GetOffLineBhavCopyDate(string StrFileName)
        {
            try
            {
                DateTime _DtBhavCopyDate = DateTime.Now;

                string StrFileDate = "";
                StrFileDate = StrFileName.ToUpper().Replace("CM", "").Replace("BHAV.CSV.ZIP", "");
                string StrDate = "";
                string StrMonth = "";
                string StrYear = "";
                StrDate = StrFileDate.Substring(0, 2);
                StrMonth = StrFileDate.Substring(2, 3);
                StrYear = StrFileDate.Substring(5, 4);
                _DtBhavCopyDate = new DateTime(Convert.ToInt32(StrYear), DateTime.ParseExact(StrMonth, "MMM", CultureInfo.CurrentCulture).Month, Convert.ToInt32(StrDate));
                return _DtBhavCopyDate;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void RdBtnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (DtBhavCopyFile != null && DtBhavCopyFile.DefaultView.Count > 0)
                {
                    foreach (DataRowView item in DtBhavCopyFile.DefaultView)
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
                DtBhavCopyFile.DefaultView.RowFilter = "";
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

                    if (DtBhavCopyFile != null && DtBhavCopyFile.Rows.Count > 0)
                    {
                        DtBhavCopyFile.DefaultView.RowFilter = "cFileName like'%" + RdTxtSearchCompany.Text.Trim() + "%'";
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

                        if (DtBhavCopyFile != null && DtBhavCopyFile.Rows.Count > 0)
                        {
                            DtBhavCopyFile.DefaultView.RowFilter = "cFileName like'%" + RdTxtSearchCompany.Text.Trim() + "%'";
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                ClsMessage._IClsMessage.ProjectExceptionMessage(ex);
            }
        }

        private void RdBtnReload_Click(object sender, EventArgs e)
        {

        }
    }
}
