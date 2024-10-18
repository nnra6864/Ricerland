// Decompiled with JetBrains decompiler
// Type: System.Number
// Assembly: System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
// MVID: E22C0DD9-A217-4DDD-9F80-50156D17FADC
// Assembly location: /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/System.Private.CoreLib.dll

using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace System
{
  internal static class Number
  {
    private static readonly string[] s_smallNumberCache = new string[300];
    private static readonly string[] s_posCurrencyFormats = new string[4]
    {
      "$#",
      "#$",
      "$ #",
      "# $"
    };
    private static readonly string[] s_negCurrencyFormats = new string[17]
    {
      "($#)",
      "-$#",
      "$-#",
      "$#-",
      "(#$)",
      "-#$",
      "#-$",
      "#$-",
      "-# $",
      "-$ #",
      "# $-",
      "$ #-",
      "$ -#",
      "#- $",
      "($ #)",
      "(# $)",
      "$- #"
    };
    private static readonly string[] s_posPercentFormats = new string[4]
    {
      "# %",
      "#%",
      "%#",
      "% #"
    };
    private static readonly string[] s_negPercentFormats = new string[12]
    {
      "-# %",
      "-#%",
      "-%#",
      "%-#",
      "%#-",
      "#-%",
      "#%-",
      "-% #",
      "# %-",
      "% #-",
      "% -#",
      "#- %"
    };
    private static readonly string[] s_negNumberFormats = new string[5]
    {
      "(#)",
      "-#",
      "- #",
      "#-",
      "# -"
    };
    private static readonly byte[] TwoDigitsCharsAsBytes = MemoryMarshal.AsBytes<char>((ReadOnlySpan<char>) "00010203040506070809101112131415161718192021222324252627282930313233343536373839404142434445464748495051525354555657585960616263646566676869707172737475767778798081828384858687888990919293949596979899").ToArray();
    private static readonly unsafe byte[] TwoDigitsBytes = new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u003655761BC5B553103BD6B01577097EA28941852F328FFD28398C7ECA4763ADAAA, 200).ToArray();

    public static void Dragon4Double(
      double value,
      int cutoffNumber,
      bool isSignificantDigits,
      ref Number.NumberBuffer number)
    {
      double num = double.IsNegative(value) ? -value : value;
      int exponent;
      ulong andBiasedExponent = Number.ExtractFractionAndBiasedExponent(value, out exponent);
      bool hasUnequalMargins = false;
      uint mantissaHighBitIdx;
      if (andBiasedExponent >> 52 != 0UL)
      {
        mantissaHighBitIdx = 52U;
        hasUnequalMargins = andBiasedExponent == 4503599627370496UL;
      }
      else
        mantissaHighBitIdx = (uint) BitOperations.Log2(andBiasedExponent);
      int decimalExponent;
      int index = (int) Number.Dragon4(andBiasedExponent, exponent, mantissaHighBitIdx, hasUnequalMargins, cutoffNumber, isSignificantDigits, number.Digits, out decimalExponent);
      number.Scale = decimalExponent + 1;
      number.Digits[index] = (byte) 0;
      number.DigitsCount = index;
    }

    public static void Dragon4Half(
      Half value,
      int cutoffNumber,
      bool isSignificantDigits,
      ref Number.NumberBuffer number)
    {
      Half half = Half.IsNegative(value) ? Half.Negate(value) : value;
      int exponent;
      ushort andBiasedExponent = Number.ExtractFractionAndBiasedExponent(value, out exponent);
      bool hasUnequalMargins = false;
      uint mantissaHighBitIdx;
      if ((int) andBiasedExponent >> 10 != 0)
      {
        mantissaHighBitIdx = 10U;
        hasUnequalMargins = andBiasedExponent == (ushort) 1024;
      }
      else
        mantissaHighBitIdx = (uint) BitOperations.Log2((uint) andBiasedExponent);
      int decimalExponent;
      int index = (int) Number.Dragon4((ulong) andBiasedExponent, exponent, mantissaHighBitIdx, hasUnequalMargins, cutoffNumber, isSignificantDigits, number.Digits, out decimalExponent);
      number.Scale = decimalExponent + 1;
      number.Digits[index] = (byte) 0;
      number.DigitsCount = index;
    }

    public static void Dragon4Single(
      float value,
      int cutoffNumber,
      bool isSignificantDigits,
      ref Number.NumberBuffer number)
    {
      float num = float.IsNegative(value) ? -value : value;
      int exponent;
      uint andBiasedExponent = Number.ExtractFractionAndBiasedExponent(value, out exponent);
      bool hasUnequalMargins = false;
      uint mantissaHighBitIdx;
      if (andBiasedExponent >> 23 != 0U)
      {
        mantissaHighBitIdx = 23U;
        hasUnequalMargins = andBiasedExponent == 8388608U;
      }
      else
        mantissaHighBitIdx = (uint) BitOperations.Log2(andBiasedExponent);
      int decimalExponent;
      int index = (int) Number.Dragon4((ulong) andBiasedExponent, exponent, mantissaHighBitIdx, hasUnequalMargins, cutoffNumber, isSignificantDigits, number.Digits, out decimalExponent);
      number.Scale = decimalExponent + 1;
      number.Digits[index] = (byte) 0;
      number.DigitsCount = index;
    }

    private static unsafe uint Dragon4(
      ulong mantissa,
      int exponent,
      uint mantissaHighBitIdx,
      bool hasUnequalMargins,
      int cutoffNumber,
      bool isSignificantDigits,
      Span<byte> buffer,
      out int decimalExponent)
    {
      int index = 0;
      Number.BigInteger result1;
      Number.BigInteger result2;
      Number.BigInteger result3;
      Number.BigInteger* bigIntegerPtr;
      if (hasUnequalMargins)
      {
        Number.BigInteger result4;
        if (exponent > 0)
        {
          Number.BigInteger.SetUInt64(out result1, 4UL * mantissa);
          result1.ShiftLeft((uint) exponent);
          Number.BigInteger.SetUInt32(out result2, 4U);
          Number.BigInteger.Pow2((uint) exponent, out result3);
          Number.BigInteger.Pow2((uint) (exponent + 1), out result4);
        }
        else
        {
          Number.BigInteger.SetUInt64(out result1, 4UL * mantissa);
          Number.BigInteger.Pow2((uint) (-exponent + 2), out result2);
          Number.BigInteger.SetUInt32(out result3, 1U);
          Number.BigInteger.SetUInt32(out result4, 2U);
        }
        bigIntegerPtr = &result4;
      }
      else
      {
        if (exponent > 0)
        {
          Number.BigInteger.SetUInt64(out result1, 2UL * mantissa);
          result1.ShiftLeft((uint) exponent);
          Number.BigInteger.SetUInt32(out result2, 2U);
          Number.BigInteger.Pow2((uint) exponent, out result3);
        }
        else
        {
          Number.BigInteger.SetUInt64(out result1, 2UL * mantissa);
          Number.BigInteger.Pow2((uint) (-exponent + 1), out result2);
          Number.BigInteger.SetUInt32(out result3, 1U);
        }
        bigIntegerPtr = &result3;
      }
      int exponent1 = (int) Math.Ceiling((double) ((int) mantissaHighBitIdx + exponent) * 0.3010299956639812 - 0.69);
      if (exponent1 > 0)
        result2.MultiplyPow10((uint) exponent1);
      else if (exponent1 < 0)
      {
        Number.BigInteger result5;
        Number.BigInteger.Pow10((uint) -exponent1, out result5);
        result1.Multiply(ref result5);
        result3.Multiply(ref result5);
        if (bigIntegerPtr != &result3)
          Number.BigInteger.Multiply(ref result3, 2U, out *bigIntegerPtr);
      }
      bool flag1 = mantissa % 2UL == 0UL;
      bool flag2;
      if (cutoffNumber == -1)
      {
        Number.BigInteger result6;
        Number.BigInteger.Add(ref result1, ref *bigIntegerPtr, out result6);
        int num = Number.BigInteger.Compare(ref result6, ref result2);
        flag2 = flag1 ? num >= 0 : num > 0;
      }
      else
        flag2 = Number.BigInteger.Compare(ref result1, ref result2) >= 0;
      if (flag2)
      {
        ++exponent1;
      }
      else
      {
        result1.Multiply10();
        result3.Multiply10();
        if (bigIntegerPtr != &result3)
          Number.BigInteger.Multiply(ref result3, 2U, out *bigIntegerPtr);
      }
      int num1 = exponent1 - buffer.Length;
      if (cutoffNumber != -1)
      {
        int num2 = !isSignificantDigits ? -cutoffNumber : exponent1 - cutoffNumber;
        if (num2 > num1)
          num1 = num2;
      }
      int num3;
      decimalExponent = num3 = exponent1 - 1;
      uint block = result2.GetBlock((uint) (result2.GetLength() - 1));
      if (block < 8U || block > 429496729U)
      {
        uint shift = (59U - (uint) BitOperations.Log2(block)) % 32U;
        result2.ShiftLeft(shift);
        result1.ShiftLeft(shift);
        result3.ShiftLeft(shift);
        if (bigIntegerPtr != &result3)
          Number.BigInteger.Multiply(ref result3, 2U, out *bigIntegerPtr);
      }
      uint num4;
      bool flag3;
      bool flag4;
      if (cutoffNumber == -1)
      {
        while (true)
        {
          num4 = Number.BigInteger.HeuristicDivide(ref result1, ref result2);
          Number.BigInteger result7;
          Number.BigInteger.Add(ref result1, ref *bigIntegerPtr, out result7);
          int num5 = Number.BigInteger.Compare(ref result1, ref result3);
          int num6 = Number.BigInteger.Compare(ref result7, ref result2);
          if (flag1)
          {
            flag3 = num5 <= 0;
            flag4 = num6 >= 0;
          }
          else
          {
            flag3 = num5 < 0;
            flag4 = num6 > 0;
          }
          if (!(flag3 | flag4) && num3 != num1)
          {
            buffer[index] = (byte) (48U + num4);
            ++index;
            result1.Multiply10();
            result3.Multiply10();
            if (bigIntegerPtr != &result3)
              Number.BigInteger.Multiply(ref result3, 2U, out *bigIntegerPtr);
            --num3;
          }
          else
            break;
        }
      }
      else if (num3 >= num1)
      {
        flag3 = false;
        flag4 = false;
        while (true)
        {
          num4 = Number.BigInteger.HeuristicDivide(ref result1, ref result2);
          if (!result1.IsZero() && num3 > num1)
          {
            buffer[index] = (byte) (48U + num4);
            ++index;
            result1.Multiply10();
            --num3;
          }
          else
            break;
        }
      }
      else
      {
        uint num7 = Number.BigInteger.HeuristicDivide(ref result1, ref result2);
        switch (num7)
        {
          case 0:
          case 1:
          case 2:
          case 3:
          case 4:
            buffer[index] = (byte) (48U + num7);
            return (uint) (index + 1);
          case 5:
            if (result1.IsZero())
              goto case 0;
            else
              goto default;
          default:
            ++decimalExponent;
            num7 = 1U;
            goto case 0;
        }
      }
      bool flag5 = flag3;
      if (flag3 == flag4)
      {
        result1.ShiftLeft(1U);
        int num8 = Number.BigInteger.Compare(ref result1, ref result2);
        flag5 = num8 < 0;
        if (num8 == 0)
          flag5 = ((int) num4 & 1) == 0;
      }
      int num9;
      if (flag5)
      {
        buffer[index] = (byte) (48U + num4);
        num9 = index + 1;
      }
      else if (num4 == 9U)
      {
        while (index != 0)
        {
          --index;
          if (buffer[index] != (byte) 57)
          {
            ++buffer[index];
            num9 = index + 1;
            goto label_54;
          }
        }
        buffer[index] = (byte) 49;
        num9 = index + 1;
        ++decimalExponent;
      }
      else
      {
        buffer[index] = (byte) (48 + (int) num4 + 1);
        num9 = index + 1;
      }
label_54:
      return (uint) num9;
    }

    public static unsafe string FormatDecimal(
      Decimal value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info)
    {
      int digits1;
      char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
      byte* digits2 = stackalloc byte[31];
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Decimal, digits2, 31);
      Number.DecimalToNumber(ref value, ref number);
      char* pointer = stackalloc char[32];
      ValueListBuilder<char> vlb = new ValueListBuilder<char>(new Span<char>((void*) pointer, 32));
      if (formatSpecifier != char.MinValue)
        Number.NumberToString<char>(ref vlb, ref number, formatSpecifier, digits1, info);
      else
        Number.NumberToStringFormat<char>(ref vlb, ref number, format, info);
      string str = vlb.AsSpan().ToString();
      vlb.Dispose();
      return str;
    }

    public static unsafe bool TryFormatDecimal<TChar>(
      Decimal value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int digits1;
      char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
      byte* digits2 = stackalloc byte[31];
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Decimal, digits2, 31);
      Number.DecimalToNumber(ref value, ref number);
      TChar* pointer = stackalloc TChar[32];
      ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) pointer, 32));
      if (formatSpecifier != char.MinValue)
        Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, digits1, info);
      else
        Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, info);
      bool flag = vlb.TryCopyTo(destination, out charsWritten);
      vlb.Dispose();
      return flag;
    }

    internal static unsafe void DecimalToNumber([ScopedRef] ref Decimal d, ref Number.NumberBuffer number)
    {
      byte* digitsPointer1 = number.GetDigitsPointer();
      number.DigitsCount = 29;
      number.IsNegative = Decimal.IsNegative(d);
      byte* bufferEnd = digitsPointer1 + 29;
      while (((int) d.Mid | (int) d.High) != 0)
        bufferEnd = Number.UInt32ToDecChars<byte>(bufferEnd, Decimal.DecDivMod1E9(ref d), 9);
      byte* decChars = Number.UInt32ToDecChars<byte>(bufferEnd, d.Low, 0);
      int num = (int) (digitsPointer1 + 29 - decChars);
      number.DigitsCount = num;
      number.Scale = num - (int) d.Scale;
      byte* digitsPointer2 = number.GetDigitsPointer();
      while (--num >= 0)
        *digitsPointer2++ = *decChars++;
      *digitsPointer2 = (byte) 0;
    }

    public static string FormatDouble(double value, string format, NumberFormatInfo info)
    {
      ValueListBuilder<char> vlb = new ValueListBuilder<char>(stackalloc char[32]);
      string str = Number.FormatDouble<char>(ref vlb, value, (ReadOnlySpan<char>) format, info) ?? vlb.AsSpan().ToString();
      vlb.Dispose();
      return str;
    }

    public static unsafe bool TryFormatDouble<TChar>(
      double value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      // ISSUE: untyped stack allocation
      ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) __untypedstackalloc(checked (new IntPtr(32) * sizeof (TChar))), 32));
      string source = Number.FormatDouble<TChar>(ref vlb, value, format, info);
      bool flag = source != null ? Number.TryCopyTo<TChar>(source, destination, out charsWritten) : vlb.TryCopyTo(destination, out charsWritten);
      vlb.Dispose();
      return flag;
    }

    private static int GetFloatingPointMaxDigitsAndPrecision(
      char fmt,
      ref int precision,
      NumberFormatInfo info,
      out bool isSignificantDigits)
    {
      if (fmt == char.MinValue)
      {
        isSignificantDigits = true;
        return precision;
      }
      int digitsAndPrecision = precision;
      if (fmt <= 'R')
      {
        switch ((int) fmt - 67)
        {
          case 0:
            break;
          case 1:
            goto label_23;
          case 2:
            goto label_10;
          case 3:
            goto label_13;
          case 4:
            goto label_16;
          default:
            switch ((int) fmt - 78)
            {
              case 0:
                goto label_13;
              case 2:
                goto label_19;
              case 4:
                goto label_22;
              default:
                goto label_23;
            }
        }
      }
      else
      {
        switch ((int) fmt - 99)
        {
          case 0:
            break;
          case 1:
            goto label_23;
          case 2:
            goto label_10;
          case 3:
            goto label_13;
          case 4:
            goto label_16;
          default:
            switch ((int) fmt - 110)
            {
              case 0:
                goto label_13;
              case 2:
                goto label_19;
              case 4:
                goto label_22;
              default:
                goto label_23;
            }
        }
      }
      if (precision == -1)
        precision = info.CurrencyDecimalDigits;
      isSignificantDigits = false;
      goto label_24;
label_10:
      if (precision == -1)
        precision = 6;
      ++precision;
      isSignificantDigits = true;
      goto label_24;
label_13:
      if (precision == -1)
        precision = info.NumberDecimalDigits;
      isSignificantDigits = false;
      goto label_24;
label_16:
      if (precision == 0)
        precision = -1;
      isSignificantDigits = true;
      goto label_24;
label_19:
      if (precision == -1)
        precision = info.PercentDecimalDigits;
      precision += 2;
      isSignificantDigits = false;
      goto label_24;
label_22:
      precision = -1;
      isSignificantDigits = true;
      goto label_24;
label_23:
      ThrowHelper.ThrowFormatException_BadFormatSpecifier();
      goto label_22;
label_24:
      return digitsAndPrecision;
    }

    private static unsafe string FormatDouble<TChar>(
      ref ValueListBuilder<TChar> vlb,
      double value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (!double.IsFinite(value))
      {
        if (double.IsNaN(value))
        {
          if (typeof (TChar) == typeof (char))
            return info.NaNSymbol;
          vlb.Append(info.NaNSymbolTChar<TChar>());
          return (string) null;
        }
        if (typeof (TChar) == typeof (char))
          return !double.IsNegative(value) ? info.PositiveInfinitySymbol : info.NegativeInfinitySymbol;
        vlb.Append(double.IsNegative(value) ? info.NegativeInfinitySymbolTChar<TChar>() : info.PositiveInfinitySymbolTChar<TChar>());
        return (string) null;
      }
      int digits1;
      char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
      byte* digits2 = stackalloc byte[769];
      if (formatSpecifier == char.MinValue)
        digits1 = 15;
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, digits2, 769);
      number.IsNegative = double.IsNegative(value);
      bool isSignificantDigits;
      int nMaxDigits = Number.GetFloatingPointMaxDigitsAndPrecision(formatSpecifier, ref digits1, info, out isSignificantDigits);
      if (value != 0.0 && (!isSignificantDigits || !Number.Grisu3.TryRunDouble(value, digits1, ref number)))
        Number.Dragon4Double(value, digits1, isSignificantDigits, ref number);
      if (formatSpecifier != char.MinValue)
      {
        if (digits1 == -1)
          nMaxDigits = Math.Max(number.DigitsCount, 17);
        Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, nMaxDigits, info);
      }
      else
        Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, info);
      return (string) null;
    }

    public static string FormatSingle(float value, string format, NumberFormatInfo info)
    {
      ValueListBuilder<char> vlb = new ValueListBuilder<char>(stackalloc char[32]);
      string str = Number.FormatSingle<char>(ref vlb, value, (ReadOnlySpan<char>) format, info) ?? vlb.AsSpan().ToString();
      vlb.Dispose();
      return str;
    }

    public static unsafe bool TryFormatSingle<TChar>(
      float value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      // ISSUE: untyped stack allocation
      ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) __untypedstackalloc(checked (new IntPtr(32) * sizeof (TChar))), 32));
      string source = Number.FormatSingle<TChar>(ref vlb, value, format, info);
      bool flag = source != null ? Number.TryCopyTo<TChar>(source, destination, out charsWritten) : vlb.TryCopyTo(destination, out charsWritten);
      vlb.Dispose();
      return flag;
    }

    private static unsafe string FormatSingle<TChar>(
      ref ValueListBuilder<TChar> vlb,
      float value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (!float.IsFinite(value))
      {
        if (float.IsNaN(value))
        {
          if (typeof (TChar) == typeof (char))
            return info.NaNSymbol;
          vlb.Append(info.NaNSymbolTChar<TChar>());
          return (string) null;
        }
        if (typeof (TChar) == typeof (char))
          return !float.IsNegative(value) ? info.PositiveInfinitySymbol : info.NegativeInfinitySymbol;
        vlb.Append(float.IsNegative(value) ? info.NegativeInfinitySymbolTChar<TChar>() : info.PositiveInfinitySymbolTChar<TChar>());
        return (string) null;
      }
      int digits1;
      char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
      byte* digits2 = stackalloc byte[114];
      if (formatSpecifier == char.MinValue)
        digits1 = 7;
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, digits2, 114);
      number.IsNegative = float.IsNegative(value);
      bool isSignificantDigits;
      int nMaxDigits = Number.GetFloatingPointMaxDigitsAndPrecision(formatSpecifier, ref digits1, info, out isSignificantDigits);
      if ((double) value != 0.0 && (!isSignificantDigits || !Number.Grisu3.TryRunSingle(value, digits1, ref number)))
        Number.Dragon4Single(value, digits1, isSignificantDigits, ref number);
      if (formatSpecifier != char.MinValue)
      {
        if (digits1 == -1)
          nMaxDigits = Math.Max(number.DigitsCount, 9);
        Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, nMaxDigits, info);
      }
      else
        Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, info);
      return (string) null;
    }

    public static string FormatHalf(Half value, string format, NumberFormatInfo info)
    {
      ValueListBuilder<char> vlb = new ValueListBuilder<char>(stackalloc char[32]);
      string str = Number.FormatHalf<char>(ref vlb, value, (ReadOnlySpan<char>) format, info) ?? vlb.AsSpan().ToString();
      vlb.Dispose();
      return str;
    }

    private static unsafe string FormatHalf<TChar>(
      ref ValueListBuilder<TChar> vlb,
      Half value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (!Half.IsFinite(value))
      {
        if (Half.IsNaN(value))
        {
          if (typeof (TChar) == typeof (char))
            return info.NaNSymbol;
          vlb.Append(info.NaNSymbolTChar<TChar>());
          return (string) null;
        }
        if (typeof (TChar) == typeof (char))
          return !Half.IsNegative(value) ? info.PositiveInfinitySymbol : info.NegativeInfinitySymbol;
        vlb.Append(Half.IsNegative(value) ? info.NegativeInfinitySymbolTChar<TChar>() : info.PositiveInfinitySymbolTChar<TChar>());
        return (string) null;
      }
      int digits1;
      char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
      byte* digits2 = stackalloc byte[21];
      if (formatSpecifier == char.MinValue)
        digits1 = 5;
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, digits2, 21);
      number.IsNegative = Half.IsNegative(value);
      bool isSignificantDigits;
      int nMaxDigits = Number.GetFloatingPointMaxDigitsAndPrecision(formatSpecifier, ref digits1, info, out isSignificantDigits);
      if (value != new Half() && (!isSignificantDigits || !Number.Grisu3.TryRunHalf(value, digits1, ref number)))
        Number.Dragon4Half(value, digits1, isSignificantDigits, ref number);
      if (formatSpecifier != char.MinValue)
      {
        if (digits1 == -1)
          nMaxDigits = Math.Max(number.DigitsCount, 5);
        Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, nMaxDigits, info);
      }
      else
        Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, info);
      return (string) null;
    }

    public static unsafe bool TryFormatHalf<TChar>(
      Half value,
      ReadOnlySpan<char> format,
      NumberFormatInfo info,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      // ISSUE: untyped stack allocation
      ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) __untypedstackalloc(checked (new IntPtr(32) * sizeof (TChar))), 32));
      string source = Number.FormatHalf<TChar>(ref vlb, value, format, info);
      bool flag = source != null ? Number.TryCopyTo<TChar>(source, destination, out charsWritten) : vlb.TryCopyTo(destination, out charsWritten);
      vlb.Dispose();
      return flag;
    }

    private static bool TryCopyTo<TChar>(
      string source,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (!(typeof (TChar) == typeof (char)))
        return Encoding.UTF8.TryGetBytes((ReadOnlySpan<char>) source, MemoryMarshal.Cast<TChar, byte>(destination), out charsWritten);
      if (source.TryCopyTo(MemoryMarshal.Cast<TChar, char>(destination)))
      {
        charsWritten = source.Length;
        return true;
      }
      charsWritten = 0;
      return false;
    }

    internal static char GetHexBase(char fmt) => (char) ((uint) fmt - 33U);

    public static unsafe string FormatInt32(
      int value,
      int hexMask,
      string format,
      IFormatProvider provider)
    {
      if (!string.IsNullOrEmpty(format))
        return FormatInt32Slow(value, hexMask, format, provider);
      return value < 0 ? Number.NegativeInt32ToDecStr(value, -1, NumberFormatInfo.GetInstance(provider).NegativeSign) : Number.UInt32ToDecStr((uint) value);

      static unsafe string FormatInt32Slow(
        int value,
        int hexMask,
        string format,
        IFormatProvider provider)
      {
        ReadOnlySpan<char> format1 = (ReadOnlySpan<char>) format;
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format1, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return value < 0 ? Number.NegativeInt32ToDecStr(value, digits1, NumberFormatInfo.GetInstance(provider).NegativeSign) : Number.UInt32ToDecStr((uint) value, digits1);
        if (ch == 'X')
          return Number.Int32ToHexStr(value & hexMask, Number.GetHexBase(formatSpecifier), digits1);
        if (ch == 'B')
          return Number.UInt32ToBinaryStr((uint) (value & hexMask), digits1);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[11];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 11);
        Number.Int32ToNumber(value, ref number);
        char* pointer = stackalloc char[32];
        ValueListBuilder<char> vlb = new ValueListBuilder<char>(new Span<char>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<char>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<char>(ref vlb, ref number, format1, instance);
        string str = vlb.AsSpan().ToString();
        vlb.Dispose();
        return str;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool TryFormatInt32<TChar>(
      int value,
      int hexMask,
      ReadOnlySpan<char> format,
      IFormatProvider provider,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (format.Length != 0)
        return TryFormatInt32Slow(value, hexMask, format, provider, destination, out charsWritten);
      return value < 0 ? Number.TryNegativeInt32ToDecStr<TChar>(value, -1, NumberFormatInfo.GetInstance(provider).NegativeSignTChar<TChar>(), destination, out charsWritten) : Number.TryUInt32ToDecStr<TChar>((uint) value, destination, out charsWritten);

      static unsafe bool TryFormatInt32Slow(
        int value,
        int hexMask,
        ReadOnlySpan<char> format,
        IFormatProvider provider,
        Span<TChar> destination,
        out int charsWritten)
      {
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return value < 0 ? Number.TryNegativeInt32ToDecStr<TChar>(value, digits1, NumberFormatInfo.GetInstance(provider).NegativeSignTChar<TChar>(), destination, out charsWritten) : Number.TryUInt32ToDecStr<TChar>((uint) value, digits1, destination, out charsWritten);
        if (ch == 'X')
          return Number.TryInt32ToHexStr<TChar>(value & hexMask, Number.GetHexBase(formatSpecifier), digits1, destination, out charsWritten);
        if (ch == 'B')
          return Number.TryUInt32ToBinaryStr<TChar>((uint) (value & hexMask), digits1, destination, out charsWritten);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[11];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 11);
        Number.Int32ToNumber(value, ref number);
        TChar* pointer = stackalloc TChar[32];
        ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, instance);
        bool flag = vlb.TryCopyTo(destination, out charsWritten);
        vlb.Dispose();
        return flag;
      }
    }

    public static unsafe string FormatUInt32(uint value, string format, IFormatProvider provider)
    {
      return string.IsNullOrEmpty(format) ? Number.UInt32ToDecStr(value) : FormatUInt32Slow(value, format, provider);

      static unsafe string FormatUInt32Slow(uint value, string format, IFormatProvider provider)
      {
        ReadOnlySpan<char> format1 = (ReadOnlySpan<char>) format;
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format1, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return Number.UInt32ToDecStr(value, digits1);
        if (ch == 'X')
          return Number.Int32ToHexStr((int) value, Number.GetHexBase(formatSpecifier), digits1);
        if (ch == 'B')
          return Number.UInt32ToBinaryStr(value, digits1);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[11];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 11);
        Number.UInt32ToNumber(value, ref number);
        char* pointer = stackalloc char[32];
        ValueListBuilder<char> vlb = new ValueListBuilder<char>(new Span<char>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<char>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<char>(ref vlb, ref number, format1, instance);
        string str = vlb.AsSpan().ToString();
        vlb.Dispose();
        return str;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool TryFormatUInt32<TChar>(
      uint value,
      ReadOnlySpan<char> format,
      IFormatProvider provider,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      return format.Length == 0 ? Number.TryUInt32ToDecStr<TChar>(value, destination, out charsWritten) : TryFormatUInt32Slow(value, format, provider, destination, out charsWritten);

      static unsafe bool TryFormatUInt32Slow(
        uint value,
        ReadOnlySpan<char> format,
        IFormatProvider provider,
        Span<TChar> destination,
        out int charsWritten)
      {
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return Number.TryUInt32ToDecStr<TChar>(value, digits1, destination, out charsWritten);
        if (ch == 'X')
          return Number.TryInt32ToHexStr<TChar>((int) value, Number.GetHexBase(formatSpecifier), digits1, destination, out charsWritten);
        if (ch == 'B')
          return Number.TryUInt32ToBinaryStr<TChar>(value, digits1, destination, out charsWritten);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[11];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 11);
        Number.UInt32ToNumber(value, ref number);
        TChar* pointer = stackalloc TChar[32];
        ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, instance);
        bool flag = vlb.TryCopyTo(destination, out charsWritten);
        vlb.Dispose();
        return flag;
      }
    }

    public static unsafe string FormatInt64(long value, string format, IFormatProvider provider)
    {
      if (!string.IsNullOrEmpty(format))
        return FormatInt64Slow(value, format, provider);
      return value < 0L ? Number.NegativeInt64ToDecStr(value, -1, NumberFormatInfo.GetInstance(provider).NegativeSign) : Number.UInt64ToDecStr((ulong) value);

      static unsafe string FormatInt64Slow(long value, string format, IFormatProvider provider)
      {
        ReadOnlySpan<char> format1 = (ReadOnlySpan<char>) format;
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format1, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return value < 0L ? Number.NegativeInt64ToDecStr(value, digits1, NumberFormatInfo.GetInstance(provider).NegativeSign) : Number.UInt64ToDecStr((ulong) value, digits1);
        if (ch == 'X')
          return Number.Int64ToHexStr(value, Number.GetHexBase(formatSpecifier), digits1);
        if (ch == 'B')
          return Number.UInt64ToBinaryStr((ulong) value, digits1);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[20];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 20);
        Number.Int64ToNumber(value, ref number);
        char* pointer = stackalloc char[32];
        ValueListBuilder<char> vlb = new ValueListBuilder<char>(new Span<char>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<char>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<char>(ref vlb, ref number, format1, instance);
        string str = vlb.AsSpan().ToString();
        vlb.Dispose();
        return str;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool TryFormatInt64<TChar>(
      long value,
      ReadOnlySpan<char> format,
      IFormatProvider provider,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (format.Length != 0)
        return TryFormatInt64Slow(value, format, provider, destination, out charsWritten);
      return value < 0L ? Number.TryNegativeInt64ToDecStr<TChar>(value, -1, NumberFormatInfo.GetInstance(provider).NegativeSignTChar<TChar>(), destination, out charsWritten) : Number.TryUInt64ToDecStr<TChar>((ulong) value, destination, out charsWritten);

      static unsafe bool TryFormatInt64Slow(
        long value,
        ReadOnlySpan<char> format,
        IFormatProvider provider,
        Span<TChar> destination,
        out int charsWritten)
      {
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return value < 0L ? Number.TryNegativeInt64ToDecStr<TChar>(value, digits1, NumberFormatInfo.GetInstance(provider).NegativeSignTChar<TChar>(), destination, out charsWritten) : Number.TryUInt64ToDecStr<TChar>((ulong) value, digits1, destination, out charsWritten);
        if (ch == 'X')
          return Number.TryInt64ToHexStr<TChar>(value, Number.GetHexBase(formatSpecifier), digits1, destination, out charsWritten);
        if (ch == 'B')
          return Number.TryUInt64ToBinaryStr<TChar>((ulong) value, digits1, destination, out charsWritten);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[20];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 20);
        Number.Int64ToNumber(value, ref number);
        char* pointer = stackalloc char[32];
        ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, instance);
        bool flag = vlb.TryCopyTo(destination, out charsWritten);
        vlb.Dispose();
        return flag;
      }
    }

    public static unsafe string FormatUInt64(ulong value, string format, IFormatProvider provider)
    {
      return string.IsNullOrEmpty(format) ? Number.UInt64ToDecStr(value) : FormatUInt64Slow(value, format, provider);

      static unsafe string FormatUInt64Slow(ulong value, string format, IFormatProvider provider)
      {
        ReadOnlySpan<char> format1 = (ReadOnlySpan<char>) format;
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format1, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return Number.UInt64ToDecStr(value, digits1);
        if (ch == 'X')
          return Number.Int64ToHexStr((long) value, Number.GetHexBase(formatSpecifier), digits1);
        if (ch == 'B')
          return Number.UInt64ToBinaryStr(value, digits1);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[21];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 21);
        Number.UInt64ToNumber(value, ref number);
        char* pointer = stackalloc char[32];
        ValueListBuilder<char> vlb = new ValueListBuilder<char>(new Span<char>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<char>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<char>(ref vlb, ref number, format1, instance);
        string str = vlb.AsSpan().ToString();
        vlb.Dispose();
        return str;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool TryFormatUInt64<TChar>(
      ulong value,
      ReadOnlySpan<char> format,
      IFormatProvider provider,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      return format.Length == 0 ? Number.TryUInt64ToDecStr<TChar>(value, destination, out charsWritten) : TryFormatUInt64Slow(value, format, provider, destination, out charsWritten);

      static unsafe bool TryFormatUInt64Slow(
        ulong value,
        ReadOnlySpan<char> format,
        IFormatProvider provider,
        Span<TChar> destination,
        out int charsWritten)
      {
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return Number.TryUInt64ToDecStr<TChar>(value, digits1, destination, out charsWritten);
        if (ch == 'X')
          return Number.TryInt64ToHexStr<TChar>((long) value, Number.GetHexBase(formatSpecifier), digits1, destination, out charsWritten);
        if (ch == 'B')
          return Number.TryUInt64ToBinaryStr<TChar>(value, digits1, destination, out charsWritten);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[21];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 21);
        Number.UInt64ToNumber(value, ref number);
        TChar* pointer = stackalloc TChar[32];
        ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, instance);
        bool flag = vlb.TryCopyTo(destination, out charsWritten);
        vlb.Dispose();
        return flag;
      }
    }

    public static unsafe string FormatInt128(Int128 value, string format, IFormatProvider provider)
    {
      if (!string.IsNullOrEmpty(format))
        return FormatInt128Slow(value, format, provider);
      return !Int128.IsPositive(value) ? Number.NegativeInt128ToDecStr(value, -1, NumberFormatInfo.GetInstance(provider).NegativeSign) : Number.UInt128ToDecStr((UInt128) value, -1);

      static unsafe string FormatInt128Slow(Int128 value, string format, IFormatProvider provider)
      {
        ReadOnlySpan<char> format1 = (ReadOnlySpan<char>) format;
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format1, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return !Int128.IsPositive(value) ? Number.NegativeInt128ToDecStr(value, digits1, NumberFormatInfo.GetInstance(provider).NegativeSign) : Number.UInt128ToDecStr((UInt128) value, digits1);
        if (ch == 'X')
          return Number.Int128ToHexStr(value, Number.GetHexBase(formatSpecifier), digits1);
        if (ch == 'B')
          return Number.UInt128ToBinaryStr(value, digits1);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[40];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 40);
        Number.Int128ToNumber(value, ref number);
        char* pointer = stackalloc char[32];
        ValueListBuilder<char> vlb = new ValueListBuilder<char>(new Span<char>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<char>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<char>(ref vlb, ref number, format1, instance);
        string str = vlb.AsSpan().ToString();
        vlb.Dispose();
        return str;
      }
    }

    public static unsafe bool TryFormatInt128<TChar>(
      Int128 value,
      ReadOnlySpan<char> format,
      IFormatProvider provider,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (format.Length != 0)
        return TryFormatInt128Slow(value, format, provider, destination, out charsWritten);
      return !Int128.IsPositive(value) ? Number.TryNegativeInt128ToDecStr<TChar>(value, -1, NumberFormatInfo.GetInstance(provider).NegativeSignTChar<TChar>(), destination, out charsWritten) : Number.TryUInt128ToDecStr<TChar>((UInt128) value, -1, destination, out charsWritten);

      static unsafe bool TryFormatInt128Slow(
        Int128 value,
        ReadOnlySpan<char> format,
        IFormatProvider provider,
        Span<TChar> destination,
        out int charsWritten)
      {
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return !Int128.IsPositive(value) ? Number.TryNegativeInt128ToDecStr<TChar>(value, digits1, NumberFormatInfo.GetInstance(provider).NegativeSignTChar<TChar>(), destination, out charsWritten) : Number.TryUInt128ToDecStr<TChar>((UInt128) value, digits1, destination, out charsWritten);
        if (ch == 'X')
          return Number.TryInt128ToHexStr<TChar>(value, Number.GetHexBase(formatSpecifier), digits1, destination, out charsWritten);
        if (ch == 'B')
          return Number.TryUInt128ToBinaryStr<TChar>(value, digits1, destination, out charsWritten);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[40];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 40);
        Number.Int128ToNumber(value, ref number);
        TChar* pointer = stackalloc TChar[32];
        ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, instance);
        bool flag = vlb.TryCopyTo(destination, out charsWritten);
        vlb.Dispose();
        return flag;
      }
    }

    public static unsafe string FormatUInt128(
      UInt128 value,
      string format,
      IFormatProvider provider)
    {
      return string.IsNullOrEmpty(format) ? Number.UInt128ToDecStr(value, -1) : FormatUInt128Slow(value, format, provider);

      static unsafe string FormatUInt128Slow(
        UInt128 value,
        string format,
        IFormatProvider provider)
      {
        ReadOnlySpan<char> format1 = (ReadOnlySpan<char>) format;
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format1, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return Number.UInt128ToDecStr(value, digits1);
        if (ch == 'X')
          return Number.Int128ToHexStr((Int128) value, Number.GetHexBase(formatSpecifier), digits1);
        if (ch == 'B')
          return Number.UInt128ToBinaryStr((Int128) value, digits1);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[40];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 40);
        Number.UInt128ToNumber(value, ref number);
        char* pointer = stackalloc char[32];
        ValueListBuilder<char> vlb = new ValueListBuilder<char>(new Span<char>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<char>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<char>(ref vlb, ref number, format1, instance);
        string str = vlb.AsSpan().ToString();
        vlb.Dispose();
        return str;
      }
    }

    public static unsafe bool TryFormatUInt128<TChar>(
      UInt128 value,
      ReadOnlySpan<char> format,
      IFormatProvider provider,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      return format.Length == 0 ? Number.TryUInt128ToDecStr<TChar>(value, -1, destination, out charsWritten) : TryFormatUInt128Slow(value, format, provider, destination, out charsWritten);

      static unsafe bool TryFormatUInt128Slow(
        UInt128 value,
        ReadOnlySpan<char> format,
        IFormatProvider provider,
        Span<TChar> destination,
        out int charsWritten)
      {
        int digits1;
        char formatSpecifier = Number.ParseFormatSpecifier(format, out digits1);
        char ch = (char) ((uint) formatSpecifier & 65503U);
        if ((ch == 'G' ? (digits1 < 1 ? 1 : 0) : (ch == 'D' ? 1 : 0)) != 0)
          return Number.TryUInt128ToDecStr<TChar>(value, digits1, destination, out charsWritten);
        if (ch == 'X')
          return Number.TryInt128ToHexStr<TChar>((Int128) value, Number.GetHexBase(formatSpecifier), digits1, destination, out charsWritten);
        if (ch == 'B')
          return Number.TryUInt128ToBinaryStr<TChar>((Int128) value, digits1, destination, out charsWritten);
        NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
        byte* digits2 = stackalloc byte[40];
        Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits2, 40);
        Number.UInt128ToNumber(value, ref number);
        TChar* pointer = stackalloc TChar[32];
        ValueListBuilder<TChar> vlb = new ValueListBuilder<TChar>(new Span<TChar>((void*) pointer, 32));
        if (formatSpecifier != char.MinValue)
          Number.NumberToString<TChar>(ref vlb, ref number, formatSpecifier, digits1, instance);
        else
          Number.NumberToStringFormat<TChar>(ref vlb, ref number, format, instance);
        bool flag = vlb.TryCopyTo(destination, out charsWritten);
        vlb.Dispose();
        return flag;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void Int32ToNumber(int value, ref Number.NumberBuffer number)
    {
      number.DigitsCount = 10;
      if (value >= 0)
      {
        number.IsNegative = false;
      }
      else
      {
        number.IsNegative = true;
        value = -value;
      }
      byte* digitsPointer1 = number.GetDigitsPointer();
      byte* decChars = Number.UInt32ToDecChars<byte>(digitsPointer1 + 10, (uint) value, 0);
      int num = (int) (digitsPointer1 + 10 - decChars);
      number.DigitsCount = num;
      number.Scale = num;
      byte* digitsPointer2 = number.GetDigitsPointer();
      while (--num >= 0)
        *digitsPointer2++ = *decChars++;
      *digitsPointer2 = (byte) 0;
    }

    public static string Int32ToDecStr(int value)
    {
      return value < 0 ? Number.NegativeInt32ToDecStr(value, -1, NumberFormatInfo.CurrentInfo.NegativeSign) : Number.UInt32ToDecStr((uint) value);
    }

    private static unsafe string NegativeInt32ToDecStr(int value, int digits, string sNegative)
    {
      if (digits < 1)
        digits = 1;
      int length = Math.Max(digits, FormattingHelpers.CountDigits((uint) -value)) + sNegative.Length;
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      char* decChars = Number.UInt32ToDecChars<char>((char*) (num + (IntPtr) length * 2), (uint) -value, digits);
      for (int index = sNegative.Length - 1; index >= 0; --index)
        *--decChars = sNegative[index];
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    internal static unsafe bool TryNegativeInt32ToDecStr<TChar>(
      int value,
      int digits,
      ReadOnlySpan<TChar> sNegative,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      int num = Math.Max(digits, FormattingHelpers.CountDigits((uint) -value)) + sNegative.Length;
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
      {
        TChar* decChars = Number.UInt32ToDecChars<TChar>(charPtr + num, (uint) -value, digits);
        for (int index = sNegative.Length - 1; index >= 0; --index)
          *--decChars = sNegative[index];
      }
      return true;
    }

    private static unsafe string Int32ToHexStr(int value, char hexBase, int digits)
    {
      if (digits < 1)
        digits = 1;
      int length = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong) (uint) value));
      string hexStr = string.FastAllocateString(length);
      IntPtr num;
      if (hexStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &hexStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.Int32ToHexChars<char>((char*) (num + (IntPtr) length * 2), (uint) value, (int) hexBase, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return hexStr;
    }

    internal static unsafe bool TryInt32ToHexStr<TChar>(
      int value,
      char hexBase,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong) (uint) value));
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
        Number.Int32ToHexChars<TChar>(charPtr + num, (uint) value, (int) hexBase, digits);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe TChar* Int32ToHexChars<TChar>(
      TChar* buffer,
      uint value,
      int hexBase,
      int digits)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      for (; --digits >= 0 || value != 0U; value >>= 4)
      {
        byte num = (byte) (value & 15U);
        *--buffer = TChar.CastFrom((int) num + (num < (byte) 10 ? 48 : hexBase));
      }
      return buffer;
    }

    private static unsafe string UInt32ToBinaryStr(uint value, int digits)
    {
      if (digits < 1)
        digits = 1;
      int length = Math.Max(digits, 32 - (int) uint.LeadingZeroCount(value));
      string binaryStr = string.FastAllocateString(length);
      IntPtr num;
      if (binaryStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &binaryStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt32ToBinaryChars<char>((char*) (num + (IntPtr) length * 2), value, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return binaryStr;
    }

    private static unsafe bool TryUInt32ToBinaryStr<TChar>(
      uint value,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      int num = Math.Max(digits, 32 - (int) uint.LeadingZeroCount(value));
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
        Number.UInt32ToBinaryChars<TChar>(charPtr + num, value, digits);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe TChar* UInt32ToBinaryChars<TChar>(TChar* buffer, uint value, int digits) where TChar : unmanaged, IUtfChar<TChar>
    {
      for (; --digits >= 0 || value != 0U; value >>= 1)
        *--buffer = TChar.CastFrom(48 + (int) (byte) (value & 1U));
      return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void UInt32ToNumber(uint value, ref Number.NumberBuffer number)
    {
      number.DigitsCount = 10;
      number.IsNegative = false;
      byte* digitsPointer1 = number.GetDigitsPointer();
      byte* decChars = Number.UInt32ToDecChars<byte>(digitsPointer1 + 10, value, 0);
      int num = (int) (digitsPointer1 + 10 - decChars);
      number.DigitsCount = num;
      number.Scale = num;
      byte* digitsPointer2 = number.GetDigitsPointer();
      while (--num >= 0)
        *digitsPointer2++ = *decChars++;
      *digitsPointer2 = (byte) 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void WriteTwoDigits<TChar>(uint value, TChar* ptr) where TChar : unmanaged, IUtfChar<TChar>
    {
      Unsafe.CopyBlockUnaligned(ref *(byte*) ptr, ref Unsafe.Add<byte>(ref MemoryMarshal.GetArrayDataReference<byte>(typeof (TChar) == typeof (char) ? Number.TwoDigitsCharsAsBytes : Number.TwoDigitsBytes), (UIntPtr) ((uint) (sizeof (TChar) * 2) * value)), (uint) (sizeof (TChar) * 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void WriteFourDigits<TChar>(uint value, TChar* ptr) where TChar : unmanaged, IUtfChar<TChar>
    {
      uint Remainder;
      (value, Remainder) = Math.DivRem(value, 100U);
      ref byte local = ref MemoryMarshal.GetArrayDataReference<byte>(typeof (TChar) == typeof (char) ? Number.TwoDigitsCharsAsBytes : Number.TwoDigitsBytes);
      Unsafe.CopyBlockUnaligned(ref *(byte*) ptr, ref Unsafe.Add<byte>(ref local, (UIntPtr) ((uint) (sizeof (TChar) * 2) * value)), (uint) (sizeof (TChar) * 2));
      Unsafe.CopyBlockUnaligned(ref *(byte*) (ptr + 2), ref Unsafe.Add<byte>(ref local, (UIntPtr) ((uint) (sizeof (TChar) * 2) * Remainder)), (uint) (sizeof (TChar) * 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void WriteDigits<TChar>(uint value, TChar* ptr, int count) where TChar : unmanaged, IUtfChar<TChar>
    {
      TChar* charPtr;
      for (charPtr = ptr + count - 1; charPtr > ptr; --charPtr)
      {
        uint num = 48U + value;
        value /= 10U;
        *charPtr = TChar.CastFrom(num - value * 10U);
      }
      *charPtr = TChar.CastFrom(48U + value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe TChar* UInt32ToDecChars<TChar>(TChar* bufferEnd, uint value) where TChar : unmanaged, IUtfChar<TChar>
    {
      if (value >= 10U)
      {
        while (value >= 100U)
        {
          bufferEnd -= 2;
          (uint Quotient, uint Remainder) tuple = Math.DivRem(value, 100U);
          value = tuple.Quotient;
          Number.WriteTwoDigits<TChar>(tuple.Remainder, bufferEnd);
        }
        if (value >= 10U)
        {
          bufferEnd -= 2;
          Number.WriteTwoDigits<TChar>(value, bufferEnd);
          return bufferEnd;
        }
      }
      *--bufferEnd = TChar.CastFrom(value + 48U);
      return bufferEnd;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe TChar* UInt32ToDecChars<TChar>(TChar* bufferEnd, uint value, int digits) where TChar : unmanaged, IUtfChar<TChar>
    {
      while (value >= 100U)
      {
        bufferEnd -= 2;
        digits -= 2;
        (uint Quotient, uint Remainder) tuple = Math.DivRem(value, 100U);
        value = tuple.Quotient;
        Number.WriteTwoDigits<TChar>(tuple.Remainder, bufferEnd);
      }
      while (value != 0U || digits > 0)
      {
        --digits;
        uint Remainder;
        (value, Remainder) = Math.DivRem(value, 10U);
        *--bufferEnd = TChar.CastFrom(Remainder + 48U);
      }
      return bufferEnd;
    }

    internal static string UInt32ToDecStr(uint value)
    {
      return value < 300U ? Number.UInt32ToDecStrForKnownSmallNumber(value) : Number.UInt32ToDecStr_NoSmallNumberCheck(value);
    }

    internal static string UInt32ToDecStrForKnownSmallNumber(uint value)
    {
      return Number.s_smallNumberCache[(int) value] ?? CreateAndCacheString(value);

      [MethodImpl(MethodImplOptions.NoInlining)]
      static string CreateAndCacheString(uint value)
      {
        return Number.s_smallNumberCache[(int) value] = Number.UInt32ToDecStr_NoSmallNumberCheck(value);
      }
    }

    private static unsafe string UInt32ToDecStr_NoSmallNumberCheck(uint value)
    {
      int length = FormattingHelpers.CountDigits(value);
      string smallNumberCheck = string.FastAllocateString(length);
      IntPtr num;
      if (smallNumberCheck == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &smallNumberCheck.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt32ToDecChars<char>((char*) (num + (IntPtr) length * 2), value);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return smallNumberCheck;
    }

    private static unsafe string UInt32ToDecStr(uint value, int digits)
    {
      if (digits <= 1)
        return Number.UInt32ToDecStr(value);
      int length = Math.Max(digits, FormattingHelpers.CountDigits(value));
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt32ToDecChars<char>((char*) (num + (IntPtr) length * 2), value, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    internal static unsafe bool TryUInt32ToDecStr<TChar>(
      uint value,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int num = FormattingHelpers.CountDigits(value);
      if (num <= destination.Length)
      {
        charsWritten = num;
        fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
          Number.UInt32ToDecChars<TChar>(charPtr + num, value);
        return true;
      }
      charsWritten = 0;
      return false;
    }

    internal static unsafe bool TryUInt32ToDecStr<TChar>(
      uint value,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int val2 = FormattingHelpers.CountDigits(value);
      int num = Math.Max(digits, val2);
      if (num <= destination.Length)
      {
        charsWritten = num;
        fixed (TChar* charPtr1 = &MemoryMarshal.GetReference<TChar>(destination))
        {
          TChar* bufferEnd = charPtr1 + num;
          TChar* charPtr2 = digits > val2 ? Number.UInt32ToDecChars<TChar>(bufferEnd, value, digits) : Number.UInt32ToDecChars<TChar>(bufferEnd, value);
        }
        return true;
      }
      charsWritten = 0;
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void Int64ToNumber(long value, ref Number.NumberBuffer number)
    {
      number.DigitsCount = 19;
      if (value >= 0L)
      {
        number.IsNegative = false;
      }
      else
      {
        number.IsNegative = true;
        value = -value;
      }
      byte* digitsPointer1 = number.GetDigitsPointer();
      byte* decChars = Number.UInt64ToDecChars<byte>(digitsPointer1 + 19, (ulong) value, 0);
      int num = (int) (digitsPointer1 + 19 - decChars);
      number.DigitsCount = num;
      number.Scale = num;
      byte* digitsPointer2 = number.GetDigitsPointer();
      while (--num >= 0)
        *digitsPointer2++ = *decChars++;
      *digitsPointer2 = (byte) 0;
    }

    public static string Int64ToDecStr(long value)
    {
      return value < 0L ? Number.NegativeInt64ToDecStr(value, -1, NumberFormatInfo.CurrentInfo.NegativeSign) : Number.UInt64ToDecStr((ulong) value);
    }

    private static unsafe string NegativeInt64ToDecStr(long value, int digits, string sNegative)
    {
      if (digits < 1)
        digits = 1;
      int length = Math.Max(digits, FormattingHelpers.CountDigits((ulong) -value)) + sNegative.Length;
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      char* decChars = Number.UInt64ToDecChars<char>((char*) (num + (IntPtr) length * 2), (ulong) -value, digits);
      for (int index = sNegative.Length - 1; index >= 0; --index)
        *--decChars = sNegative[index];
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    internal static unsafe bool TryNegativeInt64ToDecStr<TChar>(
      long value,
      int digits,
      ReadOnlySpan<TChar> sNegative,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      int num = Math.Max(digits, FormattingHelpers.CountDigits((ulong) -value)) + sNegative.Length;
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
      {
        TChar* decChars = Number.UInt64ToDecChars<TChar>(charPtr + num, (ulong) -value, digits);
        for (int index = sNegative.Length - 1; index >= 0; --index)
          *--decChars = sNegative[index];
      }
      return true;
    }

    private static unsafe string Int64ToHexStr(long value, char hexBase, int digits)
    {
      if (digits < 1)
        digits = 1;
      int length = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong) value));
      string hexStr = string.FastAllocateString(length);
      IntPtr num;
      if (hexStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &hexStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.Int64ToHexChars<char>((char*) (num + (IntPtr) length * 2), (ulong) value, (int) hexBase, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return hexStr;
    }

    internal static unsafe bool TryInt64ToHexStr<TChar>(
      long value,
      char hexBase,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong) value));
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
        Number.Int64ToHexChars<TChar>(charPtr + num, (ulong) value, (int) hexBase, digits);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe TChar* Int64ToHexChars<TChar>(
      TChar* buffer,
      ulong value,
      int hexBase,
      int digits)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      for (; --digits >= 0 || value != 0UL; value >>= 4)
      {
        byte num = (byte) (value & 15UL);
        *--buffer = TChar.CastFrom((int) num + (num < (byte) 10 ? 48 : hexBase));
      }
      return buffer;
    }

    private static unsafe string UInt64ToBinaryStr(ulong value, int digits)
    {
      if (digits < 1)
        digits = 1;
      int length = Math.Max(digits, 64 - (int) ulong.LeadingZeroCount(value));
      string binaryStr = string.FastAllocateString(length);
      IntPtr num;
      if (binaryStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &binaryStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt64ToBinaryChars<char>((char*) (num + (IntPtr) length * 2), value, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return binaryStr;
    }

    private static unsafe bool TryUInt64ToBinaryStr<TChar>(
      ulong value,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      int num = Math.Max(digits, 64 - (int) ulong.LeadingZeroCount(value));
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
        Number.UInt64ToBinaryChars<TChar>(charPtr + num, value, digits);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe TChar* UInt64ToBinaryChars<TChar>(TChar* buffer, ulong value, int digits) where TChar : unmanaged, IUtfChar<TChar>
    {
      for (; --digits >= 0 || value != 0UL; value >>= 1)
        *--buffer = TChar.CastFrom(48 + (int) (byte) (value & 1UL));
      return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void UInt64ToNumber(ulong value, ref Number.NumberBuffer number)
    {
      number.DigitsCount = 20;
      number.IsNegative = false;
      byte* digitsPointer1 = number.GetDigitsPointer();
      byte* decChars = Number.UInt64ToDecChars<byte>(digitsPointer1 + 20, value, 0);
      int num = (int) (digitsPointer1 + 20 - decChars);
      number.DigitsCount = num;
      number.Scale = num;
      byte* digitsPointer2 = number.GetDigitsPointer();
      while (--num >= 0)
        *digitsPointer2++ = *decChars++;
      *digitsPointer2 = (byte) 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe TChar* UInt64ToDecChars<TChar>(TChar* bufferEnd, ulong value) where TChar : unmanaged, IUtfChar<TChar>
    {
      if (value >= 10UL)
      {
        while (value >= 100UL)
        {
          bufferEnd -= 2;
          (ulong Quotient, ulong Remainder) tuple = Math.DivRem(value, 100UL);
          value = tuple.Quotient;
          Number.WriteTwoDigits<TChar>((uint) tuple.Remainder, bufferEnd);
        }
        if (value >= 10UL)
        {
          bufferEnd -= 2;
          Number.WriteTwoDigits<TChar>((uint) value, bufferEnd);
          return bufferEnd;
        }
      }
      *--bufferEnd = TChar.CastFrom(value + 48UL);
      return bufferEnd;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe TChar* UInt64ToDecChars<TChar>(
      TChar* bufferEnd,
      ulong value,
      int digits)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      while (value >= 100UL)
      {
        bufferEnd -= 2;
        digits -= 2;
        (ulong Quotient, ulong Remainder) tuple = Math.DivRem(value, 100UL);
        value = tuple.Quotient;
        Number.WriteTwoDigits<TChar>((uint) tuple.Remainder, bufferEnd);
      }
      while (value != 0UL || digits > 0)
      {
        --digits;
        ulong Remainder;
        (value, Remainder) = Math.DivRem(value, 10UL);
        *--bufferEnd = TChar.CastFrom(Remainder + 48UL);
      }
      return bufferEnd;
    }

    internal static unsafe string UInt64ToDecStr(ulong value)
    {
      if (value < 300UL)
        return Number.UInt32ToDecStrForKnownSmallNumber((uint) value);
      int length = FormattingHelpers.CountDigits(value);
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt64ToDecChars<char>((char*) (num + (IntPtr) length * 2), value);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    internal static unsafe string UInt64ToDecStr(ulong value, int digits)
    {
      if (digits <= 1)
        return Number.UInt64ToDecStr(value);
      int length = Math.Max(digits, FormattingHelpers.CountDigits(value));
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt64ToDecChars<char>((char*) (num + (IntPtr) length * 2), value, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    internal static unsafe bool TryUInt64ToDecStr<TChar>(
      ulong value,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int num = FormattingHelpers.CountDigits(value);
      if (num <= destination.Length)
      {
        charsWritten = num;
        fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
          Number.UInt64ToDecChars<TChar>(charPtr + num, value);
        return true;
      }
      charsWritten = 0;
      return false;
    }

    internal static unsafe bool TryUInt64ToDecStr<TChar>(
      ulong value,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int val2 = FormattingHelpers.CountDigits(value);
      int num = Math.Max(digits, val2);
      if (num <= destination.Length)
      {
        charsWritten = num;
        fixed (TChar* charPtr1 = &MemoryMarshal.GetReference<TChar>(destination))
        {
          TChar* bufferEnd = charPtr1 + num;
          TChar* charPtr2 = digits > val2 ? Number.UInt64ToDecChars<TChar>(bufferEnd, value, digits) : Number.UInt64ToDecChars<TChar>(bufferEnd, value);
        }
        return true;
      }
      charsWritten = 0;
      return false;
    }

    private static unsafe void Int128ToNumber(Int128 value, ref Number.NumberBuffer number)
    {
      number.DigitsCount = 39;
      if (Int128.IsPositive(value))
      {
        number.IsNegative = false;
      }
      else
      {
        number.IsNegative = true;
        value = -value;
      }
      byte* digitsPointer1 = number.GetDigitsPointer();
      byte* decChars = Number.UInt128ToDecChars<byte>(digitsPointer1 + 39, (UInt128) value, 0);
      int num = (int) (digitsPointer1 + 39 - decChars);
      number.DigitsCount = num;
      number.Scale = num;
      byte* digitsPointer2 = number.GetDigitsPointer();
      while (--num >= 0)
        *digitsPointer2++ = *decChars++;
      *digitsPointer2 = (byte) 0;
    }

    public static string Int128ToDecStr(Int128 value)
    {
      return !Int128.IsPositive(value) ? Number.NegativeInt128ToDecStr(value, -1, NumberFormatInfo.CurrentInfo.NegativeSign) : Number.UInt128ToDecStr((UInt128) value, -1);
    }

    private static unsafe string NegativeInt128ToDecStr(Int128 value, int digits, string sNegative)
    {
      if (digits < 1)
        digits = 1;
      UInt128 uint128 = (UInt128) -value;
      int length = Math.Max(digits, FormattingHelpers.CountDigits(uint128)) + sNegative.Length;
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      char* decChars = Number.UInt128ToDecChars<char>((char*) (num + (IntPtr) length * 2), uint128, digits);
      for (int index = sNegative.Length - 1; index >= 0; --index)
        *--decChars = sNegative[index];
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    private static unsafe bool TryNegativeInt128ToDecStr<TChar>(
      Int128 value,
      int digits,
      ReadOnlySpan<TChar> sNegative,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      UInt128 uint128 = (UInt128) -value;
      int num = Math.Max(digits, FormattingHelpers.CountDigits(uint128)) + sNegative.Length;
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
      {
        TChar* decChars = Number.UInt128ToDecChars<TChar>(charPtr + num, uint128, digits);
        for (int index = sNegative.Length - 1; index >= 0; --index)
          *--decChars = sNegative[index];
      }
      return true;
    }

    private static unsafe string Int128ToHexStr(Int128 value, char hexBase, int digits)
    {
      if (digits < 1)
        digits = 1;
      UInt128 uint128 = (UInt128) value;
      int length = Math.Max(digits, FormattingHelpers.CountHexDigits(uint128));
      string hexStr = string.FastAllocateString(length);
      IntPtr num;
      if (hexStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &hexStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.Int128ToHexChars<char>((char*) (num + (IntPtr) length * 2), uint128, (int) hexBase, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return hexStr;
    }

    private static unsafe bool TryInt128ToHexStr<TChar>(
      Int128 value,
      char hexBase,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      UInt128 uint128 = (UInt128) value;
      int num = Math.Max(digits, FormattingHelpers.CountHexDigits(uint128));
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
        Number.Int128ToHexChars<TChar>(charPtr + num, uint128, (int) hexBase, digits);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe TChar* Int128ToHexChars<TChar>(
      TChar* buffer,
      UInt128 value,
      int hexBase,
      int digits)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      ulong lower = value.Lower;
      ulong upper = value.Upper;
      if (upper == 0UL)
        return Number.Int64ToHexChars<TChar>(buffer, lower, hexBase, Math.Max(digits, 1));
      buffer = Number.Int64ToHexChars<TChar>(buffer, lower, hexBase, 16);
      return Number.Int64ToHexChars<TChar>(buffer, upper, hexBase, digits - 16);
    }

    private static unsafe string UInt128ToBinaryStr(Int128 value, int digits)
    {
      if (digits < 1)
        digits = 1;
      UInt128 uint128 = (UInt128) value;
      int length = Math.Max(digits, 128 - (int) UInt128.LeadingZeroCount((UInt128) value));
      string binaryStr = string.FastAllocateString(length);
      IntPtr num;
      if (binaryStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &binaryStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt128ToBinaryChars<char>((char*) (num + (IntPtr) length * 2), uint128, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return binaryStr;
    }

    private static unsafe bool TryUInt128ToBinaryStr<TChar>(
      Int128 value,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      if (digits < 1)
        digits = 1;
      UInt128 uint128 = (UInt128) value;
      int num = Math.Max(digits, 128 - (int) UInt128.LeadingZeroCount((UInt128) value));
      if (num > destination.Length)
      {
        charsWritten = 0;
        return false;
      }
      charsWritten = num;
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(destination))
        Number.UInt128ToBinaryChars<TChar>(charPtr + num, uint128, digits);
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe TChar* UInt128ToBinaryChars<TChar>(
      TChar* buffer,
      UInt128 value,
      int digits)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      ulong lower = value.Lower;
      ulong upper = value.Upper;
      if (upper == 0UL)
        return Number.UInt64ToBinaryChars<TChar>(buffer, lower, Math.Max(digits, 1));
      buffer = Number.UInt64ToBinaryChars<TChar>(buffer, lower, 64);
      return Number.UInt64ToBinaryChars<TChar>(buffer, upper, digits - 64);
    }

    private static unsafe void UInt128ToNumber(UInt128 value, ref Number.NumberBuffer number)
    {
      number.DigitsCount = 39;
      number.IsNegative = false;
      byte* digitsPointer1 = number.GetDigitsPointer();
      byte* decChars = Number.UInt128ToDecChars<byte>(digitsPointer1 + 39, value, 0);
      int num = (int) (digitsPointer1 + 39 - decChars);
      number.DigitsCount = num;
      number.Scale = num;
      byte* digitsPointer2 = number.GetDigitsPointer();
      while (--num >= 0)
        *digitsPointer2++ = *decChars++;
      *digitsPointer2 = (byte) 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Int128DivMod1E19(ref UInt128 value)
    {
      UInt128 right = new UInt128(0UL, 10000000000000000000UL);
      (UInt128 Quotient, UInt128 Remainder) tuple = UInt128.DivRem(value, right);
      value = tuple.Quotient;
      return tuple.Remainder.Lower;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe TChar* UInt128ToDecChars<TChar>(TChar* bufferEnd, UInt128 value) where TChar : unmanaged, IUtfChar<TChar>
    {
      while (value.Upper != 0UL)
        bufferEnd = Number.UInt64ToDecChars<TChar>(bufferEnd, Number.Int128DivMod1E19(ref value), 19);
      return Number.UInt64ToDecChars<TChar>(bufferEnd, value.Lower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe TChar* UInt128ToDecChars<TChar>(
      TChar* bufferEnd,
      UInt128 value,
      int digits)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      while (value.Upper != 0UL)
      {
        bufferEnd = Number.UInt64ToDecChars<TChar>(bufferEnd, Number.Int128DivMod1E19(ref value), 19);
        digits -= 19;
      }
      return Number.UInt64ToDecChars<TChar>(bufferEnd, value.Lower, digits);
    }

    internal static unsafe string UInt128ToDecStr(UInt128 value)
    {
      if (value.Upper == 0UL)
        return Number.UInt64ToDecStr(value.Lower);
      int length = FormattingHelpers.CountDigits(value);
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt128ToDecChars<char>((char*) (num + (IntPtr) length * 2), value);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    internal static unsafe string UInt128ToDecStr(UInt128 value, int digits)
    {
      if (digits <= 1)
        return Number.UInt128ToDecStr(value);
      int length = Math.Max(digits, FormattingHelpers.CountDigits(value));
      string decStr = string.FastAllocateString(length);
      IntPtr num;
      if (decStr == null)
      {
        num = IntPtr.Zero;
      }
      else
      {
        fixed (char* chPtr = &decStr.GetPinnableReference())
          num = (IntPtr) chPtr;
      }
      Number.UInt128ToDecChars<char>((char*) (num + (IntPtr) length * 2), value, digits);
      // ISSUE: fixed variable is out of scope
      // ISSUE: __unpin statement
      __unpin(chPtr);
      return decStr;
    }

    private static unsafe bool TryUInt128ToDecStr<TChar>(
      UInt128 value,
      int digits,
      Span<TChar> destination,
      out int charsWritten)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int val2 = FormattingHelpers.CountDigits(value);
      int num = Math.Max(digits, val2);
      if (num <= destination.Length)
      {
        charsWritten = num;
        fixed (TChar* charPtr1 = &MemoryMarshal.GetReference<TChar>(destination))
        {
          TChar* bufferEnd = charPtr1 + num;
          TChar* charPtr2 = digits > val2 ? Number.UInt128ToDecChars<TChar>(bufferEnd, value, digits) : Number.UInt128ToDecChars<TChar>(bufferEnd, value);
        }
        return true;
      }
      charsWritten = 0;
      return false;
    }

    internal static char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
    {
      char c = char.MinValue;
      if (format.Length > 0)
      {
        c = format[0];
        if (char.IsAsciiLetter(c))
        {
          if (format.Length == 1)
          {
            digits = -1;
            return c;
          }
          if (format.Length == 2)
          {
            int num = (int) format[1] - 48;
            if ((uint) num < 10U)
            {
              digits = num;
              return c;
            }
          }
          else if (format.Length == 3)
          {
            int num1 = (int) format[1] - 48;
            int num2 = (int) format[2] - 48;
            if ((uint) num1 < 10U && (uint) num2 < 10U)
            {
              digits = num1 * 10 + num2;
              return c;
            }
          }
          int num3 = 0;
          int index;
          for (index = 1; (uint) index < (uint) format.Length && char.IsAsciiDigit(format[index]); num3 = num3 * 10 + (int) format[index++] - 48)
          {
            if (num3 >= 100000000)
              ThrowHelper.ThrowFormatException_BadFormatSpecifier();
          }
          if ((uint) index >= (uint) format.Length || format[index] == char.MinValue)
          {
            digits = num3;
            return c;
          }
        }
      }
      digits = -1;
      return format.Length != 0 && c != char.MinValue ? char.MinValue : 'G';
    }

    internal static void NumberToString<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      char format,
      int nMaxDigits,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      bool isCorrectlyRounded = number.Kind == Number.NumberBufferKind.FloatingPoint;
      switch (format)
      {
        case 'C':
        case 'c':
          if (nMaxDigits < 0)
            nMaxDigits = info.CurrencyDecimalDigits;
          Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
          Number.FormatCurrency<TChar>(ref vlb, ref number, nMaxDigits, info);
          break;
        case 'E':
        case 'e':
          if (nMaxDigits < 0)
            nMaxDigits = 6;
          ++nMaxDigits;
          Number.RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);
          if (number.IsNegative)
            vlb.Append(info.NegativeSignTChar<TChar>());
          Number.FormatScientific<TChar>(ref vlb, ref number, nMaxDigits, info, format);
          break;
        case 'F':
        case 'f':
          if (nMaxDigits < 0)
            nMaxDigits = info.NumberDecimalDigits;
          Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
          if (number.IsNegative)
            vlb.Append(info.NegativeSignTChar<TChar>());
          Number.FormatFixed<TChar>(ref vlb, ref number, nMaxDigits, (int[]) null, info.NumberDecimalSeparatorTChar<TChar>(), (ReadOnlySpan<TChar>) (TChar[]) null);
          break;
        case 'G':
        case 'g':
          bool suppressScientific = false;
          if (nMaxDigits < 1)
          {
            if (number.Kind == Number.NumberBufferKind.Decimal && nMaxDigits == -1)
            {
              suppressScientific = true;
              if (number.Digits[0] != (byte) 0)
                goto label_22;
              else
                goto label_24;
            }
            else
              nMaxDigits = number.DigitsCount;
          }
          Number.RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);
label_22:
          if (number.IsNegative)
            vlb.Append(info.NegativeSignTChar<TChar>());
label_24:
          Number.FormatGeneral<TChar>(ref vlb, ref number, nMaxDigits, info, (char) ((uint) format - 2U), suppressScientific);
          break;
        case 'N':
        case 'n':
          if (nMaxDigits < 0)
            nMaxDigits = info.NumberDecimalDigits;
          Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
          Number.FormatNumber<TChar>(ref vlb, ref number, nMaxDigits, info);
          break;
        case 'P':
        case 'p':
          if (nMaxDigits < 0)
            nMaxDigits = info.PercentDecimalDigits;
          number.Scale += 2;
          Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
          Number.FormatPercent<TChar>(ref vlb, ref number, nMaxDigits, info);
          break;
        case 'R':
        case 'r':
          format -= '\v';
          goto case 'G';
        default:
          ThrowHelper.ThrowFormatException_BadFormatSpecifier();
          break;
      }
    }

    internal static unsafe void NumberToStringFormat<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      ReadOnlySpan<char> format,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int num1 = 0;
      byte* digitsPointer = number.GetDigitsPointer();
      int num2 = Number.FindSection(format, *digitsPointer == (byte) 0 ? 2 : (number.IsNegative ? 1 : 0));
      int num3;
      int num4;
      int num5;
      int num6;
      bool flag1;
      bool flag2;
      while (true)
      {
        num3 = 0;
        num4 = -1;
        num5 = int.MaxValue;
        num6 = 0;
        flag1 = false;
        int num7 = -1;
        flag2 = false;
        int num8 = 0;
        int index = num2;
        fixed (char* chPtr1 = &MemoryMarshal.GetReference<char>(format))
        {
          while (index < format.Length)
          {
            char* chPtr2 = chPtr1;
            IntPtr num9 = (IntPtr) index++ * 2;
            char ch;
            if ((ch = (char) *(ushort*) ((IntPtr) chPtr2 + num9)) != char.MinValue)
            {
              switch (ch)
              {
                case '"':
                case '\'':
                  do
                    ;
                  while (index < format.Length && chPtr1[index] != char.MinValue && (int) chPtr1[index++] != (int) ch);
                  continue;
                case '#':
                  ++num3;
                  continue;
                case '%':
                  num8 += 2;
                  continue;
                case ',':
                  if (num3 > 0 && num4 < 0)
                  {
                    if (num7 >= 0)
                    {
                      if (num7 == num3)
                      {
                        ++num1;
                        continue;
                      }
                      flag2 = true;
                    }
                    num7 = num3;
                    num1 = 1;
                    continue;
                  }
                  continue;
                case '.':
                  if (num4 < 0)
                  {
                    num4 = num3;
                    continue;
                  }
                  continue;
                case '0':
                  if (num5 == int.MaxValue)
                    num5 = num3;
                  ++num3;
                  num6 = num3;
                  continue;
                case ';':
                  goto label_25;
                case 'E':
                case 'e':
                  if (index < format.Length && chPtr1[index] == '0' || index + 1 < format.Length && (chPtr1[index] == '+' || chPtr1[index] == '-') && chPtr1[index + 1] == '0')
                  {
                    do
                      ;
                    while (++index < format.Length && chPtr1[index] == '0');
                    flag1 = true;
                    continue;
                  }
                  continue;
                case '\\':
                  if (index < format.Length && chPtr1[index] != char.MinValue)
                  {
                    ++index;
                    continue;
                  }
                  continue;
                case '‰':
                  num8 += 3;
                  continue;
                default:
                  continue;
              }
            }
            else
              break;
          }
label_25:;
        }
        if (num4 < 0)
          num4 = num3;
        if (num7 >= 0)
        {
          if (num7 == num4)
            num8 -= num1 * 3;
          else
            flag2 = true;
        }
        if (*digitsPointer != (byte) 0)
        {
          number.Scale += num8;
          int pos = flag1 ? num3 : number.Scale + num3 - num4;
          Number.RoundNumber(ref number, pos, false);
          if (*digitsPointer == (byte) 0)
          {
            int section = Number.FindSection(format, 2);
            if (section != num2)
              num2 = section;
            else
              goto label_38;
          }
          else
            goto label_38;
        }
        else
          break;
      }
      if (number.Kind != Number.NumberBufferKind.FloatingPoint)
        number.IsNegative = false;
      number.Scale = 0;
label_38:
      int num10 = num5 < num4 ? num4 - num5 : 0;
      int num11 = num6 > num4 ? num4 - num6 : 0;
      int num12;
      int num13;
      if (flag1)
      {
        num12 = num4;
        num13 = 0;
      }
      else
      {
        num12 = number.Scale > num4 ? number.Scale : num4;
        num13 = number.Scale - num4;
      }
      int index1 = num2;
      Span<int> span = stackalloc int[4];
      int index2 = -1;
      if (flag2 && info.NumberGroupSeparator.Length > 0)
      {
        int[] numberGroupSizes = info._numberGroupSizes;
        int index3 = 0;
        int num14 = 0;
        int length = numberGroupSizes.Length;
        if (length != 0)
          num14 = numberGroupSizes[index3];
        int num15 = num14;
        int num16 = num12 + (num13 < 0 ? num13 : 0);
        for (int index4 = num10 > num16 ? num10 : num16; index4 > num14 && num15 != 0; num14 += num15)
        {
          ++index2;
          if (index2 >= span.Length)
          {
            int[] destination = new int[span.Length * 2];
            span.CopyTo((Span<int>) destination);
            span = (Span<int>) destination;
          }
          span[index2] = num14;
          if (index3 < length - 1)
          {
            ++index3;
            num15 = numberGroupSizes[index3];
          }
        }
      }
      if (number.IsNegative && num2 == 0 && number.Scale != 0)
        vlb.Append(info.NegativeSignTChar<TChar>());
      bool flag3 = false;
      fixed (char* chPtr3 = &MemoryMarshal.GetReference<char>(format))
      {
        byte* numPtr = digitsPointer;
label_96:
        while (index1 < format.Length)
        {
          char* chPtr4 = chPtr3;
          IntPtr num17 = (IntPtr) index1++ * 2;
          char ch1;
          if ((ch1 = (char) *(ushort*) ((IntPtr) chPtr4 + num17)) != char.MinValue && ch1 != ';')
          {
            if (num13 > 0 && (ch1 == '#' || ch1 == '.' || ch1 == '0'))
            {
              for (; num13 > 0; --num13)
              {
                vlb.Append(TChar.CastFrom(*numPtr != (byte) 0 ? (char) *numPtr++ : '0'));
                if (flag2 && num12 > 1 && index2 >= 0 && num12 == span[index2] + 1)
                {
                  vlb.Append(info.NumberGroupSeparatorTChar<TChar>());
                  --index2;
                }
                --num12;
              }
            }
            switch (ch1)
            {
              case '"':
              case '\'':
                while (index1 < format.Length && chPtr3[index1] != char.MinValue && (int) chPtr3[index1] != (int) ch1)
                  Number.AppendUnknownChar<TChar>(ref vlb, chPtr3[index1++]);
                if (index1 < format.Length && chPtr3[index1] != char.MinValue)
                {
                  ++index1;
                  continue;
                }
                continue;
              case '#':
              case '0':
                char ch2;
                if (num13 < 0)
                {
                  ++num13;
                  ch2 = num12 <= num10 ? '0' : char.MinValue;
                }
                else
                  ch2 = *numPtr != (byte) 0 ? (char) *numPtr++ : (num12 > num11 ? '0' : char.MinValue);
                if (ch2 != char.MinValue)
                {
                  vlb.Append(TChar.CastFrom(ch2));
                  if (flag2 && num12 > 1 && index2 >= 0 && num12 == span[index2] + 1)
                  {
                    vlb.Append(info.NumberGroupSeparatorTChar<TChar>());
                    --index2;
                  }
                }
                --num12;
                continue;
              case '%':
                vlb.Append(info.PercentSymbolTChar<TChar>());
                continue;
              case ',':
                continue;
              case '.':
                if (!(num12 != 0 | flag3) && (num11 < 0 || num4 < num3 && *numPtr != (byte) 0))
                {
                  vlb.Append(info.NumberDecimalSeparatorTChar<TChar>());
                  flag3 = true;
                  continue;
                }
                continue;
              case 'E':
              case 'e':
                bool positiveSign = false;
                int minDigits = 0;
                if (flag1)
                {
                  if (index1 < format.Length && chPtr3[index1] == '0')
                    ++minDigits;
                  else if (index1 + 1 < format.Length && chPtr3[index1] == '+' && chPtr3[index1 + 1] == '0')
                    positiveSign = true;
                  else if (index1 + 1 >= format.Length || chPtr3[index1] != '-' || chPtr3[index1 + 1] != '0')
                  {
                    vlb.Append(TChar.CastFrom(ch1));
                    continue;
                  }
                  while (++index1 < format.Length && chPtr3[index1] == '0')
                    ++minDigits;
                  if (minDigits > 10)
                    minDigits = 10;
                  int num18 = *digitsPointer == (byte) 0 ? 0 : number.Scale - num4;
                  Number.FormatExponent<TChar>(ref vlb, info, num18, ch1, minDigits, positiveSign);
                  flag1 = false;
                  continue;
                }
                vlb.Append(TChar.CastFrom(ch1));
                if (index1 < format.Length)
                {
                  if (chPtr3[index1] == '+' || chPtr3[index1] == '-')
                    Number.AppendUnknownChar<TChar>(ref vlb, chPtr3[index1++]);
                  while (true)
                  {
                    if (index1 < format.Length && chPtr3[index1] == '0')
                      Number.AppendUnknownChar<TChar>(ref vlb, chPtr3[index1++]);
                    else
                      goto label_96;
                  }
                }
                else
                  continue;
              case '\\':
                if (index1 < format.Length && chPtr3[index1] != char.MinValue)
                {
                  Number.AppendUnknownChar<TChar>(ref vlb, chPtr3[index1++]);
                  continue;
                }
                continue;
              case '‰':
                vlb.Append(info.PerMilleSymbolTChar<TChar>());
                continue;
              default:
                Number.AppendUnknownChar<TChar>(ref vlb, ch1);
                continue;
            }
          }
          else
            break;
        }
      }
      if (!number.IsNegative || num2 != 0 || number.Scale != 0 || vlb.Length <= 0)
        return;
      vlb.Insert(0, info.NegativeSignTChar<TChar>());
    }

    private static void FormatCurrency<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      int nMaxDigits,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      foreach (char ch in number.IsNegative ? Number.s_negCurrencyFormats[info.CurrencyNegativePattern] : Number.s_posCurrencyFormats[info.CurrencyPositivePattern])
      {
        switch (ch)
        {
          case '#':
            Number.FormatFixed<TChar>(ref vlb, ref number, nMaxDigits, info._currencyGroupSizes, info.CurrencyDecimalSeparatorTChar<TChar>(), info.CurrencyGroupSeparatorTChar<TChar>());
            break;
          case '$':
            vlb.Append(info.CurrencySymbolTChar<TChar>());
            break;
          case '-':
            vlb.Append(info.NegativeSignTChar<TChar>());
            break;
          default:
            vlb.Append(TChar.CastFrom(ch));
            break;
        }
      }
    }

    private static unsafe void FormatFixed<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      int nMaxDigits,
      int[] groupDigits,
      ReadOnlySpan<TChar> sDecimal,
      ReadOnlySpan<TChar> sGroup)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int scale = number.Scale;
      byte* digitsPointer = number.GetDigitsPointer();
      if (scale > 0)
      {
        if (groupDigits != null)
        {
          int index1 = 0;
          int length = scale;
          int num1 = 0;
          if (groupDigits.Length != 0)
          {
            int groupDigit = groupDigits[index1];
            while (scale > groupDigit && groupDigits[index1] != 0)
            {
              length += sGroup.Length;
              if (index1 < groupDigits.Length - 1)
                ++index1;
              groupDigit += groupDigits[index1];
              if ((groupDigit | length) < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            num1 = groupDigit == 0 ? 0 : groupDigits[0];
          }
          int index2 = 0;
          int num2 = 0;
          int digitsCount = number.DigitsCount;
          int num3 = scale < digitsCount ? scale : digitsCount;
          fixed (TChar* charPtr1 = &MemoryMarshal.GetReference<TChar>(vlb.AppendSpan(length)))
          {
            TChar* charPtr2 = charPtr1 + length - 1;
            for (int index3 = scale - 1; index3 >= 0; --index3)
            {
              *charPtr2-- = TChar.CastFrom(index3 < num3 ? (char) digitsPointer[index3] : '0');
              if (num1 > 0)
              {
                ++num2;
                if (num2 == num1 && index3 != 0)
                {
                  for (int index4 = sGroup.Length - 1; index4 >= 0; --index4)
                    *charPtr2-- = sGroup[index4];
                  if (index2 < groupDigits.Length - 1)
                  {
                    ++index2;
                    num1 = groupDigits[index2];
                  }
                  num2 = 0;
                }
              }
            }
            digitsPointer += num3;
          }
        }
        else
        {
          do
          {
            vlb.Append(TChar.CastFrom(*digitsPointer != (byte) 0 ? (char) *digitsPointer++ : '0'));
          }
          while (--scale > 0);
        }
      }
      else
        vlb.Append(TChar.CastFrom('0'));
      if (nMaxDigits <= 0)
        return;
      vlb.Append(sDecimal);
      if (scale < 0 && nMaxDigits > 0)
      {
        int num4 = Math.Min(-scale, nMaxDigits);
        for (int index = 0; index < num4; ++index)
          vlb.Append(TChar.CastFrom('0'));
        int num5 = scale + num4;
        nMaxDigits -= num4;
      }
      for (; nMaxDigits > 0; --nMaxDigits)
        vlb.Append(TChar.CastFrom(*digitsPointer != (byte) 0 ? (char) *digitsPointer++ : '0'));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendUnknownChar<TChar>(ref ValueListBuilder<TChar> vlb, char ch) where TChar : unmanaged, IUtfChar<TChar>
    {
      if (typeof (TChar) == typeof (char) || char.IsAscii(ch))
        vlb.Append(TChar.CastFrom(ch));
      else
        AppendNonAsciiBytes(ref vlb, ch);

      [MethodImpl(MethodImplOptions.NoInlining)]
      static void AppendNonAsciiBytes(ref ValueListBuilder<TChar> vlb, char ch)
      {
        Rune rune = new Rune(ch);
        rune.EncodeToUtf8(MemoryMarshal.AsBytes<TChar>(vlb.AppendSpan(rune.Utf8SequenceLength)));
      }
    }

    private static void FormatNumber<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      int nMaxDigits,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      foreach (char ch in number.IsNegative ? Number.s_negNumberFormats[info.NumberNegativePattern] : "#")
      {
        switch (ch)
        {
          case '#':
            Number.FormatFixed<TChar>(ref vlb, ref number, nMaxDigits, info._numberGroupSizes, info.NumberDecimalSeparatorTChar<TChar>(), info.NumberGroupSeparatorTChar<TChar>());
            break;
          case '-':
            vlb.Append(info.NegativeSignTChar<TChar>());
            break;
          default:
            vlb.Append(TChar.CastFrom(ch));
            break;
        }
      }
    }

    private static unsafe void FormatScientific<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      int nMaxDigits,
      NumberFormatInfo info,
      char expChar)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      byte* digitsPointer = number.GetDigitsPointer();
      vlb.Append(TChar.CastFrom(*digitsPointer != (byte) 0 ? (char) *digitsPointer++ : '0'));
      if (nMaxDigits != 1)
        vlb.Append(info.NumberDecimalSeparatorTChar<TChar>());
      while (--nMaxDigits > 0)
        vlb.Append(TChar.CastFrom(*digitsPointer != (byte) 0 ? (char) *digitsPointer++ : '0'));
      int num = number.Digits[0] == (byte) 0 ? 0 : number.Scale - 1;
      Number.FormatExponent<TChar>(ref vlb, info, num, expChar, 3, true);
    }

    private static unsafe void FormatExponent<TChar>(
      ref ValueListBuilder<TChar> vlb,
      NumberFormatInfo info,
      int value,
      char expChar,
      int minDigits,
      bool positiveSign)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      vlb.Append(TChar.CastFrom(expChar));
      if (value < 0)
      {
        vlb.Append(info.NegativeSignTChar<TChar>());
        value = -value;
      }
      else if (positiveSign)
        vlb.Append(info.PositiveSignTChar<TChar>());
      TChar* charPtr = stackalloc TChar[10];
      TChar* decChars = Number.UInt32ToDecChars<TChar>(charPtr + 10, (uint) value, minDigits);
      vlb.Append(new ReadOnlySpan<TChar>((void*) decChars, (int) (charPtr + 10 - decChars)));
    }

    private static unsafe void FormatGeneral<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      int nMaxDigits,
      NumberFormatInfo info,
      char expChar,
      bool suppressScientific)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      int num = number.Scale;
      bool flag = false;
      if (!suppressScientific && (num > nMaxDigits || num < -3))
      {
        num = 1;
        flag = true;
      }
      byte* digitsPointer = number.GetDigitsPointer();
      if (num > 0)
      {
        do
        {
          vlb.Append(TChar.CastFrom(*digitsPointer != (byte) 0 ? (char) *digitsPointer++ : '0'));
        }
        while (--num > 0);
      }
      else
        vlb.Append(TChar.CastFrom('0'));
      if (*digitsPointer != (byte) 0 || num < 0)
      {
        vlb.Append(info.NumberDecimalSeparatorTChar<TChar>());
        for (; num < 0; ++num)
          vlb.Append(TChar.CastFrom('0'));
        while (*digitsPointer != (byte) 0)
          vlb.Append(TChar.CastFrom(*digitsPointer++));
      }
      if (!flag)
        return;
      Number.FormatExponent<TChar>(ref vlb, info, number.Scale - 1, expChar, 2, true);
    }

    private static void FormatPercent<TChar>(
      ref ValueListBuilder<TChar> vlb,
      ref Number.NumberBuffer number,
      int nMaxDigits,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      foreach (char ch in number.IsNegative ? Number.s_negPercentFormats[info.PercentNegativePattern] : Number.s_posPercentFormats[info.PercentPositivePattern])
      {
        switch (ch)
        {
          case '#':
            Number.FormatFixed<TChar>(ref vlb, ref number, nMaxDigits, info._percentGroupSizes, info.PercentDecimalSeparatorTChar<TChar>(), info.PercentGroupSeparatorTChar<TChar>());
            break;
          case '%':
            vlb.Append(info.PercentSymbolTChar<TChar>());
            break;
          case '-':
            vlb.Append(info.NegativeSignTChar<TChar>());
            break;
          default:
            vlb.Append(TChar.CastFrom(ch));
            break;
        }
      }
    }

    internal static unsafe void RoundNumber(
      ref Number.NumberBuffer number,
      int pos,
      bool isCorrectlyRounded)
    {
      byte* digitsPointer = number.GetDigitsPointer();
      int i = 0;
      while (i < pos && digitsPointer[i] != (byte) 0)
        ++i;
      if (i == pos && ShouldRoundUp(digitsPointer, i, number.Kind, isCorrectlyRounded))
      {
        while (i > 0 && digitsPointer[i - 1] == (byte) 57)
          --i;
        if (i > 0)
        {
          byte* numPtr = digitsPointer + (i - 1);
          *numPtr = (byte) ((uint) *numPtr + 1U);
        }
        else
        {
          ++number.Scale;
          *digitsPointer = (byte) 49;
          i = 1;
        }
      }
      else
      {
        while (i > 0 && digitsPointer[i - 1] == (byte) 48)
          --i;
      }
      if (i == 0)
      {
        if (number.Kind != Number.NumberBufferKind.FloatingPoint)
          number.IsNegative = false;
        number.Scale = 0;
      }
      digitsPointer[i] = (byte) 0;
      number.DigitsCount = i;

      static unsafe bool ShouldRoundUp(
        byte* dig,
        int i,
        Number.NumberBufferKind numberKind,
        bool isCorrectlyRounded)
      {
        byte num = dig[i];
        return !(num == (byte) 0 | isCorrectlyRounded) && num >= (byte) 53;
      }
    }

    private static unsafe int FindSection(ReadOnlySpan<char> format, int section)
    {
      if (section == 0)
        return 0;
      fixed (char* chPtr1 = &MemoryMarshal.GetReference<char>(format))
      {
        int section1 = 0;
        while (section1 < format.Length)
        {
          char* chPtr2 = chPtr1;
          IntPtr num = (IntPtr) section1++ * 2;
          char ch;
          switch (ch = (char) *(ushort*) ((IntPtr) chPtr2 + num))
          {
            case char.MinValue:
              return 0;
            case '"':
            case '\'':
              do
                ;
              while (section1 < format.Length && chPtr1[section1] != char.MinValue && (int) chPtr1[section1++] != (int) ch);
              continue;
            case ';':
              if (--section == 0)
              {
                if (section1 < format.Length && chPtr1[section1] != char.MinValue && chPtr1[section1] != ';')
                  return section1;
                goto case char.MinValue;
              }
              else
                continue;
            case '\\':
              if (section1 < format.Length && chPtr1[section1] != char.MinValue)
              {
                ++section1;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
        return 0;
      }
    }

    private static ulong ExtractFractionAndBiasedExponent(double value, out int exponent)
    {
      ulong uint64Bits = BitConverter.DoubleToUInt64Bits(value);
      ulong andBiasedExponent = uint64Bits & 4503599627370495UL;
      exponent = (int) (uint64Bits >> 52) & 2047;
      if (exponent != 0)
      {
        andBiasedExponent |= 4503599627370496UL;
        exponent -= 1075;
      }
      else
        exponent = -1074;
      return andBiasedExponent;
    }

    private static ushort ExtractFractionAndBiasedExponent(Half value, out int exponent)
    {
      ushort uint16Bits = BitConverter.HalfToUInt16Bits(value);
      ushort andBiasedExponent = (ushort) ((uint) uint16Bits & 1023U);
      exponent = (int) uint16Bits >> 10 & 31;
      if (exponent != 0)
      {
        andBiasedExponent |= (ushort) 1024;
        exponent -= 25;
      }
      else
        exponent = -24;
      return andBiasedExponent;
    }

    private static uint ExtractFractionAndBiasedExponent(float value, out int exponent)
    {
      uint uint32Bits = BitConverter.SingleToUInt32Bits(value);
      uint andBiasedExponent = uint32Bits & 8388607U;
      exponent = (int) (uint32Bits >> 23) & (int) byte.MaxValue;
      if (exponent != 0)
      {
        andBiasedExponent |= 8388608U;
        exponent -= 150;
      }
      else
        exponent = -149;
      return andBiasedExponent;
    }

    private static ReadOnlySpan<double> Pow10DoubleTable
    {
      get
      {
        return RuntimeHelpers.CreateSpan<double>(__fieldref (\u003CPrivateImplementationDetails\u003E.\u00354163634AEBD5768E4BE0AF562CC018EB2E45008D13F13F14366BE52C290193D8));
      }
    }

    private static ReadOnlySpan<ulong> Pow5128Table
    {
      get
      {
        return RuntimeHelpers.CreateSpan<ulong>(__fieldref (\u003CPrivateImplementationDetails\u003E.DAE47E3594BB0B39D42660EF3FE3DC5DE5885C0A29AE0E5C21A39E79669EFE868));
      }
    }

    private static unsafe void AccumulateDecimalDigitsIntoBigInteger(
      [ScopedRef] ref Number.NumberBuffer number,
      uint firstIndex,
      uint lastIndex,
      out Number.BigInteger result)
    {
      Number.BigInteger.SetZero(out result);
      byte* p = number.GetDigitsPointer() + firstIndex;
      uint num;
      for (uint val1 = lastIndex - firstIndex; val1 != 0U; val1 -= num)
      {
        num = Math.Min(val1, 9U);
        uint uint32 = Number.DigitsToUInt32(p, (int) num);
        result.MultiplyPow10(num);
        result.Add(uint32);
        p += num;
      }
    }

    private static ulong AssembleFloatingPointBits<TFloat>(
      ulong initialMantissa,
      int initialExponent,
      bool hasZeroTail)
      where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      int num1 = (int) TFloat.NormalMantissaBits - (int) Number.BigInteger.CountSignificantBits(initialMantissa);
      int num2 = initialExponent - num1;
      ulong num3 = initialMantissa;
      int num4 = num2;
      if (num2 > TFloat.MaxBinaryExponent)
        return TFloat.InfinityBits;
      if (num2 < TFloat.MinBinaryExponent)
      {
        int num5 = num1 + num2 + TFloat.ExponentBias - 1;
        num4 = -TFloat.ExponentBias;
        if (num5 < 0)
        {
          num3 = Number.RightShiftWithRounding(num3, -num5, hasZeroTail);
          if (num3 == 0UL)
            return TFloat.ZeroBits;
          if (num3 > TFloat.DenormalMantissaMask)
            num4 = initialExponent - (num5 + 1) - num1;
        }
        else
          num3 <<= num5;
      }
      else if (num1 < 0)
      {
        num3 = Number.RightShiftWithRounding(num3, -num1, hasZeroTail);
        if (num3 > TFloat.NormalMantissaMask)
        {
          num3 >>= 1;
          ++num4;
          if (num4 > TFloat.MaxBinaryExponent)
            return TFloat.InfinityBits;
        }
      }
      else if (num1 > 0)
        num3 <<= num1;
      ulong num6 = num3 & TFloat.DenormalMantissaMask;
      return (ulong) (num4 + TFloat.ExponentBias) << (int) TFloat.DenormalMantissaBits | num6;
    }

    private static ulong ConvertBigIntegerToFloatingPointBits<TFloat>(
      ref Number.BigInteger value,
      uint integerBitsOfPrecision,
      bool hasNonZeroFractionalPart)
      where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      int denormalMantissaBits = (int) TFloat.DenormalMantissaBits;
      if (integerBitsOfPrecision <= 64U)
        return Number.AssembleFloatingPointBits<TFloat>(value.ToUInt64(), denormalMantissaBits, !hasNonZeroFractionalPart);
      (uint num1, uint Remainder) = Math.DivRem(integerBitsOfPrecision, 32U);
      uint index1 = num1 - 1U;
      uint index2 = index1 - 1U;
      int initialExponent = denormalMantissaBits + (int) index2 * 32;
      bool hasZeroTail = !hasNonZeroFractionalPart;
      ulong initialMantissa;
      if (Remainder == 0U)
      {
        initialMantissa = ((ulong) value.GetBlock(index1) << 32) + (ulong) value.GetBlock(index2);
      }
      else
      {
        int num2 = (int) Remainder;
        int num3 = 64 - num2;
        int num4 = num3 - 32;
        initialExponent += (int) Remainder;
        uint block = value.GetBlock(index2);
        uint num5 = block >> num2;
        ulong num6 = (ulong) value.GetBlock(index1) << num4;
        initialMantissa = ((ulong) value.GetBlock(num1) << num3) + num6 + (ulong) num5;
        uint num7 = (uint) ((1 << (int) Remainder) - 1);
        hasZeroTail &= ((int) block & (int) num7) == 0;
      }
      for (uint index3 = 0; (int) index3 != (int) index2; ++index3)
        hasZeroTail &= value.GetBlock(index3) == 0U;
      return Number.AssembleFloatingPointBits<TFloat>(initialMantissa, initialExponent, hasZeroTail);
    }

    private static unsafe uint DigitsToUInt32(byte* p, int count)
    {
      byte* numPtr = p + count;
      uint uint32 = 0;
      for (; p <= numPtr - 8; p += 8)
        uint32 = uint32 * 100000000U + Number.ParseEightDigitsUnrolled(p);
      for (; p != numPtr; ++p)
        uint32 = (uint) (10 * (int) uint32 + (int) *p - 48);
      return uint32;
    }

    private static unsafe ulong DigitsToUInt64(byte* p, int count)
    {
      byte* numPtr = p + count;
      ulong uint64 = 0;
      for (; numPtr - p >= 8L; p += 8)
        uint64 = uint64 * 100000000UL + (ulong) Number.ParseEightDigitsUnrolled(p);
      for (; p != numPtr; ++p)
        uint64 = 10UL * uint64 + (ulong) *p - 48UL;
      return uint64;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe uint ParseEightDigitsUnrolled(byte* chars)
    {
      ulong num1 = Unsafe.ReadUnaligned<ulong>((void*) chars);
      if (BitConverter.IsLittleEndian)
        ;
      ulong num2 = num1 - 3472328296227680304UL;
      ulong num3 = num2 * 10UL + (num2 >> 8);
      return (uint) (((long) num3 & 1095216660735L) * 4294967296000100L + ((long) (num3 >> 16) & 1095216660735L) * 42949672960001L >>> 32);
    }

    private static unsafe ulong NumberToFloatingPointBits<TFloat>(ref Number.NumberBuffer number) where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      uint digitsCount = (uint) number.DigitsCount;
      uint num1 = (uint) Math.Max(0, number.Scale);
      uint integerDigitsPresent = Math.Min(num1, digitsCount);
      uint fractionalDigitsPresent = digitsCount - integerDigitsPresent;
      if (digitsCount <= 19U)
      {
        ulong uint64 = Number.DigitsToUInt64(number.GetDigitsPointer(), (int) digitsCount);
        int q = (int) ((long) number.Scale - (long) integerDigitsPresent - (long) fractionalDigitsPresent);
        int index = Math.Abs(q);
        if (uint64 <= TFloat.MaxMantissaFastPath && index <= TFloat.MaxExponentFastPath)
        {
          double num2 = (double) uint64;
          double num3 = Number.Pow10DoubleTable[index];
          return TFloat.FloatToBits(TFloat.CreateSaturating<double>(fractionalDigitsPresent == 0U ? num2 * num3 : num2 / num3));
        }
        (int Exponent, ulong Mantissa) tuple = Number.ComputeFloat<TFloat>((long) q, uint64);
        if (tuple.Exponent > 0)
          return tuple.Mantissa | (ulong) (uint) tuple.Exponent << (int) TFloat.DenormalMantissaBits;
      }
      return Number.NumberToFloatingPointBitsSlow<TFloat>(ref number, num1, integerDigitsPresent, fractionalDigitsPresent);
    }

    private static ulong NumberToFloatingPointBitsSlow<TFloat>(
      ref Number.NumberBuffer number,
      uint positiveExponent,
      uint integerDigitsPresent,
      uint fractionalDigitsPresent)
      where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      uint num1 = (uint) TFloat.NormalMantissaBits + 1U;
      uint digitsCount = (uint) number.DigitsCount;
      uint exponent1 = positiveExponent - integerDigitsPresent;
      uint lastIndex1 = integerDigitsPresent;
      uint firstIndex = lastIndex1;
      uint lastIndex2 = digitsCount;
      Number.BigInteger result1;
      Number.AccumulateDecimalDigitsIntoBigInteger(ref number, 0U, lastIndex1, out result1);
      if (exponent1 > 0U)
      {
        if ((long) exponent1 > (long) TFloat.OverflowDecimalExponent)
          return TFloat.InfinityBits;
        result1.MultiplyPow10(exponent1);
      }
      uint integerBitsOfPrecision = Number.BigInteger.CountSignificantBits(ref result1);
      if (integerBitsOfPrecision >= num1 || fractionalDigitsPresent == 0U)
        return Number.ConvertBigIntegerToFloatingPointBits<TFloat>(ref result1, integerBitsOfPrecision, fractionalDigitsPresent > 0U);
      uint exponent2 = fractionalDigitsPresent;
      if (number.Scale < 0)
        exponent2 += (uint) -number.Scale;
      if (integerBitsOfPrecision == 0U && (long) exponent2 - (long) (int) digitsCount > (long) TFloat.OverflowDecimalExponent)
        return TFloat.ZeroBits;
      Number.BigInteger result2;
      Number.AccumulateDecimalDigitsIntoBigInteger(ref number, firstIndex, lastIndex2, out result2);
      if (result2.IsZero())
        return Number.ConvertBigIntegerToFloatingPointBits<TFloat>(ref result1, integerBitsOfPrecision, fractionalDigitsPresent > 0U);
      Number.BigInteger result3;
      Number.BigInteger.Pow10(exponent2, out result3);
      uint num2 = Number.BigInteger.CountSignificantBits(ref result2);
      uint num3 = Number.BigInteger.CountSignificantBits(ref result3);
      uint shift1 = 0;
      if (num3 > num2)
        shift1 = num3 - num2;
      if (shift1 > 0U)
        result2.ShiftLeft(shift1);
      uint num4 = num1 - integerBitsOfPrecision;
      uint shift2 = num4;
      if (integerBitsOfPrecision > 0U)
      {
        if (shift1 > shift2)
          return Number.ConvertBigIntegerToFloatingPointBits<TFloat>(ref result1, integerBitsOfPrecision, fractionalDigitsPresent > 0U);
        shift2 -= shift1;
      }
      uint num5 = shift1;
      if (Number.BigInteger.Compare(ref result2, ref result3) < 0)
        ++num5;
      result2.ShiftLeft(shift2);
      Number.BigInteger quo;
      Number.BigInteger rem;
      Number.BigInteger.DivRem(ref result2, ref result3, out quo, out rem);
      ulong uint64 = quo.ToUInt64();
      bool hasZeroTail = !number.HasNonZeroTail && rem.IsZero();
      uint num6 = Number.BigInteger.CountSignificantBits(uint64);
      if (num6 > num4)
      {
        int num7 = (int) num6 - (int) num4;
        hasZeroTail = hasZeroTail && ((long) uint64 & (1L << num7) - 1L) == 0L;
        uint64 >>= num7;
      }
      return Number.AssembleFloatingPointBits<TFloat>((result1.ToUInt64() << (int) num4) + uint64, integerBitsOfPrecision > 0U ? (int) integerBitsOfPrecision - 2 : -(int) num5 - 1, hasZeroTail);
    }

    private static ulong RightShiftWithRounding(ulong value, int shift, bool hasZeroTail)
    {
      if (shift >= 64)
        return 0;
      ulong num1 = (ulong) (1L << shift - 1) - 1UL;
      ulong num2 = 1UL << shift - 1;
      ulong num3 = 1UL << shift;
      bool lsbBit = (value & num3) > 0UL;
      bool roundBit = (value & num2) > 0UL;
      bool hasTailBits = !hasZeroTail || (value & num1) > 0UL;
      return (value >> shift) + (ulong) Number.ShouldRoundUp(lsbBit, roundBit, hasTailBits);
    }

    private static bool ShouldRoundUp(bool lsbBit, bool roundBit, bool hasTailBits)
    {
      return roundBit && hasTailBits | lsbBit;
    }

    internal static (int Exponent, ulong Mantissa) ComputeFloat<TFloat>(long q, ulong w) where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      ulong num1 = 0;
      if (w == 0UL || q < (long) TFloat.MinFastFloatDecimalExponent)
        return ();
      if (q > (long) TFloat.MaxFastFloatDecimalExponent)
        return (TFloat.InfinityExponent, 0UL);
      int num2 = BitOperations.LeadingZeroCount(w);
      w <<= num2;
      (ulong high, ulong low) productApproximation = Number.ComputeProductApproximation((int) TFloat.DenormalMantissaBits + 3, q, w);
      if (productApproximation.low == ulong.MaxValue && (q < -27L || q > 55L))
        return (-1, num1);
      int num3 = (int) (productApproximation.high >> 63);
      ulong num4 = productApproximation.high >> num3 + 64 - (int) TFloat.DenormalMantissaBits - 3;
      int num5 = Number.CalculatePower((int) q) + num3 - num2 - -TFloat.MaxBinaryExponent;
      if (num5 <= 0)
      {
        if (-num5 + 1 >= 64)
          return (0, 0UL);
        ulong num6 = num4 >> -num5 + 1;
        ulong num7 = num6 + (num6 & 1UL) >> 1;
        return (num7 >= 1UL << (int) TFloat.DenormalMantissaBits ? 1 : 0, num7);
      }
      if (productApproximation.low <= 1UL && q >= (long) TFloat.MinExponentRoundToEven && q <= (long) TFloat.MaxExponentRoundToEven && ((long) num4 & 3L) == 1L && (long) num4 << num3 + 64 - (int) TFloat.DenormalMantissaBits - 3 == (long) productApproximation.high)
        num4 &= 18446744073709551614UL;
      ulong num8 = num4 + (num4 & 1UL) >> 1;
      if (num8 >= 2UL << (int) TFloat.DenormalMantissaBits)
      {
        num8 = 1UL << (int) TFloat.DenormalMantissaBits;
        ++num5;
      }
      ulong num9 = num8 & (ulong) ~(1L << (int) TFloat.DenormalMantissaBits);
      if (num5 >= TFloat.InfinityExponent)
      {
        num5 = TFloat.InfinityExponent;
        num9 = 0UL;
      }
      return (num5, num9);
    }

    private static (ulong high, ulong low) ComputeProductApproximation(
      int bitPrecision,
      long q,
      ulong w)
    {
      int index = 2 * (int) (q - -342L);
      ulong low;
      ulong num1 = Math.BigMul(w, Number.Pow5128Table[index], out low);
      ulong num2 = bitPrecision < 64 ? ulong.MaxValue >> bitPrecision : ulong.MaxValue;
      if (((long) num1 & (long) num2) == (long) num2)
      {
        ulong num3 = Math.BigMul(w, Number.Pow5128Table[index + 1], out ulong _);
        low += num3;
        if (num3 > low)
          ++num1;
      }
      return (num1, low);
    }

    internal static int CalculatePower(int q) => (217706 * q >> 16) + 63;

    private static unsafe bool TryNumberBufferToBinaryInteger<TInteger>(
      ref Number.NumberBuffer number,
      ref TInteger value)
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      int scale = number.Scale;
      if (scale > TInteger.MaxDigitCount || scale < number.DigitsCount || !TInteger.IsSigned && number.IsNegative)
        return false;
      byte* digitsPointer = number.GetDigitsPointer();
      TInteger left = TInteger.Zero;
      while (--scale >= 0)
      {
        if (TInteger.IsGreaterThanAsUnsigned(left, TInteger.MaxValueDiv10))
          return false;
        left = TInteger.MultiplyBy10(left);
        if (*digitsPointer != (byte) 0)
        {
          TInteger integer = left + TInteger.CreateTruncating<int>((int) *digitsPointer++ - 48);
          if (!TInteger.IsSigned && integer < left)
            return false;
          left = integer;
        }
      }
      if (TInteger.IsSigned)
      {
        if (number.IsNegative)
        {
          left = -left;
          if (left > TInteger.Zero)
            return false;
        }
        else if (left < TInteger.Zero)
          return false;
      }
      value = left;
      return true;
    }

    internal static TInteger ParseBinaryInteger<TChar, TInteger>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      TInteger result;
      Number.ParsingStatus binaryInteger = Number.TryParseBinaryInteger<TChar, TInteger>(value, styles, info, out result);
      if (binaryInteger != Number.ParsingStatus.OK)
        Number.ThrowOverflowOrFormatException<TChar, TInteger>(binaryInteger, value);
      return result;
    }

    private static unsafe bool TryParseNumber<TChar>(
      [ScopedRef] ref TChar* str,
      TChar* strEnd,
      NumberStyles styles,
      ref Number.NumberBuffer number,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      ReadOnlySpan<TChar> readOnlySpan1 = ReadOnlySpan<TChar>.Empty;
      bool flag1 = false;
      ReadOnlySpan<TChar> readOnlySpan2;
      ReadOnlySpan<TChar> readOnlySpan3;
      if ((styles & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
      {
        readOnlySpan1 = info.CurrencySymbolTChar<TChar>();
        readOnlySpan2 = info.CurrencyDecimalSeparatorTChar<TChar>();
        readOnlySpan3 = info.CurrencyGroupSeparatorTChar<TChar>();
        flag1 = true;
      }
      else
      {
        readOnlySpan2 = info.NumberDecimalSeparatorTChar<TChar>();
        readOnlySpan3 = info.NumberGroupSeparatorTChar<TChar>();
      }
      int num1 = 0;
      TChar* p = str;
      uint uint32 = p < strEnd ? TChar.CastToUInt32(*p) : 0U;
      while (true)
      {
        if (!Number.IsWhite(uint32) || (styles & NumberStyles.AllowLeadingWhite) == NumberStyles.None || (num1 & 1) != 0 && (num1 & 32) == 0 && info.NumberNegativePattern != 2)
        {
          TChar* charPtr1;
          if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None && (num1 & 1) == 0 && ((IntPtr) (charPtr1 = Number.MatchChars<TChar>(p, strEnd, info.PositiveSignTChar<TChar>())) != IntPtr.Zero || (IntPtr) (charPtr1 = Number.MatchNegativeSignChars<TChar>(p, strEnd, info)) != IntPtr.Zero && (number.IsNegative = true)))
          {
            num1 |= 1;
            p = charPtr1 - 1;
          }
          else if (uint32 == 40U && (styles & NumberStyles.AllowParentheses) != NumberStyles.None && (num1 & 1) == 0)
          {
            num1 |= 3;
            number.IsNegative = true;
          }
          else
          {
            TChar* charPtr2;
            if (!readOnlySpan1.IsEmpty && (IntPtr) (charPtr2 = Number.MatchChars<TChar>(p, strEnd, readOnlySpan1)) != IntPtr.Zero)
            {
              num1 |= 32;
              readOnlySpan1 = ReadOnlySpan<TChar>.Empty;
              p = charPtr2 - 1;
            }
            else
              break;
          }
        }
        uint32 = ++p < strEnd ? TChar.CastToUInt32(*p) : 0U;
      }
      int index1 = 0;
      int index2 = 0;
      int num2 = number.Digits.Length - 1;
      int val1 = 0;
      while (true)
      {
        if (Number.IsDigit(uint32))
        {
          num1 |= 4;
          if (uint32 != 48U || (num1 & 8) != 0)
          {
            if (index1 < num2)
            {
              number.Digits[index1] = (byte) uint32;
              if (uint32 != 48U || number.Kind != Number.NumberBufferKind.Integer)
                index2 = index1 + 1;
            }
            else if (uint32 != 48U)
              number.HasNonZeroTail = true;
            if ((num1 & 16) == 0)
              ++number.Scale;
            if (index1 < num2)
            {
              if (uint32 == 48U)
                ++val1;
              else
                val1 = 0;
            }
            ++index1;
            num1 |= 8;
          }
          else if ((num1 & 16) != 0)
            --number.Scale;
        }
        else
        {
          TChar* charPtr3;
          if ((styles & NumberStyles.AllowDecimalPoint) != NumberStyles.None && (num1 & 16) == 0 && ((IntPtr) (charPtr3 = Number.MatchChars<TChar>(p, strEnd, readOnlySpan2)) != IntPtr.Zero || flag1 && (num1 & 32) == 0 && (IntPtr) (charPtr3 = Number.MatchChars<TChar>(p, strEnd, info.NumberDecimalSeparatorTChar<TChar>())) != IntPtr.Zero))
          {
            num1 |= 16;
            p = charPtr3 - 1;
          }
          else
          {
            TChar* charPtr4;
            if ((styles & NumberStyles.AllowThousands) != NumberStyles.None && (num1 & 4) != 0 && (num1 & 16) == 0 && ((IntPtr) (charPtr4 = Number.MatchChars<TChar>(p, strEnd, readOnlySpan3)) != IntPtr.Zero || flag1 && (num1 & 32) == 0 && (IntPtr) (charPtr4 = Number.MatchChars<TChar>(p, strEnd, info.NumberGroupSeparatorTChar<TChar>())) != IntPtr.Zero))
              p = charPtr4 - 1;
            else
              break;
          }
        }
        uint32 = ++p < strEnd ? TChar.CastToUInt32(*p) : 0U;
      }
      bool flag2 = false;
      number.DigitsCount = index2;
      number.Digits[index2] = (byte) 0;
      if ((num1 & 4) != 0)
      {
        if ((uint32 == 69U || uint32 == 101U) && (styles & NumberStyles.AllowExponent) != NumberStyles.None)
        {
          TChar* charPtr5 = p;
          uint32 = ++p < strEnd ? TChar.CastToUInt32(*p) : 0U;
          TChar* charPtr6;
          if ((IntPtr) (charPtr6 = Number.MatchChars<TChar>(p, strEnd, info.PositiveSignTChar<TChar>())) != IntPtr.Zero)
          {
            uint32 = (p = charPtr6) < strEnd ? TChar.CastToUInt32(*p) : 0U;
          }
          else
          {
            TChar* charPtr7;
            if ((IntPtr) (charPtr7 = Number.MatchNegativeSignChars<TChar>(p, strEnd, info)) != IntPtr.Zero)
            {
              uint32 = (p = charPtr7) < strEnd ? TChar.CastToUInt32(*p) : 0U;
              flag2 = true;
            }
          }
          if (Number.IsDigit(uint32))
          {
            int num3 = 0;
            do
            {
              num3 = num3 * 10 + ((int) uint32 - 48);
              uint32 = ++p < strEnd ? TChar.CastToUInt32(*p) : 0U;
              if (num3 > 1000)
              {
                num3 = 9999;
                while (Number.IsDigit(uint32))
                  uint32 = ++p < strEnd ? TChar.CastToUInt32(*p) : 0U;
              }
            }
            while (Number.IsDigit(uint32));
            if (flag2)
              num3 = -num3;
            number.Scale += num3;
          }
          else
          {
            p = charPtr5;
            uint32 = p < strEnd ? TChar.CastToUInt32(*p) : 0U;
          }
        }
        if (number.Kind == Number.NumberBufferKind.FloatingPoint && !number.HasNonZeroTail)
        {
          int val2 = index2 - number.Scale;
          if (val2 > 0)
          {
            int num4 = Math.Min(val1, val2);
            number.DigitsCount = index2 - num4;
            number.Digits[number.DigitsCount] = (byte) 0;
          }
        }
        while (true)
        {
          if (!Number.IsWhite(uint32) || (styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
          {
            TChar* charPtr8;
            if ((styles & NumberStyles.AllowTrailingSign) != NumberStyles.None && (num1 & 1) == 0 && ((IntPtr) (charPtr8 = Number.MatchChars<TChar>(p, strEnd, info.PositiveSignTChar<TChar>())) != IntPtr.Zero || (IntPtr) (charPtr8 = Number.MatchNegativeSignChars<TChar>(p, strEnd, info)) != IntPtr.Zero && (number.IsNegative = true)))
            {
              num1 |= 1;
              p = charPtr8 - 1;
            }
            else if (uint32 == 41U && (num1 & 2) != 0)
            {
              num1 &= -3;
            }
            else
            {
              TChar* charPtr9;
              if (!readOnlySpan1.IsEmpty && (IntPtr) (charPtr9 = Number.MatchChars<TChar>(p, strEnd, readOnlySpan1)) != IntPtr.Zero)
              {
                readOnlySpan1 = ReadOnlySpan<TChar>.Empty;
                p = charPtr9 - 1;
              }
              else
                break;
            }
          }
          uint32 = ++p < strEnd ? TChar.CastToUInt32(*p) : 0U;
        }
        if ((num1 & 2) == 0)
        {
          if ((num1 & 8) == 0)
          {
            if (number.Kind != Number.NumberBufferKind.Decimal)
              number.Scale = 0;
            if (number.Kind == Number.NumberBufferKind.Integer && (num1 & 16) == 0)
              number.IsNegative = false;
          }
          str = p;
          return true;
        }
      }
      str = p;
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Number.ParsingStatus TryParseBinaryInteger<TChar, TInteger>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info,
      out TInteger result)
      where TChar : unmanaged, IUtfChar<TChar>
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      if ((styles & ~NumberStyles.Integer) == NumberStyles.None)
        return Number.TryParseBinaryIntegerStyle<TChar, TInteger>(value, styles, info, out result);
      if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
        return Number.TryParseBinaryIntegerHexNumberStyle<TChar, TInteger>(value, styles, out result);
      return (styles & NumberStyles.AllowBinarySpecifier) != NumberStyles.None ? Number.TryParseBinaryIntegerHexOrBinaryNumberStyle<TChar, TInteger, Number.BinaryParser<TInteger>>(value, styles, out result) : Number.TryParseBinaryIntegerNumber<TChar, TInteger>(value, styles, info, out result);
    }

    private static Number.ParsingStatus TryParseBinaryIntegerNumber<TChar, TInteger>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info,
      out TInteger result)
      where TChar : unmanaged, IUtfChar<TChar>
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      result = TInteger.Zero;
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Integer, stackalloc byte[TInteger.MaxDigitCount + 1]);
      if (!Number.TryStringToNumber<TChar>(value, styles, ref number, info))
        return Number.ParsingStatus.Failed;
      return !Number.TryNumberBufferToBinaryInteger<TInteger>(ref number, ref result) ? Number.ParsingStatus.Overflow : Number.ParsingStatus.OK;
    }

    internal static Number.ParsingStatus TryParseBinaryIntegerStyle<TChar, TInteger>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info,
      out TInteger result)
      where TChar : unmanaged, IUtfChar<TChar>
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      Number.ParsingStatus binaryIntegerStyle;
      if (!value.IsEmpty)
      {
        int num = 0;
        uint uint32 = TChar.CastToUInt32(value[0]);
        if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(uint32))
        {
          do
          {
            ++num;
            if ((uint) num < (uint) value.Length)
              uint32 = TChar.CastToUInt32(value[num]);
            else
              goto label_46;
          }
          while (Number.IsWhite(uint32));
        }
        bool flag1 = false;
        if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
        {
          if (info.HasInvariantNumberSigns)
          {
            switch (uint32)
            {
              case 43:
                ++num;
                if ((uint) num < (uint) value.Length)
                {
                  uint32 = TChar.CastToUInt32(value[num]);
                  break;
                }
                goto label_46;
              case 45:
                flag1 = true;
                ++num;
                if ((uint) num < (uint) value.Length)
                {
                  uint32 = TChar.CastToUInt32(value[num]);
                  break;
                }
                goto label_46;
            }
          }
          else if (info.AllowHyphenDuringParsing && uint32 == 45U)
          {
            flag1 = true;
            ++num;
            if ((uint) num < (uint) value.Length)
              uint32 = TChar.CastToUInt32(value[num]);
            else
              goto label_46;
          }
          else
          {
            value = value.Slice(num);
            num = 0;
            ReadOnlySpan<TChar> readOnlySpan1 = info.PositiveSignTChar<TChar>();
            ReadOnlySpan<TChar> readOnlySpan2 = info.NegativeSignTChar<TChar>();
            if (!readOnlySpan1.IsEmpty && value.StartsWith<TChar>(readOnlySpan1))
            {
              num += readOnlySpan1.Length;
              if ((uint) num < (uint) value.Length)
                uint32 = TChar.CastToUInt32(value[num]);
              else
                goto label_46;
            }
            else if (!readOnlySpan2.IsEmpty && value.StartsWith<TChar>(readOnlySpan2))
            {
              flag1 = true;
              num += readOnlySpan2.Length;
              if ((uint) num < (uint) value.Length)
                uint32 = TChar.CastToUInt32(value[num]);
              else
                goto label_46;
            }
          }
        }
        bool flag2 = !TInteger.IsSigned & flag1;
        TInteger left = TInteger.Zero;
        if (Number.IsDigit(uint32))
        {
          if (uint32 == 48U)
          {
            do
            {
              ++num;
              if ((uint) num < (uint) value.Length)
                uint32 = TChar.CastToUInt32(value[num]);
              else
                goto label_44;
            }
            while (uint32 == 48U);
            if (!Number.IsDigit(uint32))
            {
              if (!TInteger.IsSigned)
              {
                flag2 = false;
                goto label_48;
              }
              else
                goto label_48;
            }
          }
          left = TInteger.CreateTruncating<uint>(uint32 - 48U);
          ++num;
          for (int index = 0; index < TInteger.MaxDigitCount - 2; ++index)
          {
            if ((uint) num >= (uint) value.Length)
            {
              if (TInteger.IsSigned)
                goto label_44;
              else
                goto label_43;
            }
            else
            {
              uint32 = TChar.CastToUInt32(value[num]);
              if (Number.IsDigit(uint32))
              {
                ++num;
                left = TInteger.MultiplyBy10(left) + TInteger.CreateTruncating<uint>(uint32 - 48U);
              }
              else
                goto label_48;
            }
          }
          if ((uint) num >= (uint) value.Length)
          {
            if (TInteger.IsSigned)
              goto label_44;
          }
          else
          {
            uint32 = TChar.CastToUInt32(value[num]);
            if (Number.IsDigit(uint32))
            {
              ++num;
              flag2 = TInteger.IsSigned ? left > TInteger.MaxValueDiv10 : ((flag2 ? 1 : 0) | (left > TInteger.MaxValueDiv10 ? 1 : (!(left == TInteger.MaxValueDiv10) ? 0 : (uint32 > 53U ? 1 : 0)))) != 0;
              left = TInteger.MultiplyBy10(left) + TInteger.CreateTruncating<uint>(uint32 - 48U);
              if (TInteger.IsSigned)
                flag2 |= TInteger.IsGreaterThanAsUnsigned(left, TInteger.MaxValue + (flag1 ? TInteger.One : TInteger.Zero));
              if ((uint) num < (uint) value.Length)
              {
                for (uint32 = TChar.CastToUInt32(value[num]); Number.IsDigit(uint32); uint32 = TChar.CastToUInt32(value[num]))
                {
                  flag2 = true;
                  ++num;
                  if ((uint) num >= (uint) value.Length)
                    goto label_47;
                }
                goto label_48;
              }
            }
            else
              goto label_48;
          }
label_43:
          if (flag2)
            goto label_47;
label_44:
          result = TInteger.IsSigned ? (flag1 ? -left : left) : left;
          binaryIntegerStyle = Number.ParsingStatus.OK;
          goto label_45;
label_47:
          result = TInteger.Zero;
          binaryIntegerStyle = Number.ParsingStatus.Overflow;
          goto label_45;
label_48:
          if (Number.IsWhite(uint32))
          {
            if ((styles & NumberStyles.AllowTrailingWhite) != NumberStyles.None)
            {
              ++num;
              while (num < value.Length && Number.IsWhite(TChar.CastToUInt32(value[num])))
                ++num;
              if ((uint) num >= (uint) value.Length)
                goto label_43;
            }
            else
              goto label_46;
          }
          if (Number.TrailingZeros<TChar>(value, num))
            goto label_43;
          else
            goto label_46;
        }
        else
          goto label_46;
      }
      else
        goto label_46;
label_45:
      return binaryIntegerStyle;
label_46:
      result = TInteger.Zero;
      binaryIntegerStyle = Number.ParsingStatus.Failed;
      goto label_45;
    }

    internal static Number.ParsingStatus TryParseBinaryIntegerHexNumberStyle<TChar, TInteger>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      out TInteger result)
      where TChar : unmanaged, IUtfChar<TChar>
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      return Number.TryParseBinaryIntegerHexOrBinaryNumberStyle<TChar, TInteger, Number.HexParser<TInteger>>(value, styles, out result);
    }

    private static Number.ParsingStatus TryParseBinaryIntegerHexOrBinaryNumberStyle<TChar, TInteger, TParser>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      out TInteger result)
      where TChar : unmanaged, IUtfChar<TChar>
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
      where TParser : struct, Number.IHexOrBinaryParser<TInteger>
    {
      Number.ParsingStatus binaryNumberStyle;
      if (!value.IsEmpty)
      {
        int index1 = 0;
        uint uint32 = TChar.CastToUInt32(value[0]);
        if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(uint32))
        {
          do
          {
            ++index1;
            if ((uint) index1 < (uint) value.Length)
              uint32 = TChar.CastToUInt32(value[index1]);
            else
              goto label_22;
          }
          while (Number.IsWhite(uint32));
        }
        bool flag = false;
        TInteger integer = TInteger.Zero;
        if (TParser.IsValidChar(uint32))
        {
          if (uint32 == 48U)
          {
            do
            {
              ++index1;
              if ((uint) index1 < (uint) value.Length)
                uint32 = TChar.CastToUInt32(value[index1]);
              else
                goto label_20;
            }
            while (uint32 == 48U);
            if (!TParser.IsValidChar(uint32))
              goto label_24;
          }
          integer = TInteger.CreateTruncating<uint>(TParser.FromChar(uint32));
          ++index1;
          for (int index2 = 0; index2 < TParser.MaxDigitCount - 1; ++index2)
          {
            if ((uint) index1 < (uint) value.Length)
            {
              uint32 = TChar.CastToUInt32(value[index1]);
              uint num = TParser.FromChar(uint32);
              if (num <= TParser.MaxDigitValue)
              {
                ++index1;
                integer = TParser.ShiftLeftForNextDigit(integer) + TInteger.CreateTruncating<uint>(num);
              }
              else
                goto label_24;
            }
            else
              goto label_20;
          }
          if ((uint) index1 < (uint) value.Length)
          {
            uint32 = TChar.CastToUInt32(value[index1]);
            if (TParser.IsValidChar(uint32))
            {
              do
              {
                ++index1;
                if ((uint) index1 < (uint) value.Length)
                  uint32 = TChar.CastToUInt32(value[index1]);
                else
                  goto label_23;
              }
              while (TParser.IsValidChar(uint32));
              flag = true;
              goto label_24;
            }
            else
              goto label_24;
          }
label_20:
          result = integer;
          binaryNumberStyle = Number.ParsingStatus.OK;
          goto label_21;
label_23:
          result = TInteger.Zero;
          binaryNumberStyle = Number.ParsingStatus.Overflow;
          goto label_21;
label_24:
          if (!Number.IsWhite(uint32))
            goto label_30;
          else
            goto label_25;
label_19:
          if (flag)
            goto label_23;
          else
            goto label_20;
label_25:
          if ((styles & NumberStyles.AllowTrailingWhite) != NumberStyles.None)
          {
            ++index1;
            while (index1 < value.Length && Number.IsWhite(TChar.CastToUInt32(value[index1])))
              ++index1;
            if ((uint) index1 >= (uint) value.Length)
              goto label_19;
          }
          else
            goto label_22;
label_30:
          if (Number.TrailingZeros<TChar>(value, index1))
            goto label_19;
          else
            goto label_22;
        }
        else
          goto label_22;
      }
      else
        goto label_22;
label_21:
      return binaryNumberStyle;
label_22:
      result = TInteger.Zero;
      binaryNumberStyle = Number.ParsingStatus.Failed;
      goto label_21;
    }

    internal static Decimal ParseDecimal<TChar>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      Decimal result;
      switch (Number.TryParseDecimal<TChar>(value, styles, info, out result))
      {
        case Number.ParsingStatus.OK:
          return result;
        case Number.ParsingStatus.Failed:
          Number.ThrowFormatException<TChar>(value);
          goto default;
        default:
          Number.ThrowOverflowException(SR.Overflow_Decimal);
          goto case Number.ParsingStatus.OK;
      }
    }

    internal static unsafe bool TryNumberToDecimal(
      ref Number.NumberBuffer number,
      ref Decimal value)
    {
      byte* digitsPointer = number.GetDigitsPointer();
      int scale = number.Scale;
      bool isNegative = number.IsNegative;
      uint num1 = (uint) *digitsPointer;
      if (num1 == 0U)
      {
        value = new Decimal(0, 0, 0, isNegative, (byte) Math.Clamp(-scale, 0, 28));
        return true;
      }
      if (scale > 29)
        return false;
      ulong lo = 0;
      while (scale > -28)
      {
        --scale;
        lo = lo * 10UL + (ulong) (num1 - 48U);
        num1 = (uint) *++digitsPointer;
        if (lo < 1844674407370955161UL)
        {
          if (num1 == 0U)
          {
            while (scale > 0)
            {
              --scale;
              lo *= 10UL;
              if (lo >= 1844674407370955161UL)
                break;
            }
            break;
          }
        }
        else
          break;
      }
      uint hi;
      for (hi = 0U; (scale > 0 || num1 != 0U && scale > -28) && (hi < 429496729U || hi == 429496729U && (lo < 11068046444225730969UL || lo == 11068046444225730969UL && num1 <= 53U)); --scale)
      {
        ulong num2 = (ulong) (uint) lo * 10UL;
        ulong num3 = (ulong) (uint) (lo >> 32) * 10UL + (num2 >> 32);
        lo = (ulong) (uint) num2 + (num3 << 32);
        hi = (uint) (num3 >> 32) + hi * 10U;
        if (num1 != 0U)
        {
          uint num4 = num1 - 48U;
          lo += (ulong) num4;
          if (lo < (ulong) num4)
            ++hi;
          num1 = (uint) *++digitsPointer;
        }
      }
      if (num1 >= 53U)
      {
        if (num1 == 53U && ((long) lo & 1L) == 0L)
        {
          byte* numPtr;
          uint num5 = (uint) *(numPtr = digitsPointer + 1);
          bool flag;
          for (flag = !number.HasNonZeroTail; num5 > 0U & flag; num5 = (uint) *++numPtr)
            flag &= num5 == 48U;
          if (flag)
            goto label_25;
        }
        if (++lo == 0UL && ++hi == 0U)
        {
          lo = 11068046444225730970UL;
          hi = 429496729U;
          ++scale;
        }
      }
label_25:
      if (scale > 0)
        return false;
      value = scale > -29 ? new Decimal((int) lo, (int) (lo >> 32), (int) hi, isNegative, (byte) -scale) : new Decimal(0, 0, 0, isNegative, (byte) 28);
      return true;
    }

    internal static TFloat ParseFloat<TChar, TFloat>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
      where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      TFloat result;
      if (!Number.TryParseFloat<TChar, TFloat>(value, styles, info, out result))
        Number.ThrowFormatException<TChar>(value);
      return result;
    }

    internal static Number.ParsingStatus TryParseDecimal<TChar>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info,
      out Decimal result)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.Decimal, stackalloc byte[31]);
      result = 0M;
      if (!Number.TryStringToNumber<TChar>(value, styles, ref number, info))
        return Number.ParsingStatus.Failed;
      return !Number.TryNumberToDecimal(ref number, ref result) ? Number.ParsingStatus.Overflow : Number.ParsingStatus.OK;
    }

    internal static bool SpanStartsWith<TChar>(ReadOnlySpan<TChar> span, TChar c) where TChar : unmanaged, IUtfChar<TChar>
    {
      return !span.IsEmpty && span[0] == c;
    }

    internal static bool SpanStartsWith<TChar>(
      ReadOnlySpan<TChar> span,
      ReadOnlySpan<TChar> value,
      StringComparison comparisonType)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      return typeof (TChar) == typeof (char) ? MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.As<TChar, char>(ref MemoryMarshal.GetReference<TChar>(span)), span.Length).StartsWith(MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.As<TChar, char>(ref MemoryMarshal.GetReference<TChar>(value)), value.Length), comparisonType) : MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.As<TChar, byte>(ref MemoryMarshal.GetReference<TChar>(span)), span.Length).StartsWithUtf8(MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.As<TChar, byte>(ref MemoryMarshal.GetReference<TChar>(value)), value.Length), comparisonType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<TChar> SpanTrim<TChar>(ReadOnlySpan<TChar> span) where TChar : unmanaged, IUtfChar<TChar>
    {
      if (typeof (TChar) == typeof (char))
      {
        ReadOnlySpan<char> span1 = MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.As<TChar, char>(ref MemoryMarshal.GetReference<TChar>(span)), span.Length).Trim();
        return MemoryMarshal.CreateReadOnlySpan<TChar>(ref Unsafe.As<char, TChar>(ref MemoryMarshal.GetReference<char>(span1)), span1.Length);
      }
      ReadOnlySpan<byte> span2 = MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.As<TChar, byte>(ref MemoryMarshal.GetReference<TChar>(span)), span.Length).TrimUtf8();
      return MemoryMarshal.CreateReadOnlySpan<TChar>(ref Unsafe.As<byte, TChar>(ref MemoryMarshal.GetReference<byte>(span2)), span2.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool SpanEqualsOrdinalIgnoreCase<TChar>(
      ReadOnlySpan<TChar> span,
      ReadOnlySpan<TChar> value)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      return typeof (TChar) == typeof (char) ? MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.As<TChar, char>(ref MemoryMarshal.GetReference<TChar>(span)), span.Length).EqualsOrdinalIgnoreCase(MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.As<TChar, char>(ref MemoryMarshal.GetReference<TChar>(value)), value.Length)) : MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.As<TChar, byte>(ref MemoryMarshal.GetReference<TChar>(span)), span.Length).EqualsOrdinalIgnoreCaseUtf8(MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.As<TChar, byte>(ref MemoryMarshal.GetReference<TChar>(value)), value.Length));
    }

    internal static bool TryParseFloat<TChar, TFloat>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      NumberFormatInfo info,
      out TFloat result)
      where TChar : unmanaged, IUtfChar<TChar>
      where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      Number.NumberBuffer number = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, stackalloc byte[TFloat.NumberBufferLength]);
      if (!Number.TryStringToNumber<TChar>(value, styles, ref number, info))
      {
        ReadOnlySpan<TChar> span = Number.SpanTrim<TChar>(value);
        ReadOnlySpan<TChar> readOnlySpan1 = info.PositiveInfinitySymbolTChar<TChar>();
        if (Number.SpanEqualsOrdinalIgnoreCase<TChar>(span, readOnlySpan1))
        {
          result = TFloat.PositiveInfinity;
          return true;
        }
        if (Number.SpanEqualsOrdinalIgnoreCase<TChar>(span, info.NegativeInfinitySymbolTChar<TChar>()))
        {
          result = TFloat.NegativeInfinity;
          return true;
        }
        ReadOnlySpan<TChar> readOnlySpan2 = info.NaNSymbolTChar<TChar>();
        if (Number.SpanEqualsOrdinalIgnoreCase<TChar>(span, readOnlySpan2))
        {
          result = TFloat.NaN;
          return true;
        }
        ReadOnlySpan<TChar> readOnlySpan3 = info.PositiveSignTChar<TChar>();
        if (Number.SpanStartsWith<TChar>(span, readOnlySpan3, StringComparison.OrdinalIgnoreCase))
        {
          span = span.Slice(readOnlySpan3.Length);
          if (Number.SpanEqualsOrdinalIgnoreCase<TChar>(span, readOnlySpan1))
          {
            result = TFloat.PositiveInfinity;
            return true;
          }
          if (Number.SpanEqualsOrdinalIgnoreCase<TChar>(span, readOnlySpan2))
          {
            result = TFloat.NaN;
            return true;
          }
          result = TFloat.Zero;
          return false;
        }
        ReadOnlySpan<TChar> readOnlySpan4 = info.NegativeSignTChar<TChar>();
        if (Number.SpanStartsWith<TChar>(span, readOnlySpan4, StringComparison.OrdinalIgnoreCase))
        {
          if (Number.SpanEqualsOrdinalIgnoreCase<TChar>(span.Slice(readOnlySpan4.Length), readOnlySpan2))
          {
            result = TFloat.NaN;
            return true;
          }
          if (info.AllowHyphenDuringParsing && Number.SpanStartsWith<TChar>(span, TChar.CastFrom('-')) && Number.SpanEqualsOrdinalIgnoreCase<TChar>(span.Slice(1), readOnlySpan2))
          {
            result = TFloat.NaN;
            return true;
          }
        }
        result = TFloat.Zero;
        return false;
      }
      result = Number.NumberToFloat<TFloat>(ref number);
      return true;
    }

    internal static unsafe bool TryStringToNumber<TChar>(
      ReadOnlySpan<TChar> value,
      NumberStyles styles,
      ref Number.NumberBuffer number,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      fixed (TChar* charPtr = &MemoryMarshal.GetReference<TChar>(value))
      {
        TChar* str = charPtr;
        if (!Number.TryParseNumber<TChar>(ref str, str + value.Length, styles, ref number, info) || (int) (str - charPtr) < value.Length && !Number.TrailingZeros<TChar>(value, (int) (str - charPtr)))
          return false;
      }
      return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool TrailingZeros<TChar>(ReadOnlySpan<TChar> value, int index) where TChar : unmanaged, IUtfChar<TChar>
    {
      return !value.Slice(index).ContainsAnyExcept<TChar>(TChar.CastFrom(char.MinValue));
    }

    private static bool IsSpaceReplacingChar(uint c) => c == 160U || c == 8239U;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe TChar* MatchNegativeSignChars<TChar>(
      TChar* p,
      TChar* pEnd,
      NumberFormatInfo info)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      TChar* charPtr = Number.MatchChars<TChar>(p, pEnd, info.NegativeSignTChar<TChar>());
      if ((IntPtr) charPtr == IntPtr.Zero && info.AllowHyphenDuringParsing && p < pEnd && TChar.CastToUInt32(*p) == 45U)
        charPtr = p + 1;
      return charPtr;
    }

    private static unsafe TChar* MatchChars<TChar>(
      TChar* p,
      TChar* pEnd,
      ReadOnlySpan<TChar> value)
      where TChar : unmanaged, IUtfChar<TChar>
    {
      fixed (TChar* charPtr1 = &MemoryMarshal.GetReference<TChar>(value))
      {
        TChar* charPtr2 = charPtr1;
        if (TChar.CastToUInt32(*charPtr2) != 0U)
        {
          do
          {
            uint uint32_1 = p < pEnd ? TChar.CastToUInt32(*p) : 0U;
            uint uint32_2 = TChar.CastToUInt32(*charPtr2);
            if ((int) uint32_1 == (int) uint32_2 || Number.IsSpaceReplacingChar(uint32_2) && uint32_1 == 32U)
            {
              ++p;
              ++charPtr2;
            }
            else
              goto label_4;
          }
          while (TChar.CastToUInt32(*charPtr2) != 0U);
          return p;
        }
label_4:;
      }
      return (TChar*) null;
    }

    private static bool IsWhite(uint ch) => ch == 32U || ch - 9U <= 4U;

    private static bool IsDigit(uint ch) => ch - 48U <= 9U;

    [DoesNotReturn]
    internal static void ThrowOverflowOrFormatException<TChar, TInteger>(
      Number.ParsingStatus status,
      ReadOnlySpan<TChar> value)
      where TChar : unmanaged, IUtfChar<TChar>
      where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      if (status == Number.ParsingStatus.Failed)
        Number.ThrowFormatException<TChar>(value);
      Number.ThrowOverflowException<TInteger>();
    }

    [DoesNotReturn]
    internal static void ThrowFormatException<TChar>(ReadOnlySpan<TChar> value) where TChar : unmanaged, IUtfChar<TChar>
    {
      throw new FormatException(SR.Format(SR.Format_InvalidStringWithValue, (object) value.ToString()));
    }

    [DoesNotReturn]
    internal static void ThrowOverflowException<TInteger>() where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      throw new OverflowException(TInteger.OverflowMessage);
    }

    [DoesNotReturn]
    internal static void ThrowOverflowException(string message)
    {
      throw new OverflowException(message);
    }

    internal static TFloat NumberToFloat<TFloat>(ref Number.NumberBuffer number) where TFloat : unmanaged, IBinaryFloatParseAndFormatInfo<TFloat>
    {
      TFloat @float = number.DigitsCount == 0 || number.Scale < TFloat.MinDecimalExponent ? TFloat.Zero : (number.Scale <= TFloat.MaxDecimalExponent ? TFloat.BitsToFloat(Number.NumberToFloatingPointBits<TFloat>(ref number)) : TFloat.PositiveInfinity);
      return !number.IsNegative ? @float : -@float;
    }

    [CompilerFeatureRequired("RefStructs")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal ref struct BigInteger
    {
      private int _length;
      private unsafe fixed uint _blocks[116];

      private static ReadOnlySpan<uint> Pow10UInt32Table
      {
        get
        {
          return RuntimeHelpers.CreateSpan<uint>(__fieldref (\u003CPrivateImplementationDetails\u003E.A516EECB41051151F0183A8B0B6F6693C43F7D9E1815F85CAAAB18E00A5269A24));
        }
      }

      private static ReadOnlySpan<int> Pow10BigNumTableIndices
      {
        get
        {
          return RuntimeHelpers.CreateSpan<int>(__fieldref (\u003CPrivateImplementationDetails\u003E.\u003872CF31969B30D16D8B7FD68ABCEBFD7F8F3336BA347CD8712D80E58CB1EB6674));
        }
      }

      private static ReadOnlySpan<uint> Pow10BigNumTable
      {
        get
        {
          return RuntimeHelpers.CreateSpan<uint>(__fieldref (\u003CPrivateImplementationDetails\u003E.BAB9BE2886696BD36593C4F3A85B4FA59F85A673FE44AB7EBB4F314165F9B6F14));
        }
      }

      public static unsafe void Add(
        [ScopedRef] ref Number.BigInteger lhs,
        [ScopedRef] ref Number.BigInteger rhs,
        out Number.BigInteger result)
      {
        ref Number.BigInteger local1 = ref (lhs._length < rhs._length ? ref rhs : ref lhs);
        ref Number.BigInteger local2 = ref (lhs._length < rhs._length ? ref lhs : ref rhs);
        int length1 = local1._length;
        int length2 = local2._length;
        result._length = length1;
        ulong num1 = 0;
        int index1 = 0;
        int index2 = 0;
        int index3 = 0;
        while (index2 < length2)
        {
          ulong num2 = num1 + (ulong) local1._blocks[index1] + (ulong) local2._blocks[index2];
          num1 = num2 >> 32;
          result._blocks[index3] = (uint) num2;
          ++index1;
          ++index2;
          ++index3;
        }
        while (index1 < length1)
        {
          ulong num3 = num1 + (ulong) local1._blocks[index1];
          num1 = num3 >> 32;
          result._blocks[index3] = (uint) num3;
          ++index1;
          ++index3;
        }
        int num4 = length1;
        if (num1 == 0UL)
          return;
        if ((uint) num4 >= 116U)
        {
          Number.BigInteger.SetZero(out result);
        }
        else
        {
          result._blocks[index3] = 1U;
          ++result._length;
        }
      }

      public static unsafe int Compare([ScopedRef] ref Number.BigInteger lhs, [ScopedRef] ref Number.BigInteger rhs)
      {
        int length1 = lhs._length;
        int length2 = rhs._length;
        int num1 = length1 - length2;
        if (num1 != 0)
          return num1;
        if (length1 == 0)
          return 0;
        for (int index = length1 - 1; index >= 0; --index)
        {
          long num2 = (long) lhs._blocks[index] - (long) rhs._blocks[index];
          if (num2 != 0L)
            return num2 <= 0L ? -1 : 1;
        }
        return 0;
      }

      public static uint CountSignificantBits(uint value)
      {
        return (uint) (32 - BitOperations.LeadingZeroCount(value));
      }

      public static uint CountSignificantBits(ulong value)
      {
        return (uint) (64 - BitOperations.LeadingZeroCount(value));
      }

      public static unsafe uint CountSignificantBits(ref Number.BigInteger value)
      {
        if (value.IsZero())
          return 0;
        uint num = (uint) (value._length - 1);
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        return num * 32U + Number.BigInteger.CountSignificantBits(^(uint&) ((IntPtr) value._blocks + (IntPtr) ((long) num * 4L)));
      }

      public static unsafe void DivRem(
        [ScopedRef] ref Number.BigInteger lhs,
        [ScopedRef] ref Number.BigInteger rhs,
        out Number.BigInteger quo,
        out Number.BigInteger rem)
      {
        if (lhs.IsZero())
        {
          Number.BigInteger.SetZero(out quo);
          Number.BigInteger.SetZero(out rem);
        }
        else
        {
          int length1 = lhs._length;
          int length2 = rhs._length;
          if (length1 == 1 && length2 == 1)
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            (uint Quotient, uint Remainder) = Math.DivRem(lhs._blocks.FixedElementField, rhs._blocks.FixedElementField);
            Number.BigInteger.SetUInt32(out quo, Quotient);
            Number.BigInteger.SetUInt32(out rem, Remainder);
          }
          else if (length2 == 1)
          {
            int num = length1;
            // ISSUE: reference to a compiler-generated field
            ulong fixedElementField = (ulong) rhs._blocks.FixedElementField;
            ulong Remainder = 0;
            for (int index = num - 1; index >= 0; --index)
            {
              ulong Quotient;
              (Quotient, Remainder) = Math.DivRem(Remainder << 32 | (ulong) lhs._blocks[index], fixedElementField);
              if (Quotient == 0UL && index == num - 1)
                --num;
              else
                quo._blocks[index] = (uint) Quotient;
            }
            quo._length = num;
            Number.BigInteger.SetUInt32(out rem, (uint) Remainder);
          }
          else if (length2 > length1)
          {
            Number.BigInteger.SetZero(out quo);
            Number.BigInteger.SetValue(out rem, ref lhs);
          }
          else
          {
            int num1 = length1 - length2 + 1;
            Number.BigInteger.SetValue(out rem, ref lhs);
            int num2 = length1;
            uint divHi = rhs._blocks[length2 - 1];
            uint divLo = rhs._blocks[length2 - 2];
            int num3 = BitOperations.LeadingZeroCount(divHi);
            int num4 = 32 - num3;
            if (num3 > 0)
            {
              divHi = divHi << num3 | divLo >> num4;
              divLo <<= num3;
              if (length2 > 2)
                divLo |= rhs._blocks[length2 - 3] >> num4;
            }
            for (int index = length1; index >= length2; --index)
            {
              int lhsStartIndex = index - length2;
              uint num5 = index < length1 ? rem._blocks[index] : 0U;
              ulong valHi = (ulong) num5 << 32 | (ulong) rem._blocks[index - 1];
              uint valLo = index > 1 ? rem._blocks[index - 2] : 0U;
              if (num3 > 0)
              {
                valHi = valHi << num3 | (ulong) (valLo >> num4);
                valLo <<= num3;
                if (index > 2)
                  valLo |= rem._blocks[index - 3] >> num4;
              }
              ulong q = valHi / (ulong) divHi;
              if (q > (ulong) uint.MaxValue)
                q = (ulong) uint.MaxValue;
              while (Number.BigInteger.DivideGuessTooBig(q, valHi, valLo, divHi, divLo))
                --q;
              if (q > 0UL && (int) Number.BigInteger.SubtractDivisor(ref rem, lhsStartIndex, ref rhs, q) != (int) num5)
              {
                Number.BigInteger.AddDivisor(ref rem, lhsStartIndex, ref rhs);
                --q;
              }
              if (num1 != 0)
              {
                if (q == 0UL && lhsStartIndex == num1 - 1)
                  --num1;
                else
                  quo._blocks[lhsStartIndex] = (uint) q;
              }
              if (index < num2)
                --num2;
            }
            quo._length = num1;
            for (int index = num2 - 1; index >= 0 && rem._blocks[index] == 0U; --index)
              --num2;
            rem._length = num2;
          }
        }
      }

      public static unsafe uint HeuristicDivide(
        ref Number.BigInteger dividend,
        ref Number.BigInteger divisor)
      {
        int length = divisor._length;
        if (dividend._length < length)
          return 0;
        int index1 = length - 1;
        uint num1 = dividend._blocks[index1] / (divisor._blocks[index1] + 1U);
        if (num1 != 0U)
        {
          int index2 = 0;
          ulong num2 = 0;
          ulong num3 = 0;
          do
          {
            ulong num4 = (ulong) divisor._blocks[index2] * (ulong) num1 + num3;
            num3 = num4 >> 32;
            ulong num5 = (ulong) dividend._blocks[index2] - (ulong) (uint) num4 - num2;
            num2 = num5 >> 32 & 1UL;
            dividend._blocks[index2] = (uint) num5;
            ++index2;
          }
          while (index2 < length);
          while (length > 0 && dividend._blocks[length - 1] == 0U)
            --length;
          dividend._length = length;
        }
        if (Number.BigInteger.Compare(ref dividend, ref divisor) >= 0)
        {
          ++num1;
          int index3 = 0;
          ulong num6 = 0;
          do
          {
            ulong num7 = (ulong) dividend._blocks[index3] - (ulong) divisor._blocks[index3] - num6;
            num6 = num7 >> 32 & 1UL;
            dividend._blocks[index3] = (uint) num7;
            ++index3;
          }
          while (index3 < length);
          while (length > 0 && dividend._blocks[length - 1] == 0U)
            --length;
          dividend._length = length;
        }
        return num1;
      }

      public static unsafe void Multiply(
        [ScopedRef] ref Number.BigInteger lhs,
        uint value,
        out Number.BigInteger result)
      {
        if (lhs._length <= 1)
        {
          Number.BigInteger.SetUInt64(out result, (ulong) lhs.ToUInt32() * (ulong) value);
        }
        else
        {
          switch (value)
          {
            case 0:
              Number.BigInteger.SetZero(out result);
              break;
            case 1:
              Number.BigInteger.SetValue(out result, ref lhs);
              break;
            default:
              int length = lhs._length;
              int index = 0;
              uint num1 = 0;
              for (; index < length; ++index)
              {
                ulong num2 = (ulong) lhs._blocks[index] * (ulong) value + (ulong) num1;
                result._blocks[index] = (uint) num2;
                num1 = (uint) (num2 >> 32);
              }
              int num3 = length;
              if (num1 != 0U)
              {
                if ((uint) num3 >= 116U)
                {
                  Number.BigInteger.SetZero(out result);
                  break;
                }
                result._blocks[index] = num1;
                ++num3;
              }
              result._length = num3;
              break;
          }
        }
      }

      public static unsafe void Multiply(
        [ScopedRef] ref Number.BigInteger lhs,
        [ScopedRef] ref Number.BigInteger rhs,
        out Number.BigInteger result)
      {
        if (lhs._length <= 1)
          Number.BigInteger.Multiply(ref rhs, lhs.ToUInt32(), out result);
        else if (rhs._length <= 1)
        {
          Number.BigInteger.Multiply(ref lhs, rhs.ToUInt32(), out result);
        }
        else
        {
          ref Number.BigInteger local1 = ref lhs;
          int length1 = lhs._length;
          ref Number.BigInteger local2 = ref rhs;
          int length2 = rhs._length;
          if (length1 < length2)
          {
            local1 = ref rhs;
            length1 = rhs._length;
            local2 = ref lhs;
            length2 = lhs._length;
          }
          int length3 = length2 + length1;
          if ((uint) length3 > 116U)
          {
            Number.BigInteger.SetZero(out result);
          }
          else
          {
            result._length = length3;
            result.Clear((uint) length3);
            int index1 = 0;
            int num1 = 0;
            while (index1 < length2)
            {
              if (local2._blocks[index1] != 0U)
              {
                int index2 = 0;
                int index3 = num1;
                ulong num2 = 0;
                do
                {
                  ulong num3 = (ulong) result._blocks[index3] + (ulong) local2._blocks[index1] * (ulong) local1._blocks[index2] + num2;
                  num2 = num3 >> 32;
                  result._blocks[index3] = (uint) num3;
                  ++index3;
                  ++index2;
                }
                while (index2 < length1);
                result._blocks[index3] = (uint) num2;
              }
              ++index1;
              ++num1;
            }
            if (length3 <= 0 || result._blocks[length3 - 1] != 0U)
              return;
            --result._length;
          }
        }
      }

      public static unsafe void Pow2(uint exponent, out Number.BigInteger result)
      {
        uint remainder;
        uint length = Number.BigInteger.DivRem32(exponent, out remainder);
        result._length = (int) length + 1;
        if ((uint) result._length > 116U)
        {
          Number.BigInteger.SetZero(out result);
        }
        else
        {
          if (length > 0U)
            result.Clear(length);
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(int&) ((IntPtr) result._blocks + (IntPtr) ((long) length * 4L)) = 1 << (int) remainder;
        }
      }

      public static unsafe void Pow10(uint exponent, out Number.BigInteger result)
      {
        Number.BigInteger result1;
        Number.BigInteger.SetUInt32(out result1, Number.BigInteger.Pow10UInt32Table[(int) exponent & 7]);
        ref Number.BigInteger local1 = ref result1;
        Number.BigInteger result2;
        Number.BigInteger.SetZero(out result2);
        ref Number.BigInteger local2 = ref result2;
        exponent >>= 3;
        uint index = 0;
        for (; exponent != 0U; exponent >>= 1)
        {
          if (((int) exponent & 1) != 0)
          {
            fixed (uint* rhs = &Number.BigInteger.Pow10BigNumTable[Number.BigInteger.Pow10BigNumTableIndices[(int) index]])
            {
              // ISSUE: cast to a reference type
              Number.BigInteger.Multiply(ref local1, (Number.BigInteger&) rhs, out local2);
            }
            ref Number.BigInteger local3 = ref local2;
            local2 = ref local1;
            local1 = ref local3;
          }
          ++index;
        }
        Number.BigInteger.SetValue(out result, ref local1);
      }

      private static unsafe uint AddDivisor(
        ref Number.BigInteger lhs,
        int lhsStartIndex,
        ref Number.BigInteger rhs)
      {
        int length1 = lhs._length;
        int length2 = rhs._length;
        ulong num1 = 0;
        for (int index = 0; index < length2; ++index)
        {
          // ISSUE: explicit reference operation
          ref uint local = @lhs._blocks[lhsStartIndex + index];
          ulong num2 = (ulong) local + num1 + (ulong) rhs._blocks[index];
          local = (uint) num2;
          num1 = num2 >> 32;
        }
        return (uint) num1;
      }

      private static bool DivideGuessTooBig(
        ulong q,
        ulong valHi,
        uint valLo,
        uint divHi,
        uint divLo)
      {
        ulong num1 = (ulong) divHi * q;
        ulong num2 = (ulong) divLo * q;
        ulong num3 = num1 + (num2 >> 32);
        ulong num4 = num2 & (ulong) uint.MaxValue;
        return num3 >= valHi && (num3 > valHi || num4 >= (ulong) valLo && num4 > (ulong) valLo);
      }

      private static unsafe uint SubtractDivisor(
        ref Number.BigInteger lhs,
        int lhsStartIndex,
        ref Number.BigInteger rhs,
        ulong q)
      {
        int num1 = lhs._length - lhsStartIndex;
        int length = rhs._length;
        ulong num2 = 0;
        for (int index = 0; index < length; ++index)
        {
          ulong num3 = num2 + (ulong) rhs._blocks[index] * q;
          uint num4 = (uint) num3;
          num2 = num3 >> 32;
          // ISSUE: explicit reference operation
          ref uint local = @lhs._blocks[lhsStartIndex + index];
          if (local < num4)
            ++num2;
          local -= num4;
        }
        return (uint) num2;
      }

      public unsafe void Add(uint value)
      {
        int length = this._length;
        if (length == 0)
        {
          Number.BigInteger.SetUInt32(out this, value);
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          this._blocks.FixedElementField += value;
          // ISSUE: reference to a compiler-generated field
          if (this._blocks.FixedElementField >= value)
            return;
          for (int index = 1; index < length; ++index)
          {
            ++this._blocks[index];
            if (this._blocks[index] > 0U)
              return;
          }
          if ((uint) length >= 116U)
          {
            Number.BigInteger.SetZero(out this);
          }
          else
          {
            this._blocks[length] = 1U;
            this._length = length + 1;
          }
        }
      }

      public unsafe uint GetBlock(uint index)
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        return ^(uint&) ((IntPtr) this._blocks + (IntPtr) ((long) index * 4L));
      }

      public int GetLength() => this._length;

      public bool IsZero() => this._length == 0;

      public void Multiply(uint value) => Number.BigInteger.Multiply(ref this, value, out this);

      public void Multiply([ScopedRef] ref Number.BigInteger value)
      {
        if (value._length <= 1)
        {
          Number.BigInteger.Multiply(ref this, value.ToUInt32(), out this);
        }
        else
        {
          Number.BigInteger result;
          Number.BigInteger.SetValue(out result, ref this);
          Number.BigInteger.Multiply(ref result, ref value, out this);
        }
      }

      public unsafe void Multiply10()
      {
        if (this.IsZero())
          return;
        int index = 0;
        int length = this._length;
        ulong num1 = 0;
        do
        {
          ulong num2 = (ulong) this._blocks[index];
          ulong num3 = (ulong) (((long) num2 << 3) + ((long) num2 << 1)) + num1;
          num1 = num3 >> 32;
          this._blocks[index] = (uint) num3;
          ++index;
        }
        while (index < length);
        if (num1 == 0UL)
          return;
        if ((uint) length >= 116U)
        {
          Number.BigInteger.SetZero(out this);
        }
        else
        {
          this._blocks[index] = (uint) num1;
          this._length = length + 1;
        }
      }

      public void MultiplyPow10(uint exponent)
      {
        if (exponent <= 9U)
        {
          this.Multiply(Number.BigInteger.Pow10UInt32Table[(int) exponent]);
        }
        else
        {
          if (this.IsZero())
            return;
          Number.BigInteger result;
          Number.BigInteger.Pow10(exponent, out result);
          this.Multiply(ref result);
        }
      }

      public static unsafe void SetUInt32(out Number.BigInteger result, uint value)
      {
        if (value == 0U)
        {
          Number.BigInteger.SetZero(out result);
        }
        else
        {
          result._blocks[0] = value;
          result._length = 1;
        }
      }

      public static unsafe void SetUInt64(out Number.BigInteger result, ulong value)
      {
        if (value <= (ulong) uint.MaxValue)
        {
          Number.BigInteger.SetUInt32(out result, (uint) value);
        }
        else
        {
          result._blocks[0] = (uint) value;
          result._blocks[1] = (uint) (value >> 32);
          result._length = 2;
        }
      }

      public static unsafe void SetValue(out Number.BigInteger result, [ScopedRef] ref Number.BigInteger value)
      {
        int length = value._length;
        result._length = length;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        Buffer.Memmove<uint>(ref result._blocks.FixedElementField, ref value._blocks.FixedElementField, (UIntPtr) length);
      }

      public static void SetZero(out Number.BigInteger result) => result._length = 0;

      public unsafe void ShiftLeft(uint shift)
      {
        int length1 = this._length;
        if (length1 == 0 || shift == 0U)
          return;
        uint remainder;
        uint length2 = Number.BigInteger.DivRem32(shift, out remainder);
        int index1 = length1 - 1;
        int index2 = index1 + (int) length2;
        if (remainder == 0U)
        {
          if ((uint) length1 >= 116U)
          {
            Number.BigInteger.SetZero(out this);
          }
          else
          {
            while (index1 >= 0)
            {
              this._blocks[index2] = this._blocks[index1];
              --index1;
              --index2;
            }
            this._length += (int) length2;
            this.Clear(length2);
          }
        }
        else
        {
          int index3 = index2 + 1;
          if ((uint) length1 >= 116U)
          {
            Number.BigInteger.SetZero(out this);
          }
          else
          {
            this._length = index3 + 1;
            uint num1 = 32U - remainder;
            uint num2 = 0;
            uint num3 = this._blocks[index1];
            uint num4 = num3 >> (int) num1;
            while (index1 > 0)
            {
              this._blocks[index3] = num2 | num4;
              num2 = num3 << (int) remainder;
              --index1;
              --index3;
              num3 = this._blocks[index1];
              num4 = num3 >> (int) num1;
            }
            this._blocks[index3] = num2 | num4;
            this._blocks[index3 - 1] = num3 << (int) remainder;
            this.Clear(length2);
            if (this._blocks[this._length - 1] != 0U)
              return;
            --this._length;
          }
        }
      }

      public unsafe uint ToUInt32() => this._length > 0 ? this._blocks.FixedElementField : 0U;

      public unsafe ulong ToUInt64()
      {
        if (this._length > 1)
        {
          // ISSUE: reference to a compiler-generated field
          return ((ulong) this._blocks[1] << 32) + (ulong) this._blocks.FixedElementField;
        }
        // ISSUE: reference to a compiler-generated field
        return this._length > 0 ? (ulong) this._blocks.FixedElementField : 0UL;
      }

      private unsafe void Clear(uint length)
      {
        // ISSUE: reference to a compiler-generated field
        NativeMemory.Clear(Unsafe.AsPointer<uint>(ref this._blocks.FixedElementField), (UIntPtr) (length * 4U));
      }

      private static uint DivRem32(uint value, out uint remainder)
      {
        remainder = value & 31U;
        return value >> 5;
      }
    }

    [CompilerFeatureRequired("RefStructs")]
    internal readonly ref struct DiyFp
    {
      public readonly ulong f;
      public readonly int e;

      public static Number.DiyFp CreateAndGetBoundaries(
        double value,
        out Number.DiyFp mMinus,
        out Number.DiyFp mPlus)
      {
        Number.DiyFp andGetBoundaries = new Number.DiyFp(value);
        andGetBoundaries.GetBoundaries(52, out mMinus, out mPlus);
        return andGetBoundaries;
      }

      public static Number.DiyFp CreateAndGetBoundaries(
        float value,
        out Number.DiyFp mMinus,
        out Number.DiyFp mPlus)
      {
        Number.DiyFp andGetBoundaries = new Number.DiyFp(value);
        andGetBoundaries.GetBoundaries(23, out mMinus, out mPlus);
        return andGetBoundaries;
      }

      public static Number.DiyFp CreateAndGetBoundaries(
        Half value,
        out Number.DiyFp mMinus,
        out Number.DiyFp mPlus)
      {
        Number.DiyFp andGetBoundaries = new Number.DiyFp(value);
        andGetBoundaries.GetBoundaries(10, out mMinus, out mPlus);
        return andGetBoundaries;
      }

      public DiyFp(double value)
      {
        this.f = Number.ExtractFractionAndBiasedExponent(value, out this.e);
      }

      public DiyFp(float value)
      {
        this.f = (ulong) Number.ExtractFractionAndBiasedExponent(value, out this.e);
      }

      public DiyFp(Half value)
      {
        this.f = (ulong) Number.ExtractFractionAndBiasedExponent(value, out this.e);
      }

      public DiyFp(ulong f, int e)
      {
        this.f = f;
        this.e = e;
      }

      public Number.DiyFp Multiply(in Number.DiyFp other)
      {
        uint num1 = (uint) (this.f >> 32);
        uint f1 = (uint) this.f;
        uint num2 = (uint) (other.f >> 32);
        uint f2 = (uint) other.f;
        ulong num3 = (ulong) num1 * (ulong) num2;
        ulong num4 = (ulong) f1 * (ulong) num2;
        ulong num5 = (ulong) num1 * (ulong) f2;
        ulong num6 = ((ulong) f1 * (ulong) f2 >> 32) + (ulong) (uint) num5 + (ulong) (uint) num4 + 2147483648UL;
        return new Number.DiyFp(num3 + (num5 >> 32) + (num4 >> 32) + (num6 >> 32), this.e + other.e + 64);
      }

      public Number.DiyFp Normalize()
      {
        int num = BitOperations.LeadingZeroCount(this.f);
        return new Number.DiyFp(this.f << num, this.e - num);
      }

      public Number.DiyFp Subtract(in Number.DiyFp other)
      {
        return new Number.DiyFp(this.f - other.f, this.e);
      }

      private void GetBoundaries(
        int implicitBitIndex,
        out Number.DiyFp mMinus,
        out Number.DiyFp mPlus)
      {
        mPlus = new Number.DiyFp((this.f << 1) + 1UL, this.e - 1).Normalize();
        mMinus = (long) this.f != 1L << implicitBitIndex ? new Number.DiyFp((this.f << 1) - 1UL, this.e - 1) : new Number.DiyFp((this.f << 2) - 1UL, this.e - 2);
        mMinus = new Number.DiyFp(mMinus.f << mMinus.e - mPlus.e, mPlus.e);
      }
    }

    internal static class Grisu3
    {
      private static ReadOnlySpan<short> CachedPowersBinaryExponent
      {
        get
        {
          return RuntimeHelpers.CreateSpan<short>(__fieldref (\u003CPrivateImplementationDetails\u003E.\u003538F052AB907338D0E8980BC5D8AD76919B39F0248ACDFAFAAA0CC76E39948F72));
        }
      }

      private static ReadOnlySpan<short> CachedPowersDecimalExponent
      {
        get
        {
          return RuntimeHelpers.CreateSpan<short>(__fieldref (\u003CPrivateImplementationDetails\u003E.B2DCA9FD613841289369C721661A31B454A090D2146EFE106203F7821567907D2));
        }
      }

      private static ReadOnlySpan<ulong> CachedPowersSignificand
      {
        get
        {
          return RuntimeHelpers.CreateSpan<ulong>(__fieldref (\u003CPrivateImplementationDetails\u003E.\u00302BF302F66F50150BCF5E322DA879E92E417084D14FBE4F5345DDCB68F863E518));
        }
      }

      private static ReadOnlySpan<uint> SmallPowersOfTen
      {
        get
        {
          return RuntimeHelpers.CreateSpan<uint>(__fieldref (\u003CPrivateImplementationDetails\u003E.A516EECB41051151F0183A8B0B6F6693C43F7D9E1815F85CAAAB18E00A5269A24));
        }
      }

      public static bool TryRunDouble(
        double value,
        int requestedDigits,
        ref Number.NumberBuffer number)
      {
        double num = double.IsNegative(value) ? -value : value;
        int length;
        int decimalExponent;
        bool flag;
        if (requestedDigits == -1)
        {
          Number.DiyFp boundaryMinus;
          Number.DiyFp boundaryPlus;
          Number.DiyFp w = Number.DiyFp.CreateAndGetBoundaries(num, out boundaryMinus, out boundaryPlus).Normalize();
          flag = Number.Grisu3.TryRunShortest(in boundaryMinus, in w, in boundaryPlus, number.Digits, out length, out decimalExponent);
        }
        else
          flag = Number.Grisu3.TryRunCounted(new Number.DiyFp(num).Normalize(), requestedDigits, number.Digits, out length, out decimalExponent);
        if (flag)
        {
          number.Scale = length + decimalExponent;
          number.Digits[length] = (byte) 0;
          number.DigitsCount = length;
        }
        return flag;
      }

      public static bool TryRunHalf(
        Half value,
        int requestedDigits,
        ref Number.NumberBuffer number)
      {
        Half half = Half.IsNegative(value) ? Half.Negate(value) : value;
        int length;
        int decimalExponent;
        bool flag;
        if (requestedDigits == -1)
        {
          Number.DiyFp boundaryMinus;
          Number.DiyFp boundaryPlus;
          Number.DiyFp w = Number.DiyFp.CreateAndGetBoundaries(half, out boundaryMinus, out boundaryPlus).Normalize();
          flag = Number.Grisu3.TryRunShortest(in boundaryMinus, in w, in boundaryPlus, number.Digits, out length, out decimalExponent);
        }
        else
          flag = Number.Grisu3.TryRunCounted(new Number.DiyFp(half).Normalize(), requestedDigits, number.Digits, out length, out decimalExponent);
        if (flag)
        {
          number.Scale = length + decimalExponent;
          number.Digits[length] = (byte) 0;
          number.DigitsCount = length;
        }
        return flag;
      }

      public static bool TryRunSingle(
        float value,
        int requestedDigits,
        ref Number.NumberBuffer number)
      {
        float num = float.IsNegative(value) ? -value : value;
        int length;
        int decimalExponent;
        bool flag;
        if (requestedDigits == -1)
        {
          Number.DiyFp boundaryMinus;
          Number.DiyFp boundaryPlus;
          Number.DiyFp w = Number.DiyFp.CreateAndGetBoundaries(num, out boundaryMinus, out boundaryPlus).Normalize();
          flag = Number.Grisu3.TryRunShortest(in boundaryMinus, in w, in boundaryPlus, number.Digits, out length, out decimalExponent);
        }
        else
          flag = Number.Grisu3.TryRunCounted(new Number.DiyFp(num).Normalize(), requestedDigits, number.Digits, out length, out decimalExponent);
        if (flag)
        {
          number.Scale = length + decimalExponent;
          number.Digits[length] = (byte) 0;
          number.DigitsCount = length;
        }
        return flag;
      }

      private static bool TryRunCounted(
        in Number.DiyFp w,
        int requestedDigits,
        Span<byte> buffer,
        out int length,
        out int decimalExponent)
      {
        int decimalExponent1;
        Number.DiyFp other = Number.Grisu3.GetCachedPowerForBinaryExponentRange(-60 - (w.e + 64), -32 - (w.e + 64), out decimalExponent1);
        int kappa;
        bool flag = Number.Grisu3.TryDigitGenCounted(w.Multiply(in other), requestedDigits, buffer, out length, out kappa);
        decimalExponent = -decimalExponent1 + kappa;
        return flag;
      }

      private static bool TryRunShortest(
        in Number.DiyFp boundaryMinus,
        in Number.DiyFp w,
        in Number.DiyFp boundaryPlus,
        Span<byte> buffer,
        out int length,
        out int decimalExponent)
      {
        int decimalExponent1;
        Number.DiyFp other = Number.Grisu3.GetCachedPowerForBinaryExponentRange(-60 - (w.e + 64), -32 - (w.e + 64), out decimalExponent1);
        Number.DiyFp w1 = w.Multiply(in other);
        Number.DiyFp low = boundaryMinus.Multiply(in other);
        Number.DiyFp high = boundaryPlus.Multiply(in other);
        int kappa;
        bool flag = Number.Grisu3.TryDigitGenShortest(in low, in w1, in high, buffer, out length, out kappa);
        decimalExponent = -decimalExponent1 + kappa;
        return flag;
      }

      private static uint BiggestPowerTen(uint number, int numberBits, out int exponentPlusOne)
      {
        int index = (numberBits + 1) * 1233 >> 12;
        uint num = Number.Grisu3.SmallPowersOfTen[index];
        if (number < num)
        {
          --index;
          num = Number.Grisu3.SmallPowersOfTen[index];
        }
        exponentPlusOne = index + 1;
        return num;
      }

      private static bool TryDigitGenCounted(
        in Number.DiyFp w,
        int requestedDigits,
        Span<byte> buffer,
        out int length,
        out int kappa)
      {
        ulong unit = 1;
        Number.DiyFp diyFp = new Number.DiyFp(1UL << -w.e, w.e);
        uint num1 = (uint) (w.f >> -diyFp.e);
        ulong rest1 = w.f & diyFp.f - 1UL;
        if (rest1 == 0UL && (requestedDigits >= 11 || num1 < Number.Grisu3.SmallPowersOfTen[requestedDigits - 1]))
        {
          length = 0;
          kappa = 0;
          return false;
        }
        uint right = Number.Grisu3.BiggestPowerTen(num1, 64 - -diyFp.e, out kappa);
        length = 0;
        while (kappa > 0)
        {
          uint Quotient;
          (Quotient, num1) = Math.DivRem(num1, right);
          buffer[length] = (byte) (48U + Quotient);
          ++length;
          --requestedDigits;
          --kappa;
          if (requestedDigits != 0)
            right /= 10U;
          else
            break;
        }
        if (requestedDigits == 0)
        {
          ulong rest2 = ((ulong) num1 << -diyFp.e) + rest1;
          return Number.Grisu3.TryRoundWeedCounted(buffer, length, rest2, (ulong) right << -diyFp.e, unit, ref kappa);
        }
        ulong num2;
        for (; requestedDigits > 0 && rest1 > unit; rest1 = num2 & diyFp.f - 1UL)
        {
          num2 = rest1 * 10UL;
          unit *= 10UL;
          uint num3 = (uint) (num2 >> -diyFp.e);
          buffer[length] = (byte) (48U + num3);
          ++length;
          --requestedDigits;
          --kappa;
        }
        if (requestedDigits == 0)
          return Number.Grisu3.TryRoundWeedCounted(buffer, length, rest1, diyFp.f, unit, ref kappa);
        buffer[0] = (byte) 0;
        length = 0;
        kappa = 0;
        return false;
      }

      private static bool TryDigitGenShortest(
        in Number.DiyFp low,
        in Number.DiyFp w,
        in Number.DiyFp high,
        Span<byte> buffer,
        out int length,
        out int kappa)
      {
        ulong unit = 1;
        Number.DiyFp other = new Number.DiyFp(low.f - unit, low.e);
        Number.DiyFp diyFp1 = new Number.DiyFp(high.f + unit, high.e);
        Number.DiyFp diyFp2 = diyFp1.Subtract(in other);
        Number.DiyFp diyFp3 = new Number.DiyFp(1UL << -w.e, w.e);
        uint num1 = (uint) (diyFp1.f >> -diyFp3.e);
        ulong rest1 = diyFp1.f & diyFp3.f - 1UL;
        uint right = Number.Grisu3.BiggestPowerTen(num1, 64 - -diyFp3.e, out kappa);
        length = 0;
        while (kappa > 0)
        {
          uint Quotient;
          (Quotient, num1) = Math.DivRem(num1, right);
          buffer[length] = (byte) (48U + Quotient);
          ++length;
          --kappa;
          ulong rest2 = ((ulong) num1 << -diyFp3.e) + rest1;
          if (rest2 < diyFp2.f)
            return Number.Grisu3.TryRoundWeedShortest(buffer, length, diyFp1.Subtract(in w).f, diyFp2.f, rest2, (ulong) right << -diyFp3.e, unit);
          right /= 10U;
        }
        do
        {
          ulong num2 = rest1 * 10UL;
          unit *= 10UL;
          diyFp2 = new Number.DiyFp(diyFp2.f * 10UL, diyFp2.e);
          uint num3 = (uint) (num2 >> -diyFp3.e);
          buffer[length] = (byte) (48U + num3);
          ++length;
          --kappa;
          rest1 = num2 & diyFp3.f - 1UL;
        }
        while (rest1 >= diyFp2.f);
        return Number.Grisu3.TryRoundWeedShortest(buffer, length, diyFp1.Subtract(in w).f * unit, diyFp2.f, rest1, diyFp3.f, unit);
      }

      private static Number.DiyFp GetCachedPowerForBinaryExponentRange(
        int minExponent,
        int maxExponent,
        out int decimalExponent)
      {
        int index = (348 + (int) Math.Ceiling((double) (minExponent + 64 - 1) * 0.3010299956639812) - 1) / 8 + 1;
        decimalExponent = (int) Number.Grisu3.CachedPowersDecimalExponent[index];
        return new Number.DiyFp(Number.Grisu3.CachedPowersSignificand[index], (int) Number.Grisu3.CachedPowersBinaryExponent[index]);
      }

      private static bool TryRoundWeedCounted(
        Span<byte> buffer,
        int length,
        ulong rest,
        ulong tenKappa,
        ulong unit,
        ref int kappa)
      {
        if (unit >= tenKappa || tenKappa - unit <= unit)
          return false;
        if (tenKappa - rest > rest && tenKappa - 2UL * rest >= 2UL * unit)
          return true;
        if (rest <= unit || tenKappa > rest - unit && tenKappa - (rest - unit) > rest - unit)
          return false;
        ++buffer[length - 1];
        for (int index = length - 1; index > 0 && buffer[index] == (byte) 58; --index)
        {
          buffer[index] = (byte) 48;
          ++buffer[index - 1];
        }
        if (buffer[0] == (byte) 58)
        {
          buffer[0] = (byte) 49;
          ++kappa;
        }
        return true;
      }

      private static bool TryRoundWeedShortest(
        Span<byte> buffer,
        int length,
        ulong distanceTooHighW,
        ulong unsafeInterval,
        ulong rest,
        ulong tenKappa,
        ulong unit)
      {
        ulong num1 = distanceTooHighW - unit;
        ulong num2 = distanceTooHighW + unit;
        for (; rest < num1 && unsafeInterval - rest >= tenKappa && (rest + tenKappa < num1 || num1 - rest >= rest + tenKappa - num1); rest += tenKappa)
          --buffer[length - 1];
        return (rest >= num2 || unsafeInterval - rest < tenKappa || rest + tenKappa >= num2 && num2 - rest <= rest + tenKappa - num2) && 2UL * unit <= rest && rest <= unsafeInterval - 4UL * unit;
      }
    }

    [CompilerFeatureRequired("RefStructs")]
    internal ref struct NumberBuffer
    {
      public int DigitsCount;
      public int Scale;
      public bool IsNegative;
      public bool HasNonZeroTail;
      public Number.NumberBufferKind Kind;
      public Span<byte> Digits;

      public unsafe NumberBuffer(Number.NumberBufferKind kind, byte* digits, int digitsLength)
        : this(kind, new Span<byte>((void*) digits, digitsLength))
      {
      }

      public NumberBuffer(Number.NumberBufferKind kind, Span<byte> digits)
      {
        this.DigitsCount = 0;
        this.Scale = 0;
        this.IsNegative = false;
        this.HasNonZeroTail = false;
        this.Kind = kind;
        this.Digits = digits;
        this.Digits[0] = (byte) 0;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public unsafe byte* GetDigitsPointer()
      {
        return (byte*) Unsafe.AsPointer<byte>(ref MemoryMarshal.GetReference<byte>(this.Digits));
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append('[');
        stringBuilder.Append('"');
        for (int index = 0; index < this.Digits.Length; ++index)
        {
          byte num = this.Digits[index];
          if (num != (byte) 0)
            stringBuilder.Append((char) num);
          else
            break;
        }
        stringBuilder.Append('"');
        stringBuilder.Append(", Length = ").Append(this.DigitsCount);
        stringBuilder.Append(", Scale = ").Append(this.Scale);
        stringBuilder.Append(", IsNegative = ").Append(this.IsNegative);
        stringBuilder.Append(", HasNonZeroTail = ").Append(this.HasNonZeroTail);
        stringBuilder.Append(", Kind = ").Append((object) this.Kind);
        stringBuilder.Append(']');
        return stringBuilder.ToString();
      }
    }

    internal enum NumberBufferKind : byte
    {
      Unknown,
      Integer,
      Decimal,
      FloatingPoint,
    }

    private interface IHexOrBinaryParser<TInteger> where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      static abstract NumberStyles AllowedStyles { get; }

      static abstract bool IsValidChar(uint ch);

      static abstract uint FromChar(uint ch);

      static abstract uint MaxDigitValue { get; }

      static abstract int MaxDigitCount { get; }

      static abstract TInteger ShiftLeftForNextDigit(TInteger value);
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private readonly struct HexParser<TInteger> : Number.IHexOrBinaryParser<TInteger> where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      public static NumberStyles AllowedStyles => NumberStyles.HexNumber;

      public static bool IsValidChar(uint ch) => HexConverter.IsHexChar((int) ch);

      public static uint FromChar(uint ch) => (uint) HexConverter.FromChar((int) ch);

      public static uint MaxDigitValue => 15;

      public static int MaxDigitCount => TInteger.MaxHexDigitCount;

      public static TInteger ShiftLeftForNextDigit(TInteger value) => TInteger.MultiplyBy16(value);
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private readonly struct BinaryParser<TInteger> : Number.IHexOrBinaryParser<TInteger> where TInteger : unmanaged, IBinaryIntegerParseAndFormatInfo<TInteger>
    {
      public static NumberStyles AllowedStyles => NumberStyles.BinaryNumber;

      public static bool IsValidChar(uint ch) => ch - 48U <= 1U;

      public static uint FromChar(uint ch) => ch - 48U;

      public static uint MaxDigitValue => 1;

      public static int MaxDigitCount => sizeof (TInteger) * 8;

      public static TInteger ShiftLeftForNextDigit(TInteger value) => value << 1;
    }

    internal enum ParsingStatus
    {
      OK,
      Failed,
      Overflow,
    }
  }
}
