using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vbAccelerator.Components.Controls
{
	/// <summary>
	/// A simple implementation of a Virtual ListBox.  A virtual ListBox
	/// contains no data, instead it just allocates space for a specified
	/// number of rows.  Whenever a row needs to be shown, the <see cref="OnDrawItem"/>
	/// method is fired which in turn fires the <see cref="DrawItem"/> event.
	/// </summary>
	public class VListBox : ListBox
	{
        [Category("Action")]
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_MOUSEWHEEL = 0x20A;
        public event ScrollEventHandler OnHorizontalScroll;
        public event ScrollEventHandler OnVerticalScroll;

        private const int SB_LINEUP = 0;
        private const int SB_LINELEFT = 0;
        private const int SB_LINEDOWN = 1;
        private const int SB_LINERIGHT = 1;
        private const int SB_PAGEUP = 2;
        private const int SB_PAGELEFT = 2;
        private const int SB_PAGEDOWN = 3;
        private const int SB_PAGERIGHT = 3;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_THUMBTRACK = 5;
        private const int SB_PAGETOP = 6;
        private const int SB_LEFT = 6;
        private const int SB_PAGEBOTTOM = 7;
        private const int SB_RIGHT = 7;
        private const int SB_ENDSCROLL = 8;
        private const int SIF_TRACKPOS = 0x10;
        private const int SIF_RANGE = 0x1;
        private const int SIF_POS = 0x4;
        private const int SIF_PAGE = 0x2;
        private const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetScrollInfo(
        IntPtr hWnd, int n, ref ScrollInfoStruct lpScrollInfo);

        private struct ScrollInfoStruct
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
            if (msg.Msg == WM_HSCROLL)
            {
                if (OnHorizontalScroll != null)
                {
                    ScrollInfoStruct si = new ScrollInfoStruct();
                    si.fMask = SIF_ALL;
                    si.cbSize = Marshal.SizeOf(si);
                    GetScrollInfo(msg.HWnd, 0, ref si);
                    if (msg.WParam.ToInt32() == SB_ENDSCROLL)
                    {
                        ScrollEventArgs sargs = new ScrollEventArgs(
                        ScrollEventType.EndScroll,
                        si.nPos);
                        OnHorizontalScroll(this, sargs);
                    }
                }
            }
            if (msg.Msg == WM_VSCROLL || msg.Msg == WM_MOUSEWHEEL)
            {
                if (OnVerticalScroll != null)
                {
                    ScrollInfoStruct si = new ScrollInfoStruct();
                    si.fMask = SIF_ALL;
                    si.cbSize = Marshal.SizeOf(si);
                    GetScrollInfo(msg.HWnd, 0, ref si);
                    if (msg.WParam.ToInt32() == SB_ENDSCROLL)
                    {
                        ScrollEventArgs sargs = new ScrollEventArgs(
                        ScrollEventType.EndScroll,
                        si.nPos);
                        OnVerticalScroll(this, sargs);
                    }
                }
            }
            base.WndProc(ref msg);
        }
 
		/*
		* Listbox Styles
		*/
		private const int LBS_NOTIFY            = 0x0001;
		private const int LBS_SORT              = 0x0002;
		private const int LBS_NOREDRAW          = 0x0004;
		private const int LBS_MULTIPLESEL       = 0x0008;
		private const int LBS_OWNERDRAWFIXED    = 0x0010;
		private const int LBS_OWNERDRAWVARIABLE = 0x0020;
		private const int LBS_HASSTRINGS        = 0x0040;
		private const int LBS_USETABSTOPS       = 0x0080;
		private const int LBS_NOINTEGRALHEIGHT  = 0x0100;
		private const int LBS_MULTICOLUMN       = 0x0200;
		private const int LBS_WANTKEYBOARDINPUT = 0x0400;
		private const int LBS_EXTENDEDSEL       = 0x0800;
		private const int LBS_DISABLENOSCROLL   = 0x1000;
		private const int LBS_NODATA            = 0x2000;

		private const int LB_GETCOUNT             = 0x018B;
		private const int LB_SETCOUNT             = 0x01A7;

		private const int LB_SETSEL               = 0x0185;
		private const int LB_SETCURSEL            = 0x0186;
		private const int LB_GETSEL               = 0x0187;
		private const int LB_GETCURSEL            = 0x0188;
		private const int LB_GETSELCOUNT          = 0x0190;
		private const int LB_GETSELITEMS          = 0x0191;

		[DllImport("user32", CharSet = CharSet.Auto)]
		private extern static int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

		#region Member Variables
		private int selectedIndex = -1;
		private VListBox.SelectedIndexCollection selectedIndices = null;
		#endregion


		/// <summary>
		/// Constructs a new instance of this class.
		/// </summary>
		public VListBox() : base()
		{
			selectedIndices = new VListBox.SelectedIndexCollection(this);
		}

		public void DefaultDrawItem(
			DrawItemEventArgs e,
			String text, Brush foreGround
			)
		{
			bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

			if (selected)
			{
				e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
			}
			else
			{
				e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
			}

			e.Graphics.DrawString(
				text, 
				this.Font, 
				//selected ? SystemBrushes.HighlightText : /*SystemBrushes.WindowText*/ foreGround,
                foreGround,
				new RectangleF(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2));
		}

		/// <summary>
		/// Gets or sets the number of virtual items in the ListBox.
		/// </summary>
		public int Count
		{
			get
			{
				return SendMessage(this.Handle, LB_GETCOUNT, 0, IntPtr.Zero);
			}
			set
			{
				SendMessage(this.Handle, LB_SETCOUNT, value, IntPtr.Zero);
			}
		}

		/// <summary>
		/// Gets/sets the DrawMode of the ListBox.  The DrawMode must always
		/// be set to <see cref="System.Windows.Forms.DrawMode.OwnerDrawFixed"/>.
		/// </summary>
		[Browsable(false)]
		public new System.Windows.Forms.DrawMode DrawMode
		{
			get
			{
				return System.Windows.Forms.DrawMode.OwnerDrawFixed;
			}
			set
			{
				if (value != System.Windows.Forms.DrawMode.OwnerDrawFixed)
				{
					throw new ArgumentException("DrawMode must be set to OwnerDrawFixed in a Virtual ListBox");
				}
			}
		}

		/// <summary>
		/// Throws an exception.  All the items for a Virtual ListBox are externally managed.
		/// </summary>
		[BrowsableAttribute(false)]
		public new ObjectCollection Items
		{
			get
			{
				throw new InvalidOperationException("A Virtual ListBox does not have an Items collection.");
			}
		}		
		
		/// <summary>
		/// Throws an exception.  All the items for a Virtual ListBox are externally managed.
		/// </summary>
		/// <remarks>The selected index can be obtained using the <see cref="SelectedIndex"/> and
		/// <see cref="SelectedIndices"/> properties.
		/// </remarks>
		[BrowsableAttribute(false)]
		public new SelectedObjectCollection SelectedItems
		{
			get
			{
				throw new InvalidOperationException("A Virtual ListBox does not have a SelectedObject collection");
			}
		}

		/// <summary>
		/// Hides the Sorted property of the ListBox control.  Any attempt to set this property
		/// to true will result in an exception.
		/// </summary>
		[BrowsableAttribute(false)]
		public new bool Sorted
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
				{
					throw new InvalidOperationException("A Virtual ListBox cannot be sorted.");
				}
			}
		}

		/// <summary>
		/// Returns the selected index in the control.  If the control has the multi-select
		/// style, then the first selected item is returned.
		/// </summary>
		public new int SelectedIndex
		{
			get
			{
				int selIndex = -1;
				if (SelectionMode == System.Windows.Forms.SelectionMode.One)
				{
					selIndex = SendMessage(this.Handle, LB_GETCURSEL, 0, IntPtr.Zero);
				}			
				else if ((SelectionMode == System.Windows.Forms.SelectionMode.MultiExtended) || 
					(SelectionMode == System.Windows.Forms.SelectionMode.MultiSimple))
				{
					int selCount = SendMessage(this.Handle, LB_GETSELCOUNT, 0, IntPtr.Zero);
					if (selCount > 0)
					{
						IntPtr buf = Marshal.AllocCoTaskMem(4);
						SendMessage(this.Handle, LB_GETSELITEMS, 1, buf);
						selIndex = Marshal.ReadInt32(buf);
						Marshal.FreeCoTaskMem(buf);
					}
				}
				return selIndex;
			}
			set
			{
				if (SelectionMode == System.Windows.Forms.SelectionMode.One)
				{
					SendMessage(this.Handle, LB_SETCURSEL, value, IntPtr.Zero);
				}
				else if ((SelectionMode == System.Windows.Forms.SelectionMode.MultiExtended) || 
					(SelectionMode == System.Windows.Forms.SelectionMode.MultiSimple))
				{
					Console.WriteLine("Working on it");
				}
			}	
		}		

		/// <summary>
		///  todo
		/// </summary>
		public new SelectedIndexCollection SelectedIndices 
		{
			get
			{
				return selectedIndices;
			}
		}

		/// <summary>
		/// Gets the selection state for an item.
		/// </summary>
		/// <param name="index">Index of the item.</param>
		/// <returns><c>true</c> if selected, <c>false</c> otherwise.</returns>
		public bool ItemSelected(int index)
		{
			bool state = false;
			if (SelectionMode == System.Windows.Forms.SelectionMode.One)
			{
				state = (SelectedIndex == index);
			}			
			else if ((SelectionMode == System.Windows.Forms.SelectionMode.MultiExtended) || 
				(SelectionMode == System.Windows.Forms.SelectionMode.MultiSimple))
			{
				state = (SendMessage(this.Handle, LB_GETSEL, index, IntPtr.Zero) != 0);
			}
			return state;
		}

		/// <summary>
		/// Sets the selection state for an item.
		/// </summary>
		/// <param name="index">Index of the item.</param>
		/// <param name="state">New selection state for the item.</param>
		public void ItemSelected(int index, bool state)
		{
			if (SelectionMode == System.Windows.Forms.SelectionMode.One)
			{
				if (state)
				{
					SelectedIndex = index;
				}
			}			
			else if ((SelectionMode == System.Windows.Forms.SelectionMode.MultiExtended) || 
				(SelectionMode == System.Windows.Forms.SelectionMode.MultiSimple))
			{
				SendMessage(this.Handle, LB_SETSEL, (state ? 1 : 0), (IntPtr) index);
			}
		}

		/// <summary>
		/// Called when an item in the control needs to be drawn, and raises the 
		/// <see cref="DrawItem"/> event.
		/// </summary>
		/// <param name="e">Details about the item that is to be drawn.</param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if ((e.State & DrawItemState.Selected ) == DrawItemState.Selected )
			{
				selectedIndex = e.Index;
			}
			base.OnDrawItem(e);
		}

		/// <summary>
		/// Sets up the <see cref="CreateParams"/> object to tell Windows
		/// how the ListBox control should be created.  In this instance
		/// the default configuration is modified to remove <c>LBS_HASSTRINGS</c>
		/// and <c>LBS_SORT</c> styles and to add <c>LBS_NODATA</c>
		/// and <c>LBS_OWNERDRAWFIXED</c> styles. This converts the ListBox
		/// into a Virtual ListBox.
		/// </summary>
		protected override System.Windows.Forms.CreateParams CreateParams
		{
			get
			{
				CreateParams defParams = base.CreateParams;
				Console.WriteLine("In Param style: {0:X8}", defParams.Style);
				defParams.Style = defParams.Style & ~LBS_HASSTRINGS;
				defParams.Style = defParams.Style & ~LBS_SORT;
				defParams.Style = defParams.Style | LBS_OWNERDRAWFIXED | LBS_NODATA;
				Console.WriteLine("Out Param style: {0:X8}", defParams.Style);
				return defParams;
			}
		}

		/// <summary>
		/// Called when the ListBox handle is destroyed.  
		/// </summary>
		/// <param name="e">Not used</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			// Nasty.  The problem is with the call to NativeUpdateSelection,
			// which calls the EnsureUpToDate on the SelectedObjectCollection method, 
			// and that is broken.
			try
			{
				base.OnHandleDestroyed(e);
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Implements a read-only collection of selected items in the
		/// VListBox.
		/// </summary>
		public new class SelectedIndexCollection : ICollection, IEnumerable
		{
			private VListBox owner = null;

			/// <summary>
			/// Creates a new instance of this class
			/// </summary>
			/// <param name="owner">The VListBox which owns the collection</param>
			public SelectedIndexCollection(VListBox owner)
			{
				this.owner = owner;
			}

			/// <summary>
			/// Returns an enumerator which allows iteration through the selected items
			/// collection.
			/// </summary>
			/// <returns></returns>
			public IEnumerator GetEnumerator()
			{
				return new SelectedIndexCollectionEnumerator(this.owner);
			}

			/// <summary>
			/// Not implemented. Throws an exception.
			/// </summary>
			/// <param name="dest">Array to copy items to</param>
			/// <param name="startIndex">First index in array to put items in.</param>
			public void CopyTo(Array dest, int startIndex)
			{
				throw new InvalidOperationException("Not implemented");
			}

			/// <summary>
			/// Returns the number of items in the collection.
			/// </summary>
			public int Count
			{
				get
				{
					return SendMessage(owner.Handle, LB_GETSELCOUNT, 0, IntPtr.Zero);
				}
			}

			/// <summary>
			/// Returns the selected item with the specified 0-based index in the collection
			/// of selected items.  
			/// </summary>
			/// <remarks>
			/// Do not use this method to enumerate through all selected
			/// items as it gets the collection of selected items each item it 
			/// is called.  The <c>foreach</c> enumerator only gets the collection
			/// of items once when it is constructed and is therefore quicker.
			/// </remarks>
			public int this[int index]
			{
				get
				{
					int selIndex = -1;
					int selCount = SendMessage(owner.Handle, LB_GETSELCOUNT, 0, IntPtr.Zero);
					if ((index < selCount) && (index > 0))
					{
						IntPtr buf = Marshal.AllocCoTaskMem(4 * (index + 1));
						SendMessage(owner.Handle, LB_GETSELITEMS, selCount, buf);
						selIndex = Marshal.ReadInt32(buf, index * 4);
						Marshal.FreeCoTaskMem(buf);
					}
					else
					{
						throw new ArgumentException("Index out of bounds", "index");
					}
					return selIndex;
					
				}
			}

			/// <summary>
			/// Returns <c>false</c>.  This collection is not synchronized for
			/// concurrent access from multiple threads.
			/// </summary>
			public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Not implemented. Throws an exception.
			/// </summary>
			public object SyncRoot
			{
				get
				{
					throw new InvalidOperationException("Synchronization not supported.");
				}
			}


		}

		/// <summary>
		/// Implements the <see cref="IEnumerator"/> interface for the selected indexes
		/// within a <see cref="VListBox"/> control.
		/// </summary>
		public class SelectedIndexCollectionEnumerator : IEnumerator, IDisposable
		{
			private IntPtr buf = IntPtr.Zero;
			private int size = 0;
			private int offset = 0;
		
			/// <summary>
			/// Constructs a new instance of this class.
			/// </summary>
			/// <param name="owner">The <see cref="VListBox"/> which owns the collection.</param>
			public SelectedIndexCollectionEnumerator(VListBox owner)
			{
				int selCount = SendMessage(owner.Handle, LB_GETSELCOUNT, 0, IntPtr.Zero);
				if (selCount > 0)
				{
					buf = Marshal.AllocCoTaskMem(4 * selCount);
					SendMessage(owner.Handle, LB_GETSELITEMS, selCount, buf);
				}
			}

			/// <summary>
			/// Clears up any resources associated with this enumerator.
			/// </summary>
			public void Dispose()
			{
				if (!buf.Equals(IntPtr.Zero))
				{
					Marshal.FreeCoTaskMem(buf);
					buf = IntPtr.Zero;
				}
			}

			/// <summary>
			/// Resets the enumerator to the start of the list.
			/// </summary>
			public void Reset()
			{
				offset = 0;
			}

			/// <summary>
			/// Returns the current object.
			/// </summary>
			public object Current
			{
				get
				{
					if (offset >= size)
					{
						throw new Exception("Collection is exhausted.");
					}
					else
					{
						int index = Marshal.ReadInt32(buf, offset * 4);
						return (object) index;
					}

				}
			}

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns><c>true</c> if the enumerator was successfully advanced to the next element; 
			/// <c>false</c> if the enumerator has passed the end of the collection.</returns>
			public bool MoveNext()
			{
				bool success = false;
				offset++;
				if (offset < size)
				{
					success = true;
				}
				return success;
			}
		}


	}
}
