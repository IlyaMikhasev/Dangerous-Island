using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Charaster : MonoBehaviour
{
    public static Charaster Instance { get; private set; }
    public static UnityEvent<int> StepCountUpdate = new();
    
    public int Step { get; private set;}

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private AudioClip[] _walk;
    [SerializeField] private float _speed;
    [SerializeField] private Transform[] _heroes;
    [SerializeField] private LayerMask _itemLayerMask;

    private GameInstance _gameInstance;
    private Transform _charasterTransform;
    private Hero _hero;
    private SoundManager _soundManager;
    private CanvasInstance _canvas;
    private SpriteRenderer spriteRenderer;
    private bool isMoving;
    private Vector3 targetPosition;
    private int _index;
    private int _life;
    private bool _reachedDestination;
    private LayerMask _maskInvizobility;
    public void SetStep(int step) { Step = step; StepCountUpdate?.Invoke(Step); } 
    /// <summary>
    /// Изменение количества жизней (получение урона , лечение), при достижение нуля вызывает смерть персонажа
    /// </summary>
    /// <param name="deltaLife"></param>
    public void СhangeLifeQuantity(int deltaLife)
    {
        isMoving = false;
        if ((_life += deltaLife) <= 0) Die();
        switch (deltaLife)
        {
            case 1:; break;
            case -1:  _hero.Hurt(); break;
            case 0: ; break;
            default:
                break;
        }

    }
    public void Attack() {
        _hero.Attack();
    }
    private void Awake()
    {
        Instance = this;
        _gameInstance = GameInstance.Instance;
        _canvas = CanvasInstance.Instance;
        _soundManager = FindObjectOfType<SoundManager>();
    }
    private void Start()
    {
        _index = PlayerPrefs.GetInt("SelectPlayer");
        var charasters = transform.Find("Charasters");
        if (charasters == null)
        {
            charasters = transform.root.Find("Charasters");
        }        
        if (charasters != null)
        {
            _charasterTransform = Instantiate(_heroes[_index],charasters);
            _charasterTransform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Object 'Charasters' not found in the hierarchy.");
        }
        _maskInvizobility = 1<<_charasterTransform.gameObject.layer | _itemLayerMask;
        _hero = _charasterTransform.GetComponent<Hero>();
        spriteRenderer = _charasterTransform.GetComponent<SpriteRenderer>();
        SetStep(PlayerPrefs.GetInt("steps"));
        _life = PlayerPrefs.GetInt("lifes");
    }
    
    private void Update()
    {
        if (GameInstance.Instance.CurrentState != GameState.PlayerTurn)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !isMoving )
        {
            
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, targetPosition,~_maskInvizobility);

            if (hit.collider == null )
            {
                Walk();
            }
                else if (hit.collider.CompareTag("Enemy"))
                {
                        if (hit.transform.gameObject.GetComponent<Enemy>().IsPlayerDanger)
                        {
                            GameInstance.Instance.StartBattle(hit.transform);
                        }
                            else
                            {
                                _canvas.Info("Обнаружен враг, лучше держаться от него подальше");
                            }
                }
                    else
                    {
                        _canvas.Info("На пути обнаруженно препятствие");
                        return;
                    }
        }
    }
    private void Walk()
    {
        if (Step > 0)
        {            
            isMoving = true;
            _hero.Walk(isMoving);
            _soundManager.PlaySound(_walk[UnityEngine.Random.Range(0,1)]);
            _reachedDestination = !isMoving;
            StartCoroutine(MoveToDestination(targetPosition));
        }
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
        ScalePlayer(deltaX < 0);
        
        RaycastHit2D hitX = Physics2D.Linecast(new Vector3(position.x + Mathf.Sign(deltaX), position.y, 0), target, ~_maskInvizobility);
        RaycastHit2D hitY = Physics2D.Linecast(new Vector3(position.x, position.y + Mathf.Sign(deltaY), 0), target, ~_maskInvizobility);

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
    /// <summary>
    /// Метод проверяющий , дошел ли персонаж до цели , с учетом погрешности
    /// </summary>
    /// <param name="currentCell"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    private bool IsReachedDestination(Vector3Int currentCell, Vector3Int destination)
    {
        return currentCell == destination || Vector3.Distance(_tilemap.CellToWorld(currentCell), _tilemap.CellToWorld(destination)) < 0.1f;
    }
    /// <summary>
    /// Coroutine дожидается перемещения персонажа на 1 клетку и запускает продолжение движения , в зависимоти от количества шагов
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    private IEnumerator MoveToDestination(Vector3 destination)
    {

        Vector3Int clickPosition = _tilemap.WorldToCell(destination);
        Vector3Int currentCell = _tilemap.WorldToCell(transform.position);
        Vector3 currentPosition = transform.position;

        while (!_reachedDestination && Step > 0)
        {
            Vector3Int nextCell = SearchOptimazingPath(destination, currentPosition);
            Vector3 nextPosition = _tilemap.CellToWorld(nextCell) + _tilemap.cellSize * 0.5f;
            yield return StartCoroutine(MoveSmoothly(currentPosition, nextPosition));

            currentCell = nextCell;
            currentPosition = nextPosition;
            Step--;
            StepCountUpdate?.Invoke(Step);
            _reachedDestination = IsReachedDestination(currentCell, clickPosition);
        }

        transform.position = currentPosition;
        isMoving = false;
        _hero.Walk(isMoving);
    }
    /// <summary>
    /// Coroutine передвигающая персонажа
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private IEnumerator MoveSmoothly(Vector3 start, Vector3 end)
    {        
        float elapsedTime = 0;

        while (elapsedTime < _speed)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / _speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = end;
    }
    /// <summary>
    /// разворот спрайта персонажа, относительно оси X
    /// </summary>
    /// <param name="isScale"></param>
    private void ScalePlayer(bool isScale)
    {
        if (isScale)
        {
            spriteRenderer.flipX = true;
        }
        else 
        {
            spriteRenderer.flipX = false;
        }
    }
    private void Die()
    {
        _hero.Die();
    }

   
}
