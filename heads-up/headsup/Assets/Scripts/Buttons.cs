using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public Button PlayButton;
    public Button MenuButton;

    // Update is called once per frame
    void Start()
    {
        PlayButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));
        MenuButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
    }
}
