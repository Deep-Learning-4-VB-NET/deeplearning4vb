' DeepLearning4VB: BOOL storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>BOOL storage: a <c>Boolean()</c>; numerically 0/1, any non-zero value (NaN included) stores True.</summary>
    Public NotInheritable Class BoolStorage
        Inherits TypedStorage(Of Boolean)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New Boolean(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As Boolean())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.BOOL
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property Booleans As Boolean()
            Get
                Return _data
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides Function GetDouble(ByVal index As Long) As Double
            Return If(_data(CInt(index)), 1.0, 0.0)
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetDouble(ByVal index As Long, ByVal value As Double)
            _data(CInt(index)) = value <> 0.0
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return If(_data(CInt(index)), 1L, 0L)
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = value <> 0L
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetBoolean(ByVal index As Long) As Boolean
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetBoolean(ByVal index As Long, ByVal value As Boolean)
            _data(CInt(index)) = value
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetString(ByVal index As Long) As String
            Return If(_data(CInt(index)), "true", "false")
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As Boolean()) As TypedStorage(Of Boolean)
            Return New BoolStorage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As Boolean
            Return value <> 0.0
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As Boolean
            Return value <> 0L
        End Function

    End Class

End Namespace
