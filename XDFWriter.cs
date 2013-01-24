using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VAGSuite
{
    class XDFWriter
    {
        private StreamWriter sw;
        private Int32 ConstantID = 1;
        public void CreateXDF(string filename, string flashfilename, int dataend, int filesize)
        {
            sw = new StreamWriter(filename, false);
            // write header information for XDF file
            sw.WriteLine("XDF");
            sw.WriteLine("1.110000");
            sw.WriteLine("");
            sw.WriteLine("DO NOT HAND EDIT!!!! (Trust me)");
            sw.WriteLine("");
            sw.WriteLine("%%HEADER%%");
            sw.WriteLine("001000 FileVers         =\"1.0\"");
            sw.WriteLine("001005 DefTitle         =\"" + flashfilename + "\"");
            sw.WriteLine("001007 DescSize         =0x1A");
            sw.WriteLine("001006 Desc             =\"EDC15P XDF by Dilemma\"");
            sw.WriteLine("001010 Author           =\"Dilemma\"");
            sw.WriteLine("001030 BinSize          =" + filesize.ToString("X6"));
            sw.WriteLine("001035 BaseOffset       =0");
            sw.WriteLine("001200 ADSAssoc         =\"\"");
            sw.WriteLine("001225 ADSCheck         =0x00");
            sw.WriteLine("001300 GenFlags         =0x0");
            sw.WriteLine("001325 ModeFlags        =0x0");
            sw.WriteLine("002000 Category0        =\"Fuel\"");
            sw.WriteLine("002001 Category1        =\"Ignition\"");
            sw.WriteLine("002002 Category2        =\"Turbo\"");
            sw.WriteLine("002003 Category3        =\"Idle\"");
            sw.WriteLine("%%END%%");
            /*sw.WriteLine("");
            sw.WriteLine("%%CHECKSUM%%");
            sw.WriteLine("000002 UniqueID         =0x1");
            sw.WriteLine("010005 Title            =\"Imported Checksum\"");
            sw.WriteLine("010010 DataStart        =0x0");
            sw.WriteLine("010015 DataEnd          =0x" + dataend.ToString("X6"));
            sw.WriteLine("010020 SizeBits         =0x20");
            sw.WriteLine("010025 StoreAddr        =0x3FFFC");
            sw.WriteLine("010030 CalcMethod       =0x0");
            sw.WriteLine("010050 Flags            =0x1");
            sw.WriteLine("%%END%%");*/
        }

        public void AddTable(string name, string description, XDFCategories category, string xunits, string yunits, string zunits, int rows, int columns, int address, bool issixteenbit, int xaxisaddress, int yaxisaddress, bool isxaxissixteenbit, bool isyaxissixteenbit, float x_correctionfactor, float y_correctionfactor, float z_correctionfactor)
        {
            if (sw != null)
            {
                if (name.StartsWith("Driver wish"))
                {
                    bool breakme = true;
                }
                if (description == string.Empty) description = name;
                ConstantID++;
                sw.WriteLine("%%TABLE%%");
                sw.WriteLine("000002 UniqueID         =0x" + ConstantID.ToString("X4"));
                sw.WriteLine("000100 Cat0ID           =0x" + ((int)category).ToString("X2"));
                sw.WriteLine("040005 Title            =\"" + name + "\"");
                sw.WriteLine("040011 DescSize         =0x" + ((int)(name.Length + 1)).ToString("X2"));
                sw.WriteLine("040010 Desc             =\"" + description + "\"");
                sw.WriteLine("040050 SizeInBits       =0x10");
                sw.WriteLine("040100 Address          =0x" + address.ToString("X6"));

                sw.WriteLine("040150 Flags            =0x1"); // 30?
                sw.WriteLine("040200 ZEq              =(X*" + z_correctionfactor.ToString("F1").Replace(",", ".") + ")/10,TH|0|0|0|0|");
                sw.WriteLine("040203 XOutType         =0x4"); // 4?
                sw.WriteLine("040304 YOutType         =0x4"); // 4?
                sw.WriteLine("040205 OutType          =0x3");
                sw.WriteLine("040230 RangeLow         =0.0000");
                sw.WriteLine("040240 RangeHigh        =65535.0000");
                //rows /= 2;
                sw.WriteLine("040300 Rows             =0x" + rows.ToString("X2"));
                sw.WriteLine("040305 Cols             =0x" + columns.ToString("X2"));
                sw.WriteLine("040320 XUnits           =\"" + xunits + "\"");
                sw.WriteLine("040325 YUnits           =\"" + yunits + "\"");
                sw.WriteLine("040330 ZUnits           =\"" + zunits + "\"");
                if (xaxisaddress != 0)
                {
                    string strxlabel = "040350 XLabels          =";
                    for (int ix = 0; ix < columns; ix++) strxlabel += "00,";
                    if (strxlabel.EndsWith(",")) strxlabel = strxlabel.Substring(0, strxlabel.Length - 1);
                    sw.WriteLine(strxlabel);
                }
                else
                {
                    sw.WriteLine("040350 XLabels          =%");
                }
                sw.WriteLine("040352 XLabelType       =0x4"); // 4?
                sw.WriteLine("040354 XEq              =(X*" + x_correctionfactor.ToString("F1").Replace(",", ".") + "),TH|0|0|0|0|");
                string strylabel = "040360 YLabels          =  ";
                for (int ix = 0; ix < columns; ix++) 
                {
                    int val = ix*10;
                    strylabel += val.ToString("D2") + ",";
                }
                if (strylabel.EndsWith(",")) strylabel = strylabel.Substring(0, strylabel.Length - 1);
                sw.WriteLine(strylabel);
                if (xaxisaddress != 0 && yaxisaddress != 0)
                {
                    sw.WriteLine("040362 YLabelType       =0x4");
                    sw.WriteLine("040364 YEq              =(X*" + y_correctionfactor.ToString("F1").Replace(",", ".") + "),TH|0|0|0|0|");
                    sw.WriteLine("040505 XLabelSource     =0x1"); // in binary
                    sw.WriteLine("040515 YLabelSource     =0x1");
                    sw.WriteLine("040600 XAddress         =0x" + xaxisaddress.ToString("X6"));
                    if (isxaxissixteenbit)
                    {
                        sw.WriteLine("040620 XAddrStep        =2");
                    }
                    else
                    {
                        sw.WriteLine("040610 XDataSize        =0x1");
                        sw.WriteLine("040620 XAddrStep        =1");
                    }
                }
                else
                {
                    sw.WriteLine("040362 YLabelType       =0x4"); // manual
                    sw.WriteLine("040364 YEq              =(X*" + y_correctionfactor.ToString("F1").Replace(",", ".") + "),TH|0|0|0|0|");
                }
                sw.WriteLine("040660 XAxisMin         =1000.000000");
                sw.WriteLine("040670 XAxisMax         =1000.000000");
                if (xaxisaddress != 0 && yaxisaddress != 0)
                {
                    sw.WriteLine("040700 YAddress         =0x" + yaxisaddress.ToString("X6"));
                    if (isyaxissixteenbit)
                    {
                        sw.WriteLine("040720 YAddrStep        =2");
                    }
                }
                sw.WriteLine("040760 YAxisMin         =1000.000000");
                sw.WriteLine("040770 YAxisMax         =1000.000000");
                sw.WriteLine("%%END%%");
            }
        }

        public void AddConstant(object value, string name, XDFCategories category, string units, int size, int address, bool issixteenbit)
        {
            if (sw != null)
            {
                ConstantID++;
                sw.WriteLine("%%CONSTANT%%");
                sw.WriteLine("000002 UniqueID         =0x" + ConstantID.ToString("X4"));
                sw.WriteLine("000100 Cat0ID           =0x" + ((int)category).ToString("X2"));
                sw.WriteLine("020005 Title            =\"" + name + "\"");
                sw.WriteLine("020011 DescSize         =0x1");
                sw.WriteLine("020010 Desc             =\"\"");
                sw.WriteLine("020020 Units            =\"" + units + "\"");
                if (issixteenbit)
                {
                    sw.WriteLine("020050 SizeInBits       =0x10");
                }
                sw.WriteLine("020100 Address          =0x" + address.ToString("X6"));
                sw.WriteLine("020200 Equation         =TH|0|0|0|0|");
                sw.WriteLine("%%END%%");
            }
        }

        public void AddFlag(string title, int address, int bitnumber)
        {
            if (sw != null)
            {
                ConstantID++;
                sw.WriteLine("%%FLAG%%");
                sw.WriteLine("000002 UniqueID         =0x" + ConstantID.ToString("X4"));
                sw.WriteLine("030005 Title            =\"" + title + "\"");
                sw.WriteLine("030011 DescSize         =0x13");
                sw.WriteLine("030010 Desc             =\"Enable\\disable VSS\"");
                sw.WriteLine("030100 Address          =0x" + address.ToString("X6"));
                sw.WriteLine("030200 BitNumber        =0x" + bitnumber.ToString("X2"));
                sw.WriteLine("%%END%%");
            }
        }

        public void CloseFile()
        {
            sw.Close();
        }



    }
}
