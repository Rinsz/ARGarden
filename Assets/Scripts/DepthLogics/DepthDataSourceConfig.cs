using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DepthDataSourceConfig", menuName = "Depth Lab/Depth Data Source Config", order = 1)]
public class DepthDataSourceConfig : ScriptableObject
{
    [Header("Depth Data Source")]
    [Tooltip("Assembly qualified class name of the depth data source implementing the IDepthDataSource interface.")]
    public string DepthSourceClassName;

    public IDepthDataSource DepthDataSource;

    public void Awake()
    {
        var type = Type.GetType(DepthSourceClassName);
        if (type != null)
            DepthDataSource = (IDepthDataSource)Activator.CreateInstance(type);
    }
}