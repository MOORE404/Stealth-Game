using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class UnderSurfaceCheck : MonoBehaviour
{
    public BoxCollider enemyCollider;
    public float timer = 2f;
    public float timermax = 2f;


    public bool isNearLowSurface = false;

    public bool IsNearLowSurface
    {
        get { return isNearLowSurface; }
    }

    public LayerMask lowSurfaceLayer;

    void Start()
    {
        if (enemyCollider == null)
        {
            Debug.LogError("Enemy Collider not assigned! Please assign a BoxCollider.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LowSurface"))
        {
            if (enemyCollider.bounds.Intersects(other.bounds))
            {
                isNearLowSurface = true;
                Debug.Log("Enemy is near the low surface.");
            }
        }
    }

    void Update()
    {
        Vector3 rayOrigin = enemyCollider.bounds.center;
        float rayDistance = 1f;
        Debug.DrawRay(rayOrigin, Vector3.up * rayDistance, Color.red); 
        if (isNearLowSurface  )
        {
            timer -= Time.deltaTime;
            if (timer <0){
            
            timer = timermax;

           
            if (!Physics.Raycast(rayOrigin, Vector3.up, rayDistance, lowSurfaceLayer, QueryTriggerInteraction.Collide))

            {
                isNearLowSurface = false;
                Debug.Log("No surface detected above. Enemy can stop crawling.");
            }
            else
            {
                            
            }
            }
        }
    }

void OnDrawGizmosSelected()
{
    if (enemyCollider != null)
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = enemyCollider.bounds.center;
        float rayDistance = 1f;
        Gizmos.DrawRay(rayOrigin, Vector3.up * rayDistance);
    }
}

}
