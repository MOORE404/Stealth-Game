using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float suspicionLevel;
    public waypoint waypoint;
    public Transform target;
    public float sightFov = 110f;
    public Animator animator;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public bool seenTarget = false;
    [HideInInspector] public Vector3 lastSeenPosition;

    public SphereCollider detectionCollider;
    private float baseDetectionRadius = 10f;

    public StateMachine stateMachine = new StateMachine();
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!(stateMachine.currentState is State_Attack)){
            stateMachine.ChangeState(new State_Patrol(this));
        }

        detectionCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        suspicionLevel = CalculateSuspicion();
        stateMachine.Update();
        if (!(stateMachine.currentState is State_Attack)) 
        {
            if (suspicionLevel > 50)
            {
                stateMachine.ChangeState(new State_Look(this));
            }
        }



    }


    public void Death()
    {
        stateMachine.ChangeState(new State_Death(this));
    }


private float CalculateSuspicion() //Inspiraton from Alien isolation
{
    float newSuspicionLevel = suspicionLevel; 
    if (target == null) return newSuspicionLevel;

    float detectionRadius = detectionCollider.radius;
    float closeRangeRadius = baseDetectionRadius * 0.3f; // Increased close range influence
    float distanceToPlayer = Vector3.Distance(transform.position, target.position);

    Vector3 rayOrigin = transform.position + Vector3.up * 1.5f; // Enemy eye level
    Vector3 targetPosition = target.position + Vector3.up * 0.5f; // Player chest level
    Vector3 direction = (targetPosition - rayOrigin).normalized;

    float angle = Vector3.Angle(transform.forward, direction);

    // Angle Multiplier (Sharper angles increase suspicion faster)
    float angleMultiplier = 1.2f - (angle / (sightFov * 0.5f));
    angleMultiplier = Mathf.Clamp(angleMultiplier, 0.2f, 1.2f); // Stronger effect

    // Distance Multiplier (Closer increases suspicion rapidly)
    float distanceMultiplier = Mathf.Clamp01(1.5f / (distanceToPlayer * 0.4f));

    // Posture Multiplier (Crouching slightly reduces suspicion gain)
    PlayerController playerController = target.GetComponent<PlayerController>();
    float postureMultiplier = (playerController != null && playerController.isCrouching) ? 1.5f : 10f;

    // Base suspicion gain rate
    float baseSuspicionGain = 50f * Time.deltaTime;

    // Final suspicion calculation
    float suspicionGain = baseSuspicionGain * angleMultiplier * distanceMultiplier * postureMultiplier;

    bool playerInFOV = angle <= sightFov * 0.5f;
    bool playerBehindEnemy = angle > sightFov * 0.5f && distanceToPlayer < detectionRadius;

    RaycastHit hit;
    bool canSeePlayer = false;

    // Perform Raycast check
    if (Physics.Raycast(rayOrigin, direction, out hit, detectionRadius))
    {
        if (hit.collider.transform == target)
        {
            canSeePlayer = true;
            lastSeenPosition = target.position;
        }
    }

    // Only increase suspicion if the player is visible (not just in FOV)
    if (playerInFOV && canSeePlayer)
    {
        newSuspicionLevel += suspicionGain;
    }
    else if (playerBehindEnemy && canSeePlayer)
    {
        float behindEnemySuspicionGain = baseSuspicionGain * 0.4f * distanceMultiplier * postureMultiplier;
        newSuspicionLevel += behindEnemySuspicionGain;
    }

    // Stronger suspicion gain in close range
    if (distanceToPlayer < closeRangeRadius)
    {
        newSuspicionLevel += 80f * Time.deltaTime;
    }

    if (canSeePlayer== false){
        newSuspicionLevel = newSuspicionLevel * .99f;
    }

    // Clamp suspicion level
    newSuspicionLevel = Mathf.Clamp(newSuspicionLevel, 0f, 100f);

    // Debug.Log($"Suspicion Level: {newSuspicionLevel:F2} | Angle: {angle:F1}° | Distance: {distanceToPlayer:F2} | " +
    //           $"AngleMultiplier: {angleMultiplier:F2} | DistanceMultiplier: {distanceMultiplier:F2} | " +
    //           $"PostureMultiplier: {postureMultiplier:F2} | " +
    //           $"BehindEnemy: {playerBehindEnemy} | CanSeePlayer: {canSeePlayer}"); 

    return newSuspicionLevel;
}

public bool CanSeePlayer()
{
    if (target == null) return false; // No target to check

    Vector3 rayOrigin = transform.position + Vector3.up * 1.5f; // Enemy eye level
    Vector3 targetPosition = target.position + Vector3.up * 0.5f; // Player chest level
    Vector3 direction = (targetPosition - rayOrigin).normalized;

    float distanceToPlayer = Vector3.Distance(transform.position, target.position);
    float angle = Vector3.Angle(transform.forward, direction);

    bool inFOV = angle <= sightFov * 0.5f; // Check if within FOV
    bool inRange = distanceToPlayer <= detectionCollider.radius; // Check if within range

    RaycastHit hit;
    
    // Debugging
    Debug.DrawRay(rayOrigin, direction * detectionCollider.radius, Color.red, 0.1f);

    // Perform Raycast check
    bool canSee = false;
    if (Physics.Raycast(rayOrigin, direction, out hit, detectionCollider.radius))
    {
        if (hit.collider.transform == target)
        {
            canSee = true; // Player is visible via raycast
        }
    }

    // Ensure all conditions are met before setting to true
    if (inFOV && inRange && canSee)
    {
        lastSeenPosition = target.position; // Store last seen position
        return true;
    }

    return false; // Player is not in sight
}







void OnDrawGizmos()
{
    if (detectionCollider == null) return;

    float detectionRadius = detectionCollider.radius;

    // Draw FOV Arc
    Gizmos.color = Color.yellow;
    Vector3 fovLine1 = Quaternion.AngleAxis(sightFov * 0.5f, Vector3.up) * transform.forward * detectionRadius;
    Vector3 fovLine2 = Quaternion.AngleAxis(-sightFov * 0.5f, Vector3.up) * transform.forward * detectionRadius;

    Gizmos.DrawLine(transform.position, transform.position + fovLine1);
    Gizmos.DrawLine(transform.position, transform.position + fovLine2);

    int numSteps = 20;
    for (int i = 0; i <= numSteps; i++)
    {
        float angle = -sightFov * 0.5f + (i / (float)numSteps) * sightFov;
        Vector3 fovStep = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * detectionRadius;
        Gizmos.DrawLine(transform.position, transform.position + fovStep);
    }

    // Draw Raycast (ONLY if the target exists)
    if (target != null)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f; // Enemy eye level
        Vector3 targetPosition = target.position + Vector3.up * 0.5f; // Player chest level
        Vector3 direction = (targetPosition - rayOrigin).normalized;
        
        Gizmos.color = Color.red; // Raycast debug line
        Gizmos.DrawLine(rayOrigin, rayOrigin + direction * detectionRadius);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, direction, out hit, detectionRadius))
        {
            // If the raycast hits something, mark the hit position
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hit.point, 0.2f); // Small sphere at hit location

            // Log the object name that was hit
        }
    }
}




}
