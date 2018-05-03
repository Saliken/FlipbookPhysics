using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public interface IFBBody : IFBSpatial
    {
        Vector2 Position { get; set; }
        float Rotation { get; set; }
        Vector2 Movement { get; set; }


        void AddMovement(float xPixels, float yPixels);
    }
}
