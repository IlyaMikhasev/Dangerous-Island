using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject _winGame;
    [SerializeField] private GameObject _loseGame;
    [SerializeField] private GameObject _errorWindow;
   
    private void Start()
    {
        string res = PlayerPrefs.GetString("final");
        if (res.Equals("win"))
        {
            _winGame.SetActive(true);
        }
        else if (res.Equals("lose"))
        {
            _loseGame.SetActive(true);
        }
        else {
            _errorWindow.SetActive(true);
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene("StartGame");
    }

    
}
