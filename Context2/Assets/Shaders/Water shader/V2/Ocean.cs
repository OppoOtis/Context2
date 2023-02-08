using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public float waveHeight = 200f;
    public float waveFrequency = 1f;
    public float waveSpeedZ = 2f;
    public float waveSpeedX = 1.5f;
    public Transform ocean;
    public GameObject oceanObject;

    Material oceanMat;

    Texture2D displacementWaves;

    // Start is called before the first frame update
    void Start()
    {
        SetVariables();
    }

    void SetVariables()
    {
        oceanMat = oceanObject.GetComponent<Renderer>().sharedMaterial;
        displacementWaves = (Texture2D)oceanMat.GetTexture("Displacement_Map");
    }

    public float WaterHeightAtPosition(Vector3 position)
    {
        return displacementWaves.GetPixelBilinear(position.x * waveFrequency/100 + Time.time * waveSpeedX/100, position.z * waveFrequency/100 + Time.time * waveSpeedZ/100).g * waveHeight/100 * ocean.localScale.x;
    }

    void OnValidate()
    {
        if (!oceanMat)
            SetVariables();
        
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        oceanMat.SetFloat("Wave_Frequency", waveFrequency/100);
        oceanMat.SetFloat("Wave_Speed", waveSpeedZ/100);
        oceanMat.SetFloat("Wave_Height", waveHeight/100);
        oceanMat.SetFloat("Wave_Speed_2", waveSpeedX / 100);

    }
}
