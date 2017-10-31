using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : FSMState {

    public AttackState(Transform[] wp)
    {
        waypoints = wp;
        stateID = FSMStateID.Attacking;
        curRotSpeed = 6.0f;
        curSpeed = 8.0f;

        //find next Waypoint position
        FindNextPoint();
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Check the distance with the player tank
        float dist = Vector3.Distance(npc.position, player.position);
        if (dist >= 20.0f && dist < 40.0f)
        {
            //Rotate to the target point (make this into function in FSMState or NPCController?)
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
            targetRotation = Quaternion.Euler(0,targetRotation.eulerAngles.y,0);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //Go Forward
            npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            Debug.Log("Switch to Chase State");
            npc.GetComponent<NPCController>().SetTransition(Transition.SawPlayer);
        }
        //Transition to patrol is the tank become too far
        else if (dist >= 300.0f)
        {
            Debug.Log("Switch to Patrol State");
            npc.GetComponent<NPCController>().SetTransition(Transition.LostPlayer);
        }
    }


    public override void Act(Transform player, Transform npc)
    {
        //Set the target position as the player position
        destPos = player.position;

        //TO DO replace with Aim method---------
        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        //--------------------------------------

        npc.GetComponent<NPCController>().FireBullets();
    }

}
