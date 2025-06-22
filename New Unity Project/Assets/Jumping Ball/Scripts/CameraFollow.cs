using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public GameObject player;//The object that the camera will follow
	private Vector3 velocity = Vector3.zero;

	void Update () 
	{
		if (player == null) return;

		if(player.transform.position.x < -2.5f || player.transform.position.x > 2.5f)
		{
			//Move the camera if the ball's x position is lower then -2.5 or higher then 2.5
			transform.position = Vector3.SmoothDamp (transform.position, new Vector3(player.transform.position.x, Vars.cameraMaxYPos + 3, -10), ref velocity, 0.12f);
		}else 
		{
			//If the position of the ball is from -2.5 to 2.5 camera will move only on the y axis
			transform.position = Vector3.SmoothDamp (transform.position, new Vector3(0, Vars.cameraMaxYPos + 3, -10), ref velocity, 0.5f);
		}
		
	}
}
