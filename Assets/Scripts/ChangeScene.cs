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

    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(changeScene);
    }
    
    void changeScene(int i)
    {
        switch (i)
        {
            case 0:
                SceneManager.LoadScene(0);
                break;
            case 1:break;
        }
    }
}
