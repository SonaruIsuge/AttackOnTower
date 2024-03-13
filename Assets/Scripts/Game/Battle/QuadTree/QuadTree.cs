using System.Collections.Generic;
using Game.Battle.Entities;
using Unity.Properties;
using UnityEngine;

namespace Game.Battle.QuadTree
{
    public class QuadTreeNode
    {
        public Rect             Boundary       { get; private set; }
        public int              MaxObjectCount { get; private set; }
        public int              Level          { get; private set; }
        public List<GameObject> Objects        { get; private set; }
        public QuadTreeNode[]   Children       { get; private set; }

        public QuadTreeNode(Rect boundary, int maxObjectCount, int level)
        {
            Boundary       = boundary;
            MaxObjectCount = maxObjectCount;
            Level          = level;
            Objects        = new List<GameObject>();
            Children       = new QuadTreeNode[4];
        }

        public void Clear()
        {
            Objects.Clear();

            for (int i = 0; i < Children.Length; i++)
            {
                if (Children[i] != null)
                {
                    Children[i].Clear();
                    Children[i] = null;
                }
            }
        }
    }

    public class QuadTree
    {
        private QuadTreeNode root;

        private const int MaxDepth = 5;

        public QuadTree(Rect boundary, int maxObjects)
        {
            root = new QuadTreeNode(boundary, maxObjects, 0);
        }

        public void Insert(GameObject obj)
        {
            InsertRecursive(root, obj);
        }

        private void InsertRecursive(QuadTreeNode node, GameObject obj)
        {
            if (node.Children[0] != null)
            {
                int index = GetChildIndex(node, obj);

                if (index != -1)
                {
                    InsertRecursive(node.Children[index], obj);
                    return;
                }
            }

            node.Objects.Add(obj);

            if (node.Objects.Count > node.MaxObjectCount && node.Level < MaxDepth)
            {
                if (node.Children[0] == null)
                {
                    SplitNode(node);
                }

                int i = 0;
                while (i < node.Objects.Count)
                {
                    int index = GetChildIndex(node, node.Objects[i]);
                    if (index != -1)
                    {
                        node.Children[index].Objects.Add(node.Objects[i]);
                        node.Objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// * * * * * * * * * * *
        /// *         *         *
        /// *    1    *    0    *
        /// *         *         *
        /// * * * * * * * * * * *
        /// *         *         *
        /// *    2    *    3    *
        /// *         *         *
        /// * * * * * * * * * * *
        /// </summary>
        private int GetChildIndex(QuadTreeNode node, GameObject obj)
        {
            int   index              = -1;
            float verticalMidpoint   = node.Boundary.xMin + (node.Boundary.width / 2);
            float horizontalMidpoint = node.Boundary.yMin + (node.Boundary.height / 2);

            bool topQuadrant    = obj.transform.position.y > horizontalMidpoint;
            bool bottomQuadrant = obj.transform.position.y < horizontalMidpoint;
            bool topAndBottomQuadrant = obj.transform.position.y <= horizontalMidpoint &&
                                        obj.transform.position.y >= horizontalMidpoint;
            bool leftQuadrant  = obj.transform.position.x < verticalMidpoint;
            bool rightQuadrant = obj.transform.position.x > verticalMidpoint;
            bool leftAndRightQuadrant = obj.transform.position.x >= verticalMidpoint &&
                                        obj.transform.position.x <= verticalMidpoint;

            if (topAndBottomQuadrant && leftAndRightQuadrant)
            {
                return -1;
            }

            if (leftQuadrant)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            else if (rightQuadrant)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }

        private void SplitNode(QuadTreeNode node)
        {
            float subWidth  = node.Boundary.width / 2;
            float subHeight = node.Boundary.height / 2;
            float x         = node.Boundary.xMin;
            float y         = node.Boundary.yMin;

            node.Children[0] = new QuadTreeNode(
                new Rect(x + subWidth, y, subWidth, subHeight), node.MaxObjectCount, node.Level + 1);
            node.Children[1] = new QuadTreeNode(
                new Rect(x, y, subWidth, subHeight), node.MaxObjectCount, node.Level + 1);
            node.Children[2] = new QuadTreeNode(
                new Rect(x, y + subHeight, subWidth, subHeight), node.MaxObjectCount, node.Level + 1);
            node.Children[3] = new QuadTreeNode(
                new Rect(x + subWidth, y + subHeight, subWidth, subHeight), node.MaxObjectCount, node.Level + 1);
        }
    }
}