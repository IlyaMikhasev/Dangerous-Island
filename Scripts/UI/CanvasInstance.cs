using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static UnityEvent<int> EndBattle = new();
    public static CanvasInstance Instance;

    [SerializeField] private Image _prefabLife;
    [SerializeField] private Charaster _charaster;
    [SerializeField] private Sprite[] _handles;
    [SerializeField] private TextMeshProUGUI _textBattle;
    [SerializeField] private TextMeshProUGUI _countStep;
    [SerializeField] private TextMeshProUGUI _countMoveTurn;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private Button _restartBattle;

    private GameInstance _gameInstance;
    private GameObject _resaultBattleP;
    private Transform _infoObject;
    private int _heart, _tmpHeart,_resB;
    private Image[] _lifes;
    private Dictionary<int, string> _kayValueBattle = new Dictionary<int, string>{ {-1,"Поражение" },{0,"Ничья" },{1,"Победа" } };
    private void Awake()
    {
        Instance = this;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _gameInstance.SwitchToUseUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _gameInstance.SwitchToTurn();
    }
    private void Start()
    {
        _gameInstance = GameInstance.Instance;
        Charaster.StepCountUpdate.AddListener(UpdateStepText);
        GameInstance.MoveTurnCountUpdate.AddListener(UpdateCountTurnText);
        _infoObject = transform.GetChild(0);
        _resaultBattleP = transform.GetChild(2).gameObject;
        HeartDisplay(PlayerPrefs.GetInt("lifes"));
    }
    /// <summary>
    /// Запуск корутины для вывода информационных сообщений пользователю
    /// </summary>
    /// <param name="info"></param>
    public void Info(string info)
    {
        StartCoroutine(Instance.Information(info));
    }
    private IEnumerator Information(string info)
    {
        _infoObject.gameObject.SetActive(true);
        TextMeshProUGUI informationText = _infoObject.GetComponent<TextMeshProUGUI>();
        informationText.text = info;
        yield return new WaitForSeconds(2f);
        _infoObject.gameObject.SetActive(false);
    }
    /// <summary>
    /// Принимает количество жизней у персонажа, создает массив и выводит на показтель жизни
    /// </summary>
    /// <param name="life"></param>
    private void HeartDisplay(int heart)
    {
        _heart = heart;
        _tmpHeart = heart;
        _lifes = new Image[_heart];

        for (int i = 0; i < _lifes.Length; i++)
        {
            // Создаем новый экземпляр изображения жизни
            _lifes[i] = Instantiate(_prefabLife, transform);

            // Изменяем положение изображения
            RectTransform rt = _lifes[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(50 + i * 75, -50);
        }
    }
    /// <summary>
    /// Закрашивание жизней
    /// </summary>
    /// <param name="deltaLife"></param>
    private void LifeUpdate(int deltaLife)
    {
        Debug.Log(_tmpHeart);
        _tmpHeart = _tmpHeart + deltaLife;
        if (_tmpHeart < 0) return;
        for (int i = 0; i < _heart; i++)
        {
            _lifes[i].color = Color.white;
            if (_tmpHeart-1 < i) _lifes[i].color = Color.black;
        }
    }
    /// <summary>
    /// Битва в камень ножницы бумага, между игроком и компьютером
    /// </summary>
    public void Battle()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }
    public void ResBattle(int indexHand)
    {
        transform.GetChild(1).gameObject.SetActive(false);        
        _textBattle.text = String.Empty;
        int resIA = UnityEngine.Random.Range(0, 2);
        _resaultBattleP.SetActive(true);
        StartCoroutine(RandomComputerHand(indexHand, resIA));
        Transform image = _resaultBattleP.transform.Find("PlayerHandle");
        image.GetComponent<Image>().sprite = _handles[indexHand];
    }
    private IEnumerator RandomComputerHand(int playerH, int computerHandIndex)
    {
        Sprite cHand = _handles[computerHandIndex];
        Transform image = _resaultBattleP.transform.Find("ComputerHandle");
        if (computerHandIndex >= _handles.Length || computerHandIndex < 0)
            yield break; // Проверяем границы индекса

        // Сохраняем текущую позицию панели
        Vector3 startRotation = image.rotation.eulerAngles;

        // Угол, до которого нужно дойти
        Vector3 targetRotation = new Vector3(startRotation.x, startRotation.y + 360f, startRotation.z);

        // Время начала анимации
        float startTime = Time.time;

        while (Time.time < startTime + rotationDuration)
        {
            // Интерполируем угол от начального значения к целевому
            image.rotation = Quaternion.Euler(Vector3.Lerp(startRotation, targetRotation, (Time.time - startTime) / rotationDuration));

            // Ждем следующий кадр
            yield return null;
        }

        // Устанавливаем конечное значение угла
        image.rotation = Quaternion.Euler(targetRotation);

        // Устанавливаем новую картинку
        image.GetComponent<Image>().sprite = cHand;
        ResText(playerH, computerHandIndex);
    }

    private void ResText(int playerH, int computerH)
    {

        if (playerH == computerH)
        {
            _resB = 0;
            _restartBattle.gameObject.SetActive(true); 
        }
        else if ((playerH == 0 && computerH == 1) || (playerH == 1 && computerH == 2) || (playerH == 2 && computerH == 0))
        {
            _resB = 1;
        }
        else
        {
            _resB = -1;
        }
        _textBattle.text = _kayValueBattle[_resB];
    }
    /// <summary>
    /// Закрывает окно битвы 
    /// </summary>
    public void ContinueGame()
    {
        _resaultBattleP.SetActive(false);
        EndBattle?.Invoke(_resB);
        if(_resB == -1) LifeUpdate(_resB);

    }
    public void RestartBatle() {
        transform.GetChild(1).gameObject.SetActive(true);
        _resaultBattleP.SetActive(false);
        _restartBattle.gameObject.SetActive(false);
    }
    /// <summary>
    /// Обновляет на экране количество доступных шагов
    /// </summary>
    /// <param name="count"></param>
    private void UpdateStepText(int count) {
        _countStep.text = count.ToString();
    }
    /// <summary>
    /// Обновляет количество ходов сделанных Игроком
    /// </summary>
    /// <param name="count"></param>
    private void UpdateCountTurnText(int count)
    {
        _countMoveTurn.text = count.ToString();
    }
    private void OnDisable()
    {
        GameInstance.MoveTurnCountUpdate.RemoveListener(UpdateCountTurnText);
        Charaster.StepCountUpdate.RemoveListener(UpdateStepText);
    }
    
}
