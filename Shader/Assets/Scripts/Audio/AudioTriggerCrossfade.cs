using UnityEngine;
using System.Collections;

[AddComponentMenu("Audio/Audio Trigger Crossfade")]
public class AudioTriggerCrossfade : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Only react to colliders with this tag. Leave empty to react to anything.")]
    public string requiredTag = "Player";

    [Tooltip("If true, the enter crossfade will only run once. Exit will be ignored.")]
    public bool triggerOnce = false;

    [Header("Audio Sources")]
    [Tooltip("AudioSource that will fade OUT on enter (e.g., outside ambience).")]
    public AudioSource sourceFromOnEnter;

    [Tooltip("AudioSource that will fade IN on enter (e.g., inside ambience). If null and 'createTargetIfMissing' is true, one will be created.")]
    public AudioSource sourceToOnEnter;

    [Tooltip("If true and 'sourceToOnEnter' is null, an AudioSource will be created on a child GameObject when needed.")]
    public bool createTargetIfMissing = true;

    [Header("Enter (OnTriggerEnter)")]
    [Tooltip("Clip to play on enter (optional). If empty, the target AudioSource keeps its current clip.")]
    public AudioClip enterClip;

    [Tooltip("Loop setting for the target source on enter.")]
    public bool enterLoop = true;

    [Tooltip("Seconds to fade OUT the 'from' source on enter.")]
    [Min(0f)] public float enterFadeOut = 1.5f;

    [Tooltip("Seconds to fade IN the 'to' source on enter.")]
    [Min(0f)] public float enterFadeIn = 1.5f;

    [Tooltip("Curve used for both enter and exit fades (0..1 time, 0..1 volume).")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("Target volume for the 'to' source when fully faded in on enter.")]
    [Range(0f, 1f)] public float enterTargetVolume = 1f;

    [Tooltip("Start volume for the 'to' source before fading in on enter. Usually 0.")]
    [Range(0f, 1f)] public float enterStartVolume = 0f;

    [Header("Exit (OnTriggerExit) - optional reverse")]
    [Tooltip("When leaving the trigger, optionally crossfade back the other way.")]
    public bool reverseOnExit = true;

    [Tooltip("Clip to play on exit (optional). If empty, the target AudioSource keeps its current clip.")]
    public AudioClip exitClip;

    [Tooltip("Loop setting for the target source on exit.")]
    public bool exitLoop = true;

    [Tooltip("Seconds to fade OUT the 'from' source on exit.")]
    [Min(0f)] public float exitFadeOut = 1.5f;

    [Tooltip("Seconds to fade IN the 'to' source on exit.")]
    [Min(0f)] public float exitFadeIn = 1.5f;

    [Tooltip("Target volume for the 'to' source when fully faded in on exit.")]
    [Range(0f, 1f)] public float exitTargetVolume = 1f;

    [Tooltip("Start volume for the 'to' source before fading in on exit. Usually 0.")]
    [Range(0f, 1f)] public float exitStartVolume = 0f;

    [Header("Debug")]
    public bool debugLogs = false;

    private Coroutine _currentFade;
    private bool _hasEnteredOnce = false;

    void Reset()
    {
        var sources = GetComponents<AudioSource>();
        if (sources.Length > 0) sourceFromOnEnter = sources[0];
        if (sources.Length > 1) sourceToOnEnter = sources[1];
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsAllowed(other.gameObject)) return;
        if (debugLogs) Debug.Log("[AudioTriggerCrossfade] OnTriggerEnter with " + other.name);
        HandleEnter();
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsAllowed(other.gameObject)) return;
        if (debugLogs) Debug.Log("[AudioTriggerCrossfade] OnTriggerExit with " + other.name);
        HandleExit();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsAllowed(other.gameObject)) return;
        if (debugLogs) Debug.Log("[AudioTriggerCrossfade] OnTriggerEnter2D with " + other.name);
        HandleEnter();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsAllowed(other.gameObject)) return;
        if (debugLogs) Debug.Log("[AudioTriggerCrossfade] OnTriggerExit2D with " + other.name);
        HandleExit();
    }

    private bool IsAllowed(GameObject go)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !go.CompareTag(requiredTag))
            return false;
        return true;
    }

    private void HandleEnter()
    {
        if (triggerOnce && _hasEnteredOnce)
        {
            if (debugLogs) Debug.Log("[AudioTriggerCrossfade] Enter ignored: triggerOnce already used.");
            return;
        }
        _hasEnteredOnce = true;

        AudioSource fromSrc = sourceFromOnEnter;
        AudioSource toSrc = EnsureTargetSource(sourceToOnEnter);

        if (toSrc == null)
        {
            if (debugLogs) Debug.LogWarning("[AudioTriggerCrossfade] No target source available on enter.");
            return;
        }

        if (enterClip != null)
        {
            toSrc.clip = enterClip;
        }
        toSrc.loop = enterLoop;

        StartCrossfade(fromSrc, toSrc, enterFadeOut, enterFadeIn, enterStartVolume, enterTargetVolume);
    }

    private void HandleExit()
    {
        if (triggerOnce)
        {
            if (debugLogs) Debug.Log("[AudioTriggerCrossfade] Exit ignored due to triggerOnce.");
            return;
        }

        if (!reverseOnExit) return;

        AudioSource fromSrc = sourceToOnEnter != null ? sourceToOnEnter : sourceFromOnEnter;
        AudioSource toSrc = sourceFromOnEnter;

        if (toSrc == null)
        {
            if (debugLogs) Debug.LogWarning("[AudioTriggerCrossfade] Exit requested but 'sourceFromOnEnter' is not assigned.");
            return;
        }

        if (exitClip != null)
        {
            toSrc.clip = exitClip;
        }
        toSrc.loop = exitLoop;

        StartCrossfade(fromSrc, toSrc, exitFadeOut, exitFadeIn, exitStartVolume, exitTargetVolume);
    }

    private AudioSource EnsureTargetSource(AudioSource candidate)
    {
        if (candidate != null) return candidate;

        if (!createTargetIfMissing)
        {
            Debug.LogError("[AudioTriggerCrossfade] 'sourceToOnEnter' is null and 'createTargetIfMissing' is false.");
            return null;
        }

        var child = new GameObject("AudioTriggerCrossfade_Target");
        child.transform.SetParent(transform);
        child.transform.localPosition = Vector3.zero;
        var newSrc = child.AddComponent<AudioSource>();
        newSrc.playOnAwake = false;
        sourceToOnEnter = newSrc;
        return newSrc;
    }

    private void StartCrossfade(
        AudioSource fromSrc,
        AudioSource toSrc,
        float fadeOutDuration,
        float fadeInDuration,
        float toStartVol,
        float toTargetVol)
    {
        if (_currentFade != null)
            StopCoroutine(_currentFade);

        _currentFade = StartCoroutine(CrossfadeRoutine(fromSrc, toSrc, fadeOutDuration, fadeInDuration, toStartVol, toTargetVol));
    }

    private IEnumerator CrossfadeRoutine(
        AudioSource fromSrc,
        AudioSource toSrc,
        float fadeOutDuration,
        float fadeInDuration,
        float toStartVol,
        float toTargetVol)
    {
        if (toSrc == null && fromSrc == null)
        {
            if (debugLogs) Debug.LogWarning("[AudioTriggerCrossfade] No AudioSources assigned.");
            yield break;
        }

        float fromStartVol = 0f;
        if (fromSrc != null)
            fromStartVol = fromSrc.volume;

        if (toSrc != null)
        {
            if (toSrc.clip != null && !toSrc.isPlaying)
                toSrc.Play();

            toStartVol = Mathf.Clamp01(toStartVol);
            toTargetVol = Mathf.Clamp01(toTargetVol);
            toSrc.volume = toStartVol;
        }

        float t = 0f;
        float maxDuration = Mathf.Max(fadeOutDuration, fadeInDuration);
        if (maxDuration <= 0f)
        {
            if (fromSrc != null)
            {
                fromSrc.volume = 0f;
                fromSrc.Stop();
            }
            if (toSrc != null)
                toSrc.volume = toTargetVol;

            _currentFade = null;
            yield break;
        }

        while (t < maxDuration)
        {
            t += Time.deltaTime;
            float normOut = Mathf.Clamp01(fadeOutDuration > 0f ? t / fadeOutDuration : 1f);
            float normIn  = Mathf.Clamp01(fadeInDuration  > 0f ? t / fadeInDuration  : 1f);

            float curveOut = fadeCurve != null ? fadeCurve.Evaluate(normOut) : normOut;
            float curveIn  = fadeCurve != null ? fadeCurve.Evaluate(normIn)  : normIn;

            if (fromSrc != null)
                fromSrc.volume = Mathf.Lerp(fromStartVol, 0f, curveOut);

            if (toSrc != null)
                toSrc.volume = Mathf.Lerp(toStartVol, toTargetVol, curveIn);

            yield return null;
        }

        if (fromSrc != null)
        {
            fromSrc.volume = 0f;
            fromSrc.Stop();
        }
        if (toSrc != null)
        {
            toSrc.volume = toTargetVol;
            if (!toSrc.isPlaying && toSrc.clip != null)
                toSrc.Play();
        }

        _currentFade = null;
    }
}
