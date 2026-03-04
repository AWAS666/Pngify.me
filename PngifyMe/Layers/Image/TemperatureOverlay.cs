using LibreHardwareMonitor.Hardware;
using PngifyMe.Layers.Helper;
using SkiaSharp;
using System;
using System.Text.Json.Serialization;

namespace PngifyMe.Layers.Image;

/// <summary>
/// Displays CPU and GPU temperature on the canvas. On Windows, CPU temperature may require running the app as administrator.
/// </summary>
[LayerDescription("TemperatureOverlay")]
public class TemperatureOverlay : ImageLayer
{
    private Computer? _computer;
    private SKPaint? _paint;
    private float? _cachedCpuTemp;
    private float? _cachedGpuTemp;
    private float _updateAccumulator;
    private const float UpdateIntervalSeconds = 0.5f;

    [Unit("Pixels")]
    public int TextSize { get; set; } = 48;

    [Unit("pixels (center)")]
    public float PosX { get; set; } = 100;

    [Unit("pixels (center)")]
    public float PosY { get; set; } = 100;

    [JsonIgnore]
    [CanvasPosition]
    public CanvasPosition2D Position
    {
        get => new() { X = PosX, Y = PosY };
        set { PosX = value.X; PosY = value.Y; }
    }

    [Unit("bool")]
    public bool ShowCpu { get; set; } = true;

    [Unit("bool")]
    public bool ShowGpu { get; set; } = true;

    public override void OnEnter()
    {
        _paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = TextSize,
            IsAntialias = true
        };

        try
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true
            };
            _computer.Open();
        }
        catch
        {
            _computer = null;
        }

        base.OnEnter();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        _updateAccumulator += dt;
        if (_updateAccumulator < UpdateIntervalSeconds || _computer == null)
            return;

        _updateAccumulator = 0f;

        try
        {
            float? cpuTemp = null;
            float? gpuTemp = null;

            foreach (IHardware hardware in _computer.Hardware)
            {
                hardware.Update();

                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (sensor.SensorType != SensorType.Temperature || sensor.Value == null)
                        continue;

                    float value = sensor.Value.Value;
                    switch (hardware.HardwareType)
                    {
                        case HardwareType.Cpu:
                            if (!cpuTemp.HasValue || value > cpuTemp.Value)
                                cpuTemp = value;
                            break;
                        case HardwareType.GpuNvidia:
                        case HardwareType.GpuAmd:
                        case HardwareType.GpuIntel:
                            if (!gpuTemp.HasValue || value > gpuTemp.Value)
                                gpuTemp = value;
                            break;
                    }
                }

                foreach (IHardware sub in hardware.SubHardware)
                {
                    sub.Update();
                    foreach (ISensor sensor in sub.Sensors)
                    {
                        if (sensor.SensorType != SensorType.Temperature || sensor.Value == null)
                            continue;

                        float value = sensor.Value.Value;
                        switch (hardware.HardwareType)
                        {
                            case HardwareType.Cpu:
                                if (!cpuTemp.HasValue || value > cpuTemp.Value)
                                    cpuTemp = value;
                                break;
                            case HardwareType.GpuNvidia:
                            case HardwareType.GpuAmd:
                            case HardwareType.GpuIntel:
                                if (!gpuTemp.HasValue || value > gpuTemp.Value)
                                    gpuTemp = value;
                                break;
                        }
                    }
                }
            }

            _cachedCpuTemp = cpuTemp;
            _cachedGpuTemp = gpuTemp;
        }
        catch
        {
            _cachedCpuTemp = null;
            _cachedGpuTemp = null;
        }
    }

    public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
    {
        if (_paint == null)
            return;

        string cpuText = ShowCpu
            ? (_cachedCpuTemp.HasValue ? $"CPU: {_cachedCpuTemp.Value:F0}°C" : "CPU: N/A")
            : string.Empty;
        string gpuText = ShowGpu
            ? (_cachedGpuTemp.HasValue ? $"GPU: {_cachedGpuTemp.Value:F0}°C" : "GPU: N/A")
            : string.Empty;
        string separator = (ShowCpu && ShowGpu) ? "  " : string.Empty;
        string text = cpuText + separator + gpuText;

        if (string.IsNullOrEmpty(text))
            return;

        canvas.DrawText(text, PosX + offsetX, PosY + offsetY, _paint);
    }

    public override void OnExit()
    {
        try
        {
            _computer?.Close();
        }
        catch
        {
            // Ignore on shutdown
        }

        _paint?.Dispose();
        _paint = null;
        _computer = null;
        base.OnExit();
    }
}
