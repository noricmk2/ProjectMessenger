using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundBase
{
    public string Name { get; private set; }
    public AudioClip Clip { get; private set; }
    public AudioSource Source { get; set; }
    public bool IsLoop { get; set; }
    private bool m_Interrupt;
    private Action<SoundBase> m_EndAction;
    private Coroutine m_UpdateCoroutine;

    public bool IsFinished { get { return !IsLoop && Progress >= 1.0f; } }
    public bool IsPlaying { get { return Source != null && Source.isPlaying; } }
    public float Progress
    {
        get
        {
            if (Source == null || Clip == null)
                return 0;
            return Source.timeSamples / Clip.samples;
        }
    }
    public SoundBase(string name)
    {
        Name = name;
        Clip = ObjectFactory.Instance.GetAudioClip(name);
        if (Clip == null)
            MSLog.LogError("not exist sound");
    }

    public void Play(bool play, bool pauseOther = false)
    {
        m_Interrupt = pauseOther;
        if (pauseOther)
            AudioManager.Instance.PauseOthers(Name);

        if (play && !IsPlaying)
        {
            Source.loop = IsLoop;
            Source.Play();
            if (m_UpdateCoroutine != null)
                AudioManager.Instance.StopCoroutine(m_UpdateCoroutine);
            m_UpdateCoroutine = AudioManager.Instance.StartCoroutine(Update_C());
        }
        else
            Source.Pause();
    }

    public IEnumerator Update_C()
    {
        while(!IsFinished)
            yield return null;
        Finish();
    }

    public void Finish()
    {
        if (m_Interrupt)
            AudioManager.Instance.ReplaySound();
        Play(false);
        if (m_EndAction != null)
            m_EndAction(this);
        UnityEngine.Object.Destroy(Source);
        Source = null;
        AudioManager.Instance.RemoveSound(Name);
    }

    public void Reset()
    {
        Source.time = 0;
        if (m_UpdateCoroutine != null)
            AudioManager.Instance.StopCoroutine(m_UpdateCoroutine);
        m_UpdateCoroutine = null;
    }

    public void SetEndAction(Action<SoundBase> endAction)
    {
        m_EndAction = endAction;
    }
}
