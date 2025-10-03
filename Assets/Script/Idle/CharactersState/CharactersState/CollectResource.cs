using UnityEngine;

public class CollectResource : StateMachineBehaviour
{
    Gatherer gatherer;
    Resource targetResource;
    private float timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gatherer = animator.GetComponent<Gatherer>();
        targetResource = gatherer.TargetResource;

        timer = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (gatherer == null || targetResource == null) return;

        timer += Time.deltaTime;

        if (timer >= gatherer.collectTime)
        {
            timer = 0f;

            if (!targetResource.IsEmpty() && !gatherer.IsFull())
            {
                int collected = targetResource.Take(1);
                gatherer.AddResource(collected);
            }

            else if (targetResource.IsEmpty() && !gatherer.IsFull())
            {
                animator.SetTrigger("FindResource");
            }

            else
            {
                animator.SetTrigger("FindBox");
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //base.OnStateExit(animator, stateInfo, layerIndex);


    }
}
