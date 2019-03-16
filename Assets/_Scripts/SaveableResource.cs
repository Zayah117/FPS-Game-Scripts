using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveableResource : SaveableObject
{
    // Instantiate Saveable Resource methods
    public static SaveableResource InstantiateSaveableResource(SaveableResource obj) {
        SaveableResource myObject = Instantiate(obj);

        // myObject.instantiated = true;
        myObject.SubscribeManager();

        return myObject;
    }

    public static SaveableResource InstantiateSaveableResource(SaveableResource obj, Transform parent) {
        SaveableResource myObject = Instantiate(obj, parent);

        // myObject.instantiated = true;
        myObject.SubscribeManager();

        return myObject;
    }

    public static SaveableResource InstantiateSaveableResource(SaveableResource obj, Transform parent, bool instantiateInWorldSpace) {
        SaveableResource myObject = Instantiate(obj, parent, instantiateInWorldSpace);

        // myObject.instantiated = true;
        myObject.SubscribeManager();

        return myObject;
    }

    public static SaveableResource InstantiateSaveableResource(SaveableResource obj, Vector3 position, Quaternion rotation) {
        SaveableResource myObject = Instantiate(obj, position, rotation);

        // myObject.instantiated = true;
        myObject.SubscribeManager();

        return myObject;
    }

    public static SaveableResource InstantiateSaveableResource(SaveableResource obj, Vector3 position, Quaternion rotation, Transform parent) {
        SaveableResource myObject = Instantiate(obj, position, rotation, parent);

        // myObject.instantiated = true;
        myObject.SubscribeManager();

        return myObject;
    }
}
