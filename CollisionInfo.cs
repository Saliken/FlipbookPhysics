using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public class CollisionInfo
    {
        public FBBody BodyA;
        public FBBody BodyB;
        public Vector2 AValidMovement;
        public Vector2 BValidMovement;

    }
}
