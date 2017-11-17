using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{ 

    public class List2D<T>
    {

        private Dictionary<Point, T> items;

        public List2D()
        {
            items = new Dictionary<Point, T>();
        }

        public void AddAt(T item, int x, int y)
        {
            
            if(items.ContainsKey(new Point(x, y)))
            {
                items[new Point(x, y)] = item;
            }

            items.Add(new Point(x, y), item);
        }
        public T GetAt(int x, int y)
        {
            if(items.TryGetValue(new Point(x, y), out var list))
            {
                return list;
            }
            else
            {
                return default(T);
            }
        }

        public void Clear()
        {
            items.Clear();
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
