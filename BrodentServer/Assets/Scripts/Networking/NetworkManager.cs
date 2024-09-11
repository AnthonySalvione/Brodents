// <copyright file="NetworkManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// SERVER SIDE NETWORKMANAGER
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

/// <summary>
/// Enumerator Id's of Server to Client requests.
/// </summary>
public enum ServerToClientId : ushort
{
    /// <summary> Sends client the tick value to ensure they are synced. </summary>
    Sync = 1,

    /// <summary> Sends the necessary information for the Client to login.
    SendInfo,
}

/// <summary>
/// Enumerator Id's of Client to Server requests.
/// </summary>
public enum ClientToServerId : ushort
{
    /// <summary> Receives initial login request
    Login = 1,

    /// <summary> Client requests info after initial login.
    RequestInfo,
}

/// <summary>
/// Public definition of the NetworkManager class for the server.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager singleton;

    [SerializeField]
    private ushort port;
    [SerializeField]
    private ushort maxClientCount;

    /// <summary>
    /// Gets the private singleton and sets the public static singleton to match.
    /// </summary>
    public static NetworkManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
            {
                singleton = value;
            }
            else if (singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    /// <summary>
    /// Gets and sets the RiptideNetworking Server object.
    /// </summary>
    public Server Server { get; private set; }

    /// <summary>
    /// Gets and sets the current tick for the server.
    /// </summary>
    public ushort CurrentTick { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        this.Server = new Server();
        this.Server.ClientConnected += this.NewPlayerConnected;
        this.Server.ClientDisconnected += this.UserLeft;
        this.Server.Start(this.port, this.maxClientCount);
    }

    private void FixedUpdate()
    {
        this.Server.Update();

        if (this.CurrentTick % 250 == 0)
        {
            this.SendSync();
        }

        this.CurrentTick++;
    }

    private void OnApplicationQuit()
    {
        this.Server.Stop();
    }

    private void NewPlayerConnected(object sender, ServerConnectedEventArgs e)
    {
    }

    private void UserLeft(object sender, ServerDisconnectedEventArgs e)
    {
    }

    private void SendSync()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.Sync);
        message.Add(this.CurrentTick);

        this.Server.SendToAll(message);
    }
}
