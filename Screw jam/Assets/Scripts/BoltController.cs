using System.Collections.Generic;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    [SerializeField] private Board[] _boards;
    [SerializeField] private Transform _startOfBolt, _endOfBolt;

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
        RaycastHit[] Boards = Physics.RaycastAll(_startOfBolt.position, _endOfBolt.position - _startOfBolt.position, Vector3.Distance(_startOfBolt.position, _endOfBolt.position));

        _boardsList.Clear();

        for (int i = 0; i < Boards.Length; i++)
        {
            Board BoardComponent = Boards[i].collider.gameObject.GetComponent<Board>();

            if (BoardComponent != null)
            {
                _boardsList.Add(BoardComponent);
            }
        }

        _boards = _boardsList.ToArray();
    }

    public void RemoveBoltFromList()
    {
        for (int i = 0; i < _boards.Length; i++)
        {
            _boards[i].CheckBoltToRemove(this.gameObject);
        }
    }

    public void StopRotation()
    {
        for (int i = 0; i < _boards.Length; i++)
        {
            _boards[i].GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void StartRotation()
    {
        Vector3 anchorPoition;
        float hingesCount = 0;

        for (int i = 0; i < _boards.Length; i++)
        {
            _boards[i].GetComponent<Rigidbody>().isKinematic = false;
        }

        HingeJoint[] hinges = gameObject.GetComponents<HingeJoint>();

        foreach (HingeJoint hinge in hinges)
        {
            anchorPoition = hinge.connectedAnchor;

            anchorPoition.z = -1.3f + hingesCount;

            hingesCount += -1.6f;

            hinge.autoConfigureConnectedAnchor = false;
            hinge.connectedAnchor = anchorPoition;
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
}
