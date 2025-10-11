using System;
using UnityEngine;
using UnityEngine.AI;
using Player;

namespace Class3
{
    public class EnemyChaseState : StateMachineBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 20f)] private float chaseSpeed = 5f;
        [SerializeField, Range(0f, 20f)] private float loseTrackRange = 5f;
        [SerializeField, Range(0f, 1f)] private float pathfindingIntervals = 0.5f;

        [Header("Animation parameters")]
        [SerializeField] private string playerOutOfRangeTrigger = "PlayerOutOfRange";

        private NavMeshAgent agent;
        private Transform attackTarget;
        private float pathfindingTimer;


        private bool IsOutOfRange()
        {
            float sqrDistanceToTarget = (attackTarget.position - agent.transform.position).sqrMagnitude;

            return sqrDistanceToTarget >= loseTrackRange * loseTrackRange;
        }

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!agent)
                agent = animator.GetComponent<NavMeshAgent>();

            if (!attackTarget)
                attackTarget = FindAnyObjectByType<PlayerController>().transform;

            agent.speed = chaseSpeed;
            agent.destination = attackTarget.position;
        }

        public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            pathfindingTimer += Time.deltaTime;

            if (pathfindingTimer >= pathfindingIntervals)
            {
                agent.destination = attackTarget.position;
                pathfindingTimer -= pathfindingIntervals;
            }

            if (IsOutOfRange())
                animator.SetTrigger(playerOutOfRangeTrigger);
        }

        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            pathfindingTimer = 0f;
        }
    }
}