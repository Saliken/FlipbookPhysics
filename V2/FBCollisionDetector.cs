using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBFutureAxisInfo
    {
        public Vector2 collisionRange;
        public Vector2 lessCollisionRange;
        public Vector2 axisDirection;
        public Vector2 normal;
        public bool Used;
    }

    public class FBCurrentAxisInfo
    {
        public float MTVAmount;
        public Vector2 MTVAxis;
        public bool Used;
    }
    public static class FBCollisionDetector
    {

        public static bool GetCollisionInformation(FBBody bodyA, FBBody bodyB, out FBCollision collisionInformation)
        {
            FBCurrentAxisInfo currentAxisInfo = new FBCurrentAxisInfo();
            FBFutureAxisInfo futureAxisInfo = new FBFutureAxisInfo();
            bool currentlyColliding = true, willCollide = true;

            collisionInformation = new FBCollision();
            collisionInformation.BodyA = bodyA;
            collisionInformation.BodyB = bodyB;
            collisionInformation.TimeOfImpact = 0;

            var allAxes = bodyA.Collider.CollisionAxes(bodyB.Collider);
            allAxes.AddRange(bodyB.Collider.CollisionAxes(bodyA.Collider));

            foreach(var axis in allAxes)
            {
                FBAxisProjection projection = new FBAxisProjection();
                bodyA.Collider.Project(axis, out projection.AMinimumValue, out projection.AMaximumValue);
                bodyB.Collider.Project(axis, out projection.BMinimumValue, out projection.BMaximumValue);

                if(currentlyColliding)
                    currentlyColliding = CheckCurrentlyColliding(projection, axis, currentAxisInfo);
                if(willCollide)
                    willCollide = CheckWillCollide(projection, axis, bodyA.MovementThisFrame, bodyB.MovementThisFrame, futureAxisInfo);
            }

            var didCollide = false;
            if (currentlyColliding)
            {
                collisionInformation.CurrentCollision = GetCurrentCollisionInformation(bodyA, bodyB, currentAxisInfo);
                didCollide = true;
            }
            else if (willCollide)
            {
                collisionInformation.FutureCollision = GetFutureCollisionInformation(bodyA, bodyB, futureAxisInfo);
                collisionInformation.TimeOfImpact = collisionInformation.FutureCollision.AMovement.ValidMovementPercent;
                didCollide = true;
            }

            collisionInformation.DidCollide = didCollide;
            return didCollide;
        }

        private static bool CheckCurrentlyColliding(FBAxisProjection projection, Vector2 axis, FBCurrentAxisInfo axisInfo)
        {
            var intervalDistance = projection.IntervalDistance();
            if(intervalDistance < 0) //Current colliding
            {
                if(!axisInfo.Used || (intervalDistance = Math.Abs(intervalDistance)) < axisInfo.MTVAmount)
                {
                    axisInfo.MTVAmount = intervalDistance;
                    axisInfo.MTVAxis = axis;
                    axisInfo.Used = true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool CheckWillCollide(FBAxisProjection projection, Vector2 axis, Vector2 aMovement, Vector2 bMovement, FBFutureAxisInfo axisInfo)
        {
            var aMovementProjection = Vector2.Dot(axis, aMovement);
            var bMovementProjection = Vector2.Dot(axis, bMovement);

            float begin, end, lessBegin;
            if(!CalcCollisionBeginAndEnd(projection.AMinimumValue, projection.AMaximumValue, aMovementProjection, projection.BMinimumValue, projection.BMaximumValue, bMovementProjection, out begin, out end, out lessBegin))
            {
                return false;
            }

            var rangeMin = begin > axisInfo.collisionRange.X || !axisInfo.Used ? begin : axisInfo.collisionRange.X;
            var rangeMax = end < axisInfo.collisionRange.Y || !axisInfo.Used ? end : axisInfo.collisionRange.Y;

            if(rangeMin > rangeMax)
            {
                return false;
            }

            if (axisInfo.collisionRange.X < rangeMin)
                axisInfo.axisDirection = axis;

            axisInfo.collisionRange = new Vector2(rangeMin, rangeMax);
            if (rangeMin == begin)
                axisInfo.collisionRange = new Vector2(lessBegin, rangeMax);

            return true;
        }
        private static FBFutureCollisionInfo GetFutureCollisionInformation(FBBody a, FBBody b, FBFutureAxisInfo axisInfo)
        {
            var collisionAxis = axisInfo.axisDirection;
            var tangentAxis = new Vector2(-collisionAxis.Y, collisionAxis.X);

            var aMovementTilCollision = a.MovementThisFrame * axisInfo.lessCollisionRange.X;
            var bMovementTilCollision = b.MovementThisFrame * axisInfo.lessCollisionRange.X;
            var aInitialMovementRemainder = a.MovementThisFrame - aMovementTilCollision;
            var bInitialMovementRemainder = b.MovementThisFrame - bMovementTilCollision;
            var aMovementRemainder = Vector2.Dot(aInitialMovementRemainder, tangentAxis) * tangentAxis;
            var bMovementRemainder = Vector2.Dot(bInitialMovementRemainder, tangentAxis) * tangentAxis;

            var aCenter = Vector2.Dot(a.Position, collisionAxis);
            var aVelocity = Vector2.Dot(a.MovementThisFrame, collisionAxis);
            var bCenter = Vector2.Dot(b.Position, collisionAxis);
            var bVelocity = Vector2.Dot(b.MovementThisFrame, collisionAxis);

            if(aVelocity > 0 && bVelocity > 0)
            {
                if (aCenter > bCenter)
                    aMovementRemainder = a.MovementThisFrame - aMovementTilCollision;
                else
                    bMovementRemainder = b.MovementThisFrame - bMovementTilCollision;
            }
            else
            {
                if (aCenter < bCenter)
                    aMovementRemainder = a.MovementThisFrame - aMovementTilCollision;
                else
                    bMovementRemainder = b.MovementThisFrame - bMovementTilCollision;
            }

            var aMovement = aMovementTilCollision + aMovementRemainder;
            var bMovement = bMovementTilCollision + bMovementRemainder;

            var aReflectedMovement = Vector2.Reflect(aMovementTilCollision, collisionAxis);
            var bReflectedMovement = Vector2.Reflect(bMovementTilCollision, collisionAxis);

            var futureCollisionInfo = new FBFutureCollisionInfo()
            {
                ACollisionInfo = new FBCollisionInformation()
                {
                    CollisionNormal = collisionAxis,
                    CollisionTangent = tangentAxis
                },
                AMovement = new FBCollisionMovementInformation()
                {
                    ValidMovement = aMovementTilCollision,
                    ValidMovementPercent = axisInfo.lessCollisionRange.X,
                    RemainderAxis = tangentAxis,
                    RemainderAxisMovement = aMovementRemainder,
                    ReflectedMovement = aReflectedMovement
                },
                BCollisionInfo = new FBCollisionInformation()
                {
                    CollisionNormal = new Vector2(-collisionAxis.X, -collisionAxis.Y),
                    CollisionTangent = new Vector2(-tangentAxis.X, -tangentAxis.Y)
                },
                BMovement = new FBCollisionMovementInformation()
                {
                    ValidMovement = bMovementTilCollision,
                    ValidMovementPercent = axisInfo.lessCollisionRange.X,
                    RemainderAxis = tangentAxis,
                    RemainderAxisMovement = bMovementRemainder,
                    ReflectedMovement = bReflectedMovement
                }
            };

            return futureCollisionInfo;
        }
        private static FBCurrentCollisionInfo GetCurrentCollisionInformation(FBBody a, FBBody b, FBCurrentAxisInfo axisInfo)
        {
            var aMoveLength = Math.Abs(a.MovementThisFrame.LengthSquared());
            var bMoveLength = Math.Abs(b.MovementThisFrame.LengthSquared());
            var combinedMovement = aMoveLength + bMoveLength;

            double aMovePercent, bMovePercent;
            if(combinedMovement != 0)
            {
                aMovePercent = aMoveLength / combinedMovement;
                bMovePercent = bMoveLength / combinedMovement;
            }
            else
            {
                aMovePercent = 0.5f;
                bMovePercent = 0.5f;
            }

            var aSeparation = axisInfo.MTVAxis * ((axisInfo.MTVAmount) + 1);
            var bSeparation = axisInfo.MTVAxis * ((-axisInfo.MTVAmount) - 1);

            return new FBCurrentCollisionInfo
            {
                MTV = aSeparation
            };
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
    }
}
