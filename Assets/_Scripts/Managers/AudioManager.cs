using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager {
    public ManagerStatus status { get; set; }

    // Not sure if AudioFX is needed
    // public AudioFX audioFX;

    [SerializeField] AudioClip playerHitTargetSound;
    [SerializeField] AudioClip playerKillTargetSound;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioClip[] explosionSounds;
    [SerializeField] AudioClip[] playerHitSounds;
    [SerializeField] AudioClip[] playerDeathSounds;
    [SerializeField] AudioClip[] playerJumpSounds;
    
    AudioSource soundSource2D;

    public void Startup() {
        Debug.Log("Audio Manager starting...");

        soundSource2D = GameObject.FindGameObjectWithTag("SoundSource").GetComponent<AudioSource>();

        status = ManagerStatus.Started;
    }

    // Specific 2D functions
    public void PlayRandomFootstep() {
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        PlaySound2DRandomPitch(clip);
    }

    public void PlayHitTargetSound(float volume) {
        PlaySound2D(playerHitTargetSound, volume);
    }

    public void PlayKillTargetSound(float volume) {
        PlaySound2D(playerKillTargetSound, volume);
    }

    public void PlayHitSound(float volume) {
        AudioClip clip = playerHitSounds[Random.Range(0, playerHitSounds.Length)];
        PlaySound2DRandomPitch(clip, volume);
    }

    public void PlayDeathSound(float volume) {
        AudioClip clip = playerDeathSounds[Random.Range(0, playerDeathSounds.Length)];
        PlaySound2DRandomPitch(clip, volume);
    }

    public void PlayJumpSound(float volume) {
        AudioClip clip = playerJumpSounds[Random.Range(0, playerJumpSounds.Length)];
        PlaySound2DRandomPitch(clip, volume);
    }

    // Specific 3D functions
    public void PlayRandomExplosion(Vector3 pos) {
        AudioClip clip = explosionSounds[Random.Range(0, explosionSounds.Length)];
        PlaySound3D(clip, pos, 0.2f, 5, 500);
    }

    // Generic 2D functions
    public void PlaySound2D(AudioClip clip) {
        soundSource2D.pitch = 1f;
        soundSource2D.PlayOneShot(clip);
    }

    public void PlaySound2D(AudioClip clip, float volume) {
        soundSource2D.pitch = 1f;
        soundSource2D.PlayOneShot(clip, volume);
    }

    public void PlaySound2D(AudioClip clip, float volume, float pitch) {
        soundSource2D.pitch = pitch;
        soundSource2D.PlayOneShot(clip, volume);
    }

    public void PlaySound2DRandomPitch(AudioClip clip) {
        soundSource2D.pitch = Random.Range(0.9f, 1.1f);
        soundSource2D.PlayOneShot(clip);
    }

    public void PlaySound2DRandomPitch(AudioClip clip, float volume) {
        soundSource2D.pitch = Random.Range(0.9f, 1.1f);
        soundSource2D.PlayOneShot(clip, volume);
    }

    // Generic 3D functions
    public void PlaySound3D(AudioClip clip, Vector3 pos) {
        PlayCustom3DAudioSource(clip, pos);
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos, float volume) {
        PlayCustom3DAudioSource(clip, pos, volume);
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos, float volume, float minDistanceFalloff, float maxDistanceFalloff) {
        PlayCustom3DAudioSource(clip, pos, volume, minDistanceFalloff, maxDistanceFalloff);
    }

    // Custom Play3DSound function returns an AudioSource so I can edit proporties 
    public static AudioSource PlayCustom3DAudioSource(AudioClip clip, Vector3 pos, float volume = 1, float minDistanceFalloff = 5, float maxDistanceFalloff = 50) {
        GameObject currentGO = Managers.Pooler.SpawnFromPool("AudioSource3D", pos, Quaternion.identity, clip.length);
        AudioSource aSource = currentGO.GetComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 1;
        aSource.maxDistance = maxDistanceFalloff;
        aSource.minDistance = minDistanceFalloff;
        aSource.Play();
        return aSource;
    }
}
