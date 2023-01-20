using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    [Header("Slider")]
    [SerializeField] Slider octavesSlider;
    [SerializeField] Slider persistenceSlider;
    [SerializeField] Slider lacunaritySlider;
    [SerializeField] Slider heightSlider;
    [SerializeField] Slider noiseScaleSlider;
    [SerializeField] Slider offsetXSlider;
    [SerializeField] Slider offsetYSlider;
    [SerializeField] Slider seedSlider;

    [Header("Value")]
    [SerializeField] TextMeshProUGUI octavesValue;
    [SerializeField] TextMeshProUGUI persistenceValue;
    [SerializeField] TextMeshProUGUI lacunarityValue;
    [SerializeField] TextMeshProUGUI heightValue;
    [SerializeField] TextMeshProUGUI noiseScaleValue;
    [SerializeField] TextMeshProUGUI offsetXValue;
    [SerializeField] TextMeshProUGUI offsetYValue;
    [SerializeField] TextMeshProUGUI seedValue;

    [Header("Toggle")]
    [SerializeField] Toggle toggleIslandTerrain;
    [SerializeField] Toggle toggleEndlessTerrain;
    [SerializeField] GameObject singleChunk;
    [SerializeField] Toggle toggleTPS;



    void Start()
    {
        octavesSlider.onValueChanged.AddListener(delegate { 
            mapGenerator.octaves = (int)octavesSlider.value;
            octavesValue.text = octavesSlider.value.ToString();
        });

        persistenceSlider.onValueChanged.AddListener(delegate { 
            mapGenerator.persistance = persistenceSlider.value;
            persistenceValue.text = persistenceSlider.value.ToString("0.00");
        });

        lacunaritySlider.onValueChanged.AddListener(delegate { 
            mapGenerator.lacunarity = lacunaritySlider.value;
            lacunarityValue.text = lacunaritySlider.value.ToString("0.00");
        });

        heightSlider.onValueChanged.AddListener(delegate { 
            mapGenerator.meshHeightMultiplier = heightSlider.value;
            heightValue.text = heightSlider.value.ToString("0.00");
        });

        noiseScaleSlider.onValueChanged.AddListener(delegate {
            mapGenerator.noiseScale = noiseScaleSlider.value;
            noiseScaleValue.text = noiseScaleSlider.value.ToString("0.00");
        });

        offsetXSlider.onValueChanged.AddListener(delegate {
            mapGenerator.offset.x = offsetXSlider.value;
            offsetXValue.text = offsetXSlider.value.ToString("0.00");
        });

        offsetYSlider.onValueChanged.AddListener(delegate {
            mapGenerator.offset.y = offsetYSlider.value;
            offsetYValue.text = offsetYSlider.value.ToString("0.00");
        });

        seedSlider.onValueChanged.AddListener(delegate {
            mapGenerator.seed = (int)seedSlider.value;
            seedValue.text = seedSlider.value.ToString();
        });

        toggleIslandTerrain.onValueChanged.AddListener(isOn => mapGenerator.useFalloff = isOn) ;
        toggleEndlessTerrain.onValueChanged.AddListener(isOn => EndlessTerrainToggle(isOn));

    }



    // Update is called once per frame
    void Update()
    {

        
    }

    void EndlessTerrainToggle(bool isOn)
    {
        mapGenerator.gameObject.GetComponent<EndlessTerrain>().enabled = isOn;
        mapGenerator.isEndlessTerrain = isOn;
        singleChunk.SetActive(!isOn);
       
    }
}
