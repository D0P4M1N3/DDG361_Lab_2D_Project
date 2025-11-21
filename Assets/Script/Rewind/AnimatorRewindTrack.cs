using UnityEngine;
using System.Collections.Generic;

public class AnimatorRewindTrack : MonoBehaviour, IRewindTrack
{
    [SerializeField] private Animator animator;

    [System.Serializable]
    public struct Key
    {
        public int Frame;
        public int StateHash;
        public float NormalizedTime;
        public bool FlipX;
    }

    [SerializeField] private int interval = 1;
    [SerializeField] private int maxKeys = 500;

    private List<Key> keys = new List<Key>();
    private RewindManager manager;
    private SpriteRenderer spriteRenderer;
    private int lastStateHash = -1;
    private bool isRewinding = false;

    void Awake()
    {
        manager = FindObjectOfType<RewindManager>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (isRewinding)
        {
            return;
        }

        int frameInt = Mathf.FloorToInt(manager.NowFrame);
        var st = animator.GetCurrentAnimatorStateInfo(0);
        bool stateChanged = (st.fullPathHash != lastStateHash);

        bool directionChanged = false;
        bool currentFlip = spriteRenderer != null ? spriteRenderer.flipX : false;
        if (keys.Count > 0)
        {
            directionChanged = (currentFlip != keys[keys.Count - 1].FlipX);
        }

        if (!stateChanged && !directionChanged && (frameInt % interval != 0))
        {
            return;
        }

        Key newKey = new Key
        {
            Frame = frameInt,
            StateHash = st.fullPathHash,
            NormalizedTime = st.normalizedTime,
            FlipX = currentFlip
        };

        keys.Add(newKey);

        lastStateHash = st.fullPathHash;

        if (keys.Count > maxKeys)
        {
            keys.RemoveAt(0);
        }
    }



    public void ApplyInterpolated(int targetFrame)
    {
        if (keys.Count == 0)
        {
            return;

        }

        int i0 = FindLastKeyOnOrBefore(targetFrame);
        int i1 = FindFirstKeyAfter(targetFrame);

        if (i0 == -1 && i1 == -1) return;
        if (i1 == -1) { ApplyKey(keys[i0]); return; }
        if (i0 == -1) { ApplyKey(keys[i1]); return; }

        Key k0 = keys[i0];
        Key k1 = keys[i1];

        bool sameState = (k0.StateHash == k1.StateHash);
        float u = Mathf.InverseLerp(k0.Frame, k1.Frame, targetFrame);

        Key interp = new Key();
        interp.StateHash = k0.StateHash;

        if (sameState)
        {
            interp.NormalizedTime = Mathf.Lerp(k0.NormalizedTime, k1.NormalizedTime, u);
        }
        else
        {
            interp.NormalizedTime = k0.NormalizedTime;
        }

        if (u < 0.5f)
        {
            interp.FlipX = k0.FlipX;
        }
        else
        {
            interp.FlipX = k1.FlipX;
        }


        ApplyKey(interp);
    }

    private void ApplyKey(Key k)
    {
        animator.Play(k.StateHash, 0, k.NormalizedTime);
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = k.FlipX;

        }
        animator.Update(0f);
    }


    private int FindFirstKeyAfter(int frame)
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

    private int FindLastKeyOnOrBefore(int frame)
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

    public void ClearFramesAfter(int frame)
    {
        keys.RemoveAll(k => k.Frame > frame);
    }

    public void StartRewind()
    {
        animator.speed = 0f;
        animator.SetBool("isRewind", true);
        isRewinding = true;
    }

    public void StopRewind()
    {
        animator.speed = 1f;
        animator.SetBool("isRewind", false);
        isRewinding = false;  
    }

}
