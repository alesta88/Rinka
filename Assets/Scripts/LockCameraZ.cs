using UnityEngine;
using Cinemachine;
using UniRx;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class LockCameraZ : CinemachineExtension
{

    private void OnEnable()
    {
        Init();
    }
    void Init()
    {
        GameModel.GameSubState.TakeUntilDestroy(this).Subscribe(substate => OnGameSubStateChanged(substate));
    }
    void OnGameSubStateChanged(Define.GameSubState substate)
    {
        if (substate == Define.GameSubState.BonusTime)
        {
            init = false;
        }
        else
        {
            init = true;
        }

    }



    [Tooltip("Lock the camera's Z position to this value")]
    public float m_YPosition = 0;
    public bool init=true;

    protected override void PostPipelineStageCallback(
    CinemachineVirtualCameraBase vcam,
    CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (init)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                pos.y = m_YPosition;
                state.RawPosition = pos;
            }
        }
        
    }
}