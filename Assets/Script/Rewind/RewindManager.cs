using System.Collections.Generic;
using UnityEngine;

public class RewindManager : MonoBehaviour
{
    public bool isRewinding = false;
    public int NowFrame = 0;

    [SerializeField] private float rewindSpeed = 100f;
    [SerializeField] private float mult = 2f;

    private List<IRewindTrack> tracks = new List<IRewindTrack>();
    private bool wasRewindingLastFrame = false;


    float frameAccumulator = 0f;
    float currentSpeed;


    void Awake()
    {
        tracks.AddRange(FindObjectsOfType<RewindTrack2D>() as IRewindTrack[]);
        tracks.AddRange(FindObjectsOfType<AnimatorRewindTrack>() as IRewindTrack[]);
    }

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartRewind();

        }

        if (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            StopAndResume();
        }

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? rewindSpeed * 100 * mult : rewindSpeed * 100;

        wasRewindingLastFrame = isRewinding;

        UpdateRewind();
    }



    //void FixedUpdate()
    //{

    //}

    void UpdateRewind()
    {
        if (!isRewinding)
        {
            NowFrame += 1;
            return;
        }

        frameAccumulator += currentSpeed * Time.unscaledDeltaTime;
        int wholeFrames = Mathf.FloorToInt(frameAccumulator);

        NowFrame -= wholeFrames;

        frameAccumulator -= wholeFrames;

        if (NowFrame < 0)
        {
            NowFrame = 0;

        }

        foreach (var track in tracks)
        {
            track.ApplyInterpolated(NowFrame);

        }
    }


    public void ScrubToFrame(int targetFrame)
    {
        NowFrame = Mathf.Max(0, targetFrame);
        foreach (var track in tracks)
        {
            track.ApplyInterpolated(targetFrame);

        }
    }

    public void StartRewind()
    {
        isRewinding = true;
        foreach (var track in tracks)
        {
            track.StartRewind();

        }
        Time.timeScale = 0f;
    }

    public void StopAndResume()
    {
        isRewinding = false;

        foreach (var track in tracks)
        {
            track.ClearFramesAfter(NowFrame);

            track.StopRewind();

            

        }

        Time.timeScale = 1f;
    }


}
