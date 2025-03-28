using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Items : MonoBehaviour
{    
    [SerializeField] private Inventory _inventory;
    [SerializeField] private GameObject _prefabBomb;
    [SerializeField] private GameObject _prefabWood;

    private Dictionary<Item,int> _hasItem;

    private void Awake()
    {
        _hasItem = new Dictionary<Item,int>();
    }
    public void AddItem(Item item)
    {
        if (_hasItem.ContainsKey(item)) {
            _hasItem[item] += 1;
            _inventory.NewItemSlot(item, _hasItem[item]);
        }
        else _hasItem.Add(item, 1);
        _inventory.NewItemSlot(item, _hasItem[item]);
    }
    private void RemoveItem(Item item) {

        if (_hasItem.ContainsKey(item))
        {
           if( _hasItem[item] > 1) _hasItem[item] -= 1;
           else _hasItem.Remove(item);
        }
        else return;
    }
    public void ItemUse(int itemId)
    {
        foreach (Item item in _hasItem.Keys) {
            if (item.Id == itemId) {
                if (itemId == 4 || itemId == 5) {
                    CustomCursor customCursor = FindAnyObjectByType<CustomCursor>();
                    customCursor.ItemToCursor(itemId == 4 ? _prefabBomb : _prefabWood);
                }
                item.UseItem();
                if (itemId != 2)
                    RemoveItem(item);
                return;
            }
        }
    }
   
}
