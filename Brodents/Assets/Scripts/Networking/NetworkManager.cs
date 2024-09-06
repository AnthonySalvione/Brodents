// <copyright file="NetworkManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Riptide;
using Riptide.Utils;
using UnityEngine;

/// <summary>
/// Enumerator Id's of Server to Slient requests.
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
}
