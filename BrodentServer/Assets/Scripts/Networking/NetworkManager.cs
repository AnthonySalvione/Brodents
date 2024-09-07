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
}

/// <summary>
/// Enumerator Id's of Client to Server requests.
/// </summary>
public enum ClientToServerId : ushort
{
    /// <summary> Receives initial login request
    Login = 1,
}

/// <summary>
/// Public definition of the NetworkManager class for the server.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
