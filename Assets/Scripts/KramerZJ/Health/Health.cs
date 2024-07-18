using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectDungeonCrawlerPJ15
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _max_health = 100;//needed for UI
        [SerializeField] private int _curr_health;
        // Start is called before the first frame update
        void Start()
        {
            _curr_health = _max_health;//or if we have saving file, use number in saving file
        }

        public virtual void Damage(int input_Damage)
        {
            _curr_health -= input_Damage;
            if (_curr_health<=0)
            {
                _curr_health = 0;
                Die();
            }
        }
        protected virtual void Die()
        {
            Destroy(this.gameObject);
        }
    }
}
