using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevel : MonoBehaviour
{
    [SerializeField] private OpenNextStepInTutorial[] _steps;
    [SerializeField] private CubeRotation _cubeRotation;
    [SerializeField] private Transform _hand, _firstCubeMovePoint, _secondCubeMovePoint;
    [SerializeField] private Image _handImage;
    [SerializeField] private float _handSpeed;
    
    private Vector3 _handPosition = Vector3.zero;
    private bool _activateThirdSetp = true, _canRotateCube = true;

    private bool _canStart = false;

    private void Start()
    {
        StartCoroutine(WaitBeforeStart());
    }

    private IEnumerator WaitBeforeStart()
    {
        yield return new WaitForSeconds(1f);
        _canStart = true;
    }

    private void Update()
    {
        if (!_canStart)
        {
            return;
        }

        if (_steps[0].CanOpenNextStep() == false)
        {
            MoveHand(_steps[0].gameObject.transform.position);
        }
        else if (_steps[1].CanOpenNextStep() == false)
        {
            MoveHand(_steps[1].gameObject.transform.position);
            if (Vector3.Distance(_steps[1].gameObject.transform.position, _hand.position) < 0.4f)
            {
                ChangeHandImagePosition(108, -131, -50);
            }
        }
        else if (_steps[2].CanOpenNextStep() == false)
        {
            if (_activateThirdSetp)
            {
                StartCoroutine(MoveCubeandAnimation());
                _activateThirdSetp = false;
            }
        }
        else if (_steps[3].CanOpenNextStep() == false)
        {
            MoveHand(_steps[3].gameObject.transform.position);
            ChangeHandImagePosition(111, -84, -180);
            _canRotateCube = false;
        }
        else if (_steps[4].CanOpenNextStep() == false)
        {
            MoveHand(_steps[4].gameObject.transform.position);
            ChangeHandImagePosition(86, -99, -180);
            _handPosition = _hand.position;
        }
        else
        {
            _canRotateCube = true;
            _hand.position = _handPosition;
            _cubeRotation.OffTutorial();
            StartCoroutine(HideHand(_handImage, 1f, 0f, 1));

            BoltTouch[] bolts = GameObject.FindObjectsOfType<BoltTouch>();
            foreach (BoltTouch bolt in bolts)
            {
                bolt.IsNotTutorialLevel();
            }

            Hole[] allHoles = GameObject.FindObjectsOfType<Hole>();
            foreach (Hole hole in allHoles)
            {
                hole.IsNotTutorialLevel();
            }
        }
    }

    private void MoveHand(Vector3 TrargetPosition)
    {
        _hand.transform.position = Vector3.MoveTowards(_hand.transform.position, TrargetPosition, _handSpeed * Time.deltaTime);
    }

    public bool CanCubeRotate()
    {
        return _canRotateCube;
    }

    private IEnumerator MoveCubeandAnimation()
    {
        Vector3 targetPosition = new Vector3(108, -131, -180);
        float elapsedTime = 0f;

        while (Vector3.Distance(_hand.position, _firstCubeMovePoint.position) > 0.01f)
        {
            _hand.position = Vector3.MoveTowards(_hand.position, _firstCubeMovePoint.position, _handSpeed * Time.deltaTime);

            _handImage.transform.localPosition = Vector3.Lerp(_handImage.transform.localPosition, targetPosition, elapsedTime * 20f);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        while (_steps[2].CanOpenNextStep() == false)
        {
            while (Vector3.Distance(_hand.position, _secondCubeMovePoint.position) > 0.01f)
            {
                _hand.position = Vector3.MoveTowards(_hand.position, _secondCubeMovePoint.position, (_handSpeed / 3) * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            _hand.position = _firstCubeMovePoint.position;
        }
    }

    private void ChangeHandImagePosition(float x, float y, float z)
    {
        Vector3 targetPosition = new Vector3(x, y, z);

        _handImage.transform.localPosition = Vector3.Lerp(_handImage.transform.localPosition, targetPosition, Time.deltaTime * 10f);
    }


    private IEnumerator HideHand(Graphic image, float startAlpha, float endAlpha, float duration)
    {
        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;

        _hand.gameObject.SetActive(false);
    }
}
