using Meta.Voice.Samples.TTSVoices;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Meta.Voice.Samples.TTSVoices.TTSSpeakerInput;

public class ItemInteract : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float raycastRange;

    [Space(10)]
    [Header("Interaction Settings")]
    [SerializeField] private Transform itemPosition;

    public Item focusedOnItem;
    public Item pickedUpItem;

    public bool inItemFocus;
    public bool hasItem;
    public bool canItemToItemInteraction;
    public bool inNarration;

    private Camera playerCam;
    private ToolTipManager toolTipManager;
    private RaycastHit hit;
    private InteractionsManager interactionsManager;

    private void Awake()
    {
        playerCam = Camera.main;
        toolTipManager = ToolTipManager.instance;
        interactionsManager = InteractionsManager.instance;
    }
    private void Start()
    {
       // inNarration = true;
    }
    public void OnEnable()
    {
        SlowmotionManager.OnEndSlowmotionEvent += OnEndNarration;
    }
    private void OnDisable()
    {
        SlowmotionManager.OnEndSlowmotionEvent -= OnEndNarration;
    }

    private void OnEndNarration()
    {
        inNarration = false;
    }
    private void LateUpdate()
    {
        ItemsRaycast();
        CheckForItems();

        if (inNarration) return;

        if (pickedUpItem || focusedOnItem) ItemInteraction();

        if (hasItem)
        {
            CheckForItemToItemInteraction();
            return;
        }
    }

    private void ItemInteraction()
    {
        if (!hasItem && focusedOnItem && focusedOnItem.isPickable && (!ToolTipManager.instance.toolTipAction.activeSelf || ToolTipManager.instance.toolTipActionText.text != "Pickup"))
        {
            ToolTipManager.instance.toolTipAction.SetActive(true);
            ToolTipManager.instance.toolTipActionText.text = "Pickup";
        }
        if(!hasItem && !focusedOnItem.isPickable && ToolTipManager.instance.toolTipAction.activeSelf) ToolTipManager.instance.toolTipAction.SetActive(false);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canItemToItemInteraction && interactionsManager.CanItemToItem(pickedUpItem,focusedOnItem))
            {
                interactionsManager.OnItemToItem(pickedUpItem, focusedOnItem);
                inNarration = true;
                return;
            }

            if (!hasItem)
            {
                if (!focusedOnItem.isPickable) return;
                PickupItem();
            }
            else
            {
                if (!pickedUpItem.isPickable || canItemToItemInteraction) return;
                DropItem();
            }

           
            hasItem = !hasItem;
        }
    }

    private void PickupItem()
    {
        pickedUpItem = focusedOnItem;

        toolTipManager.TriggerToolTipBox(false);
        if(pickedUpItem.transform.GetComponent<Rigidbody>()) Destroy(pickedUpItem.transform.GetComponent<Rigidbody>());
        pickedUpItem.transform.GetComponent<Collider>().isTrigger = true;
        pickedUpItem.gameObject.layer = 2;
        pickedUpItem.CancelInvoke("ResetItem");
        pickedUpItem.transform.SetParent(itemPosition);
        pickedUpItem.transform.localPosition = Vector3.zero;
        pickedUpItem.transform.localEulerAngles = Vector3.zero;
        pickedUpItem.OnTriggerPickup(true);

    }

    private void DropItem()
    {
        pickedUpItem.transform.AddComponent<Rigidbody>();
        pickedUpItem.transform.GetComponent<Collider>().isTrigger = false;
        pickedUpItem.OnDrop();
        pickedUpItem.gameObject.layer = 0;

        pickedUpItem.transform.SetParent(null);
        pickedUpItem.OnTriggerPickup(false);

        pickedUpItem = null;

        OnItemLoseFocus();
    }
    private void ItemsRaycast()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, raycastRange, raycastMask)) ;
    }

    private void CheckForItems()
    {
        if(hit.collider)
        {
            if (!hit.collider.CompareTag("Item"))
            {
                if(inItemFocus) OnItemLoseFocus();

                return;
            }

            if (inItemFocus) return;

            focusedOnItem = hit.collider.GetComponent<Item>();

            toolTipManager.UpdateToolTipText(focusedOnItem.itemName);
            toolTipManager.TriggerToolTipBox(true);

            inItemFocus = true;
        }
        else if(inItemFocus)
        {
            OnItemLoseFocus();
        }
    }
    private void CheckForItemToItemInteraction()
    {
        if(hit.collider && hit.collider.CompareTag("Item"))
        {
            if (canItemToItemInteraction) return;

            interactionsManager.ActionToolTip(pickedUpItem,focusedOnItem);
            canItemToItemInteraction = true;
        }
        else if(canItemToItemInteraction)
        {
            canItemToItemInteraction = false;
        }
    }

    private void OnItemLoseFocus()
    {
        toolTipManager.TriggerToolTipBox(false);

        focusedOnItem = null;

        inItemFocus = false;
    }
    public void OnItemDeleted()
    {
        pickedUpItem.OnTriggerPickup(false);
        pickedUpItem = null;
        hasItem = false;
        canItemToItemInteraction = false;
    }
}
