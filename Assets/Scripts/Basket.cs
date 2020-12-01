using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    List<string> items;
    Quaternion startRot;
    int fruitReq;
    int dairyReq;
    int vegReq;

    Color origColor;

    AudioSource[] audioSources;

    bool readyToStart;

    [SerializeField]
    GameObject startPos;



    // Start is called before the first frame update
    void Start()
    {
        items = new List<string>();
        startRot = transform.rotation;
        origColor = GetComponent<MeshRenderer>().material.color;
        audioSources = GetComponents<AudioSource>();
        readyToStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Maintain starting rotation at all times
        if (!readyToStart && GetComponent<OVRGrabbable>().isGrabbed) {
            readyToStart = true;
            //transform.position = startPos.transform.position;
        }
        transform.rotation = startRot;
    }

    private void OnCollisionEnter(Collision collision) { 
        if (!readyToStart){ return; }

        items.Add(collision.gameObject.tag); 
    }

    public bool ReadyToStart() { return readyToStart; }
    public List<string> GetItemsInCrate(){ return items; }

    public void ClearItems(){ 

        while (items.Count > 0){
            string toDestroy = items[items.Count-1];
            items.Remove(toDestroy);
        }
    }

    public void UpdateRequirements(Vector3 reqs){
        fruitReq = (int)reqs.x;
        dairyReq = (int)reqs.y;
        vegReq   = (int)reqs.z;
    }

    public void GotIncorrectItem(){
        GetComponent<MeshRenderer>().material.color = Color.red;
        audioSources[0].Play();
        Invoke(nameof(RevertToOriginalColor), 1f);
    }

    public void GotCorrectItem(){
        GetComponent<MeshRenderer>().material.color = Color.green;
        audioSources[1].Play();
        Invoke(nameof(RevertToOriginalColor), 1f);
    }

    void RevertToOriginalColor() {  GetComponent<MeshRenderer>().material.color = origColor; }
}