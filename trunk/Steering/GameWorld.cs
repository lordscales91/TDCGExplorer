using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;

namespace Steering
{
    public class GameWorld
    {
        List<Vehicle> vehicles = new List<Vehicle>();

        //the position of the crosshair
        Vector2 crosshair;

        public Vector2 Crosshair { get { return crosshair; } }

        Random random = new Random();

        public GameWorld()
        {
            Vector2 spawn_pos0 = new Vector2(
                (float)(200.0 * RandomClamped() + 200.0),
                (float)(200.0 * RandomClamped() + 200.0));
            Vehicle vehicle0 = new Vehicle(this, spawn_pos0, (float)(random.NextDouble() * 2.0 * Math.PI), Vector2.Empty, 1);
            vehicle0.Steering.SeekOn();
            vehicles.Add(vehicle0);

            crosshair = new Vector2(200, 200);
        }

        double RandomClamped()
        {
            return random.NextDouble() - random.NextDouble();
        }

        public void Update(double time_elapsed)
        {
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Update(time_elapsed);
            }
        }

        public void Render(Graphics graphics)
        {
            graphics.Clear(Color.White);
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Render(graphics);
            }
            Crosshair_Render(graphics);
        }

        void Crosshair_Render(Graphics graphics)
        {
            graphics.DrawEllipse(Pens.Red, crosshair.X - 4, crosshair.Y - 4, 8, 8);
            graphics.DrawLine(Pens.Red, crosshair.X, crosshair.Y - 8, crosshair.X, crosshair.Y + 8);
            graphics.DrawLine(Pens.Red, crosshair.X - 8, crosshair.Y, crosshair.X + 8, crosshair.Y);
        }
    }
}
