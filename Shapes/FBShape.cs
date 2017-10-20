using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{ 
    public abstract class FBShape
    {
        public Vector2 Position;
        public float Rotation;

        public abstract Vector2 NearestPoint(Vector2 to);
        public abstract List<Vector2> CollisionAxes(FBShape shapeToCheckAgainst);

        public abstract void Project(Vector2 axis, out float min, out float max);
        public abstract void Project(Vector2 axis, out float min, out float max, out float moveMin, out float moveMax, Vector2 movement);
    }
}
