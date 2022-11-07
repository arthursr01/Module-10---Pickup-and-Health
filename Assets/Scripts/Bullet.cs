using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public NetworkVariable<int> BulletDamage = new NetworkVariable<int>(3);

    public Rigidbody bullet;
    private float bulletSpeed = 20f;

    [ServerRpc]
    public void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        Rigidbody newBullet = Instantiate(bullet, transform.position, transform.rotation);
        newBullet.velocity = transform.forward * bulletSpeed;
        newBullet.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        newBullet.GetComponent<Bullet>().BulletDamage.Value = BulletDamage.Value;
        Destroy(newBullet.gameObject, 3);
    }
}
