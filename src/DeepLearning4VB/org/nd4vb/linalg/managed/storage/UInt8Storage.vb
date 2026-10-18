' DeepLearning4VB: UBYTE storage of the managed ND4VB engine.

Imports System
Imports System.Globalization

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>UBYTE (UINT8) storage: unsigned 8-bit values in a <c>Byte()</c>.</summary>
    Public NotInheritable Class UInt8Storage
        Inherits TypedStorage(Of Byte)

        ''' <summary>A zero-filled storage of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal length As Long)
            MyBase.New(New Byte(CInt(length) - 1) {})
        End Sub

        ''' <summary>A storage over <paramref name="data"/> (shared, not copied).</summary>
        Public Sub New(ByVal data As Byte())
            MyBase.New(data)
        End Sub

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property StorageType As StorageType
            Get
                Return StorageType.UBYTE
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property UBytes As Byte()
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
            _data(CInt(index)) = NumericCasts.DoubleToByte(value)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetLong(ByVal index As Long) As Long
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub SetLong(ByVal index As Long, ByVal value As Long)
            _data(CInt(index)) = CByte(value And &HFFL)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Function GetInt(ByVal index As Long) As Integer
            Return _data(CInt(index))
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function CreateSameType(ByVal data As Byte()) As TypedStorage(Of Byte)
            Return New UInt8Storage(data)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertDouble(ByVal value As Double) As Byte
            Return NumericCasts.DoubleToByte(value)
        End Function

        ''' <inheritdoc/>
        Protected Overrides Function ConvertLong(ByVal value As Long) As Byte
            Return CByte(value And &HFFL)
        End Function

    End Class

End Namespace
