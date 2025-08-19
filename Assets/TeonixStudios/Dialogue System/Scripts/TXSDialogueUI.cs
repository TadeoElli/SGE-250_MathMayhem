using HutongGames.PlayMaker;
using System.Collections;
using TMPro;
using UnityEngine;

public class TXSDialogueUI : MonoBehaviour
{
    [Header("DIALOGUE SETUP")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public AudioSource audioSource;
    public Transform charactersContainer;

    [Header("OPTIONS")]
    public float textSpeed = 0.02f;
    public float clickDelayOffset = 0f;
    public float typingVolume = 0.01f;
    public float ttsVolume = 0.2f; // Volumen de TTS configurable
    public float ttsRate = 1f; // Velocidad de TTS configurable

    [Header("EVENTS")]
    public GameObject eventTarget;
    public string fsmName;
    public string eventToSend;

    [Header("CLICK INDICATOR")]
    public GameObject clickIndicator;
    public float blinkOnTime = 0.8f;
    public float blinkOffTime = 0.5f;
    public AudioClip clickReadySound;
    public float clickReadySoundVolume = 1f; // Volumen configurable

    private string[] keys;
    private string[] avatarNames;
    private int currentIndex;
    private bool canClick;
    private float clickDelay;
    private System.Action onDialogueComplete;
    private string currentAvatarName;
    private Coroutine blinkRoutine;

    public void StartDialogue(
        string[] lineKeys,
        string[] avatarObjectNames = null,
        AudioClip voiceClip = null,
        float? volume = null,
        float? speed = null,
        float? delayOffset = null,
        float? typingVol = null,
        GameObject eventObj = null,
        string FSMName = null,
        string eventName = null,
        System.Action callback = null)
    {
        keys = lineKeys;
        avatarNames = avatarObjectNames;
        currentIndex = 0;
        textSpeed = speed ?? textSpeed;
        clickDelayOffset = delayOffset ?? clickDelayOffset;
        typingVolume = typingVol ?? typingVolume;
        eventTarget = eventObj;
        fsmName = FSMName;
        eventToSend = eventName;
        onDialogueComplete = callback;
        currentAvatarName = (avatarNames != null && avatarNames.Length == 1) ? avatarNames[0] : string.Empty;

        if (voiceClip != null && volume.HasValue)
        {
            audioSource.clip = voiceClip;
            audioSource.volume = volume.Value;
            audioSource.Play();
        }

        dialoguePanel.SetActive(true);
        ShowNextLine();
    }

    public void OnClick()
    {
        if (!canClick) return;
        currentIndex++;
        if (currentIndex < keys.Length) ShowNextLine();
        else EndDialogue();
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        audioSource.Stop();
        onDialogueComplete?.Invoke();
        if (eventTarget != null && !string.IsNullOrEmpty(eventToSend))
        {
            FsmUtils.SendEventToFSM(eventTarget, fsmName, eventToSend);
        }
        StopBlinking();
    }

    private void ShowNextLine()
    {
        string key = keys[currentIndex];
        string htmlLine = TXSDialogueManager.Instance.GetDialogueLine(key);
        string ttsKey = TXSDialogueManager.Instance.GetTTSLineKey(key);
        string ttsDialogue = string.Empty;

        foreach (Transform child in charactersContainer) child.gameObject.SetActive(false);

        if (avatarNames != null && avatarNames.Length > currentIndex && !string.IsNullOrEmpty(avatarNames[currentIndex]))
            currentAvatarName = avatarNames[currentIndex];

        if (!string.IsNullOrEmpty(currentAvatarName))
        {
            var avatar = charactersContainer.Find(currentAvatarName);
            if (avatar != null) avatar.gameObject.SetActive(true);
        }

        var globalBool = FsmVariables.GlobalVariables.FindFsmBool("TTSenabled");
        bool ttsEnabled = globalBool != null && globalBool.Value;
        clickDelay = ttsEnabled
            ? TXSDialogueManager.Instance.GetClipLength(key) + clickDelayOffset
            : clickDelayOffset;

        if (ttsEnabled)
        {
            // Llamada a  TTS
            Debug.Log(">>>>>>>>>> TTS SERVICE: " + ttsKey);
            string currentLanguage = FsmVariables.GlobalVariables.GetFsmString("Language").Value;
            ttsDialogue = TXSDialogueManager.Instance.GetTTSDialogueLine(key);
            BrowserTTS.Speak(ttsDialogue, currentLanguage, ttsVolume, ttsRate);
        }
        StartCoroutine(TypeText(htmlLine));
    }

    private IEnumerator TypeText(string fullText)
    {
        dialogueText.text = string.Empty;
        canClick = false;
        StopBlinking();

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.volume = typingVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        int i = 0;
        while (i < fullText.Length)
        {
            if (fullText[i] == '<')
            {
                int endTag = fullText.IndexOf('>', i);
                if (endTag == -1) endTag = i;
                string tag = fullText.Substring(i, endTag - i + 1);
                dialogueText.text += tag;
                i = endTag + 1;
            }
            else
            {
                dialogueText.text += fullText[i];
                if (fullText[i] != ' ' && audioSource != null && !audioSource.isPlaying)
                    audioSource.Play();
                i++;
            }
            yield return new WaitForSeconds(textSpeed);
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }

        yield return new WaitForSeconds(clickDelay);
        canClick = true;
        StartBlinking();
    }

    private void StartBlinking()
    {
        if (clickIndicator == null) return;
        clickIndicator.SetActive(true);
        blinkRoutine = StartCoroutine(BlinkIndicator());
    }

    private void StopBlinking()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
        }
    }

    private IEnumerator BlinkIndicator()
    {
        while (true)
        {
            clickIndicator.SetActive(true);
            if (clickReadySound != null && audioSource != null)
            {
                if (!audioSource.isPlaying || audioSource.clip != clickReadySound)
                {
                    audioSource.PlayOneShot(clickReadySound, clickReadySoundVolume);
                }
            }
            yield return new WaitForSeconds(blinkOnTime);
            clickIndicator.SetActive(false);
            yield return new WaitForSeconds(blinkOffTime);
        }
    }
}
