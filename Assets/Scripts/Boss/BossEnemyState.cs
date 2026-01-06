using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using ImpactFrameSystem;
using UnityEngine.Rendering;
using DG.Tweening;
using UI;

public class BossEnemyState : MonoBehaviour, IDamageable
{
    [SerializeField] private BossConfig _config;
    [SerializeField] private BossController _bossController;

    [Header("Impact Frame Settings")]
    [SerializeField] private float _impactDuration = 2f;
    [SerializeField] private float _impactIntensity = 1.2f;

    [Header("Death Settings")]
    [SerializeField] private float _deathDelay = 0.3f;

    [Header("Post Processing")]
    [SerializeField] private Volume bossAreaPostProcess;

    [Header("On end")]
    [SerializeField] private GameObject bossLimitWalls;
    [SerializeField] private DialogueData bossFirstDialogue;

    public UnityEvent OnDeath;
    public UnityEvent OnDamageTaken;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => _config.maxHealth;
    public bool IsDead { get; private set; }

    void Start()
    {
        CurrentHealth = MaxHealth;

        OnDeath.AddListener(OnDeathEvent);
    }

    /// <summary>
    /// Method used for the variable OnDeath.
    /// </summary>
    private void OnDeathEvent()
    {
        // Transition of Post Process Volume
        bossAreaPostProcess.weight = 1f;
        DOTween.To(() => bossAreaPostProcess.weight, x => bossAreaPostProcess.weight = x, 0f, 3f);

        // Update game progression
        GameManager.Instance.DefeatBoss(gameObject.name);

        // Destroy boss walls
        if (bossLimitWalls) Destroy(bossLimitWalls);

        // Restore player health
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerState playerState = player.GetComponent<PlayerState>();
        playerState.Heal(playerState.MaxHealth);

        // Set dialogue for santa
        if (name == "Santa")
        {
            PlayerAlignment playerAlignment = player.GetComponent<PlayerAlignment>();

            if (playerAlignment.AlignmentScore >= 0f)
            {
                bossFirstDialogue = Resources.Load<DialogueData>($"Data/DialogueSystem/SantaEnding1/Dialogue_SantaEnding1_0");
            }
            else
            {
                bossFirstDialogue = Resources.Load<DialogueData>($"Data/DialogueSystem/SantaEnding2/Dialogue_SantaEnding2_0");
            }
        }

        UnityEvent dialogueEndEvent = new UnityEvent();

        FindFirstObjectByType<DialogueView>(FindObjectsInactive.Include).StartNewDialogue(bossFirstDialogue, dialogueEndEvent);


        /* // Start fade in
        CanvasGroup canvasGroup = GameObject.FindGameObjectWithTag("Fade").GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1f, 2f).OnComplete(() =>
        {
            UnityEvent dialogueEndEvent = new UnityEvent();
            dialogueEndEvent.AddListener(() =>
            {
                CanvasGroup canvasGroup = GameObject.FindGameObjectWithTag("Fade").GetComponent<CanvasGroup>();
                canvasGroup.DOFade(0f, 2f);
                if(GameManager.Instance.LastDefeatedBoss() != "Santa")
                {
                    CanvasGroup canvasGroup = GameObject.FindGameObjectWithTag("Fade").GetComponent<CanvasGroup>();
                    canvasGroup.DOFade(0f, 2f);
                } else
                {
                    GameSceneManager.Instance.LoadSceneByName("EndingScene");
                }


            });

            // Launch dialogue
            FindFirstObjectByType<DialogueView>(FindObjectsInactive.Include).StartNewDialogue(bossFirstDialogue, dialogueEndEvent);
        }); */
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        if (!_bossController.IsActive) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        OnDamageTaken?.Invoke();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        var bossController = GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.IsActive = false;
        }

        
        if (ImpactFrameManager.Instance != null)
        {
            ImpactFrameManager.Instance.TriggerImpactFrame(_impactDuration, _impactIntensity, transform.position);
        }

        
        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        
        yield return new WaitForSeconds(_deathDelay);

        OnDeath?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("On death event")]
    public void EditorDeathEvent()
    {
        OnDeath?.Invoke();
    }
#endif
}