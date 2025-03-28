using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public bool IsPlayerDanger { get; private set; }
    public bool PossibilityOfAttack;
    public bool IsMoving;
    public int ID;

    [SerializeField] private int _movesPerTurn;
    [SerializeField] private LayerMask _itemsLayer;
    [SerializeField] private float _speed;

    private Tilemap _tilemap;
    private Charaster _charaster;
    private Vector3Int _currentPosition;
    private Vector3Int _targetPosition;
    private SpriteRenderer _spriteRenderer;
    private bool _isAttack;
    private Animator _animator;
    private GameInstance _gameInstance;
    public void Die()
    {
        ComputerAI.Instance.RemoveEnemy(ID);
        IsPlayerDanger = false;
        _animator.SetBool("death", true);
    }
    public void Attack() {
        _isAttack = !_isAttack;
        _animator.SetBool("attack", _isAttack);
    }
    private void Awake()
    {
        _tilemap = FindFirstObjectByType<Tilemap>();
        _currentPosition = _tilemap.WorldToCell( transform.position );
    }
    private void Start()
    {
        PossibilityOfAttack = true;
        _isAttack = false;        
        _charaster = Charaster.Instance;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _gameInstance = GameInstance.Instance;
    }
    //public void SetVector(Vector3Int target) {
    //    if (target != null && _tilemap != null)
    //    {
    //        _targetPosition = target ;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("target - " + target + ": _tilemap - " + _tilemap);
    //    }
    //}
    private void AnimationMove()
    {
        if (IsMoving)
        {
            _animator.SetBool("move", IsMoving);
            if (GameInstance.Instance.CurrentState != GameState.ComputerTurn ) { IsMoving = false; }
        }
        else
            _animator.SetBool("move", IsMoving);
    }

    private void Update()
    {        
        PlayerCheck();
        ScaleSprite(_charaster.transform.position.x < transform.position.x);
        AnimationMove();
        if (GameInstance.Instance.CurrentState != GameState.ComputerTurn)
        {
            return;
        }
        if (IsPlayerDanger && PossibilityOfAttack) Battle();
    }
    private void Battle() {
        PossibilityOfAttack = false;
        _gameInstance.StartBattle(transform);
    }
    private void Death()
    {      
        Destroy(gameObject);
    }
    private void OnMouseDown()
    {
        if (!IsPlayerDanger) {
            Debug.Log("Персонаж находиться далеко");
        }      
    }
    /// <summary>
    /// разворот врага в сторону персонажа
    /// </summary>
    /// <param name="isScale"></param>
    private void ScaleSprite(bool isScale)
    {
        if (isScale)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }
    /// <summary>
    /// Метод проверяет наличие врага в горизонтали и вертикали, на растоянии одной клетки
    /// </summary>
    /// <returns></returns>
    private bool PlayerCheck()
    {
        foreach (var neighbor in GetNeighbors(_tilemap.WorldToCell(_currentPosition)))
        {

            if ((_tilemap.WorldToCell(_charaster.transform.position) == neighbor) && IsPlayerDanger == false)
            {
              return IsPlayerDanger = true;
            }            
        }
        return IsPlayerDanger = false;
    }
    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int position)
    {
        yield return position + Vector3Int.up;    // Верхняя клетка
        yield return position + Vector3Int.down;  // Нижняя клетка
        yield return position + Vector3Int.left;  // Левая клетка
        yield return position + Vector3Int.right; // Правая клетка
    }

    /// <summary>
    /// после назначения цели , запускается корутина движения врага
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTarget(Vector3Int target)
    {
        _targetPosition = target;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        for (int i = 0; i < _movesPerTurn; i++)
        {
            Vector3Int nextPosition = SearchOptimazingPath(_targetPosition , _currentPosition);
            
            if (!IsValidPosition(nextPosition) || nextPosition == _currentPosition )
            {
                // Если дальше идти нельзя, завершаем движение                
                break;
            }
            MoveInDirection(nextPosition);
            yield return new WaitForSeconds(_speed);
        }

        // чтобы завершить ожидание в ComputerAI
       IsMoving = false;
    }

    private void MoveInDirection(Vector3Int direction)
    {
        // Определяем новую позицию, куда нужно переместиться
        Vector3Int newPosition =  direction;

        // Получаем мировые координаты новой позиции
        Vector3 targetPosition = _tilemap.GetCellCenterWorld(newPosition);
        Debug.Log("targetPosition" + targetPosition);

        // Начинаем плавное перемещение
        StartCoroutine(MoveToPosition(targetPosition));
    }

    // Метод для плавного перемещения к указанной точке
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * (_speed / Vector3.Distance(transform.position, targetPosition)); // Скорость зависит от расстояния до цели
            transform.position = Vector3.Lerp(transform.position, targetPosition, t); // Интерполируем положение
            yield return null; // Ждем один кадр
        }
        transform.position = targetPosition; // Устанавливаем точное значение после завершения анимации

        // Обновляем текущую позицию
        _currentPosition = _tilemap.WorldToCell(transform.position);
    }

    /// <summary>
    /// Метод выбора оптимального пути с отклонением от препятсвий, без их обхода
    /// </summary>
    /// <param name="target"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector3Int SearchOptimazingPath(Vector3 target, Vector3 position)
    {

        float deltaX = target.x - position.x;
        float deltaY = target.y - position.y;

        RaycastHit2D hitX = Physics2D.Linecast(new Vector3(position.x + Mathf.Sign(deltaX), position.y, 0), target);
        RaycastHit2D hitY = Physics2D.Linecast(new Vector3(position.x, position.y + Mathf.Sign(deltaY), 0), target);

        if (hitX.collider == null && hitY.collider == null)
        {

            if (Mathf.Abs(deltaX) >= Mathf.Abs(deltaY))
            {

                return _tilemap.WorldToCell(new Vector3(position.x + Mathf.Sign(deltaX), position.y, 0));
            }
            else
            {

                return _tilemap.WorldToCell(new Vector3(position.x, position.y + Mathf.Sign(deltaY), 0));
            }
        }
        else if (hitX.collider == null && hitY.collider != null)
        {
            return _tilemap.WorldToCell(new Vector3(position.x + Mathf.Sign(deltaX), position.y, 0));
        }
        else if (hitX.collider != null && hitY.collider == null)
        {
            return _tilemap.WorldToCell(new Vector3(position.x, position.y + Mathf.Sign(deltaY), 0));
        }
        else
        {
            return _tilemap.WorldToCell(position);
        }
    }

    private bool IsValidPosition(Vector3Int position)
    {
        if(ComputerAI.Instance.CellIsOccupied(position)) return false;
        // Проверяем, является ли позиция допустимой 
        RaycastHit2D hit = Physics2D.Linecast(transform.position, position + _tilemap.cellSize * 0.5f);
        return hit.collider == null || hit.collider.CompareTag("Enemy"); 
    }
}
