using System;
using System.Collections.Generic;
using Microsoft.DirectX;

namespace Steering
{
    public class SteeringBehavior
    {
        [Flags]
        enum behavior_type
        {
            none = 0x00000,
            seek = 0x00002,
            flee = 0x00004,
            arrive = 0x00008,
            wander = 0x00010,
            cohesion = 0x00020,
            separation = 0x00040,
            allignment = 0x00080,
            obstacle_avoidance = 0x00100,
            wall_avoidance = 0x00200,
            follow_path = 0x00400,
            pursuit = 0x00800,
            evade = 0x01000,
            interpose = 0x02000,
            hide = 0x04000,
            flock = 0x08000,
            offset_pursuit = 0x10000,
        };

        behavior_type flags;

        //a pointer to the owner of this instance
        Vehicle vehicle;

        //the steering force created by the combined effect of all
        //the selected behaviors
        Vector2 steeringForce;

        //these can be used to keep track of friends, pursuers, or prey
        public Vehicle TargetAgent1 { get; set; }
        //Vehicle targetAgent2;

        //the current target
        Vector2 target;
        
        public const float steeringForceTweaker = 200.0f;

        public const float weightSeek = 1.0f * steeringForceTweaker;
        public const float weightFlee = 1.0f * steeringForceTweaker;
        public const float weightArrive = 1.0f * steeringForceTweaker;
        public const float weightPursuit = 1.0f * steeringForceTweaker;

        public SteeringBehavior(Vehicle vehicle)
        {
            this.vehicle = vehicle;
            this.flags = behavior_type.none;
            this.TargetAgent1 = null;
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
            if (On(behavior_type.seek))
                steeringForce += Seek(vehicle.World.Crosshair) * weightSeek;

            if (On(behavior_type.flee))
                steeringForce += Flee(vehicle.World.Crosshair) * weightFlee;

            if (On(behavior_type.arrive))
                steeringForce += Arrive(vehicle.World.Crosshair, 2) * weightArrive;

            if (On(behavior_type.pursuit))
                steeringForce += Pursuit(TargetAgent1) * weightPursuit;

            Helper2.Truncate(ref steeringForce, vehicle.MaxForce);
            return steeringForce;
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

        bool On(behavior_type bt)
        {
            return (flags & bt) == bt;
        }

        public void FleeOn()
        {
            flags |= behavior_type.flee;
        }

        public void SeekOn()
        {
            flags |= behavior_type.seek;
        }

        public void ArriveOn()
        {
            flags |= behavior_type.arrive;
        }

        public void PursuitOn()
        {
            flags |= behavior_type.pursuit;
        }

        public void FleeOff()
        {
            if (On(behavior_type.flee))
                flags ^= behavior_type.flee;
        }

        public void SeekOff()
        {
            if (On(behavior_type.seek))
                flags ^= behavior_type.seek;
        }

        public void ArriveOff()
        {
            if (On(behavior_type.arrive))
                flags ^= behavior_type.arrive;
        }

        public void PursuitOff()
        {
            if (On(behavior_type.pursuit))
                flags ^= behavior_type.pursuit;
        }
    }
}
