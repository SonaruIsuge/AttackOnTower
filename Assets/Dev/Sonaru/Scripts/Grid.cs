
using System;
using UnityEngine;


namespace Dev.Sonaru
{
    public class Grid<TGridData>
    {
        public int RowNumber { get; private set; }
        public int ColumnNumber { get; private set; }
        public float CellSize { get; private set; }
        
        private Vector3 offsetPosition;
        private TGridData[,] gridDataArray;


        public Grid(int row, int column, float cellSize, Vector3 offsetPosition, Func<Grid<TGridData>, int, int, TGridData> createData)
        {
            this.RowNumber = row;
            this.ColumnNumber = column;
            this.CellSize = cellSize;
            this.offsetPosition = offsetPosition;

            gridDataArray = new TGridData[ColumnNumber, RowNumber];

            for (var x = 0; x < ColumnNumber; x++)
            {
                for (var y = 0; y < RowNumber; y++)
                {
                    SetData(x, y, createData(this, x, y));
                }
            }
        }


        public bool CheckCellExist(int xIndex, int yIndex)
        {
            return xIndex >= 0 && xIndex < ColumnNumber && yIndex >= 0 && yIndex < RowNumber;
        }


        public Vector3 GetWorldPosition(int xIndex, int yIndex)
        {
            return new Vector3(xIndex, 0, yIndex) * CellSize + offsetPosition;
        }


        public Vector2Int GetGridIndex(Vector3 worldPosition)
        {
            var result = Vector2Int.zero;
            result.x = Mathf.FloorToInt((worldPosition - offsetPosition).x / CellSize);
            result.y = Mathf.FloorToInt((worldPosition - offsetPosition).z / CellSize);
            return result;
        }


        public void SetData(int xIndex, int yIndex, TGridData data)
        {
            if (!CheckCellExist(xIndex, yIndex))
                return;

            gridDataArray[xIndex, yIndex] = data;
            EventManager.RaiseEvent<OnGridDataChanged<TGridData>>(new OnGridDataChanged<TGridData>(this, xIndex, yIndex, data));
        }


        public void SetData(Vector3 worldPosition, TGridData data)
        {
            var gridIndex = GetGridIndex(worldPosition);
            SetData(gridIndex.x, gridIndex.y, data);
        }


        public TGridData GetData(int xIndex, int yIndex)
        {
            if (!CheckCellExist(xIndex, yIndex))
                return default(TGridData);

            return gridDataArray[xIndex, yIndex];
        }


        public TGridData GetData(Vector3 worldPosition)
        {
            var gridIndex = GetGridIndex(worldPosition);

            return GetData(gridIndex.x, gridIndex.y);
        }
    }
}