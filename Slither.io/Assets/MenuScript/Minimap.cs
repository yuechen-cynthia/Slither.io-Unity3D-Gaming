using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {
	
	public GameObject Target;
	//Get the positon of snake.
	public Vector2 TrasformPosition(){ 
		Vector2 a = new Vector2(0,0);
		if (Target != null) {
			Vector2 newPosition = new Vector2 (Target.transform.position.x, Target.transform.position.y);
			return newPosition;
		}
		return a;

	}
}
