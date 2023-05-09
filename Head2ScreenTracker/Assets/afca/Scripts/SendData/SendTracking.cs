using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SendTracking : MonoBehaviour
{
    [SerializeField]
    bool IsActive;
    public static SendTracking Instance { get; private set; }
    private Socket sender;
    private bool isInitialized;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        AppEvents.OnOpenSocket += AppEvents_OnOpenSocket;
        AppEvents.OnCloseSocket += AppEvents_OnCloseSocket;
    }

    

    private void OnDisable()
    {
        AppEvents.OnOpenSocket -= AppEvents_OnOpenSocket;
        AppEvents.OnCloseSocket -= AppEvents_OnCloseSocket;
    }
    private void AppEvents_OnOpenSocket()
    {
        InitSocket();
    }
    private void AppEvents_OnCloseSocket()
    {
        this.CloseSocket();
    }
    #region Socket Handling
    public void InitSocket()
    {
        if (IsActive && !isInitialized)
        {
            IPAddress ipAddress = IPAddress.Parse(PlayerPrefs.GetString(EAppSettingsNames.NetworkIP.ToString()));
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, PlayerPrefs.GetInt(EAppSettingsNames.NetworkPort.ToString()));
            sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {

                // Connect to Remote EndPoint
                sender.Connect(remoteEP);
                isInitialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
    public void CloseSocket()
    {
        //if (sender != null)
        //{
        //    sender.Shutdown(SocketShutdown.Both);
        //    sender.Close();
        //}
    }
    public void SendPosition(int X, int Y)
    {
        if (IsActive)
        {
            if (!isInitialized) InitSocket();
            byte[] bytes = new byte[1024];
            if (this.sender == null) InitSocket();
            byte[] bytesToSend = Encoding.ASCII.GetBytes($"{X}_{Y}");
            try
            {
                int numOfBytesSent = sender.Send(bytesToSend);
                int numOfBytesReceived = sender.Receive(bytes);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        else
        {
            Debug.LogWarning("Send Trackig is not active. Please set Property 'IsActive' in Script 'SendTracking'");
        }

    }
    #endregion
}
