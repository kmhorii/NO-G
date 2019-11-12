using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelecter : MonoBehaviour
{
    private void Awake()
    {
        EventSystem.current = this.GetComponent<EventSystem>();
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
