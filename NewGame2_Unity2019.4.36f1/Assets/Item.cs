using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Item.IStats
{
    public Entity entity;

    public Stats stats = new Stats(); //Maybe call this selfStats and make a new Stats var. that's called entityStats/enhancingStats that represents what stats are enhanced when an Entity wields this Item

    /// <summary>
    /// Stats of an Item correlate to either enhancements of Stats of Entities wielding that Item and/or to Stats of the Item itself (e.g. a Sword's strength (really its damage) will be multiplied by the wielding Entities strength, thereby enhancing
    /// Entity Stats AND being a Stats belonging to itself. A Shield's strength may increase the strength of the wielding Entity, but, unless used as a weapon, does not correlate to damage that the Shield can deal. A Charm can only enhance the 
    /// wielding Entity's Stats because a Charm cannot be used as a weapon/shield/etc.).
    /// </summary>
    [Serializable]
    public class Stats
    {
        public float health = 1f;
        public float stamina = 1f;
        public float strength = 1f;
        public float toughness = 1f;
        public float durability = 1f;
    }

    public interface IStats
    {
        Entity GetEntity();

        float GetHealth();
        float GetStamina();
        float GetStrength();
        float GetToughness();
        float GetDurability();
    }

    public Entity GetEntity()
    {
        return entity;
    }

    public float GetHealth()
    {
        return stats.health;
    }

    public float GetStamina()
    {
        return stats.stamina;
    }

    public float GetStrength()
    {
        return stats.strength;
    }

    public float GetToughness()
    {
        return stats.toughness;
    }

    public float GetDurability()
    {
        return stats.durability;
    }
}
