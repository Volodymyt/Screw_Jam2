using UnityEngine;

public class BoltMovement : MonoBehaviour
{
    [SerializeField] private float _speed, _timeForMove, _screwingSpeed;
    [SerializeField] private Board _board;
    [SerializeField] private BoltController _controller;
    [SerializeField] private Transform _transform;
    [SerializeField] private BoltGlobalScript _boltGloblScript;
    [SerializeField] private bool _canMove = false, _canScrew = false, _can = false;

    private HolesChecking _activeHole;
    private CapsuleCollider _boltCollider;
    private Transform _centerOfRotation, _endOffset;
    private GameObject _meinCamera, _cube, _hole;
    private float updateInterval = 0.05f;
    private float timeElapsed = 0f;
    [SerializeField] private float lerpTime = 0f;

    private void Start()
    {
        _centerOfRotation = FindObjectOfType<CubeRotation>().transform;
        _boltGloblScript = FindObjectOfType<BoltGlobalScript>();

        _meinCamera = FindObjectOfType<Camera>().gameObject;
        _cube = FindObjectOfType<CubeRotation>().gameObject;

        _activeHole = FindObjectOfType<HolesChecking>();
    }

    private void Update()
    {
        if (_canMove && _activeHole.CheckHoles() != null)
        {
            _hole = _activeHole.CheckHoles();

            if (_hole.GetComponent<Hole>().CanScrewing() == true)
            {
                _can = true;
            }
            else
            {
                Debug.LogError("YOU CAN'T USE THIS HOLE!");
            }
        }

        if (_can)
        {
            if (_canScrew)
            {
                _transform.position = Vector3.MoveTowards(_transform.position, _hole.transform.position, _screwingSpeed * Time.deltaTime);

                _boltGloblScript.SetBoltMoveActiveFalse();
            }

            Movement(_hole);

            _controller.RemoveBoltFromList();

            if (_hole.GetComponent<Hole>().SetBoltInPanel() == true)
            {
                _transform.transform.SetParent(_meinCamera.transform);
            }
            else if (_hole.GetComponent<Hole>().SetBoltInCube() == true)
            {
                _transform.transform.SetParent(_cube.transform);
            }
            else if (_hole.GetComponent<Hole>().SetBoltInBoard() == true)
            {
                _board = _hole.transform.parent.gameObject.GetComponent<Board>();

                _transform.transform.SetParent(_board.transform.parent.gameObject.transform);

                _board.AddBolt(this.gameObject);
            }

            _controller.AddNewBolt();
        }
    }

    public void CheckingActiveBolt()
    {
        if (!_canMove)
        {
            _canMove = true;
        }
        else
        {
            _canMove = true;
        }
    }

    private void Movement(GameObject Hole)
    {

        if (_endOffset == null)
        {
            BackBoltCollider();
            _endOffset = Hole.GetComponent<Hole>().SetOffset();
        }

        lerpTime += _speed * Time.deltaTime;

        Vector3 StartOffset = _transform.position - _centerOfRotation.position;
        Vector3 EndOffsetVector = _endOffset.position - _centerOfRotation.position;

        Vector3 currentPosition = Vector3.Slerp(StartOffset, EndOffsetVector, lerpTime);

        _transform.position = _centerOfRotation.position + currentPosition;

        if (lerpTime >= 0.5f)
        {
            lerpTime = 0f;
        }

        _transform.rotation = Hole.transform.rotation;

        if (Vector3.Distance(_transform.position, EndOffsetVector) < 1.1f)
        {
            _canScrew = true;
            _canMove = false;
            lerpTime = 0f;
        }

        if (Vector3.Distance(_transform.position, Hole.transform.position) < 0.1f)
        {
            if (_canScrew == true && _activeHole.CheckHoles() == null)
            {
                _boltGloblScript.SetBoltMoveActiveTrue();
            }

            lerpTime = 0;
            _can = false;
            _canScrew = false;
            _controller.AddBoards();
            _endOffset = null;
        }
    }

    public void StayBoltCollider()
    {
        Vector3 newCenter;

        _boltCollider = gameObject.GetComponent<CapsuleCollider>();

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= updateInterval)
        {
            timeElapsed = 0f;

            newCenter = _boltCollider.center;
            newCenter.y += 0.28f;
            _boltCollider.center = newCenter;
        }
    }

    private void BackBoltCollider()
    {
        Vector3 newCenter;

        _boltCollider = gameObject.GetComponent<CapsuleCollider>();

        newCenter = _boltCollider.center;
        newCenter.y = 0;
        _boltCollider.center = newCenter;
    }

    public bool ReturneMove()
    {
        return _canMove;
    }
}
