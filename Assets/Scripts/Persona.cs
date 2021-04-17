using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persona {
	//para diferenciar a la persona
	public string nombre = "";
	//aleatorio entre 0f y 1f para determinar el tiempo de llegada de la persona
	public string aleatorio1 = "";
	//aleatorio entre 0f y 1f para determinar el tiempo de espera en en servicio de atencion
	public string[] aleatorio2 = new string[4] { "", "", "", "" };
	//tiempo de llegada del cliente despues de cliente anterior (funcion logaritmica)
	public string tiempoLlegada = "";
	//hora que llego a la cola
	public string[] momentoLlegada = new string[4] { "", "", "", "" };
	//hora que llego al servicio de atencion
	public string[] tiempoInicioServicio = new string[4] { "", "", "", "" };
	//tiempo total que estuvo en cola
	public string[] tiempoEspera = new string[4] { "", "", "", "" };
	//tiempo que se demoro en el servicio de atencion (funcion logaritmica)
	public string[] tiempoAtencion = new string[4] { "", "", "", "" };
	//hora que termino y salio del servicio en la etapa actual
	public string[] tiempoSalida = new string[4] { "", "", "", "" };
}