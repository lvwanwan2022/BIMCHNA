using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Solver
{
    //public class BinaryTree<T>
    //{
    //    BinaryTreeNode<T> root;
    //}
    //public class BinaryTreeNode<T>
    //{
    //    public BinaryTreeNode<T> leftNode;
    //    public BinaryTreeNode<T> rightNode;
    //    public BinaryTreeNode<T> parentNode;
    //    public T left { get { if (leftNode != null) 
    //            { return (T)Convert.ChangeType(leftNode,typeof(T)) ; }
    //            else { return default; } 
    //        }set { leftNode = value as BinaryTreeNode<T>;
    //        } }
    //    public T right { get { if (rightNode != null) 
    //            { return (T)Convert.ChangeType(rightNode, typeof(T)); }
    //            else { return default; }
    //        }
    //        set { rightNode = value as BinaryTreeNode<T>;
    //        } }
    //    public T parent { get { if (parentNode != null) 
    //            { return (T)Convert.ChangeType(parentNode, typeof(T)); }
    //            else { return default; }
    //        }
    //        set { parentNode =  value as BinaryTreeNode<T>;
    //        } }
    //    private T nodeValue;
    //    //public T Value=>nodeValue;
    //    public void Remove()
    //    {
    //        if (parentNode != null)
    //        {
    //            parentNode.RemoveChild(this);
    //        }
    //        parentNode = null;
    //    }
    //    public void RemoveChild(BinaryTreeNode<T> node)
    //    {
    //        if (node == null)
    //            return;
    //        if (leftNode == node)
    //            leftNode = null;
    //        if (rightNode == node)
    //            rightNode = null;
    //    }
    //    public void AppendLeft(BinaryTreeNode<T> node)
    //    {
    //        if (leftNode != null)
    //        {
    //            leftNode.Remove();
    //            leftNode = node;
    //        }
    //        if (leftNode != null)
    //            leftNode.parentNode = this;
    //    }
    //    public void AppendRight(BinaryTreeNode<T> node)
    //    {
    //        if (rightNode != null)
    //        {
    //            rightNode.Remove();
    //            rightNode = node;
    //        }
    //        if (rightNode != null)
    //            rightNode.parentNode = this;
    //    }
    //}
}
