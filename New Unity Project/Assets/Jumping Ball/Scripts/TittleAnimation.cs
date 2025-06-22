using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TittleAnimation : MonoBehaviour {

	float scale = 1;//Scale of the tittle game object
	float speed = 20;//Animation speed
	bool up = true;//When this variable is true zoom in animation will be triggered, if it's false the zoom out animation will be triggered

	void Update () 
	{
		if(up) //If up is true, value of the scale variable will start to increase
		{
			scale += Time.deltaTime / speed;
			if(scale >= 1.05f)//If value of the scale variable is 1.05 or more, start scaling down that variable
			{
				up = false;
			}
		}
		else //If up is true, value of the scale variable will start to decrease
		{
			scale -= Time.deltaTime / speed;
			if(scale <= 1f) //If value of the scale variable is 1 or less, start scaling up that variable
			{
				up = true;
			}
		}			
		GetComponent<RectTransform>().localScale = new Vector2(scale, scale);//This will scale the tittle game object up and down
	}
}
