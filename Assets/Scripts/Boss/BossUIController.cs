using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BossUIController : MonoBehaviour
{
    public static BossUIController Instance;

    [Header("Entrance Animation")]
    public float appearDuration = 0.8f;
    public Ease appearEase = Ease.OutBack;
    public float fillUpDuration = 1.5f;
    public Ease fillUpEase = Ease.OutCubic;
    public Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("Health Bar Settings")]
    public float instantUpdateSpeed = 0.2f;
    public float delayedBarWaitTime = 0.5f;
    public float delayedBarCatchUpSpeed = 1f;

    [Header("References")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI bossNameText;
    public Slider instantHpSlider;
    public Slider delayedHpSlider;
    public RectTransform containerRect;

    private Sequence _introSequence;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        canvasGroup.alpha = 0f;
        containerRect.localScale = startScale;
        if (bossNameText != null) bossNameText.alpha = 0f;
    }

    public void ActivateBossHealthBar(float maxHealth, string name)
    {
        if (_introSequence != null) _introSequence.Kill();

        canvasGroup.DOKill();
        containerRect.DOKill();
        bossNameText.DOKill();
        instantHpSlider.DOKill();
        delayedHpSlider.DOKill();

        bossNameText.text = name;
        bossNameText.alpha = 0f;

        instantHpSlider.minValue = 0f;
        instantHpSlider.maxValue = maxHealth;
        delayedHpSlider.minValue = 0f;
        delayedHpSlider.maxValue = maxHealth;

        instantHpSlider.value = 0f;
        delayedHpSlider.value = 0f;

        canvasGroup.alpha = 0f;
        containerRect.localScale = startScale;

        _introSequence = DOTween.Sequence();

        _introSequence.Append(canvasGroup.DOFade(1f, appearDuration));
        _introSequence.Join(containerRect.DOScale(Vector3.one, appearDuration).SetEase(appearEase));
        _introSequence.Join(bossNameText.DOFade(1f, appearDuration));

        _introSequence.Append(instantHpSlider.DOValue(maxHealth, fillUpDuration).SetEase(fillUpEase));
        _introSequence.Join(delayedHpSlider.DOValue(maxHealth, fillUpDuration).SetEase(fillUpEase));
    }

    public void UpdateHealth(float currentHealth)
    {
        if (_introSequence != null && _introSequence.IsActive())
        {
            _introSequence.Kill();
            canvasGroup.alpha = 1f;
            containerRect.localScale = Vector3.one;
            bossNameText.alpha = 1f;
            instantHpSlider.value = instantHpSlider.maxValue;
            delayedHpSlider.value = delayedHpSlider.maxValue;
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