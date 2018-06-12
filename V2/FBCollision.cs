using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBCollision
    {
        public FBBody BodyA;
        public FBBody BodyB;
        public bool DidCollide;
        public float TimeOfImpact;
        public FBCurrentCollisionInfo CurrentCollision;
        public FBFutureCollisionInfo FutureCollision;
    }

    public class FBCurrentCollisionInfo
    {
        public Vector2 MTV;
    }

    public class FBFutureCollisionInfo
    {
        public FBCollisionMovementInformation AMovement;
        public FBCollisionMovementInformation BMovement;
        public FBCollisionInformation ACollisionInfo;
        public FBCollisionInformation BCollisionInfo;
    }
    
}
