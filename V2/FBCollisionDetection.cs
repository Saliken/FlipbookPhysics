using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBCollision
    {
        public Vector2 seperatingVector;
        public FBCollider collidedWith;
    }
    public class FBAxisInfo
    {
        public Vector2 collisionRange;
        public Vector2 lessCollisionRange;
        public Vector2 AxisDirection;
        public Vector2 Normal;
        public float MTVAmount;
        public Vector2 MTVAxis;
    }
    public static class FBCollisionDetection
    {

        public static bool CollidesWith(this FBCollider a, FBCollider b)
        {
            return CollidesWith(a, b, null);
        }
        public static bool CollidesWith(this FBCollider a, FBCollider b, Action<FBCollision> onCollision)
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

            var collision = new FBCollision()
            {
                seperatingVector = mtv * mtvDistance,
                collidedWith = b
            };

            onCollision?.Invoke(collision);

            return true;
        }

        public static bool WillCollideWith(this FBCollider a, Vector2 aMovement, FBCollider b, Vector2 bMovement, out FBCollisionMovementInformation aMovementInfo, out FBCollisionMovementInformation bMovementInfo, bool ccd)
        {
            var axes = a.CollisionAxes(b);
            axes.AddRange(b.CollisionAxes(a));

            aMovementInfo = null;
            bMovementInfo = null;

            FBAxisInfo aInfo = new FBAxisInfo();
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
                    {
                        aInfo.AxisDirection = axis;
                    }

                    aInfo.collisionRange = new Vector2(rangeMin, rangeMax);
                    if (rangeMin == begin)
                        aInfo.lessCollisionRange = new Vector2(lessBegin, rangeMax);
                }
            }
            if (currentlyColliding) //currently colliding
            {
                //use mtv
                var aMoveLength = Math.Abs(aMovement.LengthSquared());
                var bMoveLength = Math.Abs(bMovement.LengthSquared());

                var totalMovement = aMoveLength + bMoveLength;
                double aMovePercent, bMovePercent;
                if (totalMovement != 0)
                {
                    aMovePercent = aMoveLength / totalMovement;
                    bMovePercent = bMoveLength / totalMovement;
                }
                else
                {
                    aMovePercent = 0.5f;
                    bMovePercent = 0.5f;
                }

                var aSeparation = aInfo.MTVAxis * ((aInfo.MTVAmount * (float)aMovePercent) + 1);
                var bSeparation = aInfo.MTVAxis * ((-aInfo.MTVAmount * (float)bMovePercent) - 1);

                aMovementInfo = new FBCollisionMovementInformation()
                {
                    ValidMovement = aSeparation,
                    InteriorCollision = true
                };
                bMovementInfo = new FBCollisionMovementInformation()
                {
                    ValidMovement = bSeparation,
                    InteriorCollision = true
                };
                return true;
            }
            else if (willCollide) //Will collide
            {
                //The last axis that was collided with causing the collision to actually occur.
                var collisionAxis = aInfo.AxisDirection;

                //The inverse of the collision axis, the allowed movement if blocked.
                var remainderAxis = new Vector2(-aInfo.AxisDirection.Y, aInfo.AxisDirection.X);

                //Get the valid movement for the collision and leftover.
                var aMovementTilCollision = aMovement * aInfo.lessCollisionRange.X;
                var bMovementTilCollision = bMovement * aInfo.lessCollisionRange.X;
                var aInitialMovementRemainder = aMovement - aMovementTilCollision;
                var bInitialMovementRemainder = bMovement - bMovementTilCollision;
                var aMovementRemainderAlongAxis = Vector2.Dot(aInitialMovementRemainder, remainderAxis) * remainderAxis;
                var bMovementRemainderAlongAxis = Vector2.Dot(bInitialMovementRemainder, remainderAxis) * remainderAxis;

                var aMovementRemainder = aMovementRemainderAlongAxis;
                var bMovementRemainder = bMovementRemainderAlongAxis;

                //Determine if an object is allowed to continue moving.
                var aCenter = Vector2.Dot(a.Position, collisionAxis);
                var aVel = Vector2.Dot(aMovement, collisionAxis);
                var bCenter = Vector2.Dot(b.Position, collisionAxis);
                var bVel = Vector2.Dot(bMovement, collisionAxis);

                if (aVel > 0 && bVel > 0)
                {
                    if (aCenter > bCenter)
                    {
                        aMovementRemainder = aMovement - aMovementTilCollision;
                    }
                    else
                    {
                        bMovementRemainder = bMovement - bMovementTilCollision;
                    }
                }
                else if (aVel < 0 && bVel < 0)
                {
                    if (aCenter < bCenter)
                    {
                        aMovementRemainder = aMovement - aMovementTilCollision;
                    }
                    else
                    {
                        bMovementRemainder = bMovement - bMovementTilCollision;
                    }
                }

                var aMove = aMovementTilCollision + aMovementRemainder;
                var bMove = bMovementTilCollision + bMovementRemainder;

                var aReflectedMovement = Vector2.Reflect(aMovementTilCollision, aInfo.AxisDirection);
                var bReflectedMovement = Vector2.Reflect(bMovementTilCollision, aInfo.AxisDirection);

                aMovementInfo = new FBCollisionMovementInformation()
                {
                    ValidMovement = aMovementTilCollision,
                    ValidMovementPercent = aInfo.lessCollisionRange.X,
                    RemainderAxis = remainderAxis,
                    RemainderAxisMovement = aMovementRemainder,
                    ReflectedMovement = aReflectedMovement
                };
                bMovementInfo = new FBCollisionMovementInformation()
                {
                    ValidMovement = bMovementTilCollision,
                    ValidMovementPercent = aInfo.lessCollisionRange.X,
                    RemainderAxis = remainderAxis,
                    RemainderAxisMovement = bMovementRemainder,
                    ReflectedMovement = bReflectedMovement
                };
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

            if (aMovement != bMovement)
                //Get approximately a pixel distance in order to keep objects separated.
                //lessBegin = beginC - Math.Abs(0.85f / (aMovement - bMovement));
                lessBegin = beginC;
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
            ///Formula
            ///T = D / R
            ///T = Time
            ///D = Distance
            ///R = Rate

            float R = 0;
            float D = Math.Abs(a - b);

            //Points moving in the same direction.
            if (aMovement >= 0 && bMovement >= 0 || aMovement < 0 && bMovement < 0)
            {
                R = Math.Abs(aMovement - bMovement);
            }
            else
            {
                R = Math.Abs(aMovement + bMovement);
            }

            float T = D / R;
            if (T < 0)
                T = 0;
            if (T > 1)
                T = 1;

            return T;
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
