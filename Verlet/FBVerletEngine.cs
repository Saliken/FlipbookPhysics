using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookCollision.Verlet
{
    class FBVerletEngine
    {
        public List<FBVerletPoint> points;
        public List<FBVerletEdge> edges;

        public void Update()
        {
            foreach(var edge in edges)
            {
                ConstrainEdge(edge);
            }
        }

        protected void ConstrainEdge(FBVerletEdge edge)
        {
            var direction = edge.End.Position - edge.Start.Position;
            var length = direction.Length();
            var difference = length - edge.Length;
            direction.Normalize();
            edge.Start.Position += direction * difference * 0.5f;
            edge.End.Position -= direction * difference * 0.5f;
        }
    }
}
