using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dev.Sonaru
{
    [Serializable]
    public class PathFindingVisual
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Vector3 cellPrefabSize;
        [SerializeField] private Material walkableMat;
        [SerializeField] private Material unWalkableMat;
        [SerializeField] private Material keyMat;
        [SerializeField] private Material pathMat;

        private Renderer[,] tileRenderers;

        public void InitVisual(int gridRowNumber, int gridColumnNumber)
        {
            tileRenderers = new Renderer[gridColumnNumber, gridRowNumber];
        }


        public void DrawPath(List<PathGridTile> pathList, bool includeStartEnd = true)
        {
            if(pathList == null)
                return;

            for (var i = 0; i < pathList.Count; i++)
            {
                var pathTile = pathList[i];
                if(!includeStartEnd && (i == 0 || i == pathList.Count - 1))
                    continue;
                
                tileRenderers[pathTile.XIndex, pathTile.YIndex].material = pathMat;
            }
        }


        public void ClearPath(List<PathGridTile> pathList)
        {
            if(pathList == null)
                return;

            foreach (var pathTile in pathList)
            {
                tileRenderers[pathTile.XIndex, pathTile.YIndex].material = walkableMat;
            }
        }


        public void SetKeyTile(PathGridTile tile)
        {
            tileRenderers[tile.XIndex, tile.YIndex].material = keyMat;
        }


        public void CreateTileVisual(OnGridDataChanged<PathGridTile> e)
        {
            var initPos = e.grid.GetWorldPosition(e.xIndex, e.yIndex) + cellPrefabSize / 2;
            var tileRenderer = Object.Instantiate(cellPrefab, initPos, Quaternion.identity).GetComponent<Renderer>();
            tileRenderer.material = walkableMat;
            tileRenderers[e.xIndex, e.yIndex] = tileRenderer;
        }


        public void ChangeWalkable(OnGridChangeWalkable e)
        {
            var target = tileRenderers[e.Tile.XIndex, e.Tile.YIndex];
            target.transform.Translate(e.Walkable ? Vector3.down * 0.3f : Vector3.up * 0.3f);
            target.material = e.Walkable ? walkableMat : unWalkableMat;
        }
    }
}