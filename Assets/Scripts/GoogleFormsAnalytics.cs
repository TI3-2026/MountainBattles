using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GoogleFormsAnalytics : MonoBehaviour
{
    public static GoogleFormsAnalytics Instance;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("Configuração")]
    public string formUrl = "https://docs.google.com/forms/d/e/1FAIpQLScCIfj3F9AocxTCwDKFbqwHOJeQrv_05XyPb1E-DOeBuLnG_Q/formResponse";

    [Header("IDs dos campos")]
    private string jogouMemoria = "entry.1982886362";
    private string jogouBatalha = "entry.1991319849";
    private string cartasAcertadas = "entry.1995887842";
    private string skillChecksAcertadas = "entry.39793906";
    private string skillChecksErradas = "entry.607807804";

    public void SendForm(
        bool resJogouMemoria=false,
        bool resJogouBatalha=false,
        float resCartasAcertadas=0,
        float resSkillChecksAcertadas=0,
        float resSkillChecksErradas=0)
    {
        StartCoroutine(EnviarDados(
            resJogouMemoria,
            resJogouBatalha,
            resCartasAcertadas,
            resSkillChecksAcertadas,
            resSkillChecksErradas
            ));
    }
    
    IEnumerator EnviarDados(
        bool resJogouMemoria=false,
        bool resJogouBatalha=false,
        float resCartasAcertadas=0,
        float resSkillChecksAcertadas=0,
        float resSkillChecksErradas=0)
    {
        
        WWWForm form = new WWWForm();
        if (resJogouMemoria) form.AddField(jogouMemoria, "Sim");
        if (resJogouBatalha) form.AddField(jogouBatalha, "Sim");
        if (resCartasAcertadas > 0) form.AddField(cartasAcertadas, resCartasAcertadas.ToString());
        if (resSkillChecksAcertadas > 0) form.AddField(skillChecksAcertadas, resSkillChecksAcertadas.ToString());
        if (resSkillChecksErradas > 0) form.AddField(skillChecksErradas, resSkillChecksErradas.ToString());
        
        UnityWebRequest www = UnityWebRequest.Post(formUrl, form);
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success){
            Debug.Log("Dados enviados com sucesso!");
        } else {
            Debug.Log("Erro ao enviar dados: " + www.error);
        }
    }
}