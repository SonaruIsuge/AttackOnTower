using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Sonaru
{
    public class PathFinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;
        
        private List<PathGridTile> openList;
        private List<PathGridTile> closeList;

        
        public PathFinding()
        {
            openList = new List<PathGridTile>();
            closeList = new List<PathGridTile>();
        }
        
        
        public List<PathGridTile> FindPath(Grid<PathGridTile> grid, PathGridTile startTile, PathGridTile endTile, bool enableDiagonal = true)
        {
            // clear both list and add start tile to open list
            openList.Clear();
            closeList.Clear();
            openList.Add(startTile);

            for (var x = 0; x < grid.ColumnNumber; x++)
            {
                for (var y= 0; y < grid.RowNumber; y++)
                {
                    var tile = grid.GetData(x, y);
                    tile.gCost = int.MaxValue;
                    tile.CalculateFCost();
                    tile.cameFromNode = null;
                }
            }
            
            startTile.gCost = 0;
            startTile.hCost = CalculateDistanceCost(startTile, endTile);
            startTile.CalculateFCost();

            while (openList.Count > 0)
            {
                var currentTile = GetLowestFCostTile(openList);
                if (currentTile == endTile)
                {
                    // finish calculate, return path list
                    return CalculatePath(endTile);
                }

                openList.Remove(currentTile);
                closeList.Add(currentTile);

                foreach (var neighborTile in GetNeighborList(grid, currentTile, enableDiagonal))
                {
                    if(closeList.Contains(neighborTile))
                        continue;
                    
                    if(!neighborTile.Walkable)
                        continue;

                    var tentativeGCost = currentTile.gCost + CalculateDistanceCost(currentTile, neighborTile);
                    if (tentativeGCost < neighborTile.gCost)
                    {
                        neighborTile.cameFromNode = currentTile;
                        neighborTile.gCost = tentativeGCost;
                        neighborTile.hCost = CalculateDistanceCost(neighborTile, endTile);
                        neighborTile.CalculateFCost();

                        if (!openList.Contains(neighborTile))
                        {
                            openList.Add(neighborTile);
                        }
                    }
                }
            }

            // No path find, return null
            return null;
        }
        
        
        private List<PathGridTile> CalculatePath(PathGridTile endNode)
        {
            var path = new List<PathGridTile> { endNode };
            var currentNode = endNode;
            while(currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();
            return path;
        }
        
        
        private int CalculateDistanceCost(IGridTile a, IGridTile b)
        {
            var xDis = Mathf.Abs(a.XIndex - b.XIndex);
            var yDis = Mathf.Abs(a.YIndex - b.YIndex);
            var remaining = Mathf.Abs(xDis - yDis);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDis, yDis) + MOVE_STRAIGHT_COST * remaining;
        }
        
        
        private PathGridTile GetLowestFCostTile(List<PathGridTile> tileList)
        {
            var lowestFCostTile = tileList[0];
            for(var i = 1; i < tileList.Count; i++)
            {
                if(tileList[i].fCost < lowestFCostTile.fCost)
                {
                    lowestFCostTile = tileList[i];
                }
            }
            return lowestFCostTile;
        }
        
        
        private List<PathGridTile> GetNeighborList(Grid<PathGridTile> grid, PathGridTile currentTile, bool enableDiagonal)
        {
            var neighborList = new List<PathGridTile>();

            if(currentTile.XIndex - 1 >= 0)
            {
                //Left
                neighborList.Add(grid.GetData(currentTile.XIndex - 1, currentTile.YIndex));
                //LeftDown
                if(enableDiagonal && currentTile.YIndex - 1 >= 0) 
                    neighborList.Add(grid.GetData(currentTile.XIndex - 1, currentTile.YIndex - 1));
                //LeftUp
                if(enableDiagonal && currentTile.YIndex + 1 < grid.RowNumber) 
                    neighborList.Add(grid.GetData(currentTile.XIndex - 1, currentTile.YIndex + 1));
            }
            if(currentTile.XIndex + 1 < grid.ColumnNumber)
            {
                //Right
                neighborList.Add(grid.GetData(currentTile.XIndex + 1, currentTile.YIndex));
                //RightDown
                if(enableDiagonal && currentTile.YIndex - 1 >= 0) 
                    neighborList.Add(grid.GetData(currentTile.XIndex + 1, currentTile.YIndex - 1));
                //RightUp
                if(enableDiagonal && currentTile.YIndex + 1 < grid.RowNumber) 
                    neighborList.Add(grid.GetData(currentTile.XIndex + 1, currentTile.YIndex + 1));
            }
            //Down
            if(currentTile.YIndex - 1 >= 0) 
                neighborList.Add(grid.GetData(currentTile.XIndex, currentTile.YIndex - 1));
            //Up
            if(currentTile.YIndex + 1 < grid.RowNumber) 
                neighborList.Add(grid.GetData(currentTile.XIndex, currentTile.YIndex + 1));

            return neighborList;
        }
    }
}