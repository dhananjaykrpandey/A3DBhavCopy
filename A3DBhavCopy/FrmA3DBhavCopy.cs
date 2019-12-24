using A3DBhavCopy.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace A3DBhavCopy
{
    public partial class FrmA3DBhavCopy : RadForm
    {
        public FrmA3DBhavCopy()
        {
            InitializeComponent();
            RdDockMain.AutoDetectMdiChildren = true;
            // RdSpltBtnItemThemeAqua.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeBreeze.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeDefault.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeDesert.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeOffice2013Dark.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeOffice2013Light.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeTelerikMetro.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeTelerikMetroBlue.Click += BtnThemeMetro_Click;
            
            RdSpltBtnItemThemeVisualStudio2012Dark.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeVisualStudio2012Light.Click += BtnThemeMetro_Click;
            // RdSpltBtnItemThemeWindows7.Click += BtnThemeMetro_Click;
            RdSpltBtnItemThemeWindows8.Click += BtnThemeMetro_Click;
        }
        public RadForm TryGetFormByName(string frmname)
        {
            var formType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(a => (a.BaseType == typeof(RadForm) || a.BaseType == typeof(Form)) && a.Name == frmname);

            if (formType == null) // If there is no form with the given frmname
                return null;


            return (RadForm)Activator.CreateInstance(formType);

        }


        private void OpenForm(string strMenuName, string strFormName)
        {
            RadForm _RadForm = new RadForm();
            _RadForm = TryGetFormByName(strFormName);
            HostWindow _HostWindowForm = null;
            if (ClsUtility._IClsUtility.IsFormOpen(_RadForm.GetType(), RdDockMain, out _HostWindowForm))
            {
                _HostWindowForm.ActivateAsMdiChild();
                _RadForm.Activate();
                RdDockMain.ActivateWindow(_HostWindowForm);
                RdDockMain.ActivateMdiChild(_RadForm);

            }
            else
            {
                //obj = new Form();
                // _RadForm = TryGetFormByName(strFormName);
                _RadForm.MdiParent = this;
                _RadForm.Show();
                _RadForm.Activate();
                RdDockMain.ActivateMdiChild(_RadForm);

            }
        }

        private void RdBtnMenuDownloadBhavCopy_Click(object sender, EventArgs e)
        {
            try
            {
                RadButtonElement _RadButtonElement = (RadButtonElement)sender;
                if ((!string.IsNullOrEmpty(_RadButtonElement.Tag.ToString()) && _RadButtonElement.Tag.ToString() != ""))
                {
                    OpenForm(_RadButtonElement.Text, _RadButtonElement.Tag.ToString());
                }
                if ((!string.IsNullOrEmpty(_RadButtonElement.Text) && _RadButtonElement.Text != "") && (_RadButtonElement.Text.ToUpper() == "LOG OUT & EXIST"))
                {

                    DialogResult dgRsult = MessageBox.Show(this, "Do you want exit or log out?" + Environment.NewLine + "Click Yes for exit No for Log Out.", "SBS ATM", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                    switch (dgRsult)
                    {
                        case DialogResult.Yes:
                            Application.Exit();
                            break;
                        case DialogResult.No:
                            Application.Restart();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {


                ClsMessage._IClsMessage.ProjectExceptionMessage(ex.Message);
            }

        }

        private void BtnThemeMetro_Click(object sender, EventArgs e)
        {
            try
            {
                RadMenuItem radMenuItem = (RadMenuItem)sender;
                ThemeResolutionService.ApplicationThemeName = radMenuItem.Text;
            }
            catch (Exception ex)
            {


                ClsMessage._IClsMessage.ProjectExceptionMessage(ex.Message);

            }
        }
    }
}
