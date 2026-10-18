' DeepLearning4VB: HALF storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>HALF storage: IEEE half-precision bit patterns in a <c>UShort()</c>, converted with <see cref="NumericCasts.SingleToHalf"/> and <see cref="NumericCasts.HalfToSingle"/>.</summary>
    Public NotInheritable Class HalfStorage
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
                Return StorageType.HALF
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
            Return NumericCasts.HalfToSingle(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetDouble(ByVal index As Long, ByVal value As Double)
            _data(CInt(index)) = NumericCasts.SingleToHalf(CSng(value))
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return NumericCasts.DoubleToLong(NumericCasts.HalfToSingle(_data(CInt(index))))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = NumericCasts.SingleToHalf(CSng(value))
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetFloat(ByVal index As Long) As Single
            Return NumericCasts.HalfToSingle(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetFloat(ByVal index As Long, ByVal value As Single)
            _data(CInt(index)) = NumericCasts.SingleToHalf(value)
        End Sub

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As UShort()) As TypedStorage(Of UShort)
            Return New HalfStorage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As UShort
            Return NumericCasts.SingleToHalf(CSng(value))
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As UShort
            Return NumericCasts.SingleToHalf(CSng(value))
        End Function

    End Class

End Namespace
