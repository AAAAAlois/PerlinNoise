using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] MapGenerator mapGenerator;

    [SerializeField] Slider octavesSlider;
    [SerializeField] Slider persistenceSlider;
    [SerializeField] Slider lacunaritySlider;
    [SerializeField] Slider heightSlider;
    void Start()
    {
        octavesSlider.onValueChanged.AddListener(delegate { mapGenerator.octaves = (int)octavesSlider.value; });
        persistenceSlider.onValueChanged.AddListener(delegate { mapGenerator.persistance = persistenceSlider.value; });
        lacunaritySlider.onValueChanged.AddListener(delegate { mapGenerator.lacunarity = lacunaritySlider.value; });
        heightSlider.onValueChanged.AddListener(delegate { mapGenerator.meshHeightMultiplier = heightSlider.value; });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
