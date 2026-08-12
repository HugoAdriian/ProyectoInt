using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ForkliftLiftController : MonoBehaviour
{
    #region REFERENCES

    [Header("Etapas del mástil")]

    [Tooltip("Primera sección móvil del mástil.")]
    [SerializeField] private Transform mastStage1;   // Rig_mast_2

    [Tooltip("Segunda sección móvil del mástil.")]
    [SerializeField] private Transform mastStage2;   // Rig_mast_3

    [Tooltip("Carro final / horquillas.")]
    [SerializeField] private Transform mastStage3;   // Rig4mast_4

    #endregion


    #region CONFIGURATION

    [Header("Velocidad")]
    [SerializeField] private float liftSpeed = 0.8f;

    [Header("Recorrido Etapa 1")]
    [Tooltip("Recorrido máximo de Rig_mast_2.")]
    [SerializeField] private float stage1Travel = 1.0f;

    [Header("Recorrido Etapa 2")]
    [Tooltip("Recorrido máximo de Rig_mast_3.")]
    [SerializeField] private float stage2Travel = 1.0f;

    [Header("Recorrido Etapa 3")]
    [Tooltip("Recorrido máximo de Rig4mast_4.")]
    [SerializeField] private float stage3Travel = 1.0f;

    #endregion


    #region INITIAL POSITIONS

    private float stage1InitialY;
    private float stage2InitialY;
    private float stage3InitialY;

    private float stage1MaximumY;
    private float stage2MaximumY;
    private float stage3MaximumY;

    #endregion


    #region UNITY

    private void Awake()
    {
        if (mastStage1 == null ||
            mastStage2 == null ||
            mastStage3 == null)
        {
            Debug.LogError(
                "ForkliftLiftController: " +
                "faltan referencias de las etapas del mástil.",
                this);

            enabled = false;
            return;
        }

        //--------------------------------------------------
        // Guardar posiciones originales
        //--------------------------------------------------

        stage1InitialY =
            mastStage1.localPosition.y;

        stage2InitialY =
            mastStage2.localPosition.y;

        stage3InitialY =
            mastStage3.localPosition.y;

        //--------------------------------------------------
        // Calcular límites superiores
        //--------------------------------------------------

        stage1MaximumY =
            stage1InitialY + stage1Travel;

        stage2MaximumY =
            stage2InitialY + stage2Travel;

        stage3MaximumY =
            stage3InitialY + stage3Travel;
    }

    private void Update()
    {
        float direction =
            ReadLiftDirection();

        if (Mathf.Approximately(
            direction,
            0f))
        {
            return;
        }

        if (direction > 0f)
        {
            RaiseLift();
        }
        else
        {
            LowerLift();
        }
    }

    #endregion


    #region RAISE

    private void RaiseLift()
    {
        //--------------------------------------------------
        // ETAPA 1
        // Rig_mast_2
        //--------------------------------------------------

        if (mastStage1.localPosition.y <
            stage1MaximumY - 0.001f)
        {
            MoveStage(
                mastStage1,
                stage1MaximumY,
                1f);

            return;
        }

        //--------------------------------------------------
        // ETAPA 2
        // Rig_mast_3
        //--------------------------------------------------

        if (mastStage2.localPosition.y <
            stage2MaximumY - 0.001f)
        {
            MoveStage(
                mastStage2,
                stage2MaximumY,
                1f);

            return;
        }

        //--------------------------------------------------
        // ETAPA 3
        // Rig4mast_4
        //--------------------------------------------------

        if (mastStage3.localPosition.y <
            stage3MaximumY - 0.001f)
        {
            MoveStage(
                mastStage3,
                stage3MaximumY,
                1f);

            return;
        }
    }

    #endregion


    #region LOWER

    private void LowerLift()
    {
        //--------------------------------------------------
        // BAJAR EN ORDEN INVERSO
        //--------------------------------------------------

        // ETAPA 3
        if (mastStage3.localPosition.y >
            stage3InitialY + 0.001f)
        {
            MoveStage(
                mastStage3,
                stage3InitialY,
                -1f);

            return;
        }

        // ETAPA 2
        if (mastStage2.localPosition.y >
            stage2InitialY + 0.001f)
        {
            MoveStage(
                mastStage2,
                stage2InitialY,
                -1f);

            return;
        }

        // ETAPA 1
        if (mastStage1.localPosition.y >
            stage1InitialY + 0.001f)
        {
            MoveStage(
                mastStage1,
                stage1InitialY,
                -1f);

            return;
        }
    }

    #endregion


    #region MOVE STAGE

    private void MoveStage(
        Transform stage,
        float targetY,
        float direction)
    {
        Vector3 position =
            stage.localPosition;

        position.y =
            Mathf.MoveTowards(
                position.y,
                targetY,
                liftSpeed *
                Time.deltaTime);

        stage.localPosition =
            position;
    }

    #endregion


    #region INPUT

    private float ReadLiftDirection()
    {
#if ENABLE_INPUT_SYSTEM

        if (Keyboard.current == null)
            return 0f;

        bool liftUpPressed =
            Keyboard.current.rKey.isPressed;

        bool liftDownPressed =
            Keyboard.current.fKey.isPressed;

        if (liftUpPressed &&
            !liftDownPressed)
        {
            return 1f;
        }

        if (liftDownPressed &&
            !liftUpPressed)
        {
            return -1f;
        }

#endif

        return 0f;
    }

    #endregion
}