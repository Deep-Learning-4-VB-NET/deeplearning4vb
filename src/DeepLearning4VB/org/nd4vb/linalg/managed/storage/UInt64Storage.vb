' DeepLearning4VB: UINT64 storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>UINT64 storage: unsigned 64-bit values in a <c>ULong()</c>. <see cref="ManagedStorage.GetDouble"/> reads the unsigned value; <see cref="ManagedStorage.GetLong"/> returns the bits as a signed long, as Java does.</summary>
    Public NotInheritable Class UInt64Storage
        Inherits TypedStorage(Of ULong)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New ULong(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As ULong())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.UINT64
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property ULongs As ULong()
            Get
                Return _data
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides Function GetDouble(ByVal index As Long) As Double
            Return NumericCasts.ULongToDouble(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetDouble(ByVal index As Long, ByVal value As Double)
            _data(CInt(index)) = NumericCasts.DoubleToULong(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return NumericCasts.ULongToLong(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = NumericCasts.LongToULong(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetFloat(ByVal index As Long) As Single
            Return NumericCasts.ULongToSingle(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Function GetString(ByVal index As Long) As String
            Return _data(CInt(index)).ToString(CultureInfo.InvariantCulture)
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetString(ByVal index As Long, ByVal value As String)
            Dim u As ULong
            If value IsNot Nothing AndAlso ULong.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, u) Then
                _data(CInt(index)) = u
            Else
                MyBase.SetString(index, value)
            End If
        End Sub

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As ULong()) As TypedStorage(Of ULong)
            Return New UInt64Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As ULong
            Return NumericCasts.DoubleToULong(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As ULong
            Return NumericCasts.LongToULong(value)
        End Function

    End Class

End Namespace
