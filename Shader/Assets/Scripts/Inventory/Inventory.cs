using System.Runtime.CompilerServices;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _nbrOrbs;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Orb"))
        {
            Destroy(other.gameObject);
            _nbrOrbs++;
        }
    }

    public bool HasEnoughOrbs(int nbr)
    {
        return _nbrOrbs >= nbr;
    }

    public bool Buy(int nbr)
    {
        if (HasEnoughOrbs(nbr))
        {
            _nbrOrbs -= nbr;
            return true;
        }
        return false;
    }
}
