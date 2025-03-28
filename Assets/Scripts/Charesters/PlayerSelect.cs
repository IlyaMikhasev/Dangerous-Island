using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelect : MonoBehaviour
{
    [SerializeField] private GameObject[] _charasters;
    
    private int _index ;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "StartGame")
        {
            _index = PlayerPrefs.GetInt("SelectPlayer");

            for (int i = 0; i < _charasters.Length; i++)
            {
                _charasters[i] = Instantiate(_charasters[i], transform.position, Quaternion.identity, transform);
            }
            if (_charasters[_index])
            {
                _charasters[_index].SetActive(true);
            }
        }
    }

    public void SelectLeft() {
        _charasters[_index].SetActive(false);
        _index--;
        if (_index < 0) { 
            _index = _charasters.Length - 1;
        }
        _charasters[_index].SetActive(true);
    }
    public void SelectRight()
    {
        _charasters[_index].SetActive(false);
        _index++;
        if (_index == _charasters.Length)
        {
            _index = 0;
        }
        _charasters[_index].SetActive(true);
    }
    public void StartScene() {
        PlayerPrefs.SetInt("SelectPlayer", _index);
        foreach(GameObject unit in _charasters)
            Destroy(unit);
        SceneManager.LoadScene("IslandScene");

    }
}
