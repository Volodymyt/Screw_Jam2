using System.Collections.Generic;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    [SerializeField] private Transform _startOfBolt, _endOfBolt;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private Board[] _boards;
    private HingeJoint[] _hingeJoints;
    private List<Board> _boardsList = new List<Board>();

    private void Start()
    {
        AddBoards();

        for (int i = 0; i < _boards.Length; i++)
        {
            _boards[i].AddBolt(this.gameObject);
        }
    }

    public void AddBoards()
    {
        RaycastHit[] Boards = Physics.RaycastAll(_startOfBolt.position, _endOfBolt.position - _startOfBolt.position, Vector3.Distance(_startOfBolt.position, _endOfBolt.position), _layerMask);

        _boardsList.Clear();

        for (int i = 0; i < Boards.Length; i++)
        {
            _boardsList.Add(Boards[i].collider.gameObject.GetComponent<Board>());
        }

        _boards = _boardsList.ToArray();
    }

    public void RemoveBoltFromList()
    {
        for (int i = 0; i < _boards.Length; i++)
        {
            _boards[i].CheckBoltToRemove(gameObject);
        }
    }

    public void AdjustThePositionOfAnchor()
    {
        Vector3 initialAnchorPosition;
        Vector3 initialObjectPosition;
        bool _canSetHingeJoints = true;

        if (_canSetHingeJoints)
        {
            _hingeJoints = gameObject.GetComponents<HingeJoint>();
            _canSetHingeJoints = false;
        }


        for (int i = 0; i < _boards.Length + 100; i++)
        {
            if (i < Mathf.Max(_hingeJoints.Length))
            {
                initialAnchorPosition = _hingeJoints[i].anchor;
                initialObjectPosition = transform.position;

                float zOffset = transform.position.z - initialObjectPosition.z;

                Vector3 newAnchorPosition = initialAnchorPosition;
                newAnchorPosition.z -= zOffset;

                _hingeJoints[i].anchor = newAnchorPosition;
            }
        }
    }

    public void AddAnchors(Rigidbody BoardRigidbody)
    {
        HingeJoint NewHingenJoint = gameObject.AddComponent<HingeJoint>();

        NewHingenJoint.connectedBody = BoardRigidbody.GetComponent<Rigidbody>();
        NewHingenJoint.axis = new Vector3(0, 90, 0);
    }

    public Transform GetTransform()
    {
        return _startOfBolt;
    }

    public void AddNewBolt()
    {
        AddBoards();

        for (int i = 0; i < _boards.Length; i++)
        {
            _boards[i].AddBolt(this.gameObject);
        }
    }

    public Board[] ReturnBoards()
    {
        return _boards;
    }
}
