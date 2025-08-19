#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using HutongGames.PlayMakerEditor;
using HutongGames.PlayMaker;
using System.Linq;
using HutongGames.PlayMaker.Actions;

[CustomActionEditor(typeof(ShowDialogue))]
public class SelectFSMFromGameObjectEditor : CustomActionEditor
{
    private string foldoutKey;

    public override void OnEnable()
    {
        var action = target as ShowDialogue;
        foldoutKey = "ShowDialogueOptionsFoldout_" + action.GetHashCode();
    }

    public override bool OnGUI()
    {
        var action = target as ShowDialogue;
        EditorGUILayout.BeginVertical();
        try
        {
            // Sección DIALOGUE SETUP (gris suave)
            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
            EditorGUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            GUILayout.Label("DIALOGUE SETUP", EditorStyles.boldLabel);
            EditField("dialogueKeys");
            EditField("avatarObjectNames");
            EditField("singleAvatarName");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Sección TARGET/FSM selector + EVENTS (gris oscuro)
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f);
            EditorGUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            GUILayout.Label("FSM TARGET & EVENTS", EditorStyles.boldLabel);
            EditField("targetGameObject");
            GameObject go = action?.targetGameObject?.Value;
            if (go != null)
            {
                var fsms = go.GetComponents<PlayMakerFSM>();
                var fsmNames = fsms.Select(f => f.FsmName).ToArray();
                if (fsmNames.Length > 0)
                {
                    // Forzar sincronización si el nombre guardado existe
                    if (!string.IsNullOrEmpty(action.selectedFSMName?.Value))
                    {
                        int index = System.Array.IndexOf(fsmNames, action.selectedFSMName.Value);
                        if (index >= 0) action.selectedIndex = index;
                        else
                        {
                            action.selectedIndex = 0;
                            action.selectedFSMName.Value = fsmNames[0];
                        }
                    }

                    int newIndex = EditorGUILayout.Popup("FSM Name", action.selectedIndex, fsmNames);
                    if (newIndex != action.selectedIndex)
                    {
                        action.selectedIndex = newIndex;
                        if (action.selectedFSMName != null)
                            action.selectedFSMName.Value = fsmNames[newIndex];
                        FsmEditor.SaveActions();
                        GUI.changed = true;
                    }
                }
                else EditorGUILayout.HelpBox("El GameObject no tiene FSM.", MessageType.Warning);
            }
            else EditorGUILayout.HelpBox("Seleccioná un GameObject para listar FSMs.", MessageType.Info);
            EditField("eventToSend");
            EditField("onComplete");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Agrupar opciones avanzadas al final (gris suave)
            bool showOptions = EditorPrefs.GetBool(foldoutKey, true);
            GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
            EditorGUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            showOptions = EditorGUILayout.Foldout(showOptions, "OPTIONS", true);
            if (showOptions)
            {
                EditorGUI.indentLevel++;
                EditField("volume");
                EditField("textSpeed");
                EditField("clickDelayOffset");
                EditField("typingVolume");
                EditorGUI.indentLevel--;
            }
            EditorPrefs.SetBool(foldoutKey, showOptions);
            EditorGUILayout.EndVertical();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error en editor ShowDialogue: {ex}");
        }
        finally
        {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }
        return GUI.changed;
    }
}
#endif