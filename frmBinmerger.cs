using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraEditors;

namespace VAGSuite
{
    public partial class frmBinmerger : DevExpress.XtraEditors.XtraForm
    {
        public frmBinmerger()
        {
            InitializeComponent();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(buttonEdit1.Text))
                {
                    if (File.Exists(buttonEdit2.Text))
                    {
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            // 3 bestanden bekend.. converteren
                            FileInfo fi = new FileInfo(buttonEdit1.Text);
                            FileInfo fi2 = new FileInfo(buttonEdit2.Text);
                            if (fi.Length == fi2.Length)
                            {
                                FileStream fs = File.Create(saveFileDialog1.FileName);
                                BinaryWriter bw = new BinaryWriter(fs);

                                FileStream fsi1 = File.OpenRead(buttonEdit1.Text);
                                BinaryReader br1 = new BinaryReader(fsi1);

                                FileStream fsi2 = File.OpenRead(buttonEdit2.Text);
                                BinaryReader br2 = new BinaryReader(fsi2);

                                for (int tel = 0; tel < fi.Length; tel++)
                                {
                                    Byte ib1 = br1.ReadByte();
                                    Byte ib2 = br2.ReadByte();
                                    bw.Write(ib2);
                                    bw.Write(ib1);
                                }

                                bw.Close();
                                fs.Close();
                                fsi1.Close();
                                br1.Close();
                                fsi2.Close();
                                br2.Close();
                                MessageBox.Show("Files merged successfully");
                            }
                            else
                            {
                                MessageBox.Show("File lengths don't match, unable to merge!");
                            }
                            
                        }
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            this.Close();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                buttonEdit1.Text = openFileDialog1.FileName;
            }
        }

        private void buttonEdit2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                buttonEdit2.Text = openFileDialog1.FileName;
            }
        }
    
    }
}