using UnityEngine;

public class NPCDeliverable : MonoBehaviour
{
    //this component is added to a pullable object that the player must deliver to an NPC
    [Header("NPC")]
    [SerializeField] protected int _deliverableID = 0;

    public int DeliverableID { get => _deliverableID; }
}
