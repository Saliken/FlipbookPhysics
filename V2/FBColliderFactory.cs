using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public static class FBColliderFactory
    {
        public static FBCollider CreateCircle(float radius)
        {
            return CreateCircle(radius, Vector2.Zero);
        }
        public static FBCollider CreateCircle(float radius, Vector2 offset)
        {
            var collider = new FBCircle(radius, offset);
            return collider;
        }

        public static FBCollider CreateRectangle(float width, float height, bool centered)
        {
            FBRectangle collider;
            if (centered)
                collider = new FBRectangle(-width / 2, -height / 2, width, height);
            else
                collider = new FBRectangle(0, 0, width, height);
            return collider;
        }

        public static FBCollider CreatePolygon(List<Vector2> points)
        {
            FBPolygon collider = new FBPolygon();
            points.ForEach(x => collider.AddPoint(x.X, x.Y));
            
            return collider;
        }
    }
}
