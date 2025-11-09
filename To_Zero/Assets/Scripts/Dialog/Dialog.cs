using System;
using UnityEngine;
using static GLOBAL;

[Serializable]
public struct Dialog
{
    public Character Character => character;
    public CharacterFace Face => face;
    public bool OnLeftSide => onLeftSide;
    public string Text => text;

    [SerializeField] private Character character;
    [SerializeField] private CharacterFace face;
    [SerializeField] private bool onLeftSide;
    [SerializeField, TextArea(3, 5)] private string text;
}