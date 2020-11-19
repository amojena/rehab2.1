using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    List<GameObject> items;
    Quaternion startRot;
    int fruitReq;
    int dairyReq;
    int vegReq;

    Color origColor;

    AudioSource[] audioSources;



    // Start is called before the first frame update
    void Start()
    {
        items = new List<GameObject>();
        startRot = transform.rotation;
        origColor = GetComponent<MeshRenderer>().material.color;
        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Maintain starting rotation at all times
        transform.rotation = startRot;
    }

    private void OnCollisionEnter(Collision collision) { items.Add(collision.gameObject); }

    public List<GameObject> GetItemsInCrate(){ return items; }

    public void ClearItems(){ 

        while (items.Count > 0){
            GameObject toDestroy = items[items.Count-1];
            items.Remove(toDestroy);
            Destroy(toDestroy);
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