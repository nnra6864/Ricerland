// Decompiled with JetBrains decompiler
// Type: System.ThrowHelper
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: FF4E3FE8-6250-4072-85EA-EBDA8F185C35
// Assembly location: /home/nnra/Unity/Hub/Editor/2022.3.31f1/Editor/Data/MonoBleedingEdge/lib/mono/unityjit-linux/mscorlib.dll

using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;

#nullable disable
namespace System
{
  [StackTraceHidden]
  internal static class ThrowHelper
  {
    internal static void ThrowArgumentNullException(ExceptionArgument argument)
    {
      throw ThrowHelper.CreateArgumentNullException(argument);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentNullException(ExceptionArgument argument)
    {
      return (Exception) new ArgumentNullException(argument.ToString());
    }

    internal static void ThrowArrayTypeMismatchException()
    {
      throw ThrowHelper.CreateArrayTypeMismatchException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArrayTypeMismatchException()
    {
      return (Exception) new ArrayTypeMismatchException();
    }

    internal static void ThrowArgumentException_InvalidTypeWithPointersNotSupported(Type type)
    {
      throw ThrowHelper.CreateArgumentException_InvalidTypeWithPointersNotSupported(type);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentException_InvalidTypeWithPointersNotSupported(Type type)
    {
      return (Exception) new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", (object) type));
    }

    internal static void ThrowArgumentException_DestinationTooShort()
    {
      throw ThrowHelper.CreateArgumentException_DestinationTooShort();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentException_DestinationTooShort()
    {
      return (Exception) new ArgumentException("Destination is too short.");
    }

    internal static void ThrowIndexOutOfRangeException()
    {
      throw ThrowHelper.CreateIndexOutOfRangeException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateIndexOutOfRangeException()
    {
      return (Exception) new IndexOutOfRangeException();
    }

    internal static void ThrowArgumentOutOfRangeException()
    {
      throw ThrowHelper.CreateArgumentOutOfRangeException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentOutOfRangeException()
    {
      return (Exception) new ArgumentOutOfRangeException();
    }

    internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
    {
      throw ThrowHelper.CreateArgumentOutOfRangeException(argument);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentOutOfRangeException(ExceptionArgument argument)
    {
      return (Exception) new ArgumentOutOfRangeException(argument.ToString());
    }

    internal static void ThrowArgumentOutOfRangeException_PrecisionTooLarge()
    {
      throw ThrowHelper.CreateArgumentOutOfRangeException_PrecisionTooLarge();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentOutOfRangeException_PrecisionTooLarge()
    {
      return (Exception) new ArgumentOutOfRangeException("precision", SR.Format("Precision cannot be larger than {0}.", (object) (byte) 99));
    }

    internal static void ThrowArgumentOutOfRangeException_SymbolDoesNotFit()
    {
      throw ThrowHelper.CreateArgumentOutOfRangeException_SymbolDoesNotFit();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentOutOfRangeException_SymbolDoesNotFit()
    {
      return (Exception) new ArgumentOutOfRangeException("symbol", "Format specifier was invalid.");
    }

    internal static void ThrowInvalidOperationException()
    {
      throw ThrowHelper.CreateInvalidOperationException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateInvalidOperationException()
    {
      return (Exception) new InvalidOperationException();
    }

    internal static void ThrowInvalidOperationException_OutstandingReferences()
    {
      throw ThrowHelper.CreateInvalidOperationException_OutstandingReferences();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateInvalidOperationException_OutstandingReferences()
    {
      return (Exception) new InvalidOperationException("Release all references before disposing this instance.");
    }

    internal static void ThrowInvalidOperationException_UnexpectedSegmentType()
    {
      throw ThrowHelper.CreateInvalidOperationException_UnexpectedSegmentType();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateInvalidOperationException_UnexpectedSegmentType()
    {
      return (Exception) new InvalidOperationException("Unexpected segment type.");
    }

    internal static void ThrowInvalidOperationException_EndPositionNotReached()
    {
      throw ThrowHelper.CreateInvalidOperationException_EndPositionNotReached();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateInvalidOperationException_EndPositionNotReached()
    {
      return (Exception) new InvalidOperationException("End position was not reached during enumeration.");
    }

    internal static void ThrowArgumentOutOfRangeException_PositionOutOfRange()
    {
      throw ThrowHelper.CreateArgumentOutOfRangeException_PositionOutOfRange();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentOutOfRangeException_PositionOutOfRange()
    {
      return (Exception) new ArgumentOutOfRangeException("position");
    }

    internal static void ThrowArgumentOutOfRangeException_OffsetOutOfRange()
    {
      throw ThrowHelper.CreateArgumentOutOfRangeException_OffsetOutOfRange();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentOutOfRangeException_OffsetOutOfRange()
    {
      return (Exception) new ArgumentOutOfRangeException("offset");
    }

    internal static void ThrowObjectDisposedException_ArrayMemoryPoolBuffer()
    {
      throw ThrowHelper.CreateObjectDisposedException_ArrayMemoryPoolBuffer();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateObjectDisposedException_ArrayMemoryPoolBuffer()
    {
      return (Exception) new ObjectDisposedException("ArrayMemoryPoolBuffer");
    }

    internal static void ThrowFormatException_BadFormatSpecifier()
    {
      throw ThrowHelper.CreateFormatException_BadFormatSpecifier();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateFormatException_BadFormatSpecifier()
    {
      return (Exception) new FormatException("Format specifier was invalid.");
    }

    internal static void ThrowArgumentException_OverlapAlignmentMismatch()
    {
      throw ThrowHelper.CreateArgumentException_OverlapAlignmentMismatch();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateArgumentException_OverlapAlignmentMismatch()
    {
      return (Exception) new ArgumentException("Overlapping spans have mismatching alignment.");
    }

    internal static void ThrowNotSupportedException()
    {
      throw ThrowHelper.CreateThrowNotSupportedException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateThrowNotSupportedException()
    {
      return (Exception) new NotSupportedException();
    }

    public static bool TryFormatThrowFormatException(out int bytesWritten)
    {
      bytesWritten = 0;
      ThrowHelper.ThrowFormatException_BadFormatSpecifier();
      return false;
    }

    public static bool TryParseThrowFormatException<T>(out T value, out int bytesConsumed)
    {
      value = default (T);
      bytesConsumed = 0;
      ThrowHelper.ThrowFormatException_BadFormatSpecifier();
      return false;
    }

    public static void ThrowArgumentValidationException<T>(
      ReadOnlySequenceSegment<T> startSegment,
      int startIndex,
      ReadOnlySequenceSegment<T> endSegment)
    {
      throw ThrowHelper.CreateArgumentValidationException<T>(startSegment, startIndex, endSegment);
    }

    private static Exception CreateArgumentValidationException<T>(
      ReadOnlySequenceSegment<T> startSegment,
      int startIndex,
      ReadOnlySequenceSegment<T> endSegment)
    {
      if (startSegment == null)
        return ThrowHelper.CreateArgumentNullException(ExceptionArgument.startSegment);
      if (endSegment == null)
        return ThrowHelper.CreateArgumentNullException(ExceptionArgument.endSegment);
      if (startSegment != endSegment && startSegment.RunningIndex > endSegment.RunningIndex)
        return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endSegment);
      return (uint) startSegment.Memory.Length < (uint) startIndex ? ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.startIndex) : ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endIndex);
    }

    public static void ThrowArgumentValidationException(Array array, int start)
    {
      throw ThrowHelper.CreateArgumentValidationException(array, start);
    }

    private static Exception CreateArgumentValidationException(Array array, int start)
    {
      if (array == null)
        return ThrowHelper.CreateArgumentNullException(ExceptionArgument.array);
      return (uint) start > (uint) array.Length ? ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start) : ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
    }

    public static void ThrowStartOrEndArgumentValidationException(long start)
    {
      throw ThrowHelper.CreateStartOrEndArgumentValidationException(start);
    }

    private static Exception CreateStartOrEndArgumentValidationException(long start)
    {
      return start < 0L ? ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start) : ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
    }

    internal static void ThrowWrongKeyTypeArgumentException(object key, Type targetType)
    {
      throw new ArgumentException(Environment.GetResourceString("The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.", key, (object) targetType), nameof (key));
    }

    internal static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
    {
      throw new ArgumentException(Environment.GetResourceString("The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.", value, (object) targetType), nameof (value));
    }

    internal static void ThrowKeyNotFoundException() => throw new KeyNotFoundException();

    internal static void ThrowArgumentException(ExceptionResource resource)
    {
      throw new ArgumentException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowArgumentException(
      ExceptionResource resource,
      ExceptionArgument argument)
    {
      throw new ArgumentException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)), ThrowHelper.GetArgumentName(argument));
    }

    internal static void ThrowArgumentOutOfRangeException(
      ExceptionArgument argument,
      ExceptionResource resource)
    {
      if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
        throw new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), string.Empty);
      throw new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowInvalidOperationException(ExceptionResource resource)
    {
      throw new InvalidOperationException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowSerializationException(ExceptionResource resource)
    {
      throw new SerializationException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowSecurityException(ExceptionResource resource)
    {
      throw new SecurityException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowNotSupportedException(ExceptionResource resource)
    {
      throw new NotSupportedException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowUnauthorizedAccessException(ExceptionResource resource)
    {
      throw new UnauthorizedAccessException(Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowObjectDisposedException(string objectName, ExceptionResource resource)
    {
      throw new ObjectDisposedException(objectName, Environment.GetResourceString(ThrowHelper.GetResourceName(resource)));
    }

    internal static void ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
    {
      throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
    }

    internal static void ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen()
    {
      throw new InvalidOperationException("Enumeration has either not started or has already finished.");
    }

    internal static void ThrowInvalidOperationException_InvalidOperation_EnumNotStarted()
    {
      throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
    }

    internal static void ThrowInvalidOperationException_InvalidOperation_EnumEnded()
    {
      throw new InvalidOperationException("Enumeration already finished.");
    }

    internal static void ThrowInvalidOperationException_InvalidOperation_NoValue()
    {
      throw new InvalidOperationException("Nullable object must have a value.");
    }

    private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(
      ExceptionArgument argument,
      string resource)
    {
      return new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), resource);
    }

    internal static void ThrowArgumentOutOfRange_IndexException()
    {
      throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.index, "Index was out of range. Must be non-negative and less than the size of the collection.");
    }

    internal static void ThrowIndexArgumentOutOfRange_NeedNonNegNumException()
    {
      throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.index, "Non-negative number required.");
    }

    internal static void ThrowArgumentException_Argument_InvalidArrayType()
    {
      throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
    }

    private static ArgumentException GetAddingDuplicateWithKeyArgumentException(object key)
    {
      return new ArgumentException(SR.Format("An item with the same key has already been added. Key: {0}", key));
    }

    internal static void ThrowAddingDuplicateWithKeyArgumentException(object key)
    {
      throw ThrowHelper.GetAddingDuplicateWithKeyArgumentException(key);
    }

    private static KeyNotFoundException GetKeyNotFoundException(object key)
    {
      throw new KeyNotFoundException(SR.Format("The given key '{0}' was not present in the dictionary.", (object) key.ToString()));
    }

    internal static void ThrowKeyNotFoundException(object key)
    {
      throw ThrowHelper.GetKeyNotFoundException(key);
    }

    internal static void ThrowInvalidTypeWithPointersNotSupported(Type targetType)
    {
      throw new ArgumentException(SR.Format("Cannot use type '{0}'. Only value types without pointers or references are supported.", (object) targetType));
    }

    internal static void ThrowInvalidOperationException_ConcurrentOperationsNotSupported()
    {
      throw ThrowHelper.GetInvalidOperationException("Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.");
    }

    internal static InvalidOperationException GetInvalidOperationException(string str)
    {
      return new InvalidOperationException(str);
    }

    internal static void ThrowArraySegmentCtorValidationFailedExceptions(
      Array array,
      int offset,
      int count)
    {
      throw ThrowHelper.GetArraySegmentCtorValidationFailedException(array, offset, count);
    }

    private static Exception GetArraySegmentCtorValidationFailedException(
      Array array,
      int offset,
      int count)
    {
      if (array == null)
        return (Exception) ThrowHelper.GetArgumentNullException(ExceptionArgument.array);
      if (offset < 0)
        return (Exception) ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      return count < 0 ? (Exception) ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum) : (Exception) ThrowHelper.GetArgumentException(ExceptionResource.Argument_InvalidOffLen);
    }

    private static ArgumentException GetArgumentException(ExceptionResource resource)
    {
      return new ArgumentException(resource.ToString());
    }

    private static ArgumentNullException GetArgumentNullException(ExceptionArgument argument)
    {
      return new ArgumentNullException(ThrowHelper.GetArgumentName(argument));
    }

    internal static void IfNullAndNullsAreIllegalThenThrow<T>(
      object value,
      ExceptionArgument argName)
    {
      if (value != null || (object) default (T) == null)
        return;
      ThrowHelper.ThrowArgumentNullException(argName);
    }

    internal static string GetArgumentName(ExceptionArgument argument)
    {
      switch (argument)
      {
        case ExceptionArgument.obj:
          return "obj";
        case ExceptionArgument.dictionary:
          return "dictionary";
        case ExceptionArgument.dictionaryCreationThreshold:
          return "dictionaryCreationThreshold";
        case ExceptionArgument.array:
          return "array";
        case ExceptionArgument.info:
          return "info";
        case ExceptionArgument.key:
          return "key";
        case ExceptionArgument.collection:
          return "collection";
        case ExceptionArgument.list:
          return "list";
        case ExceptionArgument.match:
          return "match";
        case ExceptionArgument.converter:
          return "converter";
        case ExceptionArgument.queue:
          return "queue";
        case ExceptionArgument.stack:
          return "stack";
        case ExceptionArgument.capacity:
          return "capacity";
        case ExceptionArgument.index:
          return "index";
        case ExceptionArgument.startIndex:
          return "startIndex";
        case ExceptionArgument.value:
          return "value";
        case ExceptionArgument.count:
          return "count";
        case ExceptionArgument.arrayIndex:
          return "arrayIndex";
        case ExceptionArgument.name:
          return "name";
        case ExceptionArgument.mode:
          return "mode";
        case ExceptionArgument.item:
          return "item";
        case ExceptionArgument.options:
          return "options";
        case ExceptionArgument.view:
          return "view";
        case ExceptionArgument.sourceBytesToCopy:
          return "sourceBytesToCopy";
        default:
          return string.Empty;
      }
    }

    private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(
      ExceptionArgument argument,
      ExceptionResource resource)
    {
      return new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), resource.ToString());
    }

    internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index()
    {
      throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
    }

    internal static void ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count()
    {
      throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
    }

    internal static string GetResourceName(ExceptionResource resource)
    {
      switch (resource)
      {
        case ExceptionResource.Argument_ImplementIComparable:
          return "At least one object must implement IComparable.";
        case ExceptionResource.Argument_InvalidType:
          return "The type of arguments passed into generic comparer methods is invalid.";
        case ExceptionResource.Argument_InvalidArgumentForComparison:
          return "Type of argument is not compatible with the generic comparer.";
        case ExceptionResource.Argument_InvalidRegistryKeyPermissionCheck:
          return "The specified RegistryKeyPermissionCheck value is invalid.";
        case ExceptionResource.ArgumentOutOfRange_NeedNonNegNum:
          return "Non-negative number required.";
        case ExceptionResource.Arg_ArrayPlusOffTooSmall:
          return "Destination array is not long enough to copy all the items in the collection. Check array index and length.";
        case ExceptionResource.Arg_NonZeroLowerBound:
          return "The lower bound of target array must be zero.";
        case ExceptionResource.Arg_RankMultiDimNotSupported:
          return "Only single dimensional arrays are supported for the requested action.";
        case ExceptionResource.Arg_RegKeyDelHive:
          return "Cannot delete a registry hive's subtree.";
        case ExceptionResource.Arg_RegKeyStrLenBug:
          return "Registry key names should not be greater than 255 characters.";
        case ExceptionResource.Arg_RegSetStrArrNull:
          return "RegistryKey.SetValue does not allow a String[] that contains a null String reference.";
        case ExceptionResource.Arg_RegSetMismatchedKind:
          return "The type of the value object did not match the specified RegistryValueKind or the object could not be properly converted.";
        case ExceptionResource.Arg_RegSubKeyAbsent:
          return "Cannot delete a subkey tree because the subkey does not exist.";
        case ExceptionResource.Arg_RegSubKeyValueAbsent:
          return "No value exists with that name.";
        case ExceptionResource.Argument_AddingDuplicate:
          return "An item with the same key has already been added.";
        case ExceptionResource.Serialization_InvalidOnDeser:
          return "OnDeserialization method was called while the object was not being deserialized.";
        case ExceptionResource.Serialization_MissingKeys:
          return "The Keys for this Hashtable are missing.";
        case ExceptionResource.Serialization_NullKey:
          return "One of the serialized keys is null.";
        case ExceptionResource.Argument_InvalidArrayType:
          return "Target array type is not compatible with the type of items in the collection.";
        case ExceptionResource.NotSupported_KeyCollectionSet:
          return "Mutating a key collection derived from a dictionary is not allowed.";
        case ExceptionResource.NotSupported_ValueCollectionSet:
          return "Mutating a value collection derived from a dictionary is not allowed.";
        case ExceptionResource.ArgumentOutOfRange_SmallCapacity:
          return "capacity was less than the current size.";
        case ExceptionResource.ArgumentOutOfRange_Index:
          return "Index was out of range. Must be non-negative and less than the size of the collection.";
        case ExceptionResource.Argument_InvalidOffLen:
          return "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.";
        case ExceptionResource.Argument_ItemNotExist:
          return "The specified item does not exist in this KeyedCollection.";
        case ExceptionResource.ArgumentOutOfRange_Count:
          return "Count must be positive and count must refer to a location within the string/array/collection.";
        case ExceptionResource.ArgumentOutOfRange_InvalidThreshold:
          return "The specified threshold for creating dictionary is out of range.";
        case ExceptionResource.ArgumentOutOfRange_ListInsert:
          return "Index must be within the bounds of the List.";
        case ExceptionResource.NotSupported_ReadOnlyCollection:
          return "Collection is read-only.";
        case ExceptionResource.InvalidOperation_CannotRemoveFromStackOrQueue:
          return "Removal is an invalid operation for Stack or Queue.";
        case ExceptionResource.InvalidOperation_EmptyQueue:
          return "Queue empty.";
        case ExceptionResource.InvalidOperation_EnumOpCantHappen:
          return "Enumeration has either not started or has already finished.";
        case ExceptionResource.InvalidOperation_EnumFailedVersion:
          return "Collection was modified; enumeration operation may not execute.";
        case ExceptionResource.InvalidOperation_EmptyStack:
          return "Stack empty.";
        case ExceptionResource.ArgumentOutOfRange_BiggerThanCollection:
          return "Larger than collection size.";
        case ExceptionResource.InvalidOperation_EnumNotStarted:
          return "Enumeration has not started. Call MoveNext.";
        case ExceptionResource.InvalidOperation_EnumEnded:
          return "Enumeration already finished.";
        case ExceptionResource.NotSupported_SortedListNestedWrite:
          return "This operation is not supported on SortedList nested types because they require modifying the original SortedList.";
        case ExceptionResource.InvalidOperation_NoValue:
          return "Nullable object must have a value.";
        case ExceptionResource.InvalidOperation_RegRemoveSubKey:
          return "Registry key has subkeys and recursive removes are not supported by this method.";
        case ExceptionResource.Security_RegistryPermission:
          return "Requested registry access is not allowed.";
        case ExceptionResource.UnauthorizedAccess_RegistryNoWrite:
          return "Cannot write to the registry key.";
        case ExceptionResource.ObjectDisposed_RegKeyClosed:
          return "Cannot access a closed registry key.";
        case ExceptionResource.NotSupported_InComparableType:
          return "A type must implement IComparable<T> or IComparable to support comparison.";
        case ExceptionResource.Argument_InvalidRegistryOptionsCheck:
          return "The specified RegistryOptions value is invalid.";
        case ExceptionResource.Argument_InvalidRegistryViewCheck:
          return "The specified RegistryView value is invalid.";
        default:
          return string.Empty;
      }
    }

    internal static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
    {
      throw ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
    }
  }
}
