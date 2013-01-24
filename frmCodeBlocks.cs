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
    public partial class frmCodeBlocks : DevExpress.XtraEditors.XtraForm
    {
        public frmCodeBlocks()
        {
            InitializeComponent();
        }

        private void frmCodeBlocks_Load(object sender, EventArgs e)
        {
            gridControl1.DataSource = Tools.Instance.codeBlockList;
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Name == "colStartAddress" || e.Column.Name == "colEndAddress" || e.Column.Name == "colAddressID")
            {
                try
                {
                    if (e.CellValue != null)
                    {
                        int addr = Convert.ToInt32(e.CellValue);
                        e.DisplayText = addr.ToString("X8");
                    }
                }
                catch(Exception)
                {

                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}