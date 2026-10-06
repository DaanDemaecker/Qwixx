using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong ClientId;
    public int PlayerNumber;
    public FixedString32Bytes Name;
    public Color Color;
    public bool IsReady;
    public int Score;


    public bool Equals(PlayerData other)
    {
        return ClientId == other.ClientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerNumber);
        serializer.SerializeValue(ref Name);

        float r = Color.r;
        float g = Color.g;
        float b = Color.b;
        float a = Color.a;

        serializer.SerializeValue(ref r);
        serializer.SerializeValue(ref g);
        serializer.SerializeValue(ref b);
        serializer.SerializeValue(ref a);

        if (serializer.IsReader)
        {
            Color = new Color(r, g, b, a);
        }

        serializer.SerializeValue(ref IsReady);
        serializer.SerializeValue(ref Score);
    }
}