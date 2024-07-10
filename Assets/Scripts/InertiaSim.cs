using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaSim : MonoBehaviour
{
    public Transform playerTransform;
    public List<Transform> BodiesHolder;
    public float inertiaMultiplier = 0.5f; // Multiplier for inertia effect
    public float swirlMultiplier = 0.2f; // Multiplier for swirl effect


    private Vector3 previousPosition;
    private float maxHeight;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        BodiesHolder = new List<Transform>();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = BodiesHolder.Count;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        previousPosition = playerTransform.position;

        if (BodiesHolder.Count > 0)
            UpdateLineRendererPoints();
    }

    

    void Start()
    {
        previousPosition = playerTransform.position;
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = playerTransform.position;
        Vector3 velocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;
        previousPosition = currentPosition;

        ApplyInertiaAndSwirl(velocity);
    }

    public void UpdateLineRendererPoints()
    {
        for (int i = 0; i < BodiesHolder.Count; i++)
        {
            print(BodiesHolder[i].transform.position);
            lineRenderer.positionCount = i+1;
            lineRenderer.SetPosition(i, BodiesHolder[i].position);
        }
    }

    void ApplyInertiaAndSwirl(Vector3 velocity)
    {
        if (velocity == Vector3.zero)
        {
            return; // No movement, no need to update
        }


        lineRenderer.SetPosition(0, BodiesHolder[0].position);
        for (int i = 1; i < BodiesHolder.Count; i++)
        {
           //inertia calcs
        }
    }
}
