using FlipbookPhysics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class FBBody
    {

        public bool Active = true;
        public FBCollider collider;
        public Rectangle AABB;
        public FBBodyType type;

        public object UserData;

        public Vector2 Position { get { return collider.Position; } set {  } }
        public float Rotation { get { return collider.Rotation; } set {  } }

        public float MovementX { get; set; }
        public float MovementY { get; set; }
        public Vector2 Movement { get { return new Vector2(MovementX, MovementY); } }

        public event CollisionEventHandler OnBeforeCollision;
        public event CollisionEventHandler OnAfterCollision;
        

        public FBBody(FBBodyType type = FBBodyType.Dynamic)
        {
            this.type = type;
        }
        
        public void Move(float x, float y)
        {
            if (type == FBBodyType.Dynamic)
            {
                MovementX += x;
                MovementY += y;
            }
        }
        public void Move(Vector2 movement)
        {
            if (type == FBBodyType.Dynamic)
            {
                MovementX += movement.X;
                MovementY += movement.Y;
            }
        }
        public void SetMove(float x, float y)
        {
            if (type == FBBodyType.Dynamic)
            {
                MovementX = x;
                MovementY = y;
            }
        }
        public void SetMove(Vector2 movement)
        {
            if (type == FBBodyType.Dynamic)
            {
                MovementX = movement.X;
                MovementY = movement.Y;
            }
        }

        public void BeforeCollision(CollisionInfo collision)
        {
            OnBeforeCollision?.Invoke(collision);
        }
        public void AfterCollision(CollisionInfo collision)
        {
            OnAfterCollision?.Invoke(collision);  
        }
    }
}
