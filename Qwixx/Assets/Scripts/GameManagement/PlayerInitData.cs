using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerInitData : IEquatable<PlayerInitData>, INetworkSerializable
{
    public ulong ClientId;
    public int PlayerNumber;
    public FixedString32Bytes Name;
    public Color Color;


    public bool Equals(PlayerInitData other)
    {
        return ClientId == other.ClientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerNumber);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Color);
    }
}