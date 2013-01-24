using System;
using System.Reflection;
using System.Collections;

namespace VAGSuite
{
	/// <summary>
	/// Author : Sree Krishna.S 
	/// Email : krishna_accent@yahoo.com
	/// Please donot remove above comment.
	/// This class is used to compare any type(property) of a class.
	/// This class automatically fetches the type of the property and compares.
	/// </summary>
	public sealed class GenericComparer:IGenericComparer 
	{
		#region GenericComparer
		/// <summary>
		/// Sorting order
		/// </summary>
		public enum SortOrder
		{
			Ascending = 0,
			Descending = 1
		}

		Type objectType;
		/// <summary>
		/// Type of the object to be compared.
		/// </summary>
		public Type ObjectType
		{
			get{return objectType;}set{objectType = value;}
		}

		
		string sortcolumn = "";
		/// <summary>
		/// Column(public property of the class) to be sorted.
		/// </summary>
		public string SortColumn
		{
			get{return sortcolumn;}set{sortcolumn = value;}
		}


		int sortingOrder = 0;
		/// <summary>
		/// Sorting order.
		/// </summary>
		public int SortingOrder
		{
			get{return sortingOrder;}set{sortingOrder = value;}
		}

		/// <summary>
		/// Compare interface implementation
		/// </summary>
		/// <param name="x">Object 1</param>
		/// <param name="y">Object 2</param>
		/// <returns>Result of comparison</returns>
		public int Compare(object x, object y)
		{
			//Dynamically get the protery info based on the protery name
			PropertyInfo propertyInfo = ObjectType.GetProperty(sortcolumn);
			//Get the value of the instance
			IComparable obj1 = (IComparable)propertyInfo.GetValue(x,null) ;
			IComparable obj2 = (IComparable)propertyInfo.GetValue(y,null) ;
			//Compare based on the sorting order.
			if(sortingOrder == 0)
				return ( obj1.CompareTo(obj2) );
			else
				return ( obj2.CompareTo(obj1) );
		}

		#endregion
	}

	/// <summary>
	/// IGenericComparer - Generic interface for object comparison
	/// </summary>
	public interface IGenericComparer : IComparer
	{
		Type ObjectType
		{
			get;set;
		}
		string SortColumn
		{
			get;set;
		}
		int SortingOrder
		{
			get;set;
		}
	}

	/// <summary>
	/// ISortable - Generic interface for sortable collection
	/// </summary>
	public interface ISortable
	{
		string SortColumn
		{
			get;set;
		}
		GenericComparer.SortOrder SortingOrder
		{
			get;set;
		}
		Type SortObjectType
		{
			get;set;
		}
		void Sort();
	}

	/// <summary>
	/// Abstract implementation of Sortable collection.
	/// </summary>
	public abstract class SortableCollectionBase : CollectionBase,ISortable
	{
		string sortcolumn="";
		public string SortColumn
		{
			get{return sortcolumn;}
			set{sortcolumn = value;}
		}

		GenericComparer.SortOrder sortingOrder = GenericComparer.SortOrder.Ascending;
		public GenericComparer.SortOrder SortingOrder
		{
			get{return sortingOrder;}set{sortingOrder = value;}
		}

		Type sortObjectType;
		public Type SortObjectType
		{
			get{return sortObjectType;} set{sortObjectType = value;} 
		}

		public virtual void Sort() 
		{
			if(sortcolumn == "") throw new Exception("Sort column required."); 
			if(SortObjectType == null) throw new Exception("Sort object type required."); 
			IGenericComparer sorter = new GenericComparer();
			sorter.ObjectType = sortObjectType;
			sorter.SortColumn = sortcolumn;
			sorter.SortingOrder = (int)sortingOrder;
			InnerList.Sort(sorter);
		}
	}


}
