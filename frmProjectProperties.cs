using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace VAGSuite
{
    public partial class frmProjectProperties : DevExpress.XtraEditors.XtraForm
    {
        public frmProjectProperties()
        {
            InitializeComponent();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        public string CarMake
        {
            get
            {
                return textEdit1.Text;
            }
            set
            {
                textEdit1.Text = value;
            }
        }
        public string CarModel
        {
            get
            {
                return textEdit2.Text;
            }
            set
            {
                textEdit2.Text = value;
            }
        }
        public string CarMY
        {
            get
            {
                return textEdit3.Text;
            }
            set
            {
                textEdit3.Text = value;
            }
        }
        public string CarVIN
        {
            get
            {
                return textEdit4.Text;
            }
            set
            {
                textEdit4.Text = value;
            }
        }
        public string ProjectName
        {
            get
            {
                return textEdit5.Text;
            }
            set
            {
                textEdit5.Text = value;
            }
        }

        public string Version
        {
            get
            {
                return textEdit6.Text;
            }
            set
            {
                textEdit6.Text = value;
            }
        }

        public string BinaryFile
        {
            get
            {
                return buttonEdit1.Text;
            }
            set
            {
                buttonEdit1.Text = value;
            }
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                buttonEdit1.Text = ofd.FileName;
            }
        }
    }
}