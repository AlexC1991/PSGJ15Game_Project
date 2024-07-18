using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDungeonCrawlerPJ15
{
    [RequireComponent(typeof(Animator))]
    public class Gun : MonoBehaviour
    {
        [SerializeField] protected bool _bullet_spread = true;
        [SerializeField] protected Vector3 _bullet_spread_variance = new Vector3(0.1f, 0.1f, 0.1f);
        [SerializeField] protected ParticleSystem _shooting_system, _impact_particle, _zombie_impact_particle;
        [SerializeField] protected Transform _bullet_spawn_point;
        [SerializeField] protected TrailRenderer _bulletTrail;
        [SerializeField] protected float _shoot_delay = 0.3f;
        [SerializeField] protected int _damage_per_hit = 14;
        [SerializeField] protected LayerMask _mask;

        protected Animator _animator;

        protected float _last_shoot_time;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _last_shoot_time = Time.time;
        }
        public virtual void Shoot()
        {
            if (_last_shoot_time + _shoot_delay < Time.time)
            {
                //could've used object pooling
                _animator.SetTrigger("Fire");
                _shooting_system.Play();
                Vector3 direction = GetDirection();
                if (Physics.Raycast(_bullet_spawn_point.position, direction, out RaycastHit hit, float.MaxValue, _mask))
                {
                    TrailRenderer trail = Instantiate(_bulletTrail, _bullet_spawn_point.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hit));

                    _last_shoot_time = Time.time;
                }
                else
                {
                    TrailRenderer trail = Instantiate(_bulletTrail, _bullet_spawn_point.position, Quaternion.identity);

                    StartCoroutine(SpawnTrail(trail, direction));
                }
            }
        }

        protected IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 Direction)
        {
            float time = 0;
            Vector3 startPos = Trail.transform.position;
            while (time < 1)
            {
                Trail.transform.position = Vector3.Lerp(startPos, startPos + Direction * 100f, time);
                time += Time.deltaTime / Trail.time;
                yield return null;
            }
            Trail.transform.position = startPos + Direction * 100f;


            Destroy(Trail.gameObject, Trail.time);
        }

        protected Vector3 GetDirection()
        {
            Vector3 direction = transform.forward;
            if (_bullet_spread)
            {
                direction += new Vector3(
                    Random.Range(-_bullet_spread_variance.x, _bullet_spread_variance.x),
                    Random.Range(-_bullet_spread_variance.y, _bullet_spread_variance.y),
                    Random.Range(-_bullet_spread_variance.z, _bullet_spread_variance.z)
                    );
                direction.Normalize();
            }
            return direction;
        }

        protected IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
        {
            float time = 0;
            Vector3 startPos = Trail.transform.position;
            while (time < 1)
            {
                Trail.transform.position = Vector3.Lerp(startPos, Hit.point, time);
                time += Time.deltaTime / Trail.time;
                yield return null;
            }
            Trail.transform.position = Hit.point;
            if (Hit.collider.gameObject.layer == LayerMask.NameToLayer("Zombie"))
            {
                Instantiate(_zombie_impact_particle, Hit.point, Quaternion.LookRotation(Hit.normal));
                if (Hit.collider is SphereCollider)
                {
                    if (Hit.collider.gameObject.TryGetComponent<ZombieHealth>(out ZombieHealth _zombieHealth))
                        _zombieHealth.Damage(_damage_per_hit, true);
                }
                else if (Hit.collider is CapsuleCollider)
                {
                    if (Hit.collider.gameObject.TryGetComponent<ZombieHealth>(out ZombieHealth _zombieHealth))
                        _zombieHealth.Damage(_damage_per_hit, false);
                }


            }
            else
            {
                Instantiate(_impact_particle, Hit.point, Quaternion.LookRotation(Hit.normal));
            }

            Destroy(Trail.gameObject, Trail.time);
        }
    }
}