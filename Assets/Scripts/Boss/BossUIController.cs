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
        canvasGroup.DOKill();
        containerRect.DOKill();
        instantHpSlider.DOKill();
        delayedHpSlider.DOKill();
        bossNameText.DOKill();

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

        Sequence introSequence = DOTween.Sequence();

        introSequence.Append(canvasGroup.DOFade(1f, appearDuration));
        introSequence.Join(containerRect.DOScale(Vector3.one, appearDuration).SetEase(appearEase));

        introSequence.Join(bossNameText.DOFade(1f, appearDuration));

        introSequence.Append(instantHpSlider.DOValue(maxHealth, fillUpDuration).SetEase(fillUpEase));
        introSequence.Join(delayedHpSlider.DOValue(maxHealth, fillUpDuration).SetEase(fillUpEase));
    }

    public void UpdateHealth(float currentHealth)
    {
        instantHpSlider.DOKill();
        instantHpSlider.DOValue(currentHealth, instantUpdateSpeed).SetEase(Ease.OutCubic);

        delayedHpSlider.DOKill();
        delayedHpSlider.DOValue(currentHealth, delayedBarCatchUpSpeed)
            .SetDelay(delayedBarWaitTime)
            .SetEase(Ease.OutCubic);
    }

    public void HideBossHealthBar()
    {
        canvasGroup.DOFade(0f, 0.5f);
    }
}