using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrocaCor : MonoBehaviour
{
    [Header("Paleta de Cores (ScriptableObject)")]
    public PaletaDeCores paleta;

    [HideInInspector]
    public int corIndex;

    public void Awake()
    {
        if (paleta == null || paleta.cores == null || paleta.cores.Length == 0)
        {
            Debug.LogError("Nenhuma paleta ou cor definida!");
            return;
        }

        if (corIndex < 0 || corIndex >= paleta.cores.Length)
        {
            Debug.LogError("Índice de cor fora dos limites!");
            return;
        }

        Color corEscolhida = paleta.cores[corIndex].cor;

        // Tenta aplicar em TextMeshPro / TextMeshProUGUI
        TMP_Text tmpElement = GetComponent<TMP_Text>();
        if (tmpElement != null)
        {
            tmpElement.color = corEscolhida;
            return;
        }

        // Tenta aplicar em componentes de UI padrão (Text, Image, etc.)
        Graphic uiElement = GetComponent<Graphic>();
        if (uiElement != null)
        {
            uiElement.color = corEscolhida;
            return;
        }

        // Tenta aplicar em Renderers normais (SpriteRenderer, MeshRenderer, etc.)
        Renderer render = GetComponent<Renderer>();
        if (render != null)
        {
            // O uso de .material cria uma cópia única do material, não afetando os outros objetos
            render.material.color = corEscolhida;
        }
    }
}
