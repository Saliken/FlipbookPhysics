using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBCollisionChecker
    {

        public int Iterations { get; set; }

        public FBCollision GetCollision(FBBody BodyA, FBBody BodyB)
        {
            throw new NotImplementedException();
        }

        public List<FBCollision> GetAllCollisions(List<FBBody> bodies, FBSpatialHash<FBBody> bodiesHashed)
        {
            var allPossibleCollisions = GetAllPossibleCollisions(bodies, bodiesHashed);
            var firstCollisions = FilterEarliestCollisions(allPossibleCollisions);
            return firstCollisions;
        }

        protected List<FBCollision> FilterEarliestCollisions(List<FBCollision> collisions)
        {
            var ordered = collisions.OrderBy(x => x.TimeOfImpact);

            List<FBCollision> earliestCollisions = new List<FBCollision>();
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

        protected List<FBCollision> GetAllPossibleCollisions(List<FBBody> bodies, FBSpatialHash<FBBody> bodiesHashed)
        {
            List<FBCollision> collisions = new List<FBCollision>();

            var allPotentialCollisionPairs = GetPotentialCollisionPairs(bodies, bodiesHashed);
            foreach (var pair in allPotentialCollisionPairs)
            {
                if(FBCollisionDetector.GetCollisionInformation(pair.BodyA, pair.BodyB, out var collisionInformation))
                {
                    collisions.Add(collisionInformation);
                }
                //if (pair.BodyA.Collider.WillCollideWith(pair.BodyA.MovementThisFrame, pair.BodyB.Collider, pair.BodyB.MovementThisFrame, out var bodyAMovementInfo, out var bodyBMovementInfo, true))
                //{
                //    var collision = new FBCollision();
                //    collision.BodyA = pair.BodyA;
                //    collision.BodyB = pair.BodyB;
                //    collision.AMovement = bodyAMovementInfo;
                //    collision.BMovement = bodyBMovementInfo;

                //    collisions.Add(collision);
                //}
            }

            return collisions;
        }

        protected List<FBPotentialCollisionPair> GetPotentialCollisionPairs(List<FBBody> bodies, FBSpatialHash<FBBody> bodiesHashed)
        {
            var pairs = new List<FBPotentialCollisionPair>();

            foreach(var body in bodies)
            {
                var potentialCollidingBodies = GetPotentialCollidingBodies(body, bodiesHashed);
                foreach(var potentialCollidingBody in potentialCollidingBodies)
                {
                    pairs.Add(new FBPotentialCollisionPair()
                    {
                        BodyA = body,
                        BodyB = potentialCollidingBody
                    });
                }   
            }

            return pairs;
        }

        protected List<FBBody> GetPotentialCollidingBodies(FBBody body, FBSpatialHash<FBBody> bodiesHashed)
        {
            List<FBBody> potentialBodies = new List<FBBody>();

            var sweptAABB = GetSweptAABB(body);
            var potentialCollisionBodies = bodiesHashed.GetRectangle(sweptAABB);
            foreach(var potentialBody in potentialCollisionBodies)
            {
                if (potentialBody != body)
                    potentialBodies.Add(potentialBody);
            }

            return potentialBodies;
        }

        protected Rectangle GetSweptAABB(FBBody body)
        {
            var x = body.AABB.Left;
            var y = body.AABB.Top;
            var x2 = body.AABB.Right;
            var y2 = body.AABB.Bottom;

            if (body.MovementThisFrame.X >= 0)
                x2 += (int)body.MovementThisFrame.X;
            else
                x -= (int)body.MovementThisFrame.X;

            if (body.MovementThisFrame.Y >= 0)
                y2 += (int)body.MovementThisFrame.Y;
            else
                y -= (int)body.MovementThisFrame.Y;

            var sweptAABB = new Rectangle(x, y, x2 - x, y2 - y);
            return sweptAABB;
        }
    }
}
