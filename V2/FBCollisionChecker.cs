using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBCollisionChecker<T> where T : FBBody
    {

        public int Iterations { get; set; }

        public FBCollision<T> GetCollision(T BodyA, T BodyB)
        {
            throw new NotImplementedException();
        }

        public List<FBCollision<T>> GetAllCollisions(List<T> bodies, FBSpatialHash<T> bodiesHashed)
        {
            var allPossibleCollisions = GetAllPossibleCollisions(bodies, bodiesHashed);
            var firstCollisions = FilterEarliestCollisions(allPossibleCollisions);
            return firstCollisions;
        }

        protected List<FBCollision<T>> FilterEarliestCollisions(List<FBCollision<T>> collisions)
        {
            var ordered = collisions.OrderBy(x => x.AMovement.ValidMovementPercent);

            List<FBCollision<T>> earliestCollisions = new List<FBCollision<T>>();
            List<FBBody> completedBodies = new List<FBBody>();

            foreach(var collision in ordered)
            {
                if(!completedBodies.Contains(collision.BodyA) && !completedBodies.Contains(collision.BodyB))
                {
                    earliestCollisions.Add(collision);
                    completedBodies.Add(collision.BodyA);
                    completedBodies.Add(collision.BodyB);
                }
            }

            return earliestCollisions;
        }

        protected List<FBCollision<T>> GetAllPossibleCollisions(List<T> bodies, FBSpatialHash<T> bodiesHashed)
        {
            List<FBCollision<T>> collisions = new List<FBCollision<T>>();

            var allPotentialCollisionPairs = GetPotentialCollisionPairs(bodies, bodiesHashed);
            foreach (var pair in allPotentialCollisionPairs)
            {
                if (pair.BodyA.Collider.WillCollideWith(pair.BodyA.Movement, pair.BodyB.Collider, pair.BodyB.Movement, out var bodyAMovementInfo, out var bodyBMovementInfo, true))
                {
                    var collision = new FBCollision<T>();
                    collision.BodyA = pair.BodyA;
                    collision.BodyB = pair.BodyB;
                    collision.AMovement = bodyAMovementInfo;
                    collision.BMovement = bodyBMovementInfo;

                    collisions.Add(collision);
                }
            }

            return collisions;
        }

        protected List<FBPotentialCollisionPair<T>> GetPotentialCollisionPairs(List<T> bodies, FBSpatialHash<T> bodiesHashed)
        {
            var pairs = new List<FBPotentialCollisionPair<T>>();

            foreach(var body in bodies)
            {
                var potentialCollidingBodies = GetPotentialCollidingBodies(body, bodiesHashed);
                foreach(var potentialCollidingBody in potentialCollidingBodies)
                {
                    pairs.Add(new FBPotentialCollisionPair<T>()
                    {
                        BodyA = body,
                        BodyB = potentialCollidingBody
                    });
                }   
            }

            return pairs;
        }

        protected List<T> GetPotentialCollidingBodies(T body, FBSpatialHash<T> bodiesHashed)
        {
            List<T> potentialBodies = new List<T>();

            var sweptAABB = GetSweptAABB(body);
            var potentialCollisionBodies = bodiesHashed.GetRectangle(sweptAABB);
            foreach(var potentialBody in potentialCollisionBodies)
            {
                if (potentialBody != body)
                    potentialBodies.Add(potentialBody);
            }

            return potentialBodies;
        }

        protected Rectangle GetSweptAABB(IFBBody body)
        {
            var x = body.AABB.Left;
            var y = body.AABB.Top;
            var x2 = body.AABB.Right;
            var y2 = body.AABB.Bottom;

            if (body.Movement.X >= 0)
                x2 += (int)body.Movement.X;
            else
                x -= (int)body.Movement.X;

            if (body.Movement.Y >= 0)
                y2 += (int)body.Movement.Y;
            else
                y -= (int)body.Movement.Y;

            var sweptAABB = new Rectangle(x, y, x2 - x, y2 - y);
            return sweptAABB;
        }
    }
}
