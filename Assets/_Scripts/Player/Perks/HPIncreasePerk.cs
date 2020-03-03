﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "HP Increase", menuName = "Perks/HP Increase", order = 1)]
public class HPIncreasePerk : Perk
{
    public float HPBoost;

    public override void Initialize(PlayerController player)
    {
        player.stats.currentHealth = player.stats.health;
        player.stats.currentHealth += HPBoost;
    }
}