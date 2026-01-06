using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color onHoverColor;
    [SerializeField] private Color normalColor;

    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _textMesh.color = onHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _textMesh.color = normalColor;
    }
}
