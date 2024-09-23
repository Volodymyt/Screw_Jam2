using System.Collections;
using UnityEngine;

public class HolesChecking : MonoBehaviour
{
    [SerializeField] private Hole[] _holes;

    private GameObject ActiveHole;

    private void Start()
    {
        _holes = FindObjectsOfType<Hole>();
    }

    public GameObject CheckHoles()
    {
        foreach (Hole HoleObj in _holes)
        {
            if (HoleObj.CheckForUse() == true)
            {
                ActiveHole = HoleObj.gameObject;
            }
        }
        StartCoroutine(SetHoleActiveFalse());

        return ActiveHole;
    }

    private IEnumerator SetHoleActiveFalse()
    {
        yield return new WaitForSeconds(0.2f);
        ActiveHole = null;
    }
}
