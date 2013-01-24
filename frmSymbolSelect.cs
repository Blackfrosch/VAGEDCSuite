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
    public partial class frmSymbolSelect : DevExpress.XtraEditors.XtraForm
    {
        SymbolCollection m_symbols;

        public string SelectedSymbol
        {
            get
            {
                if (lookUpEdit1.EditValue != null)
                {
                    return (string)lookUpEdit1.EditValue;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public frmSymbolSelect(SymbolCollection symbols)
        {
            m_symbols = symbols;
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

        private void frmSymbolSelect_Load(object sender, EventArgs e)
        {
            lookUpEdit1.Properties.ValueMember = "Varname";
            lookUpEdit1.Properties.DisplayMember = "Varname";
            lookUpEdit1.Properties.DataSource = m_symbols;
        }
    }
}