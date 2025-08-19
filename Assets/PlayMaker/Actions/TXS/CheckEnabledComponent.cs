using UnityEngine;
using HutongGames.PlayMaker;


namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory("TXS")]
    [HutongGames.PlayMaker.Tooltip("Verifica si un FSM dentro de un GameObject está activado o desactivado.")]
    public class CheckFSMState : FsmStateAction
    {
        [RequiredField]
        [UnityEngine.Tooltip("El GameObject que contiene el FSM.")]
        public FsmOwnerDefault targetGameObject;

        [UnityEngine.Tooltip("El nombre del FSM que deseas verificar.")]
        public FsmString fsmName;

        [UnityEngine.Tooltip("Variable bool que indicará si el FSM está activo.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isActive;

        public override void Reset()
        {
            targetGameObject = null;
            fsmName = "";
            isActive = null;
        }

        public override void OnEnter()
        {
            GameObject obj = Fsm.GetOwnerDefaultTarget(targetGameObject);
            if (obj == null)
            {
                Finish();
                return;
            }

            PlayMakerFSM fsm = obj.GetComponent<PlayMakerFSM>();
            if (fsm != null && fsm.FsmName == fsmName.Value)
            {
                isActive.Value = fsm.enabled;
            }
            else
            {
                isActive.Value = false;
            }

            Finish();
        }
    }
}
