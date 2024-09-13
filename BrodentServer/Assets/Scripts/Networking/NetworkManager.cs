// <copyright file="NetworkManager.cs" company="Brodents">
// Copyright (c) Brodents. All rights reserved.
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

    /// <summary> Contains which inputs are currently being pressed. </summary>
    Input,
}

/// <summary>
/// Public definition of the NetworkManager class for the server.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    /// <summary> The static instance of itself referenced as a singleton.
    private static NetworkManager singleton;

    /// <summary> Ushort that contains the port of IP address.
    [SerializeField]
    private ushort port;

    /// <summary> Ushort that contains the max amount of client connections.
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

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        Singleton = this;
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called for the first time.
    /// </summary
    private void Start()
    {
        Application.targetFrameRate = 60;

        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        this.Server = new Server();
        this.Server.ClientConnected += this.NewPlayerConnected;
        this.Server.ClientDisconnected += this.UserLeft;
        this.Server.Start(this.port, this.maxClientCount);
    }

    /// <summary>
    /// This sends the inputs at fixed intervals and also switches them off.
    /// </summary>
    private void FixedUpdate()
    {
        this.Server.Update();

        if (this.CurrentTick % 250 == 0)
        {
            this.SendSync();
        }

        this.CurrentTick++;
    }

    /// <summary>
    /// Sent to all game objects before the application is quit.
    /// </summary>
    private void OnApplicationQuit()
    {
        this.Server.Stop();
    }

    /// <summary>
    /// Event called by Riptide Networking.
    /// </summary>
    /// <param name="sender"> Object. </param>
    /// <param name="e"> Event. </param>
    private void NewPlayerConnected(object sender, ServerConnectedEventArgs e)
    {
    }

    /// <summary>
    /// Event called by riptide networking.
    /// </summary>
    /// <param name="sender"> Object. </param>
    /// <param name="e"> Event. </param>
    private void UserLeft(object sender, ServerDisconnectedEventArgs e)
    {
    }

    /// <summary>
    /// Sends the current tick of the server to all connected clients.
    /// </summary>
    private void SendSync()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.Sync);
        message.Add(this.CurrentTick);

        this.Server.SendToAll(message);
    }
}
