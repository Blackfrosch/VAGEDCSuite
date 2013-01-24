namespace VAGSuite
{
    partial class frmTransactionLog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTransactionLog));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rolllBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rollForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcTimeStamp = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSymbolname = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSymbolLength = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcIsRolledBack = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl1.Controls.Add(this.gridControl1);
            this.groupControl1.Location = new System.Drawing.Point(13, 10);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(519, 361);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Logged transactions";
            // 
            // gridControl1
            // 
            this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 21);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(515, 338);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rolllBackToolStripMenuItem,
            this.rollForwardToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 70);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // rolllBackToolStripMenuItem
            // 
            this.rolllBackToolStripMenuItem.Name = "rolllBackToolStripMenuItem";
            this.rolllBackToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rolllBackToolStripMenuItem.Text = "Roll back";
            this.rolllBackToolStripMenuItem.Click += new System.EventHandler(this.rolllBackToolStripMenuItem_Click);
            // 
            // rollForwardToolStripMenuItem
            // 
            this.rollForwardToolStripMenuItem.Name = "rollForwardToolStripMenuItem";
            this.rollForwardToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rollForwardToolStripMenuItem.Text = "Roll forward";
            this.rollForwardToolStripMenuItem.Click += new System.EventHandler(this.rollForwardToolStripMenuItem_Click);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcTimeStamp,
            this.gcSymbolname,
            this.gcAddress,
            this.gcSymbolLength,
            this.gcIsRolledBack,
            this.gcNote});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcTimeStamp, DevExpress.Data.ColumnSortOrder.Descending)});
            this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            this.gridView1.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridView1_CustomDrawCell);
            // 
            // gcTimeStamp
            // 
            this.gcTimeStamp.Caption = "Timestamp";
            this.gcTimeStamp.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.gcTimeStamp.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gcTimeStamp.FieldName = "EntryDateTime";
            this.gcTimeStamp.Name = "gcTimeStamp";
            this.gcTimeStamp.OptionsColumn.AllowEdit = false;
            this.gcTimeStamp.Visible = true;
            this.gcTimeStamp.VisibleIndex = 0;
            // 
            // gcSymbolname
            // 
            this.gcSymbolname.Caption = "Symbol";
            this.gcSymbolname.FieldName = "SymbolName";
            this.gcSymbolname.Name = "gcSymbolname";
            this.gcSymbolname.OptionsColumn.AllowEdit = false;
            this.gcSymbolname.Visible = true;
            this.gcSymbolname.VisibleIndex = 1;
            // 
            // gcAddress
            // 
            this.gcAddress.Caption = "Address";
            this.gcAddress.FieldName = "SymbolAddress";
            this.gcAddress.Name = "gcAddress";
            this.gcAddress.OptionsColumn.AllowEdit = false;
            this.gcAddress.Visible = true;
            this.gcAddress.VisibleIndex = 3;
            // 
            // gcSymbolLength
            // 
            this.gcSymbolLength.Caption = "Length";
            this.gcSymbolLength.FieldName = "SymbolLength";
            this.gcSymbolLength.Name = "gcSymbolLength";
            this.gcSymbolLength.OptionsColumn.AllowEdit = false;
            this.gcSymbolLength.Visible = true;
            this.gcSymbolLength.VisibleIndex = 4;
            // 
            // gcIsRolledBack
            // 
            this.gcIsRolledBack.Caption = "Rolled back";
            this.gcIsRolledBack.FieldName = "IsRolledBack";
            this.gcIsRolledBack.Name = "gcIsRolledBack";
            this.gcIsRolledBack.OptionsColumn.AllowEdit = false;
            this.gcIsRolledBack.Visible = true;
            this.gcIsRolledBack.VisibleIndex = 5;
            // 
            // gcNote
            // 
            this.gcNote.Caption = "Note";
            this.gcNote.FieldName = "Note";
            this.gcNote.Name = "gcNote";
            this.gcNote.Visible = true;
            this.gcNote.VisibleIndex = 2;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.Location = new System.Drawing.Point(455, 377);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton2.Location = new System.Drawing.Point(374, 377);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "Details";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton3
            // 
            this.simpleButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.simpleButton3.Location = new System.Drawing.Point(13, 377);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(85, 23);
            this.simpleButton3.TabIndex = 3;
            this.simpleButton3.Text = "Roll back";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // simpleButton4
            // 
            this.simpleButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.simpleButton4.Location = new System.Drawing.Point(104, 377);
            this.simpleButton4.Name = "simpleButton4";
            this.simpleButton4.Size = new System.Drawing.Size(85, 23);
            this.simpleButton4.TabIndex = 4;
            this.simpleButton4.Text = "Roll forward";
            this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
            // 
            // frmTransactionLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 412);
            this.Controls.Add(this.simpleButton4);
            this.Controls.Add(this.simpleButton3);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.groupControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTransactionLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transaction log";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.frmTransactionLog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraGrid.Columns.GridColumn gcTimeStamp;
        private DevExpress.XtraGrid.Columns.GridColumn gcSymbolname;
        private DevExpress.XtraGrid.Columns.GridColumn gcAddress;
        private DevExpress.XtraGrid.Columns.GridColumn gcSymbolLength;
        private DevExpress.XtraGrid.Columns.GridColumn gcIsRolledBack;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem rolllBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rollForwardToolStripMenuItem;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraEditors.SimpleButton simpleButton4;
        private DevExpress.XtraGrid.Columns.GridColumn gcNote;
    }
}