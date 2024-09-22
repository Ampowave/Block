using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] GameObject resetButton;
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowGameOverScreen()
    {
        resetButton.SetActive(true);
    }
}
