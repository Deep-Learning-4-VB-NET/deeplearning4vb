' DeepLearning4VB: FLOAT storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>FLOAT storage: 32-bit IEEE values in a <c>Single()</c>.</summary>
    Public NotInheritable Class FloatStorage
        Inherits TypedStorage(Of Single)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New Single(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As Single())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.FLOAT
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property Floats As Single()
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
            _data(CInt(index)) = CSng(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return NumericCasts.DoubleToLong(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = CSng(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetFloat(ByVal index As Long) As Single
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetFloat(ByVal index As Long, ByVal value As Single)
            _data(CInt(index)) = value
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetInt(ByVal index As Long) As Integer
            Return NumericCasts.DoubleToInt(_data(CInt(index)))
        End Function

        ''' <inheritdoc/>
        Public Overrides Function GetBoolean(ByVal index As Long) As Boolean
            Return _data(CInt(index)) <> 0.0F
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As Single()) As TypedStorage(Of Single)
            Return New FloatStorage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As Single
            Return CSng(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As Single
            Return CSng(value)
        End Function

    End Class

End Namespace
