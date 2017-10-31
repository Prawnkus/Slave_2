using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrunt : FSM {

    public enum FSMState
    {
        None,
        Patrol,
        Chase,
        Attack,
        Dead,
    }

    public FSMState currentState;

    private float currentMoveSpeed;
    private float currentRotSpeed;
    private bool bDead;
    private int health;

    public GameObject bullet;
    

    protected override void Initialize()
    {
        currentState = FSMState.Patrol;
        currentMoveSpeed = 10;
        currentRotSpeed = 6;
        bDead = false;
        elapsedTime = 0.0f;
        shootRate = 0.2f;
        health = 100;

        bulletSpawnPoint = transform.GetChild(1);

        pointList = GameObject.FindGameObjectsWithTag("Wandarpoint");
        FindNextPoint(); //TO DO WRITE THIS

        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        if (!playerTransform){
            Debug.Log("No gameobject with tag: 'Player'");
        }
    }

    protected override void FSMUpdate()
    {
        switch (currentState) {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        elapsedTime += Time.deltaTime;

        if (health <= 0){
            currentState = FSMState.Dead;
        }
    }

    protected void UpdatePatrolState()
    {
        if (Vector3.Distance(transform.position, destPos) <= 0.2f){
            FindNextPoint();
        }

        else if (Vector3.Distance(transform.position,playerTransform.position) <= 40){
            Debug.Log("Entering chase state");
            currentState = FSMState.Chase;
        }

        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * currentRotSpeed);

        transform.Translate(Vector3.forward * Time.deltaTime * currentMoveSpeed); //Replace with navmeshagent setdestination();
    }

    protected void FindNextPoint()
    {
        int rndIndex = Random.Range(0,pointList.Length);
        float rndRadius = 10f;
        Vector3 rndPosition = Vector3.zero;
        destPos = pointList[rndIndex].transform.position + rndPosition;

        if (IsInCurrentRange(destPos)){
            rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius),0,Random.Range(-rndRadius,rndRadius));
            destPos = pointList[rndIndex].transform.position + rndPosition;
        }
    }

    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);
        if (xPos <= 50 && zPos <= 50){
            return true;
        } else{
            return false;
        }
    }

    protected void UpdateChaseState()
    {
        destPos = playerTransform.position;
        float dist = Vector3.Distance(transform.position,playerTransform.position);

        if (dist <= 30.0f){
            Debug.Log("Entering attack state");
            currentState = FSMState.Attack;
        } else if (dist >= 150.0f)
        {
            Debug.Log("Entering patrol state");
            currentState = FSMState.Patrol;
        }

        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * currentRotSpeed);

        transform.Translate(Vector3.forward * Time.deltaTime * currentMoveSpeed);
    }

    protected void UpdateAttackState()
    {
        destPos = playerTransform.position;

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist >= 30 && dist <= 150) {
            currentState = FSMState.Attack;
        } else if (dist > 150)
        {
            currentState = FSMState.Patrol;
        }

        //TO DO replace with Aim method---------
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * currentRotSpeed);
        //--------------------------------------

        FireBullets();
    }

    protected void FireBullets()
    {
        if (elapsedTime >= shootRate){
            Instantiate(bullet,bulletSpawnPoint.position,bulletSpawnPoint.rotation);
            elapsedTime = 0;
        }
    }

    protected void UpdateDeadState()
    {
        if (!bDead) {
            bDead = true;
            Explode();
        }
    }

    private void Explode()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float rndX = Random.Range(10,30);
        float rndZ = Random.Range(10, 30);

        for (int i = 0; i < 3; i++){
            rb.AddExplosionForce(10000,transform.position - new Vector3(rndX,10,rndZ), 40,10);
            rb.velocity = transform.TransformDirection(new Vector3(rndX,20,rndZ));
        }

        Destroy(gameObject,1.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet"){
            health -= collision.gameObject.GetComponent<Bullet>().damage;
        }
    }
}
