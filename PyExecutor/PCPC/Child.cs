using IViewNet.Common;
using PyExecutor.Enums;
using PyExecutor.Interfaces;
using System;
using System.Diagnostics;
using System.Drawing;

namespace PyExecutor.PCPC
{
    public class Child : IChild
    {
        #region "Fields"
        private readonly Process ChildHandle;
        #endregion

        #region "Properties"
        public Operation Session { get; set; }
        /// <summary>
        /// Returns the child ID of the process
        /// </summary>
        public int Id
        {
            get
            {
                if (IsRunning)
                {
                    return ChildHandle.Id;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Returns true if the result of the handshake was success otherwise false
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Returns true if the child process is running otherwise false
        /// </summary>
        public bool IsRunning
        {
            get
            {
                try
                {
                    return !ChildHandle.HasExited;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the child process name
        /// </summary>
        public string ProcessName
        {
            get
            {
                try
                {
                    return ChildHandle.ProcessName;
                }
                catch
                {
                    return "N/A";
                }
            }
        }

        /// <summary>
        /// Returns true if the child is doing some work otherwise false
        /// </summary>
        public ChildStatus Status { get; set; }

        /// <summary>
        /// Returns the creation time of the child
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// Returns the total procecc time
        /// </summary>
        public TimeSpan TotalRunningTime
        {
            get
            {
                return DateTime.Now.Subtract(CreationTime);
            }
        }
        #endregion

        #region "Events/Delegates And Handlers"
        public delegate void OnChildErrorDelegate(ChildEventArgs Args);
        public event OnChildErrorDelegate OnChildErrorEvent;

        public delegate void OnChildSendDelegate(ChildEventArgs Args);
        public event OnChildSendDelegate OnChildSendEvent;

        public delegate void OnChildReceiveDelegate(ChildEventArgs Args);
        public event OnChildReceiveDelegate OnChildReceiveEvent;

        public void SetOnChildError(ChildEventArgs Args)
        {
            OnChildErrorEvent?.Invoke(Args);
        }

        public void SetOnChildSend(ChildEventArgs Args)
        {
            OnChildSendEvent?.Invoke(Args);
        }

        public void SetOnChildReceive(ChildEventArgs Args)
        {
            OnChildReceiveEvent?.Invoke(Args);
        }
        #endregion

        public Child(Process ChildHandle)
        {
            this.ChildHandle = ChildHandle;
            if (this.ChildHandle != null)
            {
                InitializeChild();
            }
            else
            {
                SetOnChildError(new ChildEventArgs(this, null, string.Format("[Exception] {0}", "Failed to initialize child"), Color.Red));
            }
        }

        #region "Private Methods"
        private void InitializeChild()
        {
            IsValid = false;
            Status = ChildStatus.None;
            CreationTime = DateTime.Now;
        }
        #endregion

        #region "Public Methods"
        public bool Destroy()
        {
            try
            {
                if (IsRunning == true)
                {
                    ChildHandle.Kill();
                    ChildHandle.Close();
                    ChildHandle.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}