using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineItem : Item
{
    public override void UseItem()
    {
        Charaster.Instance.ÑhangeLifeQuantity(1);
    }
}
