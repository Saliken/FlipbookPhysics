using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class Line
    {
        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }

        public float A
        {
            get
            {
                return EndPosition.Y - StartPosition.Y;
            }
        }
        public float B
        {
            get
            {
                return StartPosition.X - EndPosition.X;
            }
        }
        public float C
        {
            get
            {
                return A * StartPosition.X + B * StartPosition.Y;
            }
        }

        public Vector2 NormalRight
        {
            get
            {
                var v = (EndPosition - StartPosition);
                v.Normalize();
                return new Vector2(v.Y, -v.X);
            }
        }
        public Vector2 NormalLeft
        {
            get
            {
                var v = EndPosition - StartPosition;
                v.Normalize();
                return new Vector2(-v.Y, v.X);
            }
        }

        public Line() { }
        public Line(Vector2 startPosition, Vector2 endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public bool Intersects(Line otherLine, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;

            float delta = A * otherLine.B - otherLine.A * B;
            if(delta == 0)
            {
                //Lines are parallel
                return false;
            }
            else
            {
                float x = (otherLine.B * C - B * otherLine.C) / delta;
                float y = (A * otherLine.C - otherLine.A * C) / delta;

                if(Math.Min(StartPosition.X, EndPosition.X) <= x && x <= Math.Max(StartPosition.X, EndPosition.X)
                    && Math.Min(StartPosition.Y, EndPosition.Y) <= y && y <= Math.Max(StartPosition.Y, EndPosition.Y))
                {
                    intersectionPoint = new Vector2(x, y);
                    return true;
                }

                return false;
            }
        }
    }
}
