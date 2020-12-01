using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace PyExecutor.PCPC
{
    public class FrameIndexHandler : IDisposable
    {
        #region "Fields"
        private SortedList<int, Bitmap> InnerSortedList;
        private object InnertSortedListLock;
        private System.Timers.Timer CallHandler;
        private bool IsWorking;
        private int LastKey;
        #endregion

        #region "Properties"
        #endregion

        #region "Events & Delegates"
        public delegate void OnPopDelegate(KeyValuePair<int, Bitmap>[] CorrectSortedSequence);
        public event OnPopDelegate OnPopEvent;
        #endregion

        #region "Constructors"
        public FrameIndexHandler()
        {
            Initialize();
        }
        #endregion

        #region "Private Methods"
        private void Initialize()
        {
            InnerSortedList = new SortedList<int, Bitmap>();
            InnertSortedListLock = new object();

            CallHandler = new System.Timers.Timer(100);
            CallHandler.Elapsed += CallHandler_Elapsed;

            IsWorking = false;
            LastKey = 0;
        }

        private void CallHandler_Elapsed(object sender, ElapsedEventArgs e)
        {
            KeyValuePair<int, Bitmap>[] Result = Pop();
            if (Result != null)
            {
                OnPopEvent?.Invoke(Result);
            }
        }

        private bool CheckSequence()
        {
            if (InnerSortedList.Count >= 2)
            {
                for (int i = 1; i < InnerSortedList.Keys.Count; i++)
                {
                    if ((InnerSortedList.Keys[i] - InnerSortedList.Keys[i - 1] != 1))
                    {
                        return false;
                    }
                }
                if (LastKey == 0)
                {
                    return true;
                }
                else if((InnerSortedList.Keys[0] - LastKey) == 1)
                {
                    return true;
                }
                return false;
            }
            //else if (InnerSortedList.Count == 1) //Test purpose
            //{
            //    return true;
            //}
            else
            {
                return false;
            }
        }

        private KeyValuePair<int, Bitmap>[] Pop()
        {
            lock (InnertSortedListLock)
            {
                KeyValuePair<int, Bitmap>[] CorrectSortedSequence;
                if (CheckSequence() == true)
                {
                    CorrectSortedSequence = InnerSortedList.ToArray();
                    if (CorrectSortedSequence.Count() == 1)
                    {
                        LastKey = CorrectSortedSequence[0].Key;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Count == 1 LAST KEY {0}", LastKey);
                    }
                    else
                    {
                        LastKey = CorrectSortedSequence.Last().Key;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Count > 1 LAST KEY {0}", LastKey);
                    }
                    InnerSortedList.Clear();
                }
                else
                {
                    CorrectSortedSequence = null;
                }
                return CorrectSortedSequence;
            }
        }
        #endregion

        #region "Public Methods"
        public void Start()
        {
            if (IsWorking == false)
            {
                IsWorking = true;
                CallHandler.Start();
            }
        }
        public void Stop()
        {
            if (IsWorking == true)
            {
                IsWorking = false;
                CallHandler.Stop();
            }
        }
        public void Insert(KeyValuePair<int, Bitmap> Item)
        {
            lock (InnertSortedListLock)
            {
                InnerSortedList.Add(Item.Key, Item.Value);
            }
        }
        public void Dispose()
        {
            Stop();
            CallHandler.Dispose();
            InnerSortedList.Clear();
        }
        #endregion
    }
}