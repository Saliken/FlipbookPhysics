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
                    collision.A.BeforeCollision(collision.CollisionInfo);
                    collision.B.BeforeCollision(collision.CollisionInfo);

                    collision.A.SetMove(collision.CollisionInfo.AMovement.X, collision.CollisionInfo.AMovement.Y);
                    collision.B.SetMove(collision.CollisionInfo.BMovement.X, collision.CollisionInfo.BMovement.Y);

                    collision.A.AfterCollision(collision.CollisionInfo);
                    collision.B.AfterCollision(collision.CollisionInfo);
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
                    collision.A.BeforeCollision(collision.CollisionInfo);
                    collision.B.BeforeCollision(collision.CollisionInfo);

                    collision.A.Position += collision.CollisionInfo.AMovement;
                    collision.A.SetMove(collision.CollisionInfo.ARemainderAxisMovement.X, collision.CollisionInfo.ARemainderAxisMovement.Y);
                    collision.B.Position += collision.CollisionInfo.BMovement;
                    collision.B.SetMove(collision.CollisionInfo.BRemainderAxisMovement.X, collision.CollisionInfo.BRemainderAxisMovement.Y);

                    collision.A.AfterCollision(collision.CollisionInfo);
                    collision.B.AfterCollision(collision.CollisionInfo);
                }
            }
        }

        private static CollisionInfo ConvertToCollisionInfo(FutureCollision collision, FBBody bodyA, FBBody bodyB)
        {
            var collisionInfo = new CollisionInfo()
            {
                BodyA = bodyA,
                BodyB = bodyB,
                AMovement = new MovementInfo()
                {
                    ValidMovement = collision.AMovement,
                    RemainderMovement = collision.ARemainderAxisMovement,
                    RemainderAxis = collision.RemainderAxis,
                    ReflectedMovement = collision.AReflectedMovement
                },
                BMovement = new MovementInfo()
                {
                    ValidMovement = collision.BMovement,
                    RemainderMovement = collision.BRemainderAxisMovement,
                    RemainderAxis = collision.RemainderAxis,
                    ReflectedMovement = collision.BReflectedMovement
                }
            };
            return collisionInfo;
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
        private static List<FBPotentialCollisionPair> FilterEarliestPairs(List<FBPotentialCollisionPair> pairs)
        {
            //First sort
            var ordered = pairs.OrderBy(x => x.CollisionInfo.CollisionBeginning);

            List<FBPotentialCollisionPair> earliestPairs = new List<FBPotentialCollisionPair>();
            List<FBBody> completedPairs = new List<FBBody>();
            
            foreach(var pair in ordered)
            {
                if(!completedPairs.Contains(pair.A) && !completedPairs.Contains(pair.B))
                { 
                    earliestPairs.Add(pair);
                    completedPairs.Add(pair.A);
                    completedPairs.Add(pair.B);
                }
            }
            return earliestPairs;
        }
        private static List<FBPotentialCollisionPair> GetCollisions()
        {
            var collisions = new List<FBPotentialCollisionPair>();
            foreach (var pair in GetPairs())
            {
                FutureCollision collisionInfo;
                if (pair.A.collider.WillCollideWith(pair.A.Movement, pair.B.collider, pair.B.Movement, out collisionInfo, CCD))
                {
                    pair.CollisionInfo = collisionInfo;
                    collisions.Add(pair);
                }
            }
            return collisions;
        }
        private static List<FBPotentialCollisionPair> GetPairs()
        {
            var pairs = new List<FBPotentialCollisionPair>();
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
                            pairs.Add(new FBPotentialCollisionPair() { A = bodies[i], B = bodies[j] });
                        }
                    }
                }
            }

            return pairs;
        }
    }
}
