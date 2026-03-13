using System.Collections.Generic;
using UnityEngine;

public class TerrainTypeToGroundType : MonoBehaviour 
{


    [SerializeField] private TerainTypeCollection terainTypeCollection;
    private Dictionary<Texture, GroundType> terrainDic = new Dictionary<Texture, GroundType>();


    private void Start()
    {
        SetTerrainDic();
    }


    public GroundType TryGetGroundType(Texture texture)
    {

        if (terrainDic.ContainsKey(texture))
        {
            GroundType type = terrainDic[texture];
            Debug.Log("Found Key : " + type);
            return type;    
        }
        else
        {
            Debug.LogWarning("texture wasn't in dic");
            return null;
        }
    }
    private void SetTerrainDic()
    {
        foreach (var terrainType in terainTypeCollection.terrainTypes)
        {
            foreach(Texture texture in terrainType.possibleTextures)
            {
                if(texture == null)
                {
                    Debug.LogError("No texture in " + terrainType.name + "possibletextures[]");
                    continue;
                }
                
                if (terrainDic.ContainsKey(texture))
                {
                    Debug.LogWarning("TextureDic already contains" + texture.name + " as key");
                    continue;
                }
                else
                {
                    terrainDic.Add(texture, terrainType.GroundType);
                    Debug.Log("added " +  texture.name + "to dic with groundType : " +  terrainType.GroundType.name);
                }
            }
        }
    }


}
