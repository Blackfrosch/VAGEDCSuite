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
    public partial class frmFirmwareInfo : DevExpress.XtraEditors.XtraForm
    {
        public frmFirmwareInfo()
        {
            InitializeComponent();
        }


        public string checksumType
        {
            get
            {
                return textEdit7.Text;
            }
            set
            {
                textEdit7.Text = value;
            }
        }

        public string codeBlocks
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

        public string carDetails
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
        public string ecuDetails
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

        public string partNumber
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

        public string SoftwareID
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

        public string InfoString
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

        public string EngineType
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

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // open a codeblock browser screen
            frmCodeBlocks blocks = new frmCodeBlocks();
            blocks.Show();
        }
    }
}