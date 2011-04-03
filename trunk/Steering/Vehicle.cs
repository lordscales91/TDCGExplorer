using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;

namespace Steering
{
    public class Vehicle
    {
        //its location in the environment
        Vector2 position;

        Vector2 velocity;

        //a normalized vector pointing in the direction the entity is heading. 
        Vector2 heading;

        //a vector perpendicular to the heading vector
        Vector2 side;

        //a pointer to the world data. So a vehicle can access any obstacle,
        //path, wall or agent data
        GameWorld world;

        //the steering behavior class
        SteeringBehavior steering;

        float mass;

        //the maximum speed this entity may travel at.
        float maxSpeed;

        //the maximum force this entity can produce to power itself 
        //(think rockets and thrust)
        float maxForce;

        Vector2[] vertices = new Vector2[3] { new Vector2(-1.0f, 0.6f), new Vector2(1.0f, 0.0f), new Vector2(-1.0f, -0.6f) };

        public Vehicle(GameWorld world, Vector2 position, float rotation, Vector2 velocity, float mass)
        {
            this.world = world;
            this.position = position;
            this.heading = new Vector2((float)+Math.Sin(rotation), (float)-Math.Cos(rotation));
            this.side = Helper2.Perp(this.heading);
            steering = new SteeringBehavior(this);
            this.mass = mass;
            this.maxSpeed = 150.0f;
            this.maxForce = 2.0f * SteeringBehavior.steeringForceTweaker;
        }

        public PointF[] WorldTransform(Vector2[] points, Vector2 pos, Vector2 scale)
        {
            PointF[] transformed = new PointF[points.Length];
            
            Matrix m = Matrix.Identity;
            m.M11 = scale.X * heading.X; m.M12 = scale.X * heading.Y;
            m.M21 = scale.Y * side.X; m.M22 = scale.Y * side.Y;
            m.M41 = pos.X;
            m.M42 = pos.Y;

            for (int i = 0; i < points.Length; ++i)
            {
                Vector2 v = Vector2.TransformCoordinate(points[i], m);
                transformed[i] = new PointF(v.X, v.Y);
            }
            
            return transformed;
        }

        public double TimeElapsed;

        public void Update(double time_elapsed)
        {
            this.TimeElapsed = time_elapsed;

            //calculate the combined force from each steering behavior in the 
            //vehicle's list
            Vector2 steeringForce = steering.Calculate();

            //Acceleration = Force/Mass
            Vector2 acceleration = steeringForce * (float)(1 / mass);

            //update velocity
            velocity += acceleration * (float)time_elapsed;

            //make sure vehicle does not exceed maximum velocity
            Helper2.Truncate(ref velocity, maxSpeed);

            //update the position
            position += velocity * (float)time_elapsed;

            //update the heading if the vehicle has a non zero velocity
            if (velocity.LengthSq() > 0.00000001)
            {
                heading = Vector2.Normalize(velocity);
                side = Helper2.Perp(heading);
            }

            //treat the screen as a toroid
            Helper2.WrapRound(ref position, world.cxClient, world.cyClient);
        }

        public void Render(Graphics graphics)
        {
            Vector2 scale = new Vector2(20, 20);
            graphics.DrawPolygon(Pens.Blue, WorldTransform(vertices, position, scale));
        }

        public Vector2 Position { get { return position; } set { position = value; } }
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public Vector2 Heading { get { return heading; } }
        public Vector2 Side { get { return side; } }
        public float Mass { get { return mass; } }
        public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }
        public float MaxForce { get { return maxForce; } set { maxForce = value; } }
        public float Speed { get { return velocity.Length(); } }
        public SteeringBehavior Steering { get { return steering; } }
        public GameWorld World { get { return world; } }
    }
}
