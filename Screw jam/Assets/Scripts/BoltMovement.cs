using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoltMovement : MonoBehaviour
{
    [SerializeField] private float _speed, _timeForMove, _screwingSpeed;
    [SerializeField] private Transform _transform, _end, _start;
    [SerializeField] private Board _board;

    private BoltController _controller;
    private BoltTouch _boltTouch;
    private BoxCollider _capsuleCollider;
    private Board _oldBoard;
    private BoltGlobalScript _boltGloblScript;
    private bool _canMove = false, _canScrew = false, _can = false;
    private HolesChecking _activeHole;
    private BoxCollider _boltCollider;
    private Transform _centerOfRotation, _endOffset;
    private GameObject _meinCamera, _cube, _hole;
    private float updateInterval = 0.01f;
    private float timeElapsed = 0f;
    private float lerpTime = 0f;

    private void Awake()
    {
        _capsuleCollider = gameObject.GetComponent<BoxCollider>();
        _controller = gameObject.GetComponent<BoltController>();
        _boltTouch = gameObject.GetComponent<BoltTouch>();
    }

    private void Start()
    {
        _centerOfRotation = FindObjectOfType<CubeRotation>().transform;
        _boltGloblScript = FindObjectOfType<BoltGlobalScript>();

        _meinCamera = FindObjectOfType<Camera>().gameObject;
        _cube = FindObjectOfType<CubeRotation>().gameObject;

        _activeHole = FindObjectOfType<HolesChecking>();

        _oldBoard = _board;

        StartCoroutine(MoveBoltOnTheStart());
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
            var holeComponent = _hole.GetComponent<Hole>();

            if (_canScrew)
            {
                _transform.position = Vector3.MoveTowards(_transform.position, _hole.transform.position, _screwingSpeed * Time.deltaTime);

                if (holeComponent.SetBoltInBoard())
                {
                    _boltTouch.SetBoard(holeComponent.ReturnBoard());
                }

                _boltGloblScript.SetBoltMoveActiveFalse();
            }

            _controller.RemoveBoltFromList();

            if (holeComponent.SetBoltInPanel())
            {
                _transform.transform.SetParent(_meinCamera.transform);

                HingeJoint[] hingeJoints1 = gameObject.GetComponents<HingeJoint>();

                foreach (var joint in hingeJoints1)
                {
                    Destroy(joint);
                }
            }
            else if (holeComponent.SetBoltInCube())
            {
                _transform.transform.SetParent(_cube.transform);

                HingeJoint[] hingeJoints1 = gameObject.GetComponents<HingeJoint>();

                foreach (var joint in hingeJoints1)
                {
                    Destroy(joint);
                }
            }
            else if (holeComponent.SetBoltInBoard())
            {
                _board = _hole.transform.parent.gameObject.GetComponent<Board>();

                _transform.transform.SetParent(_board.transform.parent.gameObject.transform);

                _board.AddBolt(this.gameObject);
            }

            _controller.AddNewBolt();
        }

        if (_board != null)
        {
            if (_board.ReturnBoltScrewInfo() == false && _can)
            {
                Movement(_hole);
                _controller.AddNewBolt();
            }
            else if (_board.ReturnBoltScrewInfo() == true && _can)
            {
                _board.BoltMoveToPoint();

                for (int i = 0; i < _board.ReturnBoards().Length; i++)
                {
                    _board.ReturnBoards()[i].GetComponent<Rigidbody>().isKinematic = true;
                }
            }
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

    public bool ReturnTwoBoards(GameObject board)
    {
        bool newBoard = false;

        if (_oldBoard.gameObject == _board.gameObject || board == _board.gameObject)
        {
            newBoard = true;
        }

        StartCoroutine(LoadOldBoard());

        return newBoard;
    }

    private void Movement(GameObject Hole)
    {
        var holeComponent = Hole.GetComponent<Hole>();
        var sameBoards = holeComponent.GetSameBoards();

        if (_endOffset == null)
        {
            BackBoltCollider();
            _endOffset = holeComponent.SetOffset();
        }

        for (int i = 0; i < sameBoards.Length; i++)
        {
            var boardObject = sameBoards[i];
            var rigidbody = boardObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            var board = boardObject.GetComponent<Board>();
            if (board != null)
            {
                if (board.ReturnBoltsHingenJoint())
                {
                    StartCoroutine(LoadChangedBolt(true));
                }
            }
            else
            {
                StartCoroutine(LoadChangedBolt(false));
            }
        }

        lerpTime += _speed * Time.deltaTime;

        Vector3 StartOffset = _transform.position - _centerOfRotation.position;
        Vector3 EndOffsetVector = _endOffset.position - _centerOfRotation.position;

        Vector3 currentPosition = Vector3.Slerp(StartOffset, EndOffsetVector, lerpTime);

        _transform.position = _centerOfRotation.position + currentPosition;

        if (lerpTime >= 1f)
        {
            lerpTime = 0f;
            _transform.position = Hole.transform.position;
            _canScrew = true;
            _canMove = false;
        }

        _transform.rotation = Hole.transform.rotation;

        if (_canScrew && Vector3.Distance(_transform.position, Hole.transform.position) < 0.1f)
        {
            _boltGloblScript.SetBoltMoveActiveTrue();
            _boltGloblScript.SetOldHole(_hole);

            lerpTime = 0;
            _can = false;
            _canScrew = false;
            _controller.AddBoards();
            _endOffset = null;
            _boltCollider.isTrigger = false;

            StartCoroutine(DoBoardKinematicFalse(Hole));
            StartCoroutine(DoBoltTriggerFalse());
            StartCoroutine(Wait(0.05f));
            StartCoroutine(LoadAnchores());
        }

    }

    private IEnumerator LoadAnchores()
    {
        yield return new WaitForSeconds(0.1f);

        Board[] boards = _controller.ReturnBoards();

        if (gameObject.GetComponents<HingeJoint>().Length < boards.Length)
        {
            for (int i = 0; i < boards.Length; i++)
            {
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.GetComponent<BoltController>().AddAnchors(boards[i].GetComponent<Rigidbody>());
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                gameObject.GetComponent<BoxCollider>().isTrigger = false;
            }
        }
    }

    private IEnumerator Wait(float Time)
    {
        yield return new WaitForSeconds(Time);

        GameObject[] bolts = _board.ReturnBolts();
        Board[] controllerBoards = null;
        int value = 0;

        HingeJoint[] anchores = gameObject.GetComponents<HingeJoint>();

        for (int i = 0; i < anchores.Length; i++)
        {
            Destroy(anchores[i]);
        }

        for (int i = 0; i < bolts.Length; i++)
        {
            if (bolts[i] != null)
            {
                controllerBoards = bolts[i].GetComponent<BoltController>().ReturnBoards();

                HingeJoint[] joints = bolts[i].gameObject.GetComponents<HingeJoint>();

                foreach (HingeJoint joint in joints)
                {
                    if (_board != _oldBoard)
                    {
                        for (int j = 0; j < controllerBoards.Length; j++)
                        {
                            if (value > 11)
                            {
                                Debug.Log("ok1 / " + joint.gameObject.name + " / " + value);
                                //Destroy(joint);
                                value = 0;
                            }

                            if (joint.connectedBody != controllerBoards[j].GetComponent<Rigidbody>())
                            {
                                value++;
                            }
                        }
                    }
                    else if (_board.HowManyBoltsHaveBoard() == 2 || _board.HowManyBoltsHaveBoard() == 1 || _board == _oldBoard)
                    {
                        List<HingeJoint> jointsToRemove = new List<HingeJoint>();

                        foreach (var jointt in joints)
                        {
                            bool isConnectedBodyValid = controllerBoards.Any(board => board.GetComponent<Rigidbody>() == jointt.connectedBody);

                            if (!isConnectedBodyValid)
                            {
                                jointsToRemove.Add(jointt);
                            }
                        }

                        foreach (var jointe in jointsToRemove)
                        {
                            Debug.Log("ok2");
                            Destroy(jointe);
                        }
                    }

                    /*  Dictionary<Rigidbody, HingeJoint> uniqueJoints = new Dictionary<Rigidbody, HingeJoint>();
                      List<HingeJoint> duplicateJoints = new List<HingeJoint>();

                      foreach (HingeJoint jointet in joints)
                      {
                          Rigidbody connectedBody = jointet.connectedBody;

                          if (uniqueJoints.ContainsKey(connectedBody))
                          {
                              duplicateJoints.Add(jointet);
                          }
                          else
                          {
                              uniqueJoints.Add(connectedBody, jointet);
                          }
                      }*/
                }
            }
        }

        StopCoroutine(Wait(0.1f));
    }

    public void StayBoltCollider()
    {
        Vector3 newCenter;

        _boltCollider = gameObject.GetComponent<BoxCollider>();

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= updateInterval)
        {
            timeElapsed = 0f;

            newCenter = _boltCollider.center;
            newCenter.y += 0.057f;
            _boltCollider.center = newCenter;
        }
    }

    private void BackBoltCollider()
    {
        Vector3 newCenter;

        _boltCollider = gameObject.GetComponent<BoxCollider>();

        _boltCollider.isTrigger = true;

        newCenter = _boltCollider.center;
        newCenter.y = 0;
        _boltCollider.center = newCenter;
    }

    public void MoveBack(bool CanRemoveBolt)
    {
        StartCoroutine(MoveBoltBack(CanRemoveBolt));
    }

    private IEnumerator LoadOldBoard()
    {
        yield return new WaitForSeconds(0.3f);

        _oldBoard = _board;

        StopCoroutine(LoadOldBoard());
    }

    private IEnumerator MoveBoltBack(bool CanRemoveBolt)
    {
        if (_canMove == true)
        {
            _board.SetBoltToRemuveNull();

            GameObject hole = null;

            if (_board != null)
            {
                if (Vector2.Distance(transform.position, _board.FindClosestHoleToBolt(gameObject).transform.position) < 0.7f)
                {
                    if (_board.FindBolts(gameObject) == true)
                    {
                        hole = _board.FindClosestHoleToBolt(gameObject);
                    }
                    else
                    {
                        hole = _board.FindClosestHoleToBoltOfEver(gameObject);
                    }
                }
                else
                {
                    hole = _board.FindClosestHoleToBoltOfEver(gameObject);
                }
            }

            if (CanRemoveBolt && _canMove == false)
            {
                _board.SetBoltToRemuveNull();
            }

            _capsuleCollider.center = new Vector3(0, 0, 0);

            if (_board.HaveBolt() == false)
            {
                _board.DoThat();
            }

            while (Vector3.Distance(_transform.position, hole.transform.position) > 0.01f)
            {
                _transform.position = Vector3.MoveTowards(_transform.position, hole.transform.position, (_screwingSpeed / 2) * Time.deltaTime);


                gameObject.GetComponent<BoltController>().AdjustThePositionOfAnchor();

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            _canMove = false;
        }
    }

    public void SetCanMove()
    {
        StartCoroutine(MoveBoltOnTheStart());
    }

    public bool ReturnCanMove()
    {
        return _canMove;
    }

    public void SetCanMove(bool Operator)
    {
        _canMove = Operator;
    }

    private IEnumerator MoveBoltOnTheStart()
    {
        GameObject hole = null;

        if (_board != null)
        {
            hole = _board.FindClosestHoleToBolt(gameObject);
        }
        else
        {
            _board = _boltTouch.ReturnBoard();

            hole = _board.FindClosestHoleToBolt(gameObject);
        }

        _board.SetBoltToRemuveNull();
        _board.DoThat();

        while (Vector3.Distance(_transform.position, hole.transform.position) > 0.01f)
        {
            gameObject.GetComponent<BoltController>().AdjustThePositionOfAnchor();

            _transform.position = Vector3.MoveTowards(_transform.position, hole.transform.position, _screwingSpeed * Time.deltaTime);
        }

        yield return new WaitForSeconds(0.1f);
        _canMove = false;
    }

    private IEnumerator DoBoardKinematicFalse(GameObject Hole)
    {
        yield return new WaitForSeconds(0.4f);

        var holeComponent = Hole.GetComponent<Hole>();
        var sameBoards = holeComponent.GetSameBoards();

        for (int i = 0; i < sameBoards.Length; i++)
        {
            var boardObject = sameBoards[i];
            var rigidbody = boardObject.GetComponent<Rigidbody>();
            var board = boardObject.GetComponent<Board>();

            rigidbody.isKinematic = false;

            if (board.HowManyBoltsHaveBoard() > 1)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                rigidbody.constraints = RigidbodyConstraints.None;
            }
        }
    }


    private IEnumerator LoadChangedBolt(bool Operator)
    {
        yield return new WaitForSeconds(0.2f);

        for (int j = 0; j < _controller.ReturnBoards().Length; j++)
        {
            _controller.ReturnBoards()[j].SetChangedBolt(Operator);
            _controller.ReturnBoards()[j].LoadChangedBolt(gameObject);
        }
    }

    private IEnumerator DoBoltTriggerFalse()
    {
        yield return new WaitForSeconds(0.5f);

        _capsuleCollider.isTrigger = false;

        StopCoroutine(DoBoltTriggerFalse());
    }

    public bool FindBoardsInBolt(GameObject Bolt)
    {
        int holes = 0;
        bool canAddAncores = false;
        BoltMovement bolt = Bolt.GetComponent<BoltMovement>();

        Vector3 direction = bolt.ReturnOffsets()[1].position - bolt.ReturnOffsets()[0].position;
        float maxDistance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(_transform.position, direction, maxDistance);

        foreach (RaycastHit item in hits)
        {
            if (item.collider.gameObject.CompareTag("Hole"))
            {
                holes++;
            }
        }

        if (holes > 1)
        {
            canAddAncores = true;
        }

        return canAddAncores;
    }

    public Transform[] ReturnOffsets()
    {
        Transform[] ret = { _start, _end };

        return ret;
    }
}
