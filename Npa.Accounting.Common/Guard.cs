using System;

namespace Npa.Accounting.Common;

public class Guard
{
    public static void ForLessEqualZero(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
    
    public static void ForLessEqualZero(decimal value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
    
    public static void ForLessZero(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
    
    public static void ForLessZero(decimal value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    public static void ForNullOrEmpty(string value, string parameterName)
    {
        if (String.IsNullOrEmpty(value))
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
    
    public static void ForNullOrDefault(DateTime? value, string parameterName)
    {
        if (value == null || value == default(DateTime))
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
    
    public static void ForNullOrDefault(DateOnly? value, string parameterName)
    {
        if (value == null || value == default(DateOnly))
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
    
    public static void ForNull(object? value, string parameterName)
    {
        if (value == null)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}