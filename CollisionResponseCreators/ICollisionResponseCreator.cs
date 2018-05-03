using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.CollisionResponseCreators
{
    interface ICollisionResponseCreator
    {
        void MakeResponse(CollisionInfo collision);
    }
}
