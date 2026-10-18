' DeepLearning4VB: BFLOAT16 storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>BFLOAT16 storage: bfloat16 bit patterns (the upper half of a FLOAT) in a <c>UShort()</c>, converted with <see cref="NumericCasts.SingleToBFloat16"/> and <see cref="NumericCasts.BFloat16ToSingle"/>.</summary>
    Public NotInheritable Class BFloat16Storage
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
                Return StorageType.BFLOAT16
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property HalfBits As UShort()
            Get
                Return _data
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides Function GetDouble(ByVal index As Long) As Double
            Return NumericCasts.BFloat16ToSingle(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetDouble(ByVal index As Long, ByVal value As Double)
            _data(CInt(index)) = NumericCasts.SingleToBFloat16(CSng(value))
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return NumericCasts.DoubleToLong(NumericCasts.BFloat16ToSingle(_data(CInt(index))))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = NumericCasts.SingleToBFloat16(CSng(value))
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetFloat(ByVal index As Long) As Single
            Return NumericCasts.BFloat16ToSingle(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetFloat(ByVal index As Long, ByVal value As Single)
            _data(CInt(index)) = NumericCasts.SingleToBFloat16(value)
        End Sub

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As UShort()) As TypedStorage(Of UShort)
            Return New BFloat16Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As UShort
            Return NumericCasts.SingleToBFloat16(CSng(value))
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As UShort
            Return NumericCasts.SingleToBFloat16(CSng(value))
        End Function

    End Class

End Namespace
