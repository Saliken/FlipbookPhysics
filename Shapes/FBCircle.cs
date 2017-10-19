using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class FBCircle : FBShape
    {
        public float Radius { get; set; }

        public FBCircle(float radius, Vector2 offset)
        {
            Radius = radius;
            Position = offset;
        }

        public override Vector2 NearestPoint(Vector2 to)
        {
            return Position;
        }
        public override List<Vector2> CollisionAxes(FBShape shapeToCheckAgainst)
        {
            var nearestPointOnOtherShape = shapeToCheckAgainst.NearestPoint(Position);
            var direction = nearestPointOnOtherShape - Position;
            direction.Normalize();
            return new List<Vector2>() { direction };
        }
        public override void Project(Vector2 axis, out float min, out float max)
        {
            float dot = Vector2.Dot(axis, Position);
            min = dot - Radius;
            max = dot + Radius;
        }
        public override void Project(Vector2 axis, out float min, out float max, out float moveMin, out float moveMax, Vector2 movement)
        {
            float dot = Vector2.Dot(axis, Position);
            min = dot - Radius;
            max = dot + Radius;

            dot = Vector2.Dot(axis, Position + movement);
            moveMin = dot - Radius;
            moveMax = dot + Radius;
        }
    }
}
