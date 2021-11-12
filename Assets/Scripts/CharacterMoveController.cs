using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    private Rigidbody2D rb;

    [Header("Jump")]
    public float jumpAccel;
    private bool isJumping;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;
    
    private bool isOnGround;

    private Animator anim;

    private CharacterSoundController sound;

    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;

    [Header("GameOver")]
    public float fallPositionY;
    public GameObject gameOverScreen;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isOnGround)
            {
                isJumping = true;
                sound.PlayJump();
            }
        }


        //Update animation based on isOnGround
        anim.SetBool("isOnGround", isOnGround);

        //Scoring
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if(scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        //When game over
        if(transform.position.y < fallPositionY)
        {
            GameOver();
        }

    }

    private void FixedUpdate()
    {
        //Raycast to detect ground
         RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);

            if (hit)
            {
                if (!isOnGround && rb.velocity.y <= 0) isOnGround = true;
            }
            else
                isOnGround = false;

            Vector2 velocityVector = rb.velocity;
            velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

            if (isJumping)
            {
                velocityVector.y += jumpAccel;
                isJumping = false;
            }

            rb.velocity = velocityVector;
    }

    private void GameOver()
    {
        score.FinishScoring();

        gameCamera.enabled = false;

        gameOverScreen.SetActive(true);

        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }


}
