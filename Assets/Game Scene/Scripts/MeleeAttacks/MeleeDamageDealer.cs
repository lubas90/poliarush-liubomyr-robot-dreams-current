using UnityEngine;
using StateMachineSystem.ServiceLocatorSystem;
using StateMachineSystem;
using Dummies;

namespace BehaviourTreeSystem
{
public class MeleeDamageDealer : MonoBehaviour
{
    [SerializeField] private WeaponData _data;

    private HitScanMelee _melee;
    private IHealthService _healthService;

    private void Start()
    {
        _melee = GetComponent<HitScanMelee>();
        _healthService = ServiceLocator.Instance.GetService<IHealthService>();

        _melee.OnHit += MeleeHitHandler;
    }

    private void MeleeHitHandler(Collider collider)
    {
        if (_healthService.GetHealth(collider, out Health health))
        {
            health.TakeDamage(_data.Damage);
        }
    }

    private void OnDestroy()
    {
        _melee.OnHit -= MeleeHitHandler;
    }
}

}