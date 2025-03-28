using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItem : Item
{
    public override void UseItem()
    {
        GameInstance.Instance.EndGameColor(); CanvasInstance.Instance.Info("¬ы нашли клад! осталось добратьс€ до лодки");
    }

   
}
