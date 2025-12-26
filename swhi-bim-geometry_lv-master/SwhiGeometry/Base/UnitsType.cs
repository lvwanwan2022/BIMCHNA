using System;
using System.Collections.Generic;

namespace Lv.BIM.Geometry
{
  public static class UnitsType
    {
    public const string Millimeters = "mm";
    public const string Centimeters = "cm";
    public const string Meters = "m";
    public const string Kilometers = "km";
    public const string Inches = "in";
    public const string Feet = "ft"; // smelly ones
    public const string Yards = "yd";
    public const string Miles = "mi";
    public const string None = "none";

    private static List<string> SupportedUnits = new List<string>() { Millimeters, Centimeters, Meters, Kilometers, Inches, Feet, Yards, Miles, None };

    public static bool IsUnitSupported(string unit) => SupportedUnits.Contains(unit);

    // public const string USInches = "us_in"; the smelliest ones, can add later if people scream "USA #1"
    // public const string USFeet = "us_ft"; the smelliest ones, can add later if people scream "USA #1"
    // public const string USYards = "us_yd"; the smelliest ones, can add later if people scream "USA #1"
    // public const string USMiles = "us_mi"; the smelliest ones, can add later if people scream "USA #1"

    public static double GetConversionFactor(string from, string to)
    {
      from = GetUnitsFromString(from);
      to = GetUnitsFromString(to);

      switch (from)
      {
        // METRIC
        case UnitsType.Millimeters:
          switch (to)
          {
            case UnitsType.Centimeters:
              return 0.1;
            case UnitsType.Meters:
              return 0.001;
            case UnitsType.Kilometers:
              return 1e-6;
            case UnitsType.Inches:
              return 0.0393701;
            case UnitsType.Feet:
              return 0.00328084;
            case UnitsType.Yards:
              return 0.00109361;
            case UnitsType.Miles:
              return 6.21371e-7;
          }
          break;
        case UnitsType.Centimeters:
          switch (to)
          {
            case UnitsType.Millimeters:
              return 10;
            case UnitsType.Meters:
              return 0.01;
            case UnitsType.Kilometers:
              return 1e-5;
            case UnitsType.Inches:
              return 0.393701;
            case UnitsType.Feet:
              return 0.0328084;
            case UnitsType.Yards:
              return 0.0109361;
            case UnitsType.Miles:
              return 6.21371e-6;
          }
          break;
        case UnitsType.Meters:
          switch (to)
          {
            case UnitsType.Millimeters:
              return 1000;
            case UnitsType.Centimeters:
              return 100;
            case UnitsType.Kilometers:
              return 1000;
            case UnitsType.Inches:
              return 39.3701;
            case UnitsType.Feet:
              return 3.28084;
            case UnitsType.Yards:
              return 1.09361;
            case UnitsType.Miles:
              return 0.000621371;
          }
          break;
        case UnitsType.Kilometers:
          switch (to)
          {
            case UnitsType.Millimeters:
              return 1000000;
            case UnitsType.Centimeters:
              return 100000;
            case UnitsType.Meters:
              return 1000;
            case UnitsType.Inches:
              return 39370.1;
            case UnitsType.Feet:
              return 3280.84;
            case UnitsType.Yards:
              return 1093.61;
            case UnitsType.Miles:
              return 0.621371;
          }
          break;

        // IMPERIAL
        case UnitsType.Inches:
          switch (to)
          {
            case UnitsType.Millimeters:
              return 25.4;
            case UnitsType.Centimeters:
              return 2.54;
            case UnitsType.Meters:
              return 0.0254;
            case UnitsType.Kilometers:
              return 2.54e-5;
            case UnitsType.Feet:
              return 0.0833333;
            case UnitsType.Yards:
              return 0.027777694;
            case UnitsType.Miles:
              return 1.57828e-5;
          }
          break;
        case UnitsType.Feet:
          switch (to)
          {
            case UnitsType.Millimeters:
              return 304.8;
            case UnitsType.Centimeters:
              return 30.48;
            case UnitsType.Meters:
              return 0.3048;
            case UnitsType.Kilometers:
              return 0.0003048;
            case UnitsType.Inches:
              return 12;
            case UnitsType.Yards:
              return 0.333332328;
            case UnitsType.Miles:
              return 0.000189394;
          }
          break;
        case UnitsType.Miles:
          switch (to)
          {
            case UnitsType.Millimeters:
              return 1.609e+6;
            case UnitsType.Centimeters:
              return 160934;
            case UnitsType.Meters:
              return 1609.34;
            case UnitsType.Kilometers:
              return 1.60934;
            case UnitsType.Inches:
              return 63360;
            case UnitsType.Feet:
              return 5280;
            case UnitsType.Yards:
              return 1759.99469184;
          }
          break;
        case UnitsType.None:
          return 1;
      }
      return 1;
    }

    public static string GetUnitsFromString(string unit)
    {
      if (unit == null) return null;
      switch (unit.ToLower())
      {
        case "mm":
        case "mil":
        case "millimeter":
        case "millimeters":
        case "millimetres":
          return UnitsType.Millimeters;
        case "cm":
        case "centimetre":
        case "centimeter":
        case "centimetres":
        case "centimeters":
          return UnitsType.Centimeters;
        case "m":
        case "meter":
        case "metre":
        case "meters":
        case "metres":
          return UnitsType.Meters;
        case "inches":
        case "inch":
        case "in":
          return UnitsType.Inches;
        case "feet":
        case "foot":
        case "ft":
          return UnitsType.Feet;
        case "yard":
        case "yards":
        case "yd":
          return UnitsType.Yards;
        case "miles":
        case "mile":
        case "mi":
          return UnitsType.Miles;
        case "kilometers":
        case "kilometer":
        case "km":
          return UnitsType.Kilometers;
        case "none":
          return UnitsType.None;
      }

      throw new Exception($"Cannot understand what unit {unit} is.");
    }

    public static int GetEncodingFromUnit(string unit)
    {
      switch (unit)
      {
        case Millimeters: return 1;
        case Centimeters: return 2;
        case Meters: return 3;
        case Kilometers: return 4;
        case Inches: return 5;
        case Feet: return 6;
        case Yards: return 7;
        case Miles: return 8;
      }

      return 0;
    }

    public static string GetUnitFromEncoding(double unit)
    {
      switch (unit)
      {
        case 1: return Millimeters;
        case 2: return Centimeters;
        case 3: return Meters;
        case 4: return Kilometers;
        case 5: return Inches;
        case 6: return Feet;
        case 7: return Yards;
        case 8: return Miles;
      }

      return None;
    }
  }
}
