using FlipbookPhysics.V2.CustomBodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2.CustomCollisionResolvers
{
    public class FBBasicCollisionResolver : IFBCollisionResolver<FBBasicBody>
    {
        public void Resolve(FBCollision<FBBasicBody> collision)
        {
            collision.BodyA.Position += collision.AMovement.ValidMovement;
            collision.BodyA.Movement = collision.AMovement.RemainderAxisMovement;
            collision.BodyB.Position += collision.BMovement.ValidMovement;
            collision.BodyB.Movement = collision.BMovement.RemainderAxisMovement;
        }
    }
}
