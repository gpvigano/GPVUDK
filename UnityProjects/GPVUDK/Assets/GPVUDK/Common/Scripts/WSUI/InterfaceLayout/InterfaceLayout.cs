using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InterfaceLayout
{
    public string name;
    public CanvasTransform[] canvasTransform;

    public InterfaceLayout()
    {
    }

    public void Apply()
    {
        if (canvasTransform != null)
        {
            foreach (CanvasTransform layout in canvasTransform)
            {
                layout.Apply();
            }
        }
    }

    public void Store()
    {
        if (canvasTransform != null)
        {
            foreach (CanvasTransform layout in canvasTransform)
            {
                layout.Store();
            }
        }
    }

    public InterfaceLayout Duplicate()
    {
        InterfaceLayout newLayout = new InterfaceLayout();
        newLayout.name = name;
        newLayout.canvasTransform = (CanvasTransform[])canvasTransform.Clone();
        return newLayout;
    }
    public void Validate()
    {
            foreach (CanvasTransform layout in canvasTransform)
            {
                layout.Validate();
            }
    }
}
