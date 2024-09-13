// <copyright file="TransformUpdate.cs" company="Brodents">
// Copyright (c) Brodents. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transform Update class that contains the three values: Tick, IsTeleport and Position.
/// </summary>
public class TransformUpdate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransformUpdate"/> class.
    /// </summary>
    /// <param name="tick"> Tick value. </param>
    /// <param name="isTeleport"> Did teleport boolean. </param>
    /// <param name="position"> New position value. </param>
    public TransformUpdate(ushort tick, bool isTeleport, Vector3 position)
    {
        this.Tick = tick;
        this.IsTeleport = isTeleport;
        this.Position = position;
    }

    /// <summary>
    /// Gets and sets tick value.
    /// </summary>
    public ushort Tick { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the character teleported or not.
    /// </summary>
    public bool IsTeleport { get; private set; }

    /// <summary>
    /// Gets and sets the position vector.
    /// </summary>
    public Vector3 Position { get; private set; }
}