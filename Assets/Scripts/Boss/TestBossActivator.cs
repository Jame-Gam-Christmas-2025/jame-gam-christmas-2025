using UnityEngine;

public class TestBossActivator : MonoBehaviour
{
    [Header("Boss Reference")]
    [SerializeField] private BossController _boss;

    [Header("Activation")]
    [SerializeField] private bool _activate;

    void Update()
    {
        if (_activate)
        {
            _activate = false;

            if (_boss != null)
            {
                _boss.IsActive = true;
                //Debug.Log($"Boss {_boss.name} activated!");
            }
            else
            {
                Debug.Log("Boss not assigned!");
            }
        }
    }
}