using UnityEngine;
using Cinemachine;

public class CameraMgr : MonoSingleton<CameraMgr> {
    [SerializeField] Camera m_camera;
    [SerializeField] public CinemachineVirtualCamera m_cinemachineCam;
    [SerializeField] float smoothspeed = 0.125f;


    public void Follow( Transform target ) {

        Vector3 targetposition = target.position;
        Vector3 smoothestposition = Vector3.Lerp(transform.position, targetposition, smoothspeed);
        target.position = smoothestposition;

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
           m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, zrot, Space.World); 
            composer.m_DeadZoneHeight = 0.15f;
            //PlayerMgr.Instance.PlayerInstance.m_flyVector.x = 50f;
            PlayerMgr.Instance.PlayerInstance.m_flyVector.x = 35f;
            //PlayerMgr.Instance.PlayerInstance.m_flyVector.y = 180f;
            PlayerMgr.Instance.PlayerInstance.m_flyVector.y = 200f;
            //PlayerMgr.Instance.PlayerInstance.m_glideVector.x = 5f;
            PlayerMgr.Instance.PlayerInstance.m_glideVector.x = 8f;
            //PlayerMgr.Instance.PlayerInstance.m_glideVector.y = 177f;
            PlayerMgr.Instance.PlayerInstance.m_glideVector.y = 160f;
            //PlayerMgr.Instance.PlayerInstance.m_fallVector.x = -15f;
            //PlayerMgr.Instance.PlayerInstance.m_fallVector.y = 4.5f;
            //PlayerMgr.Instance.PlayerInstance.m_leftVector.x = 5f;
            //PlayerMgr.Instance.PlayerInstance.m_leftVector.y = 200f;
            //PlayerMgr.Instance.PlayerInstance.m_LeftDuration = 10f;
            //PlayerMgr.Instance.PlayerInstance.m_rightVector.x = 10f;
            //PlayerMgr.Instance.PlayerInstance.m_rightVector.y = 150f;
            //PlayerMgr.Instance.PlayerInstance.m_rightDuration = 1f;
            //if(zrot==90)
            //{
            //    //PlayerMgr.Instance.PlayerInstance.m_flyVector.x = 50f;
            //    //PlayerMgr.Instance.PlayerInstance.m_flyVector.y = 180f;
            //    //PlayerMgr.Instance.PlayerInstance.m_glideVector.x = 5f;
            //    //PlayerMgr.Instance.PlayerInstance.m_glideVector.y = 177f;
            //    //PlayerMgr.Instance.PlayerInstance.m_fallVector.x = -15f;
            //    //PlayerMgr.Instance.PlayerInstance.m_fallVector.y = 4.5f;
            //    //PlayerMgr.Instance.PlayerInstance.m_leftVector.x = 5f;
            //    //PlayerMgr.Instance.PlayerInstance.m_leftVector.y = 200f;
            //    //PlayerMgr.Instance.PlayerInstance.m_LeftDuration = 10f;
            //    //PlayerMgr.Instance.PlayerInstance.m_rightVector.x = 10f;
            //    //PlayerMgr.Instance.PlayerInstance.m_rightVector.y = 150f;
            //    //PlayerMgr.Instance.PlayerInstance.m_rightDuration = 1f;
            //}
            m_cinemachineCam.GetComponent<LockCameraZ>().enabled = false;
        }
        if (mycam_rotation !=0&&zrot==0)
        {
            m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, 90f, Space.World);
           // composer.m_DeadZoneHeight = 1f;
            composer.m_DeadZoneHeight = 0.15f;
        }
        if (mycam_rotation !=0&&zrot!=0)
        {
            m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, zrot, Space.World);
           // composer.m_DeadZoneHeight = 1f;
            composer.m_DeadZoneHeight = 0.15f;
        }
        if(zrot==0)
        {
            m_cinemachineCam.gameObject.transform.rotation = Quaternion.Euler(0,0,0);
            PlayerMgr.Instance.PlayerInstance.m_flyVector.x = 17f;
            PlayerMgr.Instance.PlayerInstance.m_flyVector.y = 215f;
            PlayerMgr.Instance.PlayerInstance.m_glideVector.x = 16f;
            PlayerMgr.Instance.PlayerInstance.m_glideVector.y = 160f;
            composer.m_DeadZoneHeight = 0.15f;
        }
        else
        {
            composer.m_DeadZoneHeight = 0.15f;
        }


        Debug.Log("035");
    }

    public void CameraDeadZone(float zone)
    {
        var composer = m_cinemachineCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        composer.m_DeadZoneHeight = zone;
    }
    public void CameraToZero()
    {
        var composer = m_cinemachineCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        composer.transform.localRotation = Quaternion.identity;

    }

    public void YposCamera()
    {
        var mycam_xpos = m_cinemachineCam.gameObject.transform.position.y;
        m_cinemachineCam.gameObject.transform.position.Set(0f, 0f, 0f);
    }

    public void Start()
    {
       // Screen.orientation == ScreenOrientation.Portrait;
        m_cinemachineCam.gameObject.transform.position.Set(0f, 0f, 0f);
        //m_cinemachineCam.gameObject.transform.rotation = Quaternion.Euler(0f,0f,0f);
        //   m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, 0f, Space.Self);
        //if(Screen.orientation== ScreenOrientation.Portrait|| Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        //{
        //    m_cinemachineCam.m_Lens.OrthographicSize = 0.8f;
        //}
        //else
        //{
        //    m_cinemachineCam.m_Lens.OrthographicSize = 0.48f;
        //}
    }

    public void Update()
    {
       // m_cinemachineCam.gameObject.transform.Rotate(0.0f, 0.0f, +0.05f, Space.World);
    }

}
