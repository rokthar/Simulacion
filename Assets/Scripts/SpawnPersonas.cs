using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPersonas : MonoBehaviour {
	public GameObject prefabPersona;
	public Transform tfrmOrigen; //posicion en donde aparecera el objeto instanciado
	public List<GameObject> listaPersonas = new List<GameObject> ();
	//public static List<Persona> datosPersonas = new List<Persona> ();
	public static int conAbandonos = 0;
	public static int conReprobados = 0;
	public static int conDuplicados = 0;
	public static bool pararInvocacion = false;
	void Start () {
		pararInvocacion = false;
	}
	public void iniciarSpawnPersonas () {
		pararInvocacion = false;
		StartCoroutine ("invocarPersonas");
	}
	public void invocarPersona (float random1, float timeLlegada) {
		int nPerson = Random.Range(0, 1000);
		GameObject obj = (GameObject) Instantiate (prefabPersona, tfrmOrigen.position, Quaternion.identity);
		obj.name = "Persona_" + nPerson;
		listaPersonas.Add (obj);
	}
	public void pararSpawnBolitas () {
		StopCoroutine ("invocarPersonas");
		pararInvocacion = true;
	}
	private IEnumerator invocarPersonas () {
		//generacion de personas
		do {
			float random1 = Random.Range (0f, 1f);
			float timeLlegada = (-Mathf.Log (1 - random1) * (1f / UI_Simulacion.numClientesPorHora)) * 60;
			if (pararInvocacion)
				break;
			//espera el tiempo aleatorio 'logaritmico'
			yield return new WaitForSeconds (timeLlegada);
			if (!pararInvocacion)
				invocarPersona (random1, timeLlegada);
		} while (!pararInvocacion);
	}
	public void limpiarLista () {
		foreach (var i in listaPersonas) {
			Destroy (i.gameObject);
		}
		listaPersonas.Clear ();
	}
}