using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    class FBSpatialHash<T>
    {

        public List<List<T>> items;

        public FBSpatialHash()
        {

        }

        public void Add(T item, Rectangle itemAABB)
        {
            var startX = itemAABB.Left;
            var startY = itemAABB.Top;  
            var endX = itemAABB.Right;
            var endY = itemAABB.Bottom;

            for(var x = startX; x <= endX; x++)
            {
                for(var y = startY; y <= endY; y++)
                {

                }
            }
        }
    }
}
