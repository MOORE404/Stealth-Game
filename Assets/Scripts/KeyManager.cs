using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


public class KeyManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip winClip;
    public AudioClip failClip;
    public GameObject interactUIPrefab;
    public GameObject interactUIFailPrefab;
    public Transform camTransform;
    public GameObject interactUI;
    public Transform rayOrigin;
    private float rayDistance = 3.5f;
    public LayerMask keyLayer;
    public LayerMask doorLayer;
    public LayerMask noiseMakerLayer;
    public int keyAmount;
    public int noiseMakerAmount = 0;
    public bool lookingAtKey = false;
    public bool lookingAtDoor = false;
    public bool lookingAtNoiseMaker = false;
    RaycastHit hit;
    void Update()
    {
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, rayDistance, keyLayer, QueryTriggerInteraction.Collide)){
            if (Input.GetKeyDown(KeyCode.E)){
                keyAmount++;
                GameObject HitObject = hit.collider.gameObject;
                Destroy(HitObject);
                if (audioSource != null && winClip != null) audioSource.PlayOneShot(winClip);
            }
            Debug.Log("rayhit key");
            if (lookingAtKey == false){
                Vector3 spawnPosition = camTransform.position + camTransform.forward * .6f;
                interactUI = Instantiate(interactUIPrefab, spawnPosition, quaternion.identity);
                interactUI.transform.LookAt(camTransform.position);
                interactUI.transform.localScale = new Vector3(0.0001f,0.0001f, 0.0001f);
                Debug.Log("Instantiating UI");
            }
            lookingAtKey = true;
            if(lookingAtKey == true){
                interactUI.transform.LookAt(camTransform.position);
                interactUI.transform.position = camTransform.position + camTransform.forward * .6f;
            }

        }
        else{
            lookingAtKey = false;
            if (interactUI != null && lookingAtDoor == false && lookingAtKey == false && lookingAtNoiseMaker == false) Destroy(interactUI);
        }



        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, rayDistance, doorLayer, QueryTriggerInteraction.Collide)){
            if (Input.GetKeyDown(KeyCode.E) && keyAmount > 0){
                GameObject HitObject = hit.collider.gameObject;
                // hitobject in this context is door u can use gameobject as a ref to door to get animator and make it open once you interact
                HitObject.layer = default;
                Destroy(HitObject.transform.Find("barrier").gameObject);
                HitObject.GetComponent<Animator>().enabled = true;
                audioSource.PlayOneShot(winClip);
            }
            if (Input.GetKeyDown(KeyCode.E) && keyAmount == 0){
                audioSource.PlayOneShot(failClip);
                Destroy(interactUI);
                Vector3 spawnPosition = camTransform.position + camTransform.forward * .6f;
                interactUI = Instantiate(interactUIFailPrefab, spawnPosition, quaternion.identity);
                interactUI.transform.LookAt(camTransform.position);
                interactUI.transform.localScale = new Vector3(0.0001f,0.0001f, 0.0001f);
                Debug.Log("Instantiating fail UI");
            }
            
            Debug.Log("rayhit key");
            if (lookingAtDoor == false){
                Vector3 spawnPosition = camTransform.position + camTransform.forward * .6f;
                interactUI = Instantiate(interactUIPrefab, spawnPosition, quaternion.identity);
                interactUI.transform.LookAt(camTransform.position);
                interactUI.transform.localScale = new Vector3(0.0001f,0.0001f, 0.0001f);
                Debug.Log("Instantiating UI");
            }
            lookingAtDoor = true;
            if(lookingAtDoor == true){
                interactUI.transform.LookAt(camTransform.position);
                interactUI.transform.position = camTransform.position + camTransform.forward * .6f;
            }

        }
        else{
            lookingAtDoor = false;
            if (interactUI != null && lookingAtDoor == false && lookingAtKey == false && lookingAtNoiseMaker == false){
                Destroy(interactUI);
            }
        }



        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, rayDistance, noiseMakerLayer, QueryTriggerInteraction.Collide)){
            if (Input.GetKeyDown(KeyCode.E)){
                noiseMakerAmount++;
                GameObject HitObject = hit.collider.gameObject;
                Destroy(HitObject);
                if (audioSource != null && winClip != null) audioSource.PlayOneShot(winClip);
            }
            Debug.Log("rayhit noiseMaker");
            if (lookingAtNoiseMaker == false){
                Vector3 spawnPosition = camTransform.position + camTransform.forward * .6f;
                interactUI = Instantiate(interactUIPrefab, spawnPosition, quaternion.identity);
                interactUI.transform.LookAt(camTransform.position);
                interactUI.transform.localScale = new Vector3(0.0001f,0.0001f, 0.0001f);
                Debug.Log("Instantiating UI");
            }
            lookingAtNoiseMaker = true;
            if(lookingAtNoiseMaker == true){
                interactUI.transform.LookAt(camTransform.position);
                interactUI.transform.position = camTransform.position + camTransform.forward * .6f;
            }

        }
        else{
            lookingAtNoiseMaker = false;
            if (interactUI != null && lookingAtDoor == false && lookingAtKey == false && lookingAtNoiseMaker == false) Destroy(interactUI);
        }
    }



private void OnDrawGizmos()
{
    if (rayOrigin != null)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin.position, rayOrigin.position + rayOrigin.forward * rayDistance);
    }
}

}
