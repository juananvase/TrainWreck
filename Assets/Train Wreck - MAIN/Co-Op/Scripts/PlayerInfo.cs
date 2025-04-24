using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[Serializable]
public class PlayerInfo
{
    public int Index;
    public Color Color;
    public int[] DeviceIDs;
    public bool IsReady;  // New field to track if the player is ready

    // Constructor to initialize PlayerInfo, including readiness state
    public PlayerInfo(int index, Color color, ReadOnlyArray<InputDevice> devices)
    {
        Index = index;
        Color = color;
        DeviceIDs = new int[devices.Count];
        for (int i = 0; i < devices.Count; i++)
        {
            DeviceIDs[i] = devices[i].deviceId;
        }
        IsReady = false;  // Default to not ready when the player first joins
    }

    // Method to get devices from DeviceIDs
    public InputDevice[] GetDevices()
    {
        InputDevice[] devices = new InputDevice[DeviceIDs.Length];
        for (int i = 0; i < DeviceIDs.Length; i++)
        {
            devices[i] = InputSystem.GetDeviceById(DeviceIDs[i]);
        }

        return devices;
    }

    // Method to set the player's readiness state
    public void SetReady(bool isReady)
    {
        IsReady = isReady;
    }
}
