
using UnityEngine;
using Dev.Sonaru;
using Unity.Mathematics;


public class TestGrid : MonoBehaviour
{
    [SerializeField] private int rowNumber;
    [SerializeField] private int columnNumber;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 offsetPosition;

    [SerializeField] private GameObject cellPrefab;
    
    private Grid<int> gridSystem;
    private void Awake()
    {
        gridSystem = new Grid<int>(rowNumber, columnNumber, cellSize, offsetPosition);
    }


    private void Start()
    {
        for (int i = 0; i < columnNumber; i++)
        {
            for (int j = 0; j < rowNumber; j++)
            {
                Instantiate(cellPrefab, gridSystem.GetWorldPosition(i, j), quaternion.identity);
            }
        }
    }
}
