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
        public Vector2 mtv;
        public float mtvAmount;
    }
    public static class FBVerletCollisionChecker
    {
        public static bool CollidesWith(this IVerletCollidable a, IVerletCollidable b)
        {
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            Vector2 mtv = Vector2.Zero;
            float mtvDistance = float.MaxValue;
            FBVerletEdge collisionEdge = null;

            foreach (var edge in axes)
            {
                var axis = edge.NormalLeft;
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                var intervalDistance = IntervalDistance(aMin, aMax, bMin, bMax);
                if (intervalDistance >= 0)
                    return false;

                if ((intervalDistance = Math.Abs(intervalDistance)) < mtvDistance)
                {
                    mtv = axis;
                    mtvDistance = intervalDistance;
                    collisionEdge = edge;
                }
            }

            var collision = new FBVerletCollision()
            {
                mtv = mtv,
                mtvAmount = mtvDistance,
                collisionEdge = collisionEdge
            };

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
