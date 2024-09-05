using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : InitBase
{
	private Hero _target;
	public Hero Target
	{
		get { return _target; }
		set 
        { 
            _target = value; 
            // 카메라가 초기화된 후에 Target이 설정되면 Follow를 적용
            if (_virtualCam != null && _target != null)
            {
                _virtualCam.Follow = _target.transform;
            }
        }
	}

    public CinemachineVirtualCamera _virtualCam;


	protected override bool Init()
	{
		if (base.Init() == false)
			return false;
        _virtualCam = GetComponent<CinemachineVirtualCamera>();
		return true;
	}
}
