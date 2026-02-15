//This script handles spraying the plushizer ray. The plushizer ray is spreads a ray to hit the infection particle effect
// and eliminate it over time.

using UnityEngine;

public class PlushizeController : MonoBehaviour
{
    [Header("Settings")]
    public float Cooldown = 1f;
    [SerializeField] float range = 20.0f;
    [SerializeField] LayerMask strikeableMask;  // This should be a blockable

    [Header("VFX References")]
    [SerializeField] PlushizeBeam plushizeBeam; 
    [SerializeField] AVPlayer plushizeHit;      
    [SerializeField] ParticleSystem plushizeParticleSystem; // Added this missing reference

    public void Fire()
    {
        // 1. Setup direction and points
        Vector3 fireDirection = transform.forward;
        Vector3 maxRangePoint = transform.position + (fireDirection * range);

        // 2. Visual Beam Logic
        Ray ray = new Ray(transform.position, fireDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, strikeableMask))
        {
            // Set the visual beam to end where the ray hits an object
            plushizeHit.transform.position = hit.point;
            plushizeBeam.EndPoint = hit.point;
            
            // Play the impact AV (audio/visual)
            plushizeHit.Play();
        }
        else
        {
            // If the ray hits nothing, the visual beam goes to max range
            plushizeBeam.EndPoint = maxRangePoint;
            
            // Note: We don't call plushizeHit.Play() here because there is no impact point
        }

        // 3. Spray Logic (The actual "Infection Killer")
        // We trigger the particles. The collision logic we wrote previously 
        // on the Infection object will handle the "load decrease."
        if (plushizeParticleSystem != null)
        {
            if (!plushizeParticleSystem.isEmitting)
            {
                plushizeParticleSystem.Play();
            }
        }

        // Ensure the beam object is visible
        if (plushizeBeam != null)
        {
            plushizeBeam.gameObject.SetActive(true);
        }
    }

    // Optional: Stop the particles when not firing
    public void StopFiring()
    {
        if (plushizeParticleSystem != null) plushizeParticleSystem.Stop();
        if (plushizeBeam != null) plushizeBeam.gameObject.SetActive(false);
    }
}