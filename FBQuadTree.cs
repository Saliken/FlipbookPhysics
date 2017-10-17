using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookCollision
{
    
    public class FBQuadTree
    {
        private FBQuadTreeNode mainNode;

        public FBQuadTree(Rectangle worldBoundaries, int nodeCapacity, int maxLevel)
        {
            mainNode = new FBQuadTreeNode(worldBoundaries, 1, nodeCapacity, maxLevel);
        }

        public void AddBody(FBBody body)
        {
            mainNode.AddBody(body);
        }

        public List<FBBody> Nearby(FBBody body)
        {
            return null;
        }

        public void Clear()
        {
            mainNode.Clear();
        }
        
    }
}
