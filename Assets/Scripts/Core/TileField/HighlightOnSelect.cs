using UnityEngine;

public class HighlightOnSelect : MonoBehaviour, ISelectable
{
    [SerializeField] bool isWhite = true;

    private MeshRenderer _meshRenderer;
    private bool _isSelected { get; set; }

    [Header ("Materials")]
    [SerializeField] Material blackHighlightMaterial;
    [SerializeField] Material whiteHighlightMaterial;
    [SerializeField] Material blackNormalMaterial;
    [SerializeField] Material whiteNormalMaterial;

    private void Start () => _meshRenderer = GetComponent<MeshRenderer> ();

    public void OnDeselect ()
    {
        _isSelected = false;
        if (isWhite)
            _meshRenderer.material = whiteNormalMaterial;
        else
            _meshRenderer.material = blackNormalMaterial;
    }

    public void OnSelect ()
    {
        _isSelected = true;
        if (isWhite)
            _meshRenderer.material = whiteHighlightMaterial;
        else
            _meshRenderer.material = blackHighlightMaterial;
    }

    bool ISelectable.IsSelected () => _isSelected;
}