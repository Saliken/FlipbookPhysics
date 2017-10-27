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

        public FBBodyType BodyType;
        public float Mass;

        public FBShape collider;
        public Vector2 position;
        public float rotation;
        public Rectangle AABB;
        public int BodyID { get; protected set; }
        private static int nextBodyID = 1;

        private float moveX, moveY;
        public float MoveX { get { return moveX; } }
        public float MoveY { get { return moveY; } }
        public Vector2 Movement { get { return new Vector2(moveX, moveY); } }
        

        public FBBody()
        {
            BodyID = nextBodyID;
            nextBodyID++;
        }
        
        public void Move(float x, float y)
        {
            moveX += x;
            moveY += y;
            FBEngine.AddMovedBody(this);
        }
        public void SetMove(float x, float y)
        {
            moveX = x;
            moveY = y;
        }
    }
}
