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

        public object UserData;

        public Vector2 Position { get { return collider.Position; } set { collider.Position = value; } }
        public float Rotation { get { return collider.Rotation; } set { collider.Rotation = value; } }

        public float MovementX { get; set; }
        public float MovementY { get; set; }
        public Vector2 Movement { get { return new Vector2(MovementX, MovementY); } }

        public event CollisionEventHandler OnBeforeCollision;
        public event CollisionEventHandler OnAfterCollision;
        

        public FBBody()
        {
            FBEngine.bodies.Add(this);
        }
        
        public void Move(float x, float y)
        {
            MovementX += x;
            MovementY += y;

            FBEngine.AddMovedBody(this);
        }
        public void Move(Vector2 movement)
        {
            MovementX += movement.X;
            MovementY += movement.Y;

            FBEngine.AddMovedBody(this);
        }
        public void SetMove(float x, float y)
        {
            MovementX = x;
            MovementY = y;
            FBEngine.AddMovedBody(this);
        }
        public void SetMove(Vector2 movement)
        {
            MovementX = movement.X;
            MovementY = movement.Y;
            FBEngine.AddMovedBody(this);
        }

        public void BeforeCollision(FutureCollision collision)
        {
            OnBeforeCollision?.Invoke(collision);
        }
        public void AfterCollision(FutureCollision collision)
        {
            OnAfterCollision?.Invoke(collision);  
        }
    }
}
