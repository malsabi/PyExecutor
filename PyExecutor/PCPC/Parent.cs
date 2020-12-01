using IViewNet.Common;
using IViewNet.Common.Models;
using IViewNet.Server;
using PyExecutor.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace PyExecutor.PCPC
{
    public class Parent
    {
        #region "Fields"
        private Server ParentServer;
        private Dictionary<int, Child> ChildList;
        private object ChildListLock;
        #endregion

        #region "Properties"
        public ReadOnlyCollection<KeyValuePair<int, Child>> Children
        {
            get
            {
                lock (ChildListLock)
                {
                    return ChildList.ToList().AsReadOnly();
                }
            }
        }
        public ParentConfig ParentConfig { get; private set; }
        public NetConfig ServerConfig { get; private set; }
        public bool IsParentRunning { get; private set; }
        #endregion

        #region "Events/Delegates For Child"
        public delegate void OnChildCreateDelegate(string Message);
        public event OnChildCreateDelegate OnChildCreateEvent;
        public delegate void OnChildConnectDelegate(string Message);
        public event OnChildConnectDelegate OnChildConnectEvent;
        public delegate void OnChildAuthenticateDelegate(string Message);
        public event OnChildAuthenticateDelegate OnChildAuthenticateEvent;
        public delegate void OnChildReceiveDelegate(Packet Message);
        public event OnChildReceiveDelegate OnChildReceiveEvent;
        public delegate void OnChildSendDelegate(Packet Message);
        public event OnChildSendDelegate OnChildSendEvent;
        public delegate void OnChildDisconnectDelegate(string Message);
        public event OnChildDisconnectDelegate OnChildDisconnectEvent;
        public delegate void OnChildDestroyDelegate(string Message);
        public event OnChildDestroyDelegate OnChildDestroyEvent;
        #endregion

        #region "Events/Delegate For Parent"
        public delegate void OnParentCreatedDelegate(string Message);
        public event OnParentCreatedDelegate OnParentCreatedEvent;
        public delegate void OnParentClosedDelegate(string Message);
        public event OnParentClosedDelegate OnParentClosedEvent;
        public delegate void OnExceptionDelegate(string Message);
        public event OnExceptionDelegate OnExceptionEvent;
        #endregion

        #region "Event Handlers For Child"
        private void SetOnChildCreate(string Message)
        {
            OnChildCreateEvent?.Invoke(Message);
        }
        private void SetOnChildConnect(string Message)
        {
            OnChildConnectEvent?.Invoke(Message);
        }
        private void SetOnChildAuthenticate(string Message)
        {
            OnChildAuthenticateEvent?.Invoke(Message);
        }
        private void SetOnChildReceive(Packet Message)
        {
            OnChildReceiveEvent?.Invoke(Message);
        }
        private void SetOnChildSend(Packet Message)
        {
            OnChildSendEvent?.Invoke(Message);
        }
        private void SetOnChildDisconnect(string Message)
        {
            OnChildDisconnectEvent?.Invoke(Message);
        }
        private void SetOnChildDestroy(string Message)
        {
            OnChildDestroyEvent?.Invoke(Message);
        }
        #endregion

        #region "Event Handlers For Parent"
        private void SetOnParentCreated(string Message)
        {
            OnParentCreatedEvent?.Invoke(Message);
        }
        private void SetOnParentClosed(string Message)
        {
            OnParentClosedEvent?.Invoke(Message);
        }
        private void SetOnException(string Message)
        {
            OnExceptionEvent?.Invoke(Message);
        }
        #endregion

        #region "Event Handlers for Session"
        private void ParentServer_OnClientConnect(Operation Session)
        {
            SetOnChildConnect(string.Format("Child[{0}] Connected Successfully", Session.EndPoint));
            GetAuthenticationHandler(Session);
        }
        private void ParentServer_OnClientBlackList(IPAddress IP, string Reason)
        {
            SetOnException(string.Format("Blacklist {0} Attempted to connect", IP));
        }
        private void ParentServer_OnClientSend(Operation Session, Packet Packet)
        {
            SetOnChildSend(Packet);
        }
        private void ParentServer_OnClientReceive(Operation Session, Packet Message)
        {
            HandleAuthentication(Session, Message);
        }
        private void ParentServer_OnClientDisconnect(Operation Session, string Reason)
        {
            SetOnChildDisconnect(string.Format("Child[{0}] Disconnected: {1}", Session.EndPoint, Reason));
        }
        private void ParentServer_OnClientException(Operation Session, Exception Ex)
        {
            SetOnException(string.Format("Child[{0}] Exception: {1}", Session.EndPoint, Ex.Message));
        }
        #endregion

        public Parent(ParentConfig ParentConfig, NetConfig ServerConfig)
        {
            this.ParentConfig = ParentConfig;
            this.ServerConfig = ServerConfig;
            InitializeParent();
        }

        #region "Public Methods"
        public void StartParent()
        {
            if (IsParentRunning)
            {
                SetOnException("Cannot start Parent Server because it is already running.");
            }
            else
            {
                ParentServer.OnClientConnect += ParentServer_OnClientConnect;
                ParentServer.OnClientSend += ParentServer_OnClientSend;
                ParentServer.OnClientReceive += ParentServer_OnClientReceive;
                ParentServer.OnClientDisconnect += ParentServer_OnClientDisconnect;
                ParentServer.OnClientException += ParentServer_OnClientException;
                ParentServer.OnClientBlackList += ParentServer_OnClientBlackList;
                ParentServer.PacketManager = CreateCommands();
                StartListenerResult ListenerResult = ParentServer.StartListener();
                if (ListenerResult.IsOperationSuccess)
                {
                    StartAcceptorResult AcceptorResult = ParentServer.StartAcceptor();
                    if (AcceptorResult.IsOperationSuccess)
                    {
                        IsParentRunning = true;
                        SetOnParentCreated(string.Format("{0}\n{1}", ListenerResult.Message, AcceptorResult.Message));
                        for (int i = 0; i < ParentConfig.MaximumChildrenInstances; i++)
                        {
                            CreateChild();
                        }
                    }
                    else
                    {
                        SetOnException(AcceptorResult.Message);
                    }
                }
                else
                {
                    SetOnException(ListenerResult.Message);
                }
            }
        }

        public void CloseParent()
        {
            IsParentRunning = false;
            DestroyAllChildren();
            if (ParentServer.IsListening == true && ParentServer.IsShutdown == false)
            {
                ShutdownResult Result = ParentServer.Shutdown();
                if (Result.IsOperationSuccess)
                {
                    SetOnParentClosed(Result.Message);
                }
                else
                {
                    SetOnException(Result.Message);
                }
            }
        }

        public void DestroyChild(Child CurrentChild)
        {
            if (ChildList.ContainsKey(CurrentChild.Id))
            {
                ChildList.Remove(CurrentChild.Id);
            }
            if (CurrentChild.Destroy())
            {
                SetOnChildDestroy(string.Format("[Destroy] {0}", "Child Destroyed Successfully"));
            }
            else
            {
                SetOnChildDestroy(string.Format("[Destroy] {0}", "Failed to destroy child"));
            }
        }

        public void DestroyAllChildren()
        {
            foreach (KeyValuePair<int, Child> Item in Children)
            {
                DestroyChild(Item.Value);
            }
        }

        public Child GetAvailableChild()
        {
            foreach (KeyValuePair<int, Child> Item in Children)
            {
                if (Item.Value.Status.Equals(ChildStatus.Idle))
                {
                    return Item.Value;
                }
            }
            return null;
        }

        #endregion


        #region "Private Methods"
        private void InitializeParent()
        {
            ChildListLock = new object();
            ChildList = new Dictionary<int, Child>();
            ParentServer = new Server(ServerConfig);
        }

        private Child CreateChild()
        {
            Child Child = null;
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = ParentConfig.PythonShellPath,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = string.Format("{0}", ParentConfig.PythonScriptPath)
                };
                Child = new Child(Process.Start(info));
                ChildList.Add(Child.Id, Child);
                SetOnChildCreate(string.Format("Child Process[{0}:{1}] has been successfully created.", Child.ProcessName, Child.Id));
            }
            catch (Exception ex)
            {
                SetOnException(string.Format("Failed to CreateChild, Exception: {0}", ex.Message));
            }
            return Child;
        }

        private void Cycle()
        {
            while (IsParentRunning)
            {
                foreach (KeyValuePair<int, Child> Item in Children)
                {
                    if (Item.Value.IsRunning == false)
                    {
                        DestroyChild(Item.Value);
                    }
                }
                Thread.Sleep(250);
            }
        }

        private PacketManager CreateCommands()
        {
            PacketManager PacketManager = new PacketManager();
            PacketManager.AddPacket(new Packet(1111, "GetAuthentication", null));
            PacketManager.AddPacket(new Packet(2222, "SetAuthentication", null));
            PacketManager.AddPacket(new Packet(3333, "GetStatus", null));
            PacketManager.AddPacket(new Packet(4444, "SetStatus", null));
            PacketManager.AddPacket(new Packet(5555, "SetDetectionType", null));
            PacketManager.AddPacket(new Packet(6666, "GetDetectedFrame", null));
            PacketManager.AddPacket(new Packet(7777, "SetDetectedFrame", null));
            PacketManager.AddPacket(new Packet(8888, "Terminate", null));

            return PacketManager;
        }
        private void HandleChildSession(Operation Session)
        {
            lock (ChildListLock)
            {
                int Id = (int)Session.Value;
                if (ChildList.ContainsKey(Id))
                {
                    ChildList[Id].Session = Session;
                }
                else
                {
                    SetOnException(string.Format("Failed to HandleChildSession for Child[{0}]", Session.EndPoint));
                }
            }
        }
        private void GetAuthenticationHandler(Operation Session)
        {
            Session.SendPacket(new Packet(1111, "GetAuthentication", null));
        }
        private void HandleAuthentication(Operation Session, Packet Message)
        {
            if (Session.IsAuthenticated == false)
            {
                if (Message.Name.Equals("SetAuthentication"))
                {
                    Session.Value = BitConverter.ToInt32(Message.Content, 0);
                    Session.IsAuthenticated = true;
                    HandleChildSession(Session);
                    SetOnChildAuthenticate(string.Format("Child[{0}] Authenticated Successfully", Session.EndPoint));
                }
                else
                {
                    SetOnChildAuthenticate(string.Format("Child[{0}] Authentication Failed", Session.EndPoint));
                    Session.ShutdownOperation();
                }
            }
            else
            {
                HandleMessages(Session, Message);
            }
        }

        private Child GetChild(Operation Session)
        {
            lock (ChildListLock)
            {
                int Id = (int)Session.Value;
                if (ChildList.ContainsKey(Id))
                {
                    return ChildList[Id];
                }    
            }
            return null;
        }

        private void HandleMessages(Operation Session, Packet Message)
        {
            if (Message.Name.Equals("SetStatus"))
            {
                string Status = System.Text.Encoding.Default.GetString(Message.Content);
                Child CurrentChild = GetChild(Session);
                CurrentChild.Status = (ChildStatus)Enum.Parse(typeof(ChildStatus), Status);
            }
            else
            {
                SetOnChildReceive(Message);
            }
        }
        #endregion
    }
}