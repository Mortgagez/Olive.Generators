﻿using Olive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OliveGenerator
{
    partial class Program
    {
        static bool Initialize(string[] args)
        {
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory);

            if (args.Contains("/debug"))
            {
                Console.Write("Waiting for debugger to attach...");
                while (!Debugger.IsAttached) Thread.Sleep(100);
                Console.WriteLine("Attached.");
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if (!ParametersParser.Start(args))
            {
                Helper.ShowHelp();
                return false;
            }
            return true;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var fileName = args.Name.Split(',').Select(x => x.Trim())
                .FirstOrDefault(x => x.HasValue())?.ToLower();

            if (fileName.IsEmpty()) return null;
            else fileName = fileName.ToLower();

            if (!fileName.EndsWith(".dll")) fileName += ".dll";

            var file = Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(file))
            {
                return Assembly.LoadFile(file);
            }
            else
            {
                Console.WriteLine("Not found: " + file);
                return null;
            }
        }

        public static void ShowError(Exception error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR!");
            Console.WriteLine(error.Message);
            Console.ResetColor();
            Console.WriteLine(error.GetUsefulStack());
            Console.WriteLine("Press any key to end, or rerun the command with /debug.");
            Console.ReadKey();
        }
    }
}