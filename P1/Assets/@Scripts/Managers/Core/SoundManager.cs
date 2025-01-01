using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
	private AudioSource[] _audioSources = new AudioSource[(int)Define.ESound.Max];
	private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
	private GameObject _soundRoot = null;

	public void Init()
	{
		if (!PlayerPrefs.HasKey("BGM_Volume"))
			PlayerPrefs.SetFloat("BGM_Volume", 1.0f);

		if (!PlayerPrefs.HasKey("EFFECT_Volume"))
			PlayerPrefs.SetFloat("EFFECT_Volume", 1.0f);


		if (_soundRoot == null)
		{
			_soundRoot = GameObject.Find("@SoundRoot");

			if (_soundRoot == null)
			{
				_soundRoot = new GameObject { name = "@SoundRoot" };
				UnityEngine.Object.DontDestroyOnLoad(_soundRoot);

				string[] soundTypeNames = System.Enum.GetNames(typeof(Define.ESound));
				for (int count = 0; count < soundTypeNames.Length - 1; count++)
				{
					GameObject go = new GameObject { name = soundTypeNames[count] };
					_audioSources[count] = go.AddComponent<AudioSource>();
					go.transform.parent = _soundRoot.transform;
				}

				_audioSources[(int)Define.ESound.Bgm].loop = true;
			}
		}
	}

	public void Clear()
	{
		foreach (AudioSource audioSource in _audioSources)
			audioSource.Stop();

		_audioClips.Clear();
	}

	public void Play(Define.ESound type)
	{
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.Play();
	}

	public void Play(Define.ESound type, string key, float volume = 1.0f, float pitch = 1.0f)
	{
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.volume = volume;

		if (type == Define.ESound.Bgm)
		{
			LoadAudioClip(key, (audioClip) =>
			{
				if (audioSource.isPlaying)
				{
					audioSource.Stop();
					audioSource.volume = 0;

					DOTween.To(() => audioSource.volume, value => audioSource.volume = value, PlayerPrefs.GetFloat("BGM_Volume"), 3f);
					audioSource.clip = audioClip;
					audioSource.Play();

				}
				else
				{
					audioSource.volume = PlayerPrefs.GetFloat("BGM_Volume");
					audioSource.clip = audioClip;
					audioSource.Play();
				}
			});
		}
		else
		{
			LoadAudioClip(key, (audioClip) =>
			{
				audioSource.volume = PlayerPrefs.GetFloat("EFFECT_Volume");
				audioSource.pitch = pitch;
				audioSource.PlayOneShot(audioClip);
			});
		}
	}

	public void Play(Define.ESound type, AudioClip audioClip, float pitch = 1.0f)
	{
		AudioSource audioSource = _audioSources[(int)type];

		if (type == Define.ESound.Bgm)
		{
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.clip = audioClip;
			audioSource.Play();
		}
		else
		{
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip);
		}
	}

	public void Stop(Define.ESound type)
	{
		AudioSource audioSource = _audioSources[(int)type];
		audioSource.Stop();
	}

	private void LoadAudioClip(string key, Action<AudioClip> callback)
	{
		AudioClip audioClip = null;
		if (_audioClips.TryGetValue(key, out audioClip))
		{
			callback?.Invoke(audioClip);
			return;
		}

		audioClip = Managers.Resource.Load<AudioClip>(key);

		if (_audioClips.ContainsKey(key) == false)
			_audioClips.Add(key, audioClip);

		callback?.Invoke(audioClip);
	}

	public void ChangedSound(Define.ESound type)
	{
		_audioSources[(int)type].volume = PlayerPrefs.GetFloat(type == Define.ESound.Bgm ? "BGM_Volume" : "EFFECT_Volume");
	}

}
