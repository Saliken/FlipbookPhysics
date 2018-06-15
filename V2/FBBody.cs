using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FlipbookPhysics.V2
{
    public class FBBody : IFBSpatial
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Velocity;
        public float mass = 1;

        protected FBCollider collider;
        protected Vector2 movementThisFrame;

        public Vector2 MovementThisFrame { get => movementThisFrame; set => movementThisFrame = value; }
        public FBCollider Collider { get => collider; set { collider = value; collider.Parent = this; } }
        public Rectangle AABB => Collider.AABB();

        public void SetMovementThisFrame(GameTime gameTime)
        {
            movementThisFrame = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
