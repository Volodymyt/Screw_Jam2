using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private bool _holeInCube = false, _holeInPanel = false, _holeInBoard = false;
    [SerializeField] private Transform _endOffsetForBolt, _startOfHole, _endOfHole;

    [SerializeField] private bool _useThisHole = false, _canScrewing = false;
    private float _radius = 0.1f;

    [SerializeField] private GameObject[] _sameBoards;
    private List<GameObject> _boardsList = new List<GameObject>();

    public bool CanScrewing()
    {
        int HolesInBoard = 0;
        int HolesInCube = 0;
        int Boards = 0;

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

                if (HolesInBoard == Boards && HolesInCube > 0 && CheckOnTop() == true)
                {
                    _canScrewing = true;
                }

                Debug.Log("Holes In Board: " + HolesInBoard + ", Holes In Cube: " + HolesInCube + ", Boards: " + Boards);
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
        _useThisHole = true;
        StartCoroutine(SetHoleActiveFalse());
    }

    public bool CheckForUse()
    {
        return _useThisHole;
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

    private IEnumerator SetHoleActiveFalse()
    {
        yield return new WaitForSeconds(0.1f);

        _useThisHole = false;
        _canScrewing = false;
    }
}
