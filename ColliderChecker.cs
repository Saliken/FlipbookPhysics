using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class Collision
    {
        public Vector2 seperatingVector;
        public FBShape collidedWith;
    }

    public class FutureCollision
    {
        public Vector2 movement;
        public Vector2 collisionPoint;
        public Vector2 collisionFaceDirection;
        public Vector2 reflectedDirection;
    }
    public class AxisInfo
    {
        public Vector2 collisionRange;
        public Vector2 AxisDirection;
        public Vector2 Normal;
    }
    public static class ColliderChecker
    {

        public static bool CollidesWith(this FBShape a, FBShape b)
        {
            return CollidesWith(a, b, null);
        }
        public static bool CollidesWith(this FBShape a, FBShape b, Action<Collision> onCollision)
        {
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            Vector2 mtv = Vector2.Zero;
            float mtvDistance = float.MaxValue;
            foreach (var axis in axes)
            {
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
                }
            }

            var collision = new Collision()
            {
                seperatingVector = mtv * mtvDistance,
                collidedWith = b
            };

            onCollision?.Invoke(collision);

            return true;
        }

        public static bool CollidesWith(this FBShape a, FBShape b, Action<Collision> onCollision, Vector2 movement)
        {
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            Vector2 mtv = Vector2.Zero;
            float mtvDistance = float.MaxValue;
            bool futureInterval = false;

            var willIntersectCount = 0;
            var intersectCount = 0;
            foreach (var axis in axes)
            {
                bool intersecting = true, willIntersect = true;
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                var intervalDistance = IntervalDistance(aMin, aMax, bMin, bMax);
                if (intervalDistance > 0)
                    intersecting = false;

                var movementProjection = Vector2.Dot(movement, axis);
                if (movementProjection < 0)
                    aMin += movementProjection;
                else
                    aMax += movementProjection;

                intervalDistance = IntervalDistance(aMin, aMax, bMin, bMax);
                if (intervalDistance > 0)
                    willIntersect = false;

                if (intersecting)
                {
                    if (futureInterval != true)
                    {
                        if ((intervalDistance = Math.Abs(intervalDistance)) < mtvDistance)
                        {
                            mtv = axis;
                            mtvDistance = intervalDistance;
                        }
                    }

                    intersectCount++;
                }
                else if (willIntersect)
                {
                    //If we reach this it means that there did exist an axis of seperation..
                    //..but now there does not, which implies this axis is the axis it collides with..
                    //..so treat this axis as more important (for tunneling prevention).
                    if (futureInterval == false)
                    {
                        futureInterval = true;
                        mtvDistance = float.MaxValue;
                    }
                    willIntersectCount++;
                    if ((intervalDistance = Math.Abs(intervalDistance)) < mtvDistance)
                    {
                        mtv = axis;
                        mtvDistance = intervalDistance;
                    }
                }
                else
                {
                    return false; //axis of seperation.
                }
            }

            var collision = new Collision()
            {
                seperatingVector = ValidateAxis(a.Position - b.Position, mtv) * mtvDistance,
                collidedWith = b
            };

            onCollision?.Invoke(collision);

            return true;
        }
        public static bool WillCollideWith(this FBShape a, FBShape b, Vector2 movement, out Vector2 validMovement)
        {
            validMovement = movement;
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            AxisInfo aInfo = new AxisInfo();
            aInfo.collisionRange.X = float.MinValue;
            aInfo.collisionRange.Y = float.MaxValue;
            foreach (var axis in axes)
            {
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                var movementProjection = Vector2.Dot(axis, movement);
                if (movementProjection < 0)
                {
                    var tFirst = aMin + movementProjection;
                    var tLast = aMax + movementProjection;

                    if (aMax < bMin) //Moving away from a future collision
                        return false;

                    //determine the aMax point where we begin colliding and end colliding.
                    float hitFirst, hitLast;
                    if (tFirst < bMax)
                    {
                        if (aMin < bMax)
                            hitFirst = aMin;
                        else
                            hitFirst = bMax;
                    }
                    else
                        return false;

                    if (tLast < bMin)
                    {
                        hitLast = bMin - (aMax - aMin);
                    }
                    else
                    {
                        hitLast = tFirst;
                    }

                    //determine at what percent of the velocity's movement we are colliding.
                    var vN = aMin;
                    var hitFirstP = Math.Abs((hitFirst - vN) / (tFirst - vN));
                    var hitLastP = Math.Abs((hitLast - vN) / (tFirst - vN));

                    var rangeMin = hitFirstP > aInfo.collisionRange.X ? hitFirstP : aInfo.collisionRange.X;
                    var rangeMax = hitLastP < aInfo.collisionRange.Y ? hitLastP : aInfo.collisionRange.Y;

                    if (rangeMin > rangeMax)
                        return false;
                    if (aInfo.collisionRange.X < rangeMin)
                        aInfo.AxisDirection = axis;

                    aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
                }
                else
                {
                    var tFirst = aMax + movementProjection;
                    var tLast = aMin + movementProjection;

                    if (aMin > bMax) //Moving away from a future collision
                        return false;

                    //determine the aMax point where we begin colliding and end colliding.
                    float hitFirst, hitLast;
                    if (tFirst > bMin)
                    {
                        if (aMax > bMin)
                            hitFirst = aMax;
                        else
                            hitFirst = bMin;
                    }
                    else
                        return false;

                    if (tLast > bMax)
                    {
                        hitLast = bMax + (aMax - aMin);
                    }
                    else
                    {
                        hitLast = tFirst;
                    }

                    //determine at what percent of the velocity's movement we are colliding.
                    var vN = aMax;
                    var hitFirstP = Math.Abs((hitFirst - vN) / (tFirst - vN));
                    var hitLastP = Math.Abs((hitLast - vN) / (tFirst - vN));

                    //get new intersecting range
                    var rangeMin = hitFirstP > aInfo.collisionRange.X ? hitFirstP : aInfo.collisionRange.X;
                    var rangeMax = hitLastP < aInfo.collisionRange.Y ? hitLastP : aInfo.collisionRange.Y;

                    if (rangeMin > rangeMax)
                        return false;
                    if (aInfo.collisionRange.X < rangeMin)
                        aInfo.AxisDirection = axis;

                    aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
                }
            }



            //If we got here then we have a future collision.
            var movementTilCollision = (movement * (aInfo.collisionRange.X - 0.5f));
            var movementRemainder = (movement - movementTilCollision);
            var movementAmount = Vector2.Dot(movementRemainder, new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));

            validMovement = (movementTilCollision) + (movementAmount * new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));
            return true;
        }
        public static bool WillCollideWith(this FBShape a, Vector2 movement, FBShape b, Vector2 bMovement, out Vector2 validMovement)
        {
            validMovement = movement;
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            AxisInfo aInfo = new AxisInfo();
            aInfo.collisionRange.X = float.MinValue;
            aInfo.collisionRange.Y = float.MaxValue;
            foreach (var axis in axes)
            {
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                var movementProjection = Vector2.Dot(axis, movement);
                if (movementProjection < 0)
                {
                    var tFirst = aMin + movementProjection;
                    var tLast = aMax + movementProjection;

                    if (aMax < bMin) //Moving away from a future collision
                        return false;

                    //determine the aMax point where we begin colliding and end colliding.
                    float hitFirst, hitLast;
                    if (tFirst < bMax)
                    {
                        if (aMin < bMax)
                            hitFirst = aMin;
                        else
                            hitFirst = bMax;
                    }
                    else
                        return false;

                    if (tLast < bMin)
                    {
                        hitLast = bMin - (aMax - aMin);
                    }
                    else
                    {
                        hitLast = tFirst;
                    }

                    //determine at what percent of the velocity's movement we are colliding.
                    var vN = aMin;
                    var hitFirstP = Math.Abs((hitFirst - vN) / (tFirst - vN));
                    var hitLastP = Math.Abs((hitLast - vN) / (tFirst - vN));

                    var rangeMin = hitFirstP > aInfo.collisionRange.X ? hitFirstP : aInfo.collisionRange.X;
                    var rangeMax = hitLastP < aInfo.collisionRange.Y ? hitLastP : aInfo.collisionRange.Y;

                    if (rangeMin > rangeMax)
                        return false;
                    if (aInfo.collisionRange.X < rangeMin)
                        aInfo.AxisDirection = axis;

                    aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
                }
                else
                {
                    var tFirst = aMax + movementProjection;
                    var tLast = aMin + movementProjection;

                    if (aMin > bMax) //Moving away from a future collision
                        return false;

                    //determine the aMax point where we begin colliding and end colliding.
                    float hitFirst, hitLast;
                    if (tFirst > bMin)
                    {
                        if (aMax > bMin)
                            hitFirst = aMax;
                        else
                            hitFirst = bMin;
                    }
                    else
                        return false;

                    if (tLast > bMax)
                    {
                        hitLast = bMax + (aMax - aMin);
                    }
                    else
                    {
                        hitLast = tFirst;
                    }

                    //determine at what percent of the velocity's movement we are colliding.
                    var vN = aMax;
                    var hitFirstP = Math.Abs((hitFirst - vN) / (tFirst - vN));
                    var hitLastP = Math.Abs((hitLast - vN) / (tFirst - vN));

                    //get new intersecting range
                    var rangeMin = hitFirstP > aInfo.collisionRange.X ? hitFirstP : aInfo.collisionRange.X;
                    var rangeMax = hitLastP < aInfo.collisionRange.Y ? hitLastP : aInfo.collisionRange.Y;

                    if (rangeMin > rangeMax)
                        return false;
                    if (aInfo.collisionRange.X < rangeMin)
                        aInfo.AxisDirection = axis;

                    aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
                }
            }



            //If we got here then we have a future collision.
            var movementTilCollision = (movement * (aInfo.collisionRange.X));
            var movementRemainder = (movement - movementTilCollision);
            var movementAmount = Vector2.Dot(movementRemainder, new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));

            validMovement = (movementTilCollision) + (movementAmount * new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));
            return true;
        }

        public static bool WillCollideWith2(this FBShape a, Vector2 movement, FBShape b, Vector2 bMovement, out Vector2 aValidMovement, out Vector2 bValidMovement)
        {
            aValidMovement = movement;
            bValidMovement = bMovement;
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            AxisInfo aInfo = new AxisInfo();
            aInfo.collisionRange.X = float.MinValue;
            aInfo.collisionRange.Y = float.MaxValue;
            foreach (var axis in axes)
            {
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                var bMovementProjection = Vector2.Dot(axis, bMovement);
                var aMovementProjection = Vector2.Dot(axis, movement);

                float begin, end;
                if (!CalcCollisionBeginAndEnd(aMin, aMax, aMovementProjection, bMin, bMax, bMovementProjection, out begin, out end))
                {
                    return false;
                }

                var rangeMin = begin > aInfo.collisionRange.X ? begin : aInfo.collisionRange.X;
                var rangeMax = end < aInfo.collisionRange.Y ? end : aInfo.collisionRange.Y;

                if (rangeMin > rangeMax)
                    return false;

                if (aInfo.collisionRange.X < rangeMin)
                    aInfo.AxisDirection = axis;

                aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
            }

            var aFinalMovement = movement * aInfo.collisionRange.X;
            var bFinalMovement = bMovement * aInfo.collisionRange.X;

            var aMoveRemainder = movement - aFinalMovement;
            var bMoveRemainder = bMovement - bFinalMovement;

            var aRemainderAmount = Vector2.Dot(aMoveRemainder, new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));
            var bRemainderAmount = Vector2.Dot(bMoveRemainder, new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));

            var aMove = aFinalMovement + (aRemainderAmount * new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));
            var bMove = bFinalMovement + (bRemainderAmount * new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X));

            aValidMovement = aMove;
            bValidMovement = bMove;
            return true;
        }



        private static bool CalcCollisionBeginAndEnd(float aMin, float aMax, float aMovement, float bMin, float bMax, float bMovement, out float begin, out float end)
        {
            begin = -1;
            end = -1;

            float beginC = 0, endC = 0;

            if (aMovement >= 0)
            {
                beginC = CalcCollisionTime(aMax, aMovement, bMin, bMovement);
                endC = CalcCollisionTime(aMin, aMovement, bMax, bMovement);
            }
            else if (aMovement < 0)
            {
                beginC = CalcCollisionTime(aMin, aMovement, bMax, bMovement);
                endC = CalcCollisionTime(aMax, aMovement, bMin, bMovement);
            }

            Console.WriteLine("Begin : " + beginC);
            Console.WriteLine("End : " + endC);
            Console.WriteLine();

            if (beginC == 1 && endC == 1) //Never collides.
                return false;

            if (beginC == 0 && endC == 0) //Never collides
                return false;

            begin = beginC;
            end = endC;

            return true;
        }

        private static float CalcCollisionTime(float a, float aMovement, float b, float bMovement)
        {
            var distance = b - a;
            if (distance > aMovement)
                distance = aMovement;

            float c;
            if (aMovement != bMovement)
                c = (b - a) * (1 / (aMovement - bMovement));
            else
                c = (b - a);

            Console.WriteLine(c);

            if (c < 0)
                c = 0;
            if (c > 1)
                c = 1;
            return c;

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
