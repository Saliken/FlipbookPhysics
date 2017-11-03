﻿using Microsoft.Xna.Framework;
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
        public FBCollider collidedWith;
    }

    public class FutureCollision
    {
        public bool DidCollide;
        public Vector2 RemainderAxis;
        public Vector2 AMovement;
        public Vector2 BMovement;
        public Vector2 ARemainder;
        public Vector2 BRemainder;
        public Vector2 ARemainderAxisMovement;
        public Vector2 BRemainderAxisMovement;
        public Vector2 AReflectedAxis;
        public Vector2 BReflectedAxis;
        public Vector2 AReflectedMovement;
        public Vector2 BReflectedMovement;
        public float CollisionBeginning;
    }
    public class AxisInfo
    {
        public Vector2 collisionRange;
        public Vector2 lessCollisionRange;
        public Vector2 AxisDirection;
        public Vector2 Normal;
        public float MTVAmount;
        public Vector2 MTVAxis;
    }
    public static class ColliderChecker
    {

        public static bool CollidesWith(this FBCollider a, FBCollider b)
        {
            return CollidesWith(a, b, null);
        }
        public static bool CollidesWith(this FBCollider a, FBCollider b, Action<Collision> onCollision)
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

        public static bool WillCollide(FBCollider a, Vector2 aMovement, FBCollider b, Vector2 bMovement, out FutureCollision collision)
        {
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            collision = new FutureCollision();
            collision.DidCollide = false;

            AxisInfo aInfo = new AxisInfo();
            aInfo.MTVAmount = float.MaxValue;
            aInfo.collisionRange.X = float.MinValue;
            aInfo.collisionRange.Y = float.MaxValue;

            bool currentlyColliding = true;
            bool willCollide = true;

            foreach (var axis in axes)
            {
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                if (currentlyColliding)
                {
                    var intervalDistance = IntervalDistance(aMin, aMax, bMin, bMax);
                    if (intervalDistance < 0) //Currently colliding, save mtv.
                    {
                        if ((intervalDistance = Math.Abs(intervalDistance)) < aInfo.MTVAmount)
                        {
                            aInfo.MTVAmount = intervalDistance;
                            aInfo.MTVAxis = axis;
                        }
                    }
                    else
                    {
                        currentlyColliding = false;
                    }
                }

                if (willCollide)
                {
                    var bMovementProjection = Vector2.Dot(axis, bMovement);
                    var aMovementProjection = Vector2.Dot(axis, aMovement);

                    float begin, end, lessBegin;
                    if (!CalcCollisionBeginAndEnd(aMin, aMax, aMovementProjection, bMin, bMax, bMovementProjection, out begin, out end, out lessBegin))
                    {
                        willCollide = false;
                    }

                    var rangeMin = begin > aInfo.collisionRange.X ? begin : aInfo.collisionRange.X;
                    var rangeMax = end < aInfo.collisionRange.Y ? end : aInfo.collisionRange.Y;

                    if (rangeMin > rangeMax)
                    {
                        willCollide = false;
                    }

                    if (aInfo.collisionRange.X < rangeMin)
                        aInfo.AxisDirection = axis;

                    aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
                    if (rangeMin == begin)
                        aInfo.lessCollisionRange = new Vector2(lessBegin, rangeMax);
                }
            }
            if (currentlyColliding) //currently colliding
            {
                //use mtv
                collision.DidCollide = true;
                collision.AMovement = aInfo.MTVAxis * (aInfo.MTVAmount / 2);
                collision.BMovement = aInfo.MTVAxis * (-aInfo.MTVAmount / 2);
                collision.ARemainder = Vector2.Zero;
                collision.BRemainder = Vector2.Zero;
                collision.ARemainderAxisMovement = Vector2.Zero;
                collision.BRemainderAxisMovement = Vector2.Zero;
                collision.CollisionBeginning = 0;
                return true;
            }
            else if (willCollide) //Will collide
            {
                var aFinalMovement = aMovement * aInfo.lessCollisionRange.X;
                var bFinalMovement = bMovement * aInfo.lessCollisionRange.X;

                var aMoveRemainder = aMovement - aFinalMovement;
                var bMoveRemainder = bMovement - bFinalMovement;

                var remainderAxis = new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X);

                var aRemainderAmount = Vector2.Dot(aMoveRemainder, remainderAxis);
                var bRemainderAmount = Vector2.Dot(bMoveRemainder, remainderAxis);

                var aRemainderMovement = (aRemainderAmount * remainderAxis);
                var bRemainderMovement = (bRemainderAmount * remainderAxis);

                var aMove = aFinalMovement + aRemainderMovement;
                var bMove = bFinalMovement + bRemainderMovement;

                collision.DidCollide = true;
                collision.AMovement = aFinalMovement;
                collision.BMovement = bFinalMovement;
                collision.ARemainder = aMoveRemainder;
                collision.BRemainder = bMoveRemainder;
                collision.RemainderAxis = remainderAxis;
                collision.ARemainderAxisMovement = aRemainderMovement;
                collision.BRemainderAxisMovement = bRemainderMovement;
                collision.CollisionBeginning = aInfo.lessCollisionRange.X;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool WillCollideWith(this FBCollider a, Vector2 aMovement, FBCollider b, Vector2 bMovement, out FutureCollision collision, bool ccd)
        {
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            collision = new FutureCollision();
            collision.DidCollide = false;

            AxisInfo aInfo = new AxisInfo();
            aInfo.MTVAmount = float.MaxValue;
            aInfo.collisionRange.X = float.MinValue;
            aInfo.collisionRange.Y = float.MaxValue;

            bool currentlyColliding = true;
            bool willCollide = ccd;

            foreach (var axis in axes)
            {
                float aMin, aMax, bMin, bMax;
                a.Project(axis, out aMin, out aMax);
                b.Project(axis, out bMin, out bMax);

                if (currentlyColliding)
                {
                    var intervalDistance = IntervalDistance(aMin, aMax, bMin, bMax);
                    if (intervalDistance < 0) //Currently colliding, save mtv.
                    {
                        if ((intervalDistance = Math.Abs(intervalDistance)) < aInfo.MTVAmount)
                        {
                            aInfo.MTVAmount = intervalDistance;
                            aInfo.MTVAxis = axis;
                        }
                    }
                    else
                    {
                        currentlyColliding = false;
                    }
                }

                if (willCollide)
                {
                    var bMovementProjection = Vector2.Dot(axis, bMovement);
                    var aMovementProjection = Vector2.Dot(axis, aMovement);

                    float begin, end, lessBegin;
                    if (!CalcCollisionBeginAndEnd(aMin, aMax, aMovementProjection, bMin, bMax, bMovementProjection, out begin, out end, out lessBegin))
                    {
                        willCollide = false;
                    }

                    var rangeMin = begin > aInfo.collisionRange.X ? begin : aInfo.collisionRange.X;
                    var rangeMax = end < aInfo.collisionRange.Y ? end : aInfo.collisionRange.Y;
                    
                    if (rangeMin > rangeMax)
                    {
                        willCollide = false;
                    }

                    if (aInfo.collisionRange.X < rangeMin)
                        aInfo.AxisDirection = axis;

                    aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
                    if (rangeMin == begin)
                        aInfo.lessCollisionRange = new Vector2(lessBegin, rangeMax);
                }
            }
            if (currentlyColliding) //currently colliding
            {
                //use mtv
                collision.DidCollide = true;
                collision.AMovement = aInfo.MTVAxis * ((aInfo.MTVAmount / 2) + 1);
                collision.BMovement = aInfo.MTVAxis * ((-aInfo.MTVAmount / 2) - 1);
                collision.ARemainder = Vector2.Zero;
                collision.BRemainder = Vector2.Zero;
                collision.ARemainderAxisMovement = Vector2.Zero;
                collision.BRemainderAxisMovement = Vector2.Zero;
                collision.CollisionBeginning = 0;
                return true;
            }
            else if(willCollide) //Will collide
            {
                var aFinalMovement = aMovement * aInfo.lessCollisionRange.X;
                var bFinalMovement = bMovement * aInfo.lessCollisionRange.X;

                var aMoveRemainder = aMovement - aFinalMovement;
                var bMoveRemainder = bMovement - bFinalMovement;

                var remainderAxis = new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X);

                var aRemainderAmount = Vector2.Dot(aMoveRemainder, remainderAxis);
                var bRemainderAmount = Vector2.Dot(bMoveRemainder, remainderAxis);

                var aRemainderMovement = (aRemainderAmount * remainderAxis);
                var bRemainderMovement = (bRemainderAmount * remainderAxis);

                var aMove = aFinalMovement + aRemainderMovement;
                var bMove = bFinalMovement + bRemainderMovement;

                var aReflectedMovement = Vector2.Reflect(aFinalMovement, aInfo.AxisDirection);
                var bReflectedMovement = Vector2.Reflect(bFinalMovement, aInfo.AxisDirection);
                

                collision.DidCollide = true;
                collision.AMovement = aFinalMovement;
                collision.BMovement = bFinalMovement;
                collision.ARemainder = aMoveRemainder;
                collision.BRemainder = bMoveRemainder;
                collision.RemainderAxis = remainderAxis;
                collision.ARemainderAxisMovement = aRemainderMovement;
                collision.BRemainderAxisMovement = bRemainderMovement;
                collision.CollisionBeginning = aInfo.lessCollisionRange.X;
                collision.AReflectedMovement = aReflectedMovement;
                collision.BReflectedMovement = bReflectedMovement;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool CalcCollisionBeginAndEnd(float aMin, float aMax, float aMovement, float bMin, float bMax, float bMovement, out float begin, out float end, out float lessBegin)
        {
            begin = -1;
            end = -1;
            lessBegin = -1;

            float beginC = 0, endC = 0;

            var aCenter = aMax - (aMax - aMin) / 2;
            var bCenter = bMax - (bMax - bMin) / 2;
            if (aMovement >= bMovement)
            {
                beginC = CalcCollisionTime(aMax, aMovement, bMin, bMovement);
                endC = CalcCollisionTime(aMin, aMovement, bMax, bMovement);
            }
            else 
            {
                beginC = CalcCollisionTime(aMin, aMovement, bMax, bMovement);
                endC = CalcCollisionTime(aMax, aMovement, bMin, bMovement);
            }

            if (beginC == 1 && endC == 1) //Never collides.
                return false;

            if (beginC == 0 && endC == 0) //Never collides
                return false;

            if(aMovement != bMovement)
                //Get approximately a pixel distance in order to keep objects separated.
                lessBegin = beginC - Math.Abs(0.85f / (aMovement - bMovement));
            else
            {
                lessBegin = beginC;
            }
            
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
