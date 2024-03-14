using System.Collections.Generic;
using UnityEngine;

public interface ICollider
{
    ulong Id     { get; }
    float X      { get; }
    float Y      { get; }
    float Radius { get; }

    void OnCollideEnter();
    void OnCollideExit();
}

public class QuadTreeNode
{
    public Rect                         Boundary      { get; private set; }
    public int                          MaxObjects    { get; private set; }
    public int                          Level         { get; private set; }
    public Dictionary<ulong, ICollider> CollidersById { get; private set; }
    public QuadTreeNode[]               Children      { get; private set; }

    public QuadTreeNode(Rect boundary, int maxObjects, int level)
    {
        Boundary      = boundary;
        MaxObjects    = maxObjects;
        Level         = level;
        CollidersById = new Dictionary<ulong, ICollider>();
        Children      = new QuadTreeNode[4];
    }

    public void Clear()
    {
        CollidersById.Clear();

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
    public  QuadTreeNode Root => _root;
    private QuadTreeNode _root;
    private int          _maxDepth = 5;

    private List<ulong> _idToRemove = new List<ulong>();

    public QuadTree(Rect boundary, int maxObjects, int maxDepth)
    {
        _maxDepth = maxDepth;
        _root     = new QuadTreeNode(boundary, maxObjects, 0);
    }

    #region - Insert -

    public void Insert(ICollider collider)
    {
        InsertRecursive(_root, collider);
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
        node.CollidersById.TryAdd(collider.Id, collider);

        // 超過最大值，拆分Node
        if (node.CollidersById.Count > node.MaxObjects && node.Level < _maxDepth)
        {
            if (node.Children[0] == null)
            {
                SplitNode(node);
            }

            _idToRemove.Clear();
            foreach (var id in node.CollidersById.Keys)
            {
                var value   = node.CollidersById[id];
                var isDirty = false;
                for (int j = 0; j < 4; j++)
                {
                    if (IsRectOverlapCollider(node.Children[j].Boundary, value))
                    {
                        isDirty = true;
                        node.Children[j].CollidersById.TryAdd(id, value);
                    }
                }

                if (isDirty)
                    _idToRemove.Add(id);
            }

            foreach (var id in _idToRemove)
            {
                node.CollidersById.Remove(id);
            }
        }
    }

    private bool IsRectOverlapCollider(Rect rect, ICollider collider)
    {
        var rectLeft   = rect.xMin;
        var rectRight  = rect.xMax;
        var rectTop    = rect.yMax;
        var rectBottom = rect.yMin;

        var colliderLeft   = collider.X - collider.Radius;
        var colliderRight  = collider.X + collider.Radius;
        var colliderTop    = collider.Y + collider.Radius;
        var colliderBottom = collider.Y - collider.Radius;

        var overlapX = colliderLeft <= rectRight && colliderRight >= rectLeft;
        var overlapY = colliderTop >= rectBottom && colliderBottom <= rectTop;

        return overlapX && overlapY;
    }

    private void SplitNode(QuadTreeNode node)
    {
        var subWidth  = node.Boundary.width / 2;
        var subHeight = node.Boundary.height / 2;
        var x         = node.Boundary.xMin;
        var y         = node.Boundary.yMin;

        node.Children[0] = new QuadTreeNode(new Rect(x + subWidth, y + subHeight, subWidth, subHeight), node.MaxObjects,
            node.Level + 1);
        node.Children[1] = new QuadTreeNode(new Rect(x, y + subHeight, subWidth, subHeight), node.MaxObjects,
            node.Level + 1);
        node.Children[2] = new QuadTreeNode(new Rect(x, y, subWidth, subHeight), node.MaxObjects, node.Level + 1);
        node.Children[3] =
            new QuadTreeNode(new Rect(x + subWidth, y, subWidth, subHeight), node.MaxObjects, node.Level + 1);
    }

    private int GetChildIndex(QuadTreeNode node, ICollider collider)
    {
        var index              = -1;
        var verticalMidpoint   = node.Boundary.xMin + (node.Boundary.width / 2);
        var horizontalMidpoint = node.Boundary.yMin + (node.Boundary.height / 2);

        var topQuadrant          = collider.Y > horizontalMidpoint;
        var bottomQuadrant       = collider.Y < horizontalMidpoint;
        var topAndBottomQuadrant = collider.Y <= horizontalMidpoint && collider.Y >= horizontalMidpoint;
        var leftQuadrant         = collider.X < verticalMidpoint;
        var rightQuadrant        = collider.X > verticalMidpoint;
        var leftAndRightQuadrant = collider.X >= verticalMidpoint && collider.X <= verticalMidpoint;

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
        RemoveRecursive(collider, _root);
    }

    private void RemoveRecursive(ICollider collider, QuadTreeNode node)
    {
        if (node == null)
            return;

        if (node.CollidersById.ContainsKey(collider.Id))
        {
            node.CollidersById.Remove(collider.Id);
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

        foreach (ICollider collider in node.CollidersById.Values)
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

        foreach (ICollider col in node.CollidersById.Values)
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
        return GetNodeContainingColliderRecursive(collider, _root);
    }

    private QuadTreeNode GetNodeContainingColliderRecursive(ICollider collider, QuadTreeNode node)
    {
        if (node == null)
            return null;

        if (node.CollidersById.ContainsKey(collider.Id))
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