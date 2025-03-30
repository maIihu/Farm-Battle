using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadScene : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(LoadGameScene), 3f);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GamePlayScene");
    }
}
