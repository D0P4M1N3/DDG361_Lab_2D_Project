using UnityEngine;

public interface IRewindTrack
{
    void ApplyInterpolated(int targetFrame);

    void ClearFramesAfter(int frame);

    void StartRewind();
    void StopRewind();


}

