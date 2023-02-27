using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster)), RequireComponent(typeof(CanvasScaler))]
public class MenuUIInteraction : MonoBehaviour
{
    Canvas prevUI;
    Canvas myUI;

    [Header("Only assign items to the top level UI")]
    [SerializeField] GameState startState = GameState.NONE;
    [SerializeField] KeyCode menuKey; // replace in commandpattern later?
    bool startCanvas;

    public static GameState gameState;

    private void Awake()
    {
        myUI = GetComponent<Canvas>();
        myUI.enabled = false;
    }

    private void Start()
    {
        if (startState != GameState.NONE)
        {
            startCanvas = true;
            gameState = startState;
            myUI.enabled = true;
            SwitchGameState(gameState);
        }
        else
        {
            startCanvas = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
            if (!startCanvas && myUI.enabled)
            {
                if (prevUI != null)
                {
                    OnClickPrevMenu();
                }
            }

            if (startCanvas)
            {
                if (myUI.enabled && gameState == GameState.GAME_PAUSED)
                {
                    myUI.enabled = false;
                    SwitchGameState(GameState.GAME_PLAYING);
                }
                else if(!myUI.enabled && gameState == GameState.GAME_PLAYING)
                {
                    myUI.enabled = true;
                    SwitchGameState(GameState.GAME_PAUSED);
                }
            }
        }
    }

    public void SwitchGameState(GameState newState)
    {
        gameState = newState;

        switch (gameState)
        {
            case GameState.GAME_START:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 1;
                break;
            case GameState.GAME_PAUSED:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
                break;
            case GameState.GAME_PLAYING:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                myUI.enabled = false;
                break;
            default:
                break;
        }
    }

    public void ActivateMyUI()
    {
        myUI.enabled = true;
    }

    public void OnClickNextMenu(MenuUIInteraction nextUI)
    {
        if (nextUI != null)
        {
            myUI.enabled = false;
            nextUI.myUI.enabled = true;
            nextUI.prevUI = myUI;
            nextUI.menuKey = menuKey;
        }
    }

    public void OnClickPrevMenu()
    {
        if (prevUI != null)
        {
            myUI.enabled = false;
            prevUI.enabled = true;
        }
    }

    public void OnPressingQuit()
    {
        Application.Quit();
    }

    public void OnClickLoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OnClickDebugTestMessage(string message)
    {
        Debug.Log(message);
    }

    public void OnClickResume()
    {
        SwitchGameState(GameState.GAME_PLAYING);
    }
}

public enum GameState { GAME_START = 0, GAME_PAUSED = 1, GAME_PLAYING = 2, NONE = 3 }