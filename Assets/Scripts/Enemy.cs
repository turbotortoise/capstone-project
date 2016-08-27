using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ui=UnityEngine.UI;


public class Enemy : MonoBehaviour {

    bool isHit, hasTransformed, isTransforming, isAttacking;

    //for moving
    float
        runRadius = 12f,
        walkRadius = 5f,
        sRadius = 2f,
        runSpeed = 10f,
        walkSpeed = 2.5f,
        curSpeed = 0f,
        accelRate = 0.1f,
    //for health/color
        health = 1f,
        regenSpeed,
        transformSpeed = 0.1f;
    Color dormantColor;

#pragma warning disable 0108
    Rigidbody rigidbody;
#pragma warning restore 0108
    ui::Text displayText;

    Vector3 origPos;
    float dormantPerc;
    //for player
    Player player;
    float playerDistance; //distance player currently is
    float playerPower; //power of player's attacks
    //for attacking
    public bool isStable = true;
    public Attack attackPrefab;
    float
        waitTime = 0.75f, //uses last attack wait time (recharge)
        prevAttTime,
        addTime, //if the enemy doesn't feel like attacking
        laTime = 2.8f, //long ranged attack time
        saTime = 1f, //short ranged attack time
        srPower = 0.2f,
        lrPower = 0.4f;
    public float attPower = 0f;
    Vector3 tempPosition;
    Queue<Weapon> attackQueue = new Queue<Weapon>();
    bool newCivilian = true;
    string
        firstText = "Hello there.",
        secondText = "Hello again.";

    void Awake() {
        var canvas = GameObject.FindWithTag("Canvas");
        displayText = canvas.GetComponentInChildren<ui::Text>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start() {
        origPos = transform.position;
        tag = "Enemy";
        player = GameObject.Find("Player").GetComponent<Player>();
        displayText.text = "";
        StartCoroutine(Attacking());
    }


    IEnumerator Attacking() {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            Attack();
        }
    }


    public void AppyDamage(float damage) {
        health -= damage;
        if (health<0) Kill();
    }

    public void Kill() { Destroy(gameObject); }


    void ApproachPlayer() {
        if (isStable) return;
        if (playerDistance <= sRadius) {
            //physical attack, enemy does not move
            if (curSpeed > 0f) curSpeed -= accelRate;
            else curSpeed = 0f;
        }
        else if (playerDistance <= walkRadius) {
            //enemy approaches player slowly
            if (curSpeed > walkSpeed) curSpeed -= accelRate;
            else curSpeed = walkSpeed;
        }
        else if (playerDistance <= runRadius) {
            //gotta run man
            if (curSpeed < runSpeed) curSpeed += accelRate;
            else curSpeed = runSpeed;
        }
        if ((curSpeed != 0) && (!hasTransformed)) {
            hasTransformed = true;
            isTransforming = true;
        }
        //for rotating
        //print("Distance: " + playerDistance + "\nSpeed: " + curSpeed);
        transform.rotation = Quaternion.LookRotation(
            player.transform.position - transform.position);
        rigidbody.MovePosition(
            transform.position+transform.forward*
            (curSpeed * Time.deltaTime));
    }

    void Move() {
        //changes color of civilian to show health
        if (!isStable) {
            if (isHit) {
                isHit = false;
                //do isHit animation yay
                //maybe also a delay?
            }
            if (isTransforming) {
                health -= transformSpeed * Time.deltaTime;
                if (health <= 0f) {
                    isTransforming = false;
                }
            }
            if (health >= 1f) {
                isTransforming = false;
                isStable = true;
                //do transformation animation
            }

            GetComponent<Renderer>().material.color =
                new Color(health,health,health);
        }
        else {
            if (isHit) {
                if (dormantPerc < 1) {
                    dormantPerc += transformSpeed * Time.deltaTime;
                    var tempColor = new Color (1f - (dormantColor.r * dormantPerc),
                        1f - (dormantColor.g * dormantPerc),
                        1f - (dormantColor.b * dormantPerc));
                    GetComponent<Renderer>().material.color = tempColor;
                }
            }
        }
    }

    void Attack() {
        if (playerDistance <= sRadius) { }
        if (playerDistance <= walkRadius) {
            var newAttack = Instantiate<Attack>(attackPrefab);
            newAttack.tag = "Attack";
            //attackQueue.Add(newAttack);
            if (Random.value>0.5) {
                newAttack.name = "Short";
                newAttack.GetComponent<Attack>().power = srPower;
            } else {
                newAttack.name = "Long";
                newAttack.GetComponent<Attack>().power = lrPower;
            }

            tempPosition = player.transform.position; //finds player
            print("attacking");
            //var newAttack = attackQueue[0];
            newAttack.transform.position = transform.position+transform.forward;
            newAttack.GetComponent<Rigidbody>().AddForce(
                (transform.forward)*20, ForceMode.Impulse);
            isAttacking = false;
            //attackQueue.Remove(newAttack);
        }
    }


    void Speak() {
        if (newCivilian) {
            displayText.text = firstText;
            newCivilian = false;
        } else displayText.text = secondText;
    }


    void OnTriggerEnter(Collider other) {
        if (!isTransforming || !other.attachedRigidbody) {
            //can't be hit during initial transformation (need to think about more)
            var note = other.attachedRigidbody.GetComponent<NoteAttack>();

            if (!note) return;
            //if the player attacked take damage
            health -= note.power;
            isHit = true;
        }
    }


    void Update() {

        //always check where the player is
        playerDistance = Vector3.Distance(
            player.transform.position, transform.position);

        //when civilian is unstable, attack the player
        if (!isHit && !isAttacking) ApproachPlayer();
        if (isTransforming || isHit) Move();

        //civilian is stable, player can talk to them
        if (isStable) {
            if (isHit) Move();
            if (playerDistance <= 5f) {
                //rotate to face player
                transform.rotation = Quaternion.LookRotation(
                    player.transform.position-transform.position);
                player.canTalk = true;
                if (Input.GetKey("space")) Speak(); //player won't jump
            } else player.canTalk = false;
        }
    }
}


