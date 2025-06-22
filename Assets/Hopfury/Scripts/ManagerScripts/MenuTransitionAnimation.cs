using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTransitionAnimation : MonoBehaviour {

    public int menu = 0;//This variable is set from the "Menus.cs" script, and it will determine which menu should be shown after transition animation
    public Image image;//Menu transition image
    private bool up = true;//When this variable is true fade in animation will be triggered, if it's false the fade out animation will be triggered
    private float alpha = 0;//This variable will determine the transparency of the image of the "image" variable

    void Update() 
    {
        if(up) 
        {
            image.enabled = true;
            alpha += Time.deltaTime * 3;//Increment the alpha variable
            if(alpha >= 1f) //Until the value of the alpha variable reaches 1 or more
            {
                up = false; //Setting this variable to false will trigger the fade out animation
                
                if(menu == 0) //Show the desired menu (value of the menu variable is set from the "Menus.cs" script
                {
                    GetComponent<Menus> ().ShowLevelSelectMenu();
                }else if(menu == 1) 
                {
                    GetComponent<Menus> ().HideLevelSelectMenu();
                }else if(menu == 2) 
                {
                    GetComponent<Menus> ().LoadLevel();
                }else if(menu == 3) 
                {
                    GetComponent<Menus> ().RestartLevel();
                }else if(menu == 4) 
                {
                    GetComponent<Menus> ().ExitToMainMenu();
                }else if(menu == 5) 
                {
                    GetComponent<Menus> ().NextLevel();
                }
            }
        }else 
        {
            alpha -= Time.deltaTime * 3; //Decrement the alpha variable
            if(alpha <= 0) //Until the value of the alpha variable reaches 0 or less
            {
                up = true;
                alpha = 0;
                image.color = new Color(0, 0, 0, 0);
                image.enabled = false;
                GetComponent<MenuTransitionAnimation> ().enabled = false;//Disable MenuTransitionAnimation script
            }
        }
        image.color = new Color(0, 0, 0, alpha);//Set the transparency of the image on each frame
    }
}
