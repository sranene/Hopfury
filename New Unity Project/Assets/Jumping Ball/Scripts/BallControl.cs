using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour {
    
    [SerializeField]
    private Rigidbody2D rb = null;
    [SerializeField]
    private CircleCollider2D circleCollider = null;
    [SerializeField]
    private ParticleSystem jumpParticle = null;//Simple particle effect when player jumps
    private bool moveRight = true;//This will determine whether the ball will move left or right.
    private bool canJump = true;//Ball needs to be grounded to be able to jump
    private bool canCollide = true;//If the value of this variable is false, the ball will be able to pass through the platform. This is used for passing through the platform above when ball jumps
    private bool jumpInitiated = false;//In case player tries to jump before the ball lands on the ground this variable will change value to true
    private float jumpTimer = 0.1f;//If jumpInitiated variable is true the ball will jump automatically if it lands on the ground in next 0.1 seconds. 

    private AudioSource jumpSound;
    private AudioSource hitSound;

    void Start() 
    {
         moveRight = Random.Range(0,2) == 0 ? true : false;//At the start of each level this will determine whether ball will start moving left or right
         jumpSound = GameObject.Find("JumpSound").GetComponent<AudioSource>();
         hitSound = GameObject.Find("HitSound").GetComponent<AudioSource>();
    }

    void Update()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if(moveRight) 
        {
            rb.position = new Vector2 (rb.position.x + 5f * Time.deltaTime, rb.position.y);//This will move the ball to the right
        }else {
            rb.position = new Vector2 (rb.position.x - 5f * Time.deltaTime, rb.position.y);//This will move the ball to the left
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(!canJump) 
            {
                jumpInitiated = true;//In case the ball is in the air and can't jump, jumpInitiated will be set to true and that will cause the ball to jump automatically if it hits the ground in the next 0.1 seconds
                return;
            }
            Jump();
        }

        if(jumpInitiated) 
        {
            jumpTimer -= Time.deltaTime;
            if(jumpTimer <= 0) //If jumpInitiated variable is true the ball will jump automatically if it lands on the ground in next 0.1 seconds (0.1 seconds is the value of the jumpTimer variable). 
            {
                jumpInitiated = false;
                jumpTimer = 0.1f;//Reset jumpTimer to 0.1 seconds
                if(!canJump) return;
                Jump();
            }
        }

        if(rb.velocity.y < 0 && canCollide) 
        {
            circleCollider.isTrigger = false;//When the trigger is set to false, the ball will not pass through the platforms anymore and it will be able to land
        }
    }

    private void Jump() 
    {
        rb.velocity = Vector2.zero;
        canJump = false;
        circleCollider.isTrigger = true;//This is used for passing through the platform above when ball jumps
        rb.AddForce(new Vector2(0, 2600));//This will cause the ball to jump
        jumpSound.Play();
        jumpParticle.Play();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.name == "Right" || col.gameObject.name == "Left") //This is used for changing the balls directions when it hits left or right barrier of the platform
        {
            moveRight = moveRight ? false : true;
            hitSound.Play();
            Vector3 contactsPos = col.contacts[0].point;
            GameObject hitParticle = Instantiate(Resources.Load("HitParticle", typeof(GameObject))) as GameObject;
            hitParticle.transform.position = contactsPos;
            if(PlayerPrefs.GetInt("Vibration") == 1) 
            {
                Handheld.Vibrate();//This will cause the device to vibrate if vibration option is checked in settings menu
            }
        }
        else if(col.gameObject.name == "Bottom")//When the ball hits the bottom of the platform
        {
            canJump = true;
            jumpParticle.Stop();

            if(transform.position.y > Vars.cameraMaxYPos)
                Vars.cameraMaxYPos = transform.position.y;
        }
        else if(col.gameObject.name == "BottomCollider")//When the ball falls from the platform and hits the bottom collider the game is over
        {
            GameObject.Find("GameManager").GetComponent<Menus> ().GameOver();
            Destroy(this.gameObject);
        }
        else if(col.gameObject.name == "FinishBottom")//When the ball jumps on the finish platform
        {
            GameObject.Find("GameManager").GetComponent<Menus> ().LevelComplete();
            GetComponent<BallControl> ().enabled = false;
            GameObject.Find("LevelCompleteSound").GetComponent<AudioSource> ().Play();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.name == "Bottom") {
            Invoke("disableJumping", 0.1f);//Disable jumping 0.1 seconds after the ball lefts the bottom of the platform
        }
    }

    private void disableJumping() {
        canJump = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        canCollide = false;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        canCollide = true;
    }
}
