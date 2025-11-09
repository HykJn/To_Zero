using System;
using UnityEngine;

[Serializable]
public struct TutorialInfo
{
    public Sprite Image => image;
    public string Name => name;
    public string Description => description;

    [SerializeField] private Sprite image;
    [SerializeField] private string name;
    [SerializeField, TextArea(1, 10)] private string description;
}