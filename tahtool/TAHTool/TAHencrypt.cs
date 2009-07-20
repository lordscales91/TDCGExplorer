using System;
using System.Collections.Generic;
using System.IO;

    class TAHancrypt
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string source_file = args[0];
                //encrypt to TAH archive
                if (Directory.Exists(source_file))
                {
                    //launch encrypt routine from here...
                    Encrypter encrypter = new Encrypter();
                    encrypter.encrypt_archive(source_file + ".tah", source_file);
                }
            }
        }

    }
