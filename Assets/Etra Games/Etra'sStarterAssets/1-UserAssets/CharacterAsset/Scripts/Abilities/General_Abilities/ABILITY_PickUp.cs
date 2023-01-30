using UnityEngine;

public class ABILITY_PickUp : EtraAbilityBaseClass
{
    // With this ability, we will pick up Rigidbodies within a certain mass and range.
    // We will raycast from the Main Camera to the pickUpRange, and if we hit a rigidbody, we will pick it up.
    // If we are picked up, the raycast will determine where the object's target position is, and we will use a spring joint to move the object to that position.

    public float pickUpRange = 3f;
    public float maxMass = 5f; // anything greater than this will not be picked up
    private Rigidbody pickedUpObject;
    private SpringJoint springJoint;
    public float springJointForce = 200f;
    public float springJointDamper = 10f;
    private Rigidbody otherRB; // we make a Rigidbody that will be kinematic, this is at the target position and is needed since spring joints need two rigidbodies to work
    public GameObject temporaryParent;

    private float originalDrag;
    private float originalAngularDrag;
    public float pickedUpDrag = 10f;

    public float pickedUpDistance = 2f;
    public float dropDistance = 5f; // if it goes too far away from the player, we will drop it

    public override void abilityStart()
    {
        GameObject otherRBGO = new GameObject("otherRB");
        otherRB = otherRBGO.AddComponent<Rigidbody>();
        otherRB.isKinematic = true;
        otherRB.useGravity = false;
        otherRB.mass = 2000f; // this helps certain components understand that this is a static object

        otherRB.transform.parent = Camera.main.transform;
        otherRB.transform.localPosition = new Vector3(0, 0, pickedUpDistance);
    }
    public override void abilityUpdate()
    {
        if (pickedUpObject == null)
        {
            // if we press E while looking at a rigidbody we raycast to, we will pick it up
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickUpRange))
                {
                    if (hit.rigidbody != null)
                    {
                        if (hit.rigidbody.mass <= maxMass)
                        {
                            pickedUpObject = hit.rigidbody;

                            // remove any existing spring joints
                            if (pickedUpObject.GetComponent<SpringJoint>() != null)
                            {
                                Destroy(pickedUpObject.GetComponent<SpringJoint>());
                            }

                            springJoint = pickedUpObject.gameObject.AddComponent<SpringJoint>();
                            springJoint.connectedBody = otherRB;
                            springJoint.spring = springJointForce;
                            springJoint.damper = springJointDamper;
                            springJoint.autoConfigureConnectedAnchor = false;
                            springJoint.connectedAnchor = Vector3.zero;
                            springJoint.anchor = Vector3.zero;

                            originalAngularDrag = pickedUpObject.angularDrag;
                            originalDrag = pickedUpObject.drag;

                            pickedUpObject.angularDrag = pickedUpDrag;
                            pickedUpObject.drag = pickedUpDrag;
                        }
                    }
                }
            }
        }
        else
        {
            // if we press E while holding an object, we will drop it
            if (Input.GetKeyDown(KeyCode.E) || Vector3.Distance(pickedUpObject.transform.position, Camera.main.transform.position) > dropDistance)
            {
                Destroy(springJoint);
                pickedUpObject.angularDrag = originalAngularDrag;
                pickedUpObject.drag = originalDrag;
                pickedUpObject = null;
            }
        }
    }



}
