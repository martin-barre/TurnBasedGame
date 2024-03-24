using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private Grid _grid;

    public void Update()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cellPos = _grid.WorldToCell(pos);
        cellPos.z = 1;
        transform.position = _grid.GetCellCenterWorld(cellPos);
    }
}