using UnityEngine;

public class BoltTouch : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private BoltMovement _bolt;
    [SerializeField] private Transform _transform;

    public void BoltButtonTouch()
    {
        if (_board.ReturneBolt() == null)
        {
            _board.CanUseAnyBolt();
            _bolt.CheckingActiveBolt();
            _board.BoltsCheking(gameObject, _transform);
        }
    }

    public void RemoveBoard()
    {
        _board = null;
    }

    public void SetBoard(Board NewBoard)
    {
        _board = NewBoard;
    }
}
