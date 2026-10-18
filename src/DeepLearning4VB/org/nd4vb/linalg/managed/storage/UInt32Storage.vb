' DeepLearning4VB: UINT32 storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>UINT32 storage: unsigned 32-bit values in a <c>UInteger()</c>.</summary>
    Public NotInheritable Class UInt32Storage
        Inherits TypedStorage(Of UInteger)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New UInteger(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As UInteger())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.UINT32
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property UInts As UInteger()
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
            _data(CInt(index)) = NumericCasts.DoubleToUInteger(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = CUInt(value And &HFFFFFFFFL)
        End Sub

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As UInteger()) As TypedStorage(Of UInteger)
            Return New UInt32Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As UInteger
            Return NumericCasts.DoubleToUInteger(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As UInteger
            Return CUInt(value And &HFFFFFFFFL)
        End Function

    End Class

End Namespace
