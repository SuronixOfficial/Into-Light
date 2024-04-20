using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;
using System;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Meta.Voice.Samples.TTSVoices;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-2)]
public class NarratorManager : MonoBehaviour
{
    [SerializeField] private string gasFunctionUrl = "";

    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private InputField narratorInputField;

    public string openingPhasePrompt, middlePhasePrompt, endingPhasePrompt;
    public TextAsset openingPhasePromptTextAsset, middlePhasePromptTextAsset, endingPhasePromptTextAsset;
    public List<string> actionLogs;

    public StoryPhase currentStoryPhase;

    private TTSSpeakerInput speakerInput;
    private SlowmotionManager slowmotionManager;
    public static NarratorManager instance;

    [SerializeField] private Animator answerTextAnim;

    public string story, quote;

    private void Awake()
    {
        LoadPrompts();
        instance = this;
        speakerInput = FindObjectOfType<TTSSpeakerInput>();
        slowmotionManager = FindObjectOfType<SlowmotionManager>();
    }
    public void OnEnable()
    {
        TTSSpeakerInput.OnEndNarrationEvent += OnEndNarration;
    }
    private void OnDisable()
    {
        TTSSpeakerInput.OnEndNarrationEvent -= OnEndNarration;
    }

    private void OnEndNarration()
    {
        answerTextAnim.SetBool("Play",false);

        if(currentStoryPhase == StoryPhase.Ending)
        {
            SavingManager.instance.OnAddStory(story);
            SceneManager.LoadScene(0);
        }
    }
    private void LoadPrompts()
    {
        openingPhasePrompt = openingPhasePromptTextAsset.text;
        middlePhasePrompt = middlePhasePromptTextAsset.text;
        endingPhasePrompt = endingPhasePromptTextAsset.text;
    }
    public void UpdateActionLogs(string newAction)
    {
        actionLogs.Add("-" + newAction);
        OnSubmitAnswer();
    }
    public void OnSubmitAnswer()
    {
        string prompt = "";

        switch(currentStoryPhase)
        {
            case StoryPhase.Opening:
                prompt = openingPhasePrompt;
                break;
            case StoryPhase.Middle:
                prompt = middlePhasePrompt;
                break;
            case StoryPhase.Ending:
                prompt = endingPhasePrompt;
                break;
        }

        slowmotionManager.EnableSlowMotion();

        if (prompt != "") prompt = prompt.Replace("<ACTION LOG>", actionLogs.ToSeparatedString("\n"));

        StartCoroutine(SendDataToGAS(prompt));
    }

    IEnumerator SendDataToGAS(string parameter)
    {
        string url = gasFunctionUrl;
        WWWForm form = new WWWForm();
        form.AddField("parameter", parameter);
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            answerText.text = www.downloadHandler.text;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        answerTextAnim.SetBool("Play", true);

        narratorInputField.text = answerText.text;
        if (story != "") story = story + "\n" + answerText.text;
        else story = answerText.text;
        speakerInput.SpeakClick();

        //if (currentStoryPhase == StoryPhase.Ending)
        //{

        //}
    }
}

public enum StoryPhase
{
    Opening,
    Middle,
    Ending
}

