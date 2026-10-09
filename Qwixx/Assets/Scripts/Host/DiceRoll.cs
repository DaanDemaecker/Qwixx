using System;
using Unity.Netcode;
using UnityEngine.Events;

[Serializable]
public enum DiceColor
{
    Color0,
    Color1,
    Color2,
    Color3,
    Color4
}

public class DiceRoll
{
    public struct DiceRollData : INetworkSerializable
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

    private DiceRollData _diceRollDataHolder = new DiceRollData();

    public DiceRollData DiceRollDataHolder
    {
        get
        {
            return _diceRollDataHolder;
        }
    }

    public UnityEvent OnRollCompleteEvent = new UnityEvent();

    public void Reset()
    {
        _diceRollDataHolder.Color0_1 = -1;
        _diceRollDataHolder.Color0_2 = -1;
        _diceRollDataHolder.Color1 = -1;
        _diceRollDataHolder.Color2 = -1;
        _diceRollDataHolder.Color3 = -1;
        _diceRollDataHolder.Color4 = -1;
    }

    public void SetValue(DiceColor color, int value)
    {
        switch(color)
        {
            case DiceColor.Color0:
                if(_diceRollDataHolder.Color0_1 <= 0)
                {
                    _diceRollDataHolder.Color0_1 = value;
                }
                else
                {
                    _diceRollDataHolder.Color0_2 = value;
                }    
                break;
            case DiceColor.Color1:
                _diceRollDataHolder.Color1 = value;
                break;
            case DiceColor.Color2:
                _diceRollDataHolder.Color2 = value;
                break;
            case DiceColor.Color3:
                _diceRollDataHolder.Color3 = value;
                break;
            case DiceColor.Color4:
                _diceRollDataHolder.Color4 = value;
                break;
        }

        if(IsComplete())
        {
            OnRollCompleteEvent.Invoke();
        }    
    }

    private bool IsComplete()
    {
        return _diceRollDataHolder.Color0_1 > 0 &&
            _diceRollDataHolder.Color0_2 > 0 &&
            _diceRollDataHolder.Color1 > 0 &&
            _diceRollDataHolder.Color2 > 0 &&
            _diceRollDataHolder.Color3 > 0 &&
            _diceRollDataHolder.Color4 > 0;
    }
}



