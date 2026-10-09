using IngameDebugConsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DiceManager : MonoBehaviour
{
    [SerializeField]
    private Host _host = null;

    private DiceRoll _rollInfo = new DiceRoll();

#if DEBUG
    private static DiceRoll _debugRollInfo = new DiceRoll();

    private static bool _debugRollEnabled = false;
#endif

    private List<Die> _dice = new();

    private bool _initialRollDone = false;

    private ulong _activePlayerId = 0;

    private void Awake()
    {
        _rollInfo.OnRollCompleteEvent.AddListener(DiceManager_OnRollComplete);
        _rollInfo.Reset();
    }

#if DEBUG

    [ConsoleMethod("SetNextRole", "Sets the values of the next role manually")]
    public static void SetNextRoll(int color0_1, int color0_2, int color1, int color2, int color3, int color4)
    {
        _debugRollEnabled = true;

        _debugRollInfo.Reset();

        _debugRollInfo.SetValue(DiceColor.Color0, color0_1);
        _debugRollInfo.SetValue(DiceColor.Color0, color0_2);
        _debugRollInfo.SetValue(DiceColor.Color1, color1);
        _debugRollInfo.SetValue(DiceColor.Color2, color2);
        _debugRollInfo.SetValue(DiceColor.Color3, color3);
        _debugRollInfo.SetValue(DiceColor.Color4, color4);
    }
#endif

    private void DiceManager_OnRollComplete()
    {
        if (_initialRollDone)
        {
            if (_host != null)
            {

#if DEBUG
                if(_debugRollEnabled)
                {
                    _host.RollComplete(_debugRollInfo.DiceRollDataHolder, _activePlayerId);
                    _debugRollEnabled = false;
                }
                else
                {
                    _host.RollComplete(_rollInfo.DiceRollDataHolder, _activePlayerId);
                }
#else
                    _host.RollComplete(_rollInfo.DiceRollDataHolder, _activePlayerId);
#endif
            }
        }
        else
        {
            _initialRollDone = true;
        }
    }

    public void AddDie(Die die)
    {
        _dice.Add(die);
        die.OnRollCompleteEvent.AddListener(DiceManager_OnSingleRollComplete);
    }

    public void Roll(ulong activePlayerId)
    {
        _activePlayerId = activePlayerId;
        _rollInfo.Reset();

        foreach (Die die in _dice)
        {
            die.MoveToStartPosition();
            die.Roll();
        }
    }

    private void DiceManager_OnSingleRollComplete(DiceColor color, int value)
    {
        _rollInfo.SetValue(color, value);
    }
}
