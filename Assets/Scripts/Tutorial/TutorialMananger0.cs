using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMananger : MonoBehaviour
{
    private List<TutorialMananger> _step = new List<TutorialMananger>();

    private int _stepNum;

    private void Start()
    {
        _stepNum = 0;
        StartCoroutine(PlayTutorial());
    }

    private IEnumerator PlayTutorial()
    {
        yield return new WaitUntil(() => _stepNum < 1);

    }

}