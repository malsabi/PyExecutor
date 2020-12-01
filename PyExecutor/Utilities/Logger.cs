using PyExecutor.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PyExecutor.Utilities
{
    public class Logger
    {
        #region "Private Fields"
        private ConcurrentQueue<Log> LogsQueue;
        private int MaximumLogs;
        private int LogsCounter;
        private bool IsProcessing;
        private object IsProcessingLock;
        #endregion

        #region "Privte Methods"
        private void Initialize()
        {
            LogsQueue = new ConcurrentQueue<Log>();
            IsProcessingLock = new object();
            IsProcessing = false;
            LogsCounter = 0;
        }

        private void Worker(object o)
        {
            while (true)
            {
                if (LogsQueue.Count == 0)
                {
                    lock (IsProcessingLock)
                    {
                        IsProcessing = false;
                        break;
                    }
                }
                else
                {
                    if (LogsQueue.TryDequeue(out Log Item))
                    {
                        if (LogsCounter == MaximumLogs)
                        {
                            ClearLogger();
                        }
                        Console.ForegroundColor = Item.Color;
                        Console.WriteLine(Item.Message);
                        LogsCounter++;
                    }
                }
            }
        }
        #endregion

        public Logger(int MaximumLogs)
        {
            Initialize();
            this.MaximumLogs = MaximumLogs;
        }

        public void Log(Log Item)
        {
            LogsQueue.Enqueue(Item);
            lock (IsProcessingLock)
            {
                if (IsProcessing == false)
                {
                    IsProcessing = true;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Worker));
                }
            }
        }

        public void ClearLogger()
        {
            Console.Clear();
            LogsCounter = 0;
        }
    }
}