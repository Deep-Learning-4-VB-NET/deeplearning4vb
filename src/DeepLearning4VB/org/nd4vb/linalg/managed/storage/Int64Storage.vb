' DeepLearning4VB: LONG storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>LONG (INT64) storage: signed 64-bit values in a <c>Long()</c>.</summary>
    Public NotInheritable Class Int64Storage
        Inherits TypedStorage(Of Long)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New Long(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As Long())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.LONG
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property Longs As Long()
            Get
                Return _data
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides Function GetDouble(ByVal index As Long) As Double
            Return CDbl(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetDouble(ByVal index As Long, ByVal value As Double)
            _data(CInt(index)) = NumericCasts.DoubleToLong(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = value
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetFloat(ByVal index As Long) As Single
            Return CSng(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetFloat(ByVal index As Long, ByVal value As Single)
            _data(CInt(index)) = NumericCasts.DoubleToLong(value)
        End Sub

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As Long()) As TypedStorage(Of Long)
            Return New Int64Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As Long
            Return NumericCasts.DoubleToLong(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As Long
            Return value
        End Function

    End Class

End Namespace
