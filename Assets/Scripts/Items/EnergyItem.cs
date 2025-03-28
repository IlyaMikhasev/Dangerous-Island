using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyItem : Item
{
    private int _addStep = 3;
    public override void UseItem()
    {
        PlayerPrefs.SetInt("steps", PlayerPrefs.GetInt("steps") + _addStep);
        Charaster.Instance.SetStep(Charaster.Instance.Step + _addStep);
    }
}
