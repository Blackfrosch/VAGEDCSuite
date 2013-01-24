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
    public partial class frmAbout : DevExpress.XtraEditors.XtraForm
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        public void SetInformation(string info)
        {
            labelControl1.Text = info;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int _step = 0;

        private void labelControl1_DoubleClick(object sender, EventArgs e)
        {
            if (_step == 0) _step++;
            else _step = 0;
        }

        private void labelControl2_DoubleClick(object sender, EventArgs e)
        {
            if (_step == 1) _step++;
            else _step = 0;
        }

        private void labelControl3_DoubleClick(object sender, EventArgs e)
        {
            if (_step == 2) _step++;
            else _step = 0;
        }

        private void labelControl4_DoubleClick(object sender, EventArgs e)
        {
            if (_step == 3) _step++;
            else _step = 0;
        }

        private void labelControl5_DoubleClick(object sender, EventArgs e)
        {
            if (_step == 4)
            {
                // show easter egg (easy to find isn't it, once you got the sourcecode ;))
                frmEast east = new frmEast();
                east.ShowDialog();
                _step = 0;
            }
        }
    }
}