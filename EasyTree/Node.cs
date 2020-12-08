﻿using System;
using System.Collections.Generic;
using EasyTree.Iterators;

namespace EasyTree
{
    public class Node
    {
        public bool IsRoot { get; protected set; }

        public bool IsLeaf
        {
            get
            {
                return _isLeaf;
            }
            protected set
            {
                if (_isLeaf != value)
                {
                    _isLeaf = value;
                    if (_isLeaf)
                    {
                        AddLeaf(this);
                    }
                    else
                    {
                        RemoveLeaf(this);
                    }
                }
            }
        }

        public Node Root { get; protected set; }

        public Node Parent { get; protected set; }

        public List<Node> Path { get; protected set; } = new List<Node>();

        public List<Node> Children { get; protected set; } = new List<Node>();

        public HashSet<Node> Leaves { get; protected set; } = new HashSet<Node>();

        public HashSet<Node> Descendants { get; protected set; } = new HashSet<Node>();

        private bool _isLeaf;

        public Node()
        {
            Parent = null;
            IsRoot = true;
            IsLeaf = true;
            Root = this;
            Path.Add(this);
        }

        public Node(Node parent)
        {
            Parent = parent;
            IsRoot = false;
            IsLeaf = true;
            Root = this;
            Path.Add(this);
            AddParent(parent);
        }

        public void AddChild(Node child)
        {
            Children.Add(child);
            child.Parent = this;

            HashSet<Node>.Enumerator dEnum = child.Descendants.GetEnumerator();
            while (dEnum.MoveNext())
            {
                AddDescendant(dEnum.Current);
            }
            AddDescendant(child);
            
            if (IsLeaf)
            {
                IsLeaf = false;
            }

            if (child.IsRoot)
            {
                child.IsRoot = false;
            }
            
            foreach (Node node in new PreOrderIterator(child))
            {
                node.RedeterminePaths();
            }
        }

        public void RemoveChild(Node child)
        {
            HashSet<Node>.Enumerator lEnum = child.Leaves.GetEnumerator();
            while (lEnum.MoveNext())
            {
                RemoveLeaf(lEnum.Current);
            }

            HashSet<Node>.Enumerator dEnum = child.Descendants.GetEnumerator();
            while (dEnum.MoveNext())
            {
                RemoveDescendant(dEnum.Current);
            }
            RemoveDescendant(child);

            child.Parent = null;
            Children.Remove(child);
            
            foreach (Node node in new PreOrderIterator(child))
            {
                node.RedeterminePaths();
            }
            
            if (Children.Count == 0)
            {
                IsLeaf = true;
            }
            child.IsRoot = true;
        }

        public void AddParent(Node parent)
        {
            parent.AddChild(this);
        }

        public void RemoveParent()
        {
            Parent.RemoveChild(this);
        }

        protected void AddDescendant(Node descendant)
        {
            Descendants.Add(descendant);
            if (Parent != null)
            {
                Parent.AddDescendant(descendant);
            }
        }

        protected void RemoveDescendant(Node descendant)
        {
            Descendants.Remove(descendant);
            if (Parent != null)
            {
                Parent.RemoveDescendant(descendant);
            }
        }

        protected void AddLeaf(Node leaf)
        {
            Leaves.Add(leaf);
            if (Parent != null)
            {
                Parent.AddLeaf(leaf);
            }
        }

        protected void RemoveLeaf(Node leaf)
        {
            Leaves.Remove(leaf);
            if (Parent != null)
            {
                Parent.RemoveLeaf(leaf);
            }
        }

        protected void RedeterminePaths()
        {
            if (Parent != null)
            {
                Root = Parent.Root;
                Path = new List<Node>(Parent.Path)
                {
                    this
                };
            }
            else
            {
                Root = this;
                Path = new List<Node>() { this };
            }
        }
    }
}
