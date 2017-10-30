using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class FBPolygon : FBCollider
    {
        public List<Vector2> Points;
        public List<FBLine> Lines;

        public List<Vector2> MovedPoints { get { return new List<Vector2>(Points.Select(x => new Vector2((x.X * (float)Math.Cos(TotalRotation) - x.Y * (float)Math.Sin(TotalRotation)) + Position.X, (x.X * (float)Math.Sin(TotalRotation) + x.Y * (float)Math.Cos(TotalRotation)) + Position.Y))); } }
        public List<FBLine> MovedLines { get { return new List<FBLine>(Lines.Select(x => new FBLine(new Vector2((x.StartPosition.X * (float)Math.Cos(TotalRotation) - x.StartPosition.Y * (float)Math.Sin(TotalRotation)) + Position.X, (x.StartPosition.X * (float)Math.Sin(TotalRotation) + x.StartPosition.Y * (float)Math.Cos(TotalRotation)) + Position.Y), new Vector2((x.EndPosition.X * (float)Math.Cos(TotalRotation) - x.EndPosition.Y * (float)Math.Sin(TotalRotation)) + Position.X, (x.EndPosition.X * (float)Math.Sin(TotalRotation) + x.EndPosition.Y * (float)Math.Cos(TotalRotation)) + Position.Y)))); } }

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
        public override List<Vector2> CollisionAxes(FBCollider otherShape)
        {
            return MovedLines.Select(x => x.NormalRight).ToList();
        }
        public override void Project(Vector2 axis, out float min, out float max)
        {
            float dot, minValue, maxValue;
            dot = Vector2.Dot(MovedPoints[0], axis);
            minValue = dot;
            maxValue = dot;
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
