using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VAGSuite
{
    public class ProfColorTable : ProfessionalColorTable
    {


        private Color m_CustomToolstripGradientMiddle;

        public Color CustomToolstripGradientMiddle
        {
            get { return m_CustomToolstripGradientMiddle; }
            set { m_CustomToolstripGradientMiddle = value; }
        }

        private Color m_CustomToolstripGradientEnd;

        public Color CustomToolstripGradientEnd
        {
            get { return m_CustomToolstripGradientEnd; }
            set { m_CustomToolstripGradientEnd = value; }
        }

        private Color m_CustomToolstripGradientBegin;

        public Color CustomToolstripGradientBegin
        {
            get { return m_CustomToolstripGradientBegin; }
            set { m_CustomToolstripGradientBegin = value; }
        }

        public override System.Drawing.Color ToolStripGradientBegin
        {
            get
            {
                return m_CustomToolstripGradientBegin;
            }
        }

        public override Color ToolStripGradientEnd
        {
            get
            {
                return m_CustomToolstripGradientEnd;
            }
        }

        public override Color ToolStripGradientMiddle
        {
            get
            {
                return m_CustomToolstripGradientMiddle;
            }
        }

    }
}
