using PyExecutor.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PyExecutor.Queues
{
    public class OutputQueue
    {
        #region "Fields"
        private ConcurrentPriorityQueue<int, Frame> InnerQueue;
        private bool IsProcessing;
        private object IsProcessingLock;
        private int FrameCounter;
        #endregion

        #region "Events/Delegates"
        public delegate void OnFrameDelegate(Frame Item);
        public event OnFrameDelegate OnFrameEvent;
        #endregion

        #region "Event Handlers"
        private void SetOnFrame(Frame Item)
        {
            OnFrameEvent?.Invoke(Item);
        }
        #endregion
        #region "Properties"
        public int GetCount
        {
            get
            {
                if (InnerQueue != null)
                {
                    return InnerQueue.Count;
                }
                return 0;
            }
        }
        #endregion

        #region "Public Methods"
        public OutputQueue()
        {
            Initialize();
        }

        public void Enqueue(Frame Item)
        {
            InnerQueue.Enqueue(Item.Index, Item);
            lock (IsProcessingLock)
            {
                if (IsProcessing == false)
                {
                    IsProcessing = true;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Worker));
                }
            }
        }

        #endregion

        #region "Private Methods"
        private void Initialize()
        {
            InnerQueue = new ConcurrentPriorityQueue<int, Frame>();
            IsProcessing = false;
            IsProcessingLock = new object();
            FrameCounter = 0;
        }

        private void Worker(object o)
        {
            while (true)
            {
                if (InnerQueue.Count == 0)
                {
                    lock (IsProcessingLock)
                    {
                        IsProcessing = false;
                        break;
                    }
                }
                else
                {
                    if (InnerQueue.TryPeek(out KeyValuePair<int, Frame> PeekResult))
                    {

                        if (PeekResult.Key == FrameCounter)
                        {
                            if (InnerQueue.TryDequeue(out KeyValuePair<int, Frame> OutResult))
                            {
                                SetOnFrame(OutResult.Value);
                                FrameCounter++;
                            }
                            else
                            {
                                Console.WriteLine("Failed At Dequeue Step, Remaining: {0}", InnerQueue.Count);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed At Peek Step, Remaining: {0}", InnerQueue.Count);
                    }
                }
            }
        }
        #endregion
    }
}
