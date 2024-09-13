// <copyright file="Interpolator.cs" company="Brodents">
// Copyright (c) Brodents. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds a way to smooth out the movement
/// between distances if there is an update between the server and client.
/// </summary>
public class Interpolator : MonoBehaviour
{
    /// <summary>
    /// Contains the value of transform updates for interpolation before it executes.
    /// </summary>
    private readonly List<TransformUpdate> futureTransformUpdates = new List<TransformUpdate>();

    /// <summary> How much time has elapsed since last interpolation.
    [SerializeField]
    private float timeElapsed = 0f;

    /// <summary> The set amount of time to reach target (amount of time to smooth movement).
    [SerializeField]
    private float timeToReachTarget = 0.05f;

    /// <summary> The threshold of difference to requre an interpolation.
    [SerializeField]
    private float movementThreshold = 0.05f;

    /// <summary> Value of movement threshold squared.
    private float squareMovementThreshold;

    /// <summary> Transform value destination.
    private TransformUpdate to;

    /// <summary> Transform value of current.
    private TransformUpdate from;

    /// <summary> Transform value of last location.
    private TransformUpdate previous;

    /// <summary>
    /// Adds future transform updates to be called by InterpolatePosition.
    /// </summary>
    /// <param name="tick"> Current tick. </param>
    /// <param name="isTeleport"> Is the player being updated. </param>
    /// <param name="position"> The position update value. </param>
    public void NewUpdate(ushort tick, bool isTeleport, Vector3 position)
    {
        if (tick <= NetworkManager.Singleton.InterpolationTick && !isTeleport)
        {
            return;
        }

        for (int i = 0; i < this.futureTransformUpdates.Count; i++)
        {
            if (tick < this.futureTransformUpdates[i].Tick)
            {
                this.futureTransformUpdates.Insert(i, new TransformUpdate(tick, isTeleport, position));
                return;
            }
        }

        this.futureTransformUpdates.Add(new TransformUpdate(tick, isTeleport, position));
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called for the first time.
    /// </summary>
    private void Start()
    {
        this.squareMovementThreshold = this.movementThreshold * this.movementThreshold;
        this.to = new TransformUpdate(NetworkManager.Singleton.ServerTick, false, this.transform.position);
        this.from = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, false, this.transform.position);
        this.previous = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, false, this.transform.position);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        for (int i = 0; i < this.futureTransformUpdates.Count; i++)
        {
            if (NetworkManager.Singleton.ServerTick >= this.futureTransformUpdates[i].Tick)
            {
                if (this.futureTransformUpdates[i].IsTeleport)
                {
                    this.to = this.futureTransformUpdates[i];
                    this.from = this.to;
                    this.previous = this.to;
                    this.transform.position = this.to.Position;
                }
                else
                {
                    this.previous = this.to;
                    this.to = this.futureTransformUpdates[i];
                    this.from = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, false, this.transform.position);
                }

                this.futureTransformUpdates.RemoveAt(i);
                i--;
                this.timeElapsed = 0f;
                float ticksToReach = this.to.Tick - this.from.Tick;

                if (ticksToReach == 0f)
                {
                    ticksToReach = 1f;
                }

                this.timeToReachTarget = ticksToReach * Time.fixedDeltaTime;
            }
        }

        this.timeElapsed += Time.deltaTime;
        this.InterpolatePosition(this.timeElapsed / this.timeToReachTarget);
    }

    /// <summary>
    /// Executes the function that interpolates the position of the character.
    /// </summary>
    /// <param name="lerpAmount"> The distance of travel. </param>
    private void InterpolatePosition(float lerpAmount)
    {
        if ((this.to.Position - this.previous.Position).sqrMagnitude < this.squareMovementThreshold)
        {
            if (this.to.Position != this.from.Position)
            {
                this.transform.position = Vector3.Lerp(this.from.Position, this.to.Position, lerpAmount);
            }

            return;
        }

        this.transform.position = Vector2.LerpUnclamped(this.from.Position, this.to.Position, lerpAmount);
    }
}