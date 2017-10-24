using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.Verlet
{
    public class FBVerletPoint
    {
        public Vector2 Position;
        public Vector2 OldPosition;
        public Vector2 Acceleration;

        public FBVerletPoint()
        {

        }

        public FBVerletPoint(Vector2 Position)
        {
            this.Position = Position;
            this.OldPosition = Position;
        }
    }
}
