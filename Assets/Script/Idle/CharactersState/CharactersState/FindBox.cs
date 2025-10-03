using UnityEngine;

public class FindBox : StateMachineBehaviour
{
    Gatherer gatherer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        gatherer = animator.GetComponent<Gatherer>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject[] box = GameObject.FindGameObjectsWithTag("Box");

        if (box.Length == 0) return;

        Storage chosen = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject obj in box)
        {
            Storage crate = obj.GetComponent<Storage>();
            if (crate != null && !crate.IsFull())
            {
                float dist = Vector3.Distance(gatherer.transform.position, obj.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    chosen = crate;
                }
            }
        }

        if (chosen != null)
        {
            gatherer.TargetPosition = chosen.transform.position;
            gatherer.TargetStorage = chosen;

            animator.SetTrigger("ToBox");
        }

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
