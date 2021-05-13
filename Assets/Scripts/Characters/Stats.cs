using System;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class Stats
    {
        public float maxHealth;
        public float currentHealth;
        public float strength;
        public float defense;
        public float healthMultiplier;
        public float damageMultiplier;

        public void ChangeHealth(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        }

        public void IncreaseStrength(float amount)
        {
            strength += amount;
        }

        public void IncreaseDefense(float amount)
        {
            defense += amount;
        }

        public void AddStatBonus(Stats bonus)
        {

        }

        public void RemoveBonus(Stats bonus)
        {

        }
    }
}