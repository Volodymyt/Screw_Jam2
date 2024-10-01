using UnityEngine;

public class BoltGlobalScript : MonoBehaviour
{
    [SerializeField] private bool _moving = true;
    [SerializeField] private GameObject _activeBolt;
 
    public GameObject ReturnActiveBolt()
    {
        return _activeBolt;
    }

    public void SetActiveBolt(GameObject NewBolt)
    {
        _activeBolt = NewBolt;
    }

    public bool CheckBolts()
    {
        return _moving;
    }

    public void SetBoltMoveActiveFalse()
    {
        _moving = false;
    }
    
    public void SetBoltMoveActiveTrue()
    {
        _moving = true;
    }
}
