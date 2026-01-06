using UnityEngine;

public class BossObjectToggle : MonoBehaviour
{
    [Header("Boss Reference")]
    [SerializeField] private BossController _boss;

    [Header("Objects to Control")]
    [SerializeField] private GameObject[] _objectsToToggle;

    [Header("Settings")]
    [Tooltip("If true, destroys objects. If false, just deactivates them.")]
    [SerializeField] private bool _destroyObjects = false;

    [Tooltip("Check boss state every X seconds")]
    [SerializeField] private float _checkInterval = 0.5f;

    private bool _bossActivated = false;
    private float _checkTimer = 0f;



    void Update()
    {
        if (_bossActivated) return;

        _checkTimer += Time.deltaTime;

        if (_checkTimer >= _checkInterval)
        {
            _checkTimer = 0f;
            CheckBossActivation();
        }
    }

    void CheckBossActivation()
    {
        if (_boss == null) return;

        if (_boss.IsActive)
        {
            _bossActivated = true;

            if (_destroyObjects)
            {
                DestroyObjects();
            }
            else
            {
                SetObjectsActive(true);
            }
        }
    }

    void SetObjectsActive(bool active)
    {
        foreach (var obj in _objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
        }
    }

    void DestroyObjects()
    {
        foreach (var obj in _objectsToToggle)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }
}