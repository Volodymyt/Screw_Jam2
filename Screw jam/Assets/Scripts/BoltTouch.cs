using System.Collections;
using UnityEngine;

public class BoltTouch : MonoBehaviour
{
    [SerializeField] private BoltGlobalScript _boltGlobalScript;
    [SerializeField] private GameObject _boardObject;
    [SerializeField] private Board _board;
    [SerializeField] private BoltMovement _bolt;
    [SerializeField] private Transform _transform;

    private void Start()
    {
        _boltGlobalScript = FindObjectOfType<BoltGlobalScript>();
    }

    public void BoltButtonTouch()
    {
        if (_boltGlobalScript.ReturnActiveBolt() == null)
        {
            _boltGlobalScript.SetBoltMoveActiveFalse();
            _board.CanUseAnyBolt();
            _bolt.CheckingActiveBolt();
            _board.BoltsCheking(gameObject, _transform);
            _boltGlobalScript.SetActiveBolt(gameObject);

            if (PlayerPrefs.GetInt("Vibrate") == 1)
            {
                Handheld.Vibrate();
            }
        }
        else
        {
            StartCoroutine(MoveBoltBack());

            if (PlayerPrefs.GetInt("Vibrate") == 1)
            {
                Handheld.Vibrate();
            }
        }
    }

    private IEnumerator MoveBoltBack()
    {
        _boltGlobalScript.ReturnActiveBolt().GetComponent<BoltMovement>().MoveBack();

        yield return new WaitForSeconds(0.1f);

        _boltGlobalScript.SetBoltMoveActiveFalse();
        _board.CanUseAnyBolt();
        _bolt.CheckingActiveBolt();
        _board.BoltsCheking(gameObject, _transform);
        _boltGlobalScript.SetActiveBolt(gameObject);
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
