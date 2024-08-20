using System.Collections;
using UnityEngine;

public class BoltMovement : MonoBehaviour
{
    [SerializeField] private float _speed, _timeForMove, _screwingSpeed;

    [SerializeField] private Board _board;
    private HolesChecking _activeHole;
    private Transform _startOffset, _centerOfRotation;
    private GameObject _meinCamera, _cube;
    private bool _canMove = false, _canScrew = false;
    private float lerpTime = 0f;

    private void Start()
    {
        _startOffset = this.transform;

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

            Movement(Hole);

            if (_canScrew)
            {
                transform.position = Vector3.MoveTowards(transform.position, Hole.transform.position, _screwingSpeed * Time.deltaTime);
            }

            if (Hole.GetComponent<Hole>().SetBoltInPanel() == true)
            {
                this.gameObject.transform.SetParent(_meinCamera.transform);
            }
            else if (Hole.GetComponent<Hole>().SetBoltInCube() == true)
            {
                this.gameObject.transform.SetParent(_cube.transform);
            }
            else if (Hole.GetComponent<Hole>().SetBoltInBoard() == true)
            {
                _board = Hole.transform.parent.gameObject.GetComponent<Board>();

                this.gameObject.transform.SetParent(_board.transform.parent.gameObject.transform);

                _board.AddBolt(this.gameObject);
            }

            StartCoroutine(SetBoltActiveFalse());
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

        Vector3 StartOffset = _startOffset.position - _centerOfRotation.position;
        Vector3 EndOffsetVector = EndOffset.position - _centerOfRotation.position;

        Vector3 currentPosition = Vector3.Slerp(StartOffset, EndOffsetVector, lerpTime);

        transform.position = _centerOfRotation.position + currentPosition;

        if (lerpTime >= 1.0f)
        {
            lerpTime = 1.0f;
        }

        transform.rotation = Hole.transform.rotation;

        if (Vector3.Distance(transform.position, EndOffset.position) < 0.1f)
        {
            _canScrew = true;
        }
    }

    private IEnumerator SetBoltActiveFalse()
    {
        yield return new WaitForSeconds(_timeForMove);

        _canMove = false;
        _canScrew = false;
    }
}
