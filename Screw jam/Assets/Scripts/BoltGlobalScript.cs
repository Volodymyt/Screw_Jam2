using UnityEngine;

public class BoltGlobalScript : MonoBehaviour
{
    private bool _moving = true, _canMoveNextBolt = true, _canClickOnHole = true, _canChangeBolt = true;
    private GameObject _activeBolt, _oldHole;
    private Board[] _allBoards;

    private void Start()
    {
        _allBoards = FindObjectsOfType<Board>();
    }

    public Board[] ReturnAllBoards()
    {
        return _allBoards;
    }

    public GameObject ReturnActiveBolt()
    {
        return _activeBolt;
    }

    public void SetActiveBolt(GameObject NewBolt)
    {
        _activeBolt = NewBolt;
    }

    public void SetChangeBolt(bool Operator)
    {
        _canChangeBolt = Operator;
    }

    public bool ReturnChangeBolt()
    {
        return _canChangeBolt;
    }

    public bool CheckBolts()
    {
        return _moving;
    }

    public bool CheckNextBoltMovement()
    {
        return _canMoveNextBolt;
    }

    public bool CanClickOnHole()
    {
        return _canClickOnHole;
    }

    public void SetClickOnHole(bool Flag)
    {
        _canClickOnHole = Flag;
    }

    public void SetNextBoltMoveFlag(bool Flag)
    {
        _canMoveNextBolt = Flag;
    }

    public GameObject GetOldHole()
    {
        return _oldHole;
    }

    public void SetOldHole(GameObject Hole)
    {
        _oldHole = Hole;
    }

    public void SetBoltMoveActiveFalse()
    {
        _moving = false;
    }

    public void SetBoltMoveActiveTrue()
    {
        _moving = true;
    }

    public bool ReturnMoving()
    {
        return _moving;
    }
}
