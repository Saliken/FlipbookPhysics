using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FlipbookPhysics
{
    public class FBRectangle : FBShape
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
            this.Position = new Vector2(x, y);
            this.width = width;
            this.height = height;
        }

        public override List<Vector2> CollisionAxes(FBShape otherShape)
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
        public override void Project(Vector2 axis, out float min, out float max, out float moveMin, out float moveMax, Vector2 movement)
        {
            float dot;
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            float minMoveValue = float.MaxValue;
            float maxMoveValue = float.MinValue;
            foreach(var point in Points)
            {
                dot = Vector2.Dot(point, axis);
                if (dot < minValue)
                    minValue = dot;
                else if (dot > maxValue)
                    maxValue = dot;

                dot = Vector2.Dot(point + movement, axis);
                if (dot < minMoveValue)
                    minMoveValue = dot;
                else if (dot > maxMoveValue)
                    maxMoveValue = dot;
            }
            min = minValue;
            max = maxValue;
            moveMin = minMoveValue;
            moveMax = maxMoveValue;
        }

        public override void Project(Vector2 axis, out float min, out float max)
        {
            float dot;
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
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
