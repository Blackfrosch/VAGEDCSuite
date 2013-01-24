using System;
using System.Collections.Generic;
using System.Text;

namespace VAGSuite
{
    public class SymbolHelper
    {
		internal bool Is1D = false;
		internal bool Is2D = false;
		internal bool Is3D = false;

        private string m_xaxisUnits = string.Empty;

        public string XaxisUnits
        {
            get { return m_xaxisUnits; }
            set { m_xaxisUnits = value; }
        }
        private string m_yaxisUnits = string.Empty;

        public string YaxisUnits
        {
            get { return m_yaxisUnits; }
            set { m_yaxisUnits = value; }
        }

        private MapSelector _mapSelector = new MapSelector();

        internal MapSelector MapSelector
        {
            get { return _mapSelector; }
            set { _mapSelector = value; }
        }

        private int _bitMask = 0x00000;

        public int BitMask
        {
            get { return _bitMask; }
            set { _bitMask = value; }
        }

        System.Drawing.Color _color = System.Drawing.Color.Black;

        public System.Drawing.Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private byte[] _currentdata;

        public byte[] Currentdata
        {
            get { return _currentdata; }
            set { _currentdata = value; }
        }

        int symbol_number = 0;

        public int Symbol_number
        {
            get { return symbol_number; }
            set { symbol_number = value; }
        }

        bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        int symbol_type = 0;

        public int Symbol_type
        {
            get { return symbol_type; }
            set { symbol_type = value; }
        }


        int internal_address = 0x00000;

        public int Internal_address
        {
            get { return internal_address; }
            set { internal_address = value; }
        }


        Int64 start_address = 0x00000;

        Int64 flash_start_address = 0x00000;

        public Int64 Flash_start_address
        {
            get { return flash_start_address; }
            set { flash_start_address = value; }
        }

        int symbol_number_ECU = 0;

        public int Symbol_number_ECU
        {
            get { return symbol_number_ECU; }
            set { symbol_number_ECU = value; }
        }

        public Int64 Start_address
        {
            get { return start_address; }
            set { start_address = value; }
        }
        int length = 0x00;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        string _userdescription = string.Empty;

        public string Userdescription
        {
            get { return _userdescription; }
            set { _userdescription = value; }
        }

        string varname = string.Empty;

        public string Varname
        {
            get { return varname; }
            set { varname = value; }
        }

        string _description = string.Empty;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        string _category = "Unknown maps";

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
        string _subcategory = "Unknown";

        public string Subcategory
        {
            get { return _subcategory; }
            set { _subcategory = value; }
        }


        private int _x_axis_length = 1;

        public int X_axis_length
        {
            get { return _x_axis_length; }
            set { _x_axis_length = value; }
        }
        private int _y_axis_length = 1;

        public int Y_axis_length
        {
            get { return _y_axis_length; }
            set { _y_axis_length = value; }
        }

        private int _x_axis_ID = 0;

        public int X_axis_ID
        {
            get { return _x_axis_ID; }
            set { _x_axis_ID = value; }
        }
        private int _y_axis_ID = 0;

        public int Y_axis_ID
        {
            get { return _y_axis_ID; }
            set { _y_axis_ID = value; }
        }

        private int _x_axis_address = 0;

        public int X_axis_address
        {
            get { return _x_axis_address; }
            set { _x_axis_address = value; }
        }
        private int _y_axis_address = 0;

        public int Y_axis_address
        {
            get { return _y_axis_address; }
            set { _y_axis_address = value; }
        }

        private bool _xaxisAssigned = false;

        public bool XaxisAssigned
        {
            get { return _xaxisAssigned; }
            set { _xaxisAssigned = value; }
        }
        private bool _yaxisAssigned = false;

        public bool YaxisAssigned
        {
            get { return _yaxisAssigned; }
            set { _yaxisAssigned = value; }
        }

        private string _x_axis_descr = string.Empty;

        public string X_axis_descr
        {
            get { return _x_axis_descr; }
            set
            {
                _x_axis_descr = value;
                _xaxisAssigned = true;
            }
        }
        private string _y_axis_descr = string.Empty;

        public string Y_axis_descr
        {
            get { return _y_axis_descr; }
            set
            {
                _y_axis_descr = value;
                _yaxisAssigned = true;
            }
        }
        private string _z_axis_descr = string.Empty;

        public string Z_axis_descr
        {
            get { return _z_axis_descr; }
            set { _z_axis_descr = value; }
        }
        private double _x_axis_correction = 1;

        public double X_axis_correction
        {
            get { return _x_axis_correction; }
            set { _x_axis_correction = value; }
        }
        private double _x_axis_offset = 0;

        public double X_axis_offset
        {
            get { return _x_axis_offset; }
            set { _x_axis_offset = value; }
        }
        private double _y_axis_correction = 1;

        public double Y_axis_correction
        {
            get { return _y_axis_correction; }
            set { _y_axis_correction = value; }
        }
        private double _y_axis_offset = 0;

        public double Y_axis_offset
        {
            get { return _y_axis_offset; }
            set { _y_axis_offset = value; }
        }
        private double _correction = 1;

        public double Correction
        {
            get { return _correction; }
            set { _correction = value; }
        }
        private double _offset = 0;

        public double Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        private int _codeBlock = 0;

        public int CodeBlock
        {
            get { return _codeBlock; }
            set { _codeBlock = value; }
        }
    }
}
