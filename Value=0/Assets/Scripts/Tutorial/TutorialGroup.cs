using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial", menuName = "Tutorial/New Tutorial")]
public class TutorialGroup : ScriptableObject
{
    public int Length => tutorials.Length;
    public TutorialInfo[] Tutorials => tutorials;
    [SerializeField] private TutorialInfo[] tutorials;
}