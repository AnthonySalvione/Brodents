// <copyright file="NetworkManager.cs" company="Brodents">
// Copyright (c) Brodents. All rights reserved.
// </copyright>

// CLIENT SIDE NETWORKMANAGER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Riptide;
using Riptide.Utils;
using UnityEngine;

/// <summary>
/// Enumerator Id's of Server to Client requests.
/// </summary>
public enum ServerToClientId : ushort
{
    /// <summary> Ensures client is on same tick as server. </summary>
    Sync = 1,

    /// <summary> Sends the necessary information for the Client to login.
    SendInfo,

    /// <summary> Sends coordinents of player from server.
    PlayerMovement,
}

/// <summary>
/// Enumerator Id's of Client to Server requests.
/// </summary>
public enum ClientToServerId : ushort
{
    /// <summary> Sends initial login request. </summary>
    Login = 1,

    /// <summary> If login reaches destination, call DidConnect. </summary>
    RequestInfo,

    /// <summary> Contains which inputs are currently being pressed. </summary>
    Input,
}

/// <summary>
/// Instantiates the NetworkManager class.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    /// <summary> The static instance of itself referenced as a singleton.
    private static NetworkManager singleton;

    /// <summary> Contains the current server tick according to the last update.
    private ushort serverTick;

    /// <summary> Ticks between position update value.
    [SerializeField]
    private ushort ticksBetweenPositionUpdates = 2;

    /// <summary> Tick divergence tolerance value.
    [SerializeField]
    private ushort tickDivergenceTolerance = 1;

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
    /// Gets and sets the RiptideNetworking Client object.
    /// </summary>
    public Client Client { get; private set; }

    /// <summary>
    /// Gets tick from server, then sets the client's tick accordingly.
    /// </summary>
    public ushort ServerTick
    {
        get => this.serverTick;
        private set
        {
            this.serverTick = value;
            this.InterpolationTick = (ushort)(value - this.TicksBetweenPositionUpdates);
        }
    }

    /// <summary>
    /// Gets and sets the interpolation tick;
    /// which fills the gaps which smooths out the lag.
    /// </summary>
    public ushort InterpolationTick { get; private set; }

    /// <summary>
    /// Gets and sets the tick value between the last value to current server value.
    /// </summary>
    public ushort TicksBetweenPositionUpdates
    {
        get => this.ticksBetweenPositionUpdates;
        private set
        {
            this.ticksBetweenPositionUpdates = value;
            this.InterpolationTick = (ushort)(this.serverTick - value);
        }
    }

    /// <summary>
    /// Receives the tick value from the server through the sync message.
    /// </summary>
    /// <param name="message"> Contains the current tick of the server. </param>
    [MessageHandler((ushort)ServerToClientId.Sync)]
    private static void Sync(Message message)
    {
        Singleton.SetTick(message.GetUShort());
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        this.Client.Update();
        this.ServerTick++;
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (!Singleton)
        {
            Singleton = this;
        }

        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called for the first time.
    /// </summary
    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        this.Client = new Client();
        this.Client.Connected += this.DidConnect;
        this.Client.ConnectionFailed += this.FailedToConnect;
        this.Client.ClientDisconnected += this.PlayerLeft;
        this.Client.Disconnected += this.DidDisconnect;

        this.Connect();

        this.ServerTick = 2;
    }

    /// <summary>
    /// Takes serverTick as a parameter and makes client's tick match it.
    /// </summary>
    /// <param name="serverTick"> Tick value of the server. </param>
    private void SetTick(ushort serverTick)
    {
        if (Mathf.Abs(this.ServerTick - serverTick) > this.tickDivergenceTolerance)
        {
            Debug.Log($"Client tick: {this.ServerTick} -> {serverTick}");
            this.ServerTick = serverTick;
        }
    }

    /// <summary>
    /// Initial connection attempt.
    /// </summary>
    private void Connect()
    {
        this.Client.Connect($"{"192.168.0.179"}:{"7777"}");
    }

    /// <summary>
    /// Is called after receiving feedback from the server.
    /// </summary>
    /// <param name="sender"> Object. </param>
    /// <param name="e"> Event. </param>
    private void DidConnect(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Is called after disconnecting from server.
    /// </summary>
    /// <param name="sender"> Object. </param>
    /// <param name="e"> Event. </param>
    private void DidDisconnect(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Is called if the Connect was called and received no feedback.
    /// </summary>
    /// <param name="sender"> Object. </param>
    /// <param name="e"> Event. </param>
    private void FailedToConnect(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Called from server if another player left.
    /// </summary>
    /// <param name="sender"> Object. </param>
    /// <param name="e"> Event. </param>
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
    }
}
