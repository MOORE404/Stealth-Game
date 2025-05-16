using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kill_Check : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip deathSound;
    public GameObject endscreen;
    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player"){
            if (audioSource != null && deathSound != null) audioSource.PlayOneShot(deathSound);
            StartCoroutine(death());
        }
    }

    public IEnumerator death(){
        endscreen.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
