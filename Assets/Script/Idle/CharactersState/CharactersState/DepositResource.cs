using UnityEngine;

public class DepositResource : StateMachineBehaviour
{
    private Gatherer gatherer;
    private Storage targetBox;
    private float timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gatherer = animator.GetComponent<Gatherer>();
        targetBox = gatherer.TargetStorage;
        timer = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (gatherer == null || targetBox == null) return;

        timer += Time.deltaTime;

        if (timer >= gatherer.collectTime) 
        {
            timer = 0f;

            if (!targetBox.IsFull() && gatherer.held > 0)
            {
                gatherer.RemoveResource(1);

                targetBox.Store(1);
            }
            else if(targetBox.IsFull() && gatherer.held > 0)
            {
                animator.SetTrigger("FindBox");
            }
            
            else
            {
                animator.SetTrigger("FindResource");
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
