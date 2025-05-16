using UnityEngine;
using System.Collections;

public class ThrowableObject : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip song;
    public float destroyTime = 3f; 
    private SphereCollider noiseCollider;
    public float noiseDelay = 1.5f; 

    void Start()
    {
        noiseCollider = GetComponent<SphereCollider>();
        if (audioSource != null && song != null) audioSource.PlayOneShot(song);


        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name + " | Tag: " + other.tag);

        if (other.CompareTag("Enemy")) 
        {
            EnemyController enemy = other.GetComponent<EnemyController>();

            if (enemy != null && !(enemy.stateMachine.currentState is State_Attack))
            {
                Debug.Log("Enemy detected: " + other.gameObject.name + " will investigate in " + noiseDelay + " seconds.");

                StartCoroutine(DelayedNoiseAlert(enemy));
            }
        }
    }

    private IEnumerator DelayedNoiseAlert(EnemyController enemy)
    {
        yield return new WaitForSeconds(noiseDelay);

        if (enemy != null) 
        {
            Vector3 finalNoisePosition = transform.position; 
            Debug.Log("Enemy now investigating at: " + finalNoisePosition);
            enemy.stateMachine.ChangeState(new State_NoiseMaker(enemy, finalNoisePosition));
        }
    }
}
