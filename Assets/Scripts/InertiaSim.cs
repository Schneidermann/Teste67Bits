using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InertiaSim : MonoBehaviour
{
    public static int Money;
    public static int BodyCapacity = 2;
    int upgradeCost = 2;

    public Transform playerTransform;
    public Transform playerModel; // Child object controlling rotation

    public float inertiaMultiplier = 0.5f; // Multiplier for inertia effect
    public List<Transform> BodiesHolder;
    public float smoothTime = 0.3f; // Time for smooth damp

    public Vector3 playerVelocity;
    private Vector3 previousPosition;
    public List<Vector3> initialPositions;
    public float increment = 0.2f;
    public List<float> bodyPileMultiplier = new List<float>();


    public float minOffset = -2.0f;
    public float maxOffset = 2.0f;

    public SkinnedMeshRenderer playerSkin;
    public TMPro.TMP_Text MoneyText;
    private TMPro.TMP_Text UpgradeCostText;
    public TMPro.TMP_Text TooltipText;
    public Button UpgradeButton;

    private void Awake()
    {
        playerSkin.material.color = Color.white;
        initialPositions = new List<Vector3>();

        previousPosition = playerTransform.position;
    }

    void Start()
    {
        

        foreach (Transform body in BodiesHolder)
        {
            initialPositions.Add(body.localPosition);
        }

        bodyPileMultiplier = new List<float>();
        bodyPileMultiplier.Add(0);
        UpgradeCostText = UpgradeButton.GetComponentInChildren<TMPro.TMP_Text>();
    }

    private void Update()
    {
        if(Money < upgradeCost)
        {
            UpgradeButton.interactable = false;
        }
        else
        {
            UpgradeButton.interactable = true;
        }


        UpgradeCostText.text = "Upgrade: $" + upgradeCost;
        MoneyText.text = "Money: " + Money.ToString() + ".  Carry Capacity: " + BodyCapacity;
        print($"Money:{Money}; BodiesLimit:{BodyCapacity}");
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = playerTransform.position;
        playerVelocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;
        previousPosition = currentPosition;

      

        ApplyInertia(playerVelocity);
    }

    void ApplyInertia(Vector3 playerVelocity)
    {
        for (int i = 1; i < BodiesHolder.Count; i++)
        {
            Transform body = BodiesHolder[i];
            // Calculate the direction of the player's movement in world space
            Vector3 playerDirection = playerVelocity.normalized;

            // Convert the player direction to the local space of the player object
            Vector3 localPlayerDirection = playerModel.InverseTransformDirection(playerDirection);

            if (playerVelocity != Vector3.zero)
            {


                // Calculate the inertia effect only along the local X-axis (forward axis)
                float inertiaEffect = localPlayerDirection.x + localPlayerDirection.z * inertiaMultiplier;

                // Calculate the target position by moving the object along its local X axis
                Vector3 targetPosition = initialPositions[i] - new Vector3(inertiaEffect, 0, 0);


                targetPosition.x = Mathf.Clamp(targetPosition.x, initialPositions[i].x + minOffset - bodyPileMultiplier[i], initialPositions[i].x + maxOffset );
                // Ensure Z position remains unchanged
                targetPosition.z = initialPositions[i].z;

                // Smoothly move the object to the target position
                body.localPosition = Vector3.Lerp(body.localPosition, targetPosition, Time.fixedDeltaTime);
            }
            else
            {
                // Smoothly return objects to their initial positions when player stops
                Vector3 targetPosition = new Vector3(initialPositions[i].x, initialPositions[i].y, initialPositions[i].z);
                body.localPosition = Vector3.Lerp(body.localPosition, targetPosition, smoothTime * Time.fixedDeltaTime);
            }
        }
    }

    public void AddBodyToPile(Transform newBody)
    {
        
        
        BodiesHolder.Add(newBody);

        initialPositions.Add(newBody.localPosition);

        float newMaxOffset = (bodyPileMultiplier.Count > 0 ? bodyPileMultiplier[bodyPileMultiplier.Count - 1] : maxOffset) + increment;
        bodyPileMultiplier.Add(newMaxOffset);
        
       
 
    }

    public void RemoveBodiesFromPile()
    {

        int bodyCount = BodiesHolder.Count;
        int bodiesRemoved = 0;

        for (int i = bodyCount - 1; i >= 1; i--)
        {
            Transform body = BodiesHolder[i];
            initialPositions.RemoveAt(i);
            BodiesHolder.RemoveAt(i);
            bodyPileMultiplier.RemoveAt(i);
            body.GetComponentInParent<NPC>().ReleaseBody();
            bodiesRemoved++;
        }


        // Calculate bonus money based on the number of bodies removed
        if (bodiesRemoved >= 2)
        {
            int bonus = (bodyCount - 1) * 2; 
            Money += bonus;
        }
        else if (bodiesRemoved <= 1)
            Money++;
       

        print("Bodies removed: " + (bodyCount - 1));
        print("Total money: " + Money);

    }

    public void BuyUpgrades()
    {
        if(Money >= upgradeCost)
        {
            Money -= upgradeCost;
            Upgrade();
            upgradeCost += Random.Range(1, 4);
        }
    }


    void Upgrade()
    {
        BodyCapacity++;

        switch (BodyCapacity)
        {
            case 2:
                TooltipText.text = $"Carry capacity +1 \n\n Change Skin color to green";
                break;
            case 3:
                playerSkin.material.color = Color.green;
                TooltipText.text = $"Carry capacity +1";
                break;
            case 4:
                TooltipText.text = $"Carry capacity +1 \n\n Change Skin color to blue";
                break;
            case 5:
                playerSkin.material.color = Color.blue;
                TooltipText.text = $"Carry capacity +1 \n\n Change Skin color to yellow";
                break;
            case 6:
                playerSkin.material.color = Color.yellow;
                TooltipText.text = $"Carry capacity +1";
                break;
        }
    }
}
