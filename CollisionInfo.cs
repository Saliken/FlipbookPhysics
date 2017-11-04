using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class CollisionInfo
    {
        public FBBody BodyA;
        public FBBody BodyB;
        public MovementInfo AMovement;
        public MovementInfo BMovement;
    }

    public class MovementInfo
    {
        public bool InteriorCollision;
        public Vector2 ValidMovement;
        public Vector2 RemainderAxisMovement;
        public Vector2 RemainderAxis;
        public Vector2 ReflectedMovement;
        public float ValidMovementPercent;
    }
}
