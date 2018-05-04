using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBCollision<T> where T : FBBody
    {
        public T BodyA;
        public T BodyB;
        public FBCollisionMovementInformation AMovement;
        public FBCollisionMovementInformation BMovement;
    }
}
