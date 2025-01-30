
//(c) Michael T. Lee

using UnityEngine;

/// <summary>
/// Manages Camera
/// </summary>
public class CameraManager {

	public RectTransform CamTransform { get; set; }

	public Camera Cam { get; set; }

	public float CameraSpeed { get; set; }

	public ExtVector MouseTarget
	{
		get
		{
			return new ExtVector(Cam.ScreenToWorldPoint(Input.mousePosition)) {Z = 0};
		}
	}

	public CameraManager(Camera camera, RectTransform rectTransform, float cameraSpeed)
	{
		Cam = camera;
		CamTransform = rectTransform;
		CameraSpeed = cameraSpeed;
	}

	public void GetCameraInput()
	{
		GetCameraZoom();
		GetCameraScrollInput();
	}

	/// <summary>
	/// Check for Camera Zoom
	/// </summary>
	private void GetCameraZoom()
	{
		var ortSize = Cam.orthographicSize;
		var newCamSizeDelta = (int)Input.mouseScrollDelta.y / 2;
		if (!(ortSize - newCamSizeDelta > 12 || ortSize - newCamSizeDelta < 3))
			Cam.orthographicSize -= newCamSizeDelta;
	}

	/// <summary>
	/// Check for camera scrolling
	/// </summary>
	private void GetCameraScrollInput()
	{
		var translation = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
			translation.y += CameraSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.S))
			translation.y -= CameraSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.A))
			translation.x -= CameraSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.D))
			translation.x += CameraSpeed * Time.deltaTime;
		CamTransform.Translate(translation);
	}




}
