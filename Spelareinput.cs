using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

// Skapar en klass vid namn "Spelareinput" där alla funktioner hos spelaren läggs in.
public class Spelareinput : MonoBehaviour
{
    // Skapar en animator och döper den till animator som senare kan användas för att bestämma animationer för karaktären.
    public Animator animator;

    // Skapar en text ruta och döper den till timerfield. 
    public TMPro.TMP_Text timerfield;

    // Skapar floatvärdet jumpstrength som kan användas vid beräkningar av jumpstrength för karaktären.
    public float jumpstrength = 10f;

    // Skapar float värdet drag som kan användas vid beräkningar av drag för karaktären. 
    public float drag = 1f;

    // Anger att boolen "hasFinished" är falsk vid start av ett nytt spel.
    public bool hasFinished = false;

    // Lägger till en Vector2 och döper den till startPos.
    Vector2 startPos;

    // Lägger till Stopwatch och döper den till timer. 
    Stopwatch timer;

    // Lägger till Collider2D och döper den till kollision.
    Collider2D kollision;

    // Lägger till Rigidbody2D och döper den till rb. 
    private Rigidbody2D rb;

    // Skapar funktionen "Restart" som används för att resetta spelaren och starta om timern. 
    void Restart()
    {
        transform.position = startPos;
        rb.velocity = Vector2.zero;
        timer.Reset();
        hasFinished = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        // Skapar funktionen "rb" som beskriver rigidbodyns kollision
        rb = GetComponent<Rigidbody2D>();

        // Skapar en funktion "kollision" som används för att bestämma kollision mot spelaren. 
        kollision = GetComponent<Collider2D>();
        
        // Lägger till en start position som används av funktionen "Restart"
        startPos = transform.position;
        
        // Skapar en funktion vid namn "timer" som skapar en stoppwatch
        timer = new Stopwatch();
    }


    // Update is called once per frame
    void Update()
    {

        // Beskriver kraften i X-led när knapptryck sker

        float moveX = Input.GetAxisRaw(SAP.axisXinput);

        animator.SetFloat(SAP.moveX, moveX);

        // När spelaren rör sig i något håll i x-led sätts isMoving till true, annars är den falsk. 

        bool isMoving = !Mathf.Approximately(moveX, 0f);

        animator.SetBool(SAP.isMoving, isMoving);

        // Beskriver kraften i Y led när knapptryck sker. 

        float moveY = Input.GetAxisRaw(SAP.axisYinput);

        animator.SetFloat(SAP.impulseY, 0f);

        // Beskriver hoppstyrkan

        if (moveY > 0)
        {
            animator.SetFloat(SAP.impulseY, jumpstrength);
        }

        // Om knappen "R" blir nedtryckt körs funktionen "Restart"

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (isMoving && !hasFinished)
        {
            timer.Start();
        }

        // Lägger till en timer

        TimeSpan ts = timer.Elapsed;
        string elapsedTime = String.Format("{0:0}:{1:00}.{2:00}",
            ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        timerfield.text = elapsedTime;

        // Om spelaren faller under y = -25 körs funktionen "Restart"

        if (transform.position.y < -25)
        {
            Restart();
        }

    }


    void FixedUpdate()
    {

        // Ger kraft i x-led 

        float forceX = animator.GetFloat(SAP.forceX);

        if (forceX != 0) rb.AddForce(new Vector2(forceX, 0));


        float impulseY = animator.GetFloat(SAP.impulseY);

        // Om impulseY inte är 0 och "isOnGround" är sant så är det möjligt att göra ett vanligt hopp på marken.

        if (impulseY != 0)
        {
            if (animator.GetBool("isOnGround"))
            {
                rb.AddForce(new Vector2(0, impulseY), ForceMode2D.Impulse);
            }

        // Om impulseY inte är 0 och "isOnWall" är sant så är det möjligt att göra ett vanligt hopp på marken.
            else if (animator.GetBool("isOnWall"))
            {
                Vector2 direction = animator.GetBool("wallRight") ? Vector2.left : Vector2.right;
                rb.AddForce(new Vector2(0, impulseY * 1.5f) + direction * drag * (jumpstrength / 2), ForceMode2D.Impulse);
            }
        }

        // Tillsätter drag på spelaren i x-led.

        rb.velocity = new Vector2(rb.velocity.x - 0.0001f * drag * Mathf.Pow(rb.velocity.x, 3), rb.velocity.y);


    }

    // Script som ser om man är på en vägg eller mark, och sedan anger sant eller falskt på de angivna variablerna.
    void OnCollisionEnter2D(Collision2D col)
    {

        Vector2 point = col.GetContact(0).point;

        RaycastHit2D ray = Physics2D.Raycast(transform.position + (Vector3.down * (kollision.bounds.extents.y + 0.2f)), Vector2.down, kollision.bounds.extents.y + 0.1f);


        if (!ray.collider)
        {


            animator.SetBool("isOnGround", false);
            animator.SetBool("isOnWall", true);


            if (point.x > transform.position.x)
            {
                animator.SetBool("wallRight", true);
            }

            else
            {
                animator.SetBool("wallRight", false);
            }
        }

        else
        {
            animator.SetBool("isOnGround", true);
            animator.SetBool("isOnWall", false);
        }

    }

    void OnCollisionExit2D(Collision2D col)
    {
        animator.SetBool("isOnGround", false);
        animator.SetBool("isOnWall", false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        UnityEngine.Debug.Log(col.tag);

        if (col.tag == "Goal")
        {
            hasFinished = true;
            timer.Stop();
        }
    }
}
