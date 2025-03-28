using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : Item
{
    
    public override void UseItem()
    {
        CanvasInstance.Instance.Info("Выберите место для взрыва");
    }
}
