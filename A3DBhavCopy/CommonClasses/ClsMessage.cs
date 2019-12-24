using System;
using System.Windows.Forms;
namespace A3DBhavCopy.CommonClasses
{
    public  class ClsMessage
    {
        private static ClsMessage _iClsMessage = null;
        public ClsMessage()

        {

        }
        public static ClsMessage _IClsMessage
        {
            get
            {
                if (_iClsMessage==null)
                {
                    _iClsMessage = new ClsMessage();
                }
                return _iClsMessage;
            }
            
        }

         readonly string   ProjectName ="A3D Bhav-Copy Analysis System";

        public  void showMessage(string msg, MessageBoxIcon Icon=MessageBoxIcon.Information)
        {
            MessageBox.Show(msg, ProjectName, MessageBoxButtons.OK, Icon);
        }
        public  void ProjectExceptionMessage(string msg)
        {
            MessageBox.Show(msg, ProjectName, MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        public  void ProjectExceptionMessage(Exception msg)
        {
            string innerex = "";
            if (msg.InnerException!=null)
            {
                innerex = msg.InnerException.ToString();
            }
            MessageBox.Show(msg + Environment.NewLine + innerex , ProjectName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public  DialogResult showQuestionMessage(string msg)
        {
            return MessageBox.Show(msg, ProjectName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        public  void showSaveMessage()
        {
            MessageBox.Show("Record save successfully!", ProjectName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public  void showDeleteMessage()
        {
            MessageBox.Show("Record deleted successfully!", ProjectName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public  DialogResult showAskDeleteMessage()
        {
            return MessageBox.Show("Are you sure want to delete this record?", ProjectName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        public  DialogResult showAskExitMessage()
        {
            return MessageBox.Show("Are you sure want to exit?", ProjectName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        public  DialogResult showAskDiscardMessage()
        {
            return MessageBox.Show("Are you sure want to discard changes?", ProjectName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
