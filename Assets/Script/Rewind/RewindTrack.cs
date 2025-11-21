using UnityEngine;
using System.Collections.Generic;

public abstract class RewindTrack<T>: RewindTrack2D where T : IRewindKey
{
    protected List<T> keys = new List<T>();


    protected abstract void ApplyFromInterpolatedKey(T k0, T k1, float u, float dt);

    //public override void ApplyExactKey(int targetFrame)
    //{
    //    if(keys.Count == 0)
    //    {
    //        return;
    //    }

    //    int idx = FindLastKeyOnOrBefore(targetFrame);
    //    var k (idx >= 0) ? keys[idx] : keys[0];

    //    ApplyExactKey(k);
    //}

    int FindFirstKeyAfter(int frame)
    {
        int lo = 0, hi = keys.Count - 1, ans = -1;
        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            if (keys[mid].Frame > frame)
            {
                ans = mid;
                hi = mid - 1;
            }
            else
            {
                lo = mid + 1;
            }
        }

        return ans;
    }

    int FindLastKeyOnOrBefore(int frame)
    {
        int lo = 0, hi = keys.Count - 1, ans = -1;

        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            if (keys[mid].Frame <= frame)
            {
                ans = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }
        return ans;
    }


}
