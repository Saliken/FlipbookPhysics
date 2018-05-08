using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBCollisionResolver
    {
        public void Resolve(FBCollision collision)
        {
            //var aVelocityModifier = collision.BodyA.Velocity - collision.BodyB.Velocity;
            //var bVelocityModifier = collision.BodyB.Velocity - collision.BodyA.Velocity;
            collision.BodyA.Position += collision.AMovement.ValidMovement;
            collision.BodyA.MovementThisFrame = collision.AMovement.ReflectedMovement;
            collision.BodyB.Position += collision.BMovement.ValidMovement;
            collision.BodyB.MovementThisFrame = collision.BMovement.ReflectedMovement;
            var aVel = collision.BodyA.Velocity;
            collision.BodyA.Velocity = collision.BodyB.Velocity;
            collision.BodyB.Velocity = aVel;
        }
    }
}
