using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Material[] bothMaterials;

    Material[] originalMaterial;
    HelpButton helpButton;

    [SerializeField]
    MeshRenderer meshRenderer;

    bool bothMatsApplied = false;

    SceneController sceneController;

    Dictionary<string, int> reqs = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        helpButton = GameObject.Find("HelpButton").GetComponent<HelpButton>();
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        bothMaterials = meshRenderer.materials;
        originalMaterial = new Material[] {bothMaterials[0]};
        meshRenderer.materials = originalMaterial;
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (helpButton.needed && !bothMatsApplied && sceneController.GetRequirement(gameObject.tag) > 0){
        meshRenderer.materials = bothMaterials;
        bothMatsApplied = true;
        }
        else if (!helpButton.needed && bothMatsApplied) {
            meshRenderer.materials = originalMaterial;
            bothMatsApplied = false;
        }
    }

    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Crate"))
            gameObject.SetActive(false);
    }
}
