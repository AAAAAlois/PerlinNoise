using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Dropdown dropdown;


    private void Start()
    {
        //dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    private void DropdownItemSelected(Dropdown dropdown)
    {
        //SceneManager.LoadScene((int)dropdown.value);
        Debug.Log(dropdown.value);
    }

    public void loadPerlin()
    {
        SceneManager.LoadScene(1);
    }

    
}
