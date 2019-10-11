using GMLoaded.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMLoaded.Collections
{
    public class Tree<T>
    {
        public TreeNode Root;

        public Tree(T RootUserdata, IEnumerable<TreeNode> Nodes)
        {
            this.Root = new TreeNode
            {
                Userdata = RootUserdata
            };
            this.Root.Add(Nodes);
        }

        public TreeNode[] GetLeaves()
        {
            List<TreeNode> Leaves = new List<TreeNode>
            {
                this.Root
            };
            Boolean Rep = true;
            while (Rep)
            {
                Rep = false;
                for (Int32 i = 0; i < Leaves.Count; i++)
                {
                    if (Leaves[i].HasChildren)
                    {
                        Leaves.Replace(Leaves[i], Leaves[i].Children);
                        Rep = true;
                        break;
                    }
                }
            }

            return Leaves.ToArray();
        }

        public class TreeNode
        {
            public List<TreeNode> Children = new List<TreeNode>();
            public TreeNode Parent;

            public T Userdata;

            public TreeNode(T Userdata = default) => this.Userdata = Userdata;

            public TreeNode(IEnumerable<TreeNode> Children) => this.Add(Children);

            public Boolean HasChildren => this.Children.Count > 0;

            public void Add(TreeNode Node)
            {
                this.Children.Add(Node);
                Node.Parent = this;
            }

            public void Add(IEnumerable<TreeNode> Nodes)
            {
                foreach (TreeNode N in Nodes)
                    this.Add(N);
            }

            public TreeNode[] PathToParent()
            {
                List<TreeNode> Nodes = new List<TreeNode>
                {
                    this
                };

                TreeNode Last = null;
                while ((Last = Nodes.Last()).Parent != null)
                    Nodes.Add(Last.Parent);

                return Nodes.ToArray();
            }

            public override String ToString() => this.Userdata == null ? "null" : this.Userdata.ToString();
        }
    }
}
