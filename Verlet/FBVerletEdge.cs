using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.Verlet
{
    public class FBVerletEdge
    {
        public FBVerletPoint Start;
        public FBVerletPoint End;
        public float Length;

        public Vector2 NormalLeft
        {
            get
            {
                var v = End.Position - Start.Position;
                v.Normalize();
                return new Vector2(-v.Y, v.X);
            }
        }
        public Vector2 NormalRight
        {
            get
            {
                var v = (End.Position - Start.Position);
                v.Normalize();
                return new Vector2(v.Y, -v.X);
            }
        }

        public FBVerletEdge() { }
        public FBVerletEdge(FBVerletPoint start, FBVerletPoint end, float length)
        {
            Start = start;
            End = end;
            Length = length;
        }

        public void Constrain()
        {
            var dx = End.Position.X - Start.Position.X;
            var dy = End.Position.Y - Start.Position.Y;
            var delta = Length / (dx * dx + dy * dy + Length) - 0.5f;
            dx *= delta;
            dy *= delta;
            End.Position.X += dx;
            End.Position.Y += dy;
            Start.Position.X -= dx;
            Start.Position.Y -= dy;

           /*var direction = End.Position - Start.Position;
            var length = direction.Length();
            var difference = length - Length;
            direction.Normalize();
            Start.Position += direction * difference * 0.5f;
            End.Position -= direction * difference * 0.5f;*/
        }
    }
}
