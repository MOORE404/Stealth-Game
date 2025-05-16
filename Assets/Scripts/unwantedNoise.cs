using UnityEngine;
using System.Collections;

public class unwantedNoise : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip alertNoise;
    public float destroyTime = 3f;
    public float noiseDelay = 1.5f;
    
    private SphereCollider noiseCollider;
    private Transform laserTransform;
    private bool triggered = false;

    void Start()
    {
        laserTransform = transform.Find("Laser");
        noiseCollider = GetComponent<SphereCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return; 
        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (audioSource != null && alertNoise != null)
                audioSource.PlayOneShot(alertNoise);

            if (laserTransform != null)
                Destroy(laserTransform.gameObject);

            if (noiseCollider != null)
                noiseCollider.enabled = false;

            EnemyController enemy = FindObjectOfType<EnemyController>();
            if (enemy != null)
            {
                Vector3 finalNoisePosition = transform.position;
                enemy.stateMachine.ChangeState(new State_Alarm(enemy, finalNoisePosition));
            }

            StartCoroutine(WaitAndDestroy());
        }
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject); 
    }
}
