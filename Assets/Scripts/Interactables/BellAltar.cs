using UnityEngine;

public class BellAltar : Interactable
{
    public float arenaRadius;

    [SerializeField] private ParticleSystem _arenaParticleSyst;
    [SerializeField] private Color _arenaParticleColor;
    [SerializeField] private string _arenaBossName;

    // TODO handle active
    private bool _isActive;
    [SerializeField] private GameObject _bellAltarProps;

    void OnValidate()
    {
        UpdateParticleSystem();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateParticleSystem();
        _arenaParticleSyst.Stop();
    }

    void Update()
    {
        ClampPlayerPosition();
    }

    // Update is called once per frame
    void UpdateParticleSystem()
    {
        if(_arenaParticleSyst != null)
        {
            ParticleSystem.ShapeModule particuleSyst = _arenaParticleSyst.shape;
            particuleSyst.radius = arenaRadius;

            ParticleSystem.MainModule mainModule = _arenaParticleSyst.main;
            mainModule.startColor = _arenaParticleColor;
        }
    }

    void ClampPlayerPosition()
    {
        if(_isActive)
        {
            GameObject playerGameObject = GameObject.Find("Player");

            if(playerGameObject != null)
            {
                Vector3 playerPosition = playerGameObject.transform.position;
                Vector3 arenaCenter = _arenaParticleSyst.transform.position;

                float arenaPlayerDistance = Vector3.Distance(playerPosition, arenaCenter);
                float worldArenaRadius = arenaRadius * transform.lossyScale.x;

                if (arenaPlayerDistance > worldArenaRadius) {
                    Vector3 direction = (playerPosition - arenaCenter).normalized;
                    playerGameObject.transform.position = arenaCenter + direction * worldArenaRadius;
                }
            } else
            {
                Debug.Log("BellAltar Player ref null, Player is required in the scene");
            }

        }
    }

    public override void Interact()
    {
        base.Interact();

        ToggleAltar();
    }

    public override void OnInteractionAvailable()
    {
        base.OnInteractionAvailable();
    }

    public override void OnInteractionUnavailable()
    {
        base.OnInteractionUnavailable();
    }
    void ToggleAltar()
    {
        if(!_isActive)
        {
            _isActive = !_isActive;


            if(_bellAltarProps != null)
            {
                CapsuleCollider altarCollider = _bellAltarProps.GetComponent<CapsuleCollider>();

                _bellAltarProps.SetActive(false);
                altarCollider.enabled = false;

                GameObject player = GameObject.Find("Player");
            
                if(player != null)
                {
                    PlayerInteractor playerInteractor = player.GetComponent<PlayerInteractor>();
                    playerInteractor.DisableInteraction();
                }

                _arenaParticleSyst.Play();
                AudioManager.Instance.PlayBellSFX(gameObject);

                Debug.Log(_bellAltarProps.transform.position);
                GameManager.Instance.SpawnBoss(_arenaBossName, _bellAltarProps.transform.position);
            }
        }
    }
}
