using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSoundHelper : MonoBehaviour {
    [SerializeField] AudioClip[] sounds;

    void PlaySound(int index) {
        if (sounds.Length - 1 >= index) {
            Managers.Audio.PlaySound2D(sounds[index]);
        } else {
            Debug.LogWarning("sound list in PlayerWeaponSoundHelper index " + index.ToString() + " does not exist.");
        }
    }

    public void Sound0() {
        PlaySound(0);
    }

    public void Sound1() {
        PlaySound(1);
    }

    public void Sound2() {
        PlaySound(2);
    }

    public void Sound3() {
        PlaySound(3);
    }
}
