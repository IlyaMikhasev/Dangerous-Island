using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodItem : Item
{
    public override void UseItem()
    {
        CanvasInstance.Instance.Info("Установите преграду");
    }
}
