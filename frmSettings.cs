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
    public partial class frmSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

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

        public string ProjectFolder
        {
            get
            {
                return buttonEdit1.Text;
            }
            set
            {
                buttonEdit1.Text = value;
                if (buttonEdit1.Text == "")
                {
                    buttonEdit1.Text = Tools.Instance.GetWorkingDirectory() + "\\Projects";
                }
            }
        }

        public bool ShowTablesUpsideDown
        {
            get
            {
                return checkEdit17.Checked;
            }
            set
            {
                checkEdit17.Checked = value;
            }
        }
        public bool RequestProjectNotes
        {
            get
            {
                return checkEdit31.Checked;
            }
            set
            {
                checkEdit31.Checked = value;
            }
        }




        public bool SynchronizeMapviewers
        {
            get
            {
                return checkEdit15.Checked;
            }
            set
            {
                checkEdit15.Checked = value;
            }
        }
        public bool SynchronizeMapviewersDifferentMaps
        {
            get
            {
                return checkEdit11.Checked;
            }
            set
            {
                checkEdit11.Checked = value;
            }
        }

        public ViewType DefaultViewType
        {
            get
            {
                return (ViewType)comboBoxEdit1.SelectedIndex;
            }
            set
            {
                int editValue = (int)value;
                if (editValue > 2) editValue = 2;
                comboBoxEdit1.SelectedIndex = editValue;
            }
        }


        public ViewSize DefaultViewSize
        {
            get
            {
                return (ViewSize)comboBoxEdit2.SelectedIndex;
            }
            set
            {
                comboBoxEdit2.SelectedIndex = (int)value;
            }
        }


       

     


        public bool AutoLoadLastFile
        {
            get
            {
                return checkEdit14.Checked;
            }
            set
            {
                checkEdit14.Checked = value;
            }
        }



        public bool NewPanelsFloating
        {
            get
            {
                return checkEdit12.Checked;
            }
            set
            {
                checkEdit12.Checked = value;
            }
        }


        public bool DisableMapviewerColors
        {
            get
            {
                return checkEdit8.Checked;
            }
            set
            {
                checkEdit8.Checked = value;
            }
        }
        public bool AutoDockSameFile
        {
            get
            {
                return checkEdit9.Checked;
            }
            set
            {
                checkEdit9.Checked = value;
            }
        }

        public bool AutoDockSameSymbol
        {
            get
            {
                return checkEdit10.Checked;
            }
            set
            {
                checkEdit10.Checked = value;
            }
        }


        public bool AutoSizeNewWindows
        {
            get
            {
                return checkEdit1.Checked;
            }
            set
            {
                checkEdit1.Checked = value;
            }
        }

        public bool UseRedAndWhiteMaps
        {
            get
            {
                return checkEdit2.Checked;
            }
            set
            {
                checkEdit2.Checked = value;
            }
        }

        public bool ViewTablesInHex
        {
            get
            {
                return checkEdit4.Checked;
            }
            set
            {
                checkEdit4.Checked = value;
            }
        }

        public bool ShowGraphsInMapViewer
        {
            get
            {
                return checkEdit5.Checked;
            }
            set
            {
                checkEdit5.Checked = value;
            }
        }

        public bool AutoSizeColumnsInViewer
        {
            get
            {
                return checkEdit7.Checked;
            }
            set
            {
                checkEdit7.Checked = value;
            }
        }

        public bool AutoUpdateChecksum
        {
            get
            {
                return checkEdit3.Checked;
            }
            set
            {
                checkEdit3.Checked = value;
            }
        }


        public bool ShowAddressesInHex
        {
            get
            {
                return checkEdit20.Checked;
            }
            set
            {
                checkEdit20.Checked = value;
            }
        }

        public bool UseCodeBlockSynchroniser
        {
            get
            {
                return checkEdit6.Checked;
            }
            set
            {
                checkEdit6.Checked = value;
            }
        }
    


        private void frmSettings_Load(object sender, EventArgs e)
        {

        }

        


       

        private AppSettings m_appSettings;

        public AppSettings AppSettings
        {
            get { return m_appSettings; }
            set { m_appSettings = value; }
        }

        private SymbolCollection m_symbols = new SymbolCollection();

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }


        private void frmSettings_Shown(object sender, EventArgs e)
        {

        }

       

    }
}