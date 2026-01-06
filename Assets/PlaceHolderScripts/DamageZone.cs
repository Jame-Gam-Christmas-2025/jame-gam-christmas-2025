using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageInterval = 1f;

    private Dictionary<IDamageable, Coroutine> _damageCoroutines = new Dictionary<IDamageable, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        // Ignore trigger colliders
        if (other.isTrigger)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null && !_damageCoroutines.ContainsKey(damageable))
        {
            Coroutine coroutine = StartCoroutine(DamageOverTime(damageable));
            _damageCoroutines.Add(damageable, coroutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Ignore trigger colliders
        if (other.isTrigger)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null && _damageCoroutines.ContainsKey(damageable))
        {
            StopCoroutine(_damageCoroutines[damageable]);
            _damageCoroutines.Remove(damageable);
        }
    }

    private IEnumerator DamageOverTime(IDamageable target)
    {
        while (true)
        {
            target.TakeDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}