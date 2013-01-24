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
    public partial class frmRebuildFileParameters : DevExpress.XtraEditors.XtraForm
    {
        public frmRebuildFileParameters()
        {
            InitializeComponent();
            dateEdit1.DateTime = DateTime.Now;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public DateTime SelectedDateTime
        {
            get
            {
                return dateEdit1.DateTime;
            }
        }

        public bool UseAsNewProjectFile
        {
            get 
            {
                return checkEdit1.Checked;
            }
        }

    }
}