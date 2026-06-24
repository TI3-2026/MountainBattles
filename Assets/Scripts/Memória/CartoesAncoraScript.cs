using UnityEngine;

public class CartoesAncoraScript : MonoBehaviour
{
    private void Awake() {
        ManagerMemoria.Instance.cartoesAncora = this;
    }   

}
