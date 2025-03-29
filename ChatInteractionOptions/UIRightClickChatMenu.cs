using Il2Cpp;
using Il2CppPantheonPersist;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;

namespace ChatInteractionOptions;

public class UIRightClickChatMenu : MonoBehaviour
{
    private bool HasSetup;
    
    private string _targetName;
    
    private UIChatInput _chatInput;
    private RectTransform backgroundRect;
    private UIWindowPanel menuPanel;

    private TextMeshProUGUI playerNameText;

    private Button _whisperButton;
    private Button _whoButton;
    private Button _groupButton;
    private Button _guildButton;
    private Button _friendButton;
    private Button _ignoreButton;
    private TextMeshProUGUI _playerNameButtonText;

    // Doing this in Awake is a bit of a pain, the window begins invisible, which has active set to false. So when
    // we go to show the window, the references wouldn't be set up yet. So lets add a setup method and call it in the
    // player hook
    public void SetupWindow()
    {
        if (HasSetup)
        {
            return;
        }

        HasSetup = true;
        menuPanel = transform.GetComponent<UIWindowPanel>();
        _chatInput = UIChatWindows.Instance.mainWindow.GetComponent<UIChatInput>();

        var layout = transform.GetChild(0);
        backgroundRect = layout.GetComponent<RectTransform>();

        // Destroy all but the last child (Cancel button) so we can copy paste it
        for (var i = layout.childCount - 2; i >= 0; i--)
        {
            DestroyImmediate(layout.GetChild(i).gameObject);
        }
        
        var cancelButton = layout.GetChild(0).gameObject;
        var cancelButtonComponent = cancelButton.GetComponent<Button>();
        cancelButtonComponent.onClick = new Button.ButtonClickedEvent();
        cancelButtonComponent.onClick.AddCall(new InvokableCall(new Action(Close)));

        var playerNameButton = Instantiate(cancelButton, layout);
        playerNameButton.transform.SetSiblingIndex(0);
        playerNameButton.GetComponent<Button>().interactable = false;
        playerNameButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        _playerNameButtonText = playerNameButton.GetComponentInChildren<TextMeshProUGUI>();
        _playerNameButtonText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        
        _whisperButton = CloneButton(cancelButton, layout, 1, SetupWhisper, "Whisper");
        _whoButton = CloneButton(cancelButton, layout, 2, () => SendCommand("who"), "Who");
        _groupButton = CloneButton(cancelButton, layout, 3, () => SendCommand("invite"), "Invite to Group");
        _guildButton = CloneButton(cancelButton, layout, 4, () => SendCommand("guildinvite"), "Invite to Guild");
        _friendButton = CloneButton(cancelButton, layout, 5, () => SendCommand("friend"), "Friend");
        _ignoreButton = CloneButton(cancelButton, layout, 6, () => SendCommand("ignore"), "Ignore");
    }

    private Button CloneButton(GameObject buttonToClone, Transform parent, int siblingIndex, Action onClick, string labelText)
    {
        var who = Instantiate(buttonToClone, parent);
        who.transform.SetSiblingIndex(siblingIndex);

        _whoButton = who.GetComponent<Button>();
        _whoButton.onClick = new Button.ButtonClickedEvent();
        _whoButton.onClick.AddCall(new InvokableCall(onClick));
        _whoButton.GetComponentInChildren<TextMeshProUGUI>().text = labelText;

        return _whoButton;
    }

    // Although the panel itself has a RectTransform of its own, the visual for the window is actually
    // driven by the first child of the window, the container for all the child elements. Therefore we must
    // set the position of the underlying rect...
    private void SetPosition(Vector2 mousePos)
    {
        var x = Math.Clamp(mousePos.x, 0, Screen.width - backgroundRect.sizeDelta.x);
        var y = Math.Clamp(mousePos.y, 0, Screen.height - backgroundRect.sizeDelta.y);
        
        backgroundRect.transform.position = new Vector2(x, y);
    }

    private void SendCommand(string command)
    {
        _chatInput.submitText = $"/{command} {_targetName}";
        _chatInput.submitting = true;
        Close();
    }
    
    public void Show(string targetName, Vector2 mousePos)
    {
        var localPlayer = EntityPlayerGameObject.LocalPlayer.Cast<EntityPlayerGameObject>();
        // This is a hack for getting guild name... player's info does not have the guild fields populated.
        // I don't know if there are other titles you can get in the game, I've yet to see one. This may break in the future
        var isInGuild = !string.IsNullOrWhiteSpace(localPlayer.info.Title);
        _guildButton.gameObject.SetActive(Globals.ShowGuildInvite && isInGuild);
        
        menuPanel.Show();

        SetPosition(mousePos);
        
        _targetName = targetName;
        _playerNameButtonText.text = _targetName;
    }

    private void SetupWhisper()
    {
        _chatInput.channelType = ChatChannelType.Whisper;
        _chatInput.whisperName = _targetName;
        _chatInput.ActivateInput(string.Empty);
        Close();
    }
    
    public void Close()
    {
        menuPanel.Hide();
    }
}