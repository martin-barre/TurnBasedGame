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
        MapManager.Instance.ActiveTilemapSpawns(false);
        NextPlayer();
        BtnNextUI.OnBtnNextClick += OnClickBtnNext;
        SpellBarUI.OnBtnSpellClick += OnClickBtnSpell;
    }

    public override void ExitState()
    {
        BtnNextUI.OnBtnNextClick -= OnClickBtnNext;
        SpellBarUI.OnBtnSpellClick -= OnClickBtnSpell;
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Node node = MapManager.Instance.WorldPositionToMapNodes(mousePosition);

        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedSpell != null) // SI JE SUIS EN TRAIN DE LANCER UN SORT
            {
                if (activeNodes.Contains(node)) // SI JE CLICK SUR UNE CASE ACTIVE -> LANCE LE SORT
                {
                    selectedSpell.Launch(currentEntity, selectedSpell, node.gridPosition);
                    currentEntity.CurrentPa -= selectedSpell.paCost;
                    selectedSpell = null;
                    MapManager.Instance.ClearOverlay1();
                }
                InitMovementState();
            }
            else // SI JE SUIS EN MODE DEPLACEMENT
            {
                if (activeNodes.Contains(node)) // SI JE CLICK SUR UNE CASE ACTIVE -> DEPLACE LE JOUEUR
                {
                    List<Node> path = Dijkstra.GetPath(currentEntity, currentEntity.CurrentPm, node);
                    currentEntity.CurrentPm -= path.Count;
                    currentEntity.SetPath(path);

                    MapManager.Instance.MoveEntity(currentEntity, node);

                    InitMovementState();
                }
            }
        }

        MapManager.Instance.ClearOverlay2();
        if (activeNodes.Contains(node))
        {
            if (selectedSpell != null)
            {
                foreach (Node tmpNode in selectedSpell.GetZoneNodes(currentEntity, node))
                {
                    if (tmpNode.type == NodeType.GROUND)
                    {
                        MapManager.Instance.AddOverlay2(tmpNode);
                    }
                }
            }
            else
            {
                foreach (Node tmp in Dijkstra.GetPath(currentEntity, currentEntity.CurrentPm, node))
                {
                    MapManager.Instance.AddOverlay2(tmp);
                }
            }
        }
    }

    public void OnClickBtnNext()
    {
        NextPlayer();
    }

    public void OnClickBtnSpell(int spellIndex)
    {
        selectedSpell = spellIndex < currentEntity.race.spells.Count
            ? currentEntity.race.spells[spellIndex]
            : null;

        if (selectedSpell == null) return;
        if (currentEntity.CurrentPa < selectedSpell.paCost) return;

        activeNodes = FOV.GetDisplacement(currentEntity, selectedSpell);
        MapManager.Instance.ClearOverlay1();
        foreach (Node node in activeNodes)
        {
            MapManager.Instance.AddOverlay1(node);
        }
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
        selectedSpell = null;
        activeNodes = Dijkstra.GetDisplacement(currentEntity, currentEntity.CurrentPm);
        MapManager.Instance.ClearOverlay1();
        foreach (Node node in activeNodes)
        {
            MapManager.Instance.AddOverlay1(node);
        }
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