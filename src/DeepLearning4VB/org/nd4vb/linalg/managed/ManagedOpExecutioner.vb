' DeepLearning4VB: the op executioner of the managed ND4VB backend (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.ops.NativeOpExecutioner; where that called libnd4j through
' NativeOps, this class runs the managed kernels (org.nd4vb.linalg.managed.kernels) with the op math of the registry
' (org.nd4vb.linalg.managed.ops.ManagedOpMath).

Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Linq
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DifferentialFunction = DeepLearning4VB.org.nd4vb.autodiff.functions.DifferentialFunction
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArrayStatistics
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports DefaultOpExecutioner = DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner.DefaultOpExecutioner
Imports OpExecutioner = DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner.OpExecutioner
Imports OpType = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op.Type
Imports Variance = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.summarystats.Variance
Imports RandomT = DeepLearning4VB.org.nd4vb.linalg.api.rng.Random
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports TadPackT = DeepLearning4VB.org.nd4vb.linalg.api.shape.TadPack
Imports ArrayOptionsHelper = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayOptionsHelper
Imports ConstantHandlerT = DeepLearning4VB.org.nd4vb.linalg.cache.ConstantHandler
Imports TADManagerT = DeepLearning4VB.org.nd4vb.linalg.cache.TADManager
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports ND4VBOpProfilerException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBOpProfilerException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports HashUtil = DeepLearning4VB.org.nd4vb.linalg.util.HashUtil
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports ManagedTADManager = DeepLearning4VB.org.nd4vb.linalg.managed.cache.ManagedTADManager
Imports BroadcastKernels = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.BroadcastKernels
Imports ElementWiseKernels = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ElementWiseKernels
Imports IndexReduceKernels = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.IndexReduceKernels
Imports KernelOptions = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.KernelOptions
Imports KernelTadPack = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.TadPack
Imports ReduceKernels = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ReduceKernels
Imports ShapeInfoHelper = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ShapeInfoHelper
Imports SummaryStatsKernels = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.SummaryStatsKernels
Imports ManagedOpMath = DeepLearning4VB.org.nd4vb.linalg.managed.ops.ManagedOpMath
Imports ManagedStorage = DeepLearning4VB.org.nd4vb.linalg.managed.storage.ManagedStorage
Imports StorageType = DeepLearning4VB.org.nd4vb.linalg.managed.storage.StorageType
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports Properties = DeepLearning4VB.JavaCompat.Properties

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' The op executioner of the managed backend (Java <c>NativeOpExecutioner</c>): it dispatches every op by its type and
    ''' <c>opName()</c> to the op math registry (<see cref="ManagedOpMath"/>) and runs the managed kernels over the op's
    ''' arrays.
    ''' </summary>
    ''' <remarks>
    ''' <para><b>Dispatch.</b> Transforms (same, float, strict, any, bool) run as transforms without y and as pairwise
    ''' ops with y (a y of another shape is broadcast to x's shape with NumPy rules, which covers Java's scalar-y
    ''' <c>pow</c> special case); scalar and scalar bool ops as scalar ops (along dimensions when the op has dimensions:
    ''' y then holds one scalar per tensor); broadcast ops along <c>Dimension</c>; reduce float, same, long and bool,
    ''' reduce3 (including the all-distances mode, <c>ComplexAccumulation</c>), index reductions and summary statistics
    ''' (<c>Variance</c>/<c>StandardDeviation</c> with <c>BiasCorrected</c>) as reductions along dimensions, with the TADs
    ''' of the shared <see cref="ManagedTADManager"/>. Random ops throw <see cref="NotSupportedException"/> (milestone M2).
    ''' Custom ops run by name when the registry has their element-wise math (<see cref="ManagedOpMath.TryGetCustomFamily"/>);
    ''' other custom ops throw <see cref="NotSupportedException"/> naming milestones M3/M4.</para>
    ''' <para><b>Arrays.</b> An array must be backed by a <see cref="ManagedDataBuffer"/> whose element 0 is the array's
    ''' first element (ND4J 1.0.0-M2: a view's <c>data()</c> is a view buffer, and <c>offset()</c> is that buffer's
    ''' offset, so the executioner does not add it); its shape information is <c>shapeInfoJava()</c>. Any rank, order,
    ''' strides and empty arrays work. Missing outputs are allocated through <c>Nd4vb</c> (<c>create</c>,
    ''' <c>createUninitialized</c>, <c>scalar</c>) or <c>INDArray.ulike()</c>, as Java did.</para>
    ''' <para><b>Differences from Java.</b>
    ''' <list type="bullet">
    ''' <item>A reduction of a vector along a dimension of size 1 runs the reduction (Java returned <c>op.noOp()</c>, a copy
    ''' of x, which is wrong for norms, counts and bool reductions).</item>
    ''' <item>A reduction of an empty x fills z with the reduction's empty value (the kernels' result for no elements), and
    ''' the "empty reduce" TF-import case returns the new z (Java returned null).</item>
    ''' <item>A broadcast op without z gets one (x's type, BOOL for broadcast bool ops; Java required z).</item>
    ''' <item>The profiling hook after the op also runs for reductions, broadcasts and scalar ops along dimensions.</item>
    ''' <item><see cref="exec(CustomOp)"/> passes <see cref="NotSupportedException"/> through unwrapped (Java wrapped every
    ''' failure in a <c>RuntimeException</c>, here <see cref="InvalidOperationException"/>).</item>
    ''' <item><see cref="createConstantBuffer(Long(), DataType)"/> and <see cref="createConstantBuffer(Double(), DataType)"/>
    ''' use the constant handler (Java's CPU executioner did not implement them).</item>
    ''' <item><see cref="ElementsThreshold"/> sets <see cref="KernelOptions.ParallelThreshold"/>; <see cref="TadThreshold"/>
    ''' has no managed counterpart and is ignored. <see cref="EnvironmentInformation"/> has no memory bandwidth entry.</item>
    ''' <item>The threshold and bitmap codecs throw <see cref="NotSupportedException"/> (later work).</item>
    ''' </list></para>
    ''' <para>Native-only members not ported: the NativeOps loop and its pointer caches, the MKL-DNN overrides,
    ''' <c>registerGraph</c>/<c>executeGraph</c> on libnd4j graphs, <c>scatterUpdate</c>, the aggregate batches
    ''' (<c>exec(Aggregate)</c> and <c>exec(Batch)</c> throw, as in <see cref="DefaultOpExecutioner"/>).</para>
    ''' </remarks>
    Public Class ManagedOpExecutioner
        Inherits DefaultOpExecutioner

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ManagedOpExecutioner))

        Private ReadOnly tadManagerField As New ManagedTADManager()
        Private constantHandlerField As ConstantHandlerT

        ''' <summary>The custom op descriptors (built on first use).</summary>
        Protected Friend customOps As IDictionary(Of String, CustomOpDescriptor) = Nothing
        Private ReadOnly customOpsLock As New Object()

        ''' <summary>The experimental mode flag (Java field <c>experimentalMode</c>; libnd4j's flag, always off here).</summary>
        Protected Friend experimentalModeField As New AtomicBoolean(False)

        ''' <summary>An executioner that uses <c>Nd4vb.ConstantHandler</c> for constant buffers (looked up on first use).</summary>
        Public Sub New()
        End Sub

        ''' <summary>An executioner with its own constant handler (for tests).</summary>
        Friend Sub New(ByVal constantHandler As ConstantHandlerT)
            Me.constantHandlerField = constantHandler
        End Sub

        ''' <summary>The constant handler: the one given to the constructor, otherwise <c>Nd4vb.ConstantHandler</c>.</summary>
        Protected Friend Overridable ReadOnly Property ConstantHandler As ConstantHandlerT
            Get
                If constantHandlerField Is Nothing Then
                    constantHandlerField = Nd4vbT.ConstantHandler
                End If
                Return constantHandlerField
            End Get
        End Property

#Region "Operands"

        ''' <summary>An array as the kernels read it: a storage, the element offset of the array's first element, the shape info.</summary>
        Friend Structure Operand
            Public ReadOnly Storage As ManagedStorage
            Public ReadOnly Offset As Long
            Public ReadOnly Info As Long()

            Public Sub New(ByVal storage As ManagedStorage, ByVal offset As Long, ByVal info As Long())
                Me.Storage = storage
                Me.Offset = offset
                Me.Info = info
            End Sub

            ''' <summary>The number of elements.</summary>
            Public ReadOnly Property Length As Long
                Get
                    Return ShapeInfoHelper.Length(Info)
                End Get
            End Property
        End Structure

        ''' <summary>
        ''' The kernel view of an array: the storage and <see cref="ManagedDataBuffer.StorageOffset"/> of <c>data()</c> and
        ''' <c>shapeInfoJava()</c>. An empty array without a buffer gets an empty storage of its type.
        ''' </summary>
        ''' <exception cref="ND4VBIllegalStateException">The array is not backed by a managed data buffer.</exception>
        Friend Shared Function operandOf(ByVal arr As INDArray, ByVal name As String) As Operand
            Dim info As Long() = arr.shapeInfoJava()
            Dim buffer As DataBuffer = arr.data()
            If buffer Is Nothing Then
                If ShapeInfoHelper.Length(info) = 0 Then
                    Return New Operand(ManagedStorage.Create(ManagedDataBuffer.toStorageType(arr.dataType()), 0), 0, info)
                End If
                Throw New ND4VBIllegalStateException("Op." & name & " has no data buffer")
            End If
            Dim managed As ManagedDataBuffer = TryCast(buffer, ManagedDataBuffer)
            If managed Is Nothing Then
                Throw New ND4VBIllegalStateException("Op." & name & " is backed by a " & CObj(buffer).GetType().Name & "; the managed backend runs only on arrays backed by a ManagedDataBuffer")
            End If
            Return New Operand(managed.Storage, managed.StorageOffset, info)
        End Function

        ''' <summary>The op's extra arguments as doubles (an empty array when there are none; non-numbers count as 0).</summary>
        Friend Shared Function extraArgsOf(ByVal op As Op) As Double()
            Return toDoubles(op.extraArgs())
        End Function

        Private Shared Function toDoubles(ByVal args() As Object) As Double()
            If args Is Nothing OrElse args.Length = 0 Then
                Return Array.Empty(Of Double)()
            End If
            Dim result(args.Length - 1) As Double
            For i As Integer = 0 To args.Length - 1
                Dim v As Object = args(i)
                If TypeOf v Is IConvertible AndAlso Not TypeOf v Is String AndAlso Not TypeOf v Is Char AndAlso Not TypeOf v Is DateTime Then
                    result(i) = Convert.ToDouble(v, CultureInfo.InvariantCulture)
                End If
            Next i
            Return result
        End Function

        ''' <summary>
        ''' The dimensions of a reduction, index reduction or scalar op: the dimensions as given
        ''' (<c>DifferentialFunction.getDimensions()</c>), otherwise the values of the op's dimensions array; Nothing when
        ''' neither is set.
        ''' </summary>
        Friend Shared Function dimensionsOf(ByVal op As Op, ByVal dimensionsArray As INDArray) As Integer()
            Dim df As DifferentialFunction = TryCast(op, DifferentialFunction)
            Dim given As Integer() = If(df Is Nothing, Nothing, df.getDimensions())
            If given IsNot Nothing Then
                Return given
            End If
            If dimensionsArray IsNot Nothing Then
                Return dimensionsArray.toIntVector()
            End If
            Return Nothing
        End Function

        ''' <summary>True for dimensions that mean the whole array (none, <c>{Integer.MaxValue}</c> or every dimension).</summary>
        Private Shared Function isWhole(ByVal dimension As Integer(), ByVal rank As Integer) As Boolean
            Return ShapeT.wholeArrayDimension(dimension) OrElse dimension.Length = rank
        End Function

        ''' <summary>The number of tensors of x along the (normalized) dimensions.</summary>
        Private Shared Function tadCount(ByVal arr As INDArray, ByVal dimension As Integer()) As Long
            If isWhole(dimension, arr.rank()) Then
                Return 1
            End If
            Dim shape As Long() = arr.shape()
            Dim n As Long = 1
            For d As Integer = 0 To shape.Length - 1
                If Array.IndexOf(dimension, d) < 0 Then
                    n *= shape(d)
                End If
            Next d
            Return n
        End Function

        ''' <summary>The values of a LONG shape or offsets buffer, without a copy when the buffer covers its whole storage.</summary>
        Private Shared Function longsOf(ByVal buffer As DataBuffer) As Long()
            Dim managed As ManagedDataBuffer = TryCast(buffer, ManagedDataBuffer)
            If managed IsNot Nothing AndAlso managed.StorageOffset = 0 Then
                Dim s As ManagedStorage = managed.Storage
                If s.StorageType = StorageType.[LONG] AndAlso s.Length = managed.length() Then
                    Return s.Longs
                End If
            End If
            Return buffer.asLong()
        End Function

        ''' <summary>
        ''' The TADs of x along the normalized dimensions from the shared <see cref="ManagedTADManager"/> (cached), as the
        ''' TAD manager's pack (<paramref name="apiPack"/>) and as the kernels' pack.
        ''' </summary>
        Private Function tadsOf(ByVal x As INDArray, ByVal info As Long(), ByVal dimension As Integer(), ByRef apiPack As TadPackT) As KernelTadPack
            Dim dims As Integer() = If(isWhole(dimension, x.rank()), Array.Empty(Of Integer)(), dimension)
            apiPack = tadManagerField.tadShapeInfoAndOffsets(x, dims)
            Return KernelTadPack.FromParts(info, dims, longsOf(apiPack.getTadShapeInfo()), longsOf(apiPack.getTadOffsets()))
        End Function

        ''' <summary>Sets every element of z to <paramref name="value"/>.</summary>
        Private Shared Sub fill(ByVal z As Operand, ByVal value As Double)
            If z.Length > 0 Then
                ElementWiseKernels.Fill(z.Storage, z.Offset, z.Info, value)
            End If
        End Sub

        ''' <summary>z = f(x, y) for x and z of the same shape: element-wise when y has as many elements, otherwise y broadcast to x's shape.</summary>
        Private Shared Sub pairwiseOrBroadcast(ByVal x As Operand, ByVal y As Operand, ByVal z As Operand, ByVal fn As PairwiseFunction, ByVal extra As Double())
            If z.Length = 0 OrElse x.Length = 0 Then
                Return
            End If
            If y.Length = x.Length Then
                ElementWiseKernels.Pairwise(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, fn, extra)
                Return
            End If
            Dim xShape As Long() = ShapeInfoHelper.GetShape(x.Info)
            Dim yShape As Long() = ShapeInfoHelper.GetShape(y.Info)
            If BroadcastKernels.CanBroadcast(xShape, yShape) AndAlso BroadcastKernels.BroadcastShape(xShape, yShape).SequenceEqual(xShape) Then
                BroadcastKernels.BroadcastPairwise(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, fn, extra)
                Return
            End If
            Throw New ND4VBIllegalStateException("Op.Y of shape " & ArrayToString(yShape) & " has neither the length of Op.X nor a shape that broadcasts to Op.X's shape " & ArrayToString(xShape))
        End Sub

        Private Shared Function randomNotSupported(ByVal op As Op) As NotSupportedException
            Return New NotSupportedException("Random op " & op.opName() & " is not implemented in the managed backend yet (phase 2 milestone M2)")
        End Function

        Private Shared Function customNotSupported(ByVal opName As String) As NotSupportedException
            Return New NotSupportedException(opName & " is not implemented in the managed backend yet (phase 2 milestone M3/M4)")
        End Function

#End Region

#Region "Legacy ops"

        ''' <summary>Executes the op and returns z.</summary>
        Public Overrides Function exec(ByVal op As Op) As INDArray
            Return exec(op, Nothing)
        End Function

        ''' <summary>Executes the op with the arrays of the context (when one is given) and returns z.</summary>
        Public Overrides Function exec(ByVal op As Op, ByVal opContext As OpContext) As INDArray
            checkForCompression(op)

            If TypeOf op Is ScalarOp Then
                exec(DirectCast(op, ScalarOp), opContext)
            ElseIf TypeOf op Is TransformOp Then
                execTransform(DirectCast(op, TransformOp), opContext)
            ElseIf TypeOf op Is ReduceOp Then
                exec(DirectCast(op, ReduceOp), opContext)
            ElseIf TypeOf op Is IndexAccumulation Then
                exec(DirectCast(op, IndexAccumulation), opContext)
            ElseIf TypeOf op Is BroadcastOp Then
                exec(DirectCast(op, BroadcastOp), opContext)
            ElseIf TypeOf op Is RandomOp Then
                Throw randomNotSupported(op)
            End If

            Return getZ(op, opContext)
        End Function

        ''' <summary>Executes an index reduction (argmax and friends) and returns z (LONG indexes within each tensor).</summary>
        Public Overrides Function exec(ByVal op As IndexAccumulation) As INDArray
            Return exec(op, Nothing)
        End Function

        ''' <summary>Executes an index reduction with the arrays of the context (when one is given) and returns z.</summary>
        Public Overridable Overloads Function exec(ByVal op As IndexAccumulation, ByVal oc As OpContext) As INDArray
            checkForCompression(op)

            Dim x As INDArray = getX(op, oc)
            Dim z As INDArray = getZ(op, oc)
            Preconditions.checkNotNull(x, "Op.x() cannot be null: Was null for op %s", op)

            Dim dimension As Integer() = ShapeT.normalizeAxis(x.rank(), dimensionsOf(op, op.dimensions()))
            Dim whole As Boolean = isWhole(dimension, x.rank())

            If x.Empty Then
                Dim shape As Long() = x.shape()
                For d As Integer = 0 To shape.Length - 1
                    If whole OrElse Array.IndexOf(dimension, d) >= 0 Then
                        Preconditions.checkArgument(shape(d) <> 0, "IndexReduce can't be issued along axis with 0 in shape")
                    End If
                Next d
            End If

            Dim keepDims As Boolean = op.KeepDims
            Dim retShape() As Long = ShapeT.reductionShape(x, dimension, True, keepDims)

            If z Is Nothing OrElse x Is z Then
                Dim ret As INDArray = Nd4vbT.createUninitialized(DataType.LONG, retShape)

                setZ(ret, op, oc)
                z = ret
            ElseIf Not retShape.SequenceEqual(z.shape()) Then
                Throw New InvalidOperationException("Z array shape does not match expected return type for op " & op.opName() & ": expected shape " & ArrayToString(retShape) & ", z.shape()=" & ArrayToString(z.shape()))
            End If

            op.validateDataTypes()

            Dim xo As Operand = operandOf(x, "X")
            Dim zo As Operand = operandOf(z, "Z")
            Dim apiPack As TadPackT = Nothing
            Dim tads As KernelTadPack = tadsOf(x, xo.Info, dimension, apiPack)

            Dim st As Long = profilingConfigurableHookIn(op, apiPack.getTadShapeInfo())

            Dim definition As IndexReduceDefinition = ManagedOpMath.IndexReduce(op.opName())
            If zo.Length > 0 Then
                IndexReduceKernels.IndexReduceAlongDimensions(xo.Storage, xo.Offset, xo.Info, zo.Storage, zo.Offset, zo.Info, tads, definition, extraArgsOf(op))
            End If

            profilingConfigurableHookOut(op, oc, st)
            Return getZ(op, oc)
        End Function

        ''' <summary>Executes a variance or standard deviation op (bias-corrected unless <c>BiasCorrected</c> is False).</summary>
        Public Overrides Function exec(ByVal op As Variance) As INDArray
            Return exec(DirectCast(op, ReduceOp))
        End Function

        ''' <summary>Executes a reduction (float, same, long, bool), a reduce3 op or a summary-statistics op and returns z.</summary>
        Public Overrides Function exec(ByVal op As ReduceOp) As INDArray
            Return exec(op, Nothing)
        End Function

        ''' <summary>Executes a reduction with the arrays of the context (when one is given) and returns z.</summary>
        Public Overridable Overloads Function exec(ByVal op As ReduceOp, ByVal oc As OpContext) As INDArray
            Dim x As INDArray = getX(op, oc)
            Dim y As INDArray = getY(op, oc)
            Dim z As INDArray = getZ(op, oc)
            Preconditions.checkNotNull(x, "Op.x() cannot be null: Was null for op %s", op)
            op.validateDataTypes(oc)

            Dim baseReduce As BaseReduceOp = TryCast(op, BaseReduceOp)
            If baseReduce IsNot Nothing AndAlso baseReduce.isEmptyReduce() Then
                'Edge case for TF import compatibility: [x,y].reduce(empty) = [x,y]
                'Note that "empty" axis is NOT the same as length 0, as in INDArray.sum(new int[0]), which means "all dimensions"
                If z IsNot Nothing Then
                    Preconditions.checkState(x.shape().SequenceEqual(z.shape()), "For empty reductions, result (z) array must have same shape as x shape. Got: x=%s, z=%s", ArrayToString(x.shape()), ArrayToString(z.shape()))
                    Dim xc As Operand = operandOf(x, "X")
                    Dim zc As Operand = operandOf(z, "Z")
                    If zc.Length > 0 Then
                        ElementWiseKernels.Copy(xc.Storage, xc.Offset, xc.Info, zc.Storage, zc.Offset, zc.Info)
                    End If
                    Return z
                Else
                    z = x.dup()
                    setZ(z, op, oc)
                    Return z
                End If
            End If

            Dim dimension As Integer() = ShapeT.normalizeAxis(x.rank(), dimensionsOf(op, op.dimensions()))
            ' reduce to scalar case, ReduceBool ops require special treatment
            If TypeOf op Is BaseReduceBoolOp AndAlso x.Empty AndAlso ShapeT.wholeArrayDimension(dimension) Then
                Dim emptyValue As Boolean = DirectCast(op, BaseReduceBoolOp).emptyValue()
                If z Is Nothing Then
                    z = Nd4vbT.scalar(emptyValue)
                    setZ(z, op, oc)
                Else
                    fill(operandOf(z, "Z"), If(emptyValue, 1.0, 0.0))
                End If

                Return z
            End If

            Dim keepDims As Boolean = op.KeepDims
            Dim retShape() As Long = ShapeT.reductionShape(x, dimension, True, keepDims)
            Dim complex As Boolean = op.ComplexAccumulation
            If complex AndAlso y Is Nothing Then
                Throw New ND4VBIllegalStateException("The all-distances mode of op " & op.opName() & " needs Op.Y")
            End If

            ' This is the result array. We create it only if we hadn't provided it before
            If z Is Nothing OrElse z Is x Then
                Dim ret As INDArray
                If complex Then
                    Dim xT As Long = tadCount(x, dimension)
                    Dim yT As Long = tadCount(y, dimension)

                    ret = Nd4vbT.create(op.resultType(), New Long() {xT, yT})
                Else
                    If y IsNot Nothing Then

                        '2 options here: either pairwise, equal sizes - OR every X TAD vs. entirety of Y
                        If x.length() = y.length() Then
                            'Pairwise
                            If tadCount(x, dimension) <> tadCount(y, dimension) Then
                                Throw New ND4VBIllegalStateException("Number of TADs along dimension don't match: (x shape = " & ArrayToString(x.shape()) & ", y shape = " & ArrayToString(y.shape()) & ", dimension = " & ArrayToString(dimension) & ")")
                            End If
                        Else
                            'Every X TAD vs. entirety of Y
                            Dim xTads As Long = tadCount(x, dimension)
                            Dim xTADSize As Long = If(xTads = 0, 0, x.length() \ xTads)

                            If xTADSize <> y.length() Then
                                Throw New ND4VBIllegalStateException("Size of TADs along dimension don't match for pairwise execution:" & " (x TAD size = " & xTADSize & ", y size = " & y.length())
                            End If
                        End If
                    End If

                    Dim dt As DataType = If(oc IsNot Nothing, op.resultType(oc), op.resultType())
                    ret = Nd4vbT.create(dt, retShape)

                End If
                setZ(ret, op, oc)
                z = ret
            Else
                ' compare length
                Dim shapeProduct As Long = (If(retShape.Length = 0, 1, ArrayUtil.prodLong(retShape)))
                If Not complex AndAlso z.length() <> shapeProduct Then
                    If Not (x.Empty AndAlso op.KeepDims) Then
                        'Empty reductions are special case: [1,0].sum(0,1,keep=true) -> shape [1,1]
                        Throw New ND4VBIllegalStateException("Shape of target array for reduction [" & ArrayToString(z.shape()) & "] doesn't match expected [" & ArrayToString(retShape) & "]")
                    End If
                ElseIf complex Then
                    Dim xT As Long = tadCount(x, dimension)
                    Dim yT As Long = tadCount(y, dimension)

                    If z.length() <> xT * yT Then
                        Throw New ND4VBIllegalStateException("Shape of target array for reduction [" & ArrayToString(z.shape()) & "] doesn't match expected [" & (xT * yT) & "]")
                    End If
                End If
            End If

            Dim xo As Operand = operandOf(x, "X")
            Dim zo As Operand = operandOf(z, "Z")
            Dim apiPack As TadPackT = Nothing
            Dim tads As KernelTadPack = tadsOf(x, xo.Info, dimension, apiPack)

            Dim st As Long = profilingConfigurableHookIn(op, apiPack.getTadShapeInfo())
            Dim extra As Double() = extraArgsOf(op)
            Dim type As OpType = op.getOpType()

            If TypeOf op Is Variance OrElse type = OpType.VARIANCE OrElse type = OpType.SUMMARYSTATS Then
                Dim biasCorrected As Boolean = If(TypeOf op Is Variance, DirectCast(op, Variance).BiasCorrected, True)
                Dim standardDeviation As Boolean = String.Equals(ManagedOpMath.SummaryStats(op.opName()).Name, "std", StringComparison.Ordinal)
                If xo.Length = 0 Then
                    fill(zo, SummaryStatsKernels.Variance(xo.Storage, xo.Offset, xo.Info, biasCorrected))
                Else
                    SummaryStatsKernels.VarianceAlongDimensions(xo.Storage, xo.Offset, xo.Info, zo.Storage, zo.Offset, zo.Info, tads, biasCorrected, standardDeviation)
                End If

            ElseIf y IsNot Nothing AndAlso type = OpType.REDUCE3 Then
                'pairwise reduction like similarity of two arrays
                Dim yo As Operand = operandOf(y, "Y")
                Dim definition As ReduceDefinition = ManagedOpMath.Reduce(op.opName())
                If xo.Length = 0 OrElse yo.Length = 0 Then
                    fill(zo, ReduceKernels.Reduce3ToScalar(xo.Storage, xo.Offset, xo.Info, xo.Storage, xo.Offset, xo.Info, definition, extra))
                ElseIf complex Then
                    Dim yPack As TadPackT = Nothing
                    Dim yTads As KernelTadPack = tadsOf(y, yo.Info, dimension, yPack)
                    If tads.TadLength <> yTads.TadLength Then
                        Throw New ND4VBIllegalStateException("Impossible to issue AllDistances operation: TAD lengths mismatch along given dimension: " & "x TAD length = " & tads.TadLength & ", y TAD length " & yTads.TadLength)
                    End If
                    ReduceKernels.Reduce3All(xo.Storage, xo.Offset, xo.Info, yo.Storage, yo.Offset, yo.Info, zo.Storage, zo.Offset, zo.Info, tads, yTads, definition, extra)
                Else
                    ' TAD vs TAD, or every TAD of x vs the whole of y
                    Dim yTads As KernelTadPack = Nothing
                    If yo.Length <> tads.TadLength Then
                        Dim yPack As TadPackT = Nothing
                        yTads = tadsOf(y, yo.Info, dimension, yPack)
                    End If
                    ReduceKernels.Reduce3AlongDimensions(xo.Storage, xo.Offset, xo.Info, yo.Storage, yo.Offset, yo.Info, zo.Storage, zo.Offset, zo.Info, tads, yTads, definition, extra)
                End If

            Else
                Select Case type
                    Case OpType.REDUCE_FLOAT, OpType.REDUCE_BOOL, OpType.REDUCE_SAME, OpType.REDUCE_LONG
                        Dim definition As ReduceDefinition = ManagedOpMath.Reduce(op.opName())
                        If xo.Length = 0 Then
                            fill(zo, ReduceKernels.ReduceToScalar(xo.Storage, xo.Offset, xo.Info, definition, extra))
                        Else
                            ReduceKernels.ReduceAlongDimensions(xo.Storage, xo.Offset, xo.Info, zo.Storage, zo.Offset, zo.Info, tads, definition, extra)
                        End If
                    Case Else
                        Throw New NotSupportedException("Unsupported op used in reduce: " & type.ToString())
                End Select
            End If

            profilingConfigurableHookOut(op, oc, st)
            Return getZ(op, oc)
        End Function

        ''' <summary>Executes a scalar or scalar bool op and returns z.</summary>
        Public Overrides Function exec(ByVal op As ScalarOp) As INDArray
            Return exec(op, Nothing)
        End Function

        ''' <summary>Executes a scalar op with the arrays of the context (when one is given) and returns z.</summary>
        Public Overridable Overloads Function exec(ByVal op As ScalarOp, ByVal oc As OpContext) As INDArray
            Dim st As Long = profilingConfigurableHookIn(op)
            Dim type As OpType = op.getOpType()

            If (oc IsNot Nothing AndAlso oc.getOutputArray(0) Is Nothing) OrElse getZ(op, oc) Is Nothing Then
                Select Case type
                    Case OpType.SCALAR
                        setZ(getX(op, oc).ulike(), op, oc)
                    Case OpType.SCALAR_BOOL
                        setZ(Nd4vbT.createUninitialized(DataType.BOOL, getX(op, oc).shape()), op, oc)
                    Case Else
                        Throw New ND4VBIllegalStateException("Unknown op type: [" & type.ToString() & "]")
                End Select
            End If

            Dim x As INDArray = getX(op, oc)
            Dim z As INDArray = getZ(op, oc)
            If x.length() <> z.length() Then
                Throw New ND4VBIllegalStateException("op.X length should be equal to op.Z length: " & "x.length()=" & x.length() & ", z.length()=" & z.length() & " - x shape info = [" & String.Join(", ", x.shapeInfoJava()) & "], z shape info = [" & String.Join(", ", z.shapeInfoJava()) & "]")
            End If

            Select Case type
                Case OpType.SCALAR, OpType.SCALAR_BOOL
                Case Else
                    Throw New ND4VBIllegalStateException("Unknown op type: [" & type.ToString() & "]")
            End Select

            Dim fn As PairwiseFunction = ManagedOpMath.Scalar(op.opName())
            Dim extra As Double() = extraArgsOf(op)
            Dim xo As Operand = operandOf(x, "X")
            Dim zo As Operand = operandOf(z, "Z")

            Dim dimension As Integer() = dimensionsOf(op, op.dimensions())
            If dimension IsNot Nothing AndAlso Not ShapeT.wholeArrayDimension(dimension) Then
                invokeScalarAlongDimension(op, oc, xo, zo, ShapeT.normalizeAxis(x.rank(), dimension), fn, extra)
            Else
                Dim scalarArray As INDArray = op.scalar()
                If scalarArray Is Nothing Then
                    Throw New ND4VBIllegalStateException("Op.scalar() is null for op " & op.opName())
                End If
                Dim so As Operand = operandOf(scalarArray, "scalar")
                If so.Length < 1 Then
                    Throw New ND4VBIllegalStateException("Op.scalar() of op " & op.opName() & " is empty")
                End If
                If zo.Length > 0 Then
                    ElementWiseKernels.Scalar(xo.Storage, xo.Offset, xo.Info, so.Storage.GetDouble(so.Offset), zo.Storage, zo.Offset, zo.Info, fn, extra)
                End If
            End If

            profilingConfigurableHookOut(op, oc, st)

            Return getZ(op, oc)
        End Function

        ''' <summary>
        ''' A scalar op along dimensions (libnd4j's <c>execScalarTad</c>): tensor i of x along the dimensions is combined
        ''' with element i of y. That is y broadcast along the remaining dimensions of x.
        ''' </summary>
        Private Sub invokeScalarAlongDimension(ByVal op As ScalarOp, ByVal oc As OpContext, ByVal xo As Operand, ByVal zo As Operand, ByVal dimension As Integer(), ByVal fn As PairwiseFunction, ByVal extra As Double())
            Dim y As INDArray = getY(op, oc)
            If y Is Nothing Then
                Throw New ND4VBIllegalStateException("Op " & op.opName() & " along dimensions needs Op.Y: one scalar per tensor along the dimensions")
            End If
            Dim yo As Operand = operandOf(y, "Y")
            If zo.Length = 0 Then
                Return
            End If
            Dim rank As Integer = CInt(xo.Info(0))
            Dim dims As Integer() = KernelTadPack.NormalizeDimensions(rank, dimension)
            Dim outer As New List(Of Integer)()
            For d As Integer = 0 To rank - 1
                If Array.IndexOf(dims, d) < 0 Then
                    outer.Add(d)
                End If
            Next d
            If outer.Count = 0 Then
                ElementWiseKernels.Scalar(xo.Storage, xo.Offset, xo.Info, yo.Storage.GetDouble(yo.Offset), zo.Storage, zo.Offset, zo.Info, fn, extra)
            Else
                BroadcastKernels.BroadcastAlongDimensions(xo.Storage, xo.Offset, xo.Info, yo.Storage, yo.Offset, yo.Info, zo.Storage, zo.Offset, zo.Info, outer.ToArray(), fn, extra)
            End If
        End Sub

        ''' <summary>
        ''' Executes a transform op: z = f(x) without y, z = f(x, y) (the pairwise form of the op) with y.
        ''' </summary>
        Private Sub execTransform(ByVal op As TransformOp, ByVal oc As OpContext)
            Dim x As INDArray = getX(op, oc)
            Dim y As INDArray = getY(op, oc)
            Dim z As INDArray = getZ(op, oc)

            Dim st As Long = profilingConfigurableHookIn(op)
            Dim type As OpType = op.getOpType()
            Dim extra As Double() = extraArgsOf(op)

            If y IsNot Nothing Then

                If z Is Nothing Then
                    setZ(Nd4vbT.create(op.resultType(), x.shape()), op, oc)
                    z = getZ(op, oc)
                End If

                op.validateDataTypes(oc, experimentalModeField.get())

                Select Case type
                    Case OpType.TRANSFORM_ANY, OpType.TRANSFORM_FLOAT, OpType.TRANSFORM_STRICT, OpType.TRANSFORM_SAME, OpType.PAIRWISE
                        If Not experimentalModeField.get() Then
                            Preconditions.checkArgument(x.dataType() = y.dataType() OrElse y.dataType() = DataType.BOOL, "Op.X and Op.Y must have the same data type, but got %s vs. %s", x.dataType(), y.dataType())
                        End If
                    Case OpType.TRANSFORM_BOOL, OpType.PAIRWISE_BOOL
                    Case Else
                        Throw New NotSupportedException("Unknown transform type: [" & type.ToString() & "]")
                End Select

                pairwiseOrBroadcast(operandOf(x, "X"), operandOf(y, "Y"), operandOf(z, "Z"), ManagedOpMath.Pairwise(op.opName()), extra)
            Else

                If z Is Nothing Then
                    setZ(Nd4vbT.createUninitialized((If(oc IsNot Nothing, op.resultType(oc), op.resultType())), x.shape()), op, oc)
                    z = getZ(op, oc)
                End If

                op.validateDataTypes(oc, experimentalModeField.get())

                Select Case type
                    Case OpType.TRANSFORM_FLOAT, OpType.TRANSFORM_STRICT, OpType.TRANSFORM_SAME, OpType.TRANSFORM_ANY, OpType.TRANSFORM_BOOL
                        Dim xo As Operand = operandOf(x, "X")
                        Dim zo As Operand = operandOf(z, "Z")
                        If zo.Length > 0 Then
                            ElementWiseKernels.Transform(xo.Storage, xo.Offset, xo.Info, zo.Storage, zo.Offset, zo.Info, ManagedOpMath.Transform(op.opName()), extra)
                        End If
                    Case Else
                        Throw New NotSupportedException("Unknown transform type: [" & type.ToString() & "]")
                End Select

            End If

            profilingConfigurableHookOut(op, oc, st)
        End Sub

        ''' <summary>Executes a broadcast or broadcast bool op (y applied to every tensor of x along <c>Dimension</c>) and returns z.</summary>
        Public Overrides Function exec(ByVal op As BroadcastOp) As INDArray
            Return exec(op, Nothing)
        End Function

        ''' <summary>Executes a broadcast op with the arrays of the context (when one is given) and returns z.</summary>
        Public Overridable Overloads Function exec(ByVal op As BroadcastOp, ByVal oc As OpContext) As INDArray
            Dim x As INDArray = getX(op, oc)
            Dim y As INDArray = getY(op, oc)
            Dim z As INDArray = getZ(op, oc)

            Dim st As Long = profilingConfigurableHookIn(op)
            Dim type As OpType = op.getOpType()
            Select Case type
                Case OpType.BROADCAST, OpType.BROADCAST_BOOL
                Case Else
                    Throw New NotSupportedException("Unknown operation type: [" & type.ToString() & "]")
            End Select

            If z Is Nothing Then
                z = Nd4vbT.createUninitialized(If(type = OpType.BROADCAST_BOOL, DataType.BOOL, x.dataType()), x.shape())
                setZ(z, op, oc)
            End If

            op.validateDataTypes(experimentalModeField.get())

            Dim dimension As Integer() = op.Dimension
            Dim zo As Operand = operandOf(z, "Z")
            If zo.Length > 0 Then
                Dim xo As Operand = operandOf(x, "X")
                Dim yo As Operand = operandOf(y, "Y")
                BroadcastKernels.BroadcastAlongDimensions(xo.Storage, xo.Offset, xo.Info, yo.Storage, yo.Offset, yo.Info, zo.Storage, zo.Offset, zo.Info, dimension, ManagedOpMath.Pairwise(op.opName()), extraArgsOf(op))
            End If

            profilingConfigurableHookOut(op, oc, st)
            Return z
        End Function

        ''' <summary>Random ops are not implemented in the managed backend yet (milestone M2).</summary>
        Public Overrides Function exec(ByVal op As RandomOp) As INDArray
            Throw randomNotSupported(op)
        End Function

        ''' <summary>Random ops are not implemented in the managed backend yet (milestone M2).</summary>
        Public Overrides Function exec(ByVal op As RandomOp, ByVal rng As RandomT) As INDArray
            Throw randomNotSupported(op)
        End Function

#End Region

#Region "Custom ops"

        ''' <summary>
        ''' Executes a custom op: missing outputs are allocated from <see cref="calculateOutputShape(CustomOp)"/>
        ''' (<c>Nd4vb.create(descriptor, False)</c>), the op's arrays, arguments and the RNG state of <c>Nd4vb.Random</c> are
        ''' put into a <see cref="ManagedOpContext"/>, and the op runs with <see cref="exec(CustomOp, OpContext)"/>.
        ''' </summary>
        ''' <exception cref="NotSupportedException">The op is not implemented in the managed backend.</exception>
        Public Overrides Function exec(ByVal op As CustomOp) As INDArray()

            Dim shapeOverride As Boolean = False
            If op.numOutputArguments() = 0 AndAlso Not op.InplaceCall Then
                Try
                    Dim list As IList(Of LongShapeDescriptor) = Me.calculateOutputShape(op)
                    If list.Count = 0 Then
                        Throw New ND4VBIllegalStateException("Op name " & op.opName() & " failed to calculate output datatypes")
                    End If

                    For Each shape As LongShapeDescriptor In list
                        op.addOutputArgument(Nd4vbT.create(shape, False))
                    Next shape

                    shapeOverride = True
                Catch e As ND4VBIllegalStateException
                    Throw
                Catch e As NotSupportedException
                    Throw
                Catch e As System.Exception
                    Throw New ND4VBIllegalStateException("Op name " & op.opName() & " - no output arrays were provided and calculateOutputShape failed to execute", e)
                End Try
            End If

            Dim name As String = op.opName()
            Try
                Using context As OpContext = buildContext()

                    ' optionally skip shape validation on op execution
                    If shapeOverride Then
                        context.shapeFunctionOverride(True)
                    End If

                    context.markInplace(op.InplaceCall)

                    ' transferring rng state
                    Dim random As RandomT = Nd4vbT.Random
                    context.setRngStates(random.rootState(), random.nodeState())

                    'transferring input/output arrays
                    context.setInputArrays(op.inputArguments())
                    context.setOutputArrays(op.outputArguments())

                    ' transferring static args
                    context.setBArguments(If(op.bArgs(), Array.Empty(Of Boolean)()))
                    context.setIArguments(If(op.iArgs(), Array.Empty(Of Long)()))
                    context.setTArguments(If(op.tArgs(), Array.Empty(Of Double)()))
                    context.setDArguments(If(op.dArgs(), Array.Empty(Of DataType)()))

                    Dim result As INDArray() = exec(op, context)
                    Dim states As Pair(Of Long, Long) = context.RngStates

                    ' pulling states back
                    random.setStates(states.First, states.Second)

                    Return result
                End Using
            Catch e As ND4VBOpProfilerException
                Throw
            Catch e As NotSupportedException
                Throw
            Catch e As System.Exception
                Throw New InvalidOperationException("Op [" & name & "] execution failed", e)
            End Try
        End Function

        ''' <summary>
        ''' Executes a custom op with the arrays and arguments of the context: the op's element-wise math from the registry
        ''' (<see cref="ManagedOpMath.TryGetCustomFamily"/>) runs as a transform of input 0, a broadcasting pairwise op of
        ''' inputs 0 and 1, or a reduction or index reduction of input 0 (dimensions from input 1 or the integer arguments,
        ''' keepDims from the first boolean argument, otherwise the first floating-point argument). A missing output 0 is
        ''' input 0 for an in-place call, otherwise allocated from <see cref="calculateOutputShape(CustomOp, OpContext)"/>.
        ''' Returns the output arrays.
        ''' </summary>
        ''' <exception cref="NotSupportedException">The op is not implemented in the managed backend.</exception>
        Public Overrides Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray()
            Dim st As Long = profilingConfigurableHookIn(op, context)
            Try
                Dim family As ManagedOpKind
                If Not ManagedOpMath.TryGetCustomFamily(op.opName(), family) Then
                    Throw customNotSupported(op.opName())
                End If

                execCustomFamily(op, context, family)

                Dim outputs As IList(Of INDArray) = context.getOutputArrays()
                If outputs.Count = 0 Then
                    Return New INDArray() {}
                Else
                    Return outputs.ToArray()
                End If
            Catch e As System.Exception When Not TypeOf e Is NotSupportedException
                log.error("Failed to execute op " & op.opName() & ". Attempted to execute with " & context.numInputArguments() & " inputs, " & context.numOutputArguments() & " outputs, " & context.numTArguments() & " targs, " & context.numBArguments() & " bargs and " & context.numIArguments() & " iargs: " & e.Message)
                Throw
            Finally
                profilingConfigurableHookOut(op, context, st)
            End Try
        End Function

        ''' <summary>True when the context (or the op) asks for an in-place call.</summary>
        Private Shared Function isInplace(ByVal op As CustomOp, ByVal context As OpContext) As Boolean
            Dim managed As ManagedOpContext = TryCast(context, ManagedOpContext)
            Return op.InplaceCall OrElse (managed IsNot Nothing AndAlso managed.Inplace)
        End Function

        Private Sub execCustomFamily(ByVal op As CustomOp, ByVal context As OpContext, ByVal family As ManagedOpKind)
            Dim name As String = op.opName()
            Dim inputs As IList(Of INDArray) = context.getInputArrays()
            If inputs.Count = 0 Then
                Throw New ND4VBIllegalStateException("Op " & name & " has no input arrays")
            End If
            Dim tArgs As Double() = context.getTArguments().ToArray()

            Dim z As INDArray = context.getOutputArray(0)
            If z Is Nothing Then
                If isInplace(op, context) Then
                    z = inputs(0)
                Else
                    Dim shapes As IList(Of LongShapeDescriptor) = calculateOutputShape(op, context)
                    If shapes.Count = 0 Then
                        Throw New ND4VBIllegalStateException("Op name " & name & " failed to calculate output datatypes")
                    End If
                    z = Nd4vbT.create(shapes(0), False)
                End If
                context.setOutputArray(0, z)
            End If

            Dim xo As Operand = operandOf(inputs(0), "input 0")
            Dim zo As Operand = operandOf(z, "output 0")

            Select Case family
                Case ManagedOpKind.TransformSame, ManagedOpKind.TransformFloat, ManagedOpKind.TransformStrict, ManagedOpKind.TransformAny, ManagedOpKind.TransformBool
                    If xo.Length <> zo.Length Then
                        Throw New ND4VBIllegalStateException("Op " & name & ": the output has " & zo.Length & " elements, the input " & xo.Length)
                    End If
                    If zo.Length > 0 Then
                        ElementWiseKernels.Transform(xo.Storage, xo.Offset, xo.Info, zo.Storage, zo.Offset, zo.Info, ManagedOpMath.Transform(name), tArgs)
                    End If

                Case ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool
                    If inputs.Count < 2 Then
                        Throw New ND4VBIllegalStateException("Op " & name & " needs 2 input arrays, got " & inputs.Count)
                    End If
                    Dim yo As Operand = operandOf(inputs(1), "input 1")
                    If zo.Length > 0 Then
                        Dim fn As PairwiseFunction = ManagedOpMath.Pairwise(name)
                        Dim xShape As Long() = ShapeInfoHelper.GetShape(xo.Info)
                        Dim yShape As Long() = ShapeInfoHelper.GetShape(yo.Info)
                        Dim zShape As Long() = ShapeInfoHelper.GetShape(zo.Info)
                        If xShape.SequenceEqual(yShape) AndAlso xShape.SequenceEqual(zShape) Then
                            ElementWiseKernels.Pairwise(xo.Storage, xo.Offset, xo.Info, yo.Storage, yo.Offset, yo.Info, zo.Storage, zo.Offset, zo.Info, fn, tArgs)
                        Else
                            BroadcastKernels.BroadcastPairwise(xo.Storage, xo.Offset, xo.Info, yo.Storage, yo.Offset, yo.Info, zo.Storage, zo.Offset, zo.Info, fn, tArgs)
                        End If
                    End If

                Case ManagedOpKind.ReduceFloat, ManagedOpKind.ReduceSame, ManagedOpKind.ReduceLong, ManagedOpKind.ReduceBool, ManagedOpKind.IndexReduce
                    Dim x As INDArray = inputs(0)
                    Dim keepDims As Boolean
                    Dim dimension As Integer() = ShapeT.normalizeAxis(x.rank(), customReductionDimensions(inputs, context.getIArguments(), context.getBArguments(), context.getTArguments(), keepDims))
                    If family = ManagedOpKind.IndexReduce Then
                        Dim definition As IndexReduceDefinition = ManagedOpMath.IndexReduce(name)
                        If xo.Length = 0 Then
                            Throw New ArgumentException("IndexReduce can't be issued along axis with 0 in shape")
                        End If
                        If zo.Length > 0 Then
                            Dim apiPack As TadPackT = Nothing
                            IndexReduceKernels.IndexReduceAlongDimensions(xo.Storage, xo.Offset, xo.Info, zo.Storage, zo.Offset, zo.Info, tadsOf(x, xo.Info, dimension, apiPack), definition, Nothing)
                        End If
                    Else
                        Dim definition As ReduceDefinition = ManagedOpMath.Reduce(name)
                        If xo.Length = 0 Then
                            fill(zo, ReduceKernels.ReduceToScalar(xo.Storage, xo.Offset, xo.Info, definition, Nothing))
                        ElseIf zo.Length > 0 Then
                            Dim apiPack As TadPackT = Nothing
                            ReduceKernels.ReduceAlongDimensions(xo.Storage, xo.Offset, xo.Info, zo.Storage, zo.Offset, zo.Info, tadsOf(x, xo.Info, dimension, apiPack), definition, Nothing)
                        End If
                    End If

                Case Else
                    Throw customNotSupported(name)
            End Select
        End Sub

        ''' <summary>
        ''' The dimensions of a custom reduction: the values of input 1 when there is one, otherwise the integer arguments
        ''' (Nothing: none given, the whole array); keepDims is the first boolean argument, otherwise the first
        ''' floating-point argument (non-zero), otherwise False.
        ''' </summary>
        Private Shared Function customReductionDimensions(ByVal inputs As IList(Of INDArray), ByVal iArgs As IList(Of Long), ByVal bArgs As IList(Of Boolean), ByVal tArgs As IList(Of Double), ByRef keepDims As Boolean) As Integer()
            If bArgs IsNot Nothing AndAlso bArgs.Count > 0 Then
                keepDims = bArgs(0)
            ElseIf tArgs IsNot Nothing AndAlso tArgs.Count > 0 Then
                keepDims = tArgs(0) <> 0.0
            Else
                keepDims = False
            End If

            If inputs.Count > 1 Then
                Dim axes As Operand = operandOf(inputs(1), "input 1")
                Dim n As Long = axes.Length
                Dim result(CInt(n) - 1) As Integer
                For i As Long = 0 To n - 1
                    result(CInt(i)) = CInt(axes.Storage.GetLong(axes.Offset + ShapeInfoHelper.OffsetOf(axes.Info, i)))
                Next i
                Return If(result.Length = 0, Nothing, result)
            End If
            If iArgs Is Nothing OrElse iArgs.Count = 0 Then
                Return Nothing
            End If
            Dim dims(iArgs.Count - 1) As Integer
            For i As Integer = 0 To iArgs.Count - 1
                dims(i) = CInt(iArgs(i))
            Next i
            Return dims
        End Function

        ''' <summary>The output shape of a custom op the managed backend implements.</summary>
        Public Overrides Function calculateOutputShape(ByVal op As CustomOp) As IList(Of LongShapeDescriptor)
            Return calculateOutputShape(op, Nothing)
        End Function

        ''' <summary>
        ''' The output shape of a custom op the managed backend implements, from the inputs and arguments of the context
        ''' (when one is given) or of the op: a transform has the shape of input 0, a pairwise op the NumPy broadcast shape
        ''' of inputs 0 and 1, a reduction the reduced shape. The data type is input 0's for "same", "strict" and "any"
        ''' transforms and for <c>assign</c>, floating point for "float" transforms and reductions, the pairwise type of the
        ''' inputs (<c>Shape.pickPairwiseDataType</c>) for pairwise ops, BOOL for bool ops and LONG for long and index
        ''' reductions. An empty list means the op is not fully initialized (missing inputs).
        ''' </summary>
        ''' <exception cref="NotSupportedException">The op is not implemented in the managed backend.</exception>
        Public Overrides Function calculateOutputShape(ByVal op As CustomOp, ByVal opContext As OpContext) As IList(Of LongShapeDescriptor)
            Dim name As String = op.opName()
            Dim family As ManagedOpKind
            If Not ManagedOpMath.TryGetCustomFamily(name, family) Then
                Throw customNotSupported(name)
            End If

            Dim result As New List(Of LongShapeDescriptor)()
            Dim inputs As IList(Of INDArray) = If(opContext IsNot Nothing, opContext.getInputArrays(), op.inputArguments())
            Dim required As Integer = If(family = ManagedOpKind.Pairwise OrElse family = ManagedOpKind.PairwiseBool, 2, 1)
            If inputs Is Nothing OrElse inputs.Count < required Then
                Return result
            End If

            Dim x As INDArray = inputs(0)
            Dim xType As DataType = x.dataType()
            Select Case family
                Case ManagedOpKind.TransformSame, ManagedOpKind.TransformStrict, ManagedOpKind.TransformAny
                    result.Add(LongShapeDescriptor.fromShape(x.shape(), xType))
                Case ManagedOpKind.TransformFloat
                    result.Add(LongShapeDescriptor.fromShape(x.shape(), If(xType.isFPType(), xType, DataType.FLOAT)))
                Case ManagedOpKind.TransformBool
                    result.Add(LongShapeDescriptor.fromShape(x.shape(), DataType.BOOL))
                Case ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool
                    Dim y As INDArray = inputs(1)
                    Dim shape As Long() = BroadcastKernels.BroadcastShape(x.shape(), y.shape())
                    Dim dt As DataType
                    If family = ManagedOpKind.PairwiseBool Then
                        dt = DataType.BOOL
                    ElseIf String.Equals(name, "assign", StringComparison.Ordinal) Then
                        dt = xType
                    Else
                        dt = ShapeT.pickPairwiseDataType(xType, y.dataType())
                    End If
                    result.Add(LongShapeDescriptor.fromShape(shape, dt))
                Case Else
                    Dim keepDims As Boolean
                    Dim iArgs As IList(Of Long) = If(opContext IsNot Nothing, opContext.getIArguments(), DirectCast(If(op.iArgs(), Array.Empty(Of Long)()), IList(Of Long)))
                    Dim bArgs As IList(Of Boolean) = If(opContext IsNot Nothing, opContext.getBArguments(), DirectCast(If(op.bArgs(), Array.Empty(Of Boolean)()), IList(Of Boolean)))
                    Dim tArgs As IList(Of Double) = If(opContext IsNot Nothing, opContext.getTArguments(), DirectCast(If(op.tArgs(), Array.Empty(Of Double)()), IList(Of Double)))
                    Dim dimension As Integer() = ShapeT.normalizeAxis(x.rank(), customReductionDimensions(inputs, iArgs, bArgs, tArgs, keepDims))
                    Dim shape As Long() = ShapeT.reductionShape(x, dimension, True, keepDims)
                    Dim dt As DataType
                    Select Case family
                        Case ManagedOpKind.ReduceFloat
                            dt = If(xType.isFPType(), xType, DataType.FLOAT)
                        Case ManagedOpKind.ReduceSame
                            dt = xType
                        Case ManagedOpKind.ReduceBool
                            dt = DataType.BOOL
                        Case Else
                            dt = DataType.LONG
                    End Select
                    result.Add(LongShapeDescriptor.fromShape(shape, dt))
            End Select
            Return result
        End Function

        ''' <summary>
        ''' The custom ops of the managed backend (the names <see cref="ManagedOpMath.Names"/> lists under
        ''' <see cref="ManagedOpKind.Custom"/>), with libnd4j's name hash (<see cref="HashUtil.getLongHash(String)"/>), one
        ''' output, 2 inputs for pairwise ops and 1 otherwise, in-place support for transforms and pairwise ops, and a
        ''' variable number (-1) of floating-point and integer arguments. Read-only.
        ''' </summary>
        Public Overrides ReadOnly Property CustomOperations As IDictionary(Of String, CustomOpDescriptor)
            Get
                SyncLock customOpsLock
                    If customOps Is Nothing Then
                        Dim map As New Dictionary(Of String, CustomOpDescriptor)(StringComparer.Ordinal)
                        For Each name As String In ManagedOpMath.Names(ManagedOpKind.Custom)
                            Dim family As ManagedOpKind
                            ManagedOpMath.TryGetCustomFamily(name, family)
                            Dim pairwise As Boolean = family = ManagedOpKind.Pairwise OrElse family = ManagedOpKind.PairwiseBool
                            Dim transform As Boolean = family = ManagedOpKind.TransformSame OrElse family = ManagedOpKind.TransformFloat OrElse family = ManagedOpKind.TransformStrict OrElse family = ManagedOpKind.TransformAny OrElse family = ManagedOpKind.TransformBool
                            map(name) = CustomOpDescriptor.builder().hash(HashUtil.getLongHash(name)).numInputs(If(pairwise, 2, 1)).numOutputs(1).allowsInplace(pairwise OrElse transform).numTArgs(-1).numIArgs(-1).build()
                        Next name
                        customOps = New ReadOnlyDictionary(Of String, CustomOpDescriptor)(map)
                    End If
                    Return customOps
                End SyncLock
            End Get
        End Property

        ''' <summary>A new <see cref="ManagedOpContext"/>.</summary>
        Public Overrides Function buildContext() As OpContext
            Return New ManagedOpContext()
        End Function

#End Region

#Region "Environment, modes and buffers"

        ''' <summary>The shared TAD manager of the managed backend.</summary>
        Public Overrides ReadOnly Property TADManager As TADManagerT
            Get
                Return tadManagerField
            End Get
        End Property

        ''' <summary>
        ''' The environment (see <see cref="DefaultOpExecutioner.EnvironmentInformation"/>) plus the backend ("CPU"), the
        ''' kernel thread count (as OpenMP and BLAS threads), the BLAS vendor ("MANAGED") and the free managed memory.
        ''' </summary>
        Public Overrides ReadOnly Property EnvironmentInformation As Properties
            Get
                Dim properties As Properties = MyBase.EnvironmentInformation
                properties.put(BACKEND_KEY, "CPU")
                properties.put(OMP_THREADS_KEY, KernelOptions.MaxDegreeOfParallelism.ToString(CultureInfo.InvariantCulture))
                properties.put(BLAS_THREADS_KEY, KernelOptions.MaxDegreeOfParallelism.ToString(CultureInfo.InvariantCulture))
                properties.put(BLAS_VENDOR_KEY, "MANAGED")
                properties.put(HOST_FREE_MEMORY_KEY, Math.Max(0L, availableMemory() - GC.GetTotalMemory(False)).ToString(CultureInfo.InvariantCulture))
                Return properties
            End Get
        End Property

        ''' <summary>Turns debug mode on or off.</summary>
        Public Overrides Sub enableDebugMode(ByVal reallyEnable As Boolean)
            debugField.set(reallyEnable)
        End Sub

        ''' <summary>Turns verbose mode on or off (the profiling hooks then log the first values of z).</summary>
        Public Overrides Sub enableVerboseMode(ByVal reallyEnable As Boolean)
            verboseField.set(reallyEnable)
        End Sub

        ''' <summary>Sets <see cref="KernelOptions.ParallelThreshold"/>, the number of elements above which kernels run in parallel (at least 1).</summary>
        Public Overrides WriteOnly Property ElementsThreshold As Integer
            Set(ByVal threshold As Integer)
                KernelOptions.ParallelThreshold = Math.Max(1, threshold)
            End Set
        End Property

        ''' <summary>Ignored: the managed kernels have no per-thread TAD threshold.</summary>
        Public Overrides WriteOnly Property TadThreshold As Integer
            Set(ByVal threshold As Integer)
                ' no managed counterpart
            End Set
        End Property

        ''' <summary>The string at <paramref name="index"/> of a UTF8 managed buffer.</summary>
        Public Overrides Function getString(ByVal buffer As DataBuffer, ByVal index As Long) As String
            Preconditions.checkArgument(TypeOf buffer Is ManagedDataBuffer AndAlso buffer.dataType() = DataType.UTF8, "Expected Utf8Buffer")
            Return DirectCast(buffer, ManagedDataBuffer).getString(index)
        End Function

        ''' <summary><see cref="OpExecutioner.ExecutionerType.MANAGED"/>.</summary>
        Public Overrides Function type() As OpExecutioner.ExecutionerType
            Return OpExecutioner.ExecutionerType.MANAGED
        End Function

        ''' <summary>The experimental mode flag (off).</summary>
        Public Overrides ReadOnly Property ExperimentalMode As Boolean
            Get
                Return experimentalModeField.get()
            End Get
        End Property

        ''' <summary>
        ''' Statistics of all elements of the array (libnd4j's <c>inspectArray</c>): minimum, maximum, mean, standard
        ''' deviation (not bias corrected) and the numbers of positive, negative, zero, infinite and NaN values.
        ''' </summary>
        Public Overrides Function inspectArray(ByVal array As INDArray) As INDArrayStatistics
            Dim o As Operand = operandOf(array, "array")
            Dim n As Long = o.Length
            Dim minValue As Double = 0.0, maxValue As Double = 0.0, sum As Double = 0.0
            Dim positive, negative, zero, inf, nan As Long
            For i As Long = 0 To n - 1
                Dim v As Double = o.Storage.GetDouble(o.Offset + ShapeInfoHelper.OffsetOf(o.Info, i))
                If i = 0 Then
                    minValue = v
                    maxValue = v
                Else
                    If v < minValue Then minValue = v
                    If v > maxValue Then maxValue = v
                End If
                sum += v
                If v = 0.0 Then zero += 1
                If v > 0.0 Then positive += 1
                If v < 0.0 Then negative += 1
                If Double.IsInfinity(v) Then inf += 1
                If Double.IsNaN(v) Then nan += 1
            Next i
            Dim mean As Double = If(n = 0, 0.0, sum / n)
            Dim stdDev As Double = If(n = 0, 0.0, SummaryStatsKernels.StandardDeviation(o.Storage, o.Offset, o.Info, False))
            Return INDArrayStatistics.builder().minValue(minValue).maxValue(maxValue).meanValue(mean).stdDevValue(stdDev).countInf(inf).countNaN(nan).countNegative(negative).countPositive(positive).countZero(zero).build()
        End Function

        ''' <summary>
        ''' A constant shape information buffer (<see cref="ShapeT.createShapeInfoArray(Long(), Long(), Long, Char, DataType, Boolean)"/>
        ''' through the constant handler): the data type flags, plus the empty flag when <paramref name="empty"/> is set or a
        ''' dimension is 0.
        ''' </summary>
        Public Overrides Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal empty As Boolean) As DataBuffer
            Return ConstantHandler.getConstantBuffer(ShapeT.createShapeInfoArray(shape, stride, elementWiseStride, order, dtype, empty), DataType.LONG)
        End Function

        ''' <summary>
        ''' A constant shape information buffer with the given extras (array type flags) and the data type flags of
        ''' <paramref name="dtype"/> added (libnd4j's <c>shapeBufferEx</c>).
        ''' </summary>
        Public Overrides Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal extras As Long) As DataBuffer
            Dim withType As Long = extras
            If ArrayOptionsHelper.dataType(extras) <> dtype Then
                withType = ArrayOptionsHelper.setOptionBit(extras, dtype)
            End If
            Return ConstantHandler.getConstantBuffer(ShapeT.createShapeInfoArray(shape, stride, elementWiseStride, order, withType), DataType.LONG)
        End Function

        ''' <summary>The TADs of the array along the dimensions, from the shared TAD manager.</summary>
        Public Overrides Function tadShapeInfoAndOffsets(ByVal array As INDArray, ByVal dimension() As Integer) As TadPackT
            Return tadManagerField.tadShapeInfoAndOffsets(array, dimension)
        End Function

        ''' <summary>A constant buffer of <paramref name="desiredType"/> holding the values (from the constant handler).</summary>
        Public Overrides Function createConstantBuffer(ByVal values() As Long, ByVal desiredType As DataType) As DataBuffer
            Return ConstantHandler.getConstantBuffer(values, desiredType)
        End Function

        ''' <summary>A constant buffer of <paramref name="desiredType"/> holding the values (from the constant handler).</summary>
        Public Overrides Function createConstantBuffer(ByVal values() As Double, ByVal desiredType As DataType) As DataBuffer
            Return ConstantHandler.getConstantBuffer(values, desiredType)
        End Function

#End Region

    End Class

End Namespace
