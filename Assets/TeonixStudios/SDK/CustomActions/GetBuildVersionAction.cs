using UnityEngine;
using HutongGames.PlayMaker;

namespace TeonixStudios.SDK.CustomActions
{
    [ActionCategory("TXS")]
    [HutongGames.PlayMaker.Tooltip("Obtiene la versión de compilación del juego desde el componente Utilities y la almacena en una variable.")]
    public class GetBuildVersionAction : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("El GameObject que tiene el script Utilities.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Variable donde se almacenará la versión de compilación.")]
        public FsmString buildVersion;

        public override void Reset()
        {
            gameObject = null;
            buildVersion = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go != null)
            {
                var utilities = go.GetComponent<Utilities>();
                if (utilities != null)
                {
                    buildVersion.Value = Utilities.GetBuildVersion();
                }
                else
                {
                    LogError("El GameObject no tiene el componente Utilities.");
                }
            }
            else
            {
                LogError("No se ha asignado un GameObject.");
            }
            Finish();
        }
    }
}