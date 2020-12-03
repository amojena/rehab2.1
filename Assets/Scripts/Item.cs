using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    Material[] bothMaterials;

    Material[] originalMaterial;
    HelpButton helpButton;

    [SerializeField]
    MeshRenderer meshRenderer;

    bool bothMatsApplied = false;

    SceneController sceneController;

    GameObject basket;

    float shortestDistToBasket;
    public float distanceTraveled = 0f;
    Vector3 prevPos;

    OVRGrabbable grabbable;
    public bool wasGrabbed = false;
    public string grabbedBy = "null";

    TextMeshPro debugText;
    

    // Start is called before the first frame update
    void Start()
    {
        helpButton = GameObject.Find("HelpButton").GetComponent<HelpButton>();
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        basket = GameObject.Find("Basket");
        grabbable = GetComponent<OVRGrabbable>();


        bothMaterials = meshRenderer.materials;
        originalMaterial = new Material[] {bothMaterials[0]};
        meshRenderer.materials = originalMaterial;

        shortestDistToBasket = Mathf.Abs(Vector3.Distance(basket.transform.position, transform.position));
        prevPos = transform.position;

        debugText = GameObject.Find("DebugText").GetComponent<TextMeshPro>();
        Debug.Log(shortestDistToBasket);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMaterial();

        if (grabbable.isGrabbed)
        {
            wasGrabbed = true;
            grabbedBy = grabbable.grabbedBy.gameObject.name;
            debugText.text = grabbable.grabbedBy.name;
        }


        distanceTraveled += Mathf.Abs(Vector3.Distance(transform.position, prevPos));
        prevPos = transform.position;
    }

    void UpdateMaterial()
    {
        if (helpButton.needed && !bothMatsApplied && sceneController.GetRequirement(gameObject.tag) > 0)
        {
            meshRenderer.materials = bothMaterials;
            bothMatsApplied = true;
        }
        else if (!helpButton.needed && bothMatsApplied)
        {
            meshRenderer.materials = originalMaterial;
            bothMatsApplied = false;
        }
    }

    public float CalcRelativeDist()
    {
        return distanceTraveled / shortestDistToBasket;
    }

    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Crate"))
            gameObject.SetActive(false);
    }
}
