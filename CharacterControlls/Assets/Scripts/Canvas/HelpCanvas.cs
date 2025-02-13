using UnityEngine;
using UnityEngine.UI;

public class HelpCanvas : MonoBehaviour
{
    [SerializeField] private Button okButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            gameObject.SetActive(false);
        }
    }


}
