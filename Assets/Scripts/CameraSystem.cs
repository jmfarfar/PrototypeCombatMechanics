using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [SerializeField] float minDistanceToTarget;
    [SerializeField] float cameraMovementSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 midPoint = (player1.position + player2.position) / 2;

        float distanceBetweenPlayers = Vector3.Distance(player1.position, player2.position);
        
        Vector3 targetPosition = 
        midPoint + 
        new Vector3(
            0f, 
            distanceBetweenPlayers * 0.3f, 
            -(Mathf.Max(minDistanceToTarget, 
            distanceBetweenPlayers)));
        
        transform.position = Vector3.Lerp(transform.position, 
        targetPosition,
        cameraMovementSpeed * Time.deltaTime);

        Vector3 vectorToTarget = (midPoint - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(vectorToTarget);
    }
}
