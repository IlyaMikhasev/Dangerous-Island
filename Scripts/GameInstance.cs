using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;
public enum GameState
{
    PlayerTurn,
    ComputerTurn,
    UseUI
}
public class GameInstance : MonoBehaviour
{

    public static GameInstance Instance;
    public GameState CurrentState { get; private set; }
    public static UnityEvent<int> MoveTurnCountUpdate = new();
    public bool IsEndGame { get; private set; }

    [SerializeField] private Charaster _charaster;
    [SerializeField] private Light2D _isEndGamePossibleColor;
    [SerializeField] private List<Transform> _spawnItemCell;
    [SerializeField] private Item[] _items;
    [SerializeField] private Tilemap _map;


    private Transform _enemy;
    private CanvasInstance _canvas;
    private const float _attackDelay = 0.3f;
    private GameState _tmpState;
    private int _moveCount;
    public void StartBattle(Transform enemy) {
        _enemy = enemy;
        _canvas.Battle();
    }
    /// <summary>
    /// Замена цвета лампы , означающей возможность завершения игры
    /// </summary>
    public void EndGameColor() {
        _isEndGamePossibleColor.color = Color.green;
        IsEndGame = true;
    } 
    // Логика для хода игрока
    public void SwitchToPlayerTurn()
    {
        ++_moveCount;
        MoveTurnCountUpdate?.Invoke(_moveCount);
        _charaster.SetStep(PlayerPrefs.GetInt("steps"));
        CurrentState = GameState.PlayerTurn;        
    }
    public void SwitchToUseUI() {
        _tmpState = CurrentState;
        CurrentState = GameState.UseUI;
    }
    public void SwitchToTurn() {
        CurrentState = _tmpState;
    }
    // Логика для хода компьютера
    public void SwitchToComputerTurn()
    {
        CurrentState = GameState.ComputerTurn;
        StartCoroutine(WaitAndMakeComputerMove());
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        CurrentState = GameState.PlayerTurn;
    }

    private void Start()
    {
        _moveCount = 0;
        _canvas = CanvasInstance.Instance;
        IsEndGame = false;
        CanvasInstance.EndBattle.AddListener(StartCEND);
        SpawnItems();
    }
    private IEnumerator WaitAndMakeComputerMove()
    {
        yield return new WaitForSeconds(1f); // Подождём одну секунду
        ComputerAI.Instance.MakeMove(); // Компьютер делает ход
    }
    private void StartCEND(int res) {
        StartCoroutine(BattleRessault(res));
    }
    private IEnumerator BattleRessault(int res) {
        Enemy zombie = _enemy.GetComponent<Enemy>();
        if (res == 1) {            
            _charaster.Attack();
            yield return new WaitForSeconds(_attackDelay);
            zombie.Die();
        }
        else if (res == -1) {
            zombie.Attack();
            yield return new WaitForSeconds(_attackDelay);
            _charaster.СhangeLifeQuantity(res);
        }
        else 
        {
            _charaster.СhangeLifeQuantity(res);
            yield break;
        }
    }
    private void OnDisable()
    {
        CanvasInstance.EndBattle.RemoveListener(StartCEND);
    }
    /// <summary>
    /// Расставление предметов по карте перед игрой
    /// </summary>
    private void SpawnItems() {
        foreach (var item in _items)
        {
            // Получаем случайный индекс
            int randomIndex = Random.Range(0, _spawnItemCell.Count);

            // Преобразуем позицию ячейки в мировые координаты
            Vector3Int cellSpawn = _map.WorldToCell(_spawnItemCell[randomIndex].position);

            // Удаляем использованную ячейку, чтобы предмет не появлялся повторно в этой позиции
            _spawnItemCell.RemoveAt(randomIndex);

            // Получаем центр клетки для спауна предмета
            Vector3 spawnPos = _map.GetCellCenterLocal(cellSpawn);

            // Создаем экземпляр предмета
            Instantiate(item, spawnPos, Quaternion.identity);
        }
    }
}
