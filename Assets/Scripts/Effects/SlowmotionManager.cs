using Meta.Voice.Samples.TTSVoices;
using Meta.WitAi.TTS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowmotionManager : MonoBehaviour
{
    [SerializeField] private float slowMotionTime;
    [SerializeField] private float slowMotionTransitionSpeed = 2;
    [SerializeField] private float narrationBloomIntensity;

    private float originalBloomIntensity;

    [SerializeField] private Volume volume;
    private Bloom bloom;
    public bool inSlowMotion;

    private int transitionDirection;

    public delegate void OnEndSlowmotion();
    public static event OnEndSlowmotion OnEndSlowmotionEvent;

    public void OnEnable()
    {
        TTSSpeakerInput.OnEndNarrationEvent += DisableSlowMotion;
    }
    private void OnDisable()
    {
        TTSSpeakerInput.OnEndNarrationEvent -= DisableSlowMotion;
    }
    public void EnableSlowMotion()
    {
        if (inSlowMotion) return;

        transitionDirection = -1;
        inSlowMotion = true;

    }
    public void DisableSlowMotion()
    {
        transitionDirection = 1;
    }
    public void ForceStopSlowmotion()
    {
        if (!inSlowMotion) return;

        transitionDirection = 0;
        inSlowMotion = false;
    }
    private void Update()
    {
        if (inSlowMotion && transitionDirection != 0)
        {
            if (!bloom)
            {
                volume.profile.TryGet(out bloom);
                originalBloomIntensity = bloom.intensity.value;
            }

            Time.timeScale += Time.deltaTime * slowMotionTransitionSpeed * transitionDirection;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            if (Mathf.Sign(transitionDirection) == -1)
            {
                bloom.intensity.value = Mathf.Lerp(originalBloomIntensity, narrationBloomIntensity, 0.25f / Mathf.Clamp(Time.timeScale,0.25f,1) - 0.25f);
            }
            else
            {
                bloom.intensity.value = Mathf.Lerp(narrationBloomIntensity, originalBloomIntensity, Mathf.Clamp(Time.timeScale, 0.25f, 1));
            }
        }

        if (transitionDirection == -1 && Time.timeScale <= 0.25f)
        {
            transitionDirection = 0;
            Time.timeScale = 0.25f;
        }
        if (transitionDirection == 1 && Time.timeScale >= 1)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;

            transitionDirection = 0;
            inSlowMotion = false;

            OnEndSlowmotionEvent();

        }
    }


}
