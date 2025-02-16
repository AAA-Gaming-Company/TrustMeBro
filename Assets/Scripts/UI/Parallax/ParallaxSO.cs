using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Parallax")]
public class ParallaxSO : ScriptableObject {
    public Sprite foreground;
    public Sprite[] intermediate;
    public Sprite background;
}
