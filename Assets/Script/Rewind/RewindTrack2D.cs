using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RewindTrack2D : MonoBehaviour, IRewindTrack
{
    [System.Serializable]
    public struct RewindKey
    {
        public int Frame;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
    }

    [SerializeField] private int interval = 5;
    [SerializeField] private int maxKeys = 500;

    [SerializeField] private List<RewindKey> keys = new List<RewindKey>();
    private Rigidbody2D rb;
    private RewindManager manager;

    private Vector2 lastVelocity;
    [SerializeField] private float speedChangeThreshold = 3f;

    private bool isRewinding = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        manager = FindObjectOfType<RewindManager>();
    }

    public void StartRewind()
    {
        isRewinding = true;
        rb.bodyType = RigidbodyType2D.Kinematic;   // freeze physics
    }

    public void StopRewind()
    {
        isRewinding = false;
        rb.bodyType = RigidbodyType2D.Dynamic;     
    }


    void FixedUpdate()
    {
        if (isRewinding)
        {
            return; 
        }

        int frame = Mathf.FloorToInt(manager.NowFrame);

        bool speedChanged =
            (rb.linearVelocity - lastVelocity).sqrMagnitude >
            speedChangeThreshold * speedChangeThreshold;

        if (!speedChanged && Time.frameCount % interval != 0)
        {
            lastVelocity = rb.linearVelocity;
            return;
        }

        keys.Add(new RewindKey
        {
            Frame = frame,
            Position = transform.position,
            Rotation = transform.eulerAngles,
            Velocity = rb.linearVelocity,
            AngularVelocity = new Vector3(0, 0, rb.angularVelocity)
        });

        lastVelocity = rb.linearVelocity;

        if (keys.Count > maxKeys)
        {
            keys.RemoveAt(0);
        }
    }

    public void ApplyInterpolated(int targetFrame)
    {
        if (keys.Count < 2)
            return;

        RewindKey keyA = keys[0];
        RewindKey keyB = keys[0];

        for (int i = keys.Count - 1; i >= 0; i--)
        {
            if (keys[i].Frame <= targetFrame)
            {
                keyA = keys[i];

                if (i < keys.Count - 1)
                {
                    keyB = keys[i + 1];

                }
                else
                {
                    keyB = keyA;
                }

                break;
            }
        }

        if (keyA.Frame == keyB.Frame)
        {
            ApplyKey(keyA);
            return;
        }

        float t = (float)(targetFrame - keyA.Frame) / (keyB.Frame - keyA.Frame);
        t = Mathf.Clamp01(t);

        transform.position = Vector3.Lerp(keyA.Position, keyB.Position, t);
        transform.eulerAngles = Vector3.Lerp(keyA.Rotation, keyB.Rotation, t);
        rb.linearVelocity = Vector3.Lerp(keyA.Velocity, keyB.Velocity, t);
        rb.angularVelocity = Mathf.Lerp(keyA.AngularVelocity.z, keyB.AngularVelocity.z, t);
    }

    private void ApplyKey(RewindKey key)
    {
        transform.position = key.Position;
        transform.eulerAngles = key.Rotation;
        rb.linearVelocity = key.Velocity;
        rb.angularVelocity = key.AngularVelocity.z;
    }

    public void ClearFramesAfter(int frame)
    {
        keys.RemoveAll(k => k.Frame > frame);
    }

    public void ResumeRecording()
    {
        isRewinding = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

}
