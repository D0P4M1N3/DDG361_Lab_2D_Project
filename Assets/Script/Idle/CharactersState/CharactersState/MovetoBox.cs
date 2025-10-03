using UnityEngine;

public class MovetoBox : StateMachineBehaviour
{
    Movement movement;
    Gatherer gatherer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        movement = animator.GetComponent<Movement>();
        gatherer = animator.GetComponent<Gatherer>();

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        gatherer.isMoving = true;
        movement.MoveTo(gatherer.TargetPosition);

        if (Vector3.Distance(gatherer.transform.position, gatherer.TargetPosition) < 0.1f)
        {
            animator.SetTrigger("Deposit");
        }

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //base.OnStateExit(animator, stateInfo, layerIndex);
        gatherer.isMoving = false;
    }
}
