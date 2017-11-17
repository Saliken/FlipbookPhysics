using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    class FBSpatialHash<T>
    {
        protected int bucketWidth;
        protected int bucketHeight;

        public List2D<List<T>> hash;


        public FBSpatialHash(int bucketWidth, int bucketHeight)
        {
            this.bucketWidth = bucketWidth;
            this.bucketHeight = bucketHeight;
        }

        public void Add(T item, Rectangle itemAABB)
        {
            var startX = itemAABB.Left / bucketWidth;
            var startY = itemAABB.Top / bucketHeight;  
            var endX = itemAABB.Right / bucketWidth;
            var endY = itemAABB.Bottom / bucketHeight;

            for(var x = startX; x <= endX; x++)
            {
                for(var y = startY; y <= endY; y++)
                {
                    var bucket = hash[x, y];
                    if (bucket == null)
                    {
                        bucket = new List<T>();
                        hash[x, y] = bucket;
                    }
                    bucket.Add(item);                    
                }
            }
        }

        #region Get Buckets
        public List<T> Get(int x, int y)
        {
            PointToHashIndex(x, y, out var iX, out var iY);
            return hash[iX, iY];
        }
        public List<T> Get(float x, float y)
        {
            PointToHashIndex(x, y, out var iX, out var iY);
            return hash[iX, iY];
        }
        public List<T> GetPoint(Point point)
        {
            PointToHashIndex(point, out var iX, out var iY);
            return hash[iX, iY];
        }
        public List<T> GetVector2(Vector2 vector2)
        {
            PointToHashIndex(vector2, out var iX, out var iY);
            return hash[iX, iY];
        }
        public List<T> GetRectangle(Rectangle rectangle)
        {
            List<T> list = new List<T>();
            var startX = rectangle.Left / bucketWidth;
            var startY = rectangle.Top / bucketHeight;
            var endX = rectangle.Right / bucketWidth;
            var endY = rectangle.Bottom / bucketHeight;

            for(int x = startX; x <= endX; x++)
            {
                for(int y = startY; y <= endY; y++)
                {
                    list.AddRange(hash.GetAt(x, y));
                }
            }

            return list;
        }
        public void GetLine(Point start, Point end)
        {

        }
        public void GetLine(Vector2 start, Vector2 end)
        {

        }
        public void GetCircle(Point position, float radius)
        {

        }
        public void GetCircle(Vector2 position, float radius)
        {

        }
        #endregion

        protected void PointToHashIndex(Point point, out int x, out int y)
        {
            x = point.X / bucketWidth;
            y = point.Y / bucketHeight;
        }
        protected void PointToHashIndex(Vector2 vector2, out int x, out int y)
        {
            x = (int)vector2.X / bucketWidth;
            y = (int)vector2.Y / bucketHeight;
        }
        protected void PointToHashIndex(int pointX, int pointY, out int x, out int y)
        {
            x = pointX / bucketWidth;
            y = pointY / bucketHeight;
        }
        protected void PointToHashIndex(float pointX, float pointY, out int x, out int y)
        {
            x = (int)pointX / bucketWidth;
            y = (int)pointY / bucketHeight;
        }
    }
}
