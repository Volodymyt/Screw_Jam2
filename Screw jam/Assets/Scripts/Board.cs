using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] _bolts, _holes, _boards;
    [SerializeField] private BoxCollider _boadrBoxCollider;
    [SerializeField] private Rigidbody _boardRigidbody;
    [SerializeField] private float _timeForMove, _moveSpeed;
    [SerializeField] private float _radius;
    [SerializeField] private bool _isMoving = true, _addBolt = false, _newHole = true;
    [SerializeField] private Board _board;
    [SerializeField] private bool _canRemoveBoard = true, _can = false;
    [SerializeField] private int _boardsCount = 0;


    private UIOptions _UIOptions;
    private HingeJoint[] _hingeJoints;
    private Transform _boltMovePoint, _boltTransform;
    private HolesChecking _freeHoles;
    private GameObject _lastBolt, _boltToRemove;
    private int _boltsCount, _boltsCountOnTheStart;
    private int value = 2;
    private bool canUseBolt = false;
    private bool _canRemoveHinge = true;

    private void Start()
    {
        _UIOptions = FindObjectOfType<UIOptions>();
        _freeHoles = FindObjectOfType<HolesChecking>();

        for (int i = 0; i < _bolts.Length; i++)
        {
            _boltsCountOnTheStart = i + 1;
        }

        _boltsCount = _boltsCountOnTheStart;
    }

    private void Update()
    {
        if (canUseBolt == true && _freeHoles.CheckHoles() != null)
        {
            StartCoroutine(CheckBolts(_boltToRemove));



            canUseBolt = false;
        }
        else if (canUseBolt == true && _freeHoles.CheckHoles() == null && _isMoving)
        {
            FindEmptyObjectsInRadius();
            BoltMoveToPoint();
        }
    }

    public void AddBolt(GameObject NewBolt)
    {
        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] == NewBolt)
            {
                _addBolt = true;
                break;
            }
            else if (_bolts[i] != NewBolt && i == value)
            {
                for (int j = 0; j < _bolts.Length; j++)
                {
                    if (_bolts[j] == null)
                    {
                        _bolts[j] = NewBolt;
                        break;
                    }
                }
            }
        }
    }

    public void CheckBoltToRemove(GameObject BoltToRemove)
    {
        StartCoroutine(CheckBolts(BoltToRemove));
    }

    private IEnumerator CheckBolts(GameObject BoltToRemove)
    {
        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] == BoltToRemove)
            {
                if (_addBolt || !_addBolt || _freeHoles.CheckHoles().GetComponent<Hole>().SetBoltInCube() || _freeHoles.CheckHoles().GetComponent<Hole>().SetBoltInBoard())
                {
                    _bolts[i] = null;
                }
            }
        }

        if (_freeHoles.CheckHoles() != null)
        {
            if (_lastBolt != null && _boltToRemove == null)
            {
                _hingeJoints = _lastBolt.gameObject.GetComponents<HingeJoint>();

                GameObject checkedHole = _freeHoles.CheckHoles();

                for (int i = 0; i < _holes.Length; i++)
                {
                    if (_holes[i] != checkedHole)
                    {
                        _newHole = false;
                        _holes[i].GetComponent<Hole>().SetOldHole();
                        _boards = checkedHole.GetComponent<Hole>().GetSameBoards();
                    }
                }

                if (_canRemoveHinge == true)
                {
                    for (int i = 0; i < _boards.Length; i++)
                    {
                        if (_boards[i].GetComponent<Board>().ReturnHole() != true)
                        {
                            _boardsCount++;
                            _boards[i].GetComponent<Board>().DoThat();
                        }
                    }

                    _canRemoveHinge = false;
                }

                if (_can == false)
                {
                    foreach (HingeJoint HingeJoint in _hingeJoints)
                    {
                        Destroy(HingeJoint);

                        if (_canRemoveBoard)
                        {
                            _UIOptions.RecoundBourds(1);
                            _canRemoveBoard = false;
                        }
                    }
                }
                else
                {
                    _lastBolt.GetComponent<BoltController>().AdjustThePositionOfAnchor();
                }
            }

            if (_freeHoles.CheckHoles().GetComponent<Hole>().CanScrewing() == true)
            {
                _boltToRemove = null;

                yield return new WaitForSeconds(_timeForMove);

                _newHole = true;
                _canRemoveHinge = true;
                _boardsCount = 0;
                _can = false;

                foreach (GameObject Bolt in _bolts)
                {
                    if (Bolt == null)
                    {
                        if (_boltsCount >= 0)
                        {
                            _boltsCount--;
                        }
                    }
                    else
                    {
                        _lastBolt = Bolt;
                    }
                }

                if (_boltsCount == 1)
                {
                    AddPhysic();
                }
                else if (_boltsCount > 1)
                {
                    _boltsCount = _boltsCountOnTheStart;
                }
                else if (_boltsCount < 1)
                {
                    _lastBolt.GetComponent<BoltTouch>().SetBoard(_board);
                }

                _addBolt = false;
            }
        }
    }

    private void AddPhysic()
    {
        _lastBolt.GetComponent<BoltController>().AddAnchors(_boardRigidbody);
        _boardRigidbody.constraints = RigidbodyConstraints.None;
        _boardRigidbody.freezeRotation = false;

        Vector3 center = _boadrBoxCollider.center;
        center.x += 0.01f;
        _boadrBoxCollider.center = center;

        _boadrBoxCollider.enabled = true;
        _boardRigidbody.useGravity = true;

        _lastBolt.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void FindEmptyObjectsInRadius()
    {
        Vector3 center = _boltToRemove.GetComponent<BoltController>().GetTransform().position;

        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Hole");

        Transform closestTransform = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in allObjects)
        {
            float distance = Vector3.Distance(center, obj.transform.position);

            if (distance < _radius)
            {
                if (distance < closestDistance)
                {
                    closestDistance = Vector3.Distance(center, obj.GetComponent<Hole>().SetOffset().transform.position);
                    closestTransform = obj.GetComponent<Hole>().SetOffset().transform;
                }
            }
        }

        if (closestTransform != null)
        {
            _boltMovePoint = closestTransform;
        }
    }

    public void BoltsCheking(GameObject BoltToRemove, Transform BoltTransform)
    {
        _boltToRemove = BoltToRemove;
        _boltTransform = BoltTransform;
        _isMoving = true;
    }

    public void CanUseAnyBolt()
    {
        canUseBolt = true;
    }

    public void DoThat()
    {
        _can = true;
    }

    public GameObject ReturneBolt()
    {
        return _boltToRemove;
    }

    public void SetOldHole()
    {
        _newHole = false;
    }

    public bool ReturnHole()
    {
        return _newHole;
    }

    private void BoltMoveToPoint()
    {
        _boltToRemove.GetComponent<BoltMovement>().StayBoltCollider();

        if (Vector3.Distance(_boltTransform.position, _boltMovePoint.position) <= 0.2f)
        {
            _isMoving = false;
        }
        else
        {
            _boltTransform.position = Vector3.MoveTowards(_boltTransform.position, _boltMovePoint.position, _moveSpeed * Time.deltaTime);
        }

        if (_lastBolt != null)
        {
            _lastBolt.GetComponent<BoltController>().AdjustThePositionOfAnchor();
        }
    }
}