using UnityEngine;

public class BoltTouch : MonoBehaviour
{
    [SerializeField] private BoltGlobalScript _boltGlobalScript;
    [SerializeField] private Board _board;
    [SerializeField] private BoltMovement _bolt;
    [SerializeField] private Transform _transform;

    private void Start()
    {
        _boltGlobalScript = FindObjectOfType<BoltGlobalScript>();
    }

    public void BoltButtonTouch()
    {
        if (_boltGlobalScript.CheckBolts() == true)
        {
            _boltGlobalScript.SetBoltMoveActiveFalse();
            _board.CanUseAnyBolt();
            _bolt.CheckingActiveBolt();
            _board.BoltsCheking(gameObject, _transform);

            if (PlayerPrefs.GetInt("Vibrate") == 1)
            {
                Handheld.Vibrate();
            }
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
