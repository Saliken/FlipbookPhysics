﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.Verlet
{
    public interface IVerletCollidable
    {

        Vector2 Center { get; }
        Vector2 NearestPoint(Vector2 to);
        List<FBVerletEdge> CollisionAxes(IVerletCollidable otherShape);
        List<FBVerletPoint> CollisionPoints();
        void Project(Vector2 axis, out float min, out float max);
    }
}
