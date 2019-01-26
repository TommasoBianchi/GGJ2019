using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Values<T> : ScriptableObject, IPrintableValue
{

    [SerializeField]
    private T value;
    public T Value { get { return value; } }

    [SerializeField]
    private string displayName;

    public event Action<T> onValueChanged;
    public event Action<string> onPrintableValueChanged;

    public void SetValue(T value)
    {
        this.value = value;

        if(onValueChanged != null)
        {
            onValueChanged(value);
        }

        if (onPrintableValueChanged != null)
        {
            onPrintableValueChanged(value.ToString());
        }
    }

    public void RemoveAllListeners()
    {
        onValueChanged = null;
        onPrintableValueChanged = null;
    }

    public override string ToString()
    {
        return (displayName == "") ? name : displayName;
    }
}

public interface IPrintableValue
{
    event Action<string> onPrintableValueChanged;
}