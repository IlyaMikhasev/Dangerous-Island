using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Hero
{
    private void Start()
    {
        Initialize();
        Setup();
        base._animator = GetComponent<Animator>();
    }
    protected override void Initialize()
    {
        base.Initialize();
        // Дополнительная логика для разбойника, если требуется
    }

    protected override void Setup()
    {
        base.Setup();
        // Специфичная настройка для разбойника, если требуется
    }

}
