using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class wincollider : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip song;
    public GameObject winscreen;
    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player"){
            if (audioSource != null && song != null) audioSource.PlayOneShot(song);
            StartCoroutine(Win());
        }
    }

    public IEnumerator Win(){
        winscreen.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
