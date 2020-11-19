using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    GameObject leftHand, rightHand;

    [SerializeField]
    TextMeshPro text;

    [SerializeField]
    Basket basket;

    Vector3 leftHandStartPos, rightHandStartPos;
    Vector3 leftHandPrevPos, rightHandPrevPos;
    Vector3 leftHandDist, rightHandDist;
    Vector3 leftHandMaxDist, rightHandMaxDist;

    int fruitsReq, vegReq, dairyReq, totalReqs;

    float totalTime;

    // Start is called before the first frame update
    void Start()
    {
        leftHandStartPos = leftHand.transform.position;
        rightHandStartPos = rightHand.transform.position;

        leftHandPrevPos = leftHandStartPos;
        rightHandPrevPos = rightHandStartPos;

        leftHandDist = Vector3.zero;
        rightHandDist = Vector3.zero;  

        leftHandMaxDist = Vector3.zero;
        rightHandMaxDist = Vector3.zero;  

        fruitsReq = Random.Range(0, 4);
        dairyReq = Random.Range(0, 4);
        vegReq = Random.Range(0, 4);
        totalReqs = fruitsReq + dairyReq + vegReq;
        basket.UpdateRequirements(new Vector3(fruitsReq, dairyReq, vegReq));

        totalTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (totalReqs == 0){
             GameOver(); 
             return;
        }
        UpdateHandDistance();
        text.text = fruitsReq + " Fruits needed\n" +
        vegReq + " Vegetables needed\n" + 
        dairyReq + " Dairy items needed\n" +
        "\nTime: " + totalTime.ToString("F1");
        totalTime += Time.deltaTime;
        UpdateRequirements();
    }

    void UpdateRequirements(){

        foreach(GameObject item in basket.GetItemsInCrate()){
            switch (item.tag)
            {
                case "Fruit":
                if (fruitsReq > 0) {
                    fruitsReq--;
                    basket.GotCorrectItem();
                }
                else basket.GotIncorrectItem();
                break;

                case "Dairy":
                if (dairyReq > 0) {
                    dairyReq--;
                    basket.GotCorrectItem();
                }
                else basket.GotIncorrectItem();
                break;

                case "Vegetable":
                if (vegReq > 0) {
                    vegReq--;
                    basket.GotCorrectItem();
                }
                else basket.GotIncorrectItem();
                break;

                default:
                break;
            }
        }

        basket.ClearItems();
        totalReqs = fruitsReq + dairyReq + vegReq;
    }


    void UpdateHandDistance()
    {
        Vector3 leftHandTempMax, rightHandTempMax;

        leftHandDist  += leftHand.transform.position - leftHandPrevPos;
        rightHandDist += rightHand.transform.position - rightHandPrevPos;

        leftHandTempMax = leftHand.transform.position - leftHandStartPos;
        if (leftHandTempMax.x > leftHandMaxDist.x) leftHandMaxDist.x = leftHandTempMax.x;
        if (leftHandTempMax.y > leftHandMaxDist.y) leftHandMaxDist.y = leftHandTempMax.y;
        if (leftHandTempMax.z > leftHandMaxDist.z) leftHandMaxDist.z = leftHandTempMax.z;

        rightHandTempMax = rightHand.transform.position - rightHandStartPos;
        if (rightHandTempMax.x > rightHandMaxDist.x) rightHandMaxDist.x = rightHandTempMax.x;
        if (rightHandTempMax.y > rightHandMaxDist.y) rightHandMaxDist.y = rightHandTempMax.y;
        if (rightHandTempMax.z > rightHandMaxDist.z) rightHandMaxDist.z = rightHandTempMax.z;

        leftHandPrevPos = leftHand.transform.position;
        rightHandPrevPos = rightHand.transform.position;
    }

    void GameOver()
    {
        Time.timeScale = 0f;

        Vector3 leftHandAvgSpeed = leftHandDist / totalTime;
        Vector3 rightHandAvgSpeed = rightHandDist / totalTime;


        /*
        Available metrics:

        - For each hand
            - Avg Hand speed, 3D
            - Avg Path length between each object release, 3D
            - Total distance, 3D
        - Total time
        - Shelves reached (green for object grabbed from the shelf, red for no object reached from that shelf)
        - Grabbing and placement accuracy (%)

        */




        text.text = "Total time: " + totalTime;
    }





}
