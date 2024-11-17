using System.Collections;
using UnityEngine;

public class BoltTouch : MonoBehaviour
{
    [SerializeField] private BoltGlobalScript _boltGlobalScript;
    [SerializeField] private GameObject _boardObject;
    [SerializeField] private Board _board;
    [SerializeField] private BoltMovement _bolt;
    [SerializeField] private Transform _transform;

    private bool canClick = true;

    private void Start()
    {
        _boltGlobalScript = FindObjectOfType<BoltGlobalScript>();
    }


    public void BoltButtonTouch()
    {
        if (!canClick)
        {
            return;
        }

        if (!_board.ReturnDid())
        {
            return;
        }

        if (!_boltGlobalScript.CheckNextBoltMovement())
        {
            return;
        }

        StartCoroutine(ClickCooldown());

        if (_boltGlobalScript.ReturnActiveBolt() == null)
        {
            _boltGlobalScript.SetBoltMoveActiveFalse();
            _board.CanUseAnyBolt();
            _bolt.CheckingActiveBolt();
            _board.BoltsCheking(gameObject, _transform);
            _boltGlobalScript.SetActiveBolt(gameObject);
        }
        else if (_boltGlobalScript.ReturnActiveBolt() == gameObject && _bolt.ReturnCanMove() == true)
        {
            _boltGlobalScript.ReturnActiveBolt().GetComponent<BoltMovement>().MoveBack(true);
            _boltGlobalScript.SetActiveBolt(null);
            _bolt.SetCanMove(false);
            // _board.SetCan(false);
        }
        else
        {
            StartCoroutine(MoveBoltBack());
        }

        if (PlayerPrefs.GetInt("Vibrate") == 1)
        {
            Handheld.Vibrate();
        }
    }

    private IEnumerator ClickCooldown()
    {
        canClick = false;
        yield return new WaitForSeconds(0.5f);
        canClick = true;
    }

    private IEnumerator MoveBoltBack()
    {
        yield return new WaitForSeconds(0.0f);

        _boltGlobalScript.ReturnActiveBolt().GetComponent<BoltMovement>().MoveBack(false);

        _boltGlobalScript.SetBoltMoveActiveFalse();
        _board.CanUseAnyBolt();
        _boltGlobalScript.SetActiveBolt(gameObject);
        _board.BoltsCheking(gameObject, _transform);
        _bolt.CheckingActiveBolt();
    }

    public Board ReturnBoard()
    {
        return _board;
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
