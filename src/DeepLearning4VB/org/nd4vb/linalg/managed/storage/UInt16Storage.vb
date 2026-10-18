' DeepLearning4VB: UINT16 storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>UINT16 storage: unsigned 16-bit values in a <c>UShort()</c>.</summary>
    Public NotInheritable Class UInt16Storage
        Inherits TypedStorage(Of UShort)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New UShort(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As UShort())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.UINT16
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property UShorts As UShort()
            Get
                Return _data
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides Function GetDouble(ByVal index As Long) As Double
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetDouble(ByVal index As Long, ByVal value As Double)
            _data(CInt(index)) = NumericCasts.DoubleToUShort(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = CUShort(value And &HFFFFL)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetInt(ByVal index As Long) As Integer
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As UShort()) As TypedStorage(Of UShort)
            Return New UInt16Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As UShort
            Return NumericCasts.DoubleToUShort(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As UShort
            Return CUShort(value And &HFFFFL)
        End Function

    End Class

End Namespace
