using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStateBattle
{

    private Entity currentEntity;
    private int currentPlayerIndex = -1;
    private Spell selectedSpell;

    private List<Node> activeNodes;

    public void Start()
    {
        MapManager.Instance.ActiveTilemapSpawns(false);
        NextPlayer();
    }

    public void Update()
    {
        if (CheckEndGame())
        {
            GameManager.Instance.SetGameState(GameState.END);
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Node node = MapManager.Instance.WorldPositionToMapNodes(mousePosition);

        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedSpell != null) // SI JE SUIS EN TRAIN DE LANCER UN SORT
            {
                if (activeNodes.Contains(node)) // SI JE CLICK SUR UNE CASE ACTIVE -> LANCE LE SORT
                {
                    selectedSpell.Launch(currentEntity, selectedSpell, node.gridPosition);
                    currentEntity.currentPa -= selectedSpell.paCost;
                    selectedSpell = null;
                    MapManager.Instance.ClearOverlay1();
                }
                InitMovementState();
            }
            else // SI JE SUIS EN MODE DEPLACEMENT
            {
                if (activeNodes.Contains(node)) // SI JE CLICK SUR UNE CASE ACTIVE -> DEPLACE LE JOUEUR
                {
                    List<Node> path = Dijkstra.GetPath(currentEntity, currentEntity.currentPm, node);
                    currentEntity.currentPm -= path.Count;
                    currentEntity.SetPath(path);

                    MapManager.Instance.MoveEntity(currentEntity, node);

                    InitMovementState();
                }
            }

            UIManager.Instance.SetSpellBar(currentEntity);
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
                foreach (Node tmp in Dijkstra.GetPath(currentEntity, currentEntity.currentPm, node))
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
        if (currentEntity.currentPa < selectedSpell.paCost) return;

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
            currentEntity.currentPm = currentEntity.race.pm;
            currentEntity.currentPa = currentEntity.race.pa;
        }

        currentPlayerIndex++;
        if (currentPlayerIndex >= GameManager.Instance.GetEntities().Count) currentPlayerIndex = 0;

        currentEntity = GameManager.Instance.GetEntities()[currentPlayerIndex];
        if (currentEntity.IsDead()) NextPlayer();
        selectedSpell = null;

        UIManager.Instance.SetInfoMessage("Player : " + (currentPlayerIndex + 1));
        UIManager.Instance.SetSpellBar(currentEntity);

        InitMovementState();
    }

    private void InitMovementState()
    {
        selectedSpell = null;
        activeNodes = Dijkstra.GetDisplacement(currentEntity, currentEntity.currentPm);
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
        if (!hasBlue && !hasRed) return true;
        else if (hasBlue && !hasRed) return true;
        else if (!hasBlue && hasRed) return true;

        return false;
    }

}
