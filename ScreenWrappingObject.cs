using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrappingObject : MonoBehaviour
{
    [HideInInspector] public bool isInitialized { get; private set; } = false;
    private void Awake()
    {
        ScreenWrapper.wrappableObjects.Add(transform);
    }
    private void OnDestroy()
    {
        ScreenWrapper.wrappableObjects.Remove(transform);
    }
    private void Update()
    {
        if(!isInitialized)
        {
            if (ScreenWrapper.IsSeenInMainCam(this.gameObject, true)) { // If the object is fully within the camera's view add a new layer so it can begin wrapping if it happens to go off screen again.
                gameObject.layer = (int)Mathf.Log(ScreenWrapper._cullingMask.value, 2); // No Clue why this works http://answers.unity.com/answers/1335946/view.html
                isInitialized = true; // Stop Repeating
            }

        }
    }
}
