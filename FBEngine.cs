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
        public static List<FBBody> movedBodies;
        public static float Speed = 1f;
        public static CollisionCheckOrder Order;
        public static bool move = true;
        public static bool CCD = true;

        static FBEngine() { }

        public static void Initialize(Rectangle worldBoundaries)
        {
            bodies = new List<FBBody>();
            movedBodies = new List<FBBody>();
        }

        public static void AddMovedBody(FBBody body)
        {
            movedBodies.Add(body);
        }

        public static void Update(GameTime gameTime)
        {
            Iterate();
            Finish();
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
            for(int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i].Active)
                {
                    for (int j = i + 1; j < bodies.Count; j++)
                    {
                        if (i == j)
                            continue;

                        if (bodies[j].Active)
                        {
                            pairs.Add(new CollisionInfo() { BodyA = bodies[i], BodyB = bodies[j] });
                        }
                    }
                }
            }

            return pairs;
        }
    }
}
