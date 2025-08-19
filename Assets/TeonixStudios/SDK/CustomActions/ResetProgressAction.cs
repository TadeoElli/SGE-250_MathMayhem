using HutongGames.PlayMaker;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TXS")]
    [Tooltip("Reset player progression values")]
    public class ResetProgressAction : FsmStateAction
    {
        public override void OnEnter()
        {
            FsmVariables globals = FsmVariables.GlobalVariables;
            globals.GetFsmInt("Level").Value = 1;
            globals.GetFsmInt("Score").Value = 0;
            globals.GetFsmInt("CurrentProgress").Value = 0;
            globals.GetFsmInt("ScoreL1").Value = 0;
            globals.GetFsmInt("ScoreL2").Value = 0;
            globals.GetFsmInt("ScoreL3").Value = 0;
            globals.GetFsmBool("TTSenabled").Value = true;
            globals.GetFsmInt("ProgressTotal").Value = 8;
            globals.GetFsmString("Language").Value = "es";

            PlayerPrefs.DeleteAll();
            LocalSDK.SaveProgress();

            Finish();
        }
    }
}
