using System;
using System.Collections.Generic;
using Framework.Common;
using Unity.Mathematics;
using UnityEngine;

namespace Dev.Sonaru
{
    public class TestGrid : MonoBehaviour
    {
        [SerializeField] private int rowNumber;
        [SerializeField] private int columnNumber;
        [SerializeField] private float cellSize;
        [SerializeField] private Vector3 offsetPosition;

        [SerializeField] private PathFindingVisual pathFindingVisual;


        private Camera mainCamera;
        
        private Grid<PathGridTile> gridSystem;
        private PathFinding pathFindingSystem;
        
        private PathGridTile startTile;
        private PathGridTile endTile;
        private List<PathGridTile> pathList;


        private void Awake()
        {
            pathFindingVisual.InitVisual(rowNumber, columnNumber);
        }


        private void OnEnable()
        {
            EventManager.Register<OnGridDataChanged<PathGridTile>>(pathFindingVisual.CreateTileVisual);
            EventManager.Register<OnGridChangeWalkable>(pathFindingVisual.ChangeWalkable);
        }


        private void Start()
        {
            mainCamera = Camera.main;
            pathList = new List<PathGridTile>();
            gridSystem = new Grid<PathGridTile>(rowNumber, columnNumber, cellSize, offsetPosition, CreateGridTile);
            pathFindingSystem = new PathFinding();
        }

        
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var tile = gridSystem.GetData(GetMouseWorldPos());
                tile?.SetWalkable(!tile.Walkable);
            }

            if (Input.GetMouseButtonDown(0))
            {
                var tile = gridSystem.GetData(GetMouseWorldPos());
                if(tile is not { Walkable: true })
                    return;

                if (startTile == null)
                {
                    ClearPath();
                    startTile = tile;
                    pathFindingVisual.SetKeyTile(startTile);
                }
                else if (endTile == null)
                {
                    endTile = tile;
                    pathFindingVisual.SetKeyTile(endTile);
                    
                    pathList = pathFindingSystem.FindPath(gridSystem, startTile, endTile);
                    pathFindingVisual.DrawPath(pathList, false);
                    
                    startTile = null;
                    endTile = null;
                }
            }
        }


        private PathGridTile CreateGridTile(Grid<PathGridTile> grid, int xIndex, int yIndex)
        {
            return new PathGridTile(grid, xIndex, yIndex);
        }
        

        private Vector3 GetMouseWorldPos()
        {
            var plane = new Plane(Vector3.up, 0);
            var ray = mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));
            
            return !plane.Raycast(ray, out var distance) ? Vector3.zero : ray.GetPoint(distance);
        }


        private void ClearPath()
        {
            if (pathList.Count <= 0) 
                return;
            
            foreach (var pathTile in pathList)
                pathTile.SetWalkable(true);
            pathFindingVisual.ClearPath(pathList);
            
            pathList.Clear();
        }
    }
}
