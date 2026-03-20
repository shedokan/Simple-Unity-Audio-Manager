using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JSAM;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

/// <summary>
/// Play-mode tests for AudioManager stop/fetch/play across sound helpers,
/// regular music helpers, and the main music helper.
/// All assets are created in code — no dependency on project-specific resources.
/// </summary>
public class AudioManagerTests
{
    private static GameObject _amGo;
    private static SoundFileObject _sound;
    private static MusicFileObject _music;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (!_amGo)
        {
            _amGo = new GameObject("AudioManager", typeof(AudioManager));

            var clip = AudioClip.Create("TestClip", 4096, 1, 44100, false);

            _sound = ScriptableObject.CreateInstance<SoundFileObject>();
            SetFiles(_sound, clip);

            _music = ScriptableObject.CreateInstance<MusicFileObject>();
            SetFiles(_music, clip);

            yield return null; // Let AudioManager.Start() run
        }

        AudioManager.StopAllSounds();
        AudioManager.StopAllMusic();
        yield return null;
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        // Suppress "AudioManager is quitting" warnings on destroy
        typeof(AudioManager)
            .GetField("isQuitting", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(null, true);

        if (_amGo) Object.Destroy(_amGo);
        if (_sound) Object.Destroy(_sound);
        if (_music) Object.Destroy(_music);
        _amGo = null;
    }

    // Injects a clip into the protected `files` field on BaseAudioFileObject.
    private static void SetFiles(BaseAudioFileObject obj, AudioClip clip)
    {
        var field = typeof(BaseAudioFileObject).GetField("files",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.SetValue(obj, new List<AudioClip> { clip });
    }

    // ── Sound helpers ────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator Sound_Play_IsPlaying()
    {
        AudioManager.PlaySound(_sound);
        yield return null;
        Assert.IsTrue(AudioManager.IsSoundPlaying(_sound));
    }

    [UnityTest]
    public IEnumerator Sound_TryGet_ReturnsHelperWithCorrectAudioFile()
    {
        AudioManager.PlaySound(_sound);
        yield return null;
        Assert.IsTrue(AudioManager.TryGetPlayingSound(_sound, out var helper));
        Assert.AreEqual(_sound, helper.AudioFile);
    }

    [UnityTest]
    public IEnumerator Sound_Stop_NotPlaying()
    {
        AudioManager.PlaySound(_sound);
        yield return null;
        AudioManager.StopSound(_sound);
        Assert.IsFalse(AudioManager.IsSoundPlaying(_sound));
    }

    [UnityTest]
    public IEnumerator Sound_StopIfPlaying_ReturnsFalseWhenNotPlaying()
    {
        Assert.IsFalse(AudioManager.StopSoundIfPlaying(_sound));
        yield return null;
    }

    [UnityTest]
    public IEnumerator Sound_StopIfPlaying_ReturnsTrueAndStops()
    {
        AudioManager.PlaySound(_sound);
        yield return null;
        Assert.IsTrue(AudioManager.StopSoundIfPlaying(_sound));
        Assert.IsFalse(AudioManager.IsSoundPlaying(_sound));
    }

    // ── Regular music helpers ────────────────────────────────────────────

    [UnityTest]
    public IEnumerator Music_Play_IsPlaying()
    {
        AudioManager.PlayMusic(_music, transform: null);
        yield return null;
        Assert.IsTrue(AudioManager.IsMusicPlaying(_music));
    }

    [UnityTest]
    public IEnumerator Music_TryGet_ReturnsHelperWithCorrectAudioFile()
    {
        AudioManager.PlayMusic(_music, transform: null);
        yield return null;
        Assert.IsTrue(AudioManager.TryGetPlayingMusic(_music, out var helper));
        Assert.AreEqual(_music, helper.AudioFile);
    }

    [UnityTest]
    public IEnumerator Music_Stop_NotPlaying()
    {
        AudioManager.PlayMusic(_music, transform: null);
        yield return null;
        AudioManager.StopMusic(_music);
        Assert.IsFalse(AudioManager.IsMusicPlaying(_music));
    }

    [UnityTest]
    public IEnumerator Music_StopIfPlaying_ReturnsFalseWhenNotPlaying()
    {
        Assert.IsFalse(AudioManager.StopMusicIfPlaying(_music));
        yield return null;
    }

    [UnityTest]
    public IEnumerator Music_StopIfPlaying_ReturnsTrueAndStops()
    {
        AudioManager.PlayMusic(_music, transform: null);
        yield return null;
        Assert.IsTrue(AudioManager.StopMusicIfPlaying(_music));
        Assert.IsFalse(AudioManager.IsMusicPlaying(_music));
    }

    // ── Main music helper ────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator MainMusic_Play_IsPlaying()
    {
        AudioManager.PlayMusic(_music, isMainMusic: true);
        yield return null;
        Assert.IsTrue(AudioManager.IsMusicPlaying(_music));
    }

    [UnityTest]
    public IEnumerator MainMusic_Play_SetsMainMusicHelperAudioFile()
    {
        AudioManager.PlayMusic(_music, isMainMusic: true);
        yield return null;
        var main = AudioManager.MainMusicHelper;
        Assert.IsNotNull(main, "MainMusicHelper should be assigned after PlayMusic(isMainMusic: true)");
        Assert.AreEqual(_music, main.AudioFile);
    }

    [UnityTest]
    public IEnumerator MainMusic_TryGet_ReturnsHelper()
    {
        AudioManager.PlayMusic(_music, isMainMusic: true);
        yield return null;
        Assert.IsTrue(AudioManager.TryGetPlayingMusic(_music, out var helper));
        Assert.AreEqual(_music, helper.AudioFile);
    }

    [UnityTest]
    public IEnumerator MainMusic_Stop_NotPlaying()
    {
        AudioManager.PlayMusic(_music, isMainMusic: true);
        yield return null;
        AudioManager.StopMusic(_music);
        Assert.IsFalse(AudioManager.IsMusicPlaying(_music));
    }
}
