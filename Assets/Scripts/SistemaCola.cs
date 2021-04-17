using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaCola : MonoBehaviour {
	public string estado = "Disponible"; 
	public string nombre = "cola N";

	void Start () {
		nombre = this.gameObject.name;
		desocuparCola();
	}

	public void ocuparCola () {
		estado = "Ocupado";
	}

	public void desocuparCola () {
		estado = "Disponible";
	}

	public Transform getPosition () {
		return this.gameObject.transform;
	}
	/*void OnTriggerEnter(Collider col){
		ocuparCola();
	}
	void OnTriggerExit(Collider col){
		desocuparCola();
	}*/
	/*void OnTriggerStay(Collider col){
		if (col.gameObject == null){
			desocuparCola();
			print("holi -- " + nombre);
		}
	}*/
}