' DeepLearning4VB: TEMPORARY STAND-IN for the ND4VB factory class Nd4vb (Java's org.nd4j.linalg.factory.Nd4j).
' Phase 2 wave 2 code calls these members; phase 2 wave 3 replaces this file with the real port of
' unported/nd4j/.../linalg/factory/Nd4j.vb. Every member throws NotImplementedException (EPS_THRESHOLD and
' PrecisionBoostAllowed hold values). Signatures follow the converter text of Nd4j.vb: Java getters getX() are
' Shared ReadOnly properties named X, Java Number is Object, varargs are ParamArray.

Imports System
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer.factory
Imports DeepLearning4VB.org.nd4vb.linalg.api.concurrency
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory
Imports DeepLearning4VB.org.nd4vb.linalg.api.ndarray
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner
Imports DeepLearning4VB.org.nd4vb.linalg.cache
Imports System.Collections.Generic

Namespace org.nd4vb.linalg.factory

    ''' <summary>Temporary stand-in for the ND4VB factory (phase 2 wave 3 ports the real class).</summary>
    Partial Public Class Nd4vb

        Private Shared Function NotYet() As System.Exception
            Return New NotImplementedException("Nd4vb stand-in (phase 2 wave 3)")
        End Function

        Public Shared EPS_THRESHOLD As Double = 0.00001

        Public Shared ReadOnly Property Executioner As OpExecutioner
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property DataBufferFactory As DataBufferFactory
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property Environment As Environment
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property ShapeInfoProvider As ShapeInfoProvider
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property ConstantHandler As ConstantHandler
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property AffinityManager As AffinityManager
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property NDArrayFactory As NDArrayFactory
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property MemoryManager As MemoryManager
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property WorkspaceManager As MemoryWorkspaceManager
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared ReadOnly Property TADManager As TADManager
            Get
                Throw NotYet()
            End Get
        End Property

        Public Shared Property PrecisionBoostAllowed As Boolean

        Public Shared Function order() As Char
            Throw NotYet()
        End Function

        Public Shared Function dataType() As DataType
            Throw NotYet()
        End Function

        Public Shared Function defaultFloatingPointType() As DataType
            Throw NotYet()
        End Function

        Public Shared Function sizeOfDataType() As Integer
            Throw NotYet()
        End Function

        Public Shared Function sizeOfDataType(ByVal dtype As DataType) As Integer
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal value As Object) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal dataType As DataType, ByVal value As Object) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal value As Double) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal value As Single) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal value As Boolean) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal value As Integer) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal value As Long) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function scalar(ByVal value As String) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function empty() As INDArray
            Throw NotYet()
        End Function

        Public Shared Function empty(ByVal type As DataType) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function exec(ByVal op As Op) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function exec(ByVal op As Op, ByVal context As OpContext) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function exec(ByVal op As CustomOp) As INDArray()
            Throw NotYet()
        End Function

        Public Shared Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray()
            Throw NotYet()
        End Function

    

        Public Shared Function create(ParamArray ByVal shape() As Long) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function create(ByVal shape() As Long, ByVal ordering As Char) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function create(ByVal data() As Double, ByVal shape() As Integer) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function create(ByVal data() As Single, ByVal shape() As Integer) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function create(ByVal dataType As DataType, ByVal shape() As Long, ByVal strides() As Long, ByVal ordering As Char) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function create(ByVal data As DataBuffer, ByVal newShape() As Long, ByVal newStride() As Long, ByVal offset As Long, ByVal ordering As Char) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function createUninitialized(ByVal type As DataType, ByVal shape() As Long, ByVal ordering As Char) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function createBuffer(ByVal underlyingBuffer As DataBuffer, ByVal offset As Long, ByVal length As Long) As DataBuffer
            Throw NotYet()
        End Function

        Public Shared Function createBuffer(ByVal data() As Integer) As DataBuffer
            Throw NotYet()
        End Function

        Public Shared Function createFromArray(ParamArray ByVal array() As Integer) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function getStrides(ByVal shape() As Integer, ByVal order As Char) As Integer()
            Throw NotYet()
        End Function

        Public Shared Function getStrides(ByVal shape() As Long, ByVal order As Char) As Long()
            Throw NotYet()
        End Function

        Public Shared Function readBinary(ByVal read As String) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function readTxt(ByVal filePath As String) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function createFromNpyFile(ByVal file As String) As INDArray
            Throw NotYet()
        End Function

        Public Shared Function createFromNpzFile(ByVal file As String) As IDictionary(Of String, INDArray)
            Throw NotYet()
        End Function

    

    End Class

End Namespace
