using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.Verlet
{
    public class FBVerletCollision
    {
        public FBVerletEdge collisionEdge;
        public FBVerletPoint collisionPoint;
        public IVerletCollidable collisionEdgeShape;
        public Vector2 mtv;
        public float mtvAmount;
    }
    public static class FBVerletCollisionChecker
    {
        public static bool CollidesWith(this IVerletCollidable a, IVerletCollidable b, out FBVerletCollision collisionResult)
        {
            collisionResult = null;

            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));
            
            FBVerletCollision collisionInfo = new FBVerletCollision();

            foreach(var edge in a.CollisionAxes(b))
            {
                var axis = edge.NormalLeft;
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                var intervalDistance = IntervalDistance(aMin, aMax, bMin, bMax);
                if (intervalDistance >= 0)
                    return false;

                if ((intervalDistance = Math.Abs(intervalDistance)) < collisionInfo.mtvAmount)
                {
                    collisionInfo.mtv = axis;
                    collisionInfo.mtvAmount = intervalDistance;
                    collisionInfo.collisionEdge = edge;
                    collisionInfo.collisionEdgeShape = a;
                }
            }

            foreach(var edge in b.CollisionAxes(a))
            {
                var axis = edge.NormalLeft;
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                var intervalDistance = IntervalDistance(aMin, aMax, bMin, bMax);
                if (intervalDistance >= 0)
                    return false;

                if ((intervalDistance = Math.Abs(intervalDistance)) < collisionInfo.mtvAmount)
                {
                    collisionInfo.mtv = axis;
                    collisionInfo.mtvAmount = intervalDistance;
                    collisionInfo.collisionEdge = edge;
                    collisionInfo.collisionEdgeShape = a;
                }
            }

            if(collisionInfo.collisionEdgeShape == b)
            {
                var temp = b;
                b = a;
                a = temp;
            }

            float minDistance = float.MaxValue;
            foreach(var point in a.CollisionPoints())
            {
                var distance = (collisionInfo.mtv * (point.Position - b.Center)).LengthSquared();
                if(distance < minDistance)
                {
                    minDistance = distance;
                    collisionInfo.collisionPoint = point;
                }
            }

            collisionResult = collisionInfo;
            return true;
        }

        private static Vector2 ValidateAxis(Vector2 direction, Vector2 axis)
        {
            if (Vector2.Dot(axis, direction) < 0)
                return -axis;

            return axis;
        }

        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
                return minB - maxA;
            else
                return minA - maxB;
        }
    }
}
