using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public Button PlayButton;
    public Button RulesButton;

    // Update is called once per frame
    void Start()
    {
        PlayButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));
        RulesButton.onClick.AddListener(() => SceneManager.LoadScene("Rules"));
    }
}
