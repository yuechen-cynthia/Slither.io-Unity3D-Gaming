using UnityEngine;
using System.Collections;

public class foodSpeicalEffect : MonoBehaviour {

    private Vector3 startSize = Vector3.zero;
    private Vector3 endSize;

    void Awake() {
        transform.localScale = startSize;
    }

    void Update() {
        endSize = Vector3.Slerp(endSize, Vector3.one, Time.deltaTime*3);
        transform.localScale = endSize;
    }

}
