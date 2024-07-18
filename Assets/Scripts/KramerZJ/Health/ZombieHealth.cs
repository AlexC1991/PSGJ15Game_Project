using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectDungeonCrawlerPJ15
{
    public class ZombieHealth : Health
    {
        [SerializeField] private float _headshot_Damage_Factor = 2.5f;
        public void Damage(int in_damage, bool is_Head)
        {
            if (is_Head)
            {
                base.Damage(Mathf.RoundToInt(in_damage * _headshot_Damage_Factor));
                return;
            }
            base.Damage(in_damage);
        }

    }
}
