using System.Collections.Generic;
using UnityEngine;

public interface ICollider
{
    float X { get; }
    float Y { get; }
    float Radius { get; }

    void OnCollideEnter();
    void OnCollideExit();
}

public class QuadTreeNode
{
    public Rect Boundary { get; private set; }
    public int MaxObjects { get; private set; }
    public int Level { get; private set; }
    public List<ICollider> Colliders { get; private set; }
    public QuadTreeNode[] Children { get; private set; }

    public QuadTreeNode(Rect boundary, int maxObjects, int level)
    {
        Boundary = boundary;
        MaxObjects = maxObjects;
        Level = level;
        Colliders = new List<ICollider>();
        Children = new QuadTreeNode[4];
    }

    public void Clear()
    {
        Colliders.Clear();

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
    public QuadTreeNode Root => root;
    private QuadTreeNode root;
    private int maxDepth = 5;

    private List<int> _overlappingIndices = new List<int>();

    public QuadTree(Rect boundary, int maxObjects, int maxDepth)
    {
        maxDepth = maxDepth;
        root     = new QuadTreeNode(boundary, maxObjects, 0);
    }

    #region - Insert -

    public void Insert(ICollider collider)
    {
        InsertRecursive(root, collider);
    }

    private void InsertRecursive(QuadTreeNode node, ICollider collider)
    {
        // 有子節點，往子節點裡塞
        if (node.Children[0] != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (IsRectOverlapCollider(node.Children[i].Boundary, collider))
                {
                    InsertRecursive(node.Children[i], collider);
                }
            }
            return;
        }

        // 已經是子節點了
        node.Colliders.Add(collider);

        // 超過最大值，拆分Node
        if (node.Colliders.Count > node.MaxObjects && node.Level < maxDepth)
        {
            if (node.Children[0] == null)
            {
                SplitNode(node);
            }

            int i = 0;
            while (i < node.Colliders.Count)
            {
                var isDirty = false;
                for (int j = 0; j < 4; j++)
                {
                    if (IsRectOverlapCollider(node.Children[j].Boundary, node.Colliders[i]))
                    {
                        isDirty = true;
                        node.Children[j].Colliders.Add(node.Colliders[i]);
                    }
                }
                if (isDirty)
                    node.Colliders.RemoveAt(i);
                i++;
            }
        }
    }

    private bool IsRectOverlapCollider(Rect rect, ICollider collider)
    {
        // 计算矩形的边界
        float rectLeft = rect.xMin;
        float rectRight = rect.xMax;
        float rectTop = rect.yMax;
        float rectBottom = rect.yMin;

        // 获取碰撞体的边界
        float colliderLeft = collider.X - collider.Radius;
        float colliderRight = collider.X + collider.Radius;
        float colliderTop = collider.Y + collider.Radius;
        float colliderBottom = collider.Y - collider.Radius;

        // 检查矩形是否与碰撞体的边界重叠
        bool overlapX = colliderLeft <= rectRight && colliderRight >= rectLeft;
        bool overlapY = colliderTop >= rectBottom && colliderBottom <= rectTop;

        return overlapX && overlapY;
    }

    private void SplitNode(QuadTreeNode node)
    {
        float subWidth = node.Boundary.width / 2;
        float subHeight = node.Boundary.height / 2;
        float x = node.Boundary.xMin;
        float y = node.Boundary.yMin;

        node.Children[0] = new QuadTreeNode(new Rect(x + subWidth, y + subHeight, subWidth, subHeight), node.MaxObjects, node.Level + 1);
        node.Children[1] = new QuadTreeNode(new Rect(x, y + subHeight, subWidth, subHeight), node.MaxObjects, node.Level + 1);
        node.Children[2] = new QuadTreeNode(new Rect(x, y, subWidth, subHeight), node.MaxObjects, node.Level + 1);
        node.Children[3] = new QuadTreeNode(new Rect(x + subWidth, y, subWidth, subHeight), node.MaxObjects, node.Level + 1);
    }

    private int GetChildIndex(QuadTreeNode node, ICollider collider)
    {
        int index = -1;
        float verticalMidpoint = node.Boundary.xMin + (node.Boundary.width / 2);
        float horizontalMidpoint = node.Boundary.yMin + (node.Boundary.height / 2);

        bool topQuadrant = collider.Y > horizontalMidpoint;
        bool bottomQuadrant = collider.Y < horizontalMidpoint;
        bool topAndBottomQuadrant = collider.Y <= horizontalMidpoint && collider.Y >= horizontalMidpoint;
        bool leftQuadrant = collider.X < verticalMidpoint;
        bool rightQuadrant = collider.X > verticalMidpoint;
        bool leftAndRightQuadrant = collider.X >= verticalMidpoint && collider.X <= verticalMidpoint;

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

    #endregion

    #region - Remove -

    public void Remove(ICollider collider)
    {
        RemoveRecursive(collider, root);
    }

    private void RemoveRecursive(ICollider collider, QuadTreeNode node)
    {
        if (node == null)
            return;

        if (node.Colliders.Contains(collider))
        {
            node.Colliders.Remove(collider);
            return;
        }

        foreach (QuadTreeNode child in node.Children)
        {
            RemoveRecursive(collider, child);
        }
    }

    #endregion

    #region - Search Objects -

    public void GetCollidersInRange(Rect range, QuadTreeNode node, List<ICollider> collidersInRange)
    {
        if (node == null)
            return;

        if (!node.Boundary.Overlaps(range))
            return;

        foreach (ICollider collider in node.Colliders)
        {
            var pos = new Vector2(collider.X, collider.Y);
            if (range.Contains(pos))
            {
                collidersInRange.Add(collider);
            }
        }

        foreach (QuadTreeNode child in node.Children)
        {
            GetCollidersInRange(range, child, collidersInRange);
        }
    }

    public void GetCollidersInRange(ICollider collider, QuadTreeNode node, List<ICollider> collidersInRange)
    {
        if (node == null)
            return;

        if (!IsRectOverlapCollider(node.Boundary, collider))
            return;

        foreach (ICollider col in node.Colliders)
        {
            collidersInRange.Add(col);
        }

        foreach (QuadTreeNode child in node.Children)
        {
            GetCollidersInRange(collider, child, collidersInRange);
        }
    }

    private bool IsPositionInBoundary(Vector3 position, Rect boundary)
    {
        return position.x >= boundary.xMin && position.x <= boundary.xMax &&
               position.y >= boundary.yMin && position.y <= boundary.yMax;
    }

    #endregion

    #region - Search Node -

    public QuadTreeNode GetNodeContainingCollider(ICollider collider)
    {
        return GetNodeContainingColliderRecursive(collider, root);
    }

    private QuadTreeNode GetNodeContainingColliderRecursive(ICollider collider, QuadTreeNode node)
    {
        if (node == null)
            return null;

        if (node.Colliders.Contains(collider))
        {
            return node;
        }

        foreach (var child in node.Children)
        {
            QuadTreeNode foundNode = GetNodeContainingColliderRecursive(collider, child);
            if (foundNode != null)
                return foundNode;
        }

        return null;
    }

    #endregion
}
