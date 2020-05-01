using UnityEngine;
using Cinemachine;

public class CameraMgr : MonoSingleton<CameraMgr> {
    [SerializeField] Camera m_camera;
    [SerializeField] CinemachineVirtualCamera m_cinemachineCam;

    public void Follow( Transform target ) {
        m_cinemachineCam.Follow = target;
        Debug.Log("Target" + target.transform.position);
    }

    ///////////////////////////
    public void RotateCamera(float zrot)
    {
        var composer = m_cinemachineCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        var mycam_rotation = m_cinemachineCam.gameObject.transform.rotation.z;
        Debug.Log("ZROT" + zrot+"  " + mycam_rotation);
        if (mycam_rotation==0f&&zrot!=0)
        {
            //m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, -90f, Space.World); 
            composer.m_DeadZoneHeight = 0.35f;
            //PlayerMgr.Instance.PlayerInstance.m_flyVector.x = 50f;
            //PlayerMgr.Instance.PlayerInstance.m_flyVector.y = 180f;
            //PlayerMgr.Instance.PlayerInstance.m_glideVector.x = 5f;
            //PlayerMgr.Instance.PlayerInstance.m_glideVector.y = 177f;
            //PlayerMgr.Instance.PlayerInstance.m_fallVector.x = -15f;
            //PlayerMgr.Instance.PlayerInstance.m_fallVector.y = 4.5f;
            //PlayerMgr.Instance.PlayerInstance.m_leftVector.x = 5f;
            //PlayerMgr.Instance.PlayerInstance.m_leftVector.y = 200f;
            //PlayerMgr.Instance.PlayerInstance.m_LeftDuration = 10f;
            //PlayerMgr.Instance.PlayerInstance.m_rightVector.x = 10f;
            //PlayerMgr.Instance.PlayerInstance.m_rightVector.y = 150f;
            //PlayerMgr.Instance.PlayerInstance.m_rightDuration = 1f;
        }
        if (mycam_rotation !=0&&zrot==0)
        {
            m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, 90f, Space.World);
           // composer.m_DeadZoneHeight = 1f;
            composer.m_DeadZoneHeight = 0.25f;
        }
        Debug.Log("035");
    }

    public void YposCamera()
    {
        var mycam_xpos = m_cinemachineCam.gameObject.transform.position.y;
        m_cinemachineCam.gameObject.transform.position.Set(0f, 0f, 0f);
    }

    public void Start()
    {
        m_cinemachineCam.gameObject.transform.position.Set(0f, 0f, 0f);
        //m_cinemachineCam.gameObject.transform.rotation = Quaternion.Euler(0f,0f,0f);
        //   m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, 0f, Space.Self);
    }

    public void Update()
    {
        m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, +0.05f, Space.World);
    }

}
