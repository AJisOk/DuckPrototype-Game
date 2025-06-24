using UnityEngine;

public class PatternChallengeNote : MonoBehaviour
{
    [SerializeField] protected int _noteID;
    [SerializeField] protected MeshRenderer _meshToHiglight;
    [SerializeField] protected Material _highlightMaterial;

    private bool _isHighlighted = false;
    private Material _originalMaterial;

    public int NoteID { get => _noteID; }

    private void Awake()
    {
        _originalMaterial = _meshToHiglight.material;
    }

    public void TryHighlightMesh()
    {
        if (_isHighlighted) return;

        _isHighlighted = true;
        _meshToHiglight.material = _highlightMaterial;
    }

    public void UnhiglightMesh()
    {
        if (_isHighlighted)
        {
            _isHighlighted = false;
            _meshToHiglight.material = _originalMaterial;
        }
    }



}
