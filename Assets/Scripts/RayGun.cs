using UnityEngine;

/// <summary>
/// Represents RayGun object
/// </summary>
public class RayGun : MonoBehaviour
{
    public LayerMask layerMask;
    public OVRInput.RawButton shootingButton;
    public LineRenderer linePrefab;
    public GameObject rayImpactPrefab;
    public Transform shootingPoint; // Ray Starting Point
    public float maxLineDistance = 5.0f; // Ray max length
    public float lineShowTimer = 0.3f;  // Time length that line survives
    public AudioSource source;  // Sound player
    public AudioClip shootingAudioClip; // Sound container

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(shootingButton))
        {
            Shoot();
        }
    }

    /// <summary>
    /// Shoot the ray from the ray gun.
    /// </summary>
    public void Shoot()
    {
        Debug.Log($"[{this.name}] Pew Pew");

        source.PlayOneShot(shootingAudioClip);

        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);   // Memo: No argument to specify the endPoint.
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, maxLineDistance, layerMask); // Physics.Raycast(Ray, HitInfo, MaxDistance, LayerMask)
                                                                                            // Return true if it intersects any collider.
        Vector3 endPoint = Vector3.zero;    // (0, 0, 0)

        if(hasHit)
        {
            // Stop the ray
            endPoint = hit.point;

            Ghost ghost = hit.transform.GetComponentInParent<Ghost>();  // Mesh collider belongs to mesh body that's child object of Ghost that has Ghost component.

            if(ghost)   // If hit object has ghost component
            {
                // Disable Ghost so that it cannot be hit twice or more
                hit.collider.enabled = false;

                //Kill Ghost
                ghost.Kill();
            }
            else
            {
                Quaternion rayImpactRotation = Quaternion.LookRotation(-hit.normal); // Reverse the direction of normal because RayImpact object can be only seen from the behind
                GameObject rayImapct = Instantiate(rayImpactPrefab, hit.point, rayImpactRotation);
                Destroy(rayImapct, 0.7f);
            }
        }
        else
        {
            endPoint = shootingPoint.position + shootingPoint.forward * maxLineDistance; // foward should be a unit vector 
        }

        // Visualize Ray
        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2; // Line has 2 vertex
        line.SetPosition(0, shootingPoint.position); // SetPosition(Vertex index, its position)
        line.SetPosition(1, endPoint);

        Destroy(line.gameObject, lineShowTimer);    // Destroy(targetObj, time to wait before destroying)
    }
}
