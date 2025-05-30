﻿
using System;

namespace PngifyMe.Layers.Helper;

/// <summary>
/// Visualization: https://easings.net/#
/// </summary>
public static class Easings
{
    private const float PI = MathF.PI;
    private const float HALF_PI = MathF.PI / 2.0f;

    /// <summary>
    /// Easing Functions enumeration
    /// </summary>
    public enum Functions
    {
        Linear,
        QuadraticEaseIn,
        QuadraticEaseOut,
        QuadraticEaseInOut,
        CubicEaseIn,
        CubicEaseOut,
        CubicEaseInOut,
        QuarticEaseIn,
        QuarticEaseOut,
        QuarticEaseInOut,
        QuinticEaseIn,
        QuinticEaseOut,
        QuinticEaseInOut,
        SineEaseIn,
        SineEaseOut,
        SineEaseInOut,
        CircularEaseIn,
        CircularEaseOut,
        CircularEaseInOut,
        ExponentialEaseIn,
        ExponentialEaseOut,
        ExponentialEaseInOut,
        ElasticEaseIn,
        ElasticEaseOut,
        ElasticEaseInOut,
        BackEaseIn,
        BackEaseOut,
        BackEaseInOut,
        BounceEaseIn,
        BounceEaseOut,
        BounceEaseInOut
    }


    /// <summary>
    /// Interpolate using the specified function.
    /// </summary>
    public static float Interpolate(float p, Functions function)
    {
        switch (function)
        {
            case Functions.Linear:
                return Linear(p);
            case Functions.QuadraticEaseOut:
                return QuadraticEaseOut(p);
            case Functions.QuadraticEaseIn:
                return QuadraticEaseIn(p);
            case Functions.QuadraticEaseInOut:
                return QuadraticEaseInOut(p);
            case Functions.CubicEaseIn:
                return CubicEaseIn(p);
            case Functions.CubicEaseOut:
                return CubicEaseOut(p);
            case Functions.CubicEaseInOut:
                return CubicEaseInOut(p);
            case Functions.QuarticEaseIn:
                return QuarticEaseIn(p);
            case Functions.QuarticEaseOut:
                return QuarticEaseOut(p);
            case Functions.QuarticEaseInOut:
                return QuarticEaseInOut(p);
            case Functions.QuinticEaseIn:
                return QuinticEaseIn(p);
            case Functions.QuinticEaseOut:
                return QuinticEaseOut(p);
            case Functions.QuinticEaseInOut:
                return (float)QuinticEaseInOut(p);
            case Functions.SineEaseIn:
                return (float)SineEaseIn(p);
            case Functions.SineEaseOut:
                return (float)SineEaseOut(p);
            case Functions.SineEaseInOut:
                return (float)SineEaseInOut(p);
            case Functions.CircularEaseIn:
                return (float)CircularEaseIn(p);
            case Functions.CircularEaseOut:
                return (float)CircularEaseOut(p);
            case Functions.CircularEaseInOut:
                return (float)CircularEaseInOut(p);
            case Functions.ExponentialEaseIn:
                return (float)ExponentialEaseIn(p);
            case Functions.ExponentialEaseOut:
                return (float)ExponentialEaseOut(p);
            case Functions.ExponentialEaseInOut:
                return (float)ExponentialEaseInOut(p);
            case Functions.ElasticEaseIn:
                return (float)ElasticEaseIn(p);
            case Functions.ElasticEaseOut:
                return (float)ElasticEaseOut(p);
            case Functions.ElasticEaseInOut:
                return (float)ElasticEaseInOut(p);
            case Functions.BackEaseIn:
                return (float)BackEaseIn(p);
            case Functions.BackEaseOut:
                return (float)BackEaseOut(p);
            case Functions.BackEaseInOut:
                return (float)BackEaseInOut(p);
            case Functions.BounceEaseIn:
                return BounceEaseIn(p);
            case Functions.BounceEaseOut:
                return BounceEaseOut(p);
            case Functions.BounceEaseInOut:
                return BounceEaseInOut(p);
            default:
                return Linear(p);
        }
    }

    /// <summary>
    /// Modeled after the line y = x
    /// </summary>
    public static float Linear(float p)
    {
        return p;
    }

    /// <summary>
    /// Modeled after the parabola y = x^2
    /// </summary>
    public static float QuadraticEaseIn(float p)
    {
        return p * p;
    }

    /// <summary>
    /// Modeled after the parabola y = -x^2 + 2x
    /// </summary>
    public static float QuadraticEaseOut(float p)
    {
        return -(p * (p - 2));
    }

    /// <summary>
    /// Modeled after the piecewise quadratic
    /// y = (1/2)((2x)^2)             ; [0, 0.5)
    /// y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
    /// </summary>
    public static float QuadraticEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            return 2 * p * p;
        }
        return -2 * p * p + 4 * p - 1;
    }

    /// <summary>
    /// Modeled after the cubic y = x^3
    /// </summary>
    public static float CubicEaseIn(float p)
    {
        return p * p * p;
    }

    /// <summary>
    /// Modeled after the cubic y = (x - 1)^3 + 1
    /// </summary>
    public static float CubicEaseOut(float p)
    {
        float f = p - 1;
        return f * f * f + 1;
    }

    /// <summary>	
    /// Modeled after the piecewise cubic
    /// y = (1/2)((2x)^3)       ; [0, 0.5)
    /// y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
    /// </summary>
    public static float CubicEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            return 4 * p * p * p;
        }
        float f = 2 * p - 2;
        return 0.5f * f * f * f + 1;
    }

    /// <summary>
    /// Modeled after the quartic x^4
    /// </summary>
    public static float QuarticEaseIn(float p)
    {
        return p * p * p * p;
    }

    /// <summary>
    /// Modeled after the quartic y = 1 - (x - 1)^4
    /// </summary>
    public static float QuarticEaseOut(float p)
    {
        float f = p - 1;
        return f * f * f * (1 - p) + 1;
    }

    /// <summary>
    /// Modeled after the piecewise quartic
    /// y = (1/2)((2x)^4)        ; [0, 0.5)
    /// y = -(1/2)((2x-2)^4 - 2) ; [0.5, 1]
    /// </summary>
    public static float QuarticEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            return 8 * p * p * p * p;
        }
        float f = p - 1;
        return -8 * f * f * f * f + 1;
    }

    /// <summary>
    /// Modeled after the quintic y = x^5
    /// </summary>
    public static float QuinticEaseIn(float p)
    {
        return p * p * p * p * p;
    }

    /// <summary>
    /// Modeled after the quintic y = (x - 1)^5 + 1
    /// </summary>
    public static float QuinticEaseOut(float p)
    {
        float f = p - 1;
        return f * f * f * f * f + 1;
    }

    /// <summary>
    /// Modeled after the piecewise quintic
    /// y = (1/2)((2x)^5)       ; [0, 0.5)
    /// y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
    /// </summary>
    public static double QuinticEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            return 16 * p * p * p * p * p;
        }
        float f = 2 * p - 2;
        return 0.5f * f * f * f * f * f + 1;
    }

    /// <summary>
    /// Modeled after quarter-cycle of sine wave
    /// </summary>
    public static double SineEaseIn(float p)
    {
        return Math.Sin((p - 1) * HALF_PI) + 1;
    }

    /// <summary>
    /// Modeled after quarter-cycle of sine wave (different phase)
    /// </summary>
    public static double SineEaseOut(float p)
    {
        return Math.Sin(p * HALF_PI);
    }

    /// <summary>
    /// Modeled after half sine wave
    /// </summary>
    public static double SineEaseInOut(float p)
    {
        return 0.5f * (1 - Math.Cos(p * PI));
    }

    /// <summary>
    /// Modeled after shifted quadrant IV of unit circle
    /// </summary>
    public static double CircularEaseIn(float p)
    {
        return 1 - Math.Sqrt(1 - p * p);
    }

    /// <summary>
    /// Modeled after shifted quadrant II of unit circle
    /// </summary>
    public static double CircularEaseOut(float p)
    {
        return Math.Sqrt((2 - p) * p);
    }

    /// <summary>	
    /// Modeled after the piecewise circular function
    /// y = (1/2)(1 - Math.Sqrt(1 - 4x^2))           ; [0, 0.5)
    /// y = (1/2)(Math.Sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
    /// </summary>
    public static double CircularEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            return 0.5f * (1 - Math.Sqrt(1 - 4 * (p * p)));
        }
        return 0.5f * (Math.Sqrt(-(2 * p - 3) * (2 * p - 1)) + 1);
    }

    /// <summary>
    /// Modeled after the exponential function y = 2^(10(x - 1))
    /// </summary>
    public static double ExponentialEaseIn(float p)
    {
        return p == 0.0f ? p : Math.Pow(2, 10 * (p - 1));
    }

    /// <summary>
    /// Modeled after the exponential function y = -2^(-10x) + 1
    /// </summary>
    public static double ExponentialEaseOut(float p)
    {
        return p == 1.0f ? p : 1 - Math.Pow(2, -10 * p);
    }

    /// <summary>
    /// Modeled after the piecewise exponential
    /// y = (1/2)2^(10(2x - 1))         ; [0,0.5)
    /// y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
    /// </summary>
    public static double ExponentialEaseInOut(float p)
    {
        if (p == 0.0 || p == 1.0)
            return p;

        if (p < 0.5f)
        {
            return 0.5f * Math.Pow(2, 20 * p - 10);
        }
        return -0.5f * Math.Pow(2, -20 * p + 10) + 1;
    }

    /// <summary>
    /// Modeled after the damped sine wave y = sin(13pi/2*x)*Math.Pow(2, 10 * (x - 1))
    /// </summary>
    public static double ElasticEaseIn(float p)
    {
        return Math.Sin(13 * HALF_PI * p) * Math.Pow(2, 10 * (p - 1));
    }

    /// <summary>
    /// Modeled after the damped sine wave y = sin(-13pi/2*(x + 1))*Math.Pow(2, -10x) + 1
    /// </summary>
    public static double ElasticEaseOut(float p)
    {
        return Math.Sin(-13 * HALF_PI * (p + 1)) * Math.Pow(2, -10 * p) + 1;
    }

    /// <summary>
    /// Modeled after the piecewise exponentially-damped sine wave:
    /// y = (1/2)*sin(13pi/2*(2*x))*Math.Pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
    /// y = (1/2)*(sin(-13pi/2*((2x-1)+1))*Math.Pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
    /// </summary>
    public static double ElasticEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            return 0.5f * Math.Sin(13 * HALF_PI * (2 * p)) * Math.Pow(2, 10 * (2 * p - 1));
        }
        return 0.5f * (Math.Sin(-13 * HALF_PI * (2 * p - 1 + 1)) * Math.Pow(2, -10 * (2 * p - 1)) + 2);
    }

    /// <summary>
    /// Modeled after the overshooting cubic y = x^3-x*sin(x*pi)
    /// </summary>
    public static double BackEaseIn(float p)
    {
        return p * p * p - p * Math.Sin(p * PI);
    }

    /// <summary>
    /// Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*sin((1-x)*pi))
    /// </summary>	
    public static double BackEaseOut(float p)
    {
        float f = 1 - p;
        return 1 - (f * f * f - f * Math.Sin(f * PI));
    }

    /// <summary>
    /// Modeled after the piecewise overshooting cubic function:
    /// y = (1/2)*((2x)^3-(2x)*sin(2*x*pi))           ; [0, 0.5)
    /// y = (1/2)*(1-((1-x)^3-(1-x)*sin((1-x)*pi))+1) ; [0.5, 1]
    /// </summary>
    public static double BackEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            float f = 2 * p;
            return 0.5f * (f * f * f - f * Math.Sin(f * PI));
        }
        else
        {
            float f = 1 - (2 * p - 1);
            return 0.5f * (1 - (f * f * f - f * Math.Sin(f * PI))) + 0.5f;
        }
    }

    /// <summary>
    /// </summary>
    public static float BounceEaseIn(float p)
    {
        return 1 - BounceEaseOut(1 - p);
    }

    /// <summary>
    /// </summary>
    public static float BounceEaseOut(float p)
    {
        if (p < 4 / 11.0f)
        {
            return 121 * p * p / 16.0f;
        }
        if (p < 8 / 11.0f)
        {
            return 363 / 40.0f * p * p - 99 / 10.0f * p + 17 / 5.0f;
        }
        if (p < 9 / 10.0f)
        {
            return 4356 / 361.0f * p * p - 35442 / 1805.0f * p + 16061 / 1805.0f;
        }
        return 54 / 5.0f * p * p - 513 / 25.0f * p + 268 / 25.0f;
    }

    /// <summary>
    /// </summary>
    public static float BounceEaseInOut(float p)
    {
        if (p < 0.5f)
        {
            return 0.5f * BounceEaseIn(p * 2);
        }
        return 0.5f * BounceEaseOut(p * 2 - 1) + 0.5f;
    }

    public static float Lerp(float start, float end, float amount)
    {
        // Ensure amount is clamped between 0 and 1
        amount = Math.Max(0, Math.Min(1, amount));

        // Perform linear interpolation
        return start + (end - start) * amount;
    }
}