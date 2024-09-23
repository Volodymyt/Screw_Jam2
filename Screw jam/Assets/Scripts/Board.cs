using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] _bolts;
    [SerializeField] private BoxCollider _boadrBoxCollider;
    [SerializeField] private Rigidbody _boardRigidbody;
    [SerializeField] private float _timeForMove, _moveSpeed;
    [SerializeField] private float _radius;
    [SerializeField] private bool _isMoving = true, _addBolt = false;
    [SerializeField] private Board _board;

    private UIOptions _UIOptions;
    private HingeJoint[] _hingeJoints;
    private Transform _boltMovePoint, _boltTransform;
    private HolesChecking _freeHoles;
    private GameObject _lastBolt, _boltToRemove;
    private int _boltsCount, _boltsCountOnTheStart;
    private int value = 2;
    private bool canUseBolt = false;

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
            if (_lastBolt != null && _boltToRemove == null)
            {
                _hingeJoints = _lastBolt.gameObject.GetComponents<HingeJoint>();

                foreach (HingeJoint HingeJoint in _hingeJoints)
                {
                    Destroy(HingeJoint);
                    _UIOptions.RecoundBourds();
                }
            }

            StartCoroutine(CheckBolts(_boltToRemove));

            canUseBolt = false;
        }
        else if (canUseBolt == true && _freeHoles.CheckHoles() == null && _isMoving)
        {
            FindEmptyObjectsInRadius();
            StartCoroutine(BoltMoveToPoint());
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
        if (_freeHoles.CheckHoles().GetComponent<Hole>().CanScrewing() == true)
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

            _boltToRemove = null;

            yield return new WaitForSeconds(_timeForMove);

            foreach (GameObject Bolt in _bolts)
            {
                if (Bolt == null)
                {
                    _boltsCount--;
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

    public GameObject ReturneBolt()
    {
        return _boltToRemove;
    }

    private IEnumerator BoltMoveToPoint()
    {
        int activeBolts = 0;

        _boltToRemove.GetComponent<BoltMovement>().StayBoltCollider();

        _boltTransform.position = Vector3.MoveTowards(_boltTransform.position, _boltMovePoint.position, _moveSpeed * Time.deltaTime);

        foreach (GameObject bolt in _bolts)
        {
            if (bolt != null)
            {
                activeBolts++;
            }
        }

        if (_lastBolt != null && _lastBolt == _boltToRemove)
        {
            if (activeBolts == 1)
            {
                _lastBolt.GetComponent<BoltController>().AdjustThePositionOfAnchor();
            }
        }

        if (Vector3.Distance(_boltTransform.position, _boltMovePoint.position) <= 0.0000001f)
        {
            _isMoving = false;
        }

        yield return null;
    }
}