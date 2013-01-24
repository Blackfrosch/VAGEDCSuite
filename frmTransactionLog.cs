using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;


namespace VAGSuite
{
    public partial class frmTransactionLog : DevExpress.XtraEditors.XtraForm
    {
        public delegate void RollBack(object sender, RollInformationEventArgs e);
        public event frmTransactionLog.RollBack onRollBack;
        public delegate void RollForward(object sender, RollInformationEventArgs e);
        public event frmTransactionLog.RollForward onRollForward;
        public delegate void NoteChanged(object sender, RollInformationEventArgs e);
        public event frmTransactionLog.NoteChanged onNoteChanged;

        public class RollInformationEventArgs : System.EventArgs
        {
            private TransactionEntry _entry;

            public TransactionEntry Entry
            {
                get { return _entry; }
                set { _entry = value; }
            }

            
            public RollInformationEventArgs(TransactionEntry entry)
            {
                this._entry = entry;
            }
        }

        public frmTransactionLog()
        {
            InitializeComponent();
        }

        public void SetTransactionLog(TransactionLog log)
        {
            gridControl1.DataSource = log.TransCollection; // should be based on symbolname
            // select the last entry and scroll to it
            gridView1.BestFitColumns();
            UpdateButtons();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // get selected row and decide what to show/not to show
            Point p = gridControl1.PointToClient(Cursor.Position);
            GridHitInfo hitinfo = gridView1.CalcHitInfo(p);
            int[] selectedrows = gridView1.GetSelectedRows();
            if (hitinfo.InRow)
            {
                int grouplevel = gridView1.GetRowLevel((int)selectedrows.GetValue(0));
                if (grouplevel >= gridView1.GroupCount)
                {
                    //Console.WriteLine("In row");
                    if (gridView1.GetFocusedRow() is TransactionEntry)
                    {
                        TransactionEntry sh = (TransactionEntry)gridView1.GetFocusedRow();
                        if (sh.IsRolledBack)
                        {
                            rollForwardToolStripMenuItem.Enabled = true;
                            rolllBackToolStripMenuItem.Enabled = false;
                        }
                        else
                        {
                            rollForwardToolStripMenuItem.Enabled = false;
                            rolllBackToolStripMenuItem.Enabled = true;
                        }
                    }
                }
            }
        }

        private void CastRollBackEvent(TransactionEntry entry)
        {
            if (onRollBack != null)
            {
                onRollBack(this, new RollInformationEventArgs(entry));
            }
        }

        private void CastNoteChangedEvent(TransactionEntry entry)
        {
            if (onNoteChanged != null)
            {
                onNoteChanged(this, new RollInformationEventArgs(entry));
            }
        }

        private void CastRollForwardEvent(TransactionEntry entry)
        {
            if (onRollForward != null)
            {
                onRollForward(this, new RollInformationEventArgs(entry));
            }

        }

        private void rolllBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] selectedrows = gridView1.GetSelectedRows();
            int grouplevel = gridView1.GetRowLevel((int)selectedrows.GetValue(0));
            if (grouplevel >= gridView1.GroupCount)
            {
                //Console.WriteLine("In row");
                if (gridView1.GetFocusedRow() is TransactionEntry)
                {
                    TransactionEntry sh = (TransactionEntry)gridView1.GetFocusedRow();
                    CastRollBackEvent(sh);
                }

            }

            // cast rollback event to the main form
            // and reload the list
        }

        private void rollForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // cast rollforward event to the main form
            // and reload the list
            int[] selectedrows = gridView1.GetSelectedRows();
            int grouplevel = gridView1.GetRowLevel((int)selectedrows.GetValue(0));
            if (grouplevel >= gridView1.GroupCount)
            {
                //Console.WriteLine("In row");
                if (gridView1.GetFocusedRow() is TransactionEntry)
                {
                    TransactionEntry sh = (TransactionEntry)gridView1.GetFocusedRow();
                    CastRollForwardEvent(sh);
                }

            }

        }

        private void UpdateButtons()
        {

            int[] selectedrows = gridView1.GetSelectedRows();
            if (selectedrows.Length == 0)
            {
                simpleButton3.Enabled = false;
                simpleButton4.Enabled = false;
            }
            else
            {
                if (gridView1.GetFocusedRow() is TransactionEntry)
                {
                    TransactionEntry sh = (TransactionEntry)gridView1.GetFocusedRow();
                    if (sh.IsRolledBack)
                    {
                        simpleButton4.Enabled = true;
                        simpleButton3.Enabled = false;
                    }
                    else
                    {
                        simpleButton4.Enabled = false;
                        simpleButton3.Enabled = true;
                    }
                }
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // highlight the rollback column?
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateButtons();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            // cast rollback
            if (gridView1.GetFocusedRow() is TransactionEntry)
            {
                TransactionEntry sh = (TransactionEntry)gridView1.GetFocusedRow();
                CastRollBackEvent(sh);
                // move the selected cursor one row up, if possible
                gridView1.FocusedRowHandle--;
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            // cast roll forward
            if (gridView1.GetFocusedRow() is TransactionEntry)
            {
                TransactionEntry sh = (TransactionEntry)gridView1.GetFocusedRow();
                CastRollForwardEvent(sh);
                gridView1.FocusedRowHandle++;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            // show the details for this transaction (including data, meaning 2 mapviewers showing the details)
            frmInfoBox info = new frmInfoBox("Still needs to be implemented");
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            // note was edited?
            if (e.Column.Name == gcNote.Name)
            {
                // save the transaction log again (and reload?)
                TransactionEntry sh = (TransactionEntry)gridView1.GetRow(e.RowHandle);
                //MessageBox.Show("You changed the note into: " + sh.Note);
                CastNoteChangedEvent(sh);
            }
        }

        private void frmTransactionLog_Shown(object sender, EventArgs e)
        {
           /* try
            {
                if (gridView1.RowCount > 0)
                {
                    gridView1.FocusedRowHandle = gridView1.RowCount - 1;
                    gridView1.MakeRowVisible(gridView1.FocusedRowHandle, false);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }*/

        }
    }
}