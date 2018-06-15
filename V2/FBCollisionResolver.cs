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
            
            if(collision.CurrentCollision != null)
            {
                collision.BodyA.Position -= collision.CurrentCollision.MTV / 2;
                collision.BodyB.Position += collision.CurrentCollision.MTV / 2;
            }
            else if(collision.FutureCollision != null)
            {
                var aVelocity = collision.BodyA.Velocity - collision.FutureCollision.AMovement.ValidMovement;
                var bVelocity = collision.BodyB.Velocity - collision.FutureCollision.BMovement.ValidMovement;
                var BodyANormalVelocity = Vector2.Dot(aVelocity, collision.FutureCollision.ACollisionInfo.CollisionNormal);
                var BodyATangentVelocity = Vector2.Dot(aVelocity, collision.FutureCollision.ACollisionInfo.CollisionTangent);
                var BodyBNormalVelocity = Vector2.Dot(bVelocity, collision.FutureCollision.BCollisionInfo.CollisionNormal);
                var BodyBTangentVelocity = Vector2.Dot(bVelocity, collision.FutureCollision.BCollisionInfo.CollisionTangent);

                var aVelAfter = (BodyANormalVelocity * 0 + 2 * BodyBNormalVelocity) / 2;
                var bVelAfter = (BodyBNormalVelocity * 0 + 2 * BodyANormalVelocity) / 2;

                var aVA = aVelAfter * collision.FutureCollision.ACollisionInfo.CollisionNormal;
                var aTA = BodyATangentVelocity * collision.FutureCollision.ACollisionInfo.CollisionTangent;
                var bVA = bVelAfter * collision.FutureCollision.BCollisionInfo.CollisionNormal;
                var bTA = BodyBTangentVelocity * collision.FutureCollision.BCollisionInfo.CollisionTangent;

                var aFinal = aVA + aTA;
                var bFinal = bVA + bTA;

                var aMove = collision.FutureCollision.AMovement.ValidMovement * 0.5f; ;
                var bMove = collision.FutureCollision.BMovement.ValidMovement * 0.5f; ;

                collision.BodyA.Position += aMove;
                collision.BodyB.Position += bMove;
                collision.BodyA.Velocity = aFinal;
                collision.BodyB.Velocity = -bFinal;
                collision.BodyA.MovementThisFrame = Vector2.Zero;
                collision.BodyB.MovementThisFrame = Vector2.Zero;

                if(aFinal.Length() > 15)
                { 
                    return;
                }
            }
        }
    }
}
