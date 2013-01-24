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
    public partial class frmProjectLogbook : DevExpress.XtraEditors.XtraForm
    {
        public frmProjectLogbook()
        {
            InitializeComponent();
        }

        public void LoadLogbookForProject(string projectFolder, string project)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Timestamp", Type.GetType("System.DateTime"));
            dt.Columns.Add("Type");
            dt.Columns.Add("Information");
            string filename = projectFolder + "\\" + project + "\\ProjectLogbook.log";
            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line = string.Empty;
                    char[] sep = new char[1];
                    sep.SetValue('|', 0);

                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(sep);
                        try
                        {
                            string dtstring = (string)values.GetValue(0);

                            int day = Convert.ToInt32(dtstring.Substring(0, 2));
                            int month = Convert.ToInt32(dtstring.Substring(2, 2));
                            int year = Convert.ToInt32(dtstring.Substring(4, 4));
                            int hour = Convert.ToInt32(dtstring.Substring(8, 2));
                            int minute = Convert.ToInt32(dtstring.Substring(10, 2));
                            int second = Convert.ToInt32(dtstring.Substring(12, 2));
                            DateTime timestamp = new DateTime(year, month, day, hour, minute, second);
                            string type = (string)values.GetValue(1);
                            string information = (string)values.GetValue(2);
                            dt.Rows.Add(timestamp, type, information);
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Couldn't load logbook entry: " + E.Message);
                        }
                    }
                }
                gridControl1.DataSource = dt;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}