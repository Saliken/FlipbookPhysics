using Microsoft.Xna.Framework;

namespace FlipbookPhysics.V2
{
    public class FBCollisionMovementInformation
    {
        public bool InteriorCollision;
        public Vector2 ValidMovement;
        public Vector2 RemainderAxisMovement;
        public Vector2 RemainderAxis;
        public Vector2 ReflectedMovement;
        public float ValidMovementPercent;
    }
}
