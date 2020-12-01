using PyExecutor.Models;
using PyExecutor.PCPC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace PyExecutor.Queues
{
    //Generic InputQueue, Used for PCPC
    public class InputQueue
    {
        #region "Fields"
        private Parent ParentServer;
        private ConcurrentQueue<Frame> InnerQueue;
        private bool IsProcessing;
        private object IsProcessingLock;
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
        public InputQueue(Parent ParentServer)
        {
            this.ParentServer = ParentServer;
            Initialize();
        }
        public void Enqueue(Frame Item)
        {
            InnerQueue.Enqueue(Item);
            lock (IsProcessingLock)
            {
                if (IsProcessing == false)
                {
                    IsProcessing = true;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Worker));
                }
            }
        }
        public void Process(IViewNet.Common.Models.Packet Message)
        {
            foreach (KeyValuePair<int, Child> Item in ParentServer.Children)
            {
                Item.Value.Session.SendPacket(Message);
            }
        }
        #endregion

        #region "Private Methods"
        private void Initialize()
        {
            InnerQueue = new ConcurrentQueue<Frame>();
            IsProcessing = false;
            IsProcessingLock = new object();
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
                    Child CurrentChild = ParentServer.GetAvailableChild();
                    if (CurrentChild != null)
                    {
                        if (InnerQueue.TryDequeue(out Frame Item))
                        {
                            byte[] Message = new byte[Item.BitmapBytes.Length + 2];
                            Buffer.BlockCopy(BitConverter.GetBytes(Item.Index), 0, Message, 0, 2);
                            Buffer.BlockCopy(Item.BitmapBytes, 0, Message, 2, Item.BitmapBytes.Length);
                            CurrentChild.Session.SendPacket(new IViewNet.Common.Models.Packet(6666, "GetDetectedFrame", Message));
                        }
                    }
                }
            }
        }
        #endregion
    }
}
