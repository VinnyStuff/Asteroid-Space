using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public bool restarting;
    public GameRules gameRules;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (restarting == true)
        {
            gameRules = GameObject.FindWithTag("GameRules").GetComponent<GameRules>();
            gameRules.restartScript = this.gameObject.GetComponent<Restart>();
        }
    }
    public void RestartButton()
    {
        restarting = true;
        SceneManager.LoadScene("Gameplay");
    }
    public void MainMenuButton()
    {
        Destroy(this.gameObject);
    }
}
