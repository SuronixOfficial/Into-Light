using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public string itemName;
    public GameObject[] itemPool;

    public GameObject GetItem()
    {
        int randomItemIndex = Random.Range(0, itemPool.Length);

        return Instantiate(itemPool[randomItemIndex]);
    }
}
