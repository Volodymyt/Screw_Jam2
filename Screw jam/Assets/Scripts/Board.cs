using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] _bolts;
    [SerializeField] private BoxCollider _boadrBoxCollider;
    [SerializeField] private Rigidbody _boardRigidbody;
    [SerializeField] private float _timeForMove, _moveSpeed;
    [SerializeField] private float _radius;
    [SerializeField] private bool _isMoving = true, _addBolt = false;
    [SerializeField] private GameObject _lastHole;

    private int value = 2;
    private Transform _boltMovePoint;
    private HolesChecking _freeHoles;
    private HingeJoint _boltHingeJoint;
    private GameObject _lastBolt, _boltToRemove;
    private int _boltsCount, _boltsCountOnTheStart;
    private bool canUseBolt = false;

    private void Start()
    {
        _freeHoles = FindObjectOfType<HolesChecking>();

        for (int i = 0; i < _bolts.Length; i++)
        {
            _boltsCountOnTheStart = i + 1;
        }

        _boltsCount = _boltsCountOnTheStart;
    }

    private void Update()
    {
        if (canUseBolt == true && _freeHoles.CheckHoles() != null)
        {
            _boltHingeJoint.GetComponent<HingeJoint>().connectedBody = null;

            StartCoroutine(CheckBolts());

            canUseBolt = false;
        }
        else if (canUseBolt == true && _freeHoles.CheckHoles() == null && _isMoving)
        {
            FindEmptyObjectsInRadius();
            StartCoroutine(BoltMoveToPoint());
        }
    }

    public void CanUseAnyBolt()
    {
        canUseBolt = true;
    }

    public void BoltsCheking(GameObject BoltToRemove, HingeJoint HingeJoint)
    {
        _boltToRemove = BoltToRemove;
        _boltHingeJoint = HingeJoint;
        _isMoving = true;
    }

    public void AddBolt(GameObject NewBolt)
    {
        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] == NewBolt)
            {
                _addBolt = true;
                break;
            }
            else if (_bolts[i] != NewBolt && i == value)
            {
                for (int j = 0; j < _bolts.Length; j++)
                {
                    if (_bolts[j] == null)
                    {
                        _bolts[j] = NewBolt;
                        break;
                    }
                }
            }
        }
    }

    private void FindHole(GameObject Center)
    {
        Vector3 center = Center.transform.position;

        GameObject[] allObjects = GameObject.FindObjectsOfType<Hole>().Select(hole => hole.gameObject).ToArray();

        foreach (GameObject obj in allObjects)
        {
            float distance = Vector3.Distance(center, obj.transform.position);

            if (distance < _radius)
            {
                _lastHole = obj;
            }
        }
    }

    private void FindEmptyObjectsInRadius()
    {
        Vector3 center = _boltToRemove.transform.position;

        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Bolt Move Point");

        Transform closestTransform = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in allObjects)
        {
            float distance = Vector3.Distance(center, obj.transform.position);
            if (distance < _radius)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTransform = obj.transform;
                }
            }
        }

        if (closestTransform != null)
        {
            _boltMovePoint = closestTransform;
        }
    }

    private IEnumerator CheckBolts()
    {
        for (int i = 0; i < _bolts.Length; i++)
        {
            if (_bolts[i] == _boltToRemove)
            {
                if (_addBolt || !_addBolt || _freeHoles.CheckHoles().GetComponent<Hole>().SetBoltInCube() || _freeHoles.CheckHoles().GetComponent<Hole>().SetBoltInBoard())
                {
                    _bolts[i] = null;
                }
            }
        }
        _boltToRemove = null;

        yield return new WaitForSeconds(_timeForMove);

        foreach (GameObject Bolt in _bolts)
        {
            if (Bolt == null)
            {
                _boltsCount--;
            }
            else
            {
                _lastBolt = Bolt;
            }
        }

        if (_boltsCount == 1)
        {
            FindHole(_lastBolt);

            Vector3 LocalPosition = _lastHole.transform.localPosition;

            Debug.Log(_lastHole.name);

            if (LocalPosition.x < 0.15f)
            {
                Vector3 center = _boadrBoxCollider.center;
                center.x += 0.01f;
                _boadrBoxCollider.center = center;
            }

            _boadrBoxCollider.enabled = true;
            _boardRigidbody.useGravity = true;
            _lastBolt.GetComponent<HingeJoint>().connectedBody = this._boardRigidbody;
            _lastBolt.GetComponent<HingeJoint>().connectedAnchor = new Vector3(LocalPosition.x, 0, 1.7f);
        }
        else if (_boltsCount > 1)
        {
            _boltsCount = _boltsCountOnTheStart;
        }

        _addBolt = false;
    }

    public GameObject ReturneBolt()
    {
        return _boltToRemove;
    }

    private IEnumerator BoltMoveToPoint()
    {
        _boltToRemove.transform.position = Vector3.MoveTowards(_boltToRemove.transform.position, _boltMovePoint.position, _moveSpeed * Time.deltaTime);

        yield return new WaitForSeconds(0.15f);

        _isMoving = false;
    }
}
