using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource select;
    public AudioSource vote;
    public AudioSource cancel;

    public void PlaySelect()
    {
        select.Play();
    }
    public void PlayVote()
    {
        vote.Play();
    }
    public void PlayCancel()
    {
        cancel.Play();
    }
}
