using UnityEngine;
using UnityEngine.UI;

public enum State
{
    Normal,
    Hovered,
    Clicked
}

public class ImageColorChanger : MonoBehaviour
{
    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _hoveredMaterial;
    [SerializeField] private Material _clickedMaterial;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetNormalColor()
    {
        _image.color = _normalMaterial.color;
    }

    public void SetHoveredColor()
    {
        _image.color = _hoveredMaterial.color;
    }

    public void SetClickedColor() 
    {
        _image.color= _clickedMaterial.color;
    }
}
