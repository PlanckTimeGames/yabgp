using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public interface ICameraController
{
    Vector3 GetLocation();
    void MoveCamera(VertexPath cameraPath);
    Transform GetReferenceTransform();
}

public class CameraRotation : MonoBehaviour, ICameraController
{
    public PathCreator cameraPath;
    public float speed = 10.0f;
    private float distance = 0;
    private BezierPath mCameraBezierPath;
    private VertexPath mCameraPath;
    private GameObject mReferenceObj;
    // Start is called before the first frame update
    void Start()
    {
        mCameraBezierPath = new BezierPath(Vector3.zero);

        // transform.forward always points towards a unit circle around (0,0,0).
        var direction = transform.position;
        direction.y = 0;
        direction = direction.normalized * 1.1f;
        transform.forward = direction - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (mCameraPath != null)
        {
            if (distance < mCameraPath.length)
            {
                distance += speed * Time.deltaTime;
                transform.position = mCameraPath.GetPointAtDistance(distance, EndOfPathInstruction.Stop);

                // transform.forward always points towards a unit circle around (0,0,0).
                var direction = transform.position;
                direction.y = 0;
                direction = direction.normalized * 1.1f;
                transform.forward = direction - transform.position;
            }
            else
            {
                mCameraPath = null;
                distance = 0;
            }
        }
    }

    public Vector3 GetLocation()
    {
        return transform.position;
    }

    public void MoveCamera(VertexPath cameraPath)
    {
        mCameraPath = cameraPath;
        distance = 0;
    }

    public Transform GetReferenceTransform()
    {
        if (mReferenceObj == null)
            mReferenceObj = new GameObject();

        return mReferenceObj.transform;
    }
}
