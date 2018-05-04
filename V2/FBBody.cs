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
        protected Vector2 position;
        protected float rotation;
        protected FBCollider collider;

        protected Vector2 movementThisFrame;

        public Vector2 Position { get => position; set => position = value; }
        public float Rotation { get => rotation; set => rotation = value; }
        public Vector2 MovementThisFrame { get => movementThisFrame; set => movementThisFrame = value; }
        public FBCollider Collider { get => collider; set { collider = value; collider.Parent = this; } }

        public Rectangle AABB => Collider.AABB();

        public virtual void SetMovementThisFrame(double elapsedMilliseconds)
        {
            return;
        }
    }
}
