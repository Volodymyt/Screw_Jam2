using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private Transform _endOffsetForBolt, _startOfHole, _endOfHole;
    [SerializeField] private float _radius = 0.1f;
    [SerializeField] private OpenNextStepInTutorial _openNextStepInTutorial;
    [SerializeField] private Board _board;
    [SerializeField] private bool _isTutorialLevel = false;

    private GameObject[] _sameBoards;
    private BoltGlobalScript _boltGlobalScript;
    private Collider _holeCollider;
    private List<GameObject> _boardsList = new List<GameObject>();

    [SerializeField] private bool _holeInCube = false, _holeInPanel = false, _holeInBoard = false;
    private bool _useThisHole = false, _canScrewing = false;
    private bool _haveBolt = false;

    private void Awake()
    {
        _holeCollider = gameObject.GetComponent<Collider>();
    }

    private void Start()
    {
        _boltGlobalScript = FindObjectOfType<BoltGlobalScript>();
    }

    public bool CanScrewing()
    {
        int HolesInBoard = 0;
        int HolesInCube = 0;
        int Boards = 0;
        bool Bolt = false;

        if (SetBoltInBoard() == true)
        {
            if (CheckOnTop() == true)
            {
                RaycastHit[] Objects = Physics.CapsuleCastAll(_startOfHole.position, _endOfHole.position, _radius, _endOfHole.position - _startOfHole.position, Vector3.Distance(_startOfHole.position, _endOfHole.position));

                for (int i = 0; i < Objects.Length; i++)
                {
                    if (Objects[i].collider.GetComponent<Hole>() != null)
                    {
                        if (Objects[i].collider.GetComponent<Hole>().SetBoltInBoard())
                        {
                            HolesInBoard++;
                        }
                        else if (Objects[i].collider.GetComponent<Hole>().SetBoltInCube())
                        {
                            HolesInCube++;
                        }
                    }
                }

                for (int i = 0; i < Objects.Length; i++)
                {
                    if (Objects[i].collider.GetComponent<Board>() != null)
                    {
                        Boards++;
                    }
                }

                for (int i = 0; i < Objects.Length; i++)
                {
                    if (Objects[i].collider.GetComponent<BoltMovement>() != null)
                    {
                        Bolt = true;
                    }
                }

                if (HolesInBoard == Boards && HolesInCube > 0 && CheckOnTop() == true && HolesInBoard + HolesInCube == CheckForScrewing() && Bolt == false)
                {
                    _canScrewing = true;
                }
            }
        }
        else if (SetBoltInCube() == true)
        {
            if (CheckOnTop() == true)
            {
                _canScrewing = true;
            }
        }
        else
        {
            _canScrewing = true;
        }

        return _canScrewing;
    }

    private int CheckForScrewing()
    {
        int holes = 0;

        Vector3 direction = _endOfHole.position - _startOfHole.position;
        float maxDistance = direction.magnitude;

        RaycastHit[] hits = Physics.CapsuleCastAll(_startOfHole.position, _endOfHole.position, 0.01f, _endOfHole.position - _startOfHole.position, Vector3.Distance(_startOfHole.position, _endOfHole.position));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Hole"))
            {
                holes++;
            }
        }

        return holes;
    }

    private bool CheckOnTop()
    {
        bool canScrew = true;

        RaycastHit[] Objects = Physics.CapsuleCastAll(_startOfHole.position, _endOffsetForBolt.position, _radius, _endOffsetForBolt.position - _startOfHole.position, Vector3.Distance(_startOfHole.position, _endOffsetForBolt.position));

        for (int i = 0; i < Objects.Length; i++)
        {
            if (Objects[i].collider.GetComponent<Board>() != null)
            {
                canScrew = false; break;
            }
        }

        return canScrew;
    }

    public void TouchHole()
    {
        if (_isTutorialLevel && _openNextStepInTutorial == null)
        {
            return;
        }

        if (CanScrewing() && _boltGlobalScript.CanClickOnHole())
        {
            Debug.Log("ok");

            if (SetBoltInBoard() == true)
            {
                if (CheckBoltsInCollider(_holeCollider) == false)
                {
                    _useThisHole = true;
                    StartCoroutine(SetHoleActiveFalse());
                }
            }
            else
            {
                Debug.Log("ok");
                StartCoroutine(OpenNextStep());
                _useThisHole = true;
                StartCoroutine(SetHoleActiveFalse());
            }
        }
        _haveBolt = false;
    }

    private bool CheckBoltsInCollider(Collider triggerCollider)
    {
        Collider[] hitColliders = Physics.OverlapBox(triggerCollider.bounds.center, triggerCollider.bounds.extents);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<BoltMovement>() != null)
            {
                _haveBolt = true;
            }
        }

        return _haveBolt;
    }

    public bool CheckForUse()
    {
        return _useThisHole;
    }

    public void IsNotTutorialLevel()
    {
        _isTutorialLevel = false;
    }

    public bool SetBoltInCube()
    {
        return _holeInCube;
    }

    public bool SetBoltInPanel()
    {
        return _holeInPanel;
    }

    public bool SetBoltInBoard()
    {
        return _holeInBoard;
    }

    public Transform SetOffset()
    {
        return _endOffsetForBolt;
    }

    public void SetOldHole()
    {
        RaycastHit[] Objects = Physics.CapsuleCastAll(_startOfHole.position, _endOfHole.position, _radius, _endOfHole.position - _startOfHole.position, Vector3.Distance(_startOfHole.position, _endOfHole.position));

        for (int i = 0; i < Objects.Length; i++)
        {
            if (Objects[i].collider.GetComponent<Board>() != null)
            {
                Objects[i].collider.GetComponent<Board>().SetOldHole();
            }
        }
    }

    public GameObject[] GetSameBoards()
    {
        RaycastHit[] Objects = Physics.CapsuleCastAll(_startOfHole.position, _endOfHole.position, _radius, _endOfHole.position - _startOfHole.position, Vector3.Distance(_startOfHole.position, _endOfHole.position));

        HashSet<GameObject> uniqueBoards = new HashSet<GameObject>(_boardsList);

        foreach (var hit in Objects)
        {
            Board board = hit.collider.GetComponent<Board>();

            if (board != null)
            {
                uniqueBoards.Add(hit.collider.gameObject);
            }
        }

        _sameBoards = uniqueBoards.ToArray();

        return _sameBoards;
    }

    public Board ReturnBoard()
    {
        return _board;
    }

    private IEnumerator OpenNextStep()
    {
        yield return new WaitForSeconds(0.75f);

        if (_isTutorialLevel)
        {
            _openNextStepInTutorial.OpenNextStep(true);
        }

        StopCoroutine(OpenNextStep());
    }

    private IEnumerator SetHoleActiveFalse()
    {
        yield return new WaitForSeconds(0.4f);

        _useThisHole = false;

        yield return new WaitForSeconds(0.4f);
        _canScrewing = false;
    }
}
