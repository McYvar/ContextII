using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster)), RequireComponent(typeof(CanvasScaler))]
public class MenuUIInteraction : MonoBehaviour
{
    Canvas prevUI;
    Canvas myUI;

    [Header("Only assign the Menu Key if this is the first menu in line")]
    [SerializeField] KeyCode menuKey; // replace in commandpattern later?
    [SerializeField] bool startCanvas;

    public static bool DoUI;

    private void Awake()
    {
        myUI = GetComponent<Canvas>();
        myUI.enabled = false;
    }

    private void Start()
    {
        if (startCanvas)
        {
            DoUI = true;
            myUI.enabled = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(menuKey) && !startCanvas)
        {
            if (!myUI.enabled) return;
            if (prevUI != null)
            {
                myUI.enabled = false;
                prevUI.enabled = true;
            }
            else if (DoUI) DoUI = false;
            else
            {
                DoUI = true;
                myUI.enabled = true;
            }
        }

        if (DoUI)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            myUI.enabled = false;
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
}
