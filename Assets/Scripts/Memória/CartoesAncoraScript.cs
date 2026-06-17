using UnityEngine;

public class CartoesAncoraScript : MonoBehaviour
{
    private void Awake() {
        ManagerLevel.Instance.cartoesAncora = this;
    }   

}
