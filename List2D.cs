using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class List2D<T>
    {
        private List<T> xPositiveYPositive;
        private List<T> xNegativeYNegative;
        private List<T> xPositiveYNegative;
        private List<T> xNegativeYPositive;

        public List2D()
        {
            xPositiveYNegative = new List<T>();
            xNegativeYPositive = new List<T>();
            xPositiveYPositive = new List<T>();
            xNegativeYNegative = new List<T>();
        }

        public void AddAt(T item, int x, int y)
        {
            if(x >= 0)
            {
                if (y >= 0)
                {
                    
                }
                else
                {

                }
            }
            else
            {
                if(y >= 0)
                {

                }
                else
                {

                }
            }
        }

        public T this[int x, int y]
        {
            get
            {
                if(x >= 0)
                {
                    if(y >= 0)
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    if(y >= 0)
                    {

                    }
                    else
                    {

                    }
                }
                return default(T);
            }
        }
    }
}
