Imports System
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' An op: one of the legacy operation types (transform, scalar, pairwise, broadcast, reduce, ...) applied to
    ''' the arrays x, y (optional) and z.
    ''' <para>
    ''' Differences from Java: the setters <c>setX</c>, <c>setY</c>, <c>setZ</c> and <c>setExtraArgs</c> stay methods
    ''' (properties named <c>X</c>/<c>Y</c>/<c>Z</c>/<c>ExtraArgs</c> would clash with <see cref="x"/>, <see cref="y"/>,
    ''' <see cref="z"/> and <see cref="extraArgs"/>: VB names are case-insensitive); <c>extraArgsBuff()</c>, which
    ''' returned a <c>java.nio</c> buffer view of the extra arguments, is deleted (use <see cref="extraArgs"/> or
    ''' <see cref="extraArgsDataBuff"/>).
    ''' </para>
    ''' </summary>
    Public Interface Op

        Enum Type
            SCALAR
            SCALAR_BOOL
            TRANSFORM_SAME
            TRANSFORM_FLOAT
            TRANSFORM_ANY
            TRANSFORM_BOOL
            TRANSFORM_STRICT
            PAIRWISE
            PAIRWISE_BOOL
            SPECIAL
            BROADCAST
            BROADCAST_BOOL
            REDUCE_LONG
            REDUCE_SAME
            REDUCE_FLOAT
            REDUCE_BOOL
            INDEXREDUCE
            VARIANCE
            REDUCE3
            GRID
            META
            AGGREGATION
            CUSTOM
            GRADIENT
            CONDITIONAL
            [LOOP]
            LOOP_COND
            [RETURN]
            RANDOM
            SUMMARYSTATS
            LOGIC
        End Enum

        ''' <summary>
        ''' Returns the extra args as a data buffer
        ''' </summary>
        Function extraArgsDataBuff(ByVal bufferType As DataType) As DataBuffer

        ''' <summary>
        ''' An op number
        ''' </summary>
        Function opNum() As Integer

        ''' <summary>
        ''' The opName of this operation
        ''' </summary>
        ''' <returns> the opName of this operation </returns>
        Function opName() As String

        ''' <summary>
        ''' The origin ndarray
        ''' </summary>
        ''' <returns> the origin ndarray </returns>
        Function x() As INDArray

        ''' <summary>
        ''' The pairwise op ndarray
        ''' </summary>
        ''' <returns> the pairwise op ndarray </returns>
        Function y() As INDArray

        ''' <summary>
        ''' The resulting ndarray
        ''' </summary>
        ''' <returns> the resulting ndarray </returns>
        Function z() As INDArray

        ''' <summary>
        ''' Extra arguments
        ''' </summary>
        ''' <returns> the extra arguments </returns>
        Function extraArgs() As Object()

        ''' <summary>
        ''' set x (the input ndarray) </summary>
        ''' <param name="x"> </param>
        Sub setX(ByVal x As INDArray)

        ''' <summary>
        ''' set z (the solution ndarray) </summary>
        ''' <param name="z"> </param>
        Sub setZ(ByVal z As INDArray)

        ''' <summary>
        ''' set y(the pairwise ndarray) </summary>
        ''' <param name="y"> </param>
        Sub setY(ByVal y As INDArray)

        ''' <param name="extraArgs"> </param>
        Sub setExtraArgs(ByVal extraArgs As Object())

        ''' <summary>
        ''' Converts this op to be a <see cref="CustomOp"/>
        ''' A <see cref="CustomOp"/> is a more flexible op
        ''' meant for multiple inputs and outputs.
        ''' The default implementation in <c>BaseOp</c>
        ''' converts a simple op to a multi input/output operation
        ''' by mapping the x and y on to inputs , the op opName
        ''' and the z on to outputs. </summary>
        ''' <returns> the equivalent <see cref="CustomOp"/> </returns>
        Function toCustomOp() As CustomOp

        ''' <summary>
        ''' Clear the input and output INDArrays, if any are set
        ''' </summary>
        Sub clearArrays()

    End Interface

End Namespace
