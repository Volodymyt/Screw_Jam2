using System.Collections.Generic;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    [SerializeField] private Board[] _boards;
    [SerializeField] private Transform _startOfBolt, _endOfBolt;

    private List<Board> _boardsList = new List<Board>();

    private void Start()
    {
        for (int i = 0; i < AddBoards().Length; i++)
        {
            AddBoards()[i].AddBolt(this.gameObject);
        }
    }

    public Board[] AddBoards()
    {
        RaycastHit[] Boards = Physics.RaycastAll(_startOfBolt.position, _endOfBolt.position - _startOfBolt.position, Vector3.Distance(_startOfBolt.position, _endOfBolt.position));

        _boardsList.Clear();

        for (int i = 0; i < Boards.Length; i++)
        {
            Board BoardComponent = Boards[i].collider.gameObject.GetComponent<Board>();

            if (BoardComponent != null)
            {
                _boardsList.Add(BoardComponent);
            }
        }

        _boards = _boardsList.ToArray();

        return _boards;
    }
}
