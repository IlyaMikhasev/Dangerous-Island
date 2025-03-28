using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private Text CountItem; 
    private Image Icon;
    public int Index;
   
    public void InicializationSlot(Item item,int count) {
        if(Icon == null)
            Icon = GetComponent<Image>();
        Icon.sprite = item.Sprite as Sprite;        
        Index = item.Id;
        if(CountItem == null)
            CountItem = GetComponentInChildren<Text>(true);
        CountItem.text = count.ToString();
    }
    public void ItemClick() {
        Items items  = FindAnyObjectByType<Items>();
        items.ItemUse(Index);
    }
}
