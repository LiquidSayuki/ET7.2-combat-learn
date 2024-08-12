using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class AnimatorTest : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float IKWeight;

        public float ForwardSpeed;
        public float BackwardSpeed;

        private float currentSpeed;
        private float targetSpeed;
        
        private Animator animator;
        private Rigidbody rigidBody;
        
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int MotionSpeed = Animator.StringToHash("MotionSpeed");

        void Start()
        {
            this.animator = this.GetComponent<Animator>();
            this.rigidBody = this.GetComponent<Rigidbody>();
            
            Debug.Log($"Human Scale{this.animator.humanScale}");
            this.animator.SetFloat(MotionSpeed,  1/this.animator.humanScale); ;
        }
        
        void Update()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            if (v > 0f)
            {
                this.targetSpeed = v * ForwardSpeed;
            }
            else if (v < 0f)
            {
                this.targetSpeed = v * BackwardSpeed;
            }
            else
            {
                this.targetSpeed = 0f;
            }
            
            this.animator.SetFloat(Direction, h);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            // Gizmos.DrawSphere(this.animator.rootPosition, 0.5f);
        }

        /// <summary>
        /// 当IK发生运算时的Event回调
        /// </summary>
        private void OnAnimatorIK(int layerIndex)
        {
            Vector3 currentPos = this.transform.position;
            // this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, this.IKWeight);
            // this.animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(currentPos.x, currentPos.y, currentPos.z));
        }
        
        /// <summary>
        /// Event 剥夺Root motion对移动的控制权
        /// </summary>
        private void OnAnimatorMove()
        {
            this.Move();
        }

        private void Move()
        {
            this.currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.2f);
            this.animator.SetFloat(Speed, this.currentSpeed);
            
            this.rigidBody.velocity = new Vector3(this.animator.velocity.x, this.rigidBody.velocity.y, this.animator.velocity.z);
        }
    }
}
