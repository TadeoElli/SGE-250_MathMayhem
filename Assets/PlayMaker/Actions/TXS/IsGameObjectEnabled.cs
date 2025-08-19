using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TXS")]
	[Tooltip("Check if an X gameobject is enabled")]
	public class IsGameObjectEnabled : FsmStateAction
	{
        [RequiredField]
        [Tooltip("El GameObject a verificar.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Variable booleana donde se almacena el resultado.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isActive;

        public override void Reset()
        {
            gameObject = null;
            isActive = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go != null)
            {
                isActive.Value = go.activeInHierarchy;
            }
            Finish();
        }

    }

}
