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
    public partial class frmDecodeVIN : DevExpress.XtraEditors.XtraForm
    {
        public frmDecodeVIN()
        {
            InitializeComponent();
        }

        private void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecodeVIN();
            }
        }

        private void DecodeVIN()
        {
            
            lblCarMake.Text = "---";
            lblCarModel.Text = "---";
            lblMakeyear.Text = "---";
            lblPlant.Text = "---";
            lblChassis.Text = "---";
            lblEngineType.Text = "---";
            
            VINDecoder decoder = new VINDecoder();
            VINCarInfo carinfo = decoder.DecodeVINNumber(textEdit1.Text);
            //lblBody.Text = carinfo.Body;

            lblCarMake.Text = carinfo.Make;
            lblCarModel.Text = carinfo.Model;
            lblMakeyear.Text = carinfo.Makeyear.ToString();
            lblPlant.Text = carinfo.PlantInfo;
            lblChassis.Text = carinfo.Platform;
            lblEngineType.Text = carinfo.EngineType;
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DecodeVIN();
            
        }

        private void frmDecodeVIN_Load(object sender, EventArgs e)
        {
            
        }

        internal void SetVinNumber(string vinnumber)
        {
            textEdit1.Text = vinnumber;
            DecodeVIN();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}