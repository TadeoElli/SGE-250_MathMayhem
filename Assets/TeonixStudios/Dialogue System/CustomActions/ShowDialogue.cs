using HutongGames.PlayMaker;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TXS")]
	[Tooltip("TXS Dialogue System")]
	public class ShowDialogue : FsmStateAction
	{
        [ActionSection("DIALOGUE LINES")]

        [ArrayEditor(VariableType.String)]
        public FsmArray dialogueKeys;


        [ActionSection("AVATARS")]

        [Tooltip("Array de avatares que deben coincidir con el indice de lineas de dialogos. Opcion alternativa: indicar el nombre del unico avatar que se mostrara para todas las lineas en Single Avatar Name.")]
        [ArrayEditor(VariableType.String)]
        public FsmArray avatarObjectNames;

        [Tooltip("Usar este avatar para todas las líneas si se deja vacío el array de avatares.")]
        public FsmString singleAvatarName;

        
        [ActionSection("OPTIONS")]

        [Tooltip("Volumen del audio (0 a 1). Solo útil si hay un AudioClip, por ahora se ignora.")]
        public FsmFloat volume = new FsmFloat { UseVariable = true };

        [Tooltip("Velocidad de tipeo (segundos por letra)")]
        public FsmFloat textSpeed = new FsmFloat { UseVariable = true };

        [Tooltip("Offset adicional al clickDelay calculado por caracteres")]
        public FsmFloat clickDelayOffset = new FsmFloat { UseVariable = true };

        [Tooltip("Volumen del sonido de tipeo")]
        public FsmFloat typingVolume = new FsmFloat { UseVariable = true };

        ///----------------
        [ActionSection("FSM EVENTS")]
        [Tooltip("GameObject que contiene los FSM.")]
        public FsmGameObject targetGameObject;

        [UIHint(UIHint.FsmString)]
        [Tooltip("Nombre del FSM seleccionado.")]
        public FsmString selectedFSMName;

        [Tooltip("Nombre del evento a disparar")]
        public FsmString eventToSend;

        [HideInInspector]
        public int selectedIndex = 0;

        ///----------------------

        [ActionSection("LOCAL EVENTS")]

        [Tooltip("Evento local que se dispara al finalizar.")]
        public FsmEvent onComplete;

        public override void Reset()
        {
            dialogueKeys = null;
            avatarObjectNames = null;
            singleAvatarName = new FsmString { UseVariable = true };
            volume = new FsmFloat { UseVariable = true };
            textSpeed = new FsmFloat { UseVariable = true };
            clickDelayOffset = new FsmFloat { UseVariable = true };
            typingVolume = new FsmFloat { UseVariable = true };
            eventToSend = null;
            onComplete = null;

            targetGameObject = null;
            selectedFSMName = null;
            selectedIndex = 0;
        }

        public override void OnEnter()
        {
            string[] keys = new string[dialogueKeys.Length];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = dialogueKeys.Get(i).ToString();

            string[] avatars;

            if (avatarObjectNames.Length > 0)
            {
                avatars = new string[avatarObjectNames.Length];
                for (int i = 0; i < avatars.Length; i++)
                    avatars[i] = avatarObjectNames.Get(i).ToString();
            }
            else if (!singleAvatarName.IsNone && !string.IsNullOrEmpty(singleAvatarName.Value))
            {
                avatars = new string[] { singleAvatarName.Value };
            }
            else
            {
                avatars = null;
            }

            GameObject go2 = targetGameObject.Value;

            TXSDialogueUI ui = GameObject.FindObjectOfType<TXSDialogueUI>();
            if (ui != null)
            {
                ui.StartDialogue(
                    keys,
                    avatars,
                    null,
                    volume.IsNone ? null : (float?)volume.Value,
                    textSpeed.IsNone ? null : (float?)textSpeed.Value,
                    clickDelayOffset.IsNone ? null : (float?)clickDelayOffset.Value,
                    typingVolume.IsNone ? null : (float?)typingVolume.Value,
                    go2,
                    selectedFSMName.Value,
                    eventToSend.Value,
                    () => {
                        Fsm.Event(onComplete);
                        Finish();
                    });
            }
            else
            {
                Debug.LogWarning("DialogueUI no encontrado en la escena.");
                Finish();
            }
        }

    }
}
