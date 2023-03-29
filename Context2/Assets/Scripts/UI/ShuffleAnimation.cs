using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleAnimation : MonoBehaviour
{
    public Animator[] shuffleAnimators;
    private bool hasStarted = false;
    public AnimationClip animationClipReference;
    public bool[] hasPlayed;

    private void OnEnable()
    {
        if (!hasStarted)
        {
            StartShuffleAnimation();
            StartCoroutine(goToNextAfterAnimation());
        }
    }

    private IEnumerator goToNextAfterAnimation()
    {
        yield return new WaitForSeconds(animationClipReference.length);
        GoToNextScreen();
        yield return null;
    }

    public void StartShuffleAnimation()
    {
        for (int i = 0; i < shuffleAnimators.Length; i++)
        {
            //shuffleAnimators[i].SetInteger("Start", i);
            shuffleAnimators[i].SetTrigger("a" + i);
        }

    }

    public void GoToNextScreen()
    {
        //ignore
    }
}
