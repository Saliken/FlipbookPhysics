using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookCollision
{
    public class Line
    {
        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }
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
    }
}
