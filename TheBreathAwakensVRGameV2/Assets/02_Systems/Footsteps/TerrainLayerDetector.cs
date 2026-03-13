using UnityEngine;

public class TerrainLayerDetector : MonoBehaviour
{


    public int GetMainTexture(Vector3 worldPos, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = worldPos - terrain.transform.position;

        int mapX = (int)((terrainPos.x / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)((terrainPos.z / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        int numLayers = splatmapData.GetLength(2);
        int maxIndex = 0;
        float maxMix = 0;

        for (int i = 0; i < numLayers; i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                maxMix = splatmapData[0, 0, i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }
}

