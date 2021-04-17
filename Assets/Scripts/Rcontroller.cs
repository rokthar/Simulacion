using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;

public class Rcontroller : MonoBehaviour
{
    public string rutaR_scripts = @"C:\Users\Wargosh\Documents\Project Simulacion Cajero Automatico\Assets\Rscripts\";
    System.Diagnostics.Process psTipoTransaccion = new System.Diagnostics.Process();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tipo_trans">
    /// 0 = monto transaccion, 
    /// 1 = respuesta transaccion dinero,
    /// 2 = respuesta transaccion ticket, 
    /// 3 = tipo transaccion</param>
    /// <returns>devuelve la respuesta del cajero de acuerdo a las probabilidades</returns>
    public string callRscript(int tipo_trans){
        string transaccion = "monto_transaccion.R";
        switch (tipo_trans){
            case 0:
                transaccion = "monto_transaccion.R";
                break;
            case 1:
                transaccion = "respuesta_transaccion_dinero.R";
                break;
            case 2:
                transaccion = "respuesta_transaccion_ticket.R";
                break;
            case 3:
                transaccion = "tipo_transaccion.R";
                break;
        }
        
        string file = "\""+ rutaR_scripts + transaccion + "\"";
        string result = string.Empty;

        try {
            var info = new ProcessStartInfo();
            info.FileName = @"C:\Program Files\R\R-3.5.2\bin\x64\Rscript.exe";
            info.WorkingDirectory = Path.GetDirectoryName(@"C:\Program Files\R\R-3.5.2\bin\x64\Rscript.exe");
            info.Arguments = file;

            info.RedirectStandardInput = false;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            using (var proc = new Process())
            {
                proc.StartInfo = info;
                proc.Start();
                result = proc.StandardOutput.ReadToEnd();
            }
            //se cura la respuesta, haciendo que de: <<[1] "respuesta">>, se vea como <<respuesta>>
            string aux = result.Replace("[1]","");
            aux = aux.Replace("\"","");
            UnityEngine.Debug.Log(aux);
            return aux.Trim(); //devuelve la respuesta del script...
        } catch (Exception ex) {
            throw new Exception("R Script failed: " + result, ex);
        }
    }
}
