using UnityEngine;

public class CoinDespawn : MonoBehaviour
{
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("coin_pop"))
        {
            AnimIsFinsihed();
        }

        if (AnimIsFinsihed())
        {
            Destroy(gameObject);
        }
     
    }

    bool AnimIsFinsihed()
    {
        return (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);
    }


}
