﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FlipbookCollision.Shapes
{
    public class FBRectangle : Shape
    {
        Vector2 Position;
        float width;
        float height;

        public List<Line> Lines
        {
            get
            {
                return new List<Line>()
                {
                    new Line(Position, Position + new Vector2(width, 0)),
                    new Line(Position + new Vector2(width, 0), Position + new Vector2(width, height)),
                    new Line(Position + new Vector2(width, height), Position + new Vector2(0, height)),
                    new Line(Position + new Vector2(0, height), Position)
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

        public override List<Vector2> CollisionAxes(Shape otherShape)
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
            float dot;
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            foreach(var point in Points)
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
