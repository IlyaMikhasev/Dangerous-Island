using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private bool _inventarySet ;
    private Slot[] _slots;

    private void Start()
    {
        _inventarySet = false;
        _slots = gameObject.GetComponentsInChildren<Slot>(true);
        gameObject.SetActive(false);
    }

    public void ButtonInventoryClick()
    {
        // Проверяем текущее состояние инвентаря
        if (_inventarySet)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }
    /// <summary>
    /// метод добавления предметов в инвентарь
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void NewItemSlot(Item item,int count){      
        foreach (Slot slot in _slots) {
            if (slot.Index == 0 )
            {
                slot.gameObject.SetActive(true);
                slot.InicializationSlot(item, count);
                return;
                
            }
            else if (slot.Index == item.Id)
                {
                slot.InicializationSlot(item, count);
                return;
                }
            else
                    continue;
            
        }
    }

    
    // Метод открытия инвентаря
    private void OpenInventory()
    {
        gameObject.SetActive(true);
        _inventarySet = true;
    }

    // Метод закрытия инвентаря
    private void CloseInventory()
    {
        gameObject.SetActive(false);
        _inventarySet = false;
    }
   

}
