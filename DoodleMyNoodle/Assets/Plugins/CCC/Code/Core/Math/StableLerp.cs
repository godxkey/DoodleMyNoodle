//
//   Written by Frederic Bessette, August 30 2018
//

using UnityEngine;

public static class StableLerps
{
    public enum UpdateType
    {
        /// <summary>
        /// Use this if you lerp within a standard Update()
        /// </summary>
        Update,

        /// <summary>
        /// Use this if you lerp within a FixedUpdate()
        /// </summary>
        FixedUpdate,

        /// <summary>
        /// Use this if you lerp within a standard Update() AND you don't want it to be affected by timescale
        /// </summary>
        UnscaledUpdate,

        /// <summary>
        /// Use this if you lerp within a FixedUpdate() AND you don't want it to be affected by timescale
        /// </summary>
        UnscaledFixedUpdate
    }

    /// <summary>
    /// Use this if you lerp within a standard Update()
    /// </summary>
    public static float StableLerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, StabilizeT(t, FramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a FixedUpdate()
    /// </summary>
    public static float StableLerpFixedUpdate(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, StabilizeT(t, FixedFramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a perculiar update setting (e.g. If you have your own timescale handling)
    /// </summary>
    public static float StableLerpCustom(float a, float b, float t, UpdateType updateType, float timescaleMultiplier = 1)
    {
        float fps;
        switch (updateType)
        {
            default:
            case UpdateType.Update:
                fps = FramesPerSecond;
                break;
            case UpdateType.FixedUpdate:
                fps = FixedFramesPerSecond;
                break;
            case UpdateType.UnscaledUpdate:
                fps = UnscaledFramesPerSecond;
                break;
            case UpdateType.UnscaledFixedUpdate:
                fps = UnscaledFixedFramesPerSecond;
                break;
        }
        return Mathf.Lerp(a, b, StabilizeT(t, fps * timescaleMultiplier));
    }
    /// <summary>
    /// Use this if you lerp with a custom update rate (e.g. You update your system 2 every frame -> updateRate=2)
    /// </summary>
    public static float StableLerpCustom(float a, float b, float t, float updateRate)
    {
        return Mathf.Lerp(a, b, StabilizeT(t, updateRate));
    }

    /// <summary>
    /// Use this if you lerp within a standard Update()
    /// </summary>
    public static float StableLerpAngle(float a, float b, float t)
    {
        return Mathf.LerpAngle(a, b, StabilizeT(t, FramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a FixedUpdate()
    /// </summary>
    public static float StableLerpAngleFixedUpdate(float a, float b, float t)
    {
        return Mathf.LerpAngle(a, b, StabilizeT(t, FixedFramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a perculiar update setting (e.g. If you have your own timescale handling)
    /// </summary>
    public static float StableLerpAngleCustom(float a, float b, float t, UpdateType updateType, float timescaleMultiplier = 1)
    {
        float fps;
        switch (updateType)
        {
            default:
            case UpdateType.Update:
                fps = FramesPerSecond;
                break;
            case UpdateType.FixedUpdate:
                fps = FixedFramesPerSecond;
                break;
            case UpdateType.UnscaledUpdate:
                fps = UnscaledFramesPerSecond;
                break;
            case UpdateType.UnscaledFixedUpdate:
                fps = UnscaledFixedFramesPerSecond;
                break;
        }
        return Mathf.LerpAngle(a, b, StabilizeT(t, fps * timescaleMultiplier));
    }
    /// <summary>
    /// Use this if you lerp with a custom update rate (e.g. You update your system 2 every frame -> updateRate=2)
    /// </summary>
    public static float StableLerpAngleCustom(float a, float b, float t, float updateRate)
    {
        return Mathf.LerpAngle(a, b, StabilizeT(t, updateRate));
    }

    /// <summary>
    /// Use this if you lerp within a standard Update()
    /// </summary>
    public static Vector2 StableLerp(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, StabilizeT(t, FramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a FixedUpdate()
    /// </summary>
    public static Vector2 StableLerpFixedUpdate(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, StabilizeT(t, FixedFramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a perculiar update setting (e.g. If you have your own timescale handling)
    /// </summary>
    public static Vector2 StableLerpCustom(Vector2 a, Vector2 b, float t, UpdateType updateType, float timescaleMultiplier = 1)
    {
        float fps;
        switch (updateType)
        {
            default:
            case UpdateType.Update:
                fps = FramesPerSecond;
                break;
            case UpdateType.FixedUpdate:
                fps = FixedFramesPerSecond;
                break;
            case UpdateType.UnscaledUpdate:
                fps = UnscaledFramesPerSecond;
                break;
            case UpdateType.UnscaledFixedUpdate:
                fps = UnscaledFixedFramesPerSecond;
                break;
        }
        return Vector2.Lerp(a, b, StabilizeT(t, fps * timescaleMultiplier));
    }
    /// <summary>
    /// Use this if you lerp with a custom update rate (e.g. You update your system 2 every frame -> updateRate=2)
    /// </summary>
    public static Vector2 StableLerpCustom(Vector2 a, Vector2 b, float t, float updateRate)
    {
        return Vector2.Lerp(a, b, StabilizeT(t, updateRate));
    }

    /// <summary>
    /// Use this if you lerp within a standard Update()
    /// </summary>
    public static Vector3 StableLerp(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, StabilizeT(t, FramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a FixedUpdate()
    /// </summary>
    public static Vector3 StableLerpFixedUpdate(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, StabilizeT(t, FixedFramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a perculiar update setting (e.g. If you have your own timescale handling)
    /// </summary>
    public static Vector3 StableLerpCustom(Vector3 a, Vector3 b, float t, UpdateType updateType, float timescaleMultiplier = 1)
    {
        float fps;
        switch (updateType)
        {
            default:
            case UpdateType.Update:
                fps = FramesPerSecond;
                break;
            case UpdateType.FixedUpdate:
                fps = FixedFramesPerSecond;
                break;
            case UpdateType.UnscaledUpdate:
                fps = UnscaledFramesPerSecond;
                break;
            case UpdateType.UnscaledFixedUpdate:
                fps = UnscaledFixedFramesPerSecond;
                break;
        }
        return Vector3.Lerp(a, b, StabilizeT(t, fps * timescaleMultiplier));
    }
    /// <summary>
    /// Use this if you lerp with a custom update rate (e.g. You update your system 2 every frame -> updateRate=2)
    /// </summary>
    public static Vector3 StableLerpCustom(Vector3 a, Vector3 b, float t, float updateRate)
    {
        return Vector3.Lerp(a, b, StabilizeT(t, updateRate));
    }

    /// <summary>
    /// Use this if you lerp within a standard Update()
    /// </summary>
    public static Quaternion StableLerp(Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Lerp(a, b, StabilizeT(t, FramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a FixedUpdate()
    /// </summary>
    public static Quaternion StableLerpFixedUpdate(Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Lerp(a, b, StabilizeT(t, FixedFramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a perculiar update setting (e.g. If you have your own timescale handling)
    /// </summary>
    public static Quaternion StableLerpCustom(Quaternion a, Quaternion b, float t, UpdateType updateType, float timescaleMultiplier = 1)
    {
        float fps;
        switch (updateType)
        {
            default:
            case UpdateType.Update:
                fps = FramesPerSecond;
                break;
            case UpdateType.FixedUpdate:
                fps = FixedFramesPerSecond;
                break;
            case UpdateType.UnscaledUpdate:
                fps = UnscaledFramesPerSecond;
                break;
            case UpdateType.UnscaledFixedUpdate:
                fps = UnscaledFixedFramesPerSecond;
                break;
        }
        return Quaternion.Lerp(a, b, StabilizeT(t, fps * timescaleMultiplier));
    }
    /// <summary>
    /// Use this if you lerp with a custom update rate (e.g. You update your system 2 every frame -> updateRate=2)
    /// </summary>
    public static Quaternion StableLerpCustom(Quaternion a, Quaternion b, float t, float updateRate)
    {
        return Quaternion.Lerp(a, b, StabilizeT(t, updateRate));
    }

    /// <summary>
    /// Use this if you lerp within a standard Update()
    /// </summary>
    public static Color StableLerp(Color a, Color b, float t)
    {
        return Color.Lerp(a, b, StabilizeT(t, FramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a FixedUpdate()
    /// </summary>
    public static Color StableLerpFixedUpdate(Color a, Color b, float t)
    {
        return Color.Lerp(a, b, StabilizeT(t, FixedFramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a perculiar update setting (e.g. If you have your own timescale handling)
    /// </summary>
    public static Color StableLerpCustom(Color a, Color b, float t, UpdateType updateType, float timescaleMultiplier = 1)
    {
        float fps;
        switch (updateType)
        {
            default:
            case UpdateType.Update:
                fps = FramesPerSecond;
                break;
            case UpdateType.FixedUpdate:
                fps = FixedFramesPerSecond;
                break;
            case UpdateType.UnscaledUpdate:
                fps = UnscaledFramesPerSecond;
                break;
            case UpdateType.UnscaledFixedUpdate:
                fps = UnscaledFixedFramesPerSecond;
                break;
        }
        return Color.Lerp(a, b, StabilizeT(t, fps * timescaleMultiplier));
    }
    /// <summary>
    /// Use this if you lerp with a custom update rate (e.g. You update your system 2 every frame -> updateRate=2)
    /// </summary>
    public static Color StableLerpCustom(Color a, Color b, float t, float updateRate)
    {
        return Color.Lerp(a, b, StabilizeT(t, updateRate));
    }

    /// <summary>
    /// Use this if you lerp within a standard Update()
    /// </summary>
    public static Color32 StableLerp(Color32 a, Color32 b, float t)
    {
        return Color32.Lerp(a, b, StabilizeT(t, FramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a FixedUpdate()
    /// </summary>
    public static Color32 StableLerpFixedUpdate(Color32 a, Color32 b, float t)
    {
        return Color32.Lerp(a, b, StabilizeT(t, FixedFramesPerSecond));
    }
    /// <summary>
    /// Use this if you lerp within a perculiar update setting (e.g. If you have your own timescale handling)
    /// </summary>
    public static Color32 StableLerpCustom(Color32 a, Color32 b, float t, UpdateType updateType, float timescaleMultiplier = 1)
    {
        float fps;
        switch (updateType)
        {
            default:
            case UpdateType.Update:
                fps = FramesPerSecond;
                break;
            case UpdateType.FixedUpdate:
                fps = FixedFramesPerSecond;
                break;
            case UpdateType.UnscaledUpdate:
                fps = UnscaledFramesPerSecond;
                break;
            case UpdateType.UnscaledFixedUpdate:
                fps = UnscaledFixedFramesPerSecond;
                break;
        }
        return Color32.Lerp(a, b, StabilizeT(t, fps * timescaleMultiplier));
    }
    /// <summary>
    /// Use this if you lerp with a custom update rate (e.g. You update your system 2 every frame -> updateRate=2)
    /// </summary>
    public static Color32 StableLerpCustom(Color32 a, Color32 b, float t, float updateRate)
    {
        return Color32.Lerp(a, b, StabilizeT(t, updateRate));
    }



    #region Internals
    private const float REFERENCE_FRAMES_PER_SECONDS = 60;

    private static float FramesPerSecond
    {
        get { return 1f / Time.deltaTime; }
    }

    private static float UnscaledFramesPerSecond
    {
        get { return 1f / Time.unscaledDeltaTime; }
    }

    private static float FixedFramesPerSecond
    {
        get { return 1f / Time.fixedDeltaTime; }
    }

    private static float UnscaledFixedFramesPerSecond
    {
        get { return 1f / Time.fixedUnscaledDeltaTime; }
    }
    #endregion

    /// <summary>
    /// Use this if you want to manually stabilize the 't' parameter in Lerp by yourself. e.g. Mathf.Lerp(a, b, StableLerps.StabilizeT(t, 1/deltaTime))
    /// </summary>
    public static float StabilizeT(float t, float updatesPerSeconds)
    {
        return 1 - Mathf.Pow(1 - Mathf.Min(t, 1), REFERENCE_FRAMES_PER_SECONDS / updatesPerSeconds);
    }
}