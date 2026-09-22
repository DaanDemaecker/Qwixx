using System;
using Unity.Netcode;

[Serializable]
public enum DiceColor
{
    Color0,
    Color1,
    Color2,
    Color3,
    Color4
}

public struct DiceRoll : INetworkSerializable
{
    public int Color0_1;
    public int Color0_2;
    public int Color1;
    public int Color2;
    public int Color3;
    public int Color4;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Color0_1);
        serializer.SerializeValue(ref Color0_2);
        serializer.SerializeValue(ref Color1);
        serializer.SerializeValue(ref Color2);
        serializer.SerializeValue(ref Color3);
        serializer.SerializeValue(ref Color4);
    }
}
