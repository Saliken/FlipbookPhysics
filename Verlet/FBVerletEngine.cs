using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.Verlet
{
    public class FBVerletEngine
    {
        List<FBVerletShape> shapes;

        public FBVerletEngine()
        {
            shapes = new List<FBVerletShape>();
        }

        public void Add(FBVerletShape shape)
        {
            shapes.Add(shape);
        }

        public void Update(float elapsedTime)
        {
            foreach(var shape in shapes)
            {
                shape.Update(elapsedTime);
            }
        }
    }
}
