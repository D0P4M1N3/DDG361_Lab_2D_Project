using UnityEngine;

public class FindResource : StateMachineBehaviour
{
    Gatherer gatherer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gatherer = animator.GetComponent<Gatherer>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");

        if (resources.Length == 0) return;

        Resource chosen = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject obj in resources)
        {
            Resource res = obj.GetComponent<Resource>();
            if (res != null && !res.IsEmpty()) 
            {
                float dist = Vector3.Distance(gatherer.transform.position, obj.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    chosen = res;
                }
            }
        }

        if (chosen != null)
        {
            gatherer.TargetPosition = chosen.transform.position;
            gatherer.TargetResource = chosen;

            animator.SetTrigger("ToResource"); 
        }

    }
}
