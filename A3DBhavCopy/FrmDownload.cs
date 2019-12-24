using A3DBhavCopy.CommonClasses;
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

namespace A3DBhavCopy
{
    public partial class FrmDownload : RadForm
    {
        public FrmDownload()
        {
            InitializeComponent();
            RdDtpFrom.Value=DateTime.Now;
            RdDtpTo.Value = DateTime.Now;
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
    }
}
