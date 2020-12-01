﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] items;

    Basket basket;
    bool spawned = false;

    // Start is called before the first frame update
    void Start()
    {
        basket = GameObject.Find("Basket").GetComponent<Basket>();
    }

    private void Update()
    {
        if (basket.ReadyToStart() && !spawned) { 

            GameObject item = items[Random.Range(0, items.Length)];
            Quaternion tempRot = item.transform.rotation;

            if (transform.parent.gameObject.CompareTag("Right Wall") && item.name.StartsWith("Milk"))
            {
                tempRot.y += 180;
            }

            Instantiate(item, transform.position, tempRot);
            spawned = true;
        }
}
}
