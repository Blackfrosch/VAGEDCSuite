using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Globalization;
using System.Threading;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Interop.Excel;

namespace VAGSuite
{
    class ExcelInterface
    {
        private Microsoft.Office.Interop.Excel.Application xla;

        private byte[] TurnMapUpsideDown(byte[] mapdata, int numcolumns, int numrows, bool issixteenbit)
        {
            byte[] mapdatanew = new byte[mapdata.Length];
            if (issixteenbit) numcolumns *= 2;
            int internal_rows = mapdata.Length / numcolumns;
            for (int tel = 0; tel < internal_rows; tel++)
            {
                for (int ctel = 0; ctel < numcolumns; ctel++)
                {
                    int orgoffset = (((internal_rows - 1) - tel) * numcolumns) + ctel;
                    mapdatanew.SetValue(mapdata.GetValue(orgoffset), (tel * numcolumns) + ctel);
                }
            }
            return mapdatanew;
        }

        public void ExportToExcel(string mapname, int address, int length, byte[] mapdata, int cols, int rows, bool isSixteenbit, int[] xaxisvalues, int[] yaxisvalues, bool isupsidedown, string xaxisdescr, string yaxisdescr, string zaxisdescr)
        {
            //en-US
            CultureInfo tci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = tci;

            try
            {
                try
                {
                    if (xla == null)
                    {
                        xla = new Microsoft.Office.Interop.Excel.Application();
                    }
                }
                catch (Exception xlaE)
                {
                    Console.WriteLine("Failed to create office application interface: " + xlaE.Message);
                }

                // turn mapdata upside down
                if (isupsidedown)
                {
                    mapdata = TurnMapUpsideDown(mapdata, cols, rows, isSixteenbit);
                }

                xla.Visible = true;
                Microsoft.Office.Interop.Excel.Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xla.ActiveSheet;
                ws.Name = "symboldata";

                // Now create the chart.
                ChartObjects chartObjs = (ChartObjects)ws.ChartObjects(Type.Missing);
                ChartObject chartObj = chartObjs.Add(100, 400, 400, 300);
                Microsoft.Office.Interop.Excel.Chart xlChart = chartObj.Chart;

                int nRows = rows;
                //if (isSixteenbit) nRows /= 2;
                int nColumns = cols;
                string upperLeftCell = "B3";
                int endRowNumber = System.Int32.Parse(upperLeftCell.Substring(1)) + nRows - 1;
                char endColumnLetter = System.Convert.ToChar(Convert.ToInt32(upperLeftCell[0]) + nColumns - 1);
                string upperRightCell = System.String.Format("{0}{1}", endColumnLetter, System.Int32.Parse(upperLeftCell.Substring(1)));
                string lowerRightCell = System.String.Format("{0}{1}", endColumnLetter, endRowNumber);
                // Send single dimensional array to Excel:

                Range rg1 = ws.get_Range("B2", "Z2");
                double[] xarray = new double[nColumns];
                double[] yarray = new double[nRows];
                ws.Cells[1, 1] = "Data for " + mapname;
                for (int i = 0; i < xarray.Length; i++)
                {
                    if (xaxisvalues.Length > i)
                    {
                        xarray[i] = (int)xaxisvalues.GetValue(i);
                    }
                    else
                    {
                        xarray[i] = i;
                    }
                    //ws.Cells[i + 3, 1] = xarray[i];
                    ws.Cells[2, 2 + i] = xarray[i];
                }
                for (int i = 0; i < yarray.Length; i++)
                {
                    if (yaxisvalues.Length > i)
                    {
                        if (isupsidedown)
                        {
                            yarray[i] = (int)yaxisvalues.GetValue((yarray.Length - 1) - i);
                        }
                        else
                        {
                            yarray[i] = (int)yaxisvalues.GetValue(i);
                        }
                    }
                    else
                    {
                        yarray[i] = i;
                    }
                    ws.Cells[i + 3, 1] = yarray[i];
                    //ws.Cells[2, 2 + i] = yarray[i];
                }


                Range rg = ws.get_Range(upperLeftCell, lowerRightCell);
                rg.Value2 = AddData(nRows, nColumns, mapdata, isSixteenbit);

                Range chartRange = ws.get_Range("A2", lowerRightCell);

                xlChart.SetSourceData(chartRange, Type.Missing);
                if (yarray.Length > 1)
                {
                    xlChart.ChartType = XlChartType.xlSurface;
                }

                // Customize axes:
                Axis xAxis = (Axis)xlChart.Axes(XlAxisType.xlCategory,
                    XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = yaxisdescr;
                try
                {
                    Axis yAxis = (Axis)xlChart.Axes(XlAxisType.xlSeriesAxis,
                        XlAxisGroup.xlPrimary);
                    yAxis.HasTitle = true;
                    yAxis.AxisTitle.Text = xaxisdescr;
                }
                catch (Exception E)
                {
                    Console.WriteLine("Failed to set y axis: " + E.Message);
                }


                Axis zAxis = (Axis)xlChart.Axes(XlAxisType.xlValue,
                    XlAxisGroup.xlPrimary);
                zAxis.HasTitle = true;
                zAxis.AxisTitle.Text = zaxisdescr;

                // Add title:
                xlChart.HasTitle = true;

                xlChart.ChartTitle.Text = mapname;

                // Remove legend:
                xlChart.HasLegend = false;
                // add 3d shade
                xlChart.SurfaceGroup.Has3DShading = true;
                /*if (File.Exists(m_currentfile + "~" + mapname + ".xls"))
                {

                }*/
                try
                {
                    wb.SaveAs(Tools.Instance.m_currentfile + "~" + mapname + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, false, null, null, null, null);
                }
                catch (Exception sE)
                {
                    Console.WriteLine("Failed to save workbook: " + sE.Message);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to export to excel: " + E.Message);
                Console.WriteLine("Failed to export to excel: " + E.Message);
            }
            tci = new CultureInfo("nl-NL");
            Thread.CurrentThread.CurrentCulture = tci;

        }

        private double[,] AddData(int nRows, int nColumns, byte[] mapdata, bool isSixteenbit)
        {
            double[,] dataArray = new double[nRows, nColumns];
            double[] xarray = new double[nColumns];
            for (int i = 0; i < xarray.Length; i++)
            {
                xarray[i] = -3.0f + i * 0.25f;
            }
            double[] yarray = xarray;

            int mapindex = 0;
            for (int i = 0; i < dataArray.GetLength(0); i++)
            {
                for (int j = 0; j < dataArray.GetLength(1); j++)
                {
                    if (isSixteenbit)
                    {
                        byte val1 = (byte)mapdata.GetValue(mapindex++);
                        byte val2 = (byte)mapdata.GetValue(mapindex++);
                        bool convertSign = false;
                        if (val1 == 0xff)
                        {
                            val1 = 0;
                            val2 = (byte)(0x100 - val2);
                            convertSign = true;
                        }
                        int ival1 = Convert.ToInt32(val1);
                        int ival2 = Convert.ToInt32(val2);
                        double value = (ival1 * 256) + ival2;
                        if (convertSign) value = -value;
                        dataArray[i, j] = value;
                    }
                    else
                    {
                        byte val1 = (byte)mapdata.GetValue(mapindex++);
                        int ival1 = Convert.ToInt32(val1);

                        double value = ival1;
                        dataArray[i, j] = value;
                    }
                }
            }
            return dataArray;
        }

        public System.Data.DataTable getDataFromXLS(string strFilePath)
        {
            try
            {
                string strConnectionString = string.Empty;
                strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + @";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
                //MessageBox.Show(strConnectionString);
                OleDbConnection cnCSV = new OleDbConnection(strConnectionString);
                cnCSV.Open();
                OleDbCommand cmdSelect = new OleDbCommand(@"SELECT * FROM [symboldata$]", cnCSV);
                OleDbDataAdapter daCSV = new OleDbDataAdapter();
                daCSV.SelectCommand = cmdSelect;
                System.Data.DataTable dtCSV = new System.Data.DataTable();
                daCSV.Fill(dtCSV);
                cnCSV.Close();
                daCSV = null;
                return dtCSV;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { }
        }

        

        

        public System.Data.DataTable getDataFromXLSSymbolHelper(string strFilePath)
        {
            try
            {
                string strConnectionString = string.Empty;
                strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + @";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
                OleDbConnection cnCSV = new OleDbConnection(strConnectionString);
                cnCSV.Open();
                OleDbCommand cmdSelect = new OleDbCommand(@"SELECT * FROM [Symbols$]", cnCSV);
                OleDbDataAdapter daCSV = new OleDbDataAdapter();
                daCSV.SelectCommand = cmdSelect;
                System.Data.DataTable dtCSV = new System.Data.DataTable();
                daCSV.Fill(dtCSV);
                cnCSV.Close();
                daCSV = null;
                return dtCSV;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { }
        }
    }
}
