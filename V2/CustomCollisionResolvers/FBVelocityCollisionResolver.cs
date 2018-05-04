using FlipbookPhysics.V2.CustomBodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2.CustomCollisionResolvers
{
    public class FBVelocityCollisionResolver : IFBCollisionResolver<FBVelocityBody>
    {
        public void Resolve(FBCollision<FBVelocityBody> collision)
        {
            var aVelocityModifier = collision.BodyA.Velocity - collision.BodyB.Velocity;
            var bVelocityModifier = collision.BodyB.Velocity - collision.BodyA.Velocity;
            collision.BodyA.Position += collision.AMovement.ValidMovement;
            collision.BodyA.MovementThisFrame = collision.AMovement.RemainderAxisMovement;
            collision.BodyB.Position += collision.BMovement.ValidMovement;
            collision.BodyB.MovementThisFrame = collision.BMovement.RemainderAxisMovement;
        }
    }
}
