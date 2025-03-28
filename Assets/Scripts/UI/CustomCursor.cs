using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    private GameObject cursorPrefab; // Префаб для визуального курсора
    private GameObject cursorInstance; // Экземпляр префаба
    private bool _itemCursor;

    private void Start()
    {
        _itemCursor = false;
    }
    /// <summary>
    /// меняем курсор на наш предмет
    /// </summary>
    /// <param name="item"></param>
    public void ItemToCursor(GameObject item) {
        cursorPrefab = item;

        Cursor.visible = false; // Отключаем стандартный курсор

        cursorInstance = Instantiate(cursorPrefab); // Создаем экземпляр префаба

        _itemCursor= true;
    }
    // Перемещаем курсор по экрану
    private void Update()
    {
        if (_itemCursor)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // Глубина расположения курсора
            cursorInstance.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            if (Input.GetMouseButtonDown(0)) {
                Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Linecast(transform.position, targetPosition);

                if (hit.collider == null || hit.collider.CompareTag("Enemy"))
                {
                    Instantiate(cursorPrefab, mousePos, Quaternion.identity);
                    OnDisable();
                    return;
                }
                else {

                    CanvasInstance.Instance.Info("Неподходящее место");
                }
                
            }
        }
    }

    private void OnDisable()
    {
        _itemCursor = false; 
        Destroy(cursorInstance); // Уничтожаем экземпляр префаба
        Cursor.visible = true; // Включаем стандартный курсор обратно
    }
}
