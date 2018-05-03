using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBScene<T> where T : FBBody
    {
        public Vector2 Gravity;
        public bool UseGravity = false;
        public bool Active = true;
        public float Speed = 1;
        public IFBCollisionResolver<T> CollisionResolver;
        public int Iterations = 2;
        public bool Debug;

        protected List<T> bodies;
        protected FBSpatialHash<T> bodiesHash;
        protected FBCollisionChecker<T> collisionChecker;


        public FBScene()
        {
            bodies = new List<T>();
            bodiesHash = new FBSpatialHash<T>(100);
            collisionChecker = new FBCollisionChecker<T>();
        }

        public void Initialize()
        {
            bodies = new List<T>();
            bodiesHash = new FBSpatialHash<T>(100);
        }

        public void Add(T body)
        {
            bodies.Add(body);
        }
        public void Clear()
        {
            bodies.Clear();
        }
        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            for (int i = 0; i < Iterations; i++)
            {
                FillSpatialHash();
                var collisions = collisionChecker.GetAllCollisions(bodies, bodiesHash);
                foreach (var collision in collisions)
                    CollisionResolver.Resolve(collision);
            }
        }

        public void Draw()
        {
            if (Debug)
            {
                FBDebugDraw.SpriteBatch.Begin();
                foreach (var body in bodies)
                {
                    if (body.Collider is FBCircle circle)
                    {
                        FBDebugDraw.Circle(circle.Position, circle.Radius, Color.White);
                    }
                    else if (body.Collider is FBRectangle rectangle)
                    {
                        FBDebugDraw.Rectangle(rectangle.Position, rectangle.width, rectangle.height, Color.White);
                    }
                    else if (body.Collider is FBPolygon polygon)
                    {
                        FBDebugDraw.Polygon(polygon, Color.White);
                    }
                }
                FBDebugDraw.SpriteBatch.End();
            }
        }

        protected void FillSpatialHash()
        {
            bodiesHash.Clear();
            bodiesHash.AddRange(bodies);
        }
    }
}
