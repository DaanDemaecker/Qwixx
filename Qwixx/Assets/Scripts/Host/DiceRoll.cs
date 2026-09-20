using Unity.Netcode;
using UnityEngine;

public enum DiceColor
{
    White,
    Red,
    Yellow,
    Green,
    Blue
}

public struct DiceRoll : INetworkSerializable
{
    public int White1;
    public int White2;
    public int Red;
    public int Yellow;
    public int Green;
    public int Blue;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref White1);
        serializer.SerializeValue(ref White2);
        serializer.SerializeValue(ref Red);
        serializer.SerializeValue(ref Yellow);
        serializer.SerializeValue(ref Green);
        serializer.SerializeValue(ref Blue);
    }
}
