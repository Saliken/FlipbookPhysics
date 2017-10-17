using FlipbookPhysics;
using FlipbookPhysics.Shapes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookCollision
{
    public static class FBBodyFactory
    {
        public static FBBody CreateCircle(float radius)
        {
            return CreateCircle(radius, Vector2.Zero);
        }
        public static FBBody CreateCircle(float radius, Vector2 offset)
        {
            var body = new FBBody();
            body.colliders.Add(new FBCircle(radius, offset));
            return body;
        }

        public static FBBody CreateRectangle(float width, float height, bool centered)
        {
            var body = new FBBody();
            if(centered)
                body.colliders.Add(new FBRectangle(-width / 2, -height / 2, width / 2, height / 2));
            else
                body.colliders.Add(new FBRectangle(0, 0, width, height));
            return body;
        }

        public static FBBody CreatePolygon(List<Vector2> points)
        {
            var body = new FBBody();
            return body;
        }
    }
}
