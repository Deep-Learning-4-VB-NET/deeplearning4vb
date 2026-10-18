Imports System
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DT = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer

Namespace org.nd4vb.linalg.cache

    ''' <summary>
    ''' A minimal array for the buffers agent's tests (wave 2 has no <c>BaseNDArray</c>): a data buffer, a shape-info buffer,
    ''' the view flag and the attached state derived from the data buffer; <c>assign(Object)</c>, <c>detach()</c> and
    ''' <c>leverageTo</c> are recorded.
    ''' </summary>
    Friend Class FakeArray
        Inherits INDArrayStub

        Public ReadOnly Buffer As DataBuffer
        Public ReadOnly ShapeInfo As Long()
        Public IsViewValue As Boolean
        Public AssignedValue As Object
        Public LeveragedTo As String
        Public Detached As Boolean

        ''' <summary>An array over <paramref name="buffer"/> with the shape information <paramref name="shapeInfo"/>.</summary>
        Public Sub New(ByVal buffer As DataBuffer, ByVal shapeInfo As Long())
            Me.Buffer = buffer
            Me.ShapeInfo = shapeInfo
        End Sub

        ''' <summary>A rank-1 'c' array over <paramref name="buffer"/>.</summary>
        Public Sub New(ByVal buffer As DataBuffer)
            Me.New(buffer, New Long() {1, buffer.length(), 1, 0, 1, AscW("c"c)})
        End Sub

        Public Overrides Function data() As DataBuffer
            Return Buffer
        End Function

        Public Overrides Function shapeInfoDataBuffer() As DataBuffer
            Return New ManagedDataBuffer(DT.LONG, ShapeInfo)
        End Function

        Public Overrides Function shapeInfoJava() As Long()
            Return ShapeInfo
        End Function

        Public Overrides Function length() As Long
            Return Buffer.length()
        End Function

        Public Overrides Function dataType() As DataType
            Return Buffer.dataType()
        End Function

        Public Overrides Function ordering() As Char
            Return ChrW(CInt(ShapeInfo(ShapeInfo.Length - 1)))
        End Function

        Public Overrides ReadOnly Property View As Boolean
            Get
                Return IsViewValue
            End Get
        End Property

        Public Overrides ReadOnly Property Attached As Boolean
            Get
                Return Buffer.Attached OrElse (Buffer.underlyingDataBuffer() IsNot Nothing AndAlso Buffer.underlyingDataBuffer().Attached)
            End Get
        End Property

        Public Overrides ReadOnly Property Compressed As Boolean
            Get
                Return Buffer.dataType() = DT.COMPRESSED
            End Get
        End Property

        Public Overrides Function assign(ByVal value As Object) As INDArray
            AssignedValue = value
            Return Me
        End Function

        Public Overrides Function detach() As INDArray
            Detached = True
            Return Me
        End Function

        Public Overrides Function leverageTo(ByVal id As String, ByVal enforceExistence As Boolean) As INDArray
            LeveragedTo = id
            Return Me
        End Function

    End Class

End Namespace
