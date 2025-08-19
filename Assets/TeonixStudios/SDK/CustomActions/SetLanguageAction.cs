using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TXS")]
    [Tooltip("Set current language value on screen")]
    public class SetLanguageAction : FsmStateAction
    {
        public FsmString languageCode;

        public override void OnEnter()
        {
            LocalSDK.SetLanguage(languageCode.Value);
            Finish();
        }
    }
}