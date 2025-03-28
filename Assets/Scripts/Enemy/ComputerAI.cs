using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ComputerAI : MonoBehaviour
{
    public static ComputerAI Instance;
    public static int EnemysCount { get; private set; }

    [SerializeField] private Tilemap _map;
    [SerializeField] private GameObject[] _enemys;
    [SerializeField] private Transform[] _spawnCell;

    private Charaster _player;
    private Dictionary<int, GameObject> _enemysInGame;
    private int _moveCount;
    private List<Vector3Int> _cellIsOccupied;
    private void Start()
    {
        EnemysCount = 0;
        _moveCount = 0;
        Instance = this;
        _player = Charaster.Instance;
        GameInstance.Instance.SwitchToPlayerTurn(); // Начинаем игру с хода игрока
        GameInstance.MoveTurnCountUpdate.AddListener(UpdateCountTurnCount);
        _cellIsOccupied = new List<Vector3Int>();
        _enemysInGame = new Dictionary<int, GameObject>();
    }
    public void RemoveEnemy(int id) {
        _enemysInGame.Remove(id);
    }
    public bool CellIsOccupied(Vector3Int cell) {
        return _cellIsOccupied.Contains(cell);
    }
    // Логика для выбора действия компьютером
    public void MakeMove()
    {
        StartCoroutine(SpawnEnemy());
        UpdateOccupiedCells();
        StartCoroutine(EnemySelection());      
    }

    /// <summary>
    /// Coroutine создает случайного врага из списка врагов в случайном месте заданном как возможный для spawn 
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEnemy() {
        if (_moveCount % 3 == 0)
        {
            Vector3Int cellSpawn = _map.WorldToCell(_spawnCell[0].position);
            int maxAttempts = 100; // Максимальное количество попыток найти свободное место
            for (int attempt = 0; attempt < maxAttempts; ++attempt)
            {
                cellSpawn = _map.WorldToCell(_spawnCell[Random.Range(0, _spawnCell.Length)].position);
                if (!CellIsOccupied(cellSpawn))
                    break;
            }
            
            if (!CellIsOccupied(cellSpawn)) // Проверяем, найдена ли подходящая клетка
            {
                ++EnemysCount;
                GameObject enamy = Instantiate(_enemys[Random.Range(0, _enemys.Length)], cellSpawn + _map.cellSize * 0.5f, Quaternion.identity);
                enamy.GetComponent<Enemy>().ID = EnemysCount;
                _enemysInGame.Add (EnemysCount, enamy);
            }
            else
            {
                Debug.LogWarning("Не удалось найти подходящую клетку для спавна.");
            }

            yield return null;
        }
    }
    /// <summary>
    /// Метод обновляет список занятых клеток врагами
    /// </summary>
    private void UpdateOccupiedCells() {
        _cellIsOccupied.Clear();
        _cellIsOccupied.Add(_map.WorldToCell(_player.transform.position));
        foreach (var enemy in _enemysInGame) {
            _cellIsOccupied.Add(_map.WorldToCell(enemy.Value.transform.position));
        }
    }
   /// <summary>
   /// coroutine перебериет врагов на поле и задает им вектор движения 
   /// </summary>
   /// <returns></returns>
    private IEnumerator EnemySelection()
    {
        foreach (var enemy in _enemysInGame)
        {
            
            EnemyStep(enemy.Value.GetComponent<Enemy>());
            enemy.Value.GetComponent<Enemy>().MoveToTarget(_map.WorldToCell(_player.transform.position));
            yield return new WaitUntil(() => enemy.Value.GetComponent<Enemy>().IsMoving == false);
        }
        // Переключаемся обратно на ход игрока
        GameInstance.Instance.SwitchToPlayerTurn();
    }
    private void EnemyStep(Enemy objectEnemy)
    {        
        objectEnemy.IsMoving = true;
        objectEnemy.PossibilityOfAttack = true;
    }
    private void UpdateCountTurnCount(int count)
    {
        _moveCount = count;
    }
    private void OnDisable()
    {
        GameInstance.MoveTurnCountUpdate.RemoveListener(UpdateCountTurnCount);
    }

}
