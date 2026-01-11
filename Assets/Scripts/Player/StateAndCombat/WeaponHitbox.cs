using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    private Collider _collider;
    private float _currentDamage;
    private GameObject _owner;
    private List<GameObject> _hitTargets = new List<GameObject>();

    [Header("Audio")]
    public AK.Wwise.Event Play_Attack_Hit_Enemy;

    [Header("Audio - Attack Type Switches")]
    public AK.Wwise.Switch SW_MC_AttackMeleeLightHit;
    public AK.Wwise.Switch SW_MC_AttackMeleeHeavyHit;

    private string _currentAttackType = "Light";

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        DisableHitbox();
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    public void SetDamage(float damage)
    {
        _currentDamage = damage;
    }

    public void SetAttackType(string attackType)
    {
        _currentAttackType = attackType;
    }

    public void EnableHitbox()
    {
        _hitTargets.Clear();
        _collider.enabled = true;
    }

    public void DisableHitbox()
    {
        _collider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner) return;

        if (other.isTrigger) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            var damageableComponent = damageable as Component;
            if (damageableComponent == null) return;

            GameObject enemyRoot = damageableComponent.gameObject;

            if (_hitTargets.Contains(enemyRoot)) return;

            damageable.TakeDamage(_currentDamage);
            _hitTargets.Add(enemyRoot);

            if (Play_Attack_Hit_Enemy != null && Play_Attack_Hit_Enemy.IsValid())
            {
                switch (_currentAttackType)
                {
                    case "Light":
                        SW_MC_AttackMeleeLightHit.SetValue(other.gameObject);
                        CameraManager.Instance.ShakeCamera(CameraManager.ShakeType.LightAttack);
                        break;
                    case "Heavy":
                        SW_MC_AttackMeleeHeavyHit.SetValue(other.gameObject);
                        CameraManager.Instance.ShakeCamera(CameraManager.ShakeType.HeavyAttack);
                        break;
                }
                Play_Attack_Hit_Enemy.Post(other.gameObject);
            }

            Debug.Log($"Hit {enemyRoot.name} part {other.name} for {_currentDamage} damage");
        }
    }
}