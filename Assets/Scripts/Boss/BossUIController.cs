using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BossUIController : MonoBehaviour
{
    public static BossUIController Instance;

    [Header("Settings")]
    public float instantUpdateSpeed = 0.2f;
    public float delayedBarWaitTime = 0.5f;
    public float delayedBarCatchUpSpeed = 1f;

    [Header("Entrance Settings")]
    public float appearDuration = 0.8f;
    public Ease appearEase = Ease.OutBack;
    public float fillUpDuration = 1.5f;
    public Ease fillUpEase = Ease.OutCubic;

    [Header("References")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI bossNameText;
    public Slider instantHpSlider;
    public Slider delayedHpSlider;
    public RectTransform containerRect;

    private Sequence _introSequence;
    private float _cachedMaxHealth;
    private bool _firstHitTaken = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup.alpha = 0f;
        containerRect.localScale = Vector3.zero;
        if (bossNameText != null) bossNameText.alpha = 0f;
    }

    public void ActivateBossHealthBar(float maxHealth, string name)
    {
        _cachedMaxHealth = maxHealth;
        _firstHitTaken = false;

        if (_introSequence != null) _introSequence.Kill();

        canvasGroup.DOKill();
        containerRect.DOKill();
        instantHpSlider.DOKill();
        delayedHpSlider.DOKill();
        if (bossNameText != null) bossNameText.DOKill();

        if (bossNameText != null)
        {
            bossNameText.text = name;
            bossNameText.alpha = 0f;
        }

        instantHpSlider.minValue = 0f;
        instantHpSlider.maxValue = maxHealth;
        instantHpSlider.value = 0f;

        delayedHpSlider.minValue = 0f;
        delayedHpSlider.maxValue = maxHealth;
        delayedHpSlider.value = 0f;

        canvasGroup.alpha = 0f;
        containerRect.localScale = Vector3.zero;

        _introSequence = DOTween.Sequence();

        _introSequence.Append(canvasGroup.DOFade(1f, appearDuration));
        _introSequence.Join(containerRect.DOScale(Vector3.one, appearDuration).SetEase(appearEase));
        if (bossNameText != null) _introSequence.Join(bossNameText.DOFade(1f, appearDuration));

        _introSequence.Append(instantHpSlider.DOValue(maxHealth, fillUpDuration).SetEase(fillUpEase));
        _introSequence.Join(delayedHpSlider.DOValue(maxHealth, fillUpDuration).SetEase(fillUpEase));
    }

    public void UpdateHealth(float currentHealth)
    {
        instantHpSlider.minValue = 0f;
        instantHpSlider.maxValue = _cachedMaxHealth;
        delayedHpSlider.minValue = 0f;
        delayedHpSlider.maxValue = _cachedMaxHealth;

        if (!_firstHitTaken)
        {
            _firstHitTaken = true;

            if (_introSequence != null) _introSequence.Kill();

            canvasGroup.alpha = 1f;
            containerRect.localScale = Vector3.one;
            if (bossNameText != null) bossNameText.alpha = 1f;

            instantHpSlider.value = _cachedMaxHealth;
            delayedHpSlider.value = _cachedMaxHealth;
        }

        instantHpSlider.DOKill();
        delayedHpSlider.DOKill();

        instantHpSlider.DOValue(currentHealth, instantUpdateSpeed).SetEase(Ease.OutCubic);

        if (currentHealth <= 0)
        {
            delayedHpSlider.DOValue(0f, instantUpdateSpeed).SetEase(Ease.OutCubic);
        }
        else
        {
            delayedHpSlider.DOValue(currentHealth, delayedBarCatchUpSpeed)
                .SetDelay(delayedBarWaitTime)
                .SetEase(Ease.Linear);
        }
    }

    public void HideBossHealthBar()
    {
        if (_introSequence != null) _introSequence.Kill();
        instantHpSlider.DOKill();
        delayedHpSlider.DOKill();
        canvasGroup.DOFade(0f, 0.5f);
    }
}