using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    
    public int Id = 0;
    public string description;
    public Sprite Sprite;
    private Sprite _spriteItem;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteItem = _spriteRenderer.sprite;
        _spriteRenderer.sprite = Sprite;

    }
    public abstract void UseItem(); 
    /// <summary>
    /// Метод поднятия предметов персонажем при в хождение в collider предмета 
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            Sprite = _spriteItem;
            
            collision.GetComponentInParent<Items>().AddItem(this);
            Destroy(gameObject);
        }
    }

     
    public static bool operator ==(Item item1,Item item2)
    {
        return item1.Id == item2.Id;
    }
    public static bool operator !=(Item item1, Item item2)
    {
        return item1.Id != item2.Id;
    }

}
