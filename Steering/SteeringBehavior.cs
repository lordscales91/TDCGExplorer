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
        public const float weightEvade = 1.0f * steeringForceTweaker;
        public const float weightWander = 1.0f * steeringForceTweaker;

        Random random = new Random();

        double RandomClamped()
        {
            return random.NextDouble() - random.NextDouble();
        }

        public SteeringBehavior(Vehicle vehicle)
        {
            this.vehicle = vehicle;
            this.flags = behavior_type.none;
            this.TargetAgent1 = null;
            //this.targetAgent2 = null;
            this.target = Vector2.Empty;

            //stuff for the wander behavior
            double theta = random.NextDouble() * 2.0 * Math.PI;

            //create a vector to a target position on the wander circle
            wanderTarget = new Vector2((float)(wanderRadius * Math.Cos(theta)), 
                (float)(wanderRadius * Math.Sin(theta)));
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

            if (On(behavior_type.evade))
                steeringForce += Evade(TargetAgent1) * weightEvade;

            if (On(behavior_type.wander))
                steeringForce += Wander() * weightWander;

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

        public Vector2 Evade(Vehicle pursuer)
        {
            Vector2 toPursuer = pursuer.Position - vehicle.Position;
            float lookAheadTime = toPursuer.Length() / (vehicle.MaxSpeed + pursuer.Speed);
            return Flee(pursuer.Position + pursuer.Velocity * lookAheadTime);
        }

        const float wanderRadius = 1.2f;
        const float wanderDistance = 2.0f;
        const double wanderJitter = 80.0;
        Vector2 wanderTarget;

        public Vector2 Wander()
        {
            //this behavior is dependent on the update rate, so this line must
            //be included when using time independent framerate.
            double jitterThisTimeSlice = wanderJitter * vehicle.TimeElapsed;

            //first, add a small random vector to the target's position
            wanderTarget += new Vector2((float)(RandomClamped() * jitterThisTimeSlice),
                (float)(RandomClamped() * jitterThisTimeSlice));

            //reproject this new vector back on to a unit circle
            wanderTarget.Normalize();

            //increase the length of the vector to the same as the radius
            //of the wander circle
            wanderTarget *= wanderRadius;

            //move the target into a position WanderDist in front of the agent
            Vector2 targetLocal = wanderTarget + new Vector2(wanderDistance, 0);

            //project the target into world space
            Vector2 targetWorld = PointToWorldSpace(targetLocal, vehicle.Heading, vehicle.Side, vehicle.Position);

            //and steer towards it
            return targetWorld - vehicle.Position;
        }

        Vector2 PointToWorldSpace(Vector2 point, Vector2 heading, Vector2 side, Vector2 pos)
        {
            Matrix m = Matrix.Identity;
            m.M11 = heading.X; m.M12 = heading.Y;
            m.M21 = side.X; m.M22 = side.Y;
            m.M41 = pos.X;
            m.M42 = pos.Y;

            return Vector2.TransformCoordinate(point, m);
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

        public void EvadeOn()
        {
            flags |= behavior_type.evade;
        }

        public void WanderOn()
        {
            flags |= behavior_type.wander;
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

        public void EvadeOff()
        {
            if (On(behavior_type.evade))
                flags ^= behavior_type.evade;
        }

        public void WanderOff()
        {
            if (On(behavior_type.wander))
                flags ^= behavior_type.wander;
        }
    }
}
