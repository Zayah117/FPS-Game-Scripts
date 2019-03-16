using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject mesh;
    [SerializeField] protected bool locked;
    public bool remainOpen;

    [SerializeField] protected List<Character> intersectedCharacters;

    void Start() {
        intersectedCharacters = new List<Character>();
    }

    public virtual void Update() {
        if (intersectedCharacters.Count > 0 && !locked) {
            Open();
            // Check for null characters
            if (intersectedCharacters.Contains(null)) {
                intersectedCharacters = intersectedCharacters.Where(c => c != null).ToList();
            }
        } else {
            if (!remainOpen) {
                Close();
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        Character character = other.gameObject.GetComponent<Character>();
        if (character) {
            if (!intersectedCharacters.Contains(character)) {
                intersectedCharacters.Add(character);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        Character character = other.gameObject.GetComponent<Character>();
        if (character) {
            if (intersectedCharacters.Contains(character)) {
                intersectedCharacters.Remove(character);
            }
        }
    }

    protected bool PlayerIntersected() {
        Character player = Managers.Gameplay.player;
        if (intersectedCharacters.Contains(player)) {
            return true;
        } else {
            return false;
        }
    }

    public void Open() {
        mesh.SetActive(false);
    }

    public void Close() {
        mesh.SetActive(true);
    }

    public void Lock() {
        locked = true;
    }

    public void Unlock() {
        locked = false;
    }

    public void OpenEvent() {
        remainOpen = true;
        Unlock();
        Open();
    }
}
