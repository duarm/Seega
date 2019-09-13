using System.Collections;
using Seega.Types;

namespace Kurenaiz.Utilities.Types
{
	public class SafeArray<T>
	{
		private T[] items;

		public SafeArray (int capacity)
		{
			items = new T[capacity];
		}

		public object this [int index]
		{
			get
			{
				return index < items.Length ? (object) items[index] : null;
			}
			set
			{
				items[index] = (T) value;
			}
		}
	}

	public class Safe2DArray<T>
	{
		private T[, ] items;

		public Safe2DArray (int rows, int columns)
		{
			items = new T[rows, columns];
		}

		public object this [int row, int column]
		{
			get
			{
				return row < items.GetLength (0) && column < items.GetLength (1) ? (object) items[row, column] : null;
			}
			set
			{
				items[row, column] = (T) value;
			}
		}
	}

	//The safe array returns null instead of OutOfBoundsException
	public class Safe2DArray : IEnumerable
	{
		private TileField[, ] items;

		public Safe2DArray (int rows, int columns)
		{
			items = new TileField[rows, columns];
		}

		public TileField this [int row, int column]
		{
			get
			{
				if (row < 0 || column < 0)
					return null;
				return row < items.GetLength (0) && column < items.GetLength (1) ? items[row, column] : null;
			}
			set
			{
				items[row, column] = value;
			}
		}

		public TileField this [Coordinates coor]
		{
			get
			{
				if (coor.x < 0 || coor.y < 0)
					return null;
				return coor.x < items.GetLength (0) && coor.y < items.GetLength (1) ? items[coor.x, coor.y] : null;
			}
			set
			{
				items[coor.x, coor.y] = value;
			}
		}

		public IEnumerator GetEnumerator ()
		{
			return items.GetEnumerator ();
		}
	}
}