using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject _interactionWorldUI;


    private void Awake()
    {
        if (_interactionWorldUI != null)
        {
            _interactionWorldUI.SetActive(false);
        }
    }

    public void Interact()
    {
        //Debug.Log("Interacted with: " + gameObject.name);
    }

    public void OnInteractionAvailable()
    {
        //Debug.Log("Player near: " + gameObject.name);
        _interactionWorldUI.SetActive(true);
    }

    public void OnInteractionUnavailable()
    {
        //Debug.Log("Player left: " + gameObject.name);
        _interactionWorldUI.SetActive(false);
    }
}