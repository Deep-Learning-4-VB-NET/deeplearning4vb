' DeepLearning4VB: SHORT storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>SHORT (INT16) storage: signed 16-bit values in a <c>Short()</c>.</summary>
    Public NotInheritable Class Int16Storage
        Inherits TypedStorage(Of Short)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New Short(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As Short())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.SHORT
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property Shorts As Short()
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
            _data(CInt(index)) = NumericCasts.DoubleToShort(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = NumericCasts.LongToShort(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetInt(ByVal index As Long) As Integer
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As Short()) As TypedStorage(Of Short)
            Return New Int16Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As Short
            Return NumericCasts.DoubleToShort(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As Short
            Return NumericCasts.LongToShort(value)
        End Function

    End Class

End Namespace
