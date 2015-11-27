﻿using UnityEngine;

namespace Assets.Utils
{
    public class AirplaneController : MonoBehaviour
    {
        public Vector3 TargetPosition;
        public float Speed = 2f;
        public float RotateSpeed = 40f;
        
        private void Update()
        {
            // Get the direction to rotate to
            var directionVector = TargetPosition - transform.position;
            var targetDirection = Mathf.Rad2Deg * Mathf.Atan2(directionVector.x, -directionVector.y) + 180;

            // Rotate in the direction of the target
            var currentDirection = transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.Euler(
                0, 0,
                RotateToWithLimit(currentDirection, targetDirection, RotateSpeed * Time.deltaTime));

            // Move in the right direction
            var direction = transform.rotation * Vector3.up;
            transform.position += direction * Time.deltaTime * Speed;
        }

        private static float RotateToWithLimit(float source, float target, float limit)
        {
            // Normalize the input
            source = DegMod(source);
            target = DegMod(target);

            // Calculate our actual difference
            var posDiff = DegMod(target - source);
            var negDiff = -DegMod(360 - posDiff);

            // Get the smallest diff
            var diff = Mathf.Abs(posDiff) < Mathf.Abs(negDiff) ? posDiff : negDiff;

            // Limit our diff so we only move at a certain speed
            var limitedDiff = diff;
            if (limitedDiff > limit)
            {
                limitedDiff = limit;
            }
            else if (limitedDiff < -limit)
            {
                limitedDiff = -limit;
            }

            return source + limitedDiff;
        }

        private static float DegMod(float value)
        {
            return (value % 360f + 360f) % 360f;
        }
    }
}