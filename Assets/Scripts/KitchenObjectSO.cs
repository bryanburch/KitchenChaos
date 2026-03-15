using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
    // using "public" access modifier for convenience and also
    // will only be reading from Scriptable Objects
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
