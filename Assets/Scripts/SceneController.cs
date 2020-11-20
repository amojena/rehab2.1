using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    GameObject leftHand, rightHand;

    [SerializeField]
    TextMeshPro text;

    [SerializeField]
    Basket basket;

    [SerializeField]
    GameObject table, startOfTable, endOfTable;

    [SerializeField]
    TextMeshPro leftHandText, rightHandText;

    [SerializeField]
    GameObject basketResetText;

    Vector3 leftHandStartPos, rightHandStartPos;
    Vector3 leftHandPrevPos, rightHandPrevPos;
    Vector3 leftHandDist, rightHandDist;
    Vector3 leftHandMaxDist, rightHandMaxDist;

    int fruitsReq, vegReq, dairyReq, totalReqs;

    float totalTime;

    bool gameOver;

    float correctInteracton, incorrectInteraction;

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

        fruitsReq = Random.Range(1, 4);
        dairyReq = Random.Range(1, 4);
        vegReq = Random.Range(1, 4);
        totalReqs = fruitsReq + dairyReq + vegReq;
        basket.UpdateRequirements(new Vector3(fruitsReq, dairyReq, vegReq));

        totalTime = 0f;
        gameOver = false;
        correctInteracton = 0;
        incorrectInteraction = 0;
        text.text = "Adjust table to desired height.\nGrab the crate to begin.";
    }

    // Update is called once per frame
    void Update()
    {
        table.transform.rotation = Quaternion.Euler(Vector3.zero);
        if (!basket.ReadyToStart()) return;
        if (totalReqs == 0){
             GameOver(); 
             return;
        }
        UpdateHandDistance();
        UpdateText();
        totalTime += Time.deltaTime;
        UpdateRequirements();
        UpdateBasketPos();
    }

    void UpdateBasketPos(){
        basket.transform.position = Vector3.Lerp(basket.transform.position, endOfTable.transform.position, Time.deltaTime * .02f);
    }

    void UpdateText(){

        text.text = fruitsReq + " Fruits needed\n" +
        vegReq + " Vegetables needed\n" + 
        dairyReq + " Dairy items needed\n" +
        "\nTime: " + totalTime.ToString("F1");
    }
    void UpdateRequirements(){

        foreach(string item in basket.GetItemsInCrate()){
            switch (item)
            {
                case "Fruit":
                if (fruitsReq > 0) {
                    fruitsReq--;
                    correctInteracton++;
                    basket.GotCorrectItem();
                }
                else {
                    incorrectInteraction++;
                    basket.GotIncorrectItem();
                }
                break;

                case "Dairy":
                if (dairyReq > 0) {
                    dairyReq--;
                    correctInteracton++;
                    basket.GotCorrectItem();
                }
                else {
                    incorrectInteraction++;
                    basket.GotIncorrectItem();
                }
                break;

                case "Vegetable":
                if (vegReq > 0) {
                    vegReq--;
                    correctInteracton++;
                    basket.GotCorrectItem();
                }
                else {
                    incorrectInteraction++;
                    basket.GotIncorrectItem();
                }
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


    bool IsCrateGrabbed(){ return basket.GetComponent<OVRGrabbable>().isGrabbed; }

    Vector3 GetAbsoluteVector(Vector3 vector){
        if (vector.x < 0) vector.x *= -1;
        if (vector.y < 0) vector.y *= -1;
        if (vector.z < 0) vector.z *= -1;
        return vector;
    }

    void GameOver()
    {
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
        
        Vector3 leftHandAvgSpeed = leftHandDist / totalTime;
        Vector3 rightHandAvgSpeed = rightHandDist / totalTime;

        leftHandText.text = "Average Hand Speed (m/s): " + GetAbsoluteVector(leftHandAvgSpeed);
        leftHandText.text += "\n Total Distance: (m): " + GetAbsoluteVector(leftHandDist);
        leftHandText.text += "\n Maximum Distance Reached: (m): " + GetAbsoluteVector(leftHandMaxDist);
        // Missing path length when grabbing an object
        
        rightHandText.text = "Average Hand Speed (m/s): " + GetAbsoluteVector(rightHandAvgSpeed);
        rightHandText.text += "\n Total Distance: (m): " + GetAbsoluteVector(rightHandDist);
        rightHandText.text += "\n Maximum Distance Reached: (m): " + GetAbsoluteVector(rightHandMaxDist);
        // Missing path length when grabbing an object

        leftHandText.gameObject.SetActive(true);
        rightHandText.gameObject.SetActive(true);

        if (!gameOver) text.gameObject.transform.position = text.gameObject.transform.position + new Vector3(0,-.35f,0);

        float accuracy = correctInteracton / (correctInteracton + incorrectInteraction);
        text.text = "Total time: " + totalTime.ToString("F2");
        text.text += "\nSuccessful interactions: " + (accuracy*100).ToString("F2") + "%";

        basketResetText.SetActive(true);


        if(IsCrateGrabbed()) SceneManager.LoadScene("PantryScene", LoadSceneMode.Single);
        gameOver = true;
    }





}
