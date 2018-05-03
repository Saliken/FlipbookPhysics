using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.CollisionResponseCreators
{
    class FBBasicCollisionResponseCreator : ICollisionResponseCreator
    {
        public void MakeResponse(CollisionInfo collision)
        {
            collision.BodyA.BeforeCollision(collision);
            collision.BodyB.BeforeCollision(collision);

            if (collision.BodyA.type == FBBodyType.Dynamic)
            {
                collision.BodyA.Position += collision.AMovement.ValidMovement;
                collision.BodyA.SetMove(collision.AMovement.RemainderAxisMovement);
            }

            if (collision.BodyB.type == FBBodyType.Dynamic)
            {
                collision.BodyB.Position += collision.BMovement.ValidMovement;
                collision.BodyB.SetMove(collision.BMovement.RemainderAxisMovement);
            }

            collision.BodyA.AfterCollision(collision);
            collision.BodyB.AfterCollision(collision);
        }
    }
}
