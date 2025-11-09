using System;
using UnityEngine;
using static GLOBAL;

[Serializable]
public struct FaceSpritePair
{
    public CharacterFace Face => _face;
    public Sprite Sprite => _sprite;

    [SerializeField] private CharacterFace _face;
    [SerializeField] private Sprite _sprite;
}