using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstacionController : MonoBehaviour {
	public GameObject[] servidores;
	public Material servidorAusente;
	public Material servidorActivo;
	public Material servidorAtencion;
	public Material servidorCaido;

	void Start () {
		todosActivos ();
	}
	public void todosActivos () {
		for (int i = 0; i < servidores.Length; i++) {
			servidores[i].GetComponent<Renderer> ().material = servidorActivo;
		}
	}
	public void todosCaidos (int activos) {
		for (int i = 0; i < servidores.Length; i++) {
			if ((i + 1) <= activos) {
				servidores[i].GetComponent<Renderer> ().material = servidorCaido;
			} 
		}
	}
	public void cambioServidores (int activos) {
		for (int i = 0; i < servidores.Length; i++) {
			if ((i + 1) <= activos) {
				servidores[i].GetComponent<Renderer> ().material = servidorActivo;
			} else {
				servidores[i].GetComponent<Renderer> ().material = servidorAusente;
			}
		}
	}
	public void ocuparServidor (int pos) {
		int i = Convert.ToInt16 (pos);
		i -= 1;
		servidores[i].GetComponent<Renderer> ().material = servidorAtencion;
	}
	public void desocuparServidor (string pos) {
		int i = Convert.ToInt16 (pos);
		i -= 1;
		servidores[i].GetComponent<Renderer> ().material = servidorActivo;
	}
	public void cajeroSinSaldo(int pos){
		pos -= 1;
		servidores[pos].GetComponent<Renderer> ().material = servidorAtencion;
	}
}