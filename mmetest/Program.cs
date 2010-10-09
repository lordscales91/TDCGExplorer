using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace MMETest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: MMETest <effect file>");
                return;
            }

            string effect_file = args[0];

            if (File.Exists(effect_file))
            {
                Program program = new Program();
                Control control = new Form();
                program.InitializeGraphics(control);
                Effect effect = program.LoadEffectFromFile(effect_file);
                program.DumpEffect(effect);
            }
        }

        GraphicsDevice device = null;

        void InitializeGraphics(Control control)
        {
            PresentationParameters pp = new PresentationParameters();
            device = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, control.Handle, pp);
        }

        private Effect LoadEffectFromFile(string source_file)
        {
            CompiledEffect compiled_effect = Effect.CompileEffectFromFile(source_file, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            return new Effect(device, compiled_effect.GetEffectCode(), CompilerOptions.None, null);
        }

        private void DumpEffect(Effect effect)
        {
            Console.WriteLine("Parameters:");
            foreach (EffectParameter parameter in effect.Parameters)
            {
                Console.WriteLine(parameter.Name);
                Console.WriteLine("\tAnnotations:");
                foreach (EffectAnnotation annotation in parameter.Annotations)
                {
                    Console.WriteLine("\t" + annotation.Name);
                }
            }
            Console.WriteLine("Techniques:");
            foreach (EffectTechnique technique in effect.Techniques)
            {
                Console.WriteLine(technique.Name);
                Console.WriteLine("\tAnnotations:");
                foreach (EffectAnnotation annotation in technique.Annotations)
                {
                    Console.WriteLine("\t" + annotation.Name);
                }
                Console.WriteLine("\tPasses:");
                foreach (EffectPass pass in technique.Passes)
                {
                    Console.WriteLine("\t" + pass.Name);
                }
            }
        }
    }
}
