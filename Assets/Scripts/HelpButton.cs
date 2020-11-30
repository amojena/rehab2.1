using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpButton : MonoBehaviour
{
    Color green = Color.green;
    Color red = Color.red;

    public bool needed = false;

    [SerializeField]
    TextMeshPro debugText;
    Vector3 startPos;
    Quaternion startRot;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = startRot;
        transform.position = new Vector3(startPos.x, transform.position.y, startPos.z);
    }

    private void OnCollisionEnter(Collision collision){
        
        debugText.text = collision.gameObject.name;
        Color currColor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = currColor == green ? red : green; 
        needed = !needed;
    }
}
