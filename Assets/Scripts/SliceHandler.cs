using System.Collections.Generic;
using UnityEngine;
using EzySlice;

/**
 * This class is an example of how to setup a cutting Plane from a GameObject
 * and how to work with coordinate systems.
 * 
 * When a Place slices a Mesh, the Mesh is in local coordinates whilst the Plane
 * is in world coordinates. The first step is to bring the Plane into the coordinate system
 * of the mesh we want to slice. This script shows how to do that.
 */
public class SliceHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> targets;
    [SerializeField] private Material cutMaterial;
    [SerializeField] private float sliceForce;
    [SerializeField] private Animator PostProcessor;
    [SerializeField] private Animator Slash;
    [SerializeField] private List<Animator> AnimationsToEnable;
    [SerializeField] private float maxWait = 0.0f;
    [SerializeField] private AudioSource swordSlice;
    
    private bool pressed = false;
    private float waitTimer = 0.0f;
    
    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        return obj.Slice(transform.position, transform.up, crossSectionMaterial);
    }

    private void OnEnable()
    {
        if(AnimationsToEnable == null)
            return;

        for(int i = 0; i < AnimationsToEnable.Count; i++)
        {
            AnimationsToEnable[i].enabled = true;
        }
    }

    private void Update()
    {
        if(pressed)
            return;
        
        if(maxWait != 0)
        {
            waitTimer += Time.deltaTime;

            if(waitTimer > maxWait)
            {
                //Waited too long, force a loss
                GameManager.SliceResult.Invoke(new List<BoxCollider>(), targets.Count);
                GameManager.EndLevel();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            pressed = true;
            TimeScale.timeSlow.Invoke();
            Slash.Play("Slash", -1, 0);
            swordSlice.Play();
            var hulls = new List<SlicedHull>();
            for(int i = 0; i < targets.Count; i++)
            {
                hulls.Add(SliceObject(targets[i],cutMaterial));
            }
            var colliders = new List<BoxCollider>();
            //operate on hulls
            for(int i = 0; i < hulls.Count; i++)
            {
                if(hulls[i] == null)
                    continue;
                var uh = hulls[i].CreateUpperHull(targets[i],cutMaterial);
                var lh = hulls[i].CreateLowerHull(targets[i], cutMaterial);
                //add a vector to make targets go slightly up instead of just to the sides
                Vector3 force1 = (transform.up + new Vector3(0, .3f, 0)) * sliceForce * Random.Range(.7f, 1.2f);
                Vector3 force2 = (-transform.up + new Vector3(0, .3f, 0)) * sliceForce * Random.Range(.7f, 1.2f);
                uh.AddComponent<Rigidbody>().AddForce(force1,ForceMode.Impulse);
                colliders.Add(uh.AddComponent<BoxCollider>());
                lh.AddComponent<Rigidbody>().AddForce(force2, ForceMode.Impulse);
                colliders.Add(lh.AddComponent<BoxCollider>());
                //disable zone
                targets[i].transform.parent.parent.parent.gameObject.SetActive(false);
            }
            GameManager.SliceResult.Invoke(colliders, targets.Count);
        }
    }



#if UNITY_EDITOR
    /**
	 * This is for Visual debugging purposes in the editor 
	 */
    public void OnDrawGizmos()
    {
        EzySlice.Plane cuttingPlane = new EzySlice.Plane();

        // the plane will be set to the same coordinates as the object that this
        // script is attached to
        // NOTE -> Debug Gizmo drawing only works if we pass the transform
        cuttingPlane.Compute(transform);

        // draw gizmos for the plane
        // NOTE -> Debug Gizmo drawing is ONLY available in editor mode. Do NOT try
        // to run this in the final build or you'll get crashes (most likey)
        cuttingPlane.OnDebugDraw();
    }

#endif
}
