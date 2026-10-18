Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.BaseOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op

Namespace org.nd4vb.linalg.api.ops.grid

    ''' <summary>
    ''' The arrays, strides, lengths and orders of one op of a grid (Java <c>GridPointers</c>, used by the CUDA grid
    ''' executioner). The JavaCPP <c>Pointer</c> fields of the Java class (<c>x</c>, <c>y</c>, <c>z</c>,
    ''' <c>xShapeInfo</c>, <c>yShapeInfo</c>, <c>zShapeInfo</c>, <c>dimensions</c>, <c>tadShape</c>,
    ''' <c>tadOffsets</c>, <c>extraArgs</c>) and therefore Lombok's all-arguments constructor are deleted as
    ''' native-only. The op type is <see cref="BaseOp.getOpType(Op)"/>, Nothing for other ops, as in Java. Java's
    ''' Lombok getter <c>getType()</c> shadows <see cref="Object.GetType"/>.
    ''' </summary>
    Public Class GridPointers

        Private type As Op.Type?
        Private opNum As Integer
        Private dtype As DataType
        Private opX As INDArray
        Private opY As INDArray
        Private opZ As INDArray
        Private xStride As Long = -1
        Private yStride As Long = -1
        Private zStride As Long = -1
        Private xLength As Long = 0
        Private yLength As Long = 0
        Private zLength As Long = 0
        Private xOrder As Char
        Private yOrder As Char
        Private zOrder As Char
        Private dimensionsLength As Integer = 0

        Public Sub New()
        End Sub

        Public Sub New(ByVal op As Op, ParamArray ByVal dimensions() As Integer)
            Me.type = BaseOp.getOpType(op)
            Me.dtype = op.x().data().dataType()
            Me.opNum = op.opNum()

            Me.opX = op.x()
            Me.opZ = op.z()

            Me.xLength = op.x().length()
            Me.zLength = op.z().length()

            Me.xOrder = op.x().ordering()
            Me.zOrder = op.z().ordering()

            Me.xStride = op.x().elementWiseStride()
            Me.zStride = op.z().elementWiseStride()
            If op.y() IsNot Nothing Then
                Me.yStride = op.y().elementWiseStride()
                Me.yLength = op.y().length()
                Me.yOrder = op.y().ordering()
                Me.opY = op.y()
            End If

            If dimensions IsNot Nothing Then
                Me.dimensionsLength = dimensions.Length
            End If
        End Sub


        Public Overridable Shadows Function [getType]() As Op.Type?
            Return type
        End Function

        Public Overridable Sub setType(ByVal type As Op.Type?)
            Me.type = type
        End Sub

        Public Overridable Function getOpNum() As Integer
            Return opNum
        End Function

        Public Overridable Sub setOpNum(ByVal opNum As Integer)
            Me.opNum = opNum
        End Sub

        Public Overridable Function getDtype() As DataType
            Return dtype
        End Function

        Public Overridable Sub setDtype(ByVal dtype As DataType)
            Me.dtype = dtype
        End Sub

        Public Overridable Function getOpX() As INDArray
            Return opX
        End Function

        Public Overridable Sub setOpX(ByVal opX As INDArray)
            Me.opX = opX
        End Sub

        Public Overridable Function getOpY() As INDArray
            Return opY
        End Function

        Public Overridable Sub setOpY(ByVal opY As INDArray)
            Me.opY = opY
        End Sub

        Public Overridable Function getOpZ() As INDArray
            Return opZ
        End Function

        Public Overridable Sub setOpZ(ByVal opZ As INDArray)
            Me.opZ = opZ
        End Sub

        Public Overridable Function getXStride() As Long
            Return xStride
        End Function

        Public Overridable Sub setXStride(ByVal xStride As Long)
            Me.xStride = xStride
        End Sub

        Public Overridable Function getYStride() As Long
            Return yStride
        End Function

        Public Overridable Sub setYStride(ByVal yStride As Long)
            Me.yStride = yStride
        End Sub

        Public Overridable Function getZStride() As Long
            Return zStride
        End Function

        Public Overridable Sub setZStride(ByVal zStride As Long)
            Me.zStride = zStride
        End Sub

        Public Overridable Function getXLength() As Long
            Return xLength
        End Function

        Public Overridable Sub setXLength(ByVal xLength As Long)
            Me.xLength = xLength
        End Sub

        Public Overridable Function getYLength() As Long
            Return yLength
        End Function

        Public Overridable Sub setYLength(ByVal yLength As Long)
            Me.yLength = yLength
        End Sub

        Public Overridable Function getZLength() As Long
            Return zLength
        End Function

        Public Overridable Sub setZLength(ByVal zLength As Long)
            Me.zLength = zLength
        End Sub

        Public Overridable Function getXOrder() As Char
            Return xOrder
        End Function

        Public Overridable Sub setXOrder(ByVal xOrder As Char)
            Me.xOrder = xOrder
        End Sub

        Public Overridable Function getYOrder() As Char
            Return yOrder
        End Function

        Public Overridable Sub setYOrder(ByVal yOrder As Char)
            Me.yOrder = yOrder
        End Sub

        Public Overridable Function getZOrder() As Char
            Return zOrder
        End Function

        Public Overridable Sub setZOrder(ByVal zOrder As Char)
            Me.zOrder = zOrder
        End Sub

        Public Overridable Function getDimensionsLength() As Integer
            Return dimensionsLength
        End Function

        Public Overridable Sub setDimensionsLength(ByVal dimensionsLength As Integer)
            Me.dimensionsLength = dimensionsLength
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As GridPointers = TryCast(obj, GridPointers)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return Object.Equals(type, o.type) AndAlso opNum.Equals(o.opNum) AndAlso dtype.Equals(o.dtype) AndAlso Object.Equals(opX, o.opX) AndAlso Object.Equals(opY, o.opY) AndAlso Object.Equals(opZ, o.opZ) AndAlso xStride.Equals(o.xStride) AndAlso yStride.Equals(o.yStride) AndAlso zStride.Equals(o.zStride) AndAlso xLength.Equals(o.xLength) AndAlso yLength.Equals(o.yLength) AndAlso zLength.Equals(o.zLength) AndAlso xOrder.Equals(o.xOrder) AndAlso yOrder.Equals(o.yOrder) AndAlso zOrder.Equals(o.zOrder) AndAlso dimensionsLength.Equals(o.dimensionsLength)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + If(type Is Nothing, 43, type.GetHashCode())
            h = h * 59 + opNum.GetHashCode()
            h = h * 59 + dtype.GetHashCode()
            h = h * 59 + If(opX Is Nothing, 43, opX.GetHashCode())
            h = h * 59 + If(opY Is Nothing, 43, opY.GetHashCode())
            h = h * 59 + If(opZ Is Nothing, 43, opZ.GetHashCode())
            h = h * 59 + xStride.GetHashCode()
            h = h * 59 + yStride.GetHashCode()
            h = h * 59 + zStride.GetHashCode()
            h = h * 59 + xLength.GetHashCode()
            h = h * 59 + yLength.GetHashCode()
            h = h * 59 + zLength.GetHashCode()
            h = h * 59 + xOrder.GetHashCode()
            h = h * 59 + yOrder.GetHashCode()
            h = h * 59 + zOrder.GetHashCode()
            h = h * 59 + dimensionsLength.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "GridPointers(" & "type=" & Objects.toString(type) & ", opNum=" & opNum.ToString() & ", dtype=" & dtype.ToString() & ", opX=" & Objects.toString(opX) & ", opY=" & Objects.toString(opY) & ", opZ=" & Objects.toString(opZ) & ", xStride=" & xStride.ToString() & ", yStride=" & yStride.ToString() & ", zStride=" & zStride.ToString() & ", xLength=" & xLength.ToString() & ", yLength=" & yLength.ToString() & ", zLength=" & zLength.ToString() & ", xOrder=" & xOrder.ToString() & ", yOrder=" & yOrder.ToString() & ", zOrder=" & zOrder.ToString() & ", dimensionsLength=" & dimensionsLength.ToString() & ")"
        End Function

    End Class

End Namespace
