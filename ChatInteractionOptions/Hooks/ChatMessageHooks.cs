using HarmonyLib;
using Il2Cpp;
using Il2CppPantheonPersist;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace ChatInteractionOptions.Hooks;

[HarmonyPatch(typeof(UIChatMessage), nameof(UIChatMessage.OnPointerClick))]
public class UIChatMessageOnClickHook
{
    private static void Postfix(UIChatMessage __instance, PointerEventData eventData)
    {
        if (PlayerTalkChannels.All(x => __instance.channel != x))
        {
            return;
        }
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            var mainWindow = UIChatWindows.Instance.mainWindow;
            var input = mainWindow.GetComponent<UIChatInput>();
            input.channelType = ChatChannelType.Whisper;
            input.whisperName = __instance.senderName;
            input.ActivateInput(string.Empty);
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var mousePos = Mouse.current.position.ReadValue();
            Globals.Menu.Show(__instance.senderName, mousePos);
        }
    }

    private static readonly ChatChannelType[] PlayerTalkChannels = {
        ChatChannelType.PlayerSay,
        ChatChannelType.Group,
        ChatChannelType.OutOfCharacter,
        ChatChannelType.Auction,
        ChatChannelType.Shout,
        ChatChannelType.Whisper,
        ChatChannelType.Guild
    };
}