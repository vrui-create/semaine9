using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class NPCAI : MonoBehaviour, IAIBehaviour
{
    [Header("Déclenchement du dialogue")]
    [SerializeField] private bool triggerOnContact = true;
    [SerializeField] private string playerTag = "Player";

    [Header("Dialogue (stub)")]
    [SerializeField] private string dialogueId = "npc_default";

    [Header("UI / Canvas à ouvrir")]
    [SerializeField] private GameObject ShopCanva;   // ton Canvas ou panel racine
    [SerializeField] private bool closeCanvasOnEnd = true;

    private AIController _controller;
    private bool _inDialogue;

    public void Initialize(object controller)
    {
        _controller = controller as AIController;
        if (_controller == null)
        {
            Debug.LogError("NPCAI nécessite un AIController valide.", this);
            enabled = false;
            return;
        }

        _controller.ChangeState(AIState.Idle);
    }

    public void Tick() { }

    public void FixedTick() { }

    public void OnStateChanged(int oldState, int newState) { }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnContact) return;
        if (_inDialogue) return;
        if (!other.CompareTag(playerTag)) return;

        StartDialogue();
    }

    private void StartDialogue()
    {
        _inDialogue = true;
        _controller?.ChangeState(AIState.Dialogue);

        if (ShopCanva != null)
        {
            ShopCanva.SetActive(true);
        }
    }

    public void EndDialogue()
    {
        Debug.Log("endCanva");
        _inDialogue = false;
        _controller?.ChangeState(AIState.Idle);

        if (closeCanvasOnEnd && ShopCanva != null)
        {
            ShopCanva.SetActive(false);
        }
    }
}
