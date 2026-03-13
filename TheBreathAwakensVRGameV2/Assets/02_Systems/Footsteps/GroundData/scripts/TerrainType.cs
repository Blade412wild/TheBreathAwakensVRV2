using UnityEngine;

[CreateAssetMenu(fileName = "TerrainType", menuName = "Scriptable Objects/TerrainType")]
public class TerrainType : ScriptableObject
{
    public GroundType GroundType;
    public Texture[] possibleTextures;

}
