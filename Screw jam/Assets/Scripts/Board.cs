using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] _bolts, _holes, _boards;
    [SerializeField] private BoxCollider _boadrBoxCollider;
    [SerializeField] private BoltGlobalScript _boltGlobalScript;
    [SerializeField] private Rigidbody _boardRigidbody;
    [SerializeField] private float _timeForMove, _moveSpeed;
    [SerializeField] private float _radius;
    [SerializeField] private bool _isMoving = true, _addBolt = false, _newHole = true;
    [SerializeField] private Board _board;
    [SerializeField] private bool _canRemoveBoard = true, _haveChangedBolt = false;
    [SerializeField] private bool _can = true, _boltScrewing = false;
    [SerializeField] private GameObject _changedBolt;

    private UIOptions _UIOptions;
    private HingeJoint[] _hingeJoints;
    [SerializeField] private Transform _boltMovePoint, _boltTransform, _closestTransform;
    private HolesChecking _freeHoles;
    private GameObject _lastBolt, _boltToRemove;
    private int _boltsCount, _boltsCountOnTheStart;
    private int value = 2;
    private bool canUseBolt = false, _deadBoard = false;
    private bool _canRemoveHinge = true;


    private void Start()
    {
        _UIOptions = FindObjectOfType<UIOptions>();
        _freeHoles = FindObjectOfType<HolesChecking>();
        _boltGlobalScript = FindObjectOfType<BoltGlobalScript>();

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
            _boltGlobalScript.SetClickOnHole(false);
        }
        else if (canUseBolt == true && _freeHoles.CheckHoles() == null && _isMoving)
        {
           // Debug.Log(gameObject.name);
            StartCoroutine(FindHole());
        }
    }

    public void AddBolt(GameObject NewBolt)
    {
        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] == NewBolt)
            {
                _addBolt = false;
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

    private IEnumerator OnPhysic()
    {
        yield return new WaitForSeconds(_timeForMove);

        _boardRigidbody.isKinematic = false;
        _boardRigidbody.useGravity = true;
        _boadrBoxCollider.isTrigger = false;

        StopCoroutine(OnPhysic());
    }

    private IEnumerator CheckBolts(GameObject BoltToRemove)
    {
        int bolts = 0;
        bool sameBoard = false;
        bool canscrew = false;
        bool wait = false;

        _boardRigidbody.isKinematic = true;
        _boardRigidbody.useGravity = false;
        _boadrBoxCollider.isTrigger = true;

        if (HowManyBoltsHaveBoard() == 1 || HowManyBoltsHaveBoard() == 2 || HowManyBoltsHaveBoard() == 3)
        {
            wait = true;

            if (HowManyBoltsHaveBoard() != 2 && HowManyBoltsHaveBoard() != 3)
            {
                StartCoroutine(OnPhysic());
                StartCoroutine(ScrewNextBolt(0.4f));
            }
        }

        if (HowManyBoltsHaveBoard() == 0)
        {
            wait = true;
        }

        if (_lastBolt != null)
        {
            _lastBolt.GetComponent<BoltController>().AdjustThePositionOfAnchor();
        }

        if (_freeHoles.CheckHoles() != null)
        {
            _boltGlobalScript.SetNextBoltMoveFlag(false);

            yield return new WaitForSeconds(0.1f);

            if (_freeHoles.CheckHoles().GetComponent<Hole>().CanScrewing() == true)
            {
                canscrew = true;
            }

            yield return new WaitForSeconds(0.2f);

            if (HowManyBoltsHaveBoard() == 0)
            {
                _boardRigidbody.isKinematic = false;
                _boardRigidbody.useGravity = true;
                _boadrBoxCollider.isTrigger = false;
            }

            for (int i = 0; i < _bolts.Length; i++)
            {
                if (_bolts[i] == BoltToRemove)
                {
                    if (_addBolt || !_addBolt || _freeHoles.CheckHoles().GetComponent<Hole>().SetBoltInCube() || _freeHoles.CheckHoles().GetComponent<Hole>().SetBoltInBoard())
                    {
                        if (_freeHoles.CheckHoles().GetComponent<Hole>().SetBoltInBoard())
                        {
                            if (_bolts[i].GetComponent<BoltMovement>().ReturnTwoBoards(gameObject) == false)
                            {
                                _boards = _freeHoles.CheckHoles().GetComponent<Hole>().GetSameBoards();

                                for (int j = 0; j < _boards.Length; j++)
                                {
                                    if (_boards[j] == gameObject)
                                    {
                                        sameBoard = true;
                                    }
                                }

                                if (!sameBoard)
                                {
                                    _bolts[i] = null;
                                }
                            }
                            else
                            {
                                _boards = _freeHoles.CheckHoles().GetComponent<Hole>().GetSameBoards();

                                for (int j = 0; j < _boards.Length; j++)
                                {
                                    if (_boards[j] == gameObject)
                                    {
                                        sameBoard = true;
                                    }
                                }

                                if (!sameBoard)
                                {
                                    _bolts[i] = null;
                                }
                            }
                        }
                        else
                        {
                            _bolts[i] = null;
                        }
                    }
                }
            }

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
                    }
                }

                if (_canRemoveHinge == true)
                {
                    for (int i = 0; i < _boards.Length; i++)
                    {
                        if (_boards[i].GetComponent<Board>().ReturnHole() != true)
                        {
                            _boards[i].GetComponent<Board>().DoThat();
                        }
                    }

                    _canRemoveHinge = false;
                }

                if (bolts == 1 || bolts == 0)
                {
                    if (_can == false && !_haveChangedBolt || HowManyBoltsHaveBoard() == 0)
                    {
                        foreach (HingeJoint HingeJoint in _hingeJoints)
                        {
                            if (_canRemoveBoard)
                            {
                                _canRemoveBoard = false;
                            }
                        }
                    }
                    else
                    {
                        _lastBolt.GetComponent<BoltController>().AdjustThePositionOfAnchor();
                    }
                }

                if (_can == false && !_haveChangedBolt && _boltToRemove == _lastBolt || HowManyBoltsHaveBoard() == 0 && _isMoving)
                {

                    foreach (HingeJoint HingeJoint in _hingeJoints)
                    {
                        if (_canRemoveBoard)
                        {
                            _canRemoveBoard = false;
                        }
                    }
                }
                else
                {
                    _lastBolt.GetComponent<BoltController>().AdjustThePositionOfAnchor();
                }

                _lastBolt.GetComponent<BoltController>().AdjustThePositionOfAnchor();

            }

            _boltToRemove = null;

            if (canscrew == true)
            {
                StartCoroutine(ScrewNextBolt(0.1f));

                yield return new WaitForSeconds(_timeForMove);

                if (HowManyBoltsHaveBoard() == 1 && wait)
                {
                    _boardRigidbody.constraints = RigidbodyConstraints.None;
                    _boardRigidbody.isKinematic = false;
                    _boardRigidbody.useGravity = true;
                    _boadrBoxCollider.isTrigger = false;
                }

                for (int i = 0; i < _boltGlobalScript.ReturnAllBoards().Length; i++)
                {
                    _boltGlobalScript.ReturnAllBoards()[i].DoBoardTrigger(false);
                }

                if (HowManyBoltsHaveBoard() == 0)
                {
                    _boardRigidbody.isKinematic = false;
                    _boardRigidbody.useGravity = true;
                    _boadrBoxCollider.isTrigger = false;
                }

                _newHole = true;
                _canRemoveHinge = true;
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
                    _boardRigidbody.isKinematic = true;
                }
                else if (_boltsCount < 1)
                {
                    _lastBolt.GetComponent<BoltTouch>().SetBoard(_board);
                }

                _addBolt = false;

                if (HowManyBoltsHaveBoard() == 1 || HowManyBoltsHaveBoard() == 0)
                {
                    _boardRigidbody.constraints = RigidbodyConstraints.None;

                    //yield return new WaitForSeconds(0.1f);

                    _boadrBoxCollider.isTrigger = false;
                }
            }
        }
    }

    public GameObject[] ReturnBoards()
    {
        return _boards;
    }

    public void DoBoardTrigger(bool Operater)
    {
        _boadrBoxCollider.isTrigger = Operater;
    }

    public void AddPhysic()
    {
        _lastBolt.GetComponent<BoltController>().AddAnchors(_boardRigidbody);
        _boardRigidbody.constraints = RigidbodyConstraints.None;
        _boardRigidbody.freezeRotation = false;
        _boardRigidbody.isKinematic = false;

        Vector3 center = _boadrBoxCollider.center;
        center.x += 0.01f;
        _boadrBoxCollider.center = center;

        _boadrBoxCollider.enabled = true;
        _boardRigidbody.useGravity = true;

        _lastBolt.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void FindEmptyObjectsInRadius()
    {
        if (_closestTransform == null && _boltMovePoint == null)
        {
            Vector3 center = Vector3.zero;

            if (_boltToRemove != null)
            {
                center = _boltToRemove.GetComponent<BoltController>().GetTransform().position;
            }

            GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Hole");

            float closestDistance = Mathf.Infinity;

            foreach (GameObject obj in allObjects)
            {
                float distance = Vector3.Distance(center, obj.transform.position);

                if (distance < _radius)
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = Vector3.Distance(center, obj.GetComponent<Hole>().SetOffset().transform.position);
                        _closestTransform = obj.GetComponent<Hole>().SetOffset().transform;
                    }
                }
            }

            if (_closestTransform != null)
            {
                _boltMovePoint = _closestTransform;
            }
            else
            {
                _boltMovePoint = null;
            }
        }
    }

    public bool FindBolts(GameObject bolt)
    {
        bool haveThatBolt = false;

        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] == bolt)
            {
                haveThatBolt = true;
            }
        }

        return haveThatBolt;
    }

    public bool HaveBolt()
    {
        int bolts = 0;
        bool oneBolt = false;

        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] != null)
            {
                bolts++;
            }
        }

        if (bolts < 1)
        {
            oneBolt = true;
        }

        return oneBolt;
    }

    public int HowManyBoltsHaveBoard()
    {
        int bolts = 0;

        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] != null)
            {
                bolts++;
            }
        }

        return bolts;
    }

    public void OnGravity()
    {
        _boardRigidbody.constraints = RigidbodyConstraints.None;
        _boardRigidbody.isKinematic = false;
        _boardRigidbody.useGravity = true;
        _boadrBoxCollider.isTrigger = false;
    }

    public bool ReturnBoltsHingenJoint()
    {
        bool haveJoints = false;

        if (_hingeJoints != null)
        {
            haveJoints = true;
        }

        return haveJoints;
    }

    public GameObject[] ReturnBolts()
    {
        return _bolts;
    }

    public GameObject FindClosestHoleToBolt(GameObject bolt)
    {
        GameObject closestHole = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < _holes.Length; i++)
        {
            GameObject hole = _holes[i];

            float distance = Vector3.Distance(bolt.transform.position, hole.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHole = hole;
            }
        }

        return closestHole;
    }

    public GameObject FindClosestHoleToBoltOfEver(GameObject bolt)
    {
        GameObject closestHole = null;
        float closestDistance = Mathf.Infinity;
        Hole[] holes;
        List<GameObject> holesObjects = new List<GameObject>();

        holes = FindObjectsOfType<Hole>();

        for (int i = 0; i < holes.Length; i++)
        {
            if (holes[i].SetBoltInBoard() == false)
            {
                holesObjects.Add(holes[i].gameObject);
            }
        }

        for (int i = 0; i < holesObjects.Count; i++)
        {
            GameObject hole = holesObjects[i];

            float distance = Vector3.Distance(bolt.transform.position, hole.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHole = hole;
            }
        }

        return closestHole;
    }

    public void BoltsCheking(GameObject BoltToRemove, Transform BoltTransform)
    {
        _boltToRemove = BoltToRemove;
        _boltTransform = BoltTransform;
        _isMoving = true;
    }

    public void SetChangedBolt(bool Operator)
    {
        _haveChangedBolt = Operator;
    }

    public void LoadChangedBolt(GameObject Bolt)
    {
        _changedBolt = Bolt;
    }

    public void CanUseAnyBolt()
    {
        canUseBolt = true;
    }

    public void SetBoltToRemuveNull()
    {
        _boltToRemove = null;
    }

    public void DoThat()
    {
        _can = true;
    }

    public void SetOldHole()
    {
        _newHole = false;
    }

    public bool ReturnHole()
    {
        return _newHole;
    }

    public void BoltMoveToPoint()
    {
        if (_boltToRemove != null)
        {
            _boltToRemove.GetComponent<BoltMovement>().StayBoltCollider();
        }

        if (_boltMovePoint != null)
        {
            float distance = Vector3.Distance(_boltTransform.position, _boltMovePoint.position);

            if (distance < 1.3f)
            {
                if (Vector3.Distance(_boltTransform.position, _boltMovePoint.position) <= 0.2f)
                {
                    _boltScrewing = false;
                    StopCoroutine(FindHole());
                    _isMoving = false;
                    _closestTransform = null;
                    _boltMovePoint = null;
                    _boltGlobalScript.SetChangeBolt(true);
                    _boltGlobalScript.SetClickOnHole(true);
                }
                else
                {
                    _boltScrewing = true;
                    _boltTransform.position = Vector3.MoveTowards(_boltTransform.position, _boltMovePoint.position, _moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                _closestTransform = null;
                StartCoroutine(FindHole());
            }
        }

        GameObject[] childObjects = _boltTransform.Cast<Transform>().Select(t => t.gameObject).ToArray();

        for (int i = 0; i < childObjects.Length; i++)
        {
            if (childObjects[i].GetComponent<HingeJoint>() != null)
            {
                childObjects[i].GetComponent<BoltController>().AdjustThePositionOfAnchor();
            }
        }
    }

    public bool ReturnBoltScrewInfo()
    {
        return _boltScrewing;
    }

    private IEnumerator ScrewNextBolt(float Time)
    {
        yield return new WaitForSeconds(Time);

        _boltGlobalScript.SetNextBoltMoveFlag(true);
        StopCoroutine(ScrewNextBolt(0.1f));
    }

    private IEnumerator AddAnchors()
    {
        yield return new WaitForSeconds(0.1f);

        Board[] boards = _lastBolt.GetComponent<BoltController>().ReturnBoards();
        HingeJoint[] joints = _lastBolt.GetComponents<HingeJoint>();

        yield return new WaitForSeconds(0.02f);

        for (int e = 0; e < boards.Length; e++)
        {
            if (joints.Length < boards.Length)
            {
                _lastBolt.GetComponent<BoltController>().AddAnchors(boards[e].gameObject.GetComponent<Rigidbody>());
            }
        }

        StopCoroutine(AddAnchors());
    }

    private IEnumerator FindHole()
    {
        FindEmptyObjectsInRadius();

        yield return new WaitForSeconds(0.1f);

        BoltMoveToPoint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_deadBoard)
        {
            if (other.gameObject.CompareTag("MainCamera"))
            {
                _UIOptions.RecoundBourds(1);
                _deadBoard = true;
            }
        }
    }
}