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
        protected int bucketDimension;

        public List2D<List<T>> hash;


        public FBSpatialHash(int bucketDimension)
        {
            this.bucketDimension = bucketDimension;
        }

        public void Add(T item, Rectangle itemAABB)
        {
            var startX = itemAABB.Left / bucketDimension;
            var startY = itemAABB.Top / bucketDimension;  
            var endX = itemAABB.Right / bucketDimension;
            var endY = itemAABB.Bottom / bucketDimension;

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
            var startX = rectangle.Left / bucketDimension;
            var startY = rectangle.Top / bucketDimension;
            var endX = rectangle.Right / bucketDimension;
            var endY = rectangle.Bottom / bucketDimension;

            for(int x = startX; x <= endX; x++)
            {
                for(int y = startY; y <= endY; y++)
                {
                    list.AddRange(hash.GetAt(x, y));
                }
            }

            return list;
        }
        public List<T> GetLine(Point start, Point end)
        {
            var list = new List<T>();
            var linePoints = FBBresenhamHelper.Line(start.X / bucketDimension, start.Y / bucketDimension, end.X / bucketDimension, end.Y / bucketDimension);
            foreach(var point in linePoints)
            {
                list.AddRange(hash.GetAt(point.X, point.Y));
            }
            return list;
        }
        public List<T> GetLine(Vector2 start, Vector2 end)
        {
            var list = new List<T>();
            var linePoints = FBBresenhamHelper.Line(start.X / bucketDimension, start.Y / bucketDimension, end.X / bucketDimension, end.Y / bucketDimension);
            foreach (var point in linePoints)
            {
                list.AddRange(hash.GetAt(point.X, point.Y));
            }
            return list;
        }
        public List<T> GetCircle(Point position, float radius)
        {
            var list = new List<T>();
            var circlePoints = FBBresenhamHelper.Circle(position.X / bucketDimension, position.Y / bucketDimension, (int)radius / bucketDimension);
            foreach(var point in circlePoints)
            {
                list.AddRange(hash.GetAt(point.X, point.Y));
            }
            return list;
        }
        public List<T> GetCircle(Vector2 position, float radius)
        {
            var list = new List<T>();
            var circlePoints = FBBresenhamHelper.Circle((int)position.X / bucketDimension, (int)position.Y / bucketDimension, (int)radius / bucketDimension);
            foreach (var point in circlePoints)
            {
                list.AddRange(hash.GetAt(point.X, point.Y));
            }
            return list;
        }
        #endregion

        protected void PointToHashIndex(Point point, out int x, out int y)
        {
            x = point.X / bucketDimension;
            y = point.Y / bucketDimension;
        }
        protected void PointToHashIndex(Vector2 vector2, out int x, out int y)
        {
            x = (int)vector2.X / bucketDimension;
            y = (int)vector2.Y / bucketDimension;
        }
        protected void PointToHashIndex(int pointX, int pointY, out int x, out int y)
        {
            x = pointX / bucketDimension;
            y = pointY / bucketDimension;
        }
        protected void PointToHashIndex(float pointX, float pointY, out int x, out int y)
        {
            x = (int)pointX / bucketDimension;
            y = (int)pointY / bucketDimension;
        }
    }
}
