using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestKillBox : MonoBehaviour
{
    public GameObject ParentObject;
    public Font myFont;
    private int count = 0;
    // Start is called before the first frame update
    public void OnButtonClick()
    {
        count++;
        GameObject newText = new GameObject("text" + count);
        newText.transform.SetParent(ParentObject.transform);
        newText.AddComponent<Text>();
        newText.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        newText.GetComponent<Text>().text = "Hello";
        newText.GetComponent<Text>().color = Color.black;
        newText.GetComponent<Text>().font = myFont;
    }
}
