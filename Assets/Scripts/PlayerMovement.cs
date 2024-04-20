using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private CharacterController controller;

    private Vector3 velocity;

    public List<string> items = new List<string>();

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

    }
    private void Update()
    {
        //foreach(Item item in FindObjectsOfType<Item>())
        //{
        //    if(!items.Contains(item.itemName)) items.Add(item.itemName);
        //}
        //FindObjectOfType<TextMeshProUGUI>().text = (items.ToSeparatedString("\n"));
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    foreach (MeshRenderer col in FindObjectsOfType<MeshRenderer>())
        //    {
        //        if (!col.GetComponent<Collider>())
        //        {
        //            if(col.transform.parent)
        //            {
        //                if (col.transform.parent.GetComponent<Collider>()) continue;
        //            }

        //            col.AddComponent<MeshCollider>();
        //        }
        //    }
        //}
        Movement();
    }

    private void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(x, 0, z).normalized;
        Vector3 moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.z;

        velocity.y += -9.81f * Time.deltaTime;

        controller.Move(((moveDirection * speed + velocity) * Time.deltaTime));

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
    }

}
