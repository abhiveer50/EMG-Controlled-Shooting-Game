using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet: MonoBehaviour
{ 

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            print("hit "+collision.gameObject.name +" !");
            // CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }
    


        if(collision.gameObject.CompareTag("Wall"))
        {
            print("hit a wall");
    // void CreateBulletImpactEffect(Collision collision){
            CreateBulletImpactEffect(collision);
            Destroy(gameObject);
        }

    }

    void CreateBulletImpactEffect(Collision collision)
    {
        ContactPoint contact =collision.contacts[0];
        GameObject hole =Instantiate(
            GlobalReference.Instance.BulletImpactEffectprefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
        hole.transform.SetParent(collision.gameObject.transform);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


