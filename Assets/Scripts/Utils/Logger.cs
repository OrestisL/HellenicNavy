using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : GenericSingleton<Logger>
{
    internal class LogMessage : Command
    {
        private string message;
        private string path;

        public LogMessage(string message, string path)
        {
            this.message = message;
            this.path = path;
        }

        bool ready;
        public override bool isFinished => ready;

        public override void Execute()
        {
            ready = false;

            using (StreamWriter sw = new StreamWriter(path, true))//true to append to file
            {
                sw.WriteLine(string.Format("{0}\n", message));
                //Debug.Log(message);
            }

            ready = true;
        }
    }


    private Queue<LogMessage> messageQueue = new Queue<LogMessage>();
    private LogMessage _currentMessage;
    private string filePath = string.Empty;
    public override void Awake()
    {
        base.Awake();
    }

    public void CreateLogFile(string logDirectory, string name)
    {
        if (filePath.Equals(string.Empty))
            filePath = Path.Combine(logDirectory, name);

        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }

        AddMessage(string.Format("Created log file at: {0}", filePath));
    }

    public void AddMessage(string message)
    {
        LogMessage log = new LogMessage(message, filePath);
        messageQueue.Enqueue(log);

        if (_currentMessage == null && messageQueue.Count > 0)
            _currentMessage = messageQueue.Dequeue();

    }

    private void Update()
    {
        ProcessLogs();
    }

    private void ProcessLogs() 
    {
        if (_currentMessage == null)
            return;

        if (!_currentMessage.isFinished)
        {
            _currentMessage.Execute();
        }
        else
        {
            if (messageQueue.Count > 0) { _currentMessage = messageQueue.Dequeue(); }
            else return;
        }
    }

}
