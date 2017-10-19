using UnityEngine;
using System.Collections;

public class SnakeBodyActions : MonoBehaviour {

    private int myOrder;    // The order of this part in the whole snake
    private Transform head;     // The location of snake head
    private Vector3 movementVelocity;   // The velocity of current part
    [Range(0.0f, 1.0f)]
    //public float smoothTime = 0.2f;    // The smooth time when a body part follows head
    public float smoothTime = 0.05f;
	//public float smoothTime = 0.3f;

    void Start() {
        head = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        for (int i = 0; i < head.GetComponent<SnakeMovement>().bodyParts.Count; i++) {
            if (gameObject == head.GetComponent<SnakeMovement>().bodyParts[i].gameObject) {
                myOrder = i;
                break;
            }
        }
    }

    void FixedUpdate() {
        // If the body part is the first one, then it follows the head
        if (myOrder == 0) {
            transform.position = Vector3.SmoothDamp(transform.position, head.position, ref movementVelocity, smoothTime);
            // Rotates the transform so the forward vector points at target's current position
            transform.LookAt(head.transform.position);
        }
        // If not, then it follows previous body part
        else {
            transform.position = Vector3.SmoothDamp(transform.position,
                head.GetComponent<SnakeMovement>().bodyParts[myOrder - 1].position, ref movementVelocity, smoothTime);
            transform.LookAt(head.GetComponent<SnakeMovement>().bodyParts[myOrder - 1].position);
        }

    }

}
