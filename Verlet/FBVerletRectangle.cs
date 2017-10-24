using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.Verlet
{
    public class FBVerletRectangle : FBVerletShape
    {
        public FBVerletRectangle(int x, int y, int width, int height)
        {
            points.Add(new FBVerletPoint(new Vector2(x, y)));
            points.Add(new FBVerletPoint(new Vector2(x + width, y)));
            points.Add(new FBVerletPoint(new Vector2(x + width, y + height)));
            points.Add(new FBVerletPoint(new Vector2(x, y + height)));

            collisionEdges.Add(new FBVerletEdge(points[0], points[1], width));
            collisionEdges.Add(new FBVerletEdge(points[1], points[2], height));
            collisionEdges.Add(new FBVerletEdge(points[2], points[3], width));
            collisionEdges.Add(new FBVerletEdge(points[3], points[0], height));
            edges.Add(new FBVerletEdge(points[0], points[2], Vector2.Distance(points[0].Position, points[2].Position)));
        }
    }
}
