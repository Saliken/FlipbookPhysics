using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FlipbookPhysics
{
    public class FBRectangle : FBCollider
    {
        public float width;
        public float height;

        public List<FBLine> Lines
        {
            get
            {
                return new List<FBLine>()
                {
                    new FBLine(Position, Position + new Vector2(width, 0)),
                    new FBLine(Position + new Vector2(width, 0), Position + new Vector2(width, height)),
                    new FBLine(Position + new Vector2(width, height), Position + new Vector2(0, height)),
                    new FBLine(Position + new Vector2(0, height), Position)
                };
            }
        }
        public List<Vector2> Points
        {
            get
            {
                return new List<Vector2>()
                {
                    Position,
                    Position + new Vector2(width, 0),
                    Position + new Vector2(width, height),
                    Position + new Vector2(0, height)
                };
            }
        }

        public FBRectangle(float x, float y, float width, float height)
        {
            this.Offset = new Vector2(x, y);
            this.width = width;
            this.height = height;
        }

        public override List<Vector2> CollisionAxes(FBCollider otherShape)
        {
            return Lines.Select(x => x.NormalRight).ToList();
        }
        public override Vector2 NearestPoint(Vector2 to)
        {
            Vector2 nearestPoint = Vector2.Zero;
            float minDistance = float.MaxValue;
            foreach(var point in Points)
            {
                var distance = Vector2.Distance(point, to);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }
            return nearestPoint;
        }

        public override void Project(Vector2 axis, out float min, out float max)
        {
            float dot, minValue, maxValue;
            dot = Vector2.Dot(Points[0], axis);
            minValue = dot;
            maxValue = dot;
            foreach (var point in Points)
            {
                dot = Vector2.Dot(point, axis);
                if (dot < minValue)
                    minValue = dot;
                else if (dot > maxValue)
                    maxValue = dot;
            }
            min = minValue;
            max = maxValue;
        }
    }
}
