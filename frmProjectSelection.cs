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
    public partial class frmProjectSelection : DevExpress.XtraEditors.XtraForm
    {
        public frmProjectSelection()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            // selected row = ok
            if (gridView1.SelectedRowsCount > 0)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public void SetDataSource(DataTable dt)
        {
            if (dt != null)
            {
                gridControl1.DataSource = dt;
            }
        }

        public string GetProjectName()
        {
            string retval = string.Empty;
            if (gridView1.SelectedRowsCount > 0)
            {
                int[] rows = gridView1.GetSelectedRows();
                if (rows.Length > 0)
                {
                    DataRowView dv = (DataRowView)gridView1.GetRow(Convert.ToInt32(rows.GetValue(0)));
                     if (dv != null)
                     {
                         //sh.Varname = dv.Row["SYMBOLNAME"].ToString();
                         if (dv.Row["Projectname"] != DBNull.Value)
                         {
                             retval = dv.Row["Projectname"].ToString();
                         }
                     }
                }
            }
            return retval;
        }
    }
}