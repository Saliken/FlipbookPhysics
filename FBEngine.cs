using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookCollision
{
    public static class FBEngine
    {
        public static FBQuadTree bodies;
        public static List<FBBody> movedBodies;
        public static float Speed = 1f;
        public static CollisionCheckOrder Order;

        static FBEngine() { }

        public static void Initialize(Rectangle worldBoundaries)
        {
            bodies = new FBQuadTree(worldBoundaries, 10, 3);
        }

        public static void AddMovedBody(FBBody body)
        {
            movedBodies.Add(body);
        }

        public static void Update(GameTime gameTime)
        {
            //Get a list of all moving objects.
            //Loop through that list, moving each object
            //Check for collisions, if they occur stop moving the object.
            foreach(var body in movedBodies)
            {
                //completely ignore tunneling, assume it's not a problem for now.
                //So all that would need to happen is to check against nearby bodies and resolve the collision.

            }

            movedBodies.Clear();
        }
    }
}
