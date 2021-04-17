using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaCaja : MonoBehaviour {
	public string estado = "Disponible";
	public string nombre = "caja N";
	public int numero = 0;
	public Transform pos;
	public int saldoTotal = 5000; //empieza con un saldo inicial de ...
	public int saldoActual = 5000; //el saldo actual que tiene el cajero
	public int cantBill5 = 200;  //la cantidad de Billetes de $5 que tiene el cajero
	public int cantBill10 = 200; //la cantidad de Billetes de $10 que tiene el cajero
	public int cantBill20 = 100; //la cantidad de Billetes de $20 que tiene el cajero
	public int conPersonas = 0;
	string estadoCajero = "ACTIVO";
	Cajero cajero;
	Connect connectBD;
	void Awake () {
		connectBD = GameObject.FindObjectOfType<Connect>();
		nombre = this.gameObject.name;
		inactivarCaja ();
	}
	void Start(){
		cajero = new Cajero();
	}
	public void ocuparCaja () {
		estado = "Ocupado";
	}
	public void desocuparCaja () {
		estado = "Disponible";
		estadoCajero = "ACTIVO";
	}
	public void inactivarCaja () {
		estado = "Inactiva";
		estadoCajero = "INACTIVO";
	}
	public void sinSaldo(){
		estado = "Sin saldo";
		estadoCajero = "SIN SALDO";
	}
	public void descontarBilletes5(int cant){
		cantBill5 -= cant;
		saldoActual -= 5 * cant;
	}
	public void descontarBilletes10(int cant){
		cantBill10 -= cant;
		saldoActual -= 10 * cant;
	}
	public void descontarBilletes20(int cant){
		cantBill20 -= cant;
		saldoActual -= 20 * cant;
	}
	public void reiniciarContador (){
		conPersonas = 0;
		saldoActual = saldoTotal;
		cantBill5 = 200;
		cantBill10 = 200;
		cantBill20 = 100;
	}
	public void incrementarContadorPersonas(){
		conPersonas++;
	}
	public int obtenerContadorPersonas(){
		return conPersonas;
	}
	public Transform getPosition () {
		return pos;
	}
	public void modificarInfoCajero(){
		cajero.num_cajero = numero;
		cajero.monto_inicial = saldoTotal;
		cajero.monto_actual = saldoActual;
		cajero.cant_cinco = cantBill5;
		cajero.cant_diez = cantBill10;
		cajero.cant_veinte = cantBill20;
		cajero.estado = estadoCajero;

		connectBD.modificarInfoCajero(cajero);
	}
}