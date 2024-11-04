using UnityEngine;

public class UIBehindParent : MonoBehaviour
{
    void Start()
    {
        // Set this GameObject to be the first child (behind others)
        transform.SetAsFirstSibling();
    }
}
