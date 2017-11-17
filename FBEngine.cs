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
        
        public static float Speed = 1f;
        public static CollisionCheckOrder Order;
        public static bool move = true;
        public static bool CCD = true;

        static FBEngine() { }

        public static void Initialize(Rectangle worldBoundaries)
        {
            bodies = new List<FBBody>();
            bodiesHash = new FBSpatialHash<FBBody>(100);
        }

        public static void AddBody(FBBody body)
        {
            bodies.Add(body);
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

                    if (collision.BodyA.type == FBBodyType.Dynamic)
                    {
                        collision.BodyA.Position += collision.AMovement.ValidMovement;
                        collision.BodyA.SetMove(collision.AMovement.RemainderAxisMovement);
                    }

                    if (collision.BodyB.type == FBBodyType.Dynamic)
                    {
                        collision.BodyB.Position += collision.BMovement.ValidMovement;
                        collision.BodyB.SetMove(collision.BMovement.RemainderAxisMovement);
                    }

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
                    if (body.type == FBBodyType.Dynamic)
                    {
                        body.Position += body.Movement;                            
                        body.SetMove(0, 0);
                    }
                }
            }
        }
        private static List<CollisionInfo> FilterEarliestPairs(List<CollisionInfo> pairs)
        {
            var ordered = pairs.OrderBy(x => x.AMovement.ValidMovementPercent);

            List<CollisionInfo> earliestPairs = new List<CollisionInfo>();
            List<FBBody> completedPairs = new List<FBBody>();
            
            foreach(var pair in ordered)
            {
                //TODO: Handle triggers as non-colliding.
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
                if (body.type == FBBodyType.Dynamic)
                {
                    pairs.AddRange(GetPairsForBody(body));
                }
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
                if(p != firstBody)
                    pairs.Add(new CollisionInfo() { BodyA = firstBody, BodyB = p });
            }
            return pairs;
        }
    }
}
