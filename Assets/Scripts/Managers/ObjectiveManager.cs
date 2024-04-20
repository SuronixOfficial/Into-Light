using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private GameObject objectiveBox;

    private NarratorManager narratorManager;

    [SerializeField] private int neededInteractions = 5;
    private int madeInteractionsCount;
    private bool hasEnded;

    private void Start()
    {
        narratorManager = NarratorManager.instance;

        OpeningScene();
    }

    private void OpeningScene()
    {
        narratorManager.OnSubmitAnswer();
        narratorManager.currentStoryPhase++;
    }
    public void NextObjective()
    {
        madeInteractionsCount++;
        objectiveText.text = "The journey ends after " + (neededInteractions - madeInteractionsCount).ToString() + " interactions";

        if(hasEnded)
        {
            objectiveBox.SetActive(false);
            return;
        }
        if (madeInteractionsCount >= neededInteractions)
        {
            narratorManager.currentStoryPhase++;
            hasEnded = true;
            
         }
    }

}
