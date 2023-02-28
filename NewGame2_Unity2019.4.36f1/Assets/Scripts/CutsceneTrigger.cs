using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public Vector3 triggerHalfExtents = Vector3.one;
    public Vector3 offset = Vector3.zero;

    public string cutsceneName;
    private bool triggered;

    public Collider[] cols;
    public static int EverythingMask = ~(0 << 32);


    private void Update()
    {
        if(!triggered)
        {
            /*Collider[] */cols = GetCollisions(transform.position + offset, triggerHalfExtents, 1 << LayerMask.NameToLayer("Player"));

            if(cols.Length > 0)
            {
                cols[0].GetComponent<TPSPlayer>().PlayCutscene(cutsceneName);
                Debug.LogError("Called PlayCutscene() on " + cols[0].transform.name);


                triggered = true;
            }
        }
    }

    public Collider[] GetCollisions(Vector3 position, Vector3 halfExtents, LayerMask layerMask, Quaternion rotation = new Quaternion())
    {
        return Physics.OverlapBox(position, halfExtents, rotation, layerMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position + offset, triggerHalfExtents * 2f);
    }
}
