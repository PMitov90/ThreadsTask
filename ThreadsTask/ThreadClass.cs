using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using static System.Console;

namespace ThreadsTask
{
    public class ThreadClass
    {
        procEnum method;

        public ThreadClass(procEnum method)
        {
            this.method = method;
        }

        public enum procEnum
        {
            WriteToFile,
            ReadFromFile
        }

        Thread td;
        static Thread td_static;
        private static EventWaitHandle write_event = new EventWaitHandle(false, EventResetMode.AutoReset);   //event to call read thread on writing
        public void ThreadStart()
        {
            if(method == procEnum.WriteToFile)
            {
                td = new Thread(new ParameterizedThreadStart(WriteMessage));
                td.Start(td);
            }

            if(method == procEnum.ReadFromFile)
            {
                td_static = new Thread(new ThreadStart(ReadMessage));
                td_static.Start();
            }
        }


        private void WriteMessage(Object td)
        {
            Random rnd = new Random();

            for(; ; )
            {
                string message = DateTime.Now.ToString() + " Thread #" + (td as Thread).ManagedThreadId + "\r\n";
                FileWorks.WriteToFileTask(message);
                write_event.Set();
                Thread.Sleep(rnd.Next(500, 3000));
            }
        }

        private void ReadMessage()
        {
            for (; ; )
            {
                write_event.WaitOne();
                FileWorks.ReadFromFile();
                write_event.Reset();
            }
        }
    }

    public static class FileWorks
    {

        private static ReaderWriterLockSlim fileLock = new ReaderWriterLockSlim();
        public static string filePath;
        static FileStream theFile;
        private static byte[] readb = new byte[0];
        private static int readPos = 0;

        public static void CreateOutputFile()
        {
            fileLock.EnterWriteLock();

            try
            {
                using (theFile = File.Create(filePath)) { };

                theFile.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("File could not be created/opened.", ex);
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        public static void WriteToFileTask(Object input)
        {
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] binput = ue.GetBytes(input as string);

            fileLock.EnterWriteLock();

            try
            {
                using (theFile = File.OpenWrite(filePath))
                {
                    theFile.Seek(0, SeekOrigin.End);
                    theFile.Write(binput);

                    theFile.Dispose();
                }
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
            
        }

        public static void ReadFromFile()
        {
            fileLock.EnterReadLock();

            try
            {
                using(theFile = File.OpenRead(filePath))
                {
                    int length = (int)theFile.Length;
                    byte[] barr = new byte[length - readPos];

                    theFile.Seek(readPos, SeekOrigin.Begin);
                    readPos += theFile.Read(barr, 0, length - readPos);
                    Write(Encoding.Unicode.GetString(barr));

                    theFile.Dispose();
                }
            }
            finally
            {
                fileLock.ExitReadLock();
            }
        }
    }
}
