using HutongGames.PlayMaker;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TXS")]
    [Tooltip("Set player progression value")]
    public class SetProgressAction : FsmStateAction
    {
        public override void OnEnter()
        {
            FsmVariables globals = FsmVariables.GlobalVariables;
            int current = globals.GetFsmInt("ProgressCurrent").Value;
            int total = globals.GetFsmInt("ProgressTotal").Value;

            if (current < total)
            {
                globals.GetFsmInt("ProgressCurrent").Value = current + 1;
                LocalSDK.SaveProgress();
                Debug.Log($">>>> Progress updated: {current + 1}/{total}");
            }

            Finish();
        }
    }
}