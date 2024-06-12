using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Animating;

public class FoodPicker : NetworkBehaviour
{
    [SerializeField] float pick_distance;
    ScoreCounter score_counter;
    private float points = 0;
    public Animator animator;
    public NetworkAnimator netAnim;
    // Start is called before the first frame update
    public override void OnStartClient()
    { // This is needed to avoid other clients controlling our character. 
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<FoodPicker>().enabled = false;
            return;
        }
    }

    private void Start()
    {
        score_counter = FindObjectOfType<ScoreCounter>();
        if (score_counter != null)
        {
            Debug.Log("Score counter found at start()");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("isPickingFood", true);
            GameObject food = CheckFoodCollision();
            if (food != null)
            {
                points += food.GetComponent<Food>().getValue();
                NetworkManager.Log("Got some food! My score is: " + points);
                score_counter.SetPoints(Mathf.RoundToInt(points));
                FoodSpawner fs = FindObjectOfType<FoodSpawner>();
                fs.RemoveObject(food);
            }
        }
        else
        {
            animator.SetBool("isPickingFood", false);
        }
    }


    private GameObject CheckFoodCollision()
    {
        int food_layer = 1 << 6;
        RaycastHit hit;
        Transform pov_t = Camera.main.transform;
        if (Physics.Raycast(pov_t.position, pov_t.forward, out hit, pick_distance, food_layer))
        {

            Debug.DrawRay(pov_t.position, pov_t.forward * hit.distance, Color.yellow);
            return hit.transform.gameObject;
        }
        Debug.DrawRay(pov_t.position, pov_t.forward * pick_distance, Color.white);
        return null;
    }

    public float GetScore()
    {
        return this.points;
    }

    [ObserversRpc]
    public void Client_FoodPickerSetEnabled(bool setting)
    {
        Debug.Log($"FoodPicker enabled? : {setting}");
        this.enabled = setting;
        return;
    }
}

