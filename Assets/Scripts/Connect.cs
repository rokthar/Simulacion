using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect : MonoBehaviour
{
      string servidor = "http://localhost/cajeros/public/index.php/";

      public void modificarInfoCajero (Cajero cajero) {
            string json = JsonUtility.ToJson (cajero);
            print (json);
            string url = servidor + "cajero/editar/" + cajero.num_cajero;

            Dictionary<string, string> headers = new Dictionary<string, string> ();
            headers.Add ("Content-Type", "application/json; charset=utf-8");
            byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

            WWW www = new WWW (url, pData, headers);
            StartCoroutine (requestModificarCajero (www));
	}
	IEnumerator requestModificarCajero (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
                  print(www.text);
		} else
                  print( www.error);
	}
      public void crearUser (Usuario user) {
            string json = JsonUtility.ToJson (user);
            print (json);
            string url = servidor + "usuario/registro";

            Dictionary<string, string> headers = new Dictionary<string, string> ();
            headers.Add ("Content-Type", "application/json; charset=utf-8");
            byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

            WWW www = new WWW (url, pData, headers);
            StartCoroutine (requestRegistroUser (www));
	}
	IEnumerator requestRegistroUser (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
                  print(www.text);
		} else
                 print( www.error);
	}
}