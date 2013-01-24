using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace VAGSuite
{
    public partial class ctrlAxisEditor : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void ViewerClose(object sender, EventArgs e);
        public event ctrlAxisEditor.ViewerClose onClose;
        
        public delegate void DataSave(object sender, EventArgs e);
        public event ctrlAxisEditor.DataSave onSave;


        public ctrlAxisEditor()
        {
            InitializeComponent();
        }

        private int m_axisID = 0;

        public int AxisID
        {
            get { return m_axisID; }
            set { m_axisID = value; }
        }
        private int m_axisAddress = 0;

        public int AxisAddress
        {
            get { return m_axisAddress; }
            set { m_axisAddress = value; }
        }

        private float m_correctionFactor = 1;

        public float CorrectionFactor
        {
            get { return m_correctionFactor; }
            set { m_correctionFactor = value; }
        }

        private string m_fileName = string.Empty;

        public string FileName
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }

        private string m_Map_name = string.Empty;

        public string Map_name
        {
            get { return m_Map_name; }
            set { m_Map_name = value; }
        }

        public void SetData(float[] data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("VALUE");
            foreach (float f in data)
            {
                dt.Rows.Add(f.ToString("F3"));
            }
            gridControl1.DataSource = dt;
        }

        public float[] GetData()
        {
            float[] retval = new float[1];
            retval.SetValue(0, 0);
            DataTable dt = (DataTable)gridControl1.DataSource;
            if(dt != null)
            {
                retval = new float[dt.Rows.Count];
                int idx = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    retval.SetValue((float)Convert.ToDouble(dr["VALUE"].ToString()), idx++);
                }
            }
            return retval;
        }

        private void CastSaveEvent()
        {
            if (onSave != null)
            {
                onSave(this, EventArgs.Empty);
            }
        }

        private void CastCloseEvent()
        {
            if (onClose != null)
            {
                onClose(this, EventArgs.Empty);
            }
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // cast save event
            gridView1.CloseEditor();
            CastSaveEvent();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                float tesvalue = (float)Convert.ToDouble(e.Value);
            }
            catch (Exception E)
            {
                e.ErrorText = "Invalid input value";
                e.Valid = false;
            }
        }
    }
}
