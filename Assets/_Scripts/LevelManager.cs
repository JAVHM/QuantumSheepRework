using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DayCycle
{
    Day,
    Night,
}

public class LevelManager : MonoBehaviour
{
    public DayCycle _currentCycle;
    public int _dayCicleTime;
    public int _dayCicleCurrentTime;
    public static LevelManager _instance;
    public LevelUI _levelUI;
    private LevelUI _instanceLevelUI;

    public Action onChangeDaycycle;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _dayCicleCurrentTime = _dayCicleTime;
        _instanceLevelUI = Instantiate(_levelUI);
        if (_dayCicleTime != 0)
            _instanceLevelUI.SetTimeText(_dayCicleCurrentTime);
        else
            _instanceLevelUI.SetTimeText();
        _instanceLevelUI.SetGameIlumination(_currentCycle);
        GameplayManager.onUnitMove += UpdateDaycicle;
    }

    public void OnDestroy()
    {
        GameplayManager.onUnitMove -= UpdateDaycicle;
    }

    public void UpdateDaycicle()
    {
        if (_dayCicleTime != 0)
        {
            _dayCicleCurrentTime--;
            _instanceLevelUI.SetTimeText(_dayCicleCurrentTime);
            if (_dayCicleCurrentTime <= 0)
            {
                _dayCicleCurrentTime = _dayCicleTime;
                _instanceLevelUI.SetTimeText(_dayCicleCurrentTime);
                if (_currentCycle == DayCycle.Day)
                {
                    _currentCycle = DayCycle.Night;
                    _instanceLevelUI.SetGameIlumination(_currentCycle);
                    onChangeDaycycle.Invoke();
                }
                else
                {
                    _currentCycle = DayCycle.Day;
                    _instanceLevelUI.SetGameIlumination(_currentCycle);
                    onChangeDaycycle.Invoke();
                }
            }
        }
        else
        {
            _instanceLevelUI.SetTimeText();
        }
    }
}
