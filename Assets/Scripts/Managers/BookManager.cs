using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leftSideText, rightSideText;
    [SerializeField] private TextMeshProUGUI leftSideTitleText, rightSideTitleText;
    [SerializeField] private Book book;
    [SerializeField] private GameObject noStorySign;

    private SavingManager savingManager;

    private void Start()
    {
        savingManager = SavingManager.instance;
        OnUpdateBook();
    }
    public void OnUpdateBook()
    {
        if (savingManager.stories.Count == 0)
        {
            noStorySign.SetActive(true);
            return;
        }

        if (savingManager.stories.Count > book.currentPage)
        {
            leftSideText.text = savingManager.stories[book.currentPage];
            leftSideTitleText.text = "Story " + (book.currentPage + 1) + ":";

            if (leftSideText.text.IndexOf("\"") != -1)
            {
                leftSideText.text = leftSideText.text.Substring(0, leftSideText.text.IndexOf("\""))
             + "<b>" + leftSideText.text.Substring(leftSideText.text.IndexOf("\""))
             + "</b>";
            }

        }
        else
        {
            leftSideText.text = "";
            leftSideTitleText.text = "";
        }
        if (savingManager.stories.Count > book.currentPage + 1)
        {
            rightSideText.text = savingManager.stories[book.currentPage + 1];
            rightSideTitleText.text = "Story " + (book.currentPage + 2) + ":";

            if (rightSideText.text.IndexOf("\"") != -1)
            {
                leftSideText.text = leftSideText.text.Substring(0, leftSideText.text.IndexOf("\""))
             + "<b>" + leftSideText.text.Substring(leftSideText.text.IndexOf("\""))
             + "</b>";
            }
        }
        else
        {
            rightSideText.text = "";
            rightSideTitleText.text = "";
        }
    }

}
