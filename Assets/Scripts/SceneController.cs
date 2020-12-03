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
    float leftHandDist, rightHandDist;
    Vector3 leftHandMaxDist, rightHandMaxDist;

    int fruitsReq, vegReq, dairyReq, totalReqs;

    float totalTime;


    float correctInteracton, incorrectInteraction;

    List<Item> itemsInGame;

    int itemsGrabbed = 0;
    float relPathLength = 0f;
    float leftDistWhileGrabbing = 0f;
    float rightDistWhileGrabbing = 0f;
    int grabbedByLeft = 0;
    int grabbedByRight = 0;

    // Start is called before the first frame update
    void Start()
    {
        itemsInGame = new List<Item>();
        leftHandStartPos = leftHand.transform.position;
        rightHandStartPos = rightHand.transform.position;

        leftHandPrevPos = leftHandStartPos;
        rightHandPrevPos = rightHandStartPos;

        leftHandDist = 0f;
        rightHandDist = 0f;  

        leftHandMaxDist = Vector3.zero;
        rightHandMaxDist = Vector3.zero;  

        fruitsReq = Random.Range(1, 4);
        dairyReq = Random.Range(1, 4);
        vegReq = Random.Range(1, 4);
        totalReqs = fruitsReq + dairyReq + vegReq;
        basket.UpdateRequirements(new Vector3(fruitsReq, dairyReq, vegReq));

        totalTime = 0f;
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
    }

    public void AddToScene(Item item) {itemsInGame.Add(item);}

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
        float leftTempDist, rightTempDist;

        leftTempDist = Mathf.Abs(Vector3.Distance(leftHand.transform.position, leftHandPrevPos));
        rightTempDist = Mathf.Abs(Vector3.Distance(rightHand.transform.position, rightHandPrevPos));

        leftHandDist += leftTempDist;
        rightHandDist += rightTempDist;

        leftHandTempMax = GetAbsoluteVector(leftHand.transform.position - leftHandStartPos);
        if (leftHandTempMax.x > leftHandMaxDist.x) leftHandMaxDist.x = leftHandTempMax.x;
        if (leftHandTempMax.y > leftHandMaxDist.y) leftHandMaxDist.y = leftHandTempMax.y;
        if (leftHandTempMax.z > leftHandMaxDist.z) leftHandMaxDist.z = leftHandTempMax.z;

        rightHandTempMax = GetAbsoluteVector(rightHand.transform.position - rightHandStartPos);
        if (rightHandTempMax.x > rightHandMaxDist.x) rightHandMaxDist.x = rightHandTempMax.x;
        if (rightHandTempMax.y > rightHandMaxDist.y) rightHandMaxDist.y = rightHandTempMax.y;
        if (rightHandTempMax.z > rightHandMaxDist.z) rightHandMaxDist.z = rightHandTempMax.z;

        leftHandPrevPos = leftHand.transform.position;
        rightHandPrevPos = rightHand.transform.position;
    }

    public int GetRequirement(string itemType)
    {
        if (itemType.Equals("Fruit")) return fruitsReq;
        else if (itemType.Equals("Vegetable")) return vegReq;
        else if (itemType.Equals("Dairy")) return dairyReq;
        else return -1;
    }

    bool IsCrateGrabbed(){ return basket.GetComponent<OVRGrabbable>().isGrabbed; }

    Vector3 GetAbsoluteVector(Vector3 vector){
        if (vector.x < 0) vector.x *= -1;
        if (vector.y < 0) vector.y *= -1;
        if (vector.z < 0) vector.z *= -1;
        return vector;
    }

    string VectorToString(Vector3 v) { return "(" + v.x.ToString("F2") + "m " + v.y.ToString("F2") + "m " + v.z.ToString("F2") + "m)";  }

    public void ItemDroppedInBasket(Item item)
    { 
        if (item.grabbedBy.Equals("CustomHandLeft"))
        {
            leftDistWhileGrabbing += item.distanceTraveled;
            grabbedByLeft++;
        }
        else if (item.grabbedBy.Equals("CustomHandRight"))
        {
            rightDistWhileGrabbing += item.distanceTraveled;
            grabbedByRight++;
        }

    }
    void GameOver()
    {

        float leftHandAvgSpeed = leftHandDist / totalTime;
        float rightHandAvgSpeed = rightHandDist / totalTime;

        leftHandText.text = "Avg Hand Speed: " + leftHandAvgSpeed.ToString("F2") + "m/s";
        leftHandText.text += "\n Distance Traveled: " + leftHandDist.ToString("F2") + "m";
        leftHandText.text += "\n Maximum Distance Reached: (m): " + VectorToString(GetAbsoluteVector(leftHandMaxDist));
        leftHandText.text += "\n Average relative path length: " + (leftDistWhileGrabbing/grabbedByLeft).ToString("F2") + "m";

        rightHandText.text = "Avg Hand Speed: " + rightHandAvgSpeed.ToString("F2") + "m/s";
        rightHandText.text += "\n Distance Traveled: " + rightHandDist.ToString("F2") + "m";
        rightHandText.text += "\n Maximum Distance Reached: (m): " + VectorToString(GetAbsoluteVector(rightHandMaxDist));
        rightHandText.text += "\n Avg relative path length: " + (rightDistWhileGrabbing / grabbedByRight).ToString("F2") + "m";

        leftHandText.gameObject.SetActive(true);
        rightHandText.gameObject.SetActive(true);

        relPathLength = leftDistWhileGrabbing + rightDistWhileGrabbing;
        relPathLength /= (grabbedByLeft + grabbedByRight);

        float accuracy = correctInteracton / (correctInteracton + incorrectInteraction);     
        bottomHandText.text = "Total time: " + totalTime.ToString("F2");
        bottomHandText.text += "\nSuccessful interactions: " + (accuracy*100).ToString("F2") + "%";
        bottomHandText.text += "\nVisual cues used: " + helpButton.GetTimesNeeded().ToString();
        bottomHandText.text += "\n Avg relative Path Length: " + relPathLength.ToString("F2");
        bottomHandText.gameObject.SetActive(true);

        text.gameObject.SetActive(false);
        basketResetText.SetActive(true);

        if(IsCrateGrabbed()) SceneManager.LoadScene("PantryScene", LoadSceneMode.Single);
    }





}
