' DeepLearning4VB: BYTE storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>BYTE (INT8) storage: signed 8-bit values in an <c>SByte()</c>.</summary>
    Public NotInheritable Class Int8Storage
        Inherits TypedStorage(Of SByte)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New SByte(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As SByte())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.BYTE
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property SBytes As SByte()
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
            _data(CInt(index)) = NumericCasts.DoubleToSByte(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = NumericCasts.LongToSByte(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetInt(ByVal index As Long) As Integer
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As SByte()) As TypedStorage(Of SByte)
            Return New Int8Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As SByte
            Return NumericCasts.DoubleToSByte(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As SByte
            Return NumericCasts.LongToSByte(value)
        End Function

    End Class

End Namespace
