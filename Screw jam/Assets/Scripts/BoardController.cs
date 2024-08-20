using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private Board _board;

    public Board RetundBoard()
    {
        return _board;
    }

    public GameObject ReturnBoardObject()
    {
        GameObject Board = _board.gameObject;

        return Board;
    }
}
