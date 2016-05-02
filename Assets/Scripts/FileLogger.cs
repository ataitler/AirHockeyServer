using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using AssemblyCSharp;

public class FileLogger : ILogger
    {
        private string mFileName;
        private StreamWriter mLogFile;

        public string FileName
        {
            get { return mFileName; }
        }

        public FileLogger(string fileName)
        {
            mFileName = fileName;
        }

        public void Init()
        {
            mLogFile = new StreamWriter(mFileName, true);
        }

        public void Terminate()
        {
            mLogFile.Close();
        }


        public void ProcessLogMessage(string logMessage)
        {
            // FileLogger implements the ProcessLogMessage method by writing the incoming
            // message to a file.
            mLogFile.WriteLine(logMessage);
        }

    }

