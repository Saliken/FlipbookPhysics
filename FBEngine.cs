﻿using Microsoft.Xna.Framework;
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
        public static bool move = false;

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
            var collisions = GetCollisions();
            var earliestCollisions = FilterEarliestPairs(collisions);

            foreach (var collision in earliestCollisions)
            {
                

                if (move)
                {
                    collision.A.BeforeCollision(collision.CollisionInfo);
                    collision.B.BeforeCollision(collision.CollisionInfo);

                    collision.A.position += collision.CollisionInfo.AMovement;
                    collision.A.SetMove(collision.CollisionInfo.ARemainderAxisMovement.X, collision.CollisionInfo.ARemainderAxisMovement.Y);
                    collision.B.position += collision.CollisionInfo.BMovement;
                    collision.B.SetMove(collision.CollisionInfo.BRemainderAxisMovement.X, collision.CollisionInfo.BRemainderAxisMovement.Y);

                    collision.A.AfterCollision(collision.CollisionInfo);
                    collision.B.AfterCollision(collision.CollisionInfo);
                }

                
            }

            //Run again to resolve remainder movement.
            collisions.Clear();
            collisions = GetCollisions();

            earliestCollisions.Clear();
            earliestCollisions = FilterEarliestPairs(collisions);
            foreach(var collision in earliestCollisions)
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

        private static void MoveBodies()
        {
            foreach (var body in bodies)
            {
                if (move)
                {
                    body.position += new Vector2(body.MoveX, body.MoveY);
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
                if (pair.A.collider.WillCollideWith(pair.A.Movement, pair.B.collider, pair.B.Movement, out collisionInfo))
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
