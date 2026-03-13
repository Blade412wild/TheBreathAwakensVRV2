using System;
using Unity.VisualScripting;
using UnityEngine;

public class FootstepAudioManager : MonoBehaviour
{

    [SerializeField] Terrain terrain;
    [SerializeField] private TerrainTypeToGroundType terrainConverter;
    [SerializeField] private TerrainLayerDetector terrainLayerDetector;
    [SerializeField] private FeetSim feetSim;


    private float currentTime = 0;

    private void Start()
    {
        feetSim.HitTerrainEvent += HandleFootHitTerrainEvent;
    }

    private void Update()
    {

    }


    private void HandleFootHitTerrainEvent(Vector3 pos)
    {
        GroundType groundType;

        int dominateTerrainLayerInt = terrainLayerDetector.GetMainTexture(pos, terrain);
        TerrainLayer dominateTerrainLayer = terrain.terrainData.terrainLayers[dominateTerrainLayerInt];
        Texture dominateTexture = dominateTerrainLayer.diffuseTexture;

        GroundType currentGroundType = terrainConverter.TryGetGroundType(dominateTexture);
    }

    public void GetGroundType()
    {

    }
}
