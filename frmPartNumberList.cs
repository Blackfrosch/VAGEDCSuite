using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace VAGSuite
{
    public partial class frmPartNumberList : DevExpress.XtraEditors.XtraForm
    {
        
        private string m_selectedpartnumber = "";
        private DataTable partnumbers = new DataTable();
        public string Selectedpartnumber
        {
            get { return m_selectedpartnumber; }
            set { m_selectedpartnumber = value; }
        }

        public frmPartNumberList()
        {
            InitializeComponent();
            partnumbers.Columns.Add("FILENAME");
            partnumbers.Columns.Add("PARTNUMBER");
            partnumbers.Columns.Add("ECUTYPE");
            partnumbers.Columns.Add("CARTYPE");
            partnumbers.Columns.Add("TUNER");
            partnumbers.Columns.Add("STAGE");
            partnumbers.Columns.Add("INFO");
            partnumbers.Columns.Add("SPEED");
            //backgroundWorker1.RunWorkerAsync();
        }

        private void LoadPartNumbersFromFiles()
        {
            if (Directory.Exists(Application.StartupPath + "\\Binaries"))
            {

                string[] binfiles = Directory.GetFiles(Application.StartupPath + "\\Binaries", "*.BIN");
                foreach (string binfile in binfiles)
                {
                    string speed = "20";
                    //if (Find20MhzSequence(binfile)) speed = "20";
                    string binfilename = Path.GetFileNameWithoutExtension(binfile);
                    string partnumber = "";

                    string enginetype = "";
                    string cartype = "";
                    string tuner = "";
                    string stage = "";
                    string additionalinfo = "";
                    if (binfilename.Contains("-"))
                    {
                        char[] sep = new char[1];
                        sep.SetValue('-', 0);
                        string[] values = binfilename.Split(sep);
                        if (values.Length == 1)
                        {
                            // assume partnumber
                            partnumber = (string)binfilename;
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        else if (values.Length == 3)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        else if (values.Length == 4)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            tuner = (string)values.GetValue(3);
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        else if (values.Length == 5)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            tuner = (string)values.GetValue(3);
                            stage = (string)values.GetValue(4);
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        else if (values.Length > 5)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            tuner = (string)values.GetValue(3);
                            stage = (string)values.GetValue(4);
                            for (int tel = 5; tel < values.Length; tel++)
                            {
                                additionalinfo += (string)values.GetValue(tel) + " ";
                            }
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                    }
                    else
                    {
                        // assume partnumber
                        partnumber = (string)binfilename;
                        partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                    }
                   // backgroundWorker1.ReportProgress(0);
                    Application.DoEvents();
                }
            }
            
        }

        private void frmPartNumberList_Load(object sender, EventArgs e)
        {
            PartnumberCollection pnc = new PartnumberCollection();
            DataTable dt = pnc.GeneratePartNumberCollection();

            LoadPartNumbersFromFiles();

            gridControl1.DataSource = dt;
            gridView1.Columns["Carmodel"].Group();
            gridView1.Columns["Model"].Group();
            gridView1.Columns["ECU type"].Group();
            gridView1.BestFitColumns();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if(rows.Length > 0)
            {
                m_selectedpartnumber = (string)gridView1.GetRowCellValue((int)rows.GetValue(0), "Partnumber");
                if (m_selectedpartnumber != null)
                {
                    if (m_selectedpartnumber != string.Empty)
                    {

                        this.Close();
                    }
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if (rows.Length > 0)
            {
                m_selectedpartnumber = (string)gridView1.GetRowCellValue((int)rows.GetValue(0), "Partnumber");
            }
            this.Close();
        }

        private int CheckInAvailableLibrary(string partnumber)
        {
            int retval = 0;
            foreach (DataRow dr in partnumbers.Rows)
            {
                if (dr["PARTNUMBER"] != DBNull.Value)
                {
                    if (dr["PARTNUMBER"].ToString() == partnumber)
                    {
                        retval = 1;
                        break;
                    }
                }
            }
            return retval;
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Partnumber")
            {
                if (e.CellValue != null)
                {
                    if (e.CellValue != DBNull.Value)
                    {
                        int type = CheckInAvailableLibrary(e.CellValue.ToString());
                        if (type == 1)
                        {
                            e.Graphics.FillRectangle(Brushes.YellowGreen, e.Bounds);
                        }
                        if (type == 2)
                        {
                            e.Graphics.FillRectangle(Brushes.YellowGreen, e.Bounds);
                        }
                    }
                }
            }
        }

        private bool Find20MhzSequence(string filename)
        {
            bool retval = false;
            FileInfo fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[32] {0x02, 0x39, 0x00, 0xBF, 0x00, 0xFF, 0xFA, 0x04,
                                            0x00, 0x39, 0x00, 0x80, 0x00, 0xFF, 0xFA, 0x04,
                                            0x02, 0x39, 0x00, 0xC0, 0x00, 0xFF, 0xFA, 0x04,
                                            0x00, 0x39, 0x00, 0x13, 0x00, 0xFF, 0xFA, 0x04};
                /*byte[] seq_mask = new byte[32] {1, 1, 1, 1, 1, 1, 1, 1,
                                            0, 0, 1, 1, 1, 0, 0, 0,   
                                            1, 1, 1, 1, 0, 0, 1, 1,
                                            1, 1, 1, 1, 0, 0, 1, 1};*/
                byte data;
                int i;
                i = 0;
                while (a_fileStream.Position < fi.Length -1)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i])
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text files|*.txt";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                gridControl1.ExportToText(sfd.FileName);
            }
        }
    }
}