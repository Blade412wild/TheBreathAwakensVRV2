using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class FootstepAudioManager : MonoBehaviour
{
    public event Action<int, int> FootstepAudioEvent;
    [SerializeField] Terrain terrain;
    [SerializeField] private TerrainTypeToGroundType terrainConverter;
    [SerializeField] private TerrainLayerDetector terrainLayerDetector;
    [SerializeField] private FeetSim feetSim;
    [SerializeField] private GroundTypeCollection groundTypeCollection;
    [SerializeField] private Foot[] feet;
    [SerializeField] private AudioSource audioSource; 
    private float currentTime = 0;

    private void Start()
    {
        feetSim.HitTerrainEvent += HandleFootHitTerrainEvent;

        foreach(var f in feet)
        {
            f.HitTerrainEvent += HandleFootHitTerrainEvent;
        }

        FootstepAudioEvent += PlayFootStepAudio;

        SetupGroundTypes();
    }


    private void HandleFootHitTerrainEvent(Vector3 pos)
    {
        GroundType groundType;

        int dominateTerrainLayerInt = terrainLayerDetector.GetMainTexture(pos, terrain);
        TerrainLayer dominateTerrainLayer = terrain.terrainData.terrainLayers[dominateTerrainLayerInt];
        Texture dominateTexture = dominateTerrainLayer.diffuseTexture;

        GroundType currentGroundType = terrainConverter.TryGetGroundType(dominateTexture);

        if (CheckIfGroundTypeIsValid(currentGroundType))
        {
            FootstepAudioEvent?.Invoke(0, currentGroundType.index);
            Debug.Log("audio footstep event;");
        }
    }

    private bool CheckIfGroundTypeIsValid(GroundType type)
    {
        if (groundTypeCollection.GroundTypes[type.index] = type)
        {
            return true;
        }
        else
        {

            if (groundTypeCollection.GroundTypes.Contains<GroundType>(type))
            {
                Debug.LogWarning("you have a ground type with a double index", type);
                Debug.LogWarning("type and index in array aren't the same", type);
            }
            else
            {
                Debug.LogWarning("you have a ground type with a double index and the groundType is not listed in ", groundTypeCollection);
            }



            return false;
        }
    }

    private void SetupGroundTypes()
    {
        for (int i = 0; i < groundTypeCollection.GroundTypes.Length - 1; i++)
        {
            groundTypeCollection.GroundTypes[i].index = i;
        }

    }

    private void OnDisable()
    {
        foreach (var f in feet)
        {
            f.HitTerrainEvent -= HandleFootHitTerrainEvent;
        }
    }

    private void PlayFootStepAudio(int i, int j)
    {
        audioSource.Play();
    }
}
