using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public struct Cell
{
    public Vector3Int Position; // Позиция клетки на Tilemap
    public int Distance;        // Расстояние от стартовой клетки
          

    public Cell(Vector3Int position, int distance)
    {
        Position = position;
        Distance = distance;
    }
}

public class Pathfinder : MonoBehaviour
{
    public Tilemap tilemap;          // Ссылка на Tilemap
    public int maxMoves = 3;         // Максимальное количество ходов
    public LayerMask obstacleLayer;  // Слой, содержащий препятствия

    private Dictionary<Vector3Int, Cell> visitedCells = new Dictionary<Vector3Int, Cell>();

    public List<Vector3Int> FindPossibleMoves(Vector3Int startPosition)
    {
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(new Cell(startPosition, 0));

        while (queue.Count > 0)
        {
            Cell currentCell = queue.Dequeue();

            if (currentCell.Distance >= maxMoves)
            {
                continue;
            }

            foreach (var neighbor in GetNeighbors(currentCell.Position))
            {
                if (!visitedCells.ContainsKey(neighbor) && IsWalkable(neighbor))
                {
                    Cell newCell = new Cell(neighbor, currentCell.Distance + 1);
                    queue.Enqueue(newCell);
                    visitedCells.Add(neighbor, newCell);
                }
            }
        }

        return new List<Vector3Int>(visitedCells.Keys);
    }

    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int position)
    {
        yield return position + Vector3Int.up;    // Верхняя клетка
        yield return position + Vector3Int.down;  // Нижняя клетка
        yield return position + Vector3Int.left;  // Левая клетка
        yield return position + Vector3Int.right; // Правая клетка
    }

    private bool IsWalkable(Vector3Int position)
    {
        Vector3 worldPosition = tilemap.CellToWorld(position);
        return !Physics2D.OverlapCircle(worldPosition, 0.2f, obstacleLayer); // Проверяем наличие препятствий
    }
}
