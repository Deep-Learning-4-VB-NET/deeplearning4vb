' DeepLearning4VB: typed element storage of the managed ND4VB engine (phase 2, see docs/PHASE2_DESIGN.md).

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>
    ''' The elements of a data buffer, held in a typed .NET array (one subclass per
    ''' <see cref="StorageType"/>). A storage has no offset or shape: data buffers and arrays are views
    ''' (storage, element offset, length/shape information) over it, so several views can share one storage.
    ''' </summary>
    ''' <remarks>
    ''' <para>Generic accessors (<see cref="GetDouble"/>, <see cref="SetDouble"/>, <see cref="GetLong"/>,
    ''' <see cref="SetLong"/>, ...) convert with Java's primitive casts (see <see cref="NumericCasts"/>):
    ''' floating point to integer truncates toward zero and saturates (NaN gives 0), integer narrowing
    ''' wraps, booleans read as 0/1 and store "value &lt;&gt; 0". The typed array properties
    ''' (<see cref="Floats"/>, <see cref="Doubles"/>, ...) return the backing array for fast loops, or
    ''' Nothing when the storage has another type.</para>
    ''' <para>Indexes are element indexes into the backing array (0-based); the storages do no other
    ''' bounds checking than the array's. Storages are not thread-safe; concurrent writes to different
    ''' elements are safe.</para>
    ''' </remarks>
    Public MustInherit Class ManagedStorage

        ''' <summary>The element type.</summary>
        Public MustOverride ReadOnly Property StorageType As StorageType

        ''' <summary>Number of elements.</summary>
        Public MustOverride ReadOnly Property Length As Long

        ''' <summary>The backing array (its element type is <see cref="StorageTypes.ElementType"/> of the storage type).</summary>
        Public MustOverride ReadOnly Property RawArray As Array

#Region "Typed fast paths"

        ''' <summary>The backing array of a FLOAT storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property Floats As Single()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a DOUBLE storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property Doubles As Double()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The bit patterns of a HALF or BFLOAT16 storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property HalfBits As UShort()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a BYTE (INT8) storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property SBytes As SByte()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a UBYTE (UINT8) storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property UBytes As Byte()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a SHORT (INT16) storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property Shorts As Short()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a UINT16 storage, otherwise Nothing (HALF and BFLOAT16 use <see cref="HalfBits"/>).</summary>
        Public Overridable ReadOnly Property UShorts As UShort()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of an INT (INT32) storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property Ints As Integer()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a UINT32 storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property UInts As UInteger()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a LONG (INT64) storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property Longs As Long()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a UINT64 storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property ULongs As ULong()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a BOOL storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property Booleans As Boolean()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The backing array of a UTF8 storage, otherwise Nothing.</summary>
        Public Overridable ReadOnly Property Strings As String()
            Get
                Return Nothing
            End Get
        End Property

#End Region

#Region "Element access"

        ''' <summary>The element as a double (unsigned types read unsigned, booleans 0/1). Throws <see cref="NotSupportedException"/> for UTF8.</summary>
        Public MustOverride Function GetDouble(ByVal index As Long) As Double

        ''' <summary>Stores a double with Java's cast to the element type (see <see cref="NumericCasts"/>). Throws for UTF8.</summary>
        Public MustOverride Sub SetDouble(ByVal index As Long, ByVal value As Double)

        ''' <summary>The element as a long (Java's <c>(long)</c> cast for floating point; UINT64 bits reinterpreted as signed). Throws for UTF8.</summary>
        Public MustOverride Function GetLong(ByVal index As Long) As Long

        ''' <summary>Stores a long, wrapping to narrower integer types and rounding to floating-point types. Throws for UTF8.</summary>
        Public MustOverride Sub SetLong(ByVal index As Long, ByVal value As Long)

        ''' <summary>The element as a single (one rounding from the stored value).</summary>
        Public Overridable Function GetFloat(ByVal index As Long) As Single
            Return CSng(GetDouble(index))
        End Function

        ''' <summary>Stores a single (Java's <c>put(long, float)</c>).</summary>
        Public Overridable Sub SetFloat(ByVal index As Long, ByVal value As Single)
            SetDouble(index, value)
        End Sub

        ''' <summary>The element as an int: Java's <c>(int)</c> cast (saturating for floating point, wrapping for wider integers).</summary>
        Public Overridable Function GetInt(ByVal index As Long) As Integer
            If StorageTypes.IsFloatingPoint(StorageType) Then
                Return NumericCasts.DoubleToInt(GetDouble(index))
            End If
            Return NumericCasts.LongToInt(GetLong(index))
        End Function

        ''' <summary>Stores an int.</summary>
        Public Overridable Sub SetInt(ByVal index As Long, ByVal value As Integer)
            SetLong(index, value)
        End Sub

        ''' <summary>True when the element is non-zero (NaN counts as non-zero).</summary>
        Public Overridable Function GetBoolean(ByVal index As Long) As Boolean
            If StorageTypes.IsFloatingPoint(StorageType) Then
                Return GetDouble(index) <> 0.0
            End If
            Return GetLong(index) <> 0L
        End Function

        ''' <summary>Stores 1 for True and 0 for False.</summary>
        Public Overridable Sub SetBoolean(ByVal index As Long, ByVal value As Boolean)
            SetLong(index, If(value, 1L, 0L))
        End Sub

        ''' <summary>
        ''' The element as text: the string of a UTF8 storage; for numeric storages the value in invariant
        ''' culture (round-trip format for floating point, "true"/"false" for BOOL).
        ''' </summary>
        Public Overridable Function GetString(ByVal index As Long) As String
            If StorageTypes.IsFloatingPoint(StorageType) Then
                Return NumericCasts.FormatDouble(GetDouble(index))
            End If
            Return GetLong(index).ToString(CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Stores text: as is in a UTF8 storage, otherwise parsed as an invariant-culture number ("true"/"false" accepted).</summary>
        Public Overridable Sub SetString(ByVal index As Long, ByVal value As String)
            If StorageTypes.IsInteger(StorageType) Then
                Dim l As Long
                If value IsNot Nothing AndAlso Long.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, l) Then
                    SetLong(index, l)
                    Return
                End If
            End If
            SetDouble(index, NumericCasts.ParseDouble(value))
        End Sub

#End Region

#Region "Bulk operations"

        ''' <summary>
        ''' Copies <paramref name="count"/> elements to another storage, converting when the types differ
        ''' (integer-to-integer copies go through <c>Long</c>, so 64-bit values stay exact; copies to and
        ''' from UTF8 go through text). Overlapping copies within one storage behave like memmove.
        ''' </summary>
        Public MustOverride Sub CopyTo(ByVal sourceIndex As Long, ByVal destination As ManagedStorage, ByVal destinationIndex As Long, ByVal count As Long)

        ''' <summary>Copies the whole storage to <paramref name="destination"/> starting at its element 0.</summary>
        Public Sub CopyTo(ByVal destination As ManagedStorage)
            CopyTo(0, destination, 0, Length)
        End Sub

        ''' <summary>
        ''' Copies elements from a .NET array (<c>Single()</c>, <c>Double()</c>, <c>SByte()</c>, <c>Byte()</c>,
        ''' <c>Short()</c>, <c>UShort()</c>, <c>Integer()</c>, <c>UInteger()</c>, <c>Long()</c>, <c>ULong()</c>,
        ''' <c>Boolean()</c> or <c>String()</c>), converting as <see cref="CopyTo(Long, ManagedStorage, Long, Long)"/> does.
        ''' A <c>UShort()</c> source holds UINT16 values, except that HALF and BFLOAT16 storages copy it as raw bit patterns.
        ''' </summary>
        Public Sub CopyFrom(ByVal source As Array, ByVal sourceIndex As Long, ByVal destinationIndex As Long, ByVal count As Long)
            If source Is Nothing Then
                Throw New ArgumentNullException(NameOf(source))
            End If
            If source.GetType().GetElementType() Is RawArray.GetType().GetElementType() Then
                Array.Copy(source, sourceIndex, RawArray, destinationIndex, count)
                Return
            End If
            Wrap(source).CopyTo(sourceIndex, Me, destinationIndex, count)
        End Sub

        ''' <summary>Copies all of <paramref name="source"/> into this storage starting at element 0.</summary>
        Public Sub CopyFrom(ByVal source As Array)
            If source Is Nothing Then
                Throw New ArgumentNullException(NameOf(source))
            End If
            CopyFrom(source, 0, 0, source.LongLength)
        End Sub

        ''' <summary>
        ''' Copies elements into a .NET array of any of the element types <see cref="CopyFrom(Array, Long, Long, Long)"/>
        ''' accepts, converting to the array's element type (raw bit patterns for HALF/BFLOAT16 into <c>UShort()</c>).
        ''' </summary>
        Public Sub CopyToArray(ByVal sourceIndex As Long, ByVal destination As Array, ByVal destinationIndex As Long, ByVal count As Long)
            If destination Is Nothing Then
                Throw New ArgumentNullException(NameOf(destination))
            End If
            If destination.GetType().GetElementType() Is RawArray.GetType().GetElementType() Then
                Array.Copy(RawArray, sourceIndex, destination, destinationIndex, count)
                Return
            End If
            CopyTo(sourceIndex, Wrap(destination), destinationIndex, count)
        End Sub

        ''' <summary>Sets <paramref name="count"/> elements from <paramref name="start"/> to a value (converted once, as <see cref="SetDouble"/> would).</summary>
        Public MustOverride Sub Fill(ByVal value As Double, ByVal start As Long, ByVal count As Long)

        ''' <summary>Sets <paramref name="count"/> elements from <paramref name="start"/> to an integer value (converted once, as <see cref="SetLong"/> would).</summary>
        Public MustOverride Sub Fill(ByVal value As Long, ByVal start As Long, ByVal count As Long)

        ''' <summary>Sets every element to a value.</summary>
        Public Sub Fill(ByVal value As Double)
            Fill(value, 0L, Length)
        End Sub

        ''' <summary>Sets every element to an integer value.</summary>
        Public Sub Fill(ByVal value As Long)
            Fill(value, 0L, Length)
        End Sub

        ''' <summary>A deep copy of the storage.</summary>
        Public MustOverride Function Clone() As ManagedStorage

        ''' <summary>A new storage of the same type holding a copy of <paramref name="count"/> elements from <paramref name="start"/>.</summary>
        Public MustOverride Function Clone(ByVal start As Long, ByVal count As Long) As ManagedStorage

        ''' <summary>A new storage of another type holding the converted elements (a plain copy when the type is the same).</summary>
        Public Function ConvertTo(ByVal type As StorageType) As ManagedStorage
            If type = StorageType Then
                Return Clone()
            End If
            Dim result As ManagedStorage = Create(type, Length)
            CopyTo(0, result, 0, Length)
            Return result
        End Function

        ''' <summary>Copies one element between storages of any types with the conversion rules of <see cref="CopyTo(Long, ManagedStorage, Long, Long)"/>.</summary>
        Friend Shared Sub ConvertElement(ByVal source As ManagedStorage, ByVal sourceIndex As Long, ByVal destination As ManagedStorage, ByVal destinationIndex As Long)
            Dim st As StorageType = source.StorageType
            Dim dt As StorageType = destination.StorageType
            If st = StorageType.UTF8 OrElse dt = StorageType.UTF8 Then
                destination.SetString(destinationIndex, source.GetString(sourceIndex))
                Return
            End If
            Select Case dt
                Case StorageType.FLOAT, StorageType.HALF, StorageType.BFLOAT16
                    destination.SetFloat(destinationIndex, source.GetFloat(sourceIndex))
                Case StorageType.DOUBLE
                    destination.SetDouble(destinationIndex, source.GetDouble(sourceIndex))
                Case Else
                    If StorageTypes.IsFloatingPoint(st) Then
                        destination.SetDouble(destinationIndex, source.GetDouble(sourceIndex))
                    Else
                        destination.SetLong(destinationIndex, source.GetLong(sourceIndex))
                    End If
            End Select
        End Sub

        ''' <summary>Element-by-element converting copy used when the storage types differ.</summary>
        Friend Shared Sub ConvertCopy(ByVal source As ManagedStorage, ByVal sourceIndex As Long, ByVal destination As ManagedStorage, ByVal destinationIndex As Long, ByVal count As Long)
            If count <= 0 Then
                Return
            End If
            If sourceIndex < 0 OrElse sourceIndex + count > source.Length Then
                Throw New ArgumentOutOfRangeException(NameOf(sourceIndex))
            End If
            If destinationIndex < 0 OrElse destinationIndex + count > destination.Length Then
                Throw New ArgumentOutOfRangeException(NameOf(destinationIndex))
            End If
            Dim st As StorageType = source.StorageType
            Dim dt As StorageType = destination.StorageType
            If st = StorageType.UTF8 OrElse dt = StorageType.UTF8 Then
                For i As Long = 0 To count - 1
                    destination.SetString(destinationIndex + i, source.GetString(sourceIndex + i))
                Next
                Return
            End If
            ' typed fast paths for the two common conversions
            If st = StorageType.FLOAT AndAlso dt = StorageType.DOUBLE Then
                Dim s As Single() = source.Floats
                Dim d As Double() = destination.Doubles
                Dim so As Integer = CInt(sourceIndex)
                Dim dOff As Integer = CInt(destinationIndex)
                For i As Integer = 0 To CInt(count) - 1
                    d(dOff + i) = s(so + i)
                Next
                Return
            End If
            If st = StorageType.DOUBLE AndAlso dt = StorageType.FLOAT Then
                Dim s As Double() = source.Doubles
                Dim d As Single() = destination.Floats
                Dim so As Integer = CInt(sourceIndex)
                Dim dOff As Integer = CInt(destinationIndex)
                For i As Integer = 0 To CInt(count) - 1
                    d(dOff + i) = CSng(s(so + i))
                Next
                Return
            End If
            Select Case dt
                Case StorageType.FLOAT, StorageType.HALF, StorageType.BFLOAT16
                    For i As Long = 0 To count - 1
                        destination.SetFloat(destinationIndex + i, source.GetFloat(sourceIndex + i))
                    Next
                Case StorageType.DOUBLE
                    For i As Long = 0 To count - 1
                        destination.SetDouble(destinationIndex + i, source.GetDouble(sourceIndex + i))
                    Next
                Case Else
                    If StorageTypes.IsFloatingPoint(st) Then
                        For i As Long = 0 To count - 1
                            destination.SetDouble(destinationIndex + i, source.GetDouble(sourceIndex + i))
                        Next
                    Else
                        For i As Long = 0 To count - 1
                            destination.SetLong(destinationIndex + i, source.GetLong(sourceIndex + i))
                        Next
                    End If
            End Select
        End Sub

#End Region

#Region "Factories"

        ''' <summary>A new zero-filled (False, Nothing for UTF8) storage of the given type and length.</summary>
        ''' <exception cref="ArgumentOutOfRangeException">The length is negative or larger than a .NET array allows.</exception>
        Public Shared Function Create(ByVal type As StorageType, ByVal length As Long) As ManagedStorage
            If length < 0 OrElse length > Integer.MaxValue Then
                Throw New ArgumentOutOfRangeException(NameOf(length), "Storage length must be between 0 and " & Integer.MaxValue & ", was " & length)
            End If
            Select Case type
                Case StorageType.DOUBLE
                    Return New DoubleStorage(length)
                Case StorageType.FLOAT
                    Return New FloatStorage(length)
                Case StorageType.HALF
                    Return New HalfStorage(length)
                Case StorageType.BFLOAT16
                    Return New BFloat16Storage(length)
                Case StorageType.LONG
                    Return New Int64Storage(length)
                Case StorageType.INT
                    Return New Int32Storage(length)
                Case StorageType.SHORT
                    Return New Int16Storage(length)
                Case StorageType.BYTE
                    Return New Int8Storage(length)
                Case StorageType.UBYTE
                    Return New UInt8Storage(length)
                Case StorageType.UINT16
                    Return New UInt16Storage(length)
                Case StorageType.UINT32
                    Return New UInt32Storage(length)
                Case StorageType.UINT64
                    Return New UInt64Storage(length)
                Case StorageType.BOOL
                    Return New BoolStorage(length)
                Case StorageType.UTF8
                    Return New Utf8Storage(length)
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(type))
            End Select
        End Function

        ''' <summary>A new storage of the given type holding the converted values of a .NET array (always a copy; see <see cref="CopyFrom(Array, Long, Long, Long)"/>).</summary>
        Public Shared Function Create(ByVal type As StorageType, ByVal source As Array) As ManagedStorage
            If source Is Nothing Then
                Throw New ArgumentNullException(NameOf(source))
            End If
            Dim result As ManagedStorage = Create(type, source.LongLength)
            result.CopyFrom(source, 0, 0, source.LongLength)
            Return result
        End Function

        ''' <summary>
        ''' A storage over an existing array, its type taken from the element type (<c>UShort()</c> gives
        ''' UINT16). The array is shared, not copied.
        ''' </summary>
        Public Shared Function Wrap(ByVal data As Array) As ManagedStorage
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            If data.Rank <> 1 Then
                Throw New ArgumentException("Only one-dimensional arrays can back a storage", NameOf(data))
            End If
            ' compare the exact element type: the CLR lets a UShort() pass a cast to Short() (and Byte()/SByte(), ...)
            Dim et As Type = data.GetType().GetElementType()
            If et Is GetType(Single) Then
                Return New FloatStorage(DirectCast(data, Single()))
            ElseIf et Is GetType(Double) Then
                Return New DoubleStorage(DirectCast(data, Double()))
            ElseIf et Is GetType(SByte) Then
                Return New Int8Storage(DirectCast(data, SByte()))
            ElseIf et Is GetType(Byte) Then
                Return New UInt8Storage(DirectCast(data, Byte()))
            ElseIf et Is GetType(Short) Then
                Return New Int16Storage(DirectCast(data, Short()))
            ElseIf et Is GetType(UShort) Then
                Return New UInt16Storage(DirectCast(data, UShort()))
            ElseIf et Is GetType(Integer) Then
                Return New Int32Storage(DirectCast(data, Integer()))
            ElseIf et Is GetType(UInteger) Then
                Return New UInt32Storage(DirectCast(data, UInteger()))
            ElseIf et Is GetType(Long) Then
                Return New Int64Storage(DirectCast(data, Long()))
            ElseIf et Is GetType(ULong) Then
                Return New UInt64Storage(DirectCast(data, ULong()))
            ElseIf et Is GetType(Boolean) Then
                Return New BoolStorage(DirectCast(data, Boolean()))
            ElseIf et Is GetType(String) Then
                Return New Utf8Storage(DirectCast(data, String()))
            End If
            Throw New ArgumentException("No storage type for arrays of " & data.GetType().GetElementType().FullName, NameOf(data))
        End Function

        ''' <summary>A FLOAT storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As Single(), Optional ByVal copy As Boolean = False) As FloatStorage
            Return New FloatStorage(CopyIf(data, copy))
        End Function

        ''' <summary>A DOUBLE storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As Double(), Optional ByVal copy As Boolean = False) As DoubleStorage
            Return New DoubleStorage(CopyIf(data, copy))
        End Function

        ''' <summary>A BYTE (INT8) storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As SByte(), Optional ByVal copy As Boolean = False) As Int8Storage
            Return New Int8Storage(CopyIf(data, copy))
        End Function

        ''' <summary>A UBYTE (UINT8) storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As Byte(), Optional ByVal copy As Boolean = False) As UInt8Storage
            Return New UInt8Storage(CopyIf(data, copy))
        End Function

        ''' <summary>A SHORT (INT16) storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As Short(), Optional ByVal copy As Boolean = False) As Int16Storage
            Return New Int16Storage(CopyIf(data, copy))
        End Function

        ''' <summary>
        ''' A UINT16 storage over <paramref name="data"/>, or a HALF or BFLOAT16 storage when
        ''' <paramref name="type"/> says so (the values are then bit patterns). Shared unless <paramref name="copy"/> is True.
        ''' </summary>
        Public Shared Function FromArray(ByVal data As UShort(), Optional ByVal type As StorageType = StorageType.UINT16, Optional ByVal copy As Boolean = False) As ManagedStorage
            Select Case type
                Case StorageType.UINT16
                    Return New UInt16Storage(CopyIf(data, copy))
                Case StorageType.HALF
                    Return New HalfStorage(CopyIf(data, copy))
                Case StorageType.BFLOAT16
                    Return New BFloat16Storage(CopyIf(data, copy))
                Case Else
                    Throw New ArgumentException("A UShort() array backs UINT16, HALF or BFLOAT16 storage, not " & type.ToString(), NameOf(type))
            End Select
        End Function

        ''' <summary>An INT (INT32) storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As Integer(), Optional ByVal copy As Boolean = False) As Int32Storage
            Return New Int32Storage(CopyIf(data, copy))
        End Function

        ''' <summary>A UINT32 storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As UInteger(), Optional ByVal copy As Boolean = False) As UInt32Storage
            Return New UInt32Storage(CopyIf(data, copy))
        End Function

        ''' <summary>A LONG (INT64) storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As Long(), Optional ByVal copy As Boolean = False) As Int64Storage
            Return New Int64Storage(CopyIf(data, copy))
        End Function

        ''' <summary>A UINT64 storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As ULong(), Optional ByVal copy As Boolean = False) As UInt64Storage
            Return New UInt64Storage(CopyIf(data, copy))
        End Function

        ''' <summary>A BOOL storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As Boolean(), Optional ByVal copy As Boolean = False) As BoolStorage
            Return New BoolStorage(CopyIf(data, copy))
        End Function

        ''' <summary>A UTF8 storage over <paramref name="data"/> (shared unless <paramref name="copy"/> is True).</summary>
        Public Shared Function FromArray(ByVal data As String(), Optional ByVal copy As Boolean = False) As Utf8Storage
            Return New Utf8Storage(CopyIf(data, copy))
        End Function

        Private Shared Function CopyIf(Of T)(ByVal data As T(), ByVal copy As Boolean) As T()
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            Return If(copy, DirectCast(data.Clone(), T()), data)
        End Function

#End Region

        ''' <summary>Type and length, for debugging.</summary>
        Public Overrides Function ToString() As String
            Return StorageType.ToString() & "[" & Length.ToString(CultureInfo.InvariantCulture) & "]"
        End Function

    End Class

    ''' <summary>
    ''' Common implementation of the storages: the backing array, length, cloning, same-type copies and
    ''' fills. Subclasses supply the element conversions.
    ''' </summary>
    ''' <typeparam name="T">The .NET element type.</typeparam>
    Public MustInherit Class TypedStorage(Of T)
        Inherits ManagedStorage

        ''' <summary>The backing array.</summary>
        Protected ReadOnly _data As T()

        ''' <summary>Wraps an array (not copied).</summary>
        Protected Sub New(ByVal data As T())
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            _data = data
        End Sub

        ''' <summary>The backing array.</summary>
        Public ReadOnly Property Data As T()
            Get
                Return _data
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property Length As Long
            Get
                Return _data.LongLength
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property RawArray As Array
            Get
                Return _data
            End Get
        End Property

        ''' <summary>A storage of the same type over <paramref name="data"/>.</summary>
        Protected MustOverride Function CreateSameType(ByVal data As T()) As TypedStorage(Of T)

        ''' <summary>A double converted to the element type, as <see cref="ManagedStorage.SetDouble"/> stores it.</summary>
        Protected MustOverride Function ConvertDouble(ByVal value As Double) As T

        ''' <summary>A long converted to the element type, as <see cref="ManagedStorage.SetLong"/> stores it.</summary>
        Protected MustOverride Function ConvertLong(ByVal value As Long) As T

        ''' <inheritdoc/>
        Public Overrides Function Clone() As ManagedStorage
            Return CreateSameType(DirectCast(_data.Clone(), T()))
        End Function

        ''' <inheritdoc/>
        Public Overrides Function Clone(ByVal start As Long, ByVal count As Long) As ManagedStorage
            If count < 0 OrElse start < 0 OrElse start + count > _data.LongLength Then
                Throw New ArgumentOutOfRangeException(NameOf(count))
            End If
            Dim copy As T() = New T(CInt(count) - 1) {}
            Array.Copy(_data, start, copy, 0L, count)
            Return CreateSameType(copy)
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub CopyTo(ByVal sourceIndex As Long, ByVal destination As ManagedStorage, ByVal destinationIndex As Long, ByVal count As Long)
            If destination Is Nothing Then
                Throw New ArgumentNullException(NameOf(destination))
            End If
            If destination.StorageType = StorageType Then
                Array.Copy(_data, sourceIndex, destination.RawArray, destinationIndex, count)
                Return
            End If
            ConvertCopy(Me, sourceIndex, destination, destinationIndex, count)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Sub Fill(ByVal value As Double, ByVal start As Long, ByVal count As Long)
            FillWith(ConvertDouble(value), start, count)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Sub Fill(ByVal value As Long, ByVal start As Long, ByVal count As Long)
            FillWith(ConvertLong(value), start, count)
        End Sub

        ''' <summary>Sets <paramref name="count"/> elements from <paramref name="start"/> to a value of the element type.</summary>
        Public Sub FillWith(ByVal value As T, ByVal start As Long, ByVal count As Long)
            If count < 0 OrElse start < 0 OrElse start + count > _data.LongLength Then
                Throw New ArgumentOutOfRangeException(NameOf(count))
            End If
            Dim d As T() = _data
            Dim s As Integer = CInt(start)
            Dim e As Integer = CInt(start + count)
            For i As Integer = s To e - 1
                d(i) = value
            Next
        End Sub

    End Class

End Namespace
