using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public float Mass = 15;
    public float MaxVelocity = 3;
    public float MaxForce = 15;

    private Vector3 velocity = Vector3.zero;

    private void Update()
	{
        if (Input.GetMouseButton(0))
        {
            Vector3 desiredPostion = Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData, 1000))
            {
                desiredPostion = hitData.point;
            }

            var desiredVelocity = desiredPostion - transform.position;
            desiredVelocity = desiredVelocity.normalized * MaxVelocity;

            var steering = desiredVelocity - velocity;
            steering = Vector3.ClampMagnitude(steering, MaxForce);
            steering /= Mass;

            velocity = Vector3.ClampMagnitude(velocity + steering, MaxVelocity);
            transform.position += velocity * Time.deltaTime;
            transform.forward = velocity.normalized;

            Debug.DrawRay(transform.position, velocity.normalized * 3, Color.green);
            Debug.DrawRay(transform.position, transform.forward * 2, Color.blue);
            Debug.DrawRay(transform.position, desiredVelocity.normalized * 2, Color.magenta);
        }
    }
}
