using System;
using UnityEngine;

public class WoodStepSound : MonoBehaviour
{
    public Transform Player;
    public LayerMask wood;
    public AudioSource woodSfx;
    public AudioClip[] WoodFloorClips;

    private Vector3 lastPosition;
    private float accumulatedDistance;
    public float stepDistance = 2f;

    private void Start() 
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        PlayerController playerController = Player.GetComponent<PlayerController>();
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(
            new Vector3(currentPosition.x, 0, currentPosition.z),
            new Vector3(lastPosition.x, 0, lastPosition.z)
        );

        accumulatedDistance += distanceMoved;

        if (accumulatedDistance >= stepDistance)
        {
            accumulatedDistance = 0f;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, wood))
            {
                woodSfx.PlayOneShot(WoodFloorClips[UnityEngine.Random.Range(0, WoodFloorClips.Length)]);
            }
        }

        lastPosition = currentPosition;
        
    }
}
