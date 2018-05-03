using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public static class Extensions
    {
        public static float AngleTo(this Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public static Vector2 RadiansToVector(this float radians, float length)
        {
            return new Vector2((float)Math.Cos(radians) * length, (float)Math.Sin(radians) * length);
        }
        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }
    }
}
