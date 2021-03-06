﻿using System;
using Assets.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Enemy
{
    [RequireComponent(typeof (AirplaneController))]
    public class EnemyBehavior : MonoBehaviour
    {
        private AirplaneController _controller;
        private GameObject _target;

        /// The direction we're currently moving in euler angles (when not chasing a _target)
        private float _currentAngle;

        private float _rotateTimer;
        private bool _chaseTarget;
        private bool _isChasingTarget;

        public float ChaseDistance = 3;

        // Use this for initialization
        private void Start()
        {
            _controller = gameObject.GetComponent<AirplaneController>();

            // Find target
            _target = GameObject.Find("Player");

            LookNearTarget();

            _currentAngle = transform.rotation.eulerAngles.z;

            if (Random.Range(0, 3) == 0)
                _chaseTarget = false;

            // Set random rotate timer
            _rotateTimer = Random.Range(1F, 4F);
        }

        // Update is called once per frame
        private void Update()
        {
            // Calculate distance to target
            var distance = _target != null ? Vector3.Distance(transform.position, _target.transform.position) : 9999f;

            if (Mathf.Abs(distance) >= ChaseDistance) // Too far away to chase
            {
                // TODO: Change to use the airplane controller instead of disabling it and taking over
                _controller.enabled = false;

                _isChasingTarget = false;
                _currentAngle = transform.rotation.eulerAngles.z;

                _rotateTimer -= Time.deltaTime;

                if (_rotateTimer <= 0)
                {
                    // Randomly rotate
                    _currentAngle += -45 + Random.Range(0, 90);

                    // Set new random rotate timer
                    _rotateTimer = Random.Range(1F, 4F);
                }

                if (Math.Abs(transform.rotation.eulerAngles.z - _currentAngle) > 2F)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                        Quaternion.Euler(0, 0, _currentAngle + 90), 10F*Time.deltaTime);
                }

                Move();
            }
            else // Chase target
            {
                _controller.enabled = true;
                _controller.TargetPosition = _target.transform.position;
            }
        }

        private void LookAtTarget()
        {
            if (_target == null)
            {
                return;
            }

            var difference = _target.transform.position - transform.position;
            difference.Normalize();

            var rotation = Mathf.Atan2(difference.y, difference.x)*Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0F, 0F, rotation - 90);
        }

        private void LookNearTarget()
        {
            LookAtTarget();
            // Look near _target
            transform.Rotate(0, 0, -20 + Random.Range(0, 40));
        }

        private void Move()
        {
            if (_isChasingTarget)
                transform.position = Vector3.MoveTowards(
                    transform.position, _target.transform.position,
                    _controller.Speed*Time.deltaTime);
            else
            {
                transform.position += transform.up*_controller.Speed*Time.deltaTime;
            }
        }
    }
}