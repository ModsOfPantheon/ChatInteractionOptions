using Il2CppInterop.Runtime.Injection;
using MelonLoader;

namespace ChatInteractionOptions;

public class ModMain : MelonMod
{
    public const string ModVersion = "1.0.0";

    public override void OnInitializeMelon()
    {
        ClassInjector.RegisterTypeInIl2Cpp<UIRightClickChatMenu>();
    }
}