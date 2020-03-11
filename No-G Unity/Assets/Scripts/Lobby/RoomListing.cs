
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    public Color SelectedColor;
    private LobbyCanvas lobbyCanvas;
    private bool Selected = false;

    [SerializeField]
    private Text _roomNameText;
    private Text RoomNameText
    {
        get { return _roomNameText; }
    }

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

	private void Start()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.LobbyCanvas.gameObject;
        if (lobbyCanvasObj == null)
            return;

        lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(RoomSelected);
        //button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(RoomNameText.text));
    }

    public void RoomSelected()
    {
        Image button = GetComponent<Image>();
        if (!Selected)
        {
            lobbyCanvas.RoomName = RoomNameText.text;
            Selected = true;

            //18, 132, 224, 255
            button.enabled = false;
        }    
        else
        {
            lobbyCanvas.RoomName = null;
            Selected = false;

            button.enabled = true;
        }
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }
	
    public void SetRoomNameText(string text)
    {
        RoomName = text;
        RoomNameText.text = RoomName;
    }
}
