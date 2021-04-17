using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBolita : MonoBehaviour {
	void OnTriggerEnter(Collider col)
	{
		Destroy(col.gameObject, 5.5f);
	}
}
