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

        [SerializeField] private GameObject cellPrefab;
    
        private Grid<bool> gridSystem;
        private void Awake()
        {
            EventManager.Register<OnGridDataChanged<bool>>(InstantiateObject);
        }


        private void Start()
        {
            gridSystem = new Grid<bool>(rowNumber, columnNumber, cellSize, offsetPosition, (g, x, y) => false);
        }


        private void InstantiateObject(OnGridDataChanged<bool> e)
        {
            Instantiate(cellPrefab, e.grid.GetWorldPosition(e.xIndex, e.yIndex), quaternion.identity);
        }
    }
}
