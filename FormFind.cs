using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Be.Windows.Forms;

namespace VAGSuite
{
	/// <summary>
	/// Summary description for FormFind.
	/// </summary>
    public class FormFind : DevExpress.XtraEditors.XtraForm
	{
		private Be.Windows.Forms.HexBox hexBox;
		private System.Windows.Forms.TextBox txtString;
		private System.Windows.Forms.RadioButton rbString;
		private System.Windows.Forms.RadioButton rbHex;
        private System.Windows.Forms.Label label1;
        private Button btnOK;
        private Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormFind()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			rbString.CheckedChanged += new EventHandler(rb_CheckedChanged);
			rbHex.CheckedChanged += new EventHandler(rb_CheckedChanged);

//			rbString.Enter += new EventHandler(rbString_Enter);
//			rbHex.Enter += new EventHandler(rbHex_Enter);

			hexBox.ByteProvider = new DynamicByteProvider(  new System.Collections.Generic.List<byte>() );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.hexBox = new Be.Windows.Forms.HexBox();
            this.txtString = new System.Windows.Forms.TextBox();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.rbHex = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // hexBox
            // 
            this.hexBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBox.Enabled = false;
            this.hexBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox.Location = new System.Drawing.Point(16, 104);
            this.hexBox.Name = "hexBox";
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.Size = new System.Drawing.Size(304, 88);
            this.hexBox.TabIndex = 3;
            // 
            // txtString
            // 
            this.txtString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtString.Location = new System.Drawing.Point(16, 56);
            this.txtString.Name = "txtString";
            this.txtString.Size = new System.Drawing.Size(304, 20);
            this.txtString.TabIndex = 1;
            // 
            // rbString
            // 
            this.rbString.Checked = true;
            this.rbString.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbString.Location = new System.Drawing.Point(16, 40);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(104, 16);
            this.rbString.TabIndex = 0;
            this.rbString.TabStop = true;
            this.rbString.Text = "String";
            // 
            // rbHex
            // 
            this.rbHex.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rbHex.Location = new System.Drawing.Point(16, 88);
            this.rbHex.Name = "rbHex";
            this.rbHex.Size = new System.Drawing.Size(104, 16);
            this.rbHex.TabIndex = 2;
            this.rbHex.Text = "Hex";
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Find";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(245, 211);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Find next";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(164, 211);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(48, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 8);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // FormFind
            // 
            this.AcceptButton = this.btnOK;
            this.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(336, 246);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbHex);
            this.Controls.Add(this.rbString);
            this.Controls.Add(this.txtString);
            this.Controls.Add(this.hexBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFind";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find";
            this.Activated += new System.EventHandler(this.FormFind_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public byte[] GetFindBytes()
		{
			if(rbString.Checked)
			{
				byte[] res = System.Text.ASCIIEncoding.ASCII.GetBytes(txtString.Text);
				return res;
			}
			else
			{
				return CollectionToArray(((DynamicByteProvider)hexBox.ByteProvider).Bytes);
			}
		}

        private byte[] CollectionToArray(System.Collections.Generic.List<byte> list)
        {
            byte[] retval = new byte[list.Count];
            int i = 0;
            try
            {
                foreach (byte b in list)
                {
                    retval[i++] = b;
                }
            }
            catch (Exception)
            {
            }
            return retval;
        }

		private void rb_CheckedChanged(object sender, System.EventArgs e)
		{
			txtString.Enabled = rbString.Checked;
			hexBox.Enabled = !txtString.Enabled;

			if(txtString.Enabled)
				txtString.Focus();
			else
				hexBox.Focus();
		}

		private void rbString_Enter(object sender, EventArgs e)
		{
			txtString.Focus();
		}

		private void rbHex_Enter(object sender, EventArgs e)
		{
			hexBox.Focus();
		}

		private void FormFind_Activated(object sender, System.EventArgs e)
		{
			if(rbString.Checked)
				txtString.Focus();
			else
				hexBox.Focus();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if(rbString.Checked && txtString.Text.Length == 0)
				DialogResult = DialogResult.Cancel;
			else if(rbHex.Checked && hexBox.ByteProvider.Length == 0)
				DialogResult = DialogResult.Cancel;
			else
				DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
