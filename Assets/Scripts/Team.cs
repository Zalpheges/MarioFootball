using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField]
    private string _ateamBrainType;
    public Type TeamBrainType => Type.GetType(_ateamBrainType);

    [SerializeField]
    private string _agoalBrainType;
    public Type GoalBrainType => Type.GetType(_agoalBrainType);

    public Player[] Players { get; private set; }
    public PlayerBrain[] Brains { get; private set; }
    public Player Goalkeeper { get; private set; }

    public PlayerBrain Brain { get; private set; }

    public Team Other => Field.Team1 == this ? Field.Team2 : Field.Team1;

    private Queue<ItemData> _items;
    private int _itemCapacity = 2;

    private int _currentPlayer = 0;
    private float _lastChange;
    private Player[] _sortedPlayers;
    private bool _hadBallAtChange;

    #region Awake/Update

    private void Awake()
    {
        Brain = GetComponentInChildren<PlayerBrain>();
    }

    private void Update()
    {
        if (Field.Team2 == this)
            return;

        Player piloted = null;
        Player hasBall = null;

        for (int i = 0; i < Players.Length; ++i)
        {
            if (Players[i].IsPiloted)
                piloted = Players[i];

            if (Players[i].HasBall)
                hasBall = Players[i];
        }

        if (Goalkeeper.IsPiloted)
            piloted = Goalkeeper;

        if (Goalkeeper.HasBall)
            hasBall = Goalkeeper;

        if (Brain.Player != piloted)
            Brain.Player = piloted;

        if (hasBall && hasBall != piloted)
        {
            if (Brain.Player)
                Brain.Player.IsPiloted = false;

            Brain.Player = hasBall;

            Brain.Player.IsPiloted = true;
            CameraManager.Follow(Brain.Player.transform);
        }
    }

    #endregion

    public void ChangePlayer(Vector3 position)
    {
        bool hasBall = false;
        foreach (Player player in Players)
            if (player.HasBall)
                hasBall = true;

        if (_lastChange + 1.5f < Time.time || _sortedPlayers == null || hasBall != _hadBallAtChange)
        {
            if (hasBall)
            {
                _sortedPlayers = Players.OrderBy(p => Vector3.Distance(p.transform.position, position)).ToArray();
                _currentPlayer = 0;
            }
            else if (Field.Ball.transform.parent)
            {
                _sortedPlayers = Players.OrderBy(p => Vector3.Distance(p.transform.position, Field.Ball.transform.position) + (p.transform.position.x < Field.Ball.transform.position.x ? 0 : 90f)).ToArray();
                _currentPlayer = -1;
            }
            else
            {
                _sortedPlayers = Players.OrderBy(p => Vector3.Distance(p.transform.position, Field.Ball.transform.position)).ToArray();
                _currentPlayer = -1;
            }

            _hadBallAtChange = hasBall;
        }

        _lastChange = Time.time;

        _currentPlayer = (_currentPlayer + 1) % _sortedPlayers.Length;

        if (Brain.Player)
            Brain.Player.IsPiloted = false;

        Brain.Player = _sortedPlayers[_currentPlayer];

        Brain.Player.IsPiloted = true;
        CameraManager.Follow(Brain.Player.transform);
    }

    #region Not Used

    public Player GetBallOwner()
    {
        foreach (Player player in Players)
            if (player.HasBall)
                return player;

        if (Goalkeeper.HasBall)
            return Goalkeeper;

        return null;
    }

    #endregion

    public bool ArePlayersAllWaiting()
    {
        foreach (Player player in Players)
            if (!player.IsWaiting)
                return false;
        return true;
    }

    #region Items

    /// <summary>
    /// Add an item to the team's item queue, only if it's not full already
    /// </summary>
    public void GainItem()
    {
        if (_items.Count == _itemCapacity)
            return;
        ItemData data;
        do
        {
            List<ItemData> itemDatas = PrefabManager.Items;
            data = itemDatas[UnityEngine.Random.Range(0, itemDatas.Count)];
        } while (data.TeamHasToLose && GameManager.LosingTeam != this);
        _items.Enqueue(data);
        UIManager.UpdateItems(_items, this);
    }

    /// <summary>
    /// Delete the oldest item from the team item queue
    /// </summary>
    /// <returns>The gameObject corresponding to the deleted item</returns>
    public ItemData GetItem()
    {
        if (_items.Count == 0)
            return null;
        ItemData item = _items.Dequeue();
        UIManager.UpdateItems(_items, this);
        return item;
    }

    #endregion

    /// <summary>
    /// Initialize the team's players and item queue
    /// </summary>
    /// <param name="players">The non-goalkeeper players</param>
    /// <param name="goalkeeper">The goalkeeper</param>
    public void Init(Player[] players, Player goalkeeper)
    {
        Players = players;
        Goalkeeper = goalkeeper;

        _items = new Queue<ItemData>(_itemCapacity);

        Brains = Players.Select(player => player.IABrain).ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>())
        {
            GameManager.OnGoalScored(this);
        }
    }
}
