using System;
using System.Collections.Generic;

namespace ThreadsTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            FileWorks.filePath = @"D:\Projects Interview\Cognyte\external files\ThreadsMsg\ThreadsMsg.txt";
            FileWorks.CreateOutputFile();
            ThreadClass td = new ThreadClass(ThreadClass.procEnum.WriteToFile);
            ThreadClass td2 = new ThreadClass(ThreadClass.procEnum.WriteToFile);
            ThreadClass td3 = new ThreadClass(ThreadClass.procEnum.ReadFromFile); ;

            td.ThreadStart();
            td2.ThreadStart();
            td3.ThreadStart();
        }
    }
}