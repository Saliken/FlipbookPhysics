using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class List2D<T>
    {
        private List<List<T>> xPositiveYPositive;
        private List<List<T>> xNegativeYNegative;
        private List<List<T>> xPositiveYNegative;
        private List<List<T>> xNegativeYPositive;

        public List2D()
        {
            xPositiveYNegative = new List<List<T>>();
            xNegativeYPositive = new List<List<T>>();
            xPositiveYPositive = new List<List<T>>();
            xNegativeYNegative = new List<List<T>>();
        }

        public void AddAt(T item, int x, int y)
        {
            var list = GetList(x, y);
            var absX = Math.Abs(x);
            var absY = Math.Abs(y);

            var column = list[absX];
            if (column == null)
                list[absX] = new List<T>();

            list[absX][absY] = item;
        }
        public T GetAt(int x, int y)
        {
            var list = GetList(x, y);
            var absX = Math.Abs(x);
            var absY = Math.Abs(y);

            var column = list[absX];
            if (column != null)
                return column[absY];

            return default(T);
        }

        public void Clear()
        {
            xPositiveYNegative.Clear();
            xPositiveYPositive.Clear();
            xNegativeYPositive.Clear();
            xNegativeYNegative.Clear();
        }

        protected List<List<T>> GetList(int x, int y)
        {
            if (x >= 0)
            {
                if (y >= 0)
                    return xPositiveYPositive;
                else
                    return xPositiveYNegative;
            }
            else
            {
                if (y >= 0)
                    return xNegativeYPositive;
                else
                    return xNegativeYNegative;
            }
        }

        public T this[int x, int y]
        {
            get
            {
                return GetAt(x, y);
            }
            set
            {
                AddAt(value, x, y);
            }
        }
    }
}
