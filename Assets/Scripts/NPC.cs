using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public bool isDown;


    public float forceMin;
    public float forceMax;

    [Space]

    public List<Rigidbody> Rbs;
    public Animator anim;
    public List<Collider> colls;

    Transform thisHipsTransform;
    InertiaSim _inertiaSim;

    // Start is called before the first frame update
    void Start()
    {
        _inertiaSim = FindAnyObjectByType<InertiaSim>();
        thisHipsTransform = Rbs.Find(x => x.gameObject.name == "mixamorig:Hips").gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Disable animator and disabel kinematics on the bones of the npc
    /// </summary>
    public void ActivateRagdoll(Transform playerRotation)
    {
        anim.enabled = false;
        float randomforce = Random.Range(forceMin, forceMax);
        Vector3 forceDirection = -playerRotation.forward;

        foreach (Rigidbody r in Rbs)
        {
            r.isKinematic = false;
            
            if(r.gameObject.name == "mixamorig:Hips")
            {
                print("found the head");
                //add force where player is looking
                r.AddForce(-forceDirection * randomforce, ForceMode.Impulse);
                print($"force of {randomforce}gs added");
            }
               
        }

        isDown = true;

        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Collider>().enabled = false;


       // Rbs.Find(x => x.gameObject.name == "mixamorig:Head").AddForce(Vector3.back, ForceMode.Impulse);
    }



    public void PickUpBody(Transform playerHipsTransform, int piledBodies)
    {
        this.transform.SetParent(playerHipsTransform);

        
        foreach (Rigidbody r in Rbs)
        {

            
            r.gameObject.layer = 6;
            if (r.gameObject.name == "mixamorig:Hips")
            {
                r.isKinematic = true;
                r.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                
            }
            if (r.gameObject.name == "mixamorig:Head")
            {
               // r.isKinematic = true;
                

            }

        }
       
        //piled bodies is the amount of objects already piled up
        this.transform.SetLocalPositionAndRotation(new Vector3(-1.4f, 0.2f + piledBodies, -0.8f), Quaternion.identity);
        this.transform.localRotation = Quaternion.Euler(-90, -90, 0);
        _inertiaSim.AddBodyToPile(thisHipsTransform);
    }


    public void ReleaseBody()
    {
        this.transform.SetParent(null);
        Destroy(this.gameObject);

    }
}
