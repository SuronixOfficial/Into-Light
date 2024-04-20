using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class InteractionsManager : MonoBehaviour
{
    [SerializeField] private ItemToItemInteractionEvent[] itemToItemInteractionEvents;
    private NarratorManager narratorManager;

    public static InteractionsManager instance;

    private Item lastPickedUpItem, lastRecieverItem;

    public List<Item> anyItems, weaponItems, foodItems,peasantItems, jarItems, basketItems;
    public List<Item> shieldItems, anvilItems, pretzelItems, cuttingBoardItems;

    private ObjectiveManager objectiveManager;

    private void Awake()
    {
        instance = this; 

        narratorManager = NarratorManager.instance;
        objectiveManager = FindObjectOfType<ObjectiveManager>();
    }
    public void ActionToolTip(Item pickedUpItem, Item recieverItem)
    {
        bool status = false;

        if (recieverItem.itemType == ItemType.Peasant)
        {
            if (pickedUpItem.itemType == ItemType.Weapon)
            {
                ToolTipManager.instance.toolTipActionText.text = "Kill";

            }
            else
            {
                ToolTipManager.instance.toolTipActionText.text = "Donate";
            }
            status = true;
        }
        if (recieverItem.itemType == ItemType.Food && pickedUpItem.itemType == ItemType.Weapon)
        {
            ToolTipManager.instance.toolTipActionText.text = "Cut";

            status = true;
        }
        if (recieverItem.itemType == ItemType.Jar && pickedUpItem.itemType == ItemType.Food)
        {
            ToolTipManager.instance.toolTipActionText.text = "Put in jar";

            status = true;
        }
        if (recieverItem.itemType == ItemType.Basket && pickedUpItem.itemType == ItemType.Food)
        {
            ToolTipManager.instance.toolTipActionText.text = "Put in basket";

            status = true;
        }
        if (recieverItem.itemType == ItemType.Basket && pickedUpItem.itemType == ItemType.Weapon)
        {
            ToolTipManager.instance.toolTipActionText.text = "Poke hole";

            status = true;
        }

        foreach (ItemToItemInteractionEvent interactionEvent in itemToItemInteractionEvents)
        {
            if (interactionEvent.interactionName.ToLower() == pickedUpItem.itemName.ToLower() + "->" + recieverItem.itemName.ToLower())
            {
                ToolTipManager.instance.toolTipActionText.text = interactionEvent.toolTipAction;
                status = true;
            }
        }

        ToolTipManager.instance.toolTipAction.SetActive(status);

    }
    public bool CanItemToItem(Item pickedUpItem, Item recieverItem)
    {
        if (recieverItem.itemType == ItemType.Peasant) return true;
        if (recieverItem.itemType == ItemType.Food && pickedUpItem.itemType == ItemType.Weapon) return true;
        if (recieverItem.itemType == ItemType.Jar && pickedUpItem.itemType == ItemType.Food) return true;
        if (recieverItem.itemType == ItemType.Basket && pickedUpItem.itemType == ItemType.Food) return true;
        if (recieverItem.itemType == ItemType.Basket && pickedUpItem.itemType == ItemType.Weapon) return true;

        foreach (ItemToItemInteractionEvent interactionEvent in itemToItemInteractionEvents)
        {
            if (interactionEvent.interactionName.ToLower() == pickedUpItem.itemName.ToLower() + "->" + recieverItem.itemName.ToLower())
            {
                return true;
            }
        }

        return false;
    }
    public void OnItemToItem(Item pickedUpItem, Item recieverItem)
    {
        objectiveManager.NextObjective();

        lastPickedUpItem = pickedUpItem;
        lastRecieverItem = recieverItem;

        if (recieverItem.itemType == ItemType.Peasant)
        {
            if (pickedUpItem.itemType != ItemType.Weapon)
            {
                Donation();
            }

            else
            {   
                Murder();
            }

            return;
        }
        if (recieverItem.itemType == ItemType.Food && pickedUpItem.itemType == ItemType.Weapon)
        {
            CutFood();

            return;
        }
        if (recieverItem.itemType == ItemType.Jar && pickedUpItem.itemType == ItemType.Food)
        {
            PutFoodInJarOrBasket();

            return;
        }
        if (recieverItem.itemType == ItemType.Basket && pickedUpItem.itemType == ItemType.Food)
        {
            PutFoodInJarOrBasket();

            return;
        }
        if (recieverItem.itemType == ItemType.Basket && pickedUpItem.itemType == ItemType.Weapon)
        {
            PokeHoleInBasket();

            return;
        }

        foreach (ItemToItemInteractionEvent interactionEvent in itemToItemInteractionEvents)
        {
            if(interactionEvent.interactionName == pickedUpItem.itemName + "->" + recieverItem.itemName)
            {
                foreach (UnityEvent callEvent in interactionEvent.callEvents)
                {
                    callEvent.Invoke();
                }

                narratorManager.UpdateActionLogs(interactionEvent.actionName);

                break;
            }
        }
    }

    public void PutFoodInJarOrBasket()
    {
        narratorManager.UpdateActionLogs("Put a " + lastPickedUpItem.itemName + " in a " + lastRecieverItem.itemName);
        DestroyPickedUpItem();
    }

    public void CutFood()
    {
        narratorManager.UpdateActionLogs("Cut a " + lastRecieverItem.itemName + " with a " + lastPickedUpItem.itemName);
        DestroyRecieverUpItem();
    }
    public void Donation()
    {
        narratorManager.UpdateActionLogs("Donated a " + lastPickedUpItem.itemName + " to a peasant");
        lastRecieverItem.transform.Find("Model").GetComponent<Animator>().SetTrigger("Happy");
        DestroyPickedUpItem();

    }
    public void Murder()
    {
        narratorManager.UpdateActionLogs("Murdered a peasant with a " + lastPickedUpItem.itemName);
        lastRecieverItem.GetComponent<Collider>().enabled = false;
        lastRecieverItem.GetComponent<Item>().enabled = false;
        lastRecieverItem.transform.Find("Model").GetComponent<Animator>().SetTrigger("Die");
        lastRecieverItem.outlineScript.enabled = false;
;       peasantItems.Remove(lastRecieverItem);
    }

    public void DestroyPickedUpItem()
    {
        FindObjectOfType<ItemInteract>().OnItemDeleted();
        Destroy(lastPickedUpItem.gameObject);
        ToolTipManager.instance.toolTipAction.SetActive(false);
    }
    public void DestroyRecieverUpItem()
    {
        Destroy(lastRecieverItem.gameObject);
    }
    public void PutOnCuttingBoard()
    {
        lastPickedUpItem.transform.SetParent(lastRecieverItem.transform.Find("Meat position"));
        lastPickedUpItem.transform.localPosition = Vector3.zero;
        lastPickedUpItem.transform.localEulerAngles = Vector3.zero;

        FindObjectOfType<ItemInteract>().OnItemDeleted();
        ToolTipManager.instance.toolTipAction.SetActive(false);
    }

    public void PokeHoleInBasket()
    {
        narratorManager.UpdateActionLogs("Poke a hole in a " + lastRecieverItem.itemName + " with a " + lastPickedUpItem.itemName);
    }
}

[System.Serializable]
public class ItemToItemInteractionEvent
{
    public string interactionName;
    public string actionName;
    public string toolTipAction;
    public UnityEvent[] callEvents;  
}