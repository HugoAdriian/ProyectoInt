using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    #region REFERENCES

    [Header("Pantalla de tareas")]
    [SerializeField] private TMP_Text orderText;

    #endregion

    #region TASK BOARD

    public void UpdateTaskBoard(
        List<PutAwayTask> tasks,
        int currentTaskIndex,
        HashSet<int> completedTaskIndexes,
        bool currentTaskCompleted)
    {
        if (orderText == null)
        {
            Debug.LogError(
                "OrderUI: Order Text no está asignado.",
                this);

            return;
        }

        if (tasks == null || tasks.Count == 0)
        {
            orderText.text =
                "NO HAY TAREAS PUT AWAY";

            return;
        }

        string text = "";

        //--------------------------------------------------
        // TAREA ACTUAL
        //--------------------------------------------------

        text += "TAREA ACTUAL\n";
        text += "-------------------------\n";

        if (currentTaskIndex >= 0 &&
            currentTaskIndex < tasks.Count)
        {
            PutAwayTask currentTask =
                tasks[currentTaskIndex];

            text +=
                $"{currentTask.taskID}\n";

            text +=
                $"Pallet: {currentTask.palletID}\n";

            text +=
                $"Producto: {currentTask.productName}\n";

            text +=
                $"Origen: {currentTask.originLocationID}\n";

            text +=
                $"Destino: {currentTask.destinationLocationID}\n";

            text +=
                currentTaskCompleted
                    ? "Estado: COMPLETADA\n"
                    : "Estado: EN PROCESO\n";
        }
        else
        {
            text += "Sin tarea activa\n";
        }

        //--------------------------------------------------
        // COMPLETADAS
        //--------------------------------------------------

        text += "\n";
        text += "COMPLETADAS\n";
        text += "-------------------------\n";

        bool hasCompletedTasks = false;

        for (int i = 0; i < tasks.Count; i++)
        {
            if (!completedTaskIndexes.Contains(i))
                continue;

            PutAwayTask task = tasks[i];

            text +=
                $"[OK] {task.taskID} | " +
                $"{task.palletID} | " +
                $"{task.destinationLocationID}\n";

            hasCompletedTasks = true;
        }

        if (!hasCompletedTasks)
        {
            text += "Ninguna\n";
        }

        //--------------------------------------------------
        // PENDIENTES
        //--------------------------------------------------

        text += "\n";
        text += "PENDIENTES\n";
        text += "-------------------------\n";

        bool hasPendingTasks = false;

        for (int i = 0; i < tasks.Count; i++)
        {
            //------------------------------------------
            // No mostrar la tarea actual
            //------------------------------------------

            if (i == currentTaskIndex)
                continue;

            //------------------------------------------
            // No mostrar tareas completadas
            //------------------------------------------

            if (completedTaskIndexes.Contains(i))
                continue;

            PutAwayTask task = tasks[i];

            text +=
                $"{task.taskID} | " +
                $"{task.palletID} | " +
                $"{task.destinationLocationID}\n";

            hasPendingTasks = true;
        }

        if (!hasPendingTasks)
        {
            text += "Ninguna\n";
        }

        //--------------------------------------------------
        // ACTUALIZAR PANTALLA
        //--------------------------------------------------

        orderText.text = text;
    }

    #endregion
}