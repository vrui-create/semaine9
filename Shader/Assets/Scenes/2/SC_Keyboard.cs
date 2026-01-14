using UnityEngine;
using UnityEngine.InputSystem;

public class SC_Keyboard : MonoBehaviour
{
    [SerializeField]Renderer renderer;
    [SerializeField]InputActionReference ip_Action;
   
    private void Awake()
    {
        ip_Action.action.started += F_Action;

    }

    private void F_Action(InputAction.CallbackContext context)
    {
        print("Scene Action");
    }



    // Update is called once per frame
    void Update()
    {
        //materials ça prend que celui ......
        renderer.materials[0].SetFloat("", 1);

        //renderer.sharedMaterials // permet de modifier tous les mesh qui posseder le même materiau, exemple d'utiliter: Tous les ennemis vous en reperer.
    }
}
