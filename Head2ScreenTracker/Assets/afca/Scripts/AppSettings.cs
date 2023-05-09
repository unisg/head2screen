using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSettings 
{
   Vector2 ScreenSize { get; set; }
    public Vector2 ScreenResolution { get; set; }
    public Vector3 ScreenOffset { get; set; }

    private static readonly Lazy<AppSettings> lazy = new Lazy<AppSettings>(() => new AppSettings());
    public static AppSettings Instance
    {
        get
        {
            return lazy.Value;
        }
    }
}
