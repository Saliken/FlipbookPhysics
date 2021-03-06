﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{ 
    public abstract class FBCollider
    {
        public V2.FBBody Parent;
        public Vector2 ParentPositionOffset;
        public float ParentRotationOffset;

        public Vector2 Position { get => ParentPositionOffset + Parent.Position; }
        public float Rotation { get => ParentRotationOffset + Parent.Rotation; }
        

        public abstract Rectangle AABB();
        public abstract Vector2 NearestPoint(Vector2 to);
        public abstract List<Vector2> CollisionAxes(FBCollider shapeToCheckAgainst);
        public abstract void Project(Vector2 axis, out float min, out float max);
    }
}
