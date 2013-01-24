using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VAGSuite
{
    public partial class frmProgress : DevExpress.XtraEditors.XtraForm
    {
        public delegate void CancelEvent(object sender, EventArgs e);
        public event frmProgress.CancelEvent onCancelOperation;


        private void CastCancelEvent()
        {
            if (onCancelOperation != null)
            {
                onCancelOperation(this, new EventArgs());
            }
        }

        public frmProgress()
        {
            InitializeComponent();
        }

        public void SetProgress(string text)
        {
            if (label1.Text != text)
            {
                label1.Text = text;
                Application.DoEvents();
            }
        }

        public void SetProgressPercentage(int percentage)
        {
            if (Convert.ToInt32(progressBarControl1.EditValue) != percentage)
            {
                progressBarControl1.EditValue = percentage;
                this.Height = 159;
                progressBarControl1.Visible = true;
                Application.DoEvents();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // cast event to cancel current operation
            CastCancelEvent();
        }
    }
}