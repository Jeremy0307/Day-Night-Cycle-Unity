using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightController : MonoBehaviour
{
    #region Fields

    [Header("Time")]
    [Tooltip("Sets the speed of the time pass.")]
    [SerializeField] private float _timeMultiplier = 0f; //Sets the speed of the time pass.
    [Tooltip("The time at which the time starts to run out.")]
    [SerializeField] private float _startHour = 0f; //The time at which the time starts to run out.
    private DateTime _currentTime = DateTime.Now; //Allows you to obtain the current date and time on the computer.
    
    [Header("UI")]
    [Tooltip("The text on which the time will be displayed.")]
    [SerializeField] private Text _timeText = null; //The text on which the time will be displayed.
    
    [Header("Sun")] 
    [Tooltip("The directional Light that we will rotate.")]
    [SerializeField] private Light _sunLight = null; //The directional Light that we will rotate.
    [Tooltip("The time at which the sun rises.")]
    [SerializeField] private float _sunRiseHour = 0f; //The time at which the sun rises.
    [Tooltip("The time at which the sun sets.")]
    [SerializeField] private float _sunSetHour = 0f; //The time at which the sun sets.
    private TimeSpan _sunRiseTime = TimeSpan.Zero;
    private TimeSpan _sunSetTime = TimeSpan.Zero;

    #endregion

    #region Methods

    private void Start()
    {
        //Defines the date and time at which the time starts to elapse.
        _currentTime = DateTime.Now.Date + TimeSpan.FromHours(_startHour); 
        
        _sunRiseTime = TimeSpan.FromHours(_sunRiseHour);
        _sunSetTime = TimeSpan.FromHours(_sunSetHour);
    }

    private void Update()
    {
        //Updates the time at each frame.
        UpdateTime();
        
        //Updates the rotation of the sun at each frame.
        RotateSun();
    }

    private void UpdateTime()
    {
        //The update time of the _currentTime variable is based on the _timeMultiplier variable.
        _currentTime = _currentTime.AddSeconds(Time.deltaTime * _timeMultiplier);

        //Check if the text is null
        if (_timeText != null) 
        {
            //If the text is null then it is equal to the current time.
            _timeText.text = _currentTime.ToString("HH : mm"); 
        }
    }

    private void RotateSun()
    {
        float sunLightRotation;

        //Check if it is daylight.
        if (_currentTime.TimeOfDay > _sunRiseTime && _currentTime.TimeOfDay < _sunSetTime)
        {
            // If this is the case, we calculate the total time between sunrise and sunset.
            TimeSpan sunRiseToSunSetDirection = CalculateTimeDifference(_sunRiseTime, _sunSetTime);
            
            //Calculates the time since sunrise.
            TimeSpan timeSinceSunrise = CalculateTimeDifference(_sunRiseTime, _currentTime.TimeOfDay);
            
            //Calculation of the percentage of time spent during the day.
            double percentage = timeSinceSunrise.TotalMinutes / sunRiseToSunSetDirection.TotalMinutes;
            
            //Calculation of the rotation.
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage); //Met la rotation à 0 au lever du soleil et à 180 au coucher.
        }
        //It is not daylight.
        else 
        {
            //Calculates the time between sunset and sunrise.
            TimeSpan sunSetToSunriseDuration = CalculateTimeDifference(_sunSetTime, _sunRiseTime);
            //Calculates the time since sunset.
            TimeSpan timeSinceSunset = CalculateTimeDifference(_sunSetTime, _currentTime.TimeOfDay);
            
            //Calculation of the percentage of time spent at night.
            double percentage = timeSinceSunset.TotalMinutes / sunSetToSunriseDuration.TotalMinutes;
            
            //Calculation of the rotation.
            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        
        //Applies the rotation of the sun to its light.
        _sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation,Vector3.right);
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        if (difference.TotalSeconds < 0) //Check if the difference is negative.
        {
            // If it is, we move on to another day.
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
    

    #endregion
}
 