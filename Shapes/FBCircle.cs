using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class FBCircle : FBCollider
    {
        public float Radius { get; set; }

        public FBCircle(float radius, Vector2 offset)
        {
            Radius = radius;
            Offset = offset;
        }

        public override Vector2 NearestPoint(Vector2 to)
        {
            return Position;
        }
        public override List<Vector2> CollisionAxes(FBCollider shapeToCheckAgainst)
        {
            var nearestPointOnOtherShape = shapeToCheckAgainst.NearestPoint(Position);
            var direction = nearestPointOnOtherShape - Position;
            if (direction == Vector2.Zero) //Centers are the same.
            {
                Random r = new Random();
                direction = new Vector2(r.Next(-100, 100), r.Next(-100, 100));
            }
            direction.Normalize();
            return new List<Vector2>() { direction };
        }
        public override void Project(Vector2 axis, out float min, out float max)
        {
            float dot = Vector2.Dot(axis, Position);
            min = dot - Radius;
            max = dot + Radius;
        }
    }
}
