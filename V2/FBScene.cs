using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public class FBScene
    {
        public Vector2 Gravity;
        public bool UseGravity = false;
        public bool Active = true;
        public float Speed = 1;
        public bool Debug;
        public float AirFriction = 10f; //reduction per second 0-100


        public int Iterations = 1;
        public FBCollisionResolver CollisionResolver;

        protected List<FBBody> bodies;
        protected FBSpatialHash<FBBody> bodiesHash;
        protected FBCollisionChecker collisionChecker;


        public FBScene()
        {
            bodies = new List<FBBody>();
            bodiesHash = new FBSpatialHash<FBBody>(100);
            collisionChecker = new FBCollisionChecker();
        }

        public void Initialize()
        {
            bodies = new List<FBBody>();
            bodiesHash = new FBSpatialHash<FBBody>(100);
        }

        public void Add(FBBody body)
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

            PreProcessBodies(gameTime);

            for (int i = 0; i < Iterations; i++)
            {
                FillSpatialHash();
                var collisions = collisionChecker.GetAllCollisions(bodies, bodiesHash);
                foreach (var collision in collisions)
                    CollisionResolver.Resolve(collision);
            }

            MoveBodies(gameTime);
        }

        protected void PreProcessBodies(GameTime gameTime)
        {
            foreach(var body in bodies)
            {
                ApplyGravity(body, gameTime);
                ApplyAirFriction(body, gameTime);

                body.SetMovementThisFrame(gameTime);
            }
        }

        protected void ApplyAirFriction(FBBody body, GameTime gameTime)
        {
            body.Velocity *= 1 - (AirFriction / 100f);
        }

        protected void ApplyGravity(FBBody body, GameTime gameTime)
        {
              
        }

        protected void MoveBodies(GameTime gameTime)
        {
            foreach(var body in bodies)
            {
                body.Position += body.MovementThisFrame;
                body.MovementThisFrame = Vector2.Zero;
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
