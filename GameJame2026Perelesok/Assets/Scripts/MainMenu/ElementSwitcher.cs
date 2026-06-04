using UnityEngine;

public class ElementSwitcher : MonoBehaviour
{
   [SerializeField] private GameObject _objectToSwitch;
   [SerializeField] private bool _switchParameter;

   public void Switch()
   {
      _objectToSwitch.SetActive(_switchParameter);
   }
}
