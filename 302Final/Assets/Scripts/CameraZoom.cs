using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    CinemachineCamera cam;
    CinemachinePositionComposer positionComposer;

    public float defaultOrthoSize = 5;
    public Vector2 defaultScreenPosition = new Vector2(0, 2);

    public float zoomedInOrthoSize = 3;
    public Vector2 zoomedInScreenPosition = new Vector2(0, 0);

    public bool canZoomInCamera = false;
    public bool canZoomOutCamera = false;

    public float zoomInRate = 1;
    public float zoomOutRate = 1;

    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        positionComposer = GetComponent<CinemachinePositionComposer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canZoomInCamera)
        {
            if (cam.Lens.OrthographicSize >= zoomedInOrthoSize) {
                cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, zoomedInOrthoSize, Time.unscaledDeltaTime * zoomInRate);
            }
            else
            {
                canZoomInCamera=false;
                cam.Lens.OrthographicSize = zoomedInOrthoSize;
            }
        }
        if (canZoomOutCamera) 
        {
            if (cam.Lens.OrthographicSize <= defaultOrthoSize)
            {
                cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, defaultOrthoSize, Time.unscaledDeltaTime * zoomOutRate);
            }
            else
            {
                canZoomOutCamera = false;
                cam.Lens.OrthographicSize = defaultOrthoSize;
            }
        }
    }

    public void OnSlow(bool isSlowed)
    {
        if (isSlowed) { ZoomInCamera(); }
        else { ZoomOutCamera(); }
    }

    void ZoomInCamera()
    {
        if (cam == null) return;

        canZoomInCamera=true;
        canZoomOutCamera=false;
    }
    void ZoomOutCamera()
    {
        if (cam == null) return;

        canZoomOutCamera=true;
        canZoomInCamera=false;
    }
}
