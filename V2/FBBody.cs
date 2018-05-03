using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FlipbookPhysics.V2
{
    public class FBBody : IFBBody
    {
        protected Vector2 position;
        protected float rotation;
        protected Vector2 movement;
        protected FBCollider collider;

        public Vector2 Position { get => position; set => position = value; }
        public float Rotation { get => rotation; set => rotation = value; }
        public Vector2 Movement { get => movement; set => movement = value; }
        public FBCollider Collider { get => collider; set { collider = value; collider.Parent = this; } }

        public Rectangle AABB => Collider.AABB();

        public void AddMovement(float xPixels, float yPixels)
        {
            movement.X += xPixels;
            movement.Y += yPixels;
        }
    }
}
