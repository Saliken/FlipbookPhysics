using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class FBPolygon : FBShape
    {
        public List<Vector2> Points;
        public List<FBLine> Lines;

        public List<Vector2> MovedPoints { get { return new List<Vector2>(Points.Select(x => new Vector2(x.X + Position.X, x.Y + Position.Y))); } }
        public List<FBLine> MovedLines { get { return new List<FBLine>(Lines.Select(x => new FBLine(x.StartPosition + Position, x.EndPosition + Position))); } }

        public FBPolygon()
        {
            Points = new List<Vector2>();
            Lines = new List<FBLine>();
        }

        public void AddPoint(float x, float y)
        {
            Points.Add(new Vector2(x, y));

            if (Points.Count > 1)
            {
                if (Lines.Count > 0)
                    Lines.Remove(Lines[Lines.Count - 1]);

                Lines.Add(new FBLine(Points[Points.Count - 2], new Vector2(x, y)));
                Lines.Add(new FBLine(new Vector2(x, y), Points[0]));
            }
        }

        public override Vector2 NearestPoint(Vector2 to)
        {
            Vector2 closestPoint = Vector2.Zero;
            float distance = float.MaxValue;
            foreach (var point in MovedPoints)
            {
                var d = Vector2.DistanceSquared(to, point);
                if (d < distance)
                {
                    closestPoint = point;
                    distance = d;
                }
            }
            return closestPoint;
        }
        public override List<Vector2> CollisionAxes(FBShape otherShape)
        {
            return MovedLines.Select(x => x.NormalRight).ToList();
        }
        public override void Project(Vector2 axis, out float min, out float max)
        {
            float dot;
            var minValue = float.MaxValue;
            var maxValue = float.MinValue;
            foreach (var point in MovedPoints)
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

        public override void Project(Vector2 axis, out float min, out float max, out float moveMin, out float moveMax, Vector2 movement)
        {
            float dot;
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            float minMoveValue = float.MaxValue;
            float maxMoveValue = float.MinValue;
            foreach (var point in MovedPoints)
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
    }
}
