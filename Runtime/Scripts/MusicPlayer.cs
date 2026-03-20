using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSAM
{
    [AddComponentMenu("AudioManager/Music Player")]
    public class MusicPlayer : BaseAudioMusicFeedback
    {
        public enum FadeBehaviour
        {
            None,
            AdditiveFadeIn,
            CrossFadeIn,
            FadeOutAndFadeIn
        }

        [Tooltip("Behaviour to trigger when the object this is attached to is created")]
        [SerializeField] protected AudioPlaybackBehaviour onStart = AudioPlaybackBehaviour.Play;

        [Tooltip("Behaviour to trigger when the object this is attached to is enabled or when the object is created")]
        [SerializeField] protected AudioPlaybackBehaviour onEnable = AudioPlaybackBehaviour.None;

        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed or set to in-active")]
        [SerializeField] protected AudioPlaybackBehaviour onDisable = AudioPlaybackBehaviour.None;

        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed")]
        [SerializeField] protected AudioPlaybackBehaviour onDestroy = AudioPlaybackBehaviour.Stop;

        [Tooltip(
            "Fade behaviour to use when music is played back.\n" +
            "None - No fading\n" +
            "AdditiveFadeIn - Music fades in with no regard for currently playing music\n" +
            "CrossFadeIn - Fades out current Main Music while fading in this music\n" +
            "FadeOutAndFadeIn - Fades out current Main Music, and only after it's done fading, fade in this music")]
        [SerializeField] protected FadeBehaviour fadeBehaviour = FadeBehaviour.None;

        [SerializeField] bool isMainMusic;

        [Tooltip("Total time of the fade process")]
        [SerializeField] float fadeTime;

        MusicChannelHelper helper;
        public MusicChannelHelper MusicHelper { get { return helper; } }

        Coroutine playRoutine;

        // Start is called before the first frame update
        void Start()
        {
            switch (onStart)
            {
                case AudioPlaybackBehaviour.Play:
                    StartCoroutine(PlayDelayed());
                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }

        MusicChannelHelper oldHelper;

        void PlayInvokation()
        {
            if (fadeBehaviour > FadeBehaviour.None)
            {
                helper = AudioManager.FadeMusicIn(audio, fadeTime, !AudioManager.MainMusicHelper.IsPlaying());
            }
            else
            {
                if (isMainMusic)
                {
                    helper = AudioManager.PlayMusic(audio, true);
                }
                else
                {
                    helper = AudioManager.PlayMusic(audio, transform);
                }
            }
        }

        void PlayBehaviour()
        {
            float time = 0;
            switch (fadeBehaviour)
            {
                case FadeBehaviour.None:
                case FadeBehaviour.AdditiveFadeIn:
                    if (AudioManager.MainMusicHelper.IsPlaying())
                    {
                        time = AudioManager.MainMusicHelper.AudioSource.time;
                    }
                    PlayInvokation();
                    if (keepPlaybackPosition)
                    {
                        helper.AudioSource.time = time;
                    }
                    break;
                case FadeBehaviour.CrossFadeIn:
                    if (keepPlaybackPosition && AudioManager.MainMusicHelper.IsPlaying())
                    {
                        time = AudioManager.FadeMainMusicOut(fadeTime).AudioSource.time;
                    }
                    PlayInvokation();
                    if (keepPlaybackPosition)
                    {
                        helper.AudioSource.time = time;
                    }
                    break;
                case FadeBehaviour.FadeOutAndFadeIn:
                    if (fadeRoutine != null) StopCoroutine(fadeRoutine);
                    fadeRoutine = StartCoroutine(FadeInOut());
                    break;
            }
        }

        Coroutine fadeRoutine;
        IEnumerator FadeInOut()
        {
            var halfTime = fadeTime / 2;

            if (keepPlaybackPosition && AudioManager.MainMusicHelper.IsPlaying())
            {
                oldHelper = AudioManager.FadeMainMusicOut(halfTime);
            }

            float time = 0;
            if (oldHelper)
            {
                while (oldHelper.IsPlaying())
                {
                    time = oldHelper.AudioSource.time;
                    yield return null;
                }
            }

            PlayInvokation();

            if (keepPlaybackPosition)
            {
                helper.AudioSource.time = time;
            }

            fadeRoutine = null;
        }

        public void Play()
        {
            if (AudioManager.IsMusicPlaying(audio) && !restartOnReplay) return;

            PlayBehaviour();
        }

        public void Stop()
        {
            if (isMainMusic)
            {
                AudioManager.StopMusic(audio, null, true);
            }
            else
            {
                AudioManager.StopMusic(audio, transform, true);
            }
            helper = null;
        }

        private void OnEnable()
        {
            switch (onEnable)
            {
                case AudioPlaybackBehaviour.Play:
                    if (playRoutine != null) StopCoroutine(playRoutine);
                    playRoutine = StartCoroutine(PlayDelayed());
                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }

        IEnumerator PlayDelayed()
        {
            yield return new WaitUntil(() => AudioManager.Instance);
            yield return new WaitUntil(() => AudioManager.Instance.Initialized);

            playRoutine = null;
            Play();
        }

        private void OnDisable()
        {
            if (AudioManagerInternal.IsQuitting) return;
            switch (onDisable)
            {
                case AudioPlaybackBehaviour.Play:
                    Play();
                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }

        private void OnDestroy()
        {
            if (AudioManagerInternal.IsQuitting) return;
            switch (onDestroy)
            {
                case AudioPlaybackBehaviour.Play:
                    Play();
                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }
    }
}