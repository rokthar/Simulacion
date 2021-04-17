using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Simulacion : MonoBehaviour {
	public Slider sliderEstacion1;
	public TextMeshProUGUI txtEst1_numServidores;
	public GameObject btnIniciarSim;
	public GameObject btnPararSim;
	public TextMeshProUGUI txtReloj;
	public TextMeshProUGUI txtDias;
	public int horaInicio = 8;
	public float timeSeconds;
	public Slider sliderVelocidadTiempo;
	public TextMeshProUGUI txtVelocidadTiempo;
	public TMP_InputField inputClientesPorHora;
	public TMP_InputField inputTiempoMaxEnCola; //tiempo maximo que la persona normal <promedio> espera en cola
	public TMP_InputField inputProbAbandono;
	public TMP_InputField inputProbDuplicados;
	public TMP_InputField inputProbReprobar;
	public TMP_InputField inputHorasCaidaSistema;
	public TMP_InputField inputNumCaidaSistema;
	public int tiempoMaxEnCola = 40;
	[Range (1, 8)]
	public Animator anim;
	//privadas
	private int auxVelTiempo = 0;
	private int hh;
	private int mm;
	private int segs;
	private int dias = 0;
	//private string ampm = " AM";
	private bool banReloj = false;
	private string[] ss = new string[2];
	SistemaCola[] colas;
	SistemaCaja[] cajas;
	//estaticas
	public static int auxEst1Servidores = 0;
	public static int numClientesPorHora = 2;
	public static int numAtencionPorHoraE1 = 10;
	public static float tiempoCaida = 0f;
	//vars de cajero
	public TextMeshProUGUI txtNumCajero;
	public TextMeshProUGUI txtSaldoInicial;
	public TextMeshProUGUI txtSaldoActual;
	public TextMeshProUGUI txtCantBill5;
	public TextMeshProUGUI txtCantBill10;
	public TextMeshProUGUI txtCantBill20;
	public TextMeshProUGUI txtEstadoCajero;
	//referencia a otros scripts
	public EstacionController estacion1;
	private bool banCajero1 = false;
	private bool banCajero2 = false;
	private bool banCajero3 = false;
	private bool banCajero4 = false;
	SpawnPersonas spawnPersonas;
	SistemaCaja sis1;
	SistemaCaja sis2;
	SistemaCaja sis3;
	SistemaCaja sis4;
	Rcontroller _Rcontrol;

	void Awake () {
		spawnPersonas = FindObjectOfType<SpawnPersonas> ();
		colas = GameObject.FindObjectsOfType<SistemaCola> ();
		cajas = GameObject.FindObjectsOfType<SistemaCaja> ();
	
		//obtiene el componente de los 4 cajeros
		sis1 = GameObject.Find ("E1_S1").GetComponent<SistemaCaja> ();
		sis2 = GameObject.Find ("E1_S2").GetComponent<SistemaCaja> ();
		sis3 = GameObject.Find ("E1_S3").GetComponent<SistemaCaja> ();
		sis4 = GameObject.Find ("E1_S4").GetComponent<SistemaCaja> ();
		_Rcontrol = GetComponent<Rcontroller>();
	}
	void Start () {
		dias = 1;
	}
	void Update () {
		//slider # de servidores en la Estacion 1
		txtEst1_numServidores.text = sliderEstacion1.value + "";
		if (auxEst1Servidores != (int) sliderEstacion1.value) {
			auxEst1Servidores = (int) sliderEstacion1.value;
			estacion1.cambioServidores (auxEst1Servidores);
			for (int i = 1; i <= 4; i++) {
				if (i <= auxEst1Servidores) {
					GameObject.Find ("E1_S" + i).GetComponent<SistemaCaja> ().desocuparCaja ();
				} else {
					GameObject.Find ("E1_S" + i).GetComponent<SistemaCaja> ().inactivarCaja ();
				}
			}
		}
		//control del reloj
		if (banReloj) {
			timeSeconds += Time.deltaTime; //incrementa el tiempo en segundos
			//ampm = convertirAMPM (timeSeconds); //determina si es AM o PM convirtiendo el tiempo total
			hh = (int) (timeSeconds / 60);
			mm = (int) timeSeconds - (hh * 60);
			if (timeSeconds.ToString ().Contains (".")) {
				ss = timeSeconds.ToString ().Split ('.');
				segs = (Convert.ToInt32 (ss[1].Substring (0, 2)) * 60) / 100;
			} else {
				segs = 0;
			}
			// por cada hora que pasa se activa la generacion de personas/h de acuerdo a la cantidad
			// que se haya establecido
			if (hh >= 24) { // 16 horas
				timeSeconds = 0f;
				dias++;
			}
			txtReloj.text = hh.ToString ("00") + ":" + mm.ToString ("00") + ":" + segs.ToString ("00");
			txtDias.text = "Día " + dias;
		} else {
			txtReloj.text = horaInicio.ToString ("00") + ":00:00";
			txtDias.text = "Día 1";
		}
		// control de la velocidad del tiempo
		txtVelocidadTiempo.text = "x" + sliderVelocidadTiempo.value;
		if (auxVelTiempo != (int) sliderVelocidadTiempo.value) {
			auxVelTiempo = (int) sliderVelocidadTiempo.value;
			Time.timeScale = auxVelTiempo;
		}
		// control del tiempo de atencion por hora de los servidores en cada etapa
		/*if (numAtencionPorHoraE1 != Convert.ToInt16 (inputAtencionPorHoraE1.text))
			numAtencionPorHoraE1 = Convert.ToInt16 (inputAtencionPorHoraE1.text);
		// control de la frecuencia de llegada de los clientes
		if (numClientesPorHora != Convert.ToInt16 (inputClientesPorHora.text))
			numClientesPorHora = Convert.ToInt16 (inputClientesPorHora.text);
		// control del tiempo maximo que puede esperar en cola una persona 'promedio'
		if (tiempoMaxEnCola != Convert.ToInt16 (inputTiempoMaxEnCola.text)) {
			tiempoMaxEnCola = Convert.ToInt16 (inputTiempoMaxEnCola.text);
		}
		*/
		if (banCajero1){
			txtNumCajero.text = "Cajero automático # 1";
			txtSaldoInicial.text = "Saldo inicial: $" + sis1.saldoTotal + "";
			txtSaldoActual.text = "Saldo actual:  $" + sis1.saldoActual + "";
			txtCantBill5.text =  "Billetes de $5   =  " + sis1.cantBill5; 
			txtCantBill10.text = "Billetes de $10  =  " + sis1.cantBill10; 
			txtCantBill20.text = "Billetes de $20  =  " + sis1.cantBill20;
			if (sis1.saldoActual > 0)
				txtEstadoCajero.text = "Estado: <color=green>ACTIVO</color>";
			else
				txtEstadoCajero.text = "Estado: <color=red>SIN SALDO</color>";
		}
		if (banCajero2){
			txtNumCajero.text = "Cajero automático # 2";
			txtSaldoInicial.text = "Saldo inicial: $" + sis2.saldoTotal+"";
			txtSaldoActual.text = "Saldo actual:  $" + sis2.saldoActual+"";
			txtCantBill5.text =  "Billetes de $5   =  " + sis2.cantBill5; 
			txtCantBill10.text = "Billetes de $10  =  " + sis2.cantBill10; 
			txtCantBill20.text = "Billetes de $20  =  " + sis2.cantBill20;
			if (sis2.saldoActual > 0)
				txtEstadoCajero.text = "Estado: <color=green>ACTIVO</color>";
			else
				txtEstadoCajero.text = "Estado: <color=red>SIN SALDO</color>";
		}
		if (banCajero3){
			txtNumCajero.text = "Cajero automático # 3";
			txtSaldoInicial.text = "Saldo inicial: $" + sis3.saldoTotal;
			txtSaldoActual.text =  "Saldo actual:  $" + sis3.saldoActual+"";
			txtCantBill5.text =  "Billetes de $5   =  " + sis3.cantBill5; 
			txtCantBill10.text = "Billetes de $10  =  " + sis3.cantBill10; 
			txtCantBill20.text = "Billetes de $20  =  " + sis3.cantBill20;
			if (sis3.saldoActual > 0)
				txtEstadoCajero.text = "Estado: <color=green>ACTIVO</color>";
			else
				txtEstadoCajero.text = "Estado: <color=red>SIN SALDO</color>";
		}
		if (banCajero4){
			txtNumCajero.text = "Cajero automático # 4";
			txtSaldoInicial.text = "Saldo inicial: $" + sis4.saldoTotal+"";
			txtSaldoActual.text = "Saldo actual:  $" + sis4.saldoActual+"";
			txtCantBill5.text =  "Billetes de $5   =  " + sis4.cantBill5; 
			txtCantBill10.text = "Billetes de $10  =  " + sis4.cantBill10; 
			txtCantBill20.text = "Billetes de $20  =  " + sis4.cantBill20;
			if (sis4.saldoActual > 0)
				txtEstadoCajero.text = "Estado: <color=green>ACTIVO</color>";
			else
				txtEstadoCajero.text = "Estado: <color=red>SIN SALDO</color>";
		}
		if (sis1.saldoActual == 0){
			estacion1.cajeroSinSaldo(1);
			sis1.sinSaldo();	
		}
		if (sis2.saldoActual == 0){
			estacion1.cajeroSinSaldo(2);
			sis2.sinSaldo();	
		}
		if (sis3.saldoActual == 0){
			estacion1.cajeroSinSaldo(3);
			sis3.sinSaldo();	
		}
		if (sis4.saldoActual == 0){
			estacion1.cajeroSinSaldo(4);
			sis4.sinSaldo();	
		}
	}
	public void btnCajero1(){
		banCajero1 = true;
		banCajero2 = false;
		banCajero3 = false;
		banCajero4 = false;		
	}
	public void btnCajero2(){
		banCajero1 = false;
		banCajero2 = true;
		banCajero3 = false;
		banCajero4 = false;		
	}
	public void btnCajero3(){
		banCajero1 = false;
		banCajero2 = false;
		banCajero3 = true;
		banCajero4 = false;		
	}
	public void btnCajero4(){
		banCajero1 = false;
		banCajero2 = false;
		banCajero3 = false;
		banCajero4 = true;		
	}
	
	public void btnIniciarSimulacion () {
		btnCajero1();
		btnPararSim.SetActive (true);
		btnIniciarSim.SetActive (false);
		spawnPersonas.iniciarSpawnPersonas ();
		timeSeconds = horaInicio * 60; //convierte horas a segundos
		banReloj = true;
		desabilitarSliderEstaciones ();
		sis1.modificarInfoCajero();
		sis2.modificarInfoCajero();
		sis3.modificarInfoCajero();
		sis4.modificarInfoCajero();
	}
	public void btnPararSimulacion () {
		btnIniciarSim.SetActive (true);
		btnPararSim.SetActive (false);
		spawnPersonas.pararSpawnBolitas ();
		spawnPersonas.limpiarLista ();
		banReloj = false;
		habilitarSliderEstaciones ();
		resetearColas_Cajas ();
		dias = 1;
	}
	private void desabilitarSliderEstaciones () {
		sliderEstacion1.interactable = false;
		//tambien desabilito las cajas
		inputClientesPorHora.interactable = false;
		inputProbAbandono.interactable = false;
		inputProbReprobar.interactable = false;
		inputHorasCaidaSistema.interactable = false;
		inputNumCaidaSistema.interactable = false;
		inputProbDuplicados.interactable = false;
	}
	private void habilitarSliderEstaciones () {
		sliderEstacion1.interactable = true;
		//tambien habilito las cajas
		inputClientesPorHora.interactable = true;
		inputProbAbandono.interactable = true;
		inputProbReprobar.interactable = true;
		inputHorasCaidaSistema.interactable = true;
		inputNumCaidaSistema.interactable = true;
		inputProbDuplicados.interactable = true;
	}
	private void resetearColas_Cajas () {
		for (int i = 0; i < colas.Length; i++) {
			colas[i].desocuparCola ();
		}
		for (int i = 0; i < cajas.Length; i++) {
			if (cajas[i].estado != "Inactiva") {
				cajas[i].desocuparCaja ();
			}
		}
	}
	public void btnMasOpciones () {
		anim.SetBool ("MasOpciones", true);
	}
	public void btnMenosOpciones () {
		anim.SetBool ("MasOpciones", false);
	}
}