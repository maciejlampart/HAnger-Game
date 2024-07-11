using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Animating;

public class FoodPicker : NetworkBehaviour
{
    [SerializeField] float pick_distance;
    ScoreCounter score_counter;

    [SerializeField]
    Score score;

    private ScoreBoard scoreboard;
    public Animator animator;
    public NetworkAnimator netAnim;
    [HideInInspector] public bool IronStomach = false;//effetto passivo da cibo, no effetti negativi da cibo
    // Start is called before the first frame update
    public override void OnStartClient()
    { // This is needed to avoid other clients controlling our character. 
        if (!base.IsOwner)
        {
            GetComponent<FoodPicker>().enabled = false;
        }
        else
        {
            StartCoroutine(locateScoreboard());
        }
        base.OnStartClient();
    }

    private void Start()
    {
        //score_counter = FindObjectOfType<ScoreCounter>();
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
            Food food = CheckFoodCollision();
            if (food != null)
            {
                float points = food.GetComponent<Food>().getValue();
                if (IronStomach)
                    if (points < 0)
                        points = 0;
                scoreboard.addPoints(points, base.Owner);
                FoodSpawner fs = FindObjectOfType<FoodSpawner>();
                fs.RemoveObject(food.gameObject);
            }
        }
        else
        {
            animator.SetBool("isPickingFood", false);
        }
    }


    private Food CheckFoodCollision()
    {
        RaycastHit hit;
        Transform pov_t = Camera.main.transform;
        if (Physics.Raycast(pov_t.position, pov_t.forward, out hit, pick_distance))
        {

            Debug.DrawRay(pov_t.position, pov_t.forward * hit.distance, Color.yellow);
            return hit.transform.gameObject.GetComponent<Food>();
        }
        Debug.DrawRay(pov_t.position, pov_t.forward * pick_distance, Color.white);
        return null;
    }

    IEnumerator locateScoreboard()
    {

        scoreboard = FindAnyObjectByType<ScoreBoard>();
        while (scoreboard == null)
        {
            yield return null;
            scoreboard = FindAnyObjectByType<ScoreBoard>();
        }
        scoreboard.spawnPlayerScore(base.Owner);
    }

    [ObserversRpc]
    public void Client_FoodPickerSetEnabled(bool setting)
    {
        if (!base.IsOwner) return;
        Debug.Log($"FoodPicker enabled? : {setting}");
        this.enabled = setting;
        return;
    }
}

