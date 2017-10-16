using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookCollision
{
    public class FBPolygon : Shape
    {
        public List<Vector2> Points;
        public List<Line> Lines;
        public Vector2 Position = Vector2.Zero;

        public List<Vector2> MovedPoints { get { return new List<Vector2>(Points.Select(x => new Vector2(x.X + Position.X, x.Y + Position.Y))); } }
        public List<Line> MovedLines { get { return new List<Line>(Lines.Select(x => new Line(x.StartPosition + Position, x.EndPosition + Position))); } }

        public FBPolygon()
        {
            Points = new List<Vector2>();
            Lines = new List<Line>();
        }

        public void AddPoint(float x, float y)
        {
            Points.Add(new Vector2(x, y));

            if (Points.Count > 1)
            {
                if (Lines.Count > 0)
                    Lines.Remove(Lines[Lines.Count - 1]);

                Lines.Add(new Line(Points[Points.Count - 2], new Vector2(x, y)));
                Lines.Add(new Line(new Vector2(x, y), Points[0]));
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
        public override List<Vector2> CollisionAxes(Shape otherShape)
        {
            return MovedLines.Select(x => x.NormalRight).ToList();
        }
        public override void Project(Vector2 axis, out float min, out float max)
        {
            float dot = Vector2.Dot(axis, MovedPoints[0]);
            var minValue = dot;
            var maxValue = dot;
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
    }
}
