using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

// Skapar en klass vid namn "Spelareinput" där alla funktioner hos spelaren läggs in.
public class Spelareinput : MonoBehaviour
{
    // Refererar till en animator i Unity och döper den till animator som senare kan användas för att bestämma animationer för karaktären. I detta projektet används denna också för att styra
    // spelar rörelsen.
    public Animator animator;

    // Skapar en refererens till en textmeshpro ruta och döper den till timerfield. 
    public TMPro.TMP_Text timerfield;

    // Skapar floatvärdet jumpstrength som kan användas vid beräkningar av jumpstrength för karaktären.
    public float jumpstrength = 10f;

    // Skapar floatvärdet drag som kan användas vid beräkningar av "luftmotstånd" för karaktären. 
    public float drag = 1f;

    // Anger att ett booleskt värde "hasFinished" är falsk vid start av ett nytt spel.
    bool hasFinished = false;

    // Skapar en variabel av typen Vector2 och döper den till startPos. Den används för att skicka tillbaka speleraren till sin startpunkt vid restart och död.  
    Vector2 startPos;

    // Skapar en variabel av typen Stopwatch döper den till timer. Används för timern uppe i högra hörnet för att visa hur lång tid det nuvarande försök pågått. 
    Stopwatch timer;

    // Skapar en variabel av typen Collider2D och döper den till kollision. Används för att bestämma kollisioner med olika objekt, som t.ex vägg och mark. 
    Collider2D kollision;

    // Skapar en referens till variabeln Rigidbody2D och döper den till rb. Används för att ge "fysik" till karaktären som t.ex gravitation. 
    Rigidbody2D rb;

    // Skapar funktionen "Restart" som används för att resetta spelaren och starta om timern. Funktionen körs när man trycker på "R" och dör. 
    void Restart()
    {
        transform.position = startPos;
        rb.velocity = Vector2.zero;
        timer.Reset();
        hasFinished = false;
    }

    // Start är kallad innan första framens uppdatering 
    void Start()
    {

        // Sätter "rb" till kompontenten Rigidbody2D som tillåter en att använda oss av Rigidbody kompontenten i scriptet.
        rb = GetComponent<Rigidbody2D>();

        // Sätter "kollision" till komponenten Collider2D som tillåter en att använda oss av Collider2D komponenten i scriptet.
        kollision = GetComponent<Collider2D>();

        // Vi sätter start positionen där spelaren börjar som används av funktionen "Restart".
        startPos = transform.position;

        // Vi skapar en ny stopwatch i variabeln timer.
        timer = new Stopwatch();
    }


    // Update is called once per frame
    void Update()
    {

        // Beskriver vilka knappar av A och D som är nedtryckta för att senare räkna ut kraften i X-led. 

        float moveX = Input.GetAxisRaw(SAP.axisXinput);

        // Vi sätter moveX på animatorn för att kunna avgöra vilken animation som ska spelas upp och sedan kunna använda det värdet i "fixedUpdate" funktionen för att avgöra
        // hastigheten av spelaren.

        animator.SetFloat(SAP.moveX, moveX);

        // När spelaren rör sig i något håll i x-led sätts isMoving till true, annars är den falsk. 

        bool isMoving = !Mathf.Approximately(moveX, 0f);

        animator.SetBool(SAP.isMoving, isMoving);

        // Beskriver kraften i Y-led när knapptryck sker. 

        float moveY = Input.GetAxisRaw(SAP.axisYinput);

        animator.SetFloat(SAP.impulseY, 0f);

        // Vi kollar vart spelaren befinner sig på på y axeln och sparar det för att senare kunna avgöra om spelaren hoppar eller inte. 

        if (moveY > 0)
        {
            animator.SetFloat(SAP.impulseY, jumpstrength);
        }

        // Om knappen "R" blir nedtryckt körs funktionen "Restart"

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        // Timern startas om "isMoving" är true och "hasFinished" är falsk.

        if (isMoving && !hasFinished)
        {
            timer.Start();
        }

        // Formaterar text strängen i textfältet i vänstra hörnet för att skapa en timer.

        TimeSpan ts = timer.Elapsed;
        string elapsedTime = String.Format("{0:0}:{1:00}.{2:00}",
            ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        timerfield.text = elapsedTime;

        // Om spelaren faller under y = -25 körs funktionen "Restart".

        if (transform.position.y < -25)
        {
            Restart();
        }

    }


    void FixedUpdate()
    {

        // Hämtar kraft i x-led.  

        float forceX = animator.GetFloat(SAP.forceX);

        if (forceX != 0) rb.AddForce(new Vector2(forceX, 0));

        // Hämtar om spelaren ska hopppa.

        float impulseY = animator.GetFloat(SAP.impulseY);

        // Logik för hopp.

        if (impulseY != 0)
        {

            // Om impulseY inte är 0 och "isOnGround" är sant så är det möjligt att göra ett vanligt hopp på marken.

            if (animator.GetBool("isOnGround"))
            {
                rb.AddForce(new Vector2(0, impulseY), ForceMode2D.Impulse);
            }

            // Om "isOnWall" är sant så är det möjligt att göra ett vägghopp.

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

        // Skapar en RaycastHit2D ray som sänder en stråle mot marken under spelaren för att bedömma om det finns någon yta under spelaren. Strålens längd är halva spelarens längd + 0.1f. 

        RaycastHit2D ray = Physics2D.Raycast(transform.position + (Vector3.down * (kollision.bounds.extents.y + 0.2f)), Vector2.down, kollision.bounds.extents.y + 0.1f);

        // Om strålen av RaycastHit2D inte prickar något objekt sätts värderna "isOnGround" till falsk och "isOnWall" sätts till true. 

        if (!ray.collider)
        {


            animator.SetBool("isOnGround", false);
            animator.SetBool("isOnWall", true);

            // Om objektets x-koordinat är högre än spelarens så befinner sig väggen till höger om spelaren. Är x-koordinaten lägre än spelaren befinner den sig till vänster.

            if (point.x > transform.position.x)
            {
                animator.SetBool("wallRight", true);
            }

            else
            {
                animator.SetBool("wallRight", false);
            }
        }

        // Om strålen av RaycastHit2D prickar något objekt sätts värderna "isOnGround" till true och "isOnWall" sätts till falsk. 

        else
        {
            animator.SetBool("isOnGround", true);
            animator.SetBool("isOnWall", false);
        }

    }

    // Om spelaren hoppar från en vägg eller mark och det inte finns någon kollision mellan spelaren och något objekt sätts värderna "isOnGround" och "isOnWall" till falsk.
    void OnCollisionExit2D(Collision2D col)
    {
        animator.SetBool("isOnGround", false);
        animator.SetBool("isOnWall", false);
    }

    // En funktion som bestämmer om spelaren gått i mål. 
    void OnTriggerEnter2D(Collider2D col)
    {

        // Om spelaren går i mål sätts värdet "hasFinished" till true och timern stannar.

        if (col.tag == "Goal")
        {
            hasFinished = true;
            timer.Stop();
        }
    }
}
