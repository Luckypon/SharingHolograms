using UnityEngine;

[ExecuteAlways]
public class FollowGameObject : MonoBehaviour
{
    public Transform ObjectToFollow;
    public Vector3 Offset;
    public bool FollowRotation;
    void Update()
    {
        transform.position = ObjectToFollow.position + Offset;
        if (FollowRotation)
        {
            transform.rotation = ObjectToFollow.rotation;
        }
    }

}
