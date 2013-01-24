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
    public partial class frmSplash : DevExpress.XtraEditors.XtraForm
    {
        public frmSplash()
        {
            InitializeComponent();
            labelControl1.Text = "ver " + Application.ProductVersion.ToString();
        }
    }
}