using System.Collections;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private bool _holeInCube = false, _holeInPanel = false, _holeInBoard = false;
    [SerializeField] private Transform _endOffsetForBolt;

    private bool _useThisHole = false;

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

    private IEnumerator SetHoleActiveFalse()
    {
        yield return new WaitForSeconds(0.5f);

        _useThisHole = false;
    }
}
