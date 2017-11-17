using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public static class FBBresenhamHelper
    {
        public static List<Point> Line(Point start, Point end)
        {
            return Line(start.X, start.Y, end.X, end.Y);
        }
        public static List<Point> Line(Vector2 start, Vector2 end)
        {
            return Line((int)start.X, (int)start.Y, (int)end.X, (int)end.Y);
        }
        public static List<Point> Line(float x, float y, float x2, float y2)
        {
            return Line((int)x, (int)y, (int)x2, (int)y2);
        }
        public static List<Point> Line(int x, int y, int x2, int y2)
        {
            List<Point> linePoints = new List<Point>();
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                linePoints.Add(new Point(x, y));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }

            return linePoints;
        }

        public static List<Point> Circle(int cX, int cY, int radius)
        {
            List<Point> points = new List<Point>();

            int error = -radius;
            int x = radius;
            int y = 0;

            while(x >= y)
            {
                int lastY = y;
                error += y;
                ++y;
                error += y;

                points.AddRange(PlotPoints(cX, cY, x, lastY));

                if(error >= 0)
                {
                    if (x != lastY)
                        points.AddRange(PlotPoints(cX, cY, lastY, x));

                    error -= x;
                    --x;
                    error -= x;
                }
            }

            return points;
        }

        private static List<Point> PlotPoints(int x1, int y1, int x2, int y2)
        {
            var points = new List<Point>();

            var startX = x1 - x2;
            var y = y1 + y2;
            var endX = x1 + x2;
            for (var x = startX; x <= endX; x++)
            {
                points.Add(new Point(x, y));
            }

            if(y2 != 0)
            {
                startX = x1 - x2;
                y = y1 - y2;
                endX = x1 + x2;
                for (var x = startX; x <= endX; x++)
                {
                    points.Add(new Point(x, y));
                }
            }

            return points;
        }
    }
}
