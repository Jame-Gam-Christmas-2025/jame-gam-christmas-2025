using UnityEngine;

public class HealthVFXScaler : MonoBehaviour
{
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private Transform _vfxTransform;
    [SerializeField] private float _minScale = 0.5f;
    [SerializeField] private float _maxScale = 2f;

    void Update()
    {
        if (_playerState == null || _vfxTransform == null)
            return;

        float healthRatio = _playerState.CurrentHealth / _playerState.MaxHealth;
        float newScale = Mathf.Lerp(_minScale, _maxScale, healthRatio);

        _vfxTransform.localScale = Vector3.one * newScale;
    }
}