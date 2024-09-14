// <copyright file="User.cs" company="Brodents">
// Copyright (c) Brodents. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the dictionary of users that are connected.
/// </summary>
public class User : MonoBehaviour
{
    /// <summary> Contains the ushort id key for each current connected user.
    private static Dictionary<ushort, User> userList = new Dictionary<ushort, User>();

    /// <summary> Gets and sets the assigned temporary value of a connected instance.
    public ushort Id { get; private set; }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        userList.Remove(this.Id);
    }
}
