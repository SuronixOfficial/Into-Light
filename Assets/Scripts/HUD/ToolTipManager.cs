using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DefaultExecutionOrder(-1)]
public class ToolTipManager : MonoBehaviour
{
    [SerializeField] private GameObject toolTipBox;
    [SerializeField] private TextMeshProUGUI toolTipText;

    public TextMeshProUGUI toolTipActionText;
    public GameObject toolTipAction;

    public bool isToolTipBoxOpen;

    public static ToolTipManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void TriggerToolTipBox(bool status)
    {
        isToolTipBoxOpen = status;

        toolTipBox.SetActive(status);
    }
    public void UpdateToolTipText(string text)
    {
        toolTipText.text = text;
    }
}
