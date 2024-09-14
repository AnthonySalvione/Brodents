// <copyright file="Character.cs" company="Brodents">
// Copyright (c) Brodents. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

/// <summary>
/// Contains the basic components of a player character.
/// </summary>
public class Character : MonoBehaviour
{
    /// <summary> The list of inputs to send to server.
    private bool[] inputs;

    /// <summary> Contains the interpolator.
    [SerializeField]
    private Interpolator interpolator;

    /// <summary>
    /// Start is called just before any of the Update methods is called for the first time.
    /// </summary>
    private void Start()
    {
        this.inputs = new bool[3]; // establishes the amount of inputs.
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            this.inputs[0] = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.inputs[1] = true;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            this.inputs[2] = true;
        }
    }

    /// <summary>
    /// This sends the inputs at fixed intervals and also switches them off.
    /// </summary>
    private void FixedUpdate()
    {
        for (int i = 0; i < this.inputs.Length; i++)
        {
            this.inputs[i] = false;
        }
    }

    #region Messages

    /// <summary>
    /// Called by FixedUpdate, sends current inputs.
    /// </summary>
    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.Input);
        message.AddBools(this.inputs);
        NetworkManager.Singleton.Client.Send(message);
    }

    [MessageHandler((ushort)ServerToClientId.PlayerMovement)]
    private static void PlayerMovement(Message message)
    {

    }

    #endregion
}
