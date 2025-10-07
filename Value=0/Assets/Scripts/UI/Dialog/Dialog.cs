using System;
using UnityEngine;
using static GLOBAL;

[Serializable]
public struct Dialog
{
    public Character Character => character;
    public string Text => text;

    [SerializeField] private Character character;
    [SerializeField] private string text;
}