using UnityEngine;

public class OpenNextStepInTutorial : MonoBehaviour
{
    [SerializeField] private bool _openNextStep = false;

    public void OpenNextStep(bool Variable)
    {
        _openNextStep = Variable;
    }

    public bool CanOpenNextStep()
    {
        return _openNextStep;
    }
}
