using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControladorBolita : MonoBehaviour {
	public enum Estacion {
		estacion_1,
		estacion_2,
		estacion_3,
		estacion_4,
		salida
	}
	public enum Estado {
		En_cola, //la persona se encuentra en la cola
		En_atencion, //la persona esta siendo atendida por alguna caja (servidor)
		Espera_servicio, //la persona se encuentra en el inicio de la cola a la espera de una caja disponible
		Abandonando //la persona decidio abandonar el lugar
	}
	public Estacion estacion;
	public Estado estado = Estado.En_cola;
	public float velMovimiento = 5f;
	public float tiempoEnCola = 0f;
	public float tiempoEnColaGeneral = 0f;
	public List<SistemaCola> colaE1 = new List<SistemaCola> ();
	public SistemaCaja[] sisCajas;
	public List<SistemaCaja> listaCajasE1 = new List<SistemaCaja> ();
	public List<int> colaE1ints = new List<int> ();
	public Material matNormal;
	public Material matAbandonando;
	public Material matInconforme;
	public Material matRetiraDinero;
	SistemaCola[] colas;
	SistemaCaja[] cajas;
	//privadas
	private int conAux = 0;
	private bool banEsperaCaja = false;
	private bool banIraCola = true;
	//referencia a otros componentes
	NavMeshAgent nav;
	Renderer render;
	UI_Simulacion simulacion;
	Rcontroller _Rcontrol;
	Connect connectBD;
	//usar modelo
	Usuario usuario;
	void Awake () {
		nav = GetComponent<NavMeshAgent> ();
		render = GetComponent<Renderer> ();
		colas = GameObject.FindObjectsOfType<SistemaCola> ();
		cajas = GameObject.FindObjectsOfType<SistemaCaja> ();
		simulacion = FindObjectOfType<UI_Simulacion> ();
		//obtener R script
		_Rcontrol = GameObject.FindObjectOfType<Rcontroller>();
		connectBD = GameObject.FindObjectOfType<Connect>();
	}
	void Start () {
		usuario = new Usuario();
		usuario.usuario = this.gameObject.name;

		estacion = Estacion.estacion_1; //establece a la persona generada en la primera etapa
		estado = Estado.En_cola; //establece a la persona generada 'en cola'
		render.material = matNormal; //el color de la persona en estado 'Normal'
		tiempoEnCola = 0f;
		banEsperaCaja = false;
		banIraCola = true;
		//recorre todas las 'colas' encontradas
		for (int i = 0; i < colas.Length; i++) {
			if (colas[i].nombre.Contains ("E1 ")) { //almacena todas las 'colas' de la E1 en una lista
				colaE1.Add (colas[i]);
				continue;
			}
		}
		//ordenar colas de la estacion 1
		for (int i = colaE1.Count - 1; i >= 0; i--) {
			for (int j = 0; j < colaE1.Count; j++) {
				if (colaE1[j].nombre == ("E1 cola " + i)) {
					colaE1ints.Add (j);
					break;
				}
			}
		}
		//recorre todas las 'cajas' encontradas
		for (int i = 0; i < cajas.Length; i++) {
			if (cajas[i].nombre.Contains ("E1")) { //almacena todas las 'cajas' de la E1 en una lista
				listaCajasE1.Add (cajas[i]);
			}
		}
		nav.isStopped = true; //desactiva el movimiento
	}
	void Update () {
		switch (estacion) {
			case Estacion.estacion_1:
				if (estado == Estado.En_cola) {
					irColaE1 ();
				} else if (estado == Estado.Espera_servicio) {
					//tiempoEnCola = 0f;
					verificarCajaDisponibleE1 ();
				} else if (estado == Estado.Abandonando) {
					tiempoEnCola = 0f;
				} else if (estado == Estado.En_atencion) {
					tiempoEnCola = 0f;
				}
				break;
			case Estacion.estacion_2:
				Destroy (this.gameObject, .2f);
				break;
			case Estacion.estacion_3:
				Destroy (this.gameObject, .2f);
				break;
			case Estacion.estacion_4:
				Destroy (this.gameObject, .2f);
				break;
			case Estacion.salida:
				Destroy (this.gameObject, .2f);
				break;
		}

		//Tiempo que se demora en realizar todas las etapas
		/*tiempoEnColaGeneral += Time.deltaTime;
		if (tiempoEnColaGeneral >= simulacion.tiempoMaxEnCola) { //si el tiempo supera los 40 mins
			if (render.material != matInconforme) {
				render.material = matInconforme;
			}
		}*/
	}
	// ------------------------------------------- ETAPA 1 ------------------------------------------------
	private void irColaE1 () {
		// control del tiempo de la persona
		tiempoEnCola += Time.deltaTime;
		//calculo la distancia que existe entre este objeto y la siguiente posicion de la cola
		float dst = Vector3.Distance (colaE1[colaE1ints[conAux]].getPosition ().position, this.transform.position);
		if (dst >= 0.15f) { //si se encuentra alejado del punto
			//mueve el agente al punto especificado (si esta disponible)
			if (colaE1[colaE1ints[conAux]].estado == "Disponible") {
				banIraCola = true;
				if (conAux != 0) {
					colaE1[colaE1ints[conAux - 1]].desocuparCola (); //desocupa la cola actual
				}
				colaE1[colaE1ints[conAux]].ocuparCola (); //asigna el puesto de la cola como suyo
				if (colaE1[colaE1ints[conAux]].nombre.Contains ("cola 0"))
					estado = Estado.Espera_servicio;
			}
			if (banIraCola) {
				nav.isStopped = false;
				nav.destination = colaE1[colaE1ints[conAux]].getPosition ().position; //fija el destino
				nav.speed = velMovimiento; //da velocidad al agente para moverlo a su destino
			}
		} else { //si se encuentra en el punto
			banIraCola = false;
			nav.isStopped = true; //desactiva el movimiento
			nav.speed = 0f; //le quita la velocidad de movimiento (por si acaso)
			if (!colaE1[colaE1ints[conAux]].nombre.Contains ("cola 0")) //si no se encuentra en el inicio de la cola
				conAux++;
		}
	}
	private void verificarCajaDisponibleE1 () {
		for (int i = listaCajasE1.Count - 1; i >= 0; i--) {
			if (estado == Estado.Espera_servicio && listaCajasE1[i].estado == "Disponible") {
				if (!banEsperaCaja) {
					banEsperaCaja = true;
					StartCoroutine (ir_a_CajaE1 (i));
				}
				break;
			}
		}
	}
	IEnumerator ir_a_CajaE1 (int i) {
		yield return new WaitForSeconds (0.5f);
		if (estado == Estado.Espera_servicio && listaCajasE1[i].estado == "Disponible") {
			//ir a la caja
			listaCajasE1[i].ocuparCaja ();
			simulacion.estacion1.ocuparServidor (listaCajasE1[i].numero);

			//almacena el numero del cajero en el modelo Usuario para luego enviarlo a la BD
			usuario.id_cajero = listaCajasE1[i].numero;
			usuario.hora = simulacion.txtReloj.text;
			usuario.fecha = simulacion.txtDias.text;

			estado = Estado.En_atencion;
			nav.isStopped = false; //activa el movimiento
			nav.speed = velMovimiento;
			nav.destination = listaCajasE1[i].getPosition ().position; //fija el destino
			yield return new WaitForSeconds (1f);
			colaE1[colaE1ints[conAux]].desocuparCola (); //desocupa la cola del inicio

			// tiempo aleatorio (logaritmico) de atencion
			float random2 = UnityEngine.Random.Range (0f, 1f); //aleatorio entre 0f a 1f
			float tiempoAtencion = (-Mathf.Log (1 - random2) * (1f / UI_Simulacion.numAtencionPorHoraE1)) * 60;
			if (tiempoAtencion < 1f)
				tiempoAtencion = 1f;
			//print (this.name + " espera " + tiempoAtencion + " minutos de atencion en Etapa 1");
			
			string resConsulta = ""; //almacena la respueda al hacer una consulta en el cajero
			//devolver el tipo de transaccion
			switch (_Rcontrol.callRscript(3))
			{
				case "Retiro":
					//almacenar tipo de transaccion...
					usuario.tipo_transaccion = "Retiro";

					retirarDinero(i);
					break;
				case "Otros":
					//almacenar tipo de transaccion...
					usuario.tipo_transaccion = "Otros";

					//Como configuracion del la cuenta... etc
					resConsulta = _Rcontrol.callRscript(2);
					//almacenar respuesta de la consulta
					usuario.respuesta = resConsulta;
					break;
				case "Avance efectivo":
					//almacenar tipo de transaccion...
					usuario.tipo_transaccion = "Avance efectivo";

					retirarDinero(i);
					break;
				case "Consulta CC":
					//almacenar tipo de transaccion...
					usuario.tipo_transaccion = "Consulta CC";

					resConsulta = _Rcontrol.callRscript(2);
					//almacenar respuesta de la consulta
					usuario.respuesta = resConsulta;
					break;
				case "Consulta CA":
					//almacenar tipo de transaccion...
					usuario.tipo_transaccion = "Consulta CA";

					resConsulta = _Rcontrol.callRscript(2);
					//almacenar respuesta de la consulta
					usuario.respuesta = resConsulta;
					break;
				case "Retiro CC":
					//almacenar tipo de transaccion...
					usuario.tipo_transaccion = "Retiro CC";

					retirarDinero(i);
					break;
			}

			connectBD.crearUser(usuario);				

			yield return new WaitForSeconds (tiempoAtencion); // espera el tiempo de atecion

			listaCajasE1[i].desocuparCaja ();
			simulacion.estacion1.desocuparServidor (listaCajasE1[i].nombre.Substring (4, 1));
			listaCajasE1[i].incrementarContadorPersonas ();

			Destroy (this.gameObject, .2f);
		}
		banEsperaCaja = false;
	}

	public void retirarDinero(int i){
		string resCajero = _Rcontrol.callRscript(1);
		usuario.respuesta = resCajero;
		if (resCajero == "Aprobada"){
			int cantRetirar = 0;
			string monto = _Rcontrol.callRscript(0); //devolver monto a retirar
			if (monto != "mayor"){
				cantRetirar = Convert.ToInt16(monto);
			}
			//no se definio o es mayor a 60
			if (cantRetirar == 0) {
				// se genera un aleatorio entre 13 y 50, esto se multiplicara por 5
				// que es la cantidad minima que tiene el cajero automatico
				// y de esta manera se determina aleatoriamente cuanto retirará la persona
				// es decir, $250 = $5 * 50
				cantRetirar = UnityEngine.Random.Range(13, 50);
				cantRetirar = 5 * cantRetirar; //saldo a retirar
			}
	
			usuario.monto_retirado = cantRetirar;
			
			if (listaCajasE1[i].saldoActual >= cantRetirar){ //si el cajero tiene ese saldo disponible
				//la pelota cambia de color a verde representando 'retirando dinero'
				if (render.material != matRetiraDinero)
					render.material = matRetiraDinero;
				while(cantRetirar > 0){
					if (cantRetirar > 20){
						float probrd = UnityEngine.Random.Range(0f, 100f);
						if (probrd < 40){ //billete de 20
							if (listaCajasE1[i].cantBill20 > 0){
								cantRetirar -= 20;
								listaCajasE1[i].descontarBilletes20(1);
							}
						}else if (probrd < 80){ //billete de 10
							if (listaCajasE1[i].cantBill10 > 0){
								cantRetirar -= 10;
								listaCajasE1[i].descontarBilletes10(1);
							}
						}else{ //billete de 5
							if (listaCajasE1[i].cantBill5 > 0){
								cantRetirar -= 5;
								listaCajasE1[i].descontarBilletes5(1);
							}
						}
						//evito que se ejecute las condiciones de abajo mientras tenga mas de $20
						continue; 
					}
					if (cantRetirar == 20){
						if (listaCajasE1[i].cantBill20 > 0){
							cantRetirar -= 20;
							listaCajasE1[i].descontarBilletes20(1);
							continue;
						}
						if (listaCajasE1[i].cantBill10 >= 2){
							cantRetirar -= 20;
							listaCajasE1[i].descontarBilletes10(2);
							continue;
						}
					}
					if (cantRetirar >= 10){
						if (listaCajasE1[i].cantBill10 > 0){
							cantRetirar -= 10;
							listaCajasE1[i].descontarBilletes10(1);
							continue;
						}
						if (listaCajasE1[i].cantBill5 >= 2){
							cantRetirar -= 10;
							listaCajasE1[i].descontarBilletes5(2);
							continue;
						}
					}
					if (cantRetirar >= 5){
						if (listaCajasE1[i].cantBill5 > 0){
							cantRetirar -= 5;
							listaCajasE1[i].descontarBilletes5(1);
						}
					}
				}
				listaCajasE1[i].modificarInfoCajero();
			}else{// si el cajero no tiene la cantidad designada por la persona
				//la pelota cambia de color a amarrillo representando 'incorformidad'
				if (render.material != matInconforme)
					render.material = matInconforme;
				//FALTA
			}
		}else{
			//almacenar respuesta del cajero

			if (render.material != matAbandonando)
				render.material = matAbandonando;
		}
	}
}