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
    TextMeshPro helpText;

    float waitTimer;
    bool timer = false;

    int timesNeeded = 0;
    Basket basket;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        helpText = GetComponentInChildren<TextMeshPro>();
        startPos = transform.position;
        startRot = transform.rotation;
        waitTimer = 0f;
        basket = GameObject.Find("Basket").GetComponent<Basket>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = startRot;
        transform.position = startPos;

        if (timer)
        {
            waitTimer += Time.deltaTime;
            helpText.text = (5f - waitTimer).ToString("F2");
        }

        if (waitTimer >= 5f) {
            timer = false;
            needed = false;
            waitTimer = 0f;
            GetComponent<MeshRenderer>().material.color = red;
            helpText.text = "HELP";

        }
    }

    public int GetTimesNeeded() { return timesNeeded;  }



    private void OnCollisionExit(Collision collision){
        // game has not started
        if (!basket.ReadyToStart()) return;
        // platform on which button stands collision

        if (collision.gameObject.name.Equals("Platform")) return;
        // visual cue cooldown

        if (timer) return;

       
        GetComponent<MeshRenderer>().material.color = green;
        needed = true;
        timer = true;
        timesNeeded++;
    }
}
