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

        public FBCircle(float radius, Vector2 position)
        {
            Radius = radius;
            Position = position;
        }

        public override Rectangle AABB()
        {
            var x = Position.X - Radius;
            var y = Position.Y - Radius;
            var width = Position.X + Radius + x;
            var height = Position.Y + Radius - y;
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        public override Vector2 NearestPoint(Vector2 to)
        {
            return Position;
        }
        public override List<Vector2> CollisionAxes(FBCollider shapeToCheckAgainst)
        {
            var nearestPointOnOtherShape = shapeToCheckAgainst.NearestPoint(Position);
            var direction = Position -  nearestPointOnOtherShape;
            if (direction == Vector2.Zero) //Centers are the same.
            {
                direction = new Vector2(0, 1);
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
