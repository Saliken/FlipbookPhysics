using Microsoft.Xna.Framework;
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
            if (!collision.DidCollide)
                return;

            //Steps
            //1. Project the velocity along the normal and tangential collision vectors
            //2. Use momentum formula to calculate new normal velocity
            //var aVelocityModifier = collision.BodyA.Velocity - collision.BodyB.Velocity;
            //var bVelocityModifier = collision.BodyB.Velocity - collision.BodyA.Velocity;

            //Step 1
            if(collision.CurrentCollision != null)
            {
                collision.BodyA.Position += collision.CurrentCollision.MTV;
                 collision.BodyB.Position -= collision.CurrentCollision.MTV;
            }
            else if(collision.FutureCollision != null)
            {
                var BodyANormalVelocity = Vector2.Dot(collision.BodyA.Velocity, collision.FutureCollision.ACollisionInfo.CollisionNormal);
                var BodyATangentVelocity = Vector2.Dot(collision.BodyA.Velocity, collision.FutureCollision.ACollisionInfo.CollisionTangent);
                var BodyBNormalVelocity = Vector2.Dot(collision.BodyB.Velocity, collision.FutureCollision.BCollisionInfo.CollisionNormal);
                var BodyBTangentVelocity = Vector2.Dot(collision.BodyB.Velocity, collision.FutureCollision.BCollisionInfo.CollisionTangent);

                var aVelAfter = (BodyANormalVelocity * 0 + 2 * BodyBNormalVelocity) / 2;
                var bVelAfter = (BodyBNormalVelocity * 0 + 2 * BodyANormalVelocity) / 2;

                var aVA = aVelAfter * collision.FutureCollision.ACollisionInfo.CollisionNormal;
                var aTA = BodyATangentVelocity * collision.FutureCollision.ACollisionInfo.CollisionTangent;
                var bVA = bVelAfter * collision.FutureCollision.BCollisionInfo.CollisionNormal;
                var bTA = BodyBTangentVelocity * collision.FutureCollision.BCollisionInfo.CollisionTangent;

                var aFinal = aVA + aTA;
                var bFinal = bVA + bTA;

                collision.BodyA.Position += collision.FutureCollision.AMovement.ValidMovement;
                collision.BodyB.Position += collision.FutureCollision.BMovement.ValidMovement;
                collision.BodyA.Velocity += aFinal;
                collision.BodyB.Velocity += -bFinal;


                //collision.BodyA.Position += collision.FutureCollision.AMovement.ValidMovement;
                //collision.BodyA.MovementThisFrame = collision.FutureCollision.AMovement.ReflectedMovement;
                //collision.BodyB.Position += collision.FutureCollision.BMovement.ValidMovement;
                //collision.BodyB.MovementThisFrame = collision.FutureCollision.BMovement.ReflectedMovement;
                //var aVel = collision.BodyA.Velocity;
                //collision.BodyA.Velocity = collision.BodyB.Velocity;
                //collision.BodyB.Velocity = aVel;
            }
        }
    }
}
