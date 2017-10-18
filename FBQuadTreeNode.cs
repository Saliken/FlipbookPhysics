using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    class FBQuadTreeNode
    {
        public Rectangle bounds;
        private List<FBBody> bodies;
        private FBQuadTreeNode[] subNodes;
        private int level;
        private int maxLevel;
        private int capacity;

        private Rectangle TopLeftRectangle { get { return new Rectangle(bounds.X, bounds.Y, bounds.Width / 2, bounds.Height / 2); } }
        private Rectangle TopRightRectangle { get { return new Rectangle(bounds.Center.X, bounds.Y, bounds.Width / 2, bounds.Height / 2); } }
        private Rectangle BottomLeftRectangle { get { return new Rectangle(bounds.X, bounds.Center.Y, bounds.Width / 2, bounds.Height / 2); } }
        private Rectangle BottomRightRectangle { get { return new Rectangle(bounds.Center.X, bounds.Center.Y, bounds.Width / 2, bounds.Height / 2); } }

        public FBQuadTreeNode(Rectangle boundaries, int level, int capacity, int maxLevel)
        {
            bounds = boundaries;
            bodies = new List<FBBody>();
            this.capacity = capacity;
            this.level = level;
            this.maxLevel = maxLevel;
        }

        public void AddBody(FBBody body)
        {
            if (subNodes == null)
            {
                bodies.Add(body);

                if(bodies.Count >= capacity && level != maxLevel)
                {
                    Split();
                    int i = 0;
                    while(i < bodies.Count)
                    {
                        var subNode = GetValidSubNode(bodies[i]);
                        if (subNode != null)
                        {
                            subNode.AddBody(bodies[i]);
                            bodies.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }
            else
            {
                var subnode = GetValidSubNode(body);
                if (subnode != null)
                    subnode.AddBody(body);
                else
                    bodies.Add(body);
            }
        }
        public void Clear()
        {
            bodies.Clear();
            if(subNodes != null)
            {
                foreach (var subNode in subNodes)
                    subNode.Clear();
                subNodes = null;
            }
        }
        private FBQuadTreeNode GetValidSubNode(FBBody body)
        {
            foreach (var subNode in subNodes)
            {
                if (subNode.bounds.Contains(body.AABB))
                {
                    return subNode;
                }
            }
            return null;
        }
        private void Split()
        {
            if (subNodes == null)
            {
                subNodes = new FBQuadTreeNode[4];
                subNodes[0] = new FBQuadTreeNode(TopLeftRectangle, level + 1, capacity, maxLevel);
                subNodes[1] = new FBQuadTreeNode(TopRightRectangle, level + 1, capacity, maxLevel);
                subNodes[2] = new FBQuadTreeNode(BottomLeftRectangle, level + 1, capacity, maxLevel);
                subNodes[3] = new FBQuadTreeNode(BottomRightRectangle, level + 1, capacity, maxLevel);
            }
        }
    }
}
