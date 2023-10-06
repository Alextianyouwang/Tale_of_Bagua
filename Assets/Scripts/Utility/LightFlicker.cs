using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.5f; // Minimum intensity of the spotlight
    public float maxIntensity = 1.5f; // Maximum intensity of the spotlight
    public float minFlickerInterval = 0.1f; // Minimum interval between flickers
    public float maxFlickerInterval = 0.5f; // Maximum interval between flickers

    private Light spotlight;
    private float baseIntensity;
    private bool flickering = false;

    private void Start()
    {
        spotlight = GetComponent<Light>();
        baseIntensity = 1;

        // Start the random flickering coroutine
        StartCoroutine(RandomFlicker());
    }

    private void Update()
    {
        // Check if the spotlight is currently flickering
        if (flickering)
        {
            // Generate a random intensity between the minimum and maximum values
            float randomIntensity = Random.Range(minIntensity, maxIntensity);

            // Apply the random intensity to the spotlight
            spotlight.intensity = baseIntensity * randomIntensity;
        }
    }

    private System.Collections.IEnumerator RandomFlicker()
    {
        while (true)
        {
            // Generate a random flicker interval
            float flickerInterval = Random.Range(minFlickerInterval, maxFlickerInterval);

            // Set the spotlight as flickering
            flickering = true;

            // Wait for the flicker interval
            yield return new WaitForSeconds(flickerInterval);

            flickerInterval = Random.Range(0.1f, 0.6f);                                                                                                                                         

            // Set the spotlight as not flickering
            flickering = false;

            // Reset the spotlight intensity
            spotlight.intensity = baseIntensity;

            yield return new WaitForSeconds(flickerInterval);
        }
    }
}
