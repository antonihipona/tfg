using UnityEngine;
using UnityEngine.UI;

public class BuyableColor : MonoBehaviour
{
    
    public Color color;
    public GameObject tick;
    public GameObject target;

    public UICustomizationManager.ItemData itemData;
    
    private Button _button;
    private BuyColorSection _colorSection;

    private void OnEnable()
    {
        SetSelected(false);
        _button = GetComponent<Button>();
        GetComponent<Image>().color = color;
        _colorSection = GetComponentInParent<BuyColorSection>();
        _button.onClick.AddListener(() => _colorSection.ChangeSelected(this));
    }

    public void SetSelected(bool _isSelected)
    {
        if (_isSelected)
        {
            tick.SetActive(true);
            var targetRenderers = target.GetComponentsInChildren<Renderer>();
            foreach (var renderer in targetRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.color = color;
                }
            }
        }
        else
        {
            tick.SetActive(false);
        }
    }

}
