using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleAnimation : MonoBehaviour
{
    public Animator[] shuffleAnimators;
    private bool hasStarted = false;
    public AnimationClip animationClipReference;
    public GameManager gameManager;
    public bool[] hasPlayed;

    private void OnEnable()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
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
        Debug.Log("me is broken");
        gameManager.FinishShuffle();
    }
}
