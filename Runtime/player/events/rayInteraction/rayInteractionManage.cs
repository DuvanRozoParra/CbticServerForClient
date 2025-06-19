using System.Collections.Generic;
using UnityEngine;

public class RayInteractionManage : MonoBehaviour
{
    private Dictionary<string, RayInteraction> rayInteractions;

    void Awake()
    {
        rayInteractions = new Dictionary<string, RayInteraction>();
        RayInteraction[] foundInteractions = FindObjectsByType<RayInteraction>(FindObjectsSortMode.None);

        foreach (var interaction in foundInteractions)
        {
            if (!rayInteractions.ContainsKey(interaction.idRayInteraction))
            {
                rayInteractions.Add(interaction.idRayInteraction, interaction);
                // Debug.Log($"Registrado: {interaction.idRayInteraction}");
            }
            else Debug.LogWarning($"El evento {interaction.idRayInteraction} ya está registrado.");

        }
    }

    public void TriggerEvent(MessageRayInteraction data)
    {
        if (rayInteractions.TryGetValue(data.idRayInteraction, out RayInteraction interaction))
            interaction.ExecutedAction(data.actionType);
        else
            Debug.LogWarning($"El evento {data.idRayInteraction} no fue encontrado.");
    }
}