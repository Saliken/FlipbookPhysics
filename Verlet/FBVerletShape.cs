using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.Verlet
{
    public class FBVerletShape : IVerletCollidable
    {
        public List<FBVerletPoint> points;
        public List<FBVerletEdge> edges;
        public List<FBVerletEdge> collisionEdges;

        public Vector2 Center
        {
            get
            {
                Vector2 center = Vector2.Zero;
                foreach (var p in points)
                {
                    center += p.Position;
                }
                return new Vector2(center.X / points.Count, center.Y / points.Count);
            }
        }

        public FBVerletShape()
        {
            points = new List<FBVerletPoint>();
            edges = new List<FBVerletEdge>();
        }

        public void Accelerate(Vector2 acceleration)
        {
            foreach (var point in points)
                point.Acceleration = acceleration;
        }

        public List<FBVerletEdge> CollisionAxes(IVerletCollidable otherShape)
        {
            return collisionEdges;
        }

        public List<FBVerletPoint> CollisionPoints()
        {
            return points;
        }

        public Vector2 NearestPoint(Vector2 to)
        {
            throw new NotImplementedException();
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(float timestep)
        {
            foreach(var point in points)
            {
                var p = point.Position;
                var movement = (point.Position - point.OldPosition) + point.Acceleration * timestep * timestep;
                point.Position += movement;
                point.OldPosition = p;
            }

            foreach(var edge in edges)
            {
                edge.Constrain();
            }
            
            foreach(var edge in collisionEdges)
            {
                edge.Constrain();
            }
        }
    }
}
