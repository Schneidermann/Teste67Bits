using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{

    public float forceMin;
    public float forceMax;

    [Space]

    public List<Rigidbody> Rbs;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Disable animator and disabel kinematics on the bones of the npc
    /// </summary>
    public void ActivateRagdoll()
    {
        anim.enabled = false;
        float randomforce = Random.Range(forceMin, forceMax);

        foreach (Rigidbody r in Rbs)
        {
            r.isKinematic = false;
            if(r.gameObject.name == "mixamorig:Hips")
            {
                print("found the head");
                r.AddForce(Vector3.back * randomforce, ForceMode.Impulse);
                print($"force of {randomforce}gs added");
            }
               
        }

       // Rbs.Find(x => x.gameObject.name == "mixamorig:Head").AddForce(Vector3.back, ForceMode.Impulse);
    }
}
