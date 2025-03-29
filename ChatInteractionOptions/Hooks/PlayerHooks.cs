using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ChatInteractionOptions.Hooks;

[HarmonyPatch(typeof(EntityPlayerGameObject), nameof(EntityPlayerGameObject.NetworkStart))]
public class PlayerHooks
{
    private static void Postfix(EntityPlayerGameObject __instance)
    {
        if (__instance.NetworkId.Value == 1)
        {
            return;
        }

        if (__instance.NetworkId.Value != EntityPlayerGameObject.LocalPlayerId.Value)
        {
            return;
        }
        
        var midPanel = UIPanelRoots.Instance.Mid;

        var rightClickMenu = midPanel.GetComponentInChildren<UIRightClickEntityMenu>(true);
        var copy = Object.Instantiate(rightClickMenu, midPanel);
        var copyComponent = copy.GetComponent<UIRightClickEntityMenu>();
        Object.Destroy(copyComponent);

        copy.transform.GetChild(0).GetComponent<RectTransform>().pivot = new Vector2(0, 0);
        var chatComponent = copy.gameObject.AddComponent<UIRightClickChatMenu>();
        Globals.Menu = chatComponent;
        
        chatComponent.SetupWindow();
    }
}