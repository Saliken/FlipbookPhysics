using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FlipbookPhysics.V2.CustomBodies
{
    public class FBVelocityBody : FBBody
    {
        public Vector2 Velocity;

        public override void SetMovementThisFrame(double elapsedMilliseconds)
        {
            MovementThisFrame = Velocity * (float)(elapsedMilliseconds / 1000);
        }
    }
}
