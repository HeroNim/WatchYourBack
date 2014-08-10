using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public class QuadTree<T>
    {
        Node root;
        int quadWidth;
        int quadHeight;

        public QuadTree(int x, int y, int width, int height, int depth = 0)
        {
            quadWidth = width;
            quadHeight = height;
            root = new Node(x, y, width, height);
            Subdivide(depth);
        }

        public void Subdivide(int depth)
        {
            while (depth != 0)
            {
                Subdivide(root);
                depth--;
                quadWidth /= 2;
                quadHeight /= 2;
            }
        }

        public void Add(T item, Rectangle position)
        {
            Add(item, position, root);
        }

        public void Remove(T item)
        {
            Remove(item, root);
        }

        /// <summary>
        /// Returns all items in the quads which intercept the given line
        /// </summary>
        /// <param name="line">A line</param>
        /// <returns>A list of all applicable items</returns>
        public List<T> Intersects(Line line)
        {
            List<T> list = new List<T>();
            Intersects(line, root, list);
            list = list.Distinct().ToList();
            return list;
        }

        private void Subdivide(Node node)
        {
            if (!node.IsLeaf)
                foreach (Node child in node.Children)
                    Subdivide(child);
            else
            {
                node.IsLeaf = false;
                node.Children.Add(new Node(node.X, node.Y, node.Width / 2, node.Height / 2));
                node.Children.Add(new Node(node.X + node.Width / 2, node.Y, node.Width / 2, node.Height / 2));
                node.Children.Add(new Node(node.X, node.Y + node.Height / 2, node.Width / 2, node.Height / 2));
                node.Children.Add(new Node(node.X + node.Width / 2, node.Y + node.Height / 2, node.Width / 2, node.Height / 2));
            }
        }

        private void Add(T item, Rectangle position, Node node)
        {
            if (CollisionHelper.CheckCollision(position, node.Quad) == false)
                return;
            if (!node.IsLeaf)
                foreach (Node child in node.Children)
                    Add(item, position, child);
            else
                if (!node.Contents.Contains(item))
                    node.Contents.Add(item);
        }

        private void Remove(T item, Node node)
        {
            if (!node.IsLeaf)
                foreach (Node child in node.Children)
                    Remove(item, child);
            else
                if (node.Contents.Contains(item))
                    node.Contents.Remove(item);
        }

        private void Intersects(Line line, Node node, List<T> list)
        {
            if (CollisionHelper.CheckCollision(line, node.Quad) == false)
                return;
            if (!node.IsLeaf)
            {
                foreach (Node child in node.Children)
                {
                    Intersects(line, child, list);
                }
            }
            else
            {
                list.AddRange(node.Contents);               
                return;
            }
        }

        public int Width { get { return quadWidth; } }
        public int Height { get { return quadHeight; } }
       
        private class Node
        {
            Rectangle quad;
            List<Node> children;
            bool isLeaf;
            List<T> contents;

            public Node(int x, int y, int width, int height)
            {
                quad = new Rectangle(x, y, width, height);
                children = new List<Node>();
                isLeaf = true;
                contents = new List<T>();
            }

            public Rectangle Quad
            {
                get { return quad; }
                set { quad = value; }
            }

            public bool IsLeaf
            {
                get { return isLeaf; }
                set { isLeaf = value; }
            }

            public List<Node> Children
            {
                get { return children; }
                set { children = value; }
            }

            public List<T> Contents
            {
                get { return contents; }
                set { contents = value; }
            }

            public int X { get { return quad.X; } }
            public int Y { get { return quad.Y; } }
            public int Width { get { return quad.Width; } }
            public int Height { get { return quad.Height; } }

            public override string ToString()
            {
                return (X.ToString() + ", " + Y.ToString() + ", " + Width.ToString() + ", " + Height.ToString() + ")");
            }
        }
    }
}
