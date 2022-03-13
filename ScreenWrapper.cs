using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenWrapper : MonoBehaviour
{
    public delegate void Teleporting(GameObject gO);
    /// <summary>
    /// Executes before the game object is teleported (wrapped)
    /// </summary>
    public static event Teleporting OnTeleport;

    public LayerMask cullingMask;
    public static LayerMask _cullingMask { get; private set; } = 0;
    [Tooltip("Will The Camera Move around the scene? (More Expensive)")]public bool stationaryCamera;
    static Vector3 cameraWorldBounds;
    static Vector3 cameraLocalBounds;
    static Vector3[] cameraOffsets;
    public static List<Transform> wrappableObjects = new List<Transform>();
    private static List<GameObject> activeCameras = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        _cullingMask = cullingMask;
        UpdateCameraBounds();
        UpdateCameraOffsets();
        CreateCameras(cameraOffsets, cullingMask);
    }

    void CreateCameras(Vector3[] cameraPositions, LayerMask layer)
    {
        GameObject parent = new GameObject("ScreenWrapper");
        for(int i = 0; i < cameraPositions.Length; i ++) // Create 8 new cameras and place at specific locations relative to central camera
        {
            GameObject newCam = new GameObject("Cam_" + i, typeof(Camera));
            newCam.transform.parent = parent.transform;
            activeCameras.Add(newCam);
            Camera newCamCam = newCam.GetComponent<Camera>();
            newCamCam.cullingMask = layer;
            newCamCam.clearFlags = CameraClearFlags.Depth;
            newCamCam.orthographic = true;
            newCam.transform.position = cameraPositions[i];
        }
    }
    void MoveCameras()
    {
        for(int i = 0; i < activeCameras.Count; i ++)
        {
            activeCameras[i].transform.position = cameraOffsets[i];
            activeCameras[i].GetComponent<Camera>().depth = Camera.main.depth;
        }
    }
    void UpdateCameraBounds()
    {
        cameraWorldBounds = new Vector3(transform.position.x + Camera.main.orthographicSize * Screen.width / Screen.height, // Get the top left corner of the camera in worldSpace.
                                        transform.position.y + Camera.main.orthographicSize);
        cameraLocalBounds = new Vector3(Camera.main.orthographicSize * Screen.width / Screen.height, // Get the width of camera view in Local space.
                                        Camera.main.orthographicSize);
    }
    void UpdateCameraOffsets()
    {
        cameraOffsets = new Vector3[] {
            cameraWorldBounds - 3 * new Vector3(cameraLocalBounds.x, 0) + new Vector3(0, cameraLocalBounds.y) + new Vector3(0, 0, transform.position.z), // Cam_1
            cameraWorldBounds - new Vector3(cameraLocalBounds.x, 0) + new Vector3(0, cameraLocalBounds.y) + new Vector3(0, 0, transform.position.z), // Cam_2
            cameraWorldBounds + cameraLocalBounds, // Cam_3
            cameraWorldBounds - 3 * new Vector3(cameraLocalBounds.x, 0) - new Vector3(0, cameraLocalBounds.y) + new Vector3(0, 0, transform.position.z), // Cam_4
            // Skip Cam 5 because it's main camera
            cameraWorldBounds + new Vector3(cameraLocalBounds.x, 0) - new Vector3(0, cameraLocalBounds.y) + new Vector3(0, 0, transform.position.z), // Cam_6
            cameraWorldBounds - 3 * cameraLocalBounds + new Vector3(0, 0, transform.position.z), // Cam_7
            cameraWorldBounds - cameraLocalBounds - 2 * new Vector3(0, cameraLocalBounds.y) + new Vector3(0, 0, transform.position.z), // Cam_8
            cameraWorldBounds + new Vector3(cameraLocalBounds.x, 0) - 3 * new Vector3(0, cameraLocalBounds.y) + new Vector3(0, 0, transform.position.z) // Cam_9
    };
    }
    public void CheckPassingCameraBound(Transform gObject)
    {

        if (gObject.GetComponent<ScreenWrappingObject>().isInitialized)
        {
            if (gObject.position.x >= cameraWorldBounds.x)
            {
                if (OnTeleport != null) OnTeleport(gObject.gameObject);
                gObject.position -= 2 * new Vector3(cameraLocalBounds.x, 0);
            }
            if (gObject.position.y >= cameraWorldBounds.y)
            {
                if (OnTeleport != null) OnTeleport(gObject.gameObject);
                gObject.position -= 2 * new Vector3(0, cameraLocalBounds.y);
            }
            if (gObject.position.x <= (cameraWorldBounds - 2 * cameraLocalBounds).x)
            {
                if (OnTeleport != null) OnTeleport(gObject.gameObject);
                gObject.position += 2 * new Vector3(cameraLocalBounds.x, 0);
            }
            if (gObject.position.y <= (cameraWorldBounds - 2 * cameraLocalBounds).y)
            {
                if (OnTeleport != null) OnTeleport(gObject.gameObject);
                gObject.position += 2 * new Vector3(0, cameraLocalBounds.y);
            }
        }
    }
    public static bool IsSeenInMainCam(GameObject gO, bool fullObject = false)
    {
        Vector3 objectWidth = Vector3.zero;
        if (fullObject)
        {
            objectWidth = gO.GetComponent<Collider2D>().bounds.size;
        }
        if (gO.transform.position.x < cameraWorldBounds.x - objectWidth.x                               // Defining a bounding box for the camera view "site". 
         && gO.transform.position.x > (cameraWorldBounds - 2 * cameraLocalBounds).x + objectWidth.x     // - if fullObject is true, account for the width of the object and only return true
         && gO.transform.position.y < cameraWorldBounds.y - objectWidth.y                               // if the entire object('s collider) can be seen in the camera
         && gO.transform.position.y > (cameraWorldBounds - 2 * cameraLocalBounds).y + objectWidth.y)
            {
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if(!stationaryCamera)
        {
            UpdateCameraOffsets();
        }
        UpdateCameraBounds();
    }
    private void FixedUpdate()
    {
        foreach(Transform wrappableObject in wrappableObjects)
        {
            CheckPassingCameraBound(wrappableObject);
        }
    }
    private void LateUpdate()
    {
        if(!stationaryCamera)
        {
            MoveCameras();
        }
    }
}
