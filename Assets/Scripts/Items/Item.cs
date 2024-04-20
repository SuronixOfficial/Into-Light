using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

public class Item : MonoBehaviour
{
    public string itemName;
    public bool isPickable;
    public ItemType itemType;

    private Vector3 originalPos;
    private Quaternion originalRotation;
    public Outline outlineScript;

    
    private void Awake()
    {
        outlineScript = GetComponent<Outline>();
    }
    private void OnDestroy()
    {
        switch (itemType)
        {
            case ItemType.Any:
                InteractionsManager.instance.anyItems.Remove(this);
                break;
            case ItemType.Weapon:
                InteractionsManager.instance.weaponItems.Remove(this);
                break;
            case ItemType.Food:
                InteractionsManager.instance.foodItems.Remove(this);
                break;
            case ItemType.Peasant:
                InteractionsManager.instance.peasantItems.Remove(this);
                break;
            case ItemType.Jar:
                InteractionsManager.instance.jarItems.Remove(this);
                break;
            case ItemType.Basket:
                InteractionsManager.instance.basketItems.Remove(this);
                break;

        }

        switch (itemName)
        {
            case "Shield":
                InteractionsManager.instance.shieldItems.Remove(this);
                break;
            case "Anvil":
                InteractionsManager.instance.anvilItems.Remove(this);
                break;
            case "Pretzel":
                InteractionsManager.instance.pretzelItems.Remove(this);
                break;
            case "Cutting board":
                InteractionsManager.instance.cuttingBoardItems.Remove(this);
                break;
        }
    }
    private void Start()
    {
        originalPos = transform.position;
        originalRotation = transform.rotation;

        switch(itemType)
        {
            case ItemType.Any:
                InteractionsManager.instance.anyItems.Add(this);
                break;
            case ItemType.Weapon:
                InteractionsManager.instance.weaponItems.Add(this);
                break;
            case ItemType.Food:
                InteractionsManager.instance.foodItems.Add(this);
                break;     
            case ItemType.Peasant:
                InteractionsManager.instance.peasantItems.Add(this);
                break;
            case ItemType.Jar:
                InteractionsManager.instance.jarItems.Add(this);
                break;
            case ItemType.Basket:
                InteractionsManager.instance.basketItems.Add(this);
                break;

        }

        switch(itemName)
        {
            case "Shield":
                InteractionsManager.instance.shieldItems.Add(this);
                break;
            case "Anvil":
                InteractionsManager.instance.anvilItems.Add(this);
                break;
            case "Pretzel":
                InteractionsManager.instance.pretzelItems.Add(this);
                break;
            case "Cutting board":
                InteractionsManager.instance.cuttingBoardItems.Add(this);
                break;
        }
       //if(!gameObject.GetComponent<BoxCollider>()) gameObject.AddComponent<BoxCollider>();
       // char[] letters = transform.name.ToCharArray();
       // letters[0] = char.ToUpper(letters[0]);
       // itemName = letters[0].ToString() + transform.name.Substring(1);
       // itemName = itemName.Replace("_", " ");
       // itemName = itemName.Replace("(", "");
       // itemName = itemName.Replace(")", "");
       // itemName = Regex.Replace(itemName, @"\d+", "");
       // isPickable = true;
       // transform.tag = "Item";
    }
    
    public void OnDrop()
    {
        Invoke("ResetItem",10f);
    }

    private void ResetItem()
    {
        Destroy(GetComponent<Rigidbody>());

        transform.position = originalPos;
        transform.rotation = originalRotation;
    }

    public void OnTriggerPickup(bool status)
    {
        foreach (Item item in InteractionsManager.instance.peasantItems)
        {
            if (!item) continue;

            item.outlineScript.enabled = status;
        }

        if (itemType == ItemType.Weapon)
        {
            foreach (Item item in InteractionsManager.instance.foodItems)
            {
                if (!item) continue;

                item.outlineScript.enabled = status;
            }
            foreach (Item item in InteractionsManager.instance.basketItems)
            {
                if (!item) continue;

                item.outlineScript.enabled = status;
            }
        }
        if (itemType == ItemType.Food)
        {
            foreach (Item item in InteractionsManager.instance.jarItems)
            {
                if (!item) continue;
                item.outlineScript.enabled = status;
            }
            foreach (Item item in InteractionsManager.instance.basketItems)
            {
                if (!item) continue;
                item.outlineScript.enabled = status;
            }
        }

        switch (itemName)
        {
            case "One handed sword":
                foreach (Item item in InteractionsManager.instance.shieldItems)
                {
                    if (!item) continue;
                    item.outlineScript.enabled = status;
                }
                break;
            case "Two handed sword":
                foreach (Item item in InteractionsManager.instance.shieldItems)
                {
                    if (!item) continue;
                    item.outlineScript.enabled = status;
                }
                break;
            case "Short sword":
                foreach (Item item in InteractionsManager.instance.shieldItems)
                {
                    if (!item) continue;
                    item.outlineScript.enabled = status;
                }
                break;
            case "Armor piece":
                foreach (Item item in InteractionsManager.instance.anvilItems)
                {
                    if (!item) continue;
                    item.outlineScript.enabled = status;
                }
                break;
            case "Piece of cheese":
                foreach (Item item in InteractionsManager.instance.pretzelItems)
                {
                    if (!item) continue;
                    item.outlineScript.enabled = status;
                }
                break;
            case "Meat":
                foreach (Item item in InteractionsManager.instance.cuttingBoardItems)
                {
                    if (!item) continue;
                    item.outlineScript.enabled = status;
                }
                break;
        }
    }
}
public enum ItemType
{
    Any,
    Weapon,
    Food,
    Peasant,
    Jar,
    Basket
}
