using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleState : BaseState<GameStateMachine.GameState>
{
    private Entity CurrentEntity => GameManager.Instance.GetEntities()[GameManager.Instance.CurrentPlayerIndex.Value];

    private Spell selectedSpell;

    private List<Node> activeNodes = new();

    public BattleState(GameStateMachine.GameState key) : base(key) { }

    public override void EnterState()
    {
        MapManager.Instance.ActiveTilemapSpawns(false);
        BtnNextUI.OnBtnNextClick += OnClickBtnNext;
        SpellBarUI.OnBtnSpellClick += OnClickBtnSpell;
        GameCommand.OnSpellLaunch += OnSpellLaunch;
        GameCommand.OnMoveClick += OnMoveClick;

        GameManager.Instance.CurrentPlayerIndex.OnValueChanged += OnCurrentPlayerIndexChanged;
        OnCurrentPlayerIndexChanged(0, GameManager.Instance.CurrentPlayerIndex.Value);

        GameCommand.OnNextTurn += NextPlayer;
    }

    public override void ExitState()
    {
        BtnNextUI.OnBtnNextClick -= OnClickBtnNext;
        SpellBarUI.OnBtnSpellClick -= OnClickBtnSpell;
        GameCommand.OnSpellLaunch -= OnSpellLaunch;
        GameCommand.OnMoveClick -= OnMoveClick;
        GameCommand.OnNextTurn -= NextPlayer;

        GameManager.Instance.CurrentPlayerIndex.OnValueChanged -= OnCurrentPlayerIndexChanged;

        MapManager.Instance.ClearOverlay1();
        MapManager.Instance.ClearOverlay2();
    }

    public override void UpdateState()
    {
        if (CurrentEntity == null || CurrentEntity.data.Team != GameManager.Instance.CurrentTeam) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Node node = MapManager.Instance.WorldPositionToMapNodes(mousePosition);

        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedSpell != null) // SI JE SUIS EN TRAIN DE LANCER UN SORT
            {
                if (activeNodes.Contains(node)) // SI JE CLICK SUR UNE CASE ACTIVE -> LANCE LE SORT
                {
                    GameCommand.Instance.SendLaunchSpellEvent(selectedSpell.id, node.gridPosition);
                }
                else
                {
                    InitMovementState();
                    selectedSpell = null;
                }
            }
            else // SI JE SUIS EN MODE DEPLACEMENT
            {
                if (activeNodes.Contains(node)) // SI JE CLICK SUR UNE CASE ACTIVE -> DEPLACE LE JOUEUR
                {
                    GameCommand.Instance.SendMoveClickEvent(node.gridPosition);
                }
            }
        }

        MapManager.Instance.ClearOverlay2();
        if (activeNodes.Contains(node))
        {
            if (selectedSpell != null)
            {
                var zone = selectedSpell.GetZoneNodes(CurrentEntity, node);
                MapManager.Instance.AddOverlay2(zone);
            }
            else
            {
                var path = Dijkstra.GetPath(CurrentEntity, CurrentEntity.CurrentPm, node);
                MapManager.Instance.AddOverlay2(path);
            }
        }
    }

    public void OnClickBtnNext()
    {
        if (CurrentEntity == null || CurrentEntity.data.Team != GameManager.Instance.CurrentTeam) return;
        GameCommand.Instance.SendNextTurnEvent();
    }

    private void OnSpellLaunch(int spellId, Vector2Int cellPos)
    {
        var spell = CurrentEntity.Race.Spells.Find(o => o.id == spellId);
        spell.Launch(CurrentEntity, spell, cellPos);
        CurrentEntity.CurrentPa -= spell.paCost;
        InitMovementState();
    }

    private void OnMoveClick(Vector2Int cellPos)
    {
        var node = MapManager.Instance.GetNode(cellPos);
        List<Node> path = Dijkstra.GetPath(CurrentEntity, CurrentEntity.CurrentPm, node);
        CurrentEntity.CurrentPm -= path.Count;
        CurrentEntity.SetPath(path);
        MapManager.Instance.MoveEntity(CurrentEntity, node);
        InitMovementState();
    }

    public void OnClickBtnSpell(int spellIndex)
    {
        if (CurrentEntity == null || CurrentEntity.data.Team != GameManager.Instance.CurrentTeam) return;

        selectedSpell = spellIndex < CurrentEntity.Race.Spells.Count
            ? CurrentEntity.Race.Spells[spellIndex]
            : null;

        if (selectedSpell == null) return;
        if (CurrentEntity.CurrentPa < selectedSpell.paCost) return;

        activeNodes = FOV.GetDisplacement(CurrentEntity, selectedSpell);
        MapManager.Instance.ClearOverlay1();
        MapManager.Instance.AddOverlay1(activeNodes);
    }

    private void NextPlayer()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (CheckEndGame()) return;

        if (CurrentEntity != null)
        {
            CurrentEntity.CurrentPm = CurrentEntity.Race.Pm;
            CurrentEntity.CurrentPa = CurrentEntity.Race.Pa;
        }

        int nextPlayerIndex = GameManager.Instance.CurrentPlayerIndex.Value + 1;
        if (nextPlayerIndex >= GameManager.Instance.GetEntities().Count) nextPlayerIndex = 0;

        while (CurrentEntity.IsDead())
        {
            nextPlayerIndex += 1;
            if (nextPlayerIndex >= GameManager.Instance.GetEntities().Count) nextPlayerIndex = 0;
        }

        GameManager.Instance.CurrentPlayerIndex.Value = nextPlayerIndex;
    }

    private void InitMovementState()
    {
        MapManager.Instance.ClearOverlay1();
        MapManager.Instance.ClearOverlay2();

        if (CurrentEntity == null || CurrentEntity.data.Team != GameManager.Instance.CurrentTeam) return;

        selectedSpell = null;
        activeNodes = Dijkstra.GetDisplacement(CurrentEntity, CurrentEntity.CurrentPm);
        MapManager.Instance.AddOverlay1(activeNodes);
    }

    private bool CheckEndGame()
    {
        bool hasBlue = GameManager.Instance.GetEntities().Any(e => !e.IsDead() && e.data.Team == Team.BLUE);
        bool hasRed = GameManager.Instance.GetEntities().Any(e => !e.IsDead() && e.data.Team == Team.RED);

        if (!hasBlue || !hasRed)
        {
            GameStateMachine.Instance.StateEnum.Value = GameStateMachine.GameState.End;
        }

        return !hasBlue || !hasRed;
    }

    private void OnCurrentPlayerIndexChanged(int oldIndex = default, int newIndex = default)
    {
        if (CurrentEntity != null)
        {
            CurrentEntity.CurrentPm = CurrentEntity.Race.Pm;
            CurrentEntity.CurrentPa = CurrentEntity.Race.Pa;
        }

        selectedSpell = null;
        InitMovementState();
    }
}