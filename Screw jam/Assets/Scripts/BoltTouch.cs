using UnityEngine;

public class BoltTouch : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private BoltMovement _bolt;
    [SerializeField] private HingeJoint _hingeJoint;

    public void BoltButtonTouch(GameObject Bolt)
    {
        if (_board.ReturneBolt() == null)
        {
            _board.CanUseAnyBolt();
            _bolt.CheckingActiveBolt();
            _board.BoltsCheking(this.gameObject, _hingeJoint);
        }
    }
}
