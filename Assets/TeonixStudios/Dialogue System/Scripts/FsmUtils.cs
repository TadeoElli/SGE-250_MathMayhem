using HutongGames.PlayMaker;
using UnityEngine;

public static class FsmUtils
{
    public static void SendEventToFSM(GameObject target, string fsmName, string eventName)
    {
        if (target == null || string.IsNullOrEmpty(eventName)) return;
        foreach (var fsm in target.GetComponents<PlayMakerFSM>())
        {
            if (string.IsNullOrEmpty(fsmName) || fsm.FsmName == fsmName)
            {
                fsm.Fsm.Event(eventName);
                Debug.Log($"Enviado evento '{eventName}' al FSM '{fsm.FsmName}' en '{target.name}'");
                return;
            }
        }
        Debug.LogWarning($"No se encontró FSM '{fsmName}' en '{target.name}'");
    }
}
