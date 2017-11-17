using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public static class FBEngine
    {
        public static List<FBBody> bodies;
        public static FBSpatialHash<FBBody> bodiesHash;
        
        public static List<FBBody> movedBodies;
        public static float Speed = 1f;
        public static CollisionCheckOrder Order;
        public static bool move = true;
        public static bool CCD = true;

        static FBEngine() { }

        public static void Initialize(Rectangle worldBoundaries)
        {
            bodiesHash = new FBSpatialHash<FBBody>(100);
            movedBodies = new List<FBBody>();
        }

        public static void AddBody(FBBody body)
        {
            bodies = new List<FBBody>();
        }

        public static void AddMovedBody(FBBody body)
        {
            movedBodies.Add(body);
        }

        public static void Update(GameTime gameTime)
        {
            FillSpatialHash();
            Iterate();
            Finish();
        }

        private static void FillSpatialHash()
        {
            bodiesHash.Clear();
            foreach(var body in bodies)
            {
                bodiesHash.Add(body, body.AABB);
            }
        }
        private static void Finish()
        {
            var collisions = GetCollisions();
            var earliestCollisions = FilterEarliestPairs(collisions);
            foreach (var collision in earliestCollisions)
            {
                if (move)
                {
                    collision.BodyA.BeforeCollision(collision);
                    collision.BodyB.BeforeCollision(collision);

                    collision.BodyA.SetMove(collision.AMovement.ValidMovement);
                    collision.BodyB.SetMove(collision.BMovement.ValidMovement);

                    collision.BodyA.AfterCollision(collision);
                    collision.BodyB.AfterCollision(collision);
                }
            }

            MoveBodies();
        }

        private static void Iterate()
        {
            var collisions = GetCollisions();
            var earliestCollisions = FilterEarliestPairs(collisions);

            foreach (var collision in earliestCollisions)
            {
                if (move)
                {
                    collision.BodyA.BeforeCollision(collision);
                    collision.BodyB.BeforeCollision(collision);

                    collision.BodyA.Position += collision.AMovement.ValidMovement;
                    collision.BodyA.SetMove(collision.AMovement.RemainderAxisMovement);
                    collision.BodyB.Position += collision.BMovement.ValidMovement;
                    collision.BodyB.SetMove(collision.BMovement.RemainderAxisMovement);

                    collision.BodyA.AfterCollision(collision);
                    collision.BodyB.AfterCollision(collision);
                }
            }
        }

        private static void MoveBodies()
        {
            foreach (var body in bodies)
            {
                if (move)
                {
                    body.Position += body.Movement;
                    body.SetMove(0, 0);
                }
            }
        }
        private static List<CollisionInfo> FilterEarliestPairs(List<CollisionInfo> pairs)
        {
            //First sort
            var ordered = pairs.OrderBy(x => x.AMovement.ValidMovementPercent);

            List<CollisionInfo> earliestPairs = new List<CollisionInfo>();
            List<FBBody> completedPairs = new List<FBBody>();
            
            foreach(var pair in ordered)
            {
                if(!completedPairs.Contains(pair.BodyA) && !completedPairs.Contains(pair.BodyB))
                { 
                    earliestPairs.Add(pair);
                    completedPairs.Add(pair.BodyA);
                    completedPairs.Add(pair.BodyB);
                }
            }
            return earliestPairs;
        }
        private static List<CollisionInfo> GetCollisions()
        {
            var collisions = new List<CollisionInfo>();
            foreach (var pair in GetPairs())
            {
                if (pair.BodyA.collider.WillCollideWith(pair.BodyA.Movement, pair.BodyB.collider, pair.BodyB.Movement, out var aMovementInfo, out var bMovementInfo, CCD))
                {
                    pair.AMovement = aMovementInfo;
                    pair.BMovement = bMovementInfo;

                    collisions.Add(pair);
                }
            }
            return collisions;
        }
        private static List<CollisionInfo> GetPairs()
        {
            var pairs = new List<CollisionInfo>();

            foreach(var body in bodies)
            {
                pairs.AddRange(GetPairsForBody(body));
            }

            return pairs;
        }

        private static List<CollisionInfo> GetPairsForBody(FBBody firstBody)
        {
            List<CollisionInfo> pairs = new List<CollisionInfo>();
            var x = firstBody.AABB.Left;
            var y = firstBody.AABB.Top;
            var x2 = firstBody.AABB.Right;
            var y2 = firstBody.AABB.Bottom;

            if (firstBody.MovementX >= 0)
                x2 += (int)firstBody.MovementX;
            else
                x -= (int)firstBody.MovementX;

            if (firstBody.MovementY >= 0)
                y2 += (int)firstBody.MovementY;
            else
                y -= (int)firstBody.MovementY;

            var sweptAABB = new Rectangle(x, y, x2 - x, y2 - y);

            var potentials = bodiesHash.GetRectangle(sweptAABB);
            foreach(var p in potentials)
            {
                pairs.Add(new CollisionInfo() { BodyA = firstBody, BodyB = p });
            }
            return pairs;
        }
    }
}
