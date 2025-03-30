using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour
{
    private Animator _ani;

    private void Awake()
    {
        _ani = GetComponentInChildren<Animator>();
    }

    public void LoadCutScene()
    {
        _ani.SetTrigger("PressKey");
        SceneManager.LoadScene("CutScene");
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
