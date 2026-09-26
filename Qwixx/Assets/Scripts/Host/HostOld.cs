using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class HostOld : NetworkBehaviour
{
    [Serializable]
    private struct DieColorPrefabPair
    {
        public DiceColor Color;
        public GameObject Prefab;
    }

    [SerializeField]
    private List<DieColorPrefabPair> _diePrefabs = null;

    private List<Die> _diceObjects = new();

    private PlayerOld _player = null;

    private ulong _lastPlayerId = ulong.MaxValue;

    public const string DIE_SPAWN_OBJECT_TAG = "DieSpawn";

    private DiceRoll _diceRoll = new DiceRoll();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SpawnDice();

            _diceRoll.Reset();
            _diceRoll.RollCompleteEvent.AddListener(AllRollsComplete);
        }
    }

    public void RegisterPlayer(PlayerOld player)
    {
        _player = player;
    }

    public void RollDice(ulong clientId)
    {
        _lastPlayerId = clientId;

        _diceRoll.Reset();

        foreach (Die die in _diceObjects)
        {
            die.MoveToStartPosition();
            die.Roll();
        }
    }

    private void AllRollsComplete(DiceRoll.DiceRollData data)
    {
        RollDiceResultClientRpc(data, _lastPlayerId);
    }

    [ClientRpc]
    private void RollDiceResultClientRpc(DiceRoll.DiceRollData diceRoll, ulong clientId)
    {
        if (_player != null)
        {
            _player.ReceiveRoll(diceRoll, clientId);
        }
    }

    private void SpawnDice()
    {
        List<GameObject> startPostions = new List<GameObject>(GameObject.FindGameObjectsWithTag(DIE_SPAWN_OBJECT_TAG));

        int index = 0;

        foreach (var prefabPair in _diePrefabs)
        {
            Vector3 startPos = Vector3.zero;

            if(startPostions.Count > 0)
            {
                startPos = startPostions[index % startPostions.Count].transform.position;
                ++index;
            }

            SpawnDie(prefabPair, startPos);

            if(prefabPair.Color == DiceColor.Color0)
            {
                if (startPostions.Count > 0)
                {
                    startPos = startPostions[index % startPostions.Count].transform.position;
                    ++index;
                }

                SpawnDie(prefabPair, startPos);
            }
        }
    }

    private void SpawnDie(DieColorPrefabPair pair, Vector3 startPos)
    {
        if(pair.Prefab == null)
        {
            return;
        }

        var die = Instantiate(pair.Prefab);

        Die dieComponent = null;

        if (die.TryGetComponent<Die>(out dieComponent))
        {
            dieComponent.BeginPosition = startPos;
            dieComponent.MoveToStartPosition();
            dieComponent.RollCompleteEvent.AddListener(RollComplete);

            _diceObjects.Add(dieComponent);
        }
    }

    private void RollComplete(DiceColor color, int value)
    {
        _diceRoll.SetValue(color, value);
    }
}
