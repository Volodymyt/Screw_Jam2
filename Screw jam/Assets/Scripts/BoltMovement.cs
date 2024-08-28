using System.Collections;
using UnityEngine;

public class BoltMovement : MonoBehaviour
{
    [SerializeField] private float _speed, _timeForMove, _screwingSpeed;
    [SerializeField] private Board _board;
    [SerializeField] private BoltController _controller;
    [SerializeField] private Transform _transform;

    private HolesChecking _activeHole;
    private Transform _centerOfRotation;
    private GameObject _meinCamera, _cube;
    private bool _canMove = false, _canScrew = false;
    private float lerpTime = 0f;

    private void Start()
    {
        _centerOfRotation = FindObjectOfType<CubeRotation>().transform;

        _meinCamera = FindObjectOfType<Camera>().gameObject;
        _cube = FindObjectOfType<CubeRotation>().gameObject;

        _activeHole = FindObjectOfType<HolesChecking>();
    }

    private void Update()
    {
        if (_canMove && _activeHole.CheckHoles() != null)
        {
            GameObject Hole = _activeHole.CheckHoles();

            if (Hole.GetComponent<Hole>().CanScrewing() == true)
            {
                if (_canScrew)
                {
                    _transform.position = Vector3.MoveTowards(_transform.position, Hole.transform.position, _screwingSpeed * Time.deltaTime);
                }

                Movement(Hole);

                _controller.RemoveBoltFromList();

                if (Hole.GetComponent<Hole>().SetBoltInPanel() == true)
                {
                    _transform.transform.SetParent(_meinCamera.transform);
                }
                else if (Hole.GetComponent<Hole>().SetBoltInCube() == true)
                {
                    _transform.transform.SetParent(_cube.transform);
                }
                else if (Hole.GetComponent<Hole>().SetBoltInBoard() == true)
                {
                    _board = Hole.transform.parent.gameObject.GetComponent<Board>();

                    _transform.transform.SetParent(_board.transform.parent.gameObject.transform);

                    _board.AddBolt(this.gameObject);
                }

                StartCoroutine(SetBoltActiveFalse());
            }
            else
            {
                Debug.LogError("YOU CAN'T USE THIS HOLE!");
            }
        }
    }

    public void CheckingActiveBolt()
    {
        if (!_canMove)
        {
            _canMove = true;
        }
    }

    private void Movement(GameObject Hole)
    {
        Transform EndOffset = Hole.GetComponent<Hole>().SetOffset();

        lerpTime += _speed * Time.deltaTime;

        Vector3 StartOffset = _transform.position - _centerOfRotation.position;
        Vector3 EndOffsetVector = EndOffset.position - _centerOfRotation.position;

        Vector3 currentPosition = Vector3.Slerp(StartOffset, EndOffsetVector, lerpTime);

        _transform.position = _centerOfRotation.position + currentPosition;

        if (lerpTime >= 1f)
        {
            lerpTime = 1f;
        }

        _transform.rotation = Hole.transform.rotation;

        if (Vector3.Distance(_transform.position, EndOffsetVector) < 0.1f)
        {
            _canScrew = true;
        }
    }

    private IEnumerator SetBoltActiveFalse()
    {
        _controller.AddNewBolt();

        yield return new WaitForSeconds(_timeForMove);

        _canMove = false;
        _canScrew = false;
        _controller.AddBoards();
    }
}
