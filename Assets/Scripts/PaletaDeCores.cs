using UnityEngine;

[CreateAssetMenu(fileName = "NovaPaletaDeCores", menuName = "Paleta de Cores (Custom)")]
public class PaletaDeCores : ScriptableObject
{
    [System.Serializable]
    public class CorNomeada
    {
        public string nome;
        public Color cor = Color.white;
    }

    [Header("Lista Global de Cores")]
    public CorNomeada[] cores;
}
