using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [SerializeField] float minDistanceToTarget;
    [SerializeField] float cameraMovementSpeed;

    void LateUpdate()
    {
        Vector3 midPoint = (player1.position + player2.position) / 2;

        float distanceBetweenPlayers = Vector3.Distance(player1.position, player2.position);

        float clampedDistance = Mathf.Max(minDistanceToTarget, distanceBetweenPlayers);

        Vector3 targetPosition =
        new Vector3(midPoint.x * 0.5f, midPoint.y, midPoint.z) +
        new Vector3(
            0f,
            Mathf.Sqrt(clampedDistance),
            -clampedDistance
        );

        transform.position = Vector3.Lerp(transform.position,
        targetPosition,
        cameraMovementSpeed * Time.deltaTime);

        Vector3 vectorToTarget = (midPoint - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(vectorToTarget);
    }
}
