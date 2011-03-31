using System;
using System.Collections.Generic;
using Microsoft.DirectX;

namespace Steering
{
    public class SteeringBehavior
    {
        //a pointer to the owner of this instance
        Vehicle vehicle;

        //the steering force created by the combined effect of all
        //the selected behaviors
        Vector2 steeringForce;

        //these can be used to keep track of friends, pursuers, or prey
        //Vehicle targetAgent1;
        //Vehicle targetAgent2;

        //the current target
        Vector2 target;
        
        public const float steeringForceTweaker = 200.0f;

        public const float weightSeek = 1.0f * steeringForceTweaker;
        public const float weightFlee = 0.0f * steeringForceTweaker;
        public const float weightArrive = 0.0f * steeringForceTweaker;
        //public const float weightPursuit = 0.0f;

        public SteeringBehavior(Vehicle vehicle)
        {
            this.vehicle = vehicle;
            //this.targetAgent1 = null;
            //this.targetAgent2 = null;
            this.target = Vector2.Empty;
        }

        //calculates and sums the steering forces from any active behaviors
        public Vector2 Calculate()
        {
            steeringForce = CalculateWeightedSum();
            return steeringForce;
        }

        public Vector2 CalculateWeightedSum()
        {
            //Seek
            steeringForce += Seek(vehicle.World.Crosshair) * weightSeek;

            //Flee
            steeringForce += Flee(vehicle.World.Crosshair) * weightFlee;

            //Arrive
            steeringForce += Arrive(vehicle.World.Crosshair, 2) * weightArrive;

            //Pursuit
            //steeringForce += Pursuit(targetAgent1) * weightPursuit;

            Truncate(ref steeringForce, vehicle.MaxForce);
            return steeringForce;
        }
        public void Truncate(ref Vector2 v, float max)
        {
            if (v.Length() > max)
            {
                v.Normalize();
                v *= max;
            }
        }

        public Vector2 Seek(Vector2 targetPos)
        {
            Vector2 desiredVelocity = Vector2.Normalize(targetPos - vehicle.Position)
                * (float)vehicle.MaxSpeed;
            return desiredVelocity - vehicle.Velocity;
        }

        public Vector2 Flee(Vector2 targetPos)
        {
            Vector2 desiredVelocity = Vector2.Normalize(vehicle.Position - targetPos)
                * (float)vehicle.MaxSpeed;
            return desiredVelocity - vehicle.Velocity;
        }

        public Vector2 Arrive(Vector2 targetPos, int deceleration)
        {
            Vector2 toTarget = targetPos - vehicle.Position;
            float distance = toTarget.Length();
            if (distance > 0)
            {
                float speed = distance / ((float)deceleration * 0.3f);
                speed = Math.Min(speed, vehicle.MaxSpeed);
                Vector2 desiredVelocity = toTarget * speed * (1 / distance);
                return desiredVelocity - vehicle.Velocity;
            }
            return Vector2.Empty;
        }

        public Vector2 Pursuit(Vehicle evader)
        {
            Vector2 toEvader = evader.Position - vehicle.Position;
            float relativeHeading = Vector2.Dot(vehicle.Heading, evader.Heading);
            if (Vector2.Dot(toEvader, vehicle.Heading) > 0f &&
                relativeHeading < -0.95f) //acos(0.95)=18 degs
            {
                return Seek(evader.Position);
            }
            float lookAheadTime = toEvader.Length() / (vehicle.MaxSpeed + evader.Speed);
            return Seek(evader.Position + evader.Velocity * lookAheadTime);
        }
    }
}
