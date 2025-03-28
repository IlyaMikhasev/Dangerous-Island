using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeIslandToBoat : MonoBehaviour
{
    private float _timeEscapeIsland = 2f;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            bool endGame = GameInstance.Instance.IsEndGame;
            if (endGame)
            {
                CanvasInstance.Instance.Info("Поздравляю, тебе удалось выбраться!");
                StartCoroutine(CloseScene());
            }
            else 
            {
                CanvasInstance.Instance.Info("Иди ищи золото трус!");
            } 

            
        }

    }

    private IEnumerator CloseScene() {
        yield return new WaitForSeconds(_timeEscapeIsland);
        SceneManager.LoadScene("EndGame");
    }
}
