// <copyright file="NetworkManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
}

/// <summary>
/// Instantiates the NetworkManager class.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager singleton;
    private ushort serverTick;

    [SerializeField]
    private ushort ticksBetweenPositionUpdates = 2;

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

    [MessageHandler((ushort)ServerToClientId.Sync)]
    private static void Sync(Message message)
    {
        Singleton.SetTick(message.GetUShort());
    }

    private void FixedUpdate()
    {
        this.Client.Update();
        this.ServerTick++;
    }

    /// <summary>
    /// Executes upon the first time this class file is opened.
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
    /// Also executes upon first time file is opened, but after Awake().
    /// </summary>
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

    private void Connect()
    {
        this.Client.Connect($"{"192.168.0.179"}:{"7777"}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
    }
}
