// <copyright file="NetworkManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// CLIENT SIDE NETWORKMANAGER
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
    /// <summary> Ensures client is on same tick as server. </summary>
    Sync = 1,
}

/// <summary>
/// Enumerator Id's of Client to Server requests.
/// </summary>
public enum ClientToServerId : ushort
{
    /// <summary> Sends initial login request. </summary>
    Login = 1,
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
    /// Gets the private singleton and set's the public static singleton to match.
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

    private void SetTick(ushort serverTick)
    {
        if (Mathf.Abs(this.ServerTick - serverTick) > this.tickDivergenceTolerance)
        {
            Debug.Log($"Client tick: {this.ServerTick} -> {serverTick}");
            this.ServerTick = serverTick;
        }
    }
}
