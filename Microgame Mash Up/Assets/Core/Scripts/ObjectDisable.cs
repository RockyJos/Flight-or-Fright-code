using UnityEngine;

namespace GamePack
{
    public class ObjectDisable : MonoBehaviour
    {
        void Enable()
        {
            gameObject.SetActive(true);
        }

        void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}