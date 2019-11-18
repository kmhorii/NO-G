using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InputFieldManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    string sceneName;
    [SerializeField]
    EventSystem currentSystem;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        currentSystem = EventSystem.current;
    }

    private void Update()
    {
        TabToAdvance();
        EnterToSubmit();
    }

    public void TabToAdvance()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!currentSystem.alreadySelecting && currentSystem.currentSelectedGameObject != null
                /*&& EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null*/)
            {
                Selectable nextElement;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    nextElement = currentSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                }
                else
                {
                    nextElement = currentSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

                }
                if (nextElement != null)
                {
                   // Debug.Log(nextElement.gameObject);
                    InputField inputfield = nextElement.GetComponent<InputField>();
                    if (inputfield != null)
                        inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
                    else
                    {
                        Button button = nextElement.GetComponent<Button>();
                        if(button != null)
                        {
                            EventSystem.current.SetSelectedGameObject(nextElement.gameObject, new BaseEventData(EventSystem.current));
                        }
                    }

                    EventSystem.current.SetSelectedGameObject(nextElement.gameObject, new BaseEventData(EventSystem.current));
                }
                
            }
        }
    }

    public void EnterToSubmit()
    {
        bool alertLoaded = SceneManager.GetSceneByName("Alert").isLoaded;
        EventSystem system = EventSystem.current;
        if (Input.GetKeyDown(KeyCode.Return) && system == currentSystem)
        {
            if(sceneName == "Register")
            {
                Register registration = GameObject.Find("Panel").GetComponentInChildren<Register>();
                registration.CreateAccount();
            }
            else if(sceneName == "Login")
            {
                PlayFabAuth login = GameObject.Find("PlayFabAuth").GetComponent<PlayFabAuth>();
                login.Login();
            }
        }
    }
}
