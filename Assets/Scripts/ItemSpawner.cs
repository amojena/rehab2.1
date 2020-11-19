using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] items;

    // Start is called before the first frame update
    void Start()
    {
        GameObject item = items[Random.Range(0, items.Length)];

        if (transform.parent.gameObject.CompareTag("Right Wall") && item.name.StartsWith("Milk")){
            Quaternion tempRot = item.transform.rotation;
            tempRot.y *= -1;
            item.transform.rotation = tempRot;
        }

        Instantiate(item, transform.position, item.transform.rotation);
    }
}
