using UnityEngine;

public class OnSelectChangeSpriteColor : MonoBehaviour, ISelectable
{
    [Header ("Colors")]
    [SerializeField] Color selectedColor;
    Color normalColor;
    SpriteRenderer spriteRenderer;

    private void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        normalColor = spriteRenderer.color;
    }

    public void OnDeselect ()
    {
        spriteRenderer.color = normalColor;
    }

    public void OnSelect ()
    {
        spriteRenderer.color = selectedColor;
    }
}