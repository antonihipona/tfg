using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSection : MonoBehaviour
{
    private CustomizableColor _currentSelectedColor;

    public void ChangeSelected(CustomizableColor _color)
    {
        if (_currentSelectedColor != null)
            _currentSelectedColor.SetSelected(false);
        _color.SetSelected(true);
        _currentSelectedColor = _color;
    }
}
