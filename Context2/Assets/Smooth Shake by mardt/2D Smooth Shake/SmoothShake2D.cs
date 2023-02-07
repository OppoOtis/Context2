using System.Collections;
using UnityEngine;

namespace SmoothShake
{
    public class SmoothShake2D : MonoBehaviour
    {
        /*

        Thx for purchasing Smooth Shake 2D

        Made by mardt (also known as daburo)

        Documentation and usage explanation video can be found in the documentation.pdf file

        */

        [SerializeField] bool constantShake = false;
        [Header("Settings for fading in / out")]
        [SerializeField] bool fadeIn = false;
        [SerializeField] bool fadeOut = true;
        [SerializeField] float fadeOutDuration = 1f;
        [SerializeField] float fadeInDuration = 1f;
        [Tooltip("The amount of time you want the camera to shake before / after fading")]
        [SerializeField] float additionalShakeDuration = 0f;

        [Header("Shake settings")]
        [SerializeField] float positionIntensity = 1f;
        [SerializeField] float xPositionFrequency = 52f;
        [SerializeField] float yPositionFrequency = 42f;
        [Tooltip("Generally between 0 and 1")]
        [SerializeField] float rotationIntensity = 0.1f;
        [SerializeField] float rotationFrequency = 23f;

        bool saveConstantShake = false;
        bool saveFadeIn = false;
        bool saveFadeOut = true;
        float saveFadeOutDuration = 1f;
        float saveFadeInDuration = 1f;
        float saveAdditionalShakeDuration = 0f;
        float savePositionIntensity = 1f;
        float savexPositionFrequency = 52f;
        float saveyPositionFrequency = 42f;
        float saveRotationIntensity = 0.1f;
        float saveRotationFrequency = 23f;

        private bool shakeStarted = false;
        private Vector3 startPosition;
        private float rot;
        private float newPositionIntensity = 1f;
        private float newRotationIntensity = 1f;
        private bool shaking = false;

        private void Awake()
        {
            saveConstantShake = constantShake;
            saveFadeIn = fadeIn;
            saveFadeOut = fadeOut;
            saveFadeOutDuration = fadeOutDuration;
            saveFadeInDuration = fadeInDuration;
            saveAdditionalShakeDuration = additionalShakeDuration;
            savePositionIntensity = positionIntensity;
            savexPositionFrequency = xPositionFrequency;
            saveyPositionFrequency = yPositionFrequency;
            saveRotationIntensity = rotationIntensity;
            saveRotationFrequency = rotationFrequency;
        }

        public void StartShake()
        {
            if (!shaking)
            {
                shaking = true;

                startPosition = transform.localPosition;

                rot = transform.localRotation.z;

                newPositionIntensity = positionIntensity;
                newRotationIntensity = rotationIntensity;

                shakeStarted = true;

                if (!constantShake)
                {
                    if (fadeOut && !fadeIn)
                    {
                        StartCoroutine(ShakeDurationTimer(additionalShakeDuration));
                    }
                    else if (fadeIn && !fadeOut)
                    {
                        StartCoroutine(ShakeFadeIn(fadeInDuration));
                    }
                    else if (fadeIn && fadeOut)
                    {
                        StartCoroutine(ShakeFadeIn(fadeInDuration));
                    }
                    else if (!fadeIn && !fadeOut)
                    {
                        Debug.LogError("No shake activated. You need to at least activate a constant shake or a fade in / fade out");
                    }
                    else
                    {
                        Debug.LogError("No shake activated. Try checking / unchecking fade in or fade out");
                    }
                }

                if (constantShake)
                {
                    if (fadeOut)
                    {
                        Debug.LogError("Constant shake can't have a fade out");
                    }
                    if (fadeIn)
                    {
                        StartCoroutine(ShakeFadeIn(fadeInDuration));
                    }
                }
            }

        }

        public void CustomShakeFadeIn(float newFadeInDuration, float newAdditionalShakeDuration, bool shakeUntilStopped, float customPositionIntensity, float customXFrequency, float customYFrequency, float customRotationIntensity, float customRotationFrequency)
        {
            fadeIn = true;
            fadeOut = false;

            fadeInDuration = newFadeInDuration;
            additionalShakeDuration = newAdditionalShakeDuration;
            constantShake = shakeUntilStopped;
            positionIntensity = customPositionIntensity;
            xPositionFrequency = customXFrequency;
            yPositionFrequency = customYFrequency;
            rotationIntensity = customRotationIntensity;
            rotationFrequency = customRotationFrequency;

            StartShake();
        }

        public void SimpleShakeFadeIn(float newFadeInDuration, bool shakeUntilStopped, float customPositionIntensity, float customRotationIntensity)
        {
            fadeIn = true;
            fadeOut = false;

            fadeInDuration = newFadeInDuration;
            constantShake = shakeUntilStopped;
            positionIntensity = customPositionIntensity;
            rotationIntensity = customRotationIntensity;

            StartShake();
        }

        public void SimpleShakeFadeOut(float newFadeOutDuration, float customPositionIntensity, float customRotationIntensity)
        {
            fadeIn = false;
            fadeOut = true;

            constantShake = false;

            fadeInDuration = newFadeOutDuration;
            positionIntensity = customPositionIntensity;
            rotationIntensity = customRotationIntensity;

            StartShake();
        }

        public void CustomShakeFadeOut(float newFadeOutDuration, float newAdditionalShakeDuration, float customPositionIntensity, float customXFrequency, float customYFrequency, float customRotationIntensity, float customRotationFrequency)
        {
            fadeIn = false;
            fadeOut = true;

            constantShake = false;

            fadeInDuration = newFadeOutDuration;
            additionalShakeDuration = newAdditionalShakeDuration;
            positionIntensity = customPositionIntensity;
            xPositionFrequency = customXFrequency;
            yPositionFrequency = customYFrequency;
            rotationIntensity = customRotationIntensity;
            rotationFrequency = customRotationFrequency;

            StartShake();
        }

        public void SimpleShakeFadeInOut(float newFadeInDuration, float newFadeOutDuration, float newAdditionalShakeDuration, float customPositionIntensity, float customRotationIntensity)
        {
            fadeIn = true;
            fadeOut = true;

            constantShake = false;

            fadeInDuration = newFadeInDuration;
            fadeOutDuration = newFadeOutDuration;
            additionalShakeDuration = newAdditionalShakeDuration;
            positionIntensity = customPositionIntensity;
            rotationIntensity = customRotationIntensity;

            StartShake();
        }

        public void CustomShakeFadeInOut(float newFadeInDuration, float newFadeOutDuration, float newAdditionalShakeDuration, float customPositionIntensity, float customXFrequency, float customYFrequency, float customRotationIntensity, float customRotationFrequency)
        {
            fadeIn = true;
            fadeOut = true;

            constantShake = false;

            fadeInDuration = newFadeInDuration;
            fadeOutDuration = newFadeOutDuration;
            additionalShakeDuration = newAdditionalShakeDuration;
            positionIntensity = customPositionIntensity;
            xPositionFrequency = customXFrequency;
            yPositionFrequency = customYFrequency;
            rotationIntensity = customRotationIntensity;
            rotationFrequency = customRotationFrequency;

            StartShake();
        }

        public void fadeOutCurrentShake(float duration)
        {
            StartCoroutine(ShakeFadeOut(duration));
        }

        public void StopShake()
        {
            shakeStarted = false;
            StopAllCoroutines();
            transform.localPosition = startPosition;
            returnDefaultValues();
            shaking = false;
        }

        private void Update()
        {
            if (shakeStarted)
            {
                float x = startPosition.x + Mathf.Sin(Time.time * xPositionFrequency) * newPositionIntensity;
                float y = startPosition.y + Mathf.Sin(Time.time * yPositionFrequency) * newPositionIntensity;
                transform.localPosition = new Vector3(x, y, startPosition.z);

                float rotShake = Mathf.Sin(Time.time * rotationFrequency) * newRotationIntensity;

                transform.localRotation = Quaternion.Euler(0, 0, rotShake);
            }
        }

        IEnumerator ShakeDurationTimer(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (!constantShake)
            {
                if (fadeOut)
                {
                    StartCoroutine(ShakeFadeOut(fadeOutDuration));
                }
                if (!fadeOut)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, rot);
                    transform.localPosition = startPosition;
                    returnDefaultValues();
                    shaking = false;
                }
            }

            yield return null;

        }


        IEnumerator ShakeFadeOut(float duration)
        {

            float startMagnitude = newPositionIntensity;
            float startRotMagnitude = newRotationIntensity;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                newPositionIntensity = Mathf.Lerp(startMagnitude, 0, t / duration);
                newRotationIntensity = Mathf.Lerp(startRotMagnitude, 0, t / duration);

                yield return null;
            }
            transform.localRotation = Quaternion.Euler(0, 0, rot);
            transform.localPosition = startPosition;

            returnDefaultValues();
            shakeStarted = false;
            shaking = false;

            yield return null;
        }

        IEnumerator ShakeFadeIn(float duration)
        {

            float startMagnitude = newPositionIntensity;
            float startRotMagnitude = newRotationIntensity;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                newPositionIntensity = Mathf.Lerp(0, startMagnitude, t / duration);
                newRotationIntensity = Mathf.Lerp(0, startRotMagnitude, t / duration);

                yield return null;
            }

            StartCoroutine(ShakeDurationTimer(additionalShakeDuration));

            yield return null;
        }

        private void returnDefaultValues()
        {
            constantShake = saveConstantShake;
            fadeIn = saveFadeIn;
            fadeOut = saveFadeOut;
            fadeOutDuration = saveFadeOutDuration;
            fadeInDuration = saveFadeInDuration;
            additionalShakeDuration = saveAdditionalShakeDuration;
            positionIntensity = savePositionIntensity;
            xPositionFrequency = savexPositionFrequency;
            yPositionFrequency = saveyPositionFrequency;
            rotationIntensity = saveRotationIntensity;
            rotationFrequency = saveRotationFrequency;
        }

        //Script by mardt 
    }
}
