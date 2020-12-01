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
    TextMeshPro leftHandText, rightHandText, bottomHandText;

    [SerializeField]
    GameObject basketResetText;

    [SerializeField]
    HelpButton helpButton;

    Vector3 leftHandStartPos, rightHandStartPos;
    Vector3 leftHandPrevPos, rightHandPrevPos;
    Vector3 leftHandDist, rightHandDist;
    Vector3 leftHandMaxDist, rightHandMaxDist;

    int fruitsReq, vegReq, dairyReq, totalReqs;

    float totalTime;

    bool gameOver;

    float correctInteracton, incorrectInteraction;

    List<GameObject> itemsInGame;

    OVRGrabber leftHandOVR, rightHandOVR;

    Vector3 leftDistWhileGrabbing, rightDistWhileGrabbing;
    float timeGrabbing = 0f;

    // Start is called before the first frame update
    void Start()
    {
        itemsInGame = new List<GameObject>();
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

        leftHandOVR = GameObject.Find("CustomHandLeft").GetComponent<OVRGrabber>();
        rightHandOVR = GameObject.Find("CustomHandRight").GetComponent<OVRGrabber>();
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
        //UpdateBasketPos();
    }

    public void AddToScene(GameObject item) {itemsInGame.Add(item);}

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
            // item = tag of gameobject
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

        leftHandDist  += GetAbsoluteVector(leftHand.transform.position - leftHandPrevPos);
        rightHandDist += GetAbsoluteVector(rightHand.transform.position - rightHandPrevPos);

        leftHandTempMax = GetAbsoluteVector(leftHand.transform.position - leftHandStartPos);
        if (leftHandTempMax.x > leftHandMaxDist.x) leftHandMaxDist.x = leftHandTempMax.x;
        if (leftHandTempMax.y > leftHandMaxDist.y) leftHandMaxDist.y = leftHandTempMax.y;
        if (leftHandTempMax.z > leftHandMaxDist.z) leftHandMaxDist.z = leftHandTempMax.z;

        rightHandTempMax = GetAbsoluteVector(rightHand.transform.position - rightHandStartPos);
        if (rightHandTempMax.x > rightHandMaxDist.x) rightHandMaxDist.x = rightHandTempMax.x;
        if (rightHandTempMax.y > rightHandMaxDist.y) rightHandMaxDist.y = rightHandTempMax.y;
        if (rightHandTempMax.z > rightHandMaxDist.z) rightHandMaxDist.z = rightHandTempMax.z;


        if (leftHandOVR.grabbedObject != null)
        {
            leftDistWhileGrabbing += GetAbsoluteVector(leftHand.transform.position - leftHandPrevPos);
            timeGrabbing += Time.deltaTime;
        }

        if (rightHandOVR.grabbedObject != null)
        {
            rightDistWhileGrabbing += GetAbsoluteVector(rightHand.transform.position - rightHandPrevPos);
            timeGrabbing += Time.deltaTime;
        }



        leftHandPrevPos = leftHand.transform.position;
        rightHandPrevPos = rightHand.transform.position;
    }

    public int GetRequirement(string itemType)
    {
       switch (itemType)
        {
            case "Fruit":
                return fruitsReq;
            case "Vegetable":
                return vegReq;
            case "Dairy":
                return dairyReq;
            default:
                return -1;
        }
    }

    bool IsCrateGrabbed(){ return basket.GetComponent<OVRGrabbable>().isGrabbed; }

    Vector3 GetAbsoluteVector(Vector3 vector){
        if (vector.x < 0) vector.x *= -1;
        if (vector.y < 0) vector.y *= -1;
        if (vector.z < 0) vector.z *= -1;
        return vector;
    }

    Vector3 DivideVectors(Vector3 a, Vector3 b)
    {
        Vector3 aa = GetAbsoluteVector(a);
        Vector3 bb = GetAbsoluteVector(b);
        return new Vector3(aa.x / bb.x, aa.y / bb.y, aa.z / bb.z);
    }

    void GameOver()
    {
        /*
        Available metrics:

        - For each hand
            - Avg Hand speed, 3D
            - Avg Path length when grabbing object, 3D
            - Total distance, 3D
        - Total time
        - Grabbing and placement accuracy (%)
        - # of times visual cues were needed

        */

        Vector3 leftHandAvgSpeed = leftHandDist / totalTime;
        Vector3 rightHandAvgSpeed = rightHandDist / totalTime;

        leftHandText.text = "Average Hand Speed (m/s): " + GetAbsoluteVector(leftHandAvgSpeed);
        leftHandText.text += "\n Total Distance: (m): " + GetAbsoluteVector(leftHandDist);
        leftHandText.text += "\n Maximum Distance Reached: (m): " + GetAbsoluteVector(leftHandMaxDist);
        leftHandText.text += "\n Average path length (m): " + DivideVectors(leftDistWhileGrabbing, leftHandDist);
        
        rightHandText.text = "Average Hand Speed (m/s): " + GetAbsoluteVector(rightHandAvgSpeed);
        rightHandText.text += "\n Total Distance: (m): " + GetAbsoluteVector(rightHandDist);
        rightHandText.text += "\n Maximum Distance Reached: (m): " + GetAbsoluteVector(rightHandMaxDist);
        rightHandText.text += "\n Average path length (m): " + DivideVectors(rightDistWhileGrabbing, rightHandDist);

        leftHandText.gameObject.SetActive(true);
        rightHandText.gameObject.SetActive(true);

        if (!gameOver) text.gameObject.transform.position = text.gameObject.transform.position + new Vector3(0,-.35f,0);

        float accuracy = correctInteracton / (correctInteracton + incorrectInteraction);
        bottomHandText.text = "Total time: " + totalTime.ToString("F2");
        bottomHandText.text += "\nSuccessful interactions: " + (accuracy*100).ToString("F2") + "%";
        bottomHandText.text += "\nVisual cues used: " + helpButton.GetTimesNeeded().ToString();
        bottomHandText.gameObject.SetActive(true);

        text.gameObject.SetActive(false);

        basketResetText.SetActive(true);


        if(IsCrateGrabbed()) SceneManager.LoadScene("PantryScene", LoadSceneMode.Single);
        gameOver = true;
    }





}
