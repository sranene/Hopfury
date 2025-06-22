using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vars : MonoBehaviour
{   //This script is used to store static variables that are used throughout the game
    public static float cameraMaxYPos = -3; //Used in "CameraFollow.cs" script to determine the camera's y pos. When the ball falls of the platform camera will not follow ball's y position downward
    public static string currentLevel = "0"; //Used in "Menus.cs" script to determine which level should be loaded
}
