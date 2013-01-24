namespace VAGSuite
{
    partial class frmDecodeVIN
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblCarMake = new DevExpress.XtraEditors.LabelControl();
            this.lblCarModel = new DevExpress.XtraEditors.LabelControl();
            this.lblEngineType = new DevExpress.XtraEditors.LabelControl();
            this.lblMakeyear = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.lblPlant = new DevExpress.XtraEditors.LabelControl();
            this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
            this.lblChassis = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(83, 12);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(302, 20);
            this.textEdit1.TabIndex = 0;
            this.textEdit1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textEdit1_KeyDown);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(401, 9);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Decode";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(9, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(56, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "VIN number";
            // 
            // lblCarMake
            // 
            this.lblCarMake.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCarMake.Location = new System.Drawing.Point(196, 47);
            this.lblCarMake.Name = "lblCarMake";
            this.lblCarMake.Size = new System.Drawing.Size(12, 13);
            this.lblCarMake.TabIndex = 3;
            this.lblCarMake.Text = "---";
            // 
            // lblCarModel
            // 
            this.lblCarModel.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblCarModel.Location = new System.Drawing.Point(196, 66);
            this.lblCarModel.Name = "lblCarModel";
            this.lblCarModel.Size = new System.Drawing.Size(12, 13);
            this.lblCarModel.TabIndex = 4;
            this.lblCarModel.Text = "---";
            // 
            // lblEngineType
            // 
            this.lblEngineType.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblEngineType.Location = new System.Drawing.Point(196, 142);
            this.lblEngineType.Name = "lblEngineType";
            this.lblEngineType.Size = new System.Drawing.Size(12, 13);
            this.lblEngineType.TabIndex = 5;
            this.lblEngineType.Text = "---";
            // 
            // lblMakeyear
            // 
            this.lblMakeyear.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblMakeyear.Location = new System.Drawing.Point(196, 104);
            this.lblMakeyear.Name = "lblMakeyear";
            this.lblMakeyear.Size = new System.Drawing.Size(12, 13);
            this.lblMakeyear.TabIndex = 6;
            this.lblMakeyear.Text = "---";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(83, 104);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(47, 13);
            this.labelControl6.TabIndex = 10;
            this.labelControl6.Text = "Makeyear";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(83, 142);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(57, 13);
            this.labelControl7.TabIndex = 9;
            this.labelControl7.Text = "Engine type";
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(83, 66);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(28, 13);
            this.labelControl8.TabIndex = 8;
            this.labelControl8.Text = "Model";
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(83, 47);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(25, 13);
            this.labelControl9.TabIndex = 7;
            this.labelControl9.Text = "Make";
            // 
            // labelControl10
            // 
            this.labelControl10.Location = new System.Drawing.Point(83, 123);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(72, 13);
            this.labelControl10.TabIndex = 12;
            this.labelControl10.Text = "Assembly plant";
            // 
            // lblPlant
            // 
            this.lblPlant.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblPlant.Location = new System.Drawing.Point(196, 123);
            this.lblPlant.Name = "lblPlant";
            this.lblPlant.Size = new System.Drawing.Size(12, 13);
            this.lblPlant.TabIndex = 11;
            this.lblPlant.Text = "---";
            // 
            // labelControl12
            // 
            this.labelControl12.Location = new System.Drawing.Point(83, 85);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Size = new System.Drawing.Size(40, 13);
            this.labelControl12.TabIndex = 14;
            this.labelControl12.Text = "Platform";
            // 
            // lblChassis
            // 
            this.lblChassis.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblChassis.Location = new System.Drawing.Point(196, 85);
            this.lblChassis.Name = "lblChassis";
            this.lblChassis.Size = new System.Drawing.Size(12, 13);
            this.lblChassis.TabIndex = 13;
            this.lblChassis.Text = "---";
            // 
            // simpleButton2
            // 
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.simpleButton2.Location = new System.Drawing.Point(401, 214);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 17;
            this.simpleButton2.Text = "Close";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // frmDecodeVIN
            // 
            this.AcceptButton = this.simpleButton1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.simpleButton2;
            this.ClientSize = new System.Drawing.Size(488, 249);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.labelControl12);
            this.Controls.Add(this.lblChassis);
            this.Controls.Add(this.labelControl10);
            this.Controls.Add(this.lblPlant);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.labelControl9);
            this.Controls.Add(this.lblMakeyear);
            this.Controls.Add(this.lblEngineType);
            this.Controls.Add(this.lblCarModel);
            this.Controls.Add(this.lblCarMake);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.textEdit1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDecodeVIN";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VIN decoder";
            this.Load += new System.EventHandler(this.frmDecodeVIN_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblCarMake;
        private DevExpress.XtraEditors.LabelControl lblCarModel;
        private DevExpress.XtraEditors.LabelControl lblEngineType;
        private DevExpress.XtraEditors.LabelControl lblMakeyear;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.LabelControl lblPlant;
        private DevExpress.XtraEditors.LabelControl labelControl12;
        private DevExpress.XtraEditors.LabelControl lblChassis;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
    }
}