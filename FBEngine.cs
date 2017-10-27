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
            //NOTE:
            //I should keep a variable on each body that stores their final movement once these iterations are complete.

            //Two steps.
            //First we need every pair of potential collisions to check for.
            var collisions = new List<FBPotentialCollisionPair>();
            foreach(var pair in GetPairs())
            {
                FutureCollision collisionInfo;
                if(pair.A.collider.WillCollideWith2(pair.A.Movement, pair.B.collider, pair.B.Movement, out collisionInfo))
                {
                    //This pair collided, keep track of it and it's collisionInfo;
                    pair.CollisionInfo = collisionInfo;
                    collisions.Add(pair);
                }
            }

            //Determine the set of collisions that are the earliest for each body.
            var earliestCollisions = FilterEarliestPairs(collisions);

            //Resolve the earliest collisions, queue up movement for the next iteration.
            foreach(var collision in earliestCollisions)
            {
                //Before collision

                collision.A.position += collision.CollisionInfo.AMovement;
                collision.A.SetMove(collision.CollisionInfo.ARemainderAxisMovement.X, collision.CollisionInfo.ARemainderAxisMovement.Y);
                collision.B.position += collision.CollisionInfo.BMovement;
                collision.B.SetMove(collision.CollisionInfo.BRemainderAxisMovement.X, collision.CollisionInfo.BRemainderAxisMovement.Y);

                //After collision
            }

            //Now do another iteration, this time resolve all
            collisions.Clear();
            foreach(var pair in GetPairs())
            {
                FutureCollision collisionInfo;
                if (pair.A.collider.WillCollideWith2(pair.A.Movement, pair.B.collider, pair.B.Movement, out collisionInfo))
                {
                    pair.CollisionInfo = collisionInfo;
                    collisions.Add(pair);
                }
            }

            earliestCollisions.Clear();
            earliestCollisions = FilterEarliestPairs(collisions);
            foreach(var collision in earliestCollisions)
            {
                collision.A.SetMove(collision.CollisionInfo.AMovement.X, collision.CollisionInfo.AMovement.Y);
                collision.B.SetMove(collision.CollisionInfo.BMovement.X, collision.CollisionInfo.BMovement.Y);
            }

            foreach(var body in bodies)
            {
                body.position += new Vector2(body.MoveX, body.MoveY);
            }

            movedBodies.Clear();
        }

        public static List<FBPotentialCollisionPair> FilterEarliestPairs(List<FBPotentialCollisionPair> pairs)
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

        public static List<FBPotentialCollisionPair> GetPairs()
        {
            var pairs = new List<FBPotentialCollisionPair>();
            for(int i = 0; i < bodies.Count; i++)
            {
                for(int j = i + 1; j < bodies.Count; j++)
                {
                    if (i == j)
                        continue;

                    pairs.Add(new FBPotentialCollisionPair() { A = bodies[i], B = bodies[j] });
                }
            }

            return pairs;
        }
    }
}
