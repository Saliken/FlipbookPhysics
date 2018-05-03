using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    public interface IFBCollisionResolver<T> where T : IFBBody
    {
        void Resolve(FBCollision<T> collision);
    }
}
