using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsPickup : MonoBehaviour
{
    [Header("Settings")]
    public Transform holdPos;
    public LayerMask pickupLayer;

    [Header("Feel Settings")]
    public float pickupRange = 3f;
    public float pickupForce = 500f; // Snappy
    public float heldDrag = 20f;     // No wobble
    public float throwForce = 15f;

    private Rigidbody heldObjRB;
    private float originalDrag;

    void Update()
    {
        // 1. INPUT: Left Click to Pickup / Drop
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (heldObjRB == null) TryPickup();
            else DropObject();
        }

        // 2. INPUT: Right Click to Throw
        if (Mouse.current.rightButton.wasPressedThisFrame && heldObjRB != null)
        {
            ThrowObject();
        }
    }

    void FixedUpdate()
    {
        if (heldObjRB != null)
        {
            MoveObject();
        }
    }

    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange, pickupLayer))
        {
            heldObjRB = hit.transform.GetComponent<Rigidbody>();
            if (heldObjRB != null)
            {
                originalDrag = heldObjRB.linearDamping;
                
                // Physics Setup
                heldObjRB.useGravity = false;
                heldObjRB.linearDamping = heldDrag;
            }
        }
    }

    void MoveObject()
    {
        if (Vector3.Distance(heldObjRB.position, holdPos.position) > 0.1f)
        {
            Vector3 moveDir = (holdPos.position - heldObjRB.position);
            heldObjRB.AddForce(moveDir * pickupForce);
        }
    }

    void DropObject()
    {
        heldObjRB.useGravity = true;
        heldObjRB.linearDamping = originalDrag;
        heldObjRB = null;
    }

    void ThrowObject()
    {
        Rigidbody throwRB = heldObjRB;
        DropObject();
        throwRB.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }
}