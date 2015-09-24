using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour {

	[HideInInspector] public static CameraManager cameraManager;

	public Button cameraLockButton;
	public Image cameraLockIcon;
	public Sprite lockedSprite, unlockedSprite;

	public float smoothness;
	public Slider distanceSlider;
	private Transform activeObject;
	private Camera cam;

	// Second attempt at camera controls
	public float distance = 5.0f;
	public float mouseSensitivity = 2.0f, cameraControlSensitivity = 2.0f;
	public float yMinAngle = 0f, yMaxAngle = 85f, xMinAngle = 90f, xMaxAngle = 270f;
	public float distanceMin = 1f, distanceMax = 20f;
	public float distanceScale;

	public bool locked = false;

	private Quaternion interimRotation;
	private float interimAngleY;

	private Rigidbody rigidbody;
	public float x = 0.0f, y = 0.0f;
	private Vector3 targetPosition;
	private Quaternion targetRotation;
	private bool updateTransform = false;

	void Awake()
	{
		if (cameraManager == null) 
		{
			cameraManager = this;
		}
	}

	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		rigidbody = GetComponent<Rigidbody> ();
		if (rigidbody != null) 
		{
			rigidbody.freezeRotation = true;
		}
	}

	void LateUpdate()
	{
		if (activeObject) 
		{
			if(Input.GetMouseButton(1) && !locked)
			{
				updateTransform = true;
				x += Input.GetAxis("Mouse X") * mouseSensitivity;
				y -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
				distance = Mathf.Clamp (distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
				distanceSlider.value = Mathf.Lerp (0f,1f,((distance-distanceMin)/(distanceMax-distanceMin)));
			}
			// else if (touch controls)
			if(updateTransform)
			{
				updateTransform = false;

				y = ClampAngle(y, yMinAngle, yMaxAngle);
				x = ClampAngle(x, xMinAngle, xMaxAngle);

				Quaternion rotation = Quaternion.Euler(y, x, 0);

				Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
				Vector3 position = rotation * negDistance + activeObject.position;

				targetRotation = rotation;
				targetPosition = position;
			}
			if(targetPosition != null && targetRotation != null)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothness * Time.deltaTime);
				transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);
			}
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F) 
		{
			angle += 360F;
		}
		if (angle > 360F) 
		{
			angle -= 360F;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public void testFunction ()
	{
		float xForward = Mathf.Cos (transform.rotation.eulerAngles.x * Mathf.Deg2Rad) * Mathf.Sin (transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
		float yforward = - Mathf.Sin (transform.rotation.eulerAngles.x * Mathf.Deg2Rad);
		float zForward = Mathf.Cos (transform.rotation.eulerAngles.x  * Mathf.Deg2Rad) * Mathf.Cos (transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
		Vector3 forward = new Vector3 (xForward, yforward, zForward).normalized;
		print (forward);
		print (transform.rotation);
		print (-transform.forward);
	}

	public void MoveToObject (Transform obj)
	{
		activeObject = obj;
		float minDistScalar = 1.1f;
		distanceMin = minDistScalar * Mathf.Max (activeObject.lossyScale.x, activeObject.lossyScale.y, activeObject.lossyScale.z);
		distanceMax = distanceMin * distanceScale;

		updateTransform = true;

	}
	
	public void ChangeVerticalAngle (int direction)
	{
		// Change currentVerticalAngleIndex
		int dir = (int) Mathf.Sign (direction);
		y += dir * cameraControlSensitivity;

		MoveToObject (activeObject);

	}

	public void ChangeHorizontalAngle (int direction)
	{
		// Change currentHorizontalAngleIndex
		int dir = (int) Mathf.Sign (direction);
		x += dir * cameraControlSensitivity;
		MoveToObject (activeObject);
	}

	public void SetDistance (float newDistance)
	{
		//distance = newDistance;// * distanceMax;
		distance = Mathf.Lerp (distanceMin, distanceMax, newDistance);

		MoveToObject (activeObject);

	}
	public void sliderMove()
	{
		SetDistance(distanceSlider.value);
	}

	public void ToggleCameraLock()
	{
		locked = !locked;
		if (locked) 
		{
			cameraLockIcon.sprite = lockedSprite;
		} 
		else 
		{
			cameraLockIcon.sprite = unlockedSprite;
		}
	}

	public void ReturnToInterimRotation()
	{
		//targetRotation = interimRotation;
		y = interimAngleY;
		MoveToObject (activeObject);
	}

	public void TopDownView()
	{
		interimRotation = transform.rotation;
		interimAngleY = y;
		y = yMaxAngle;
		MoveToObject (activeObject);
	}
}
