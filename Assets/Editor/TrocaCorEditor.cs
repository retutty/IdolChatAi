using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrocaCor))]
public class TrocaCorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TrocaCor script = (TrocaCor)target;

        // Opcional: mostrar a referência da paleta no inspector se ela fosse public não [HideInInspector],
        // Mas vamos desenhar um ObjectField padrão para ser seguro
        SerializedProperty paletaProp = serializedObject.FindProperty("paleta");
        EditorGUILayout.PropertyField(paletaProp, new GUIContent("Paleta de Cores"));

        if (script.paleta != null && script.paleta.cores != null && script.paleta.cores.Length > 0)
        {
            string[] nomesCores = new string[script.paleta.cores.Length];
            for (int i = 0; i < script.paleta.cores.Length; i++)
            {
                nomesCores[i] = string.IsNullOrEmpty(script.paleta.cores[i].nome) ? $"Cor {i}" : script.paleta.cores[i].nome;
            }

            SerializedProperty corIndexProp = serializedObject.FindProperty("corIndex");
            corIndexProp.intValue = EditorGUILayout.Popup("Cor a aplicar", corIndexProp.intValue, nomesCores);
        }
        else
        {
            EditorGUILayout.HelpBox("Atribua uma Paleta de Cores com itens para continuar.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
