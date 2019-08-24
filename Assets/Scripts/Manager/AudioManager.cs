using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MSUtil;

public class AudioManager : Singleton<AudioManager>
{
    private Dictionary<string, SoundBase> m_RegisterSoundDic = new Dictionary<string, SoundBase>();
    private List<SoundBase> m_PauseList = new List<SoundBase>();

    public void PlaySound(string name, bool loop = false, bool interrupts = false, Action<SoundBase> endAction = null)
    {
        var sound = RegisterSound(name, loop);
        sound.SetEndAction(endAction);
        sound.Play(true, interrupts);
    }

    public SoundBase RegisterSound(string name, bool loop = false)
    {
        SoundBase sound;
        if (m_RegisterSoundDic.ContainsKey(name))
            sound = m_RegisterSoundDic[name];
        else
        {
            sound = new SoundBase(name);
            m_RegisterSoundDic[name] = sound;
        }
        sound.IsLoop = loop;
        if(sound.Source == null)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;
        }
        return sound;
    }

    public void PauseSound(string name)
    {
        if (m_RegisterSoundDic.ContainsKey(name))
            m_RegisterSoundDic[name].Play(false);
    }

    public void PauseOthers(string name)
    {
        var iter = m_RegisterSoundDic.GetEnumerator();
        while(iter.MoveNext())
        {
            var sound = iter.Current.Value;
            if (sound.Name != name && sound.IsPlaying)
            {
                sound.Play(false);
                m_PauseList.Add(sound);
            }
        }
    }

    public void ReplaySound()
    {
        for (int i = 0; i < m_PauseList.Count;)
        {
            m_PauseList[i].Play(true);
            m_PauseList.Remove(m_PauseList[i]);
        }
    }

    public void StopSound(string name)
    {
        if (!string.IsNullOrEmpty(name) && m_RegisterSoundDic.ContainsKey(name))
        {
            var sound = m_RegisterSoundDic[name];
            if (m_PauseList.Contains(sound))
                m_PauseList.Remove(sound);
            sound.Finish();
        }
    }

    public void StopAllSound()
    {
        var iter = m_RegisterSoundDic.GetEnumerator();
        while (iter.MoveNext())
        {
            var sound = iter.Current.Value;
            sound.Play(false);
            Destroy(sound.Source);
            sound.Source = null;
        }
        Release();
    }

    public void RemoveSound(string name)
    {
        if (m_RegisterSoundDic.ContainsKey(name))
            m_RegisterSoundDic.Remove(name);
    }

    public void Release()
    {
        m_RegisterSoundDic.Clear();
        m_PauseList.Clear();
    }
}
