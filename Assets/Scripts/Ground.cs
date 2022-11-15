using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ground : NetworkBehaviour
{

   
    public float movementSpeed = 10.0f;
    private Vector3 dir = Vector3.left;
    public float movementRangeOne = -5;
    public float movementRangeTwo = 5;

    public NetworkVariable<Vector3> PositionChange = new NetworkVariable<Vector3>();

    // Start is called before the first frame update

    // Taken and edited for needs from https://answers.unity.com/questions/1558555/moving-an-object-left-and-right.html
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(dir * movementSpeed * Time.deltaTime);


        if (transform.position.x <= movementRangeOne)
        {
            // move left
            Vector3[] results = CalcMovement();
            dir = Vector3.right;
            RequestPositionForMovementServerRpc(results[0]);
        }
        else if (transform.position.x >= movementRangeTwo)
        {
            Vector3[] results = CalcMovement();
            dir = Vector3.left;
            RequestPositionForMovementServerRpc(results[0]);
            // move right
        }
    }

    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 posChange)
    {
        if (!IsServer && !IsHost) return;

        PositionChange.Value = posChange;
        
    }

    private Vector3[] CalcMovement()
    {
        
        float x_move = 0.0f;

        Vector3 moveVect = new Vector3(x_move, 0, 0);
        moveVect *= movementSpeed;

        return new[] {moveVect};
    }
}

    