using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;

namespace VAGSuite
{
    public partial class ctrlReleaseNotes : DevExpress.XtraEditors.XtraUserControl
    {
        private DateTime m_LatestReleaseDate = DateTime.MinValue;
        public ctrlReleaseNotes()
        {
            InitializeComponent();
        }

        public static string GetExceptionType(Exception ex)
        {
            string exceptionType = ex.GetType().ToString();
            return exceptionType.Substring(
                exceptionType.LastIndexOf('.') + 1);
        }


        public DateTime StringToDateTime(string cultureName, string dateTimeString)
        {
            CultureInfo culture = new CultureInfo(cultureName);

            //Console.WriteLine();

            // Convert each string in the dateStrings array.
            DateTime dateTimeValue = DateTime.MinValue;

            // Display the first part of the output line.
            //Console.Write(lineFmt, dateStr, cultureName, null);

            try
            {
                // Convert the string to a DateTime object.
                dateTimeValue = Convert.ToDateTime(dateTimeString, culture);

                // Display the DateTime object in a fixed format 
                // if Convert succeeded.
                Console.WriteLine("{0:yyyy-MMM-dd}", dateTimeValue);
            }
            catch (Exception ex)
            {
                // Display the exception type if Parse failed.
                Console.WriteLine("{0}", GetExceptionType(ex));
            }
            return dateTimeValue;

        }


        public void LoadXML(string filename)
        {
            DataSet ds = new DataSet();
            //DataTable dt = new DataTable();
            try
            {
                
                /*dt.TableName = "channel";
                dt.Columns.Add("description");
                dt.Columns.Add("link");
                dt.Columns.Add("pubDate");
                dt.Columns.Add("docs");
                dt.Columns.Add("rating");
                dt.Columns.Add("generator");

                dt.ReadXml(filename);*/
                ds.ReadXml(filename);
                
                if (ds.Tables.Count > 2)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        // get the info from the channel table
                        //System.Globalization.Calendar cal = System.Globalization.CultureInfo.CreateSpecificCulture("nl-NL").Calendar;
                        //System.Globalization.CultureInfo.CreateSpecificCulture("nl-NL").DateTimeFormat


                        m_LatestReleaseDate = StringToDateTime("en-US", ds.Tables[1].Rows[0]["pubDate"].ToString());
                        Console.WriteLine("Release date: " + m_LatestReleaseDate.ToString());
                    }
                    ds.Tables[2].Columns.Add("Date", System.Type.GetType("System.DateTime"));
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {
                        try
                        {
                            dr["Date"] = StringToDateTime("en-US", dr["pubDate"].ToString()).Date;
                        }
                        catch (Exception convE)
                        {
                            Console.WriteLine("Failed to convert datetime: " + convE.Message);
                        }
                    }

                    gridControl1.DataSource = ds.Tables[2];
                    //gridView1.SetMasterRowExpanded(0, true);
                    gridView1.ExpandAllGroups();
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridControl1.ShowPrintPreview();

        }


    }
}
