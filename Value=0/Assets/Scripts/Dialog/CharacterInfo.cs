using System.Linq;
using UnityEngine;
using static GLOBAL;

[CreateAssetMenu(menuName = "Dialog/CharacterInfo", fileName = "New Character Info")]
public class CharacterInfo : ScriptableObject
{
    #region =====Properties=====

    public Sprite this[CharacterFace face] => (from pair in _pairs where pair.Face == face select pair.Sprite).FirstOrDefault();

    #endregion

    #region =====Fields=====

    [SerializeField] private FaceSpritePair[] _pairs;

    #endregion
}
