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
        }

        public static void AddMovedBody(FBBody body)
        {
            movedBodies.Add(body);
        }

        public static void Update(GameTime gameTime)
        {
            foreach(var body in movedBodies)
            {
                //Two step future SAT.
                //Step 1: Run SAT and determine the first object all objects will collide with, remove duplicates.
                //Step 2: Using the remainder of the velocity run future-SAT again, stopping at the first collision.
                
            }

            movedBodies.Clear();
        }
    }
}
