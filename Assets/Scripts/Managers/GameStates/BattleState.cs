using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleState : BaseState<GameStateMachine.GameState>
{
    public static event Action<Entity> OnPlayerChanged;
    public static event Action<int> OnPlayerIndexChanged;

    private Entity currentEntity;
    private int currentPlayerIndex = -1;
    private Spell selectedSpell;

    private List<Node> activeNodes = new();

    public BattleState(GameStateMachine.GameState key) : base(key) { }

    public override void EnterState()
    {
        BtnNextUI.OnBtnNextClick += OnClickBtnNext;
        SpellBarUI.OnBtnSpellClick += OnClickBtnSpell;
        GameCommand.OnSpellLaunch += OnSpellLaunch;
        GameCommand.OnMoveClick += OnMoveClick;
        GameCommand.OnNextTurn += NextPlayer;

        NextPlayer();
    }

    public override void ExitState()
    {
        BtnNextUI.OnBtnNextClick -= OnClickBtnNext;
        SpellBarUI.OnBtnSpellClick -= OnClickBtnSpell;
        GameCommand.OnSpellLaunch -= OnSpellLaunch;
        GameCommand.OnMoveClick -= OnMoveClick;
        GameCommand.OnNextTurn -= NextPlayer;


        MapManager.Instance.ClearOverlay1();
        MapManager.Instance.ClearOverlay2();
    }

    public override GameStateMachine.GameState GetNextState()
    {
        if (CheckEndGame())
        {
            return GameStateMachine.GameState.End;
        }
        else
        {
            return StateKey;
        }
    }

    public override void UpdateState()
    {
        if (currentEntity == null || currentEntity.team != GameManager.Instance.CurrentTeam) return;

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
                var zone = selectedSpell.GetZoneNodes(currentEntity, node);
                MapManager.Instance.AddOverlay2(zone);
            }
            else
            {
                var path = Dijkstra.GetPath(currentEntity, currentEntity.CurrentPm, node);
                MapManager.Instance.AddOverlay2(path);
            }
        }
    }

    public void OnClickBtnNext()
    {
        if (currentEntity == null || currentEntity.team != GameManager.Instance.CurrentTeam) return;
        GameCommand.Instance.SendNextTurnEvent();
    }

    private void OnSpellLaunch(int spellId, Vector2Int cellPos)
    {
        var spell = currentEntity.race.spells.Find(o => o.id == spellId);
        spell.Launch(currentEntity, spell, cellPos);
        currentEntity.CurrentPa -= spell.paCost;
        InitMovementState();
    }

    private void OnMoveClick(Vector2Int cellPos)
    {
        var node = MapManager.Instance.GetNode(cellPos);
        List<Node> path = Dijkstra.GetPath(currentEntity, currentEntity.CurrentPm, node);
        currentEntity.CurrentPm -= path.Count;
        currentEntity.SetPath(path);
        MapManager.Instance.MoveEntity(currentEntity, node);
        InitMovementState();
    }

    public void OnClickBtnSpell(int spellIndex)
    {
        if (currentEntity == null || currentEntity.team != GameManager.Instance.CurrentTeam) return;

        selectedSpell = spellIndex < currentEntity.race.spells.Count
            ? currentEntity.race.spells[spellIndex]
            : null;

        if (selectedSpell == null) return;
        if (currentEntity.CurrentPa < selectedSpell.paCost) return;

        activeNodes = FOV.GetDisplacement(currentEntity, selectedSpell);
        MapManager.Instance.ClearOverlay1();
        MapManager.Instance.AddOverlay1(activeNodes);
    }

    private void NextPlayer()
    {
        if (CheckEndGame()) return;

        if (currentEntity != null)
        {
            currentEntity.CurrentPm = currentEntity.race.pm;
            currentEntity.CurrentPa = currentEntity.race.pa;
        }

        currentPlayerIndex++;
        if (currentPlayerIndex >= GameManager.Instance.GetEntities().Count) currentPlayerIndex = 0;

        currentEntity = GameManager.Instance.GetEntities()[currentPlayerIndex];
        if (currentEntity.IsDead()) NextPlayer();
        selectedSpell = null;

        OnPlayerChanged?.Invoke(currentEntity);
        OnPlayerIndexChanged?.Invoke(currentPlayerIndex);

        InitMovementState();
    }

    private void InitMovementState()
    {
        MapManager.Instance.ClearOverlay1();
        MapManager.Instance.ClearOverlay2();

        if (currentEntity == null || currentEntity.team != GameManager.Instance.CurrentTeam) return;

        selectedSpell = null;
        activeNodes = Dijkstra.GetDisplacement(currentEntity, currentEntity.CurrentPm);
        MapManager.Instance.AddOverlay1(activeNodes);
    }

    private bool CheckEndGame()
    {
        bool hasBlue = false;
        bool hasRed = false;
        foreach (Entity entity in GameManager.Instance.GetEntities())
        {
            if (!entity.IsDead() && entity.team == Team.BLUE) hasBlue = true;
            if (!entity.IsDead() && entity.team == Team.RED) hasRed = true;
        }
        return !hasBlue || !hasRed;
    }
}