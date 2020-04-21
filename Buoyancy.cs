using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour {

    private Rigidbody rigidbody;
    private ControllerBoat ControllerParent;
    private Collider ColliderParent;
    private Ocean Ocean;

    public bool isFloating;
    public bool inWater;
    public bool touchingWater;
    public bool submerged;
    public float Bouyancy;
    public float FluidViscosity;
    private float rotationAngleX;
    private float rotationAngleZ;
    private int randomHorizontalRot;
    private int randomVerticalRot;
    private bool call;
    [Range(1, 50)]
    public int WavesStrength;
    [Range(0.001f, 3f)]
    public float RotationSpeed;


    // Start is called before the first frame update
    void Awake() {

        rigidbody = GetComponent<Rigidbody>();
        ControllerParent = GetComponent<ControllerBoat>();
        ColliderParent = GetComponent<BoxCollider>();
        Ocean = GameObject.FindObjectOfType<Ocean>();

        rotationAngleX = 0f;
        rotationAngleZ = 0f;

        // Randomize beginning rotation side
        randomHorizontalRot = (Random.value < 0.5f) ? 1 : -1;
        randomVerticalRot = (Random.value < 0.5f) ? 1 : -1;

        call = true;
    }



    // Update is called once per frame
    void FixedUpdate() {

        // Get collider bounds
        float colliderTop = ColliderParent.bounds.center.y + ColliderParent.bounds.extents.y;
        float colliderCenter = ColliderParent.bounds.center.y;
        float colliderBottom = ColliderParent.bounds.center.y - ColliderParent.bounds.extents.y;

        // Set up informational bools
        inWater = (colliderCenter <= Ocean.transform.position.y && !isFloating) ? true : false;
        touchingWater = (!inWater && colliderBottom <= Ocean.transform.position.y) ? true : false;
        submerged = (colliderTop <= Ocean.transform.position.y) ? true : false;
        isFloating = (rigidbody.velocity.sqrMagnitude < 0.09 && touchingWater && !submerged) ? true : false;

        // Once the object is stable in water, stop forces to prevent jitter
        if (isFloating) {

            // reset forces
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;

            // Rock with waves
            if (call) {
                StartCoroutine(RockTheBoat());
                call = false;
            }

        } else {
            rigidbody.useGravity = true;
        }

        // In Water. Add Forces.
        if (inWater) {

            Vector3 drag = rigidbody.velocity * -1 * FluidViscosity * rigidbody.mass;
            Vector3 bouyancy = new Vector3(0, Bouyancy, 0);
            Vector3 force = drag + bouyancy * rigidbody.mass;

            rigidbody.AddForce(force);
        }
    }


    IEnumerator RockTheBoat() {

        yield return new WaitForSeconds(0.01f);

        rotationAngleX += RotationSpeed;
        rotationAngleZ += RotationSpeed;
                
        var verticalRotation = randomVerticalRot * Mathf.Sin(rotationAngleX) * WavesStrength;
        var horizontalRotation = randomHorizontalRot * Mathf.Sin(rotationAngleX) * WavesStrength * 1.5f;  // horizontal movement, in z, is a bigger factor of the vertical movement in x

        transform.localRotation = Quaternion.Euler(verticalRotation, 0, horizontalRotation);

        call = true;        
    }
}
