using UnityEngine;

public class BoltGlobalScript : MonoBehaviour
{
    [SerializeField] private bool _moving = true;

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
