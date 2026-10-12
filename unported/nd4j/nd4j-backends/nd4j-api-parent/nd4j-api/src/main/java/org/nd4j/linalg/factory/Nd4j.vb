Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArgMax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax
Imports ArgMin = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports org.nd4j.linalg.factory.ops
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports var = lombok.var
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports Nd4jContext = org.nd4j.context.Nd4jContext
Imports FlatArray = org.nd4j.graph.FlatArray
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports org.nd4j.linalg.api.buffer
Imports DataBufferFactory = org.nd4j.linalg.api.buffer.factory.DataBufferFactory
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports BasicAffinityManager = org.nd4j.linalg.api.concurrency.BasicAffinityManager
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports MemoryWorkspaceManager = org.nd4j.linalg.api.memory.MemoryWorkspaceManager
Imports org.nd4j.linalg.api.ndarray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports DefaultOpExecutioner = org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Mmul = org.nd4j.linalg.api.ops.impl.reduce.Mmul
Imports ReplaceNans = org.nd4j.linalg.api.ops.impl.scalar.ReplaceNans
Imports ScatterUpdate = org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate
Imports Diag = org.nd4j.linalg.api.ops.impl.shape.Diag
Imports DiagPart = org.nd4j.linalg.api.ops.impl.shape.DiagPart
Imports Stack = org.nd4j.linalg.api.ops.impl.shape.Stack
Imports Pad = org.nd4j.linalg.api.ops.impl.transforms.Pad
Imports Mode = org.nd4j.linalg.api.ops.impl.transforms.Pad.Mode
Imports Reverse = org.nd4j.linalg.api.ops.impl.transforms.custom.Reverse
Imports Tile = org.nd4j.linalg.api.ops.impl.shape.Tile
Imports RandomExponential = org.nd4j.linalg.api.ops.random.custom.RandomExponential
Imports org.nd4j.linalg.api.ops.random.impl
Imports DefaultRandom = org.nd4j.linalg.api.rng.DefaultRandom
Imports Distribution = org.nd4j.linalg.api.rng.distribution.Distribution
Imports DefaultDistributionFactory = org.nd4j.linalg.api.rng.distribution.factory.DefaultDistributionFactory
Imports DistributionFactory = org.nd4j.linalg.api.rng.distribution.factory.DistributionFactory
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports BasicConstantHandler = org.nd4j.linalg.cache.BasicConstantHandler
Imports ConstantHandler = org.nd4j.linalg.cache.ConstantHandler
Imports BasicNDArrayCompressor = org.nd4j.linalg.compression.BasicNDArrayCompressor
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports ConvolutionInstance = org.nd4j.linalg.convolution.ConvolutionInstance
Imports DefaultConvolutionInstance = org.nd4j.linalg.convolution.DefaultConvolutionInstance
Imports EnvironmentalAction = org.nd4j.linalg.env.EnvironmentalAction
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ND4JUnknownDataTypeException = org.nd4j.linalg.exception.ND4JUnknownDataTypeException
Imports NoAvailableBackendException = org.nd4j.linalg.factory.Nd4jBackend.NoAvailableBackendException
Imports BasicMemoryManager = org.nd4j.linalg.api.memory.BasicMemoryManager
Imports MemoryManager = org.nd4j.linalg.api.memory.MemoryManager
Imports DeallocatorService = org.nd4j.linalg.api.memory.deallocation.DeallocatorService
Imports org.nd4j.common.primitives
Imports NDArrayStrings = org.nd4j.linalg.string.NDArrayStrings
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports LongUtils = org.nd4j.linalg.util.LongUtils
Imports PropertyParser = org.nd4j.common.tools.PropertyParser
Imports VersionCheck = org.nd4j.versioncheck.VersionCheck

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.linalg.factory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Nd4j
	Public Class Nd4j

		''' <summary>
		''' Bitwise namespace - operations related to bitwise manipulation of arrays
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field bitwise was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly bitwise_Conflict As New NDBitwise()
		''' <summary>
		''' Math namespace - general mathematical operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field math was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly math_Conflict As New NDMath()
		''' <summary>
		''' Random namespace - (pseudo) random number generation methods
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field random was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly random_Conflict As New NDRandom()
		''' <summary>
		''' Neural network namespace - operations related to neural networks
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nn was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly nn_Conflict As New NDNN()

		''' <summary>
		''' Loss function namespace - operations related to loss functions.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field loss was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly loss_Conflict As New NDLoss()

		''' <summary>
		''' Convolutional network namespace - operations related to convolutional neural networks
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cnn was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly cnn_Conflict As New NDCNN()

		''' <summary>
		''' Recurrent neural network namespace - operations related to recurrent neural networks
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field rnn was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly rnn_Conflict As New NDRNN()

		''' <summary>
		''' Image namespace - operations related to images
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field image was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly image_Conflict As New NDImage()

		''' <summary>
		''' Bitwise namespace - operations related to bitwise manipulation of arrays
		''' </summary>
		Public Shared Function bitwise() As NDBitwise
			Return bitwise_Conflict
		End Function

		''' <summary>
		''' Math namespace - general mathematical operations
		''' </summary>
		Public Shared Function math() As NDMath
			Return math_Conflict
		End Function

		''' <summary>
		''' Random namespace - (pseudo) random number generation methods
		''' </summary>
		Public Shared Function random() As NDRandom
			Return random_Conflict
		End Function

		''' <summary>
		''' Neural network namespace - operations related to neural networks
		''' </summary>
		Public Shared Function nn() As NDNN
			Return nn_Conflict
		End Function

		''' <summary>
		''' Loss function namespace - operations related to loss functions.
		''' </summary>
		Public Shared Function loss() As NDLoss
			Return loss_Conflict
		End Function

		''' <summary>
		''' Convolutional network namespace - operations related to convolutional neural networks
		''' </summary>
		Public Shared Function cnn() As NDCNN
			Return cnn_Conflict
		End Function

		''' <summary>
		''' Recurrent neural network namespace - operations related to recurrent neural networks
		''' </summary>
		Public Shared Function rnn() As NDRNN
			Return rnn_Conflict
		End Function

		''' <summary>
		''' Image namespace - operations related to images
		''' </summary>
		Public Shared Function image() As NDImage
			Return image_Conflict
		End Function

		Private Const DATA_BUFFER_OPS As String = "databufferfactory"
		Private Const CONVOLUTION_OPS As String = "convops"
		'''@deprecated Use <seealso cref="ND4JSystemProperties.DTYPE"/> 
		<Obsolete("Use <seealso cref=""ND4JSystemProperties.DTYPE""/>")>
		Public Const DTYPE As String = ND4JSystemProperties.DTYPE
		Private Const BLAS_OPS As String = "blas.ops"
		Public Const NATIVE_OPS As String = "native.ops"
		Private Const ORDER_KEY As String = "ndarray.order"
		Private Const NDARRAY_FACTORY_CLASS As String = "ndarrayfactory.class"
		Private Const OP_EXECUTIONER As String = "opexec"

		Public Const DISTRIBUTION As String = "dist"
		Private Const SHAPEINFO_PROVIDER As String = "shapeinfoprovider"
		Private Const CONSTANT_PROVIDER As String = "constantsprovider"
		Private Const AFFINITY_MANAGER As String = "affinitymanager"
		'disable toString() on compressed arrays for debugging. Should be off by default.
		Private Const COMPRESSION_DEBUG As String = "compressiondebug"
		Private Const MEMORY_MANAGER As String = "memorymanager"
		Private Const WORKSPACE_MANAGER As String = "workspacemanager"
		Private Const RANDOM_PROVIDER As String = "random"
		'''@deprecated Use <seealso cref="ND4JSystemProperties.LOG_INITIALIZATION"/> 
		<Obsolete("Use <seealso cref=""ND4JSystemProperties.LOG_INITIALIZATION""/>")>
		Public Const LOG_INIT_ENV_PROPERTY As String = ND4JSystemProperties.LOG_INITIALIZATION

		'the datatype used for allocating buffers
'JAVA TO VB CONVERTER NOTE: The field dtype was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shared dtype_Conflict As DataType = DataType.FLOAT
		'the allocation mode for the heap
		Public Shared alloc As DataBuffer.AllocationMode = DataBuffer.AllocationMode.MIXED_DATA_TYPES
		Public Shared EPS_THRESHOLD As Double = 1e-5
		Private Shared allowsOrder As Boolean = False
		Public Shared compressDebug As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: public static volatile boolean preventUnpack;
		Public Shared preventUnpack As Boolean
'JAVA TO VB CONVERTER NOTE: The field backend was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared backend_Conflict As Nd4jBackend
'JAVA TO VB CONVERTER NOTE: The field randomFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared randomFactory_Conflict As RandomFactory
'JAVA TO VB CONVERTER NOTE: The field workspaceManager was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared workspaceManager_Conflict As MemoryWorkspaceManager
'JAVA TO VB CONVERTER NOTE: The field deallocatorService was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared deallocatorService_Conflict As DeallocatorService
		Private Shared defaultFloatingPointDataType As AtomicReference(Of DataType)

		Private Shared DATA_BUFFER_FACTORY_INSTANCE As DataBufferFactory
		Private Shared BLAS_WRAPPER_INSTANCE As BlasWrapper
		Protected Friend Shared INSTANCE As NDArrayFactory
		Private Shared CONVOLUTION_INSTANCE As ConvolutionInstance
		Private Shared OP_EXECUTIONER_INSTANCE As OpExecutioner
		Private Shared DISTRIBUTION_FACTORY As DistributionFactory
'JAVA TO VB CONVERTER NOTE: The field shapeInfoProvider was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared shapeInfoProvider_Conflict As ShapeInfoProvider
'JAVA TO VB CONVERTER NOTE: The field constantHandler was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared constantHandler_Conflict As ConstantHandler
'JAVA TO VB CONVERTER NOTE: The field affinityManager was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared affinityManager_Conflict As AffinityManager
'JAVA TO VB CONVERTER NOTE: The field memoryManager was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared memoryManager_Conflict As MemoryManager

		Private Shared fallbackMode As AtomicBoolean

		Protected Friend Shared props As New Properties()

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Private Shared ReadOnly logger As Logger = Logger.getLogger(GetType(Nd4j).FullName)

		Private Shared ReadOnly EMPTY_ARRAYS((DataType.values().length) - 1) As INDArray

		Shared Sub New()
			fallbackMode = New AtomicBoolean(False)
'JAVA TO VB CONVERTER NOTE: The variable nd4j was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim nd4j_Conflict As New Nd4j()
			nd4j_Conflict.initContext()
		End Sub

		''' <summary>
		''' See <seealso cref="pad(INDArray, INDArray)"/>.  Uses 0 padding.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pad(@NonNull INDArray toPad, @NonNull int[][] padWidth)
		Public Shared Function pad(ByVal toPad As INDArray, ByVal padWidth()() As Integer) As INDArray
			Return pad(toPad, Nd4j.createFromArray(padWidth))
		End Function

		''' <summary>
		''' See <seealso cref="pad(INDArray, INDArray)"/>.  Uses 0 padding, and uses padWidth for all dimensions.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pad(@NonNull INDArray toPad, @NonNull int... padWidth)
		Public Shared Function pad(ByVal toPad As INDArray, ParamArray ByVal padWidth() As Integer) As INDArray
			Return pad(toPad, padWidth, Pad.Mode.CONSTANT, 0)
		End Function

		''' <summary>
		''' See <seealso cref="pad(INDArray, INDArray, Pad.Mode, Double)"/> with zero padding (zeros for padValue).
		''' </summary>
		Public Shared Function pad(ByVal toPad As INDArray, ByVal padding As INDArray) As INDArray
			Return pad(toPad, padding, Pad.Mode.CONSTANT, 0)
		End Function

		''' <summary>
		''' See <seealso cref="pad(INDArray, INDArray, Mode, Double)"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pad(@NonNull INDArray toPad, @NonNull int[][] padWidth, @NonNull Pad.Mode padMode, double padValue)
		Public Shared Function pad(ByVal toPad As INDArray, ByVal padWidth()() As Integer, ByVal padMode As Pad.Mode, ByVal padValue As Double) As INDArray
			Return pad(toPad, Nd4j.createFromArray(padWidth), padMode, padValue)
		End Function

		''' <summary>
		''' See <seealso cref="pad(INDArray, INDArray, Mode, Double)"/>, uses padWidth for all dimensions.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pad(@NonNull INDArray toPad, @NonNull int[] padWidth, @NonNull Pad.Mode padMode, double padValue)
		Public Shared Function pad(ByVal toPad As INDArray, ByVal padWidth() As Integer, ByVal padMode As Pad.Mode, ByVal padValue As Double) As INDArray
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim pads[][] As Integer = new Integer[toPad.rank()][padWidth.Length]
			Dim pads()() As Integer = RectangularArrays.RectangularIntegerArray(toPad.rank(), padWidth.Length)
			For i As Integer = 0 To pads.Length - 1
				pads(i) = padWidth
			Next i
			Return pad(toPad, pads, padMode, padValue)
		End Function

		''' <summary>
		''' Pad the given ndarray to the size along each dimension.
		''' </summary>
		''' <param name="toPad"> the ndarray to pad </param>
		''' <param name="padWidth"> the width to pad along each dimension </param>
		''' <param name="padMode"> the mode to pad in </param>
		''' <param name="padValue"> the value used during padding.  Only used when padMode is <seealso cref="Pad.Mode.CONSTANT"/>. </param>
		''' <returns> the padded ndarray
		''' based on the specified mode </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pad(@NonNull INDArray toPad, @NonNull INDArray padWidth, @NonNull Pad.Mode padMode, double padValue)
		Public Shared Function pad(ByVal toPad As INDArray, ByVal padWidth As INDArray, ByVal padMode As Pad.Mode, ByVal padValue As Double) As INDArray

			Preconditions.checkArgument(toPad.rank() = padWidth.size(0), "Must provide padding values for each dimension.  Expected %s pairs for a rank %s array, got %s", toPad.rank(), toPad.rank(), padWidth.size(0))

			Dim newShape(toPad.rank() - 1) As Long
			For i As Integer = 0 To newShape.Length - 1
				newShape(i) = toPad.size(i) + padWidth.getRow(i).sumNumber().intValue()
			Next i
			Dim [out] As INDArray = Nd4j.createUninitialized(toPad.dataType(), newShape)
			Dim op As New Pad(toPad, padWidth, [out], padMode, padValue)

			Return Nd4j.Executioner.exec(op)(0)
		End Function

		''' <summary>
		''' Append the given array with the specified value size along a particular axis.
		''' The prepend method has the same signature and prepends the given array. </summary>
		''' <param name="arr"> the array to append to </param>
		''' <param name="padAmount"> the pad amount of the array to be returned </param>
		''' <param name="val"> the value to append </param>
		''' <param name="axis"> the axis to append to </param>
		''' <returns> the newly created array </returns>
		Public Shared Function append(ByVal arr As INDArray, ByVal padAmount As Integer, ByVal val As Double, ByVal axis As Integer) As INDArray
			Return appendImpl(arr, padAmount, val, axis, True)
		End Function

		''' <summary>
		''' See <seealso cref="append(INDArray, Integer, Double, Integer)"/>. This function calls the implementation with appendFlag = false
		''' to prepend.
		''' </summary>
		Public Shared Function prepend(ByVal arr As INDArray, ByVal padAmount As Integer, ByVal val As Double, ByVal axis As Integer) As INDArray
			Return appendImpl(arr, padAmount, val, axis, False)
		End Function

		' For this function we actually want the 'See also' tag. (Private methods do not generate javadoc, This Javadoc for
		' maintaining the code.)
		''' <summary>
		''' Append / Prepend shared implementation. </summary>
		''' <param name="appendFlag"> flag to determine Append / Prepend. </param>
		''' <seealso cref= #append(INDArray, int, double, int) </seealso>
		Private Shared Function appendImpl(ByVal arr As INDArray, ByVal padAmount As Integer, ByVal val As Double, ByVal axis As Integer, ByVal appendFlag As Boolean) As INDArray
			If padAmount = 0 Then
				Return arr
			End If
			Dim paShape() As Long = ArrayUtil.copy(arr.shape())
			If axis < 0 Then
				axis = axis + arr.shape().Length
			End If
			paShape(axis) = padAmount
			Dim concatArray As INDArray = Nd4j.valueArrayOf(paShape, val, arr.dataType())
			Return If(appendFlag, Nd4j.concat(axis, arr, concatArray), Nd4j.concat(axis, concatArray, arr))
		End Function

		''' <summary>
		''' Expand the array dimensions.
		''' This is equivalent to
		''' adding a new axis dimension </summary>
		''' <param name="input"> the input array </param>
		''' <param name="dimension"> the dimension to add the
		'''                  new axis at </param>
		''' <returns> the array with the new axis dimension </returns>
		Public Shared Function expandDims(ByVal input As INDArray, ByVal dimension As Integer) As INDArray
			If dimension < 0 Then
				dimension += input.rank()
			End If
			Dim shape() As Long = input.shape()
			Dim indexes(input.rank()) As Long
			For i As Integer = 0 To indexes.Length - 1
				indexes(i) = If(i < dimension, shape(i), If(i = dimension, 1, shape(i - 1)))
			Next i
			Return input.reshape(input.ordering(), indexes)
		End Function

		''' <summary>
		''' Squeeze : removes a dimension of size 1 </summary>
		''' <param name="input"> the input array </param>
		''' <param name="dimension"> the dimension to remove </param>
		''' <returns> the array with dimension removed </returns>
		Public Shared Function squeeze(ByVal input As INDArray, ByVal dimension As Integer) As INDArray
			If dimension < 0 Then
				dimension += input.rank()
			End If
			Dim shape() As Long = input.shape()
			Preconditions.checkState(shape(dimension) = 1, String.Format("Squeeze: Only dimension of size 1 can be squeezed. " & "Attempted to squeeze dimension {0:D} of array with shape {1} (size {2:D}).", dimension, ArrayUtils.toString(shape), shape(dimension)))

			Dim newShape() As Long = ArrayUtil.removeIndex(shape, dimension)
			Return input.reshape(input.ordering(), newShape)
		End Function

		''' <summary>
		''' Backend specific:
		''' Returns whether specifying the order
		''' for the blas impl is allowed (cblas) </summary>
		''' <returns> true if the blas impl
		''' can support specifying array order </returns>
		Public Shared Function allowsSpecifyOrdering() As Boolean
			Return allowsOrder
		End Function

		''' <summary>
		''' In place shuffle of an ndarray
		''' along a specified set of dimensions </summary>
		''' <param name="toShuffle"> the ndarray to shuffle </param>
		''' <param name="random"> the random to use </param>
		''' <param name="dimension"> the dimension to do the shuffle </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void shuffle(INDArray toShuffle, Random random, @NonNull int... dimension)
		Public Shared Sub shuffle(ByVal toShuffle As INDArray, ByVal random As Random, ParamArray ByVal dimension() As Integer)
			INSTANCE.shuffle(toShuffle, random, dimension)
		End Sub

		''' <summary>
		''' In place shuffle of an ndarray
		''' along a specified set of dimensions </summary>
		''' <param name="toShuffle"> the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void shuffle(INDArray toShuffle, @NonNull int... dimension)
		Public Shared Sub shuffle(ByVal toShuffle As INDArray, ParamArray ByVal dimension() As Integer)
			INSTANCE.shuffle(toShuffle, New Random(), dimension)
		End Sub

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a specified set of dimensions </summary>
		''' <param name="toShuffle"> the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void shuffle(Collection<INDArray> toShuffle, @NonNull int... dimension)
		Public Shared Sub shuffle(ByVal toShuffle As ICollection(Of INDArray), ParamArray ByVal dimension() As Integer)
			INSTANCE.shuffle(toShuffle, New Random(), dimension)
		End Sub

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a specified set of dimensions </summary>
		''' <param name="toShuffle"> the ndarray to shuffle </param>
		''' <param name="dimension"> the dimension to do the shuffle </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void shuffle(Collection<INDArray> toShuffle, Random rnd, @NonNull int... dimension)
		Public Shared Sub shuffle(ByVal toShuffle As ICollection(Of INDArray), ByVal rnd As Random, ParamArray ByVal dimension() As Integer)
			INSTANCE.shuffle(toShuffle, rnd, dimension)
		End Sub

		''' <summary>
		''' Symmetric in place shuffle of an ndarray
		''' along a variable dimensions
		''' </summary>
		''' <param name="toShuffle"> the ndarray to shuffle </param>
		''' <param name="dimensions"> the dimension to do the shuffle. Please note - order matters here. </param>
		Public Shared Sub shuffle(ByVal toShuffle As IList(Of INDArray), ByVal rnd As Random, ByVal dimensions As IList(Of Integer()))
			INSTANCE.shuffle(toShuffle, rnd, dimensions)
		End Sub

		''' <summary>
		''' Get the primary distributions
		''' factory
		''' </summary>
		''' <returns> the primary distributions </returns>
		Public Shared ReadOnly Property Distributions As DistributionFactory
			Get
				Return DISTRIBUTION_FACTORY
			End Get
		End Property

		''' <summary>
		''' Get the current random generator
		''' </summary>
		''' <returns> the current random generator </returns>
		Public Shared ReadOnly Property Random As org.nd4j.linalg.api.rng.Random
			Get
				Return randomFactory_Conflict.Random
			End Get
		End Property

		''' <summary>
		''' Get the  RandomFactory instance
		''' </summary>
		''' <returns> the  RandomFactory instance </returns>
		Public Shared ReadOnly Property RandomFactory As RandomFactory
			Get
				Return randomFactory_Conflict
			End Get
		End Property

		''' <summary>
		''' Get the convolution singleton
		''' </summary>
		''' <returns> the convolution singleton </returns>
		Public Shared Property Convolution As ConvolutionInstance
			Get
				Return CONVOLUTION_INSTANCE
			End Get
			Set(ByVal convolutionInstance As ConvolutionInstance)
				If convolutionInstance Is Nothing Then
					Throw New System.ArgumentException("No null instances allowed")
				End If
				CONVOLUTION_INSTANCE = convolutionInstance
			End Set
		End Property


		''' <summary>
		''' Returns the shape of the ndarray </summary>
		''' <param name="arr"> the array to get the shape of </param>
		''' <returns> the shape of tihs ndarray </returns>
		Public Shared Function shape(ByVal arr As INDArray) As Long()
			Return arr.shape()
		End Function

		''' <summary>
		''' Create an ndarray based on the given data </summary>
		''' <param name="sliceShape"> the shape of each slice </param>
		''' <param name="arrays"> the arrays of data to create </param>
		''' <returns> the ndarray of the specified shape where
		''' number of slices is equal to array length and each
		''' slice is the specified shape </returns>
		Public Shared Function create(ByVal sliceShape() As Integer, ParamArray ByVal arrays()() As Single) As INDArray
			Dim slices As Integer = arrays.Length
			Dim ret As INDArray = Nd4j.createUninitialized(DataType.FLOAT, ArrayUtil.toLongArray(ArrayUtil.combine(New Integer() {slices}, sliceShape)))
			Dim i As Integer = 0
			Do While i < ret.slices()
				ret.putSlice(i, Nd4j.create(arrays(i)).reshape(ArrayUtil.toLongArray(sliceShape)))
				i += 1
			Loop
			Return ret
		End Function

		''' <summary>
		''' See <seealso cref="create(LongShapeDescriptor, Boolean)"/> with initialize set to true.
		''' </summary>
		Public Shared Function create(ByVal descriptor As LongShapeDescriptor) As INDArray
			Return create(descriptor, True)
		End Function

		''' <summary>
		''' Create an ndarray based on the given description, </summary>
		''' <param name="descriptor"> object with data for array creation. </param>
		''' <param name="initialize"> true/false creates initialized/uninitialized array. </param>
		''' <returns> the ndarray of the specified description. </returns>
		Public Shared Function create(ByVal descriptor As LongShapeDescriptor, ByVal initialize As Boolean) As INDArray
			If descriptor.Empty AndAlso descriptor.rank() = 0 Then
				Return Nd4j.empty(descriptor.dataType())
			End If
			If initialize Then
				Return create(descriptor.dataType(), descriptor.getShape(), descriptor.getStride(), descriptor.getOrder())
			Else
				Return createUninitialized(descriptor.dataType(), descriptor.getShape(), descriptor.getOrder())
			End If
		End Function

		''' <summary>
		''' See <seealso cref="create(Integer[], Single[]...)"/>
		''' </summary>
		Public Shared Function create(ByVal sliceShape() As Integer, ParamArray ByVal arrays()() As Double) As INDArray
			Dim slices As Integer = arrays.Length
			Dim ret As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, ArrayUtil.toLongArray(ArrayUtil.combine(New Integer() {slices}, sliceShape)))
			Dim i As Integer = 0
			Do While i < ret.slices()
				ret.putSlice(i, Nd4j.create(arrays(i)).reshape(ArrayUtil.toLongArray(sliceShape)))
				i += 1
			Loop
			Return ret
		End Function

		''' <summary>
		''' Get the backend Environment instance </summary>
		''' <returns> The backend Environment instance </returns>
		Public Shared ReadOnly Property Environment As Environment
			Get
				Return backend_Conflict.Environment
			End Get
		End Property

		''' <summary>
		''' Get the operation executioner instance.
		''' </summary>
		''' <returns> the operation executioner instance. </returns>
		Public Shared ReadOnly Property Executioner As OpExecutioner
			Get
				Return OP_EXECUTIONER_INSTANCE
			End Get
		End Property

		''' <summary>
		''' Get the data buffer factory instance.
		''' </summary>
		''' <returns> the data buffer factory instance. </returns>
		Public Shared ReadOnly Property DataBufferFactory As DataBufferFactory
			Get
				Return DATA_BUFFER_FACTORY_INSTANCE
			End Get
		End Property

		''' <summary>
		'''  Roll the specified axis backwards,
		'''  until it lies in a given position.
		'''  Starting ends up being zero.
		'''  See numpy's rollaxis </summary>
		''' <param name="a"> the array to roll </param>
		''' <param name="axis"> the axis to roll backwards </param>
		''' <returns> the rolled ndarray </returns>
		Public Shared Function rollAxis(ByVal a As INDArray, ByVal axis As Integer) As INDArray
			Return rollAxis(a, axis, 0)
		End Function

		''' <summary>
		''' Get the maximum  values for a dimension. </summary>
		''' <param name="arr"> input array. </param>
		''' <param name="dimension"> the dimension along which to get the maximum </param>
		''' <returns> array of maximum values. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray argMax(INDArray arr, @NonNull int... dimension)
		Public Shared Function argMax(ByVal arr As INDArray, ParamArray ByVal dimension() As Integer) As INDArray
			Dim imax As val = New ArgMax(New INDArray(){arr},Nothing,False, dimension)
			Return Nd4j.Executioner.exec(imax)(0)
		End Function

		''' <summary>
		''' See <seealso cref="argMax(INDArray, Integer...)"/> but return minimum values.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray argMin(INDArray arr, @NonNull int... dimension)
		Public Shared Function argMin(ByVal arr As INDArray, ParamArray ByVal dimension() As Integer) As INDArray
			Dim imin As val = New ArgMin(New INDArray(){arr}, Nothing,False,dimension)
			Return Nd4j.Executioner.exec(imin)(0)
		End Function

		''' <summary>
		'''  Roll the specified axis backwards,
		'''  until it lies in a given position.
		'''  See numpy's rollaxis </summary>
		''' <param name="a"> the array to roll </param>
		''' <param name="axis"> the axis to roll backwards </param>
		''' <param name="start"> the starting point </param>
		''' <returns> the rolled ndarray </returns>
		Public Shared Function rollAxis(ByVal a As INDArray, ByVal axis As Integer, ByVal start As Integer) As INDArray
			If axis < 0 Then
				axis += a.rank()
			End If
			If start < 0 Then
				start += a.rank()
			End If
			If axis = start Then
				Return a
			End If
			If axis < start Then
				start -= 1
			End If
			If Not (axis >= 0 AndAlso axis < a.rank()) Then
				Throw New System.ArgumentException("Axis must be >= 0 && < start")
			End If
			If Not (start >= 0 AndAlso axis < a.rank() + 1) Then
				Throw New System.ArgumentException("Axis must be >= 0 && < start")
			End If

'JAVA TO VB CONVERTER NOTE: The variable range was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim range_Conflict As IList(Of Integer) = New List(Of Integer)(Ints.asList(ArrayUtil.range(0, a.rank())))
			range_Conflict.RemoveAt(axis)
			range_Conflict.Insert(start, axis)
			Dim newRange() As Integer = Ints.toArray(range_Conflict)
			Return a.permute(newRange)

		End Function

		''' <summary>
		''' Tensor matrix multiplication.
		''' Both tensors must be the same rank
		''' </summary>
		''' <param name="a"> the left tensor </param>
		''' <param name="b"> the  right tensor </param>
		''' <param name="result"> the result array </param>
		''' <param name="axes"> the axes for each array to do matrix multiply along </param>
		''' <returns> the result array </returns>
		Public Shared Function tensorMmul(ByVal a As INDArray, ByVal b As INDArray, ByVal result As INDArray, ByVal axes()() As Integer) As INDArray
			Dim validationLength As Integer = Math.Min(axes(0).Length, axes(1).Length)
			For i As Integer = 0 To validationLength - 1
				If a.size(axes(0)(i)) <> b.size(axes(1)(i)) Then
					Throw New System.ArgumentException("Size of the given axes at each dimension must be the same size.")
				End If
				If axes(0)(i) < 0 Then
					axes(0)(i) += a.rank()
				End If
				If axes(1)(i) < 0 Then
					axes(1)(i) += b.rank()
				End If

			Next i

			Dim listA As IList(Of Integer) = New List(Of Integer)()
			Dim i As Integer = 0
			Do While i < a.rank()
				If Not Ints.contains(axes(0), i) Then
					listA.Add(i)
				End If
				i += 1
			Loop

			Dim newAxesA() As Integer = Ints.concat(Ints.toArray(listA), axes(0))

			Dim listB As IList(Of Integer) = New List(Of Integer)()
			i = 0
			Do While i < b.rank()
				If Not Ints.contains(axes(1), i) Then
					listB.Add(i)
				End If
				i += 1
			Loop

			Dim newAxesB() As Integer = Ints.concat(axes(1), Ints.toArray(listB))

			Dim n2 As Integer = 1
			Dim aLength As Integer = Math.Min(a.rank(), axes(0).Length)
			For i As Integer = 0 To aLength - 1
				n2 *= a.size(axes(0)(i))
			Next i

			'if listA and listB are empty these donot initialize.
			'so initializing with {1} which will then get overriden if not empty
			Dim newShapeA() As Long = {-1, n2}
			Dim oldShapeA() As Long = getOldShape(listA, a)

			Dim n3 As Integer = 1
			Dim bNax As Integer = Math.Min(b.rank(), axes(1).Length)
			For i As Integer = 0 To bNax - 1
				n3 *= b.size(axes(1)(i))
			Next i

			Dim newShapeB() As Long = {n3, -1}
			Dim oldShapeB() As Long = getOldShape(listB, b)

			Dim at As INDArray = a.permute(newAxesA).reshape(newShapeA)
			Dim bt As INDArray = b.permute(newAxesB).reshape(newShapeB)
			Dim ret As INDArray = at.mmul(bt,result)

			Dim aPlusB() As Long = Longs.concat(oldShapeA, oldShapeB)
			Return ret.reshape(aPlusB)
		End Function

		' Some duplicate code that refactored out:
		Private Shared Function getOldShape(ByVal list As IList(Of Integer), ByVal x As INDArray) As Long()
			Dim res() As Long
			If list.Count = 0 Then
				res = New Long() {1}
			Else
				res= Longs.toArray(list)
				For i As Integer = 0 To res.Length - 1
					res(i) = x.size(CInt(res(i)))
				Next i
			End If
			Return res
		End Function

		''' <summary>
		''' Tensor matrix multiplication.
		''' Both tensors must be the same rank
		''' </summary>
		''' <param name="a"> the left tensor </param>
		''' <param name="b"> the  right tensor </param>
		''' <param name="axes"> the axes for each array to do matrix multiply along </param>
		''' <returns> the multiplication result. </returns>
		Public Shared Function tensorMmul(ByVal a As INDArray, ByVal b As INDArray, ByVal axes()() As Integer) As INDArray
			Dim op As CustomOp = DynamicCustomOp.builder("tensordot").addInputs(a, b).addIntegerArguments(axes(0).Length).addIntegerArguments(axes(0)).addIntegerArguments(axes(1).Length).addIntegerArguments(axes(1)).build()

			Dim l As IList(Of LongShapeDescriptor) = op.calculateOutputShape()
			Dim [out] As INDArray = Nd4j.create(l(0).asDataType(a.dataType()))
			op.addOutputArgument([out])
			Nd4j.exec(op)

			Return [out]
		End Function

		''' <summary>
		''' matrix multiply: implements op(a)*op(b)
		''' 
		''' where op(x) means transpose x (or not) depending on
		''' setting of arguments transposea and transposeb.<br>
		''' so gemm(a,b,false,false) == a.mmul(b), gemm(a,b,true,false) == a.transpose().mmul(b) etc. </summary>
		''' <param name="a"> first matrix </param>
		''' <param name="b"> second matrix </param>
		''' <param name="transposeA"> if true: transpose matrix a before mmul </param>
		''' <param name="transposeB"> if true: transpose matrix b before mmul </param>
		''' <returns> result </returns>
		Public Shared Function gemm(ByVal a As INDArray, ByVal b As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean) As INDArray
			Dim cRows As Long = (If(transposeA, a.columns(), a.rows()))
			Dim cCols As Long = (If(transposeB, b.rows(), b.columns()))
			Dim c As INDArray = Nd4j.createUninitialized(a.dataType(), New Long() {cRows, cCols},If(a.ordering() = "c"c AndAlso b.ordering() = "c"c, "c"c, "f"c))
			Return gemm(a, b, c, transposeA, transposeB, 1.0, 0.0)
		End Function

		''' <summary>
		'''  Matrix multiply: Implements c = alpha*op(a)*op(b) + beta*c where op(X) means transpose X (or not)
		''' depending on setting of arguments transposeA and transposeB.<br>
		''' Note that matrix c MUST be fortran order, have zero offset and have c.data().length == c.length().
		''' i.e., the result array must not be a view. An exception will be thrown otherwise.<br>
		''' (Note: some views are allowed, if and only if they have f order and are contiguous in the buffer other than an
		''' offset. Put another way, they must be f order and have strides identical to a non-view/default array of the same shape)<br>
		''' Don't use this unless you know about level 3 blas and NDArray storage orders. </summary>
		''' <param name="a"> First matrix </param>
		''' <param name="b"> Second matrix </param>
		''' <param name="c"> result matrix. Used in calculation (assuming beta != 0) and result is stored in this. f order, and not a view only </param>
		''' <param name="transposeA"> if true: transpose matrix a before mmul </param>
		''' <param name="transposeB"> if true: transpose matrix b before mmul </param>
		''' <returns> result, i.e., matrix c is returned for convenience </returns>
		Public Shared Function gemm(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double) As INDArray
			Preconditions.checkArgument(c.elementWiseStride() = 1, "Nd4j.gemm() C array should NOT be a view")

			Nd4j.exec(New Mmul(a, b, c, alpha, beta, MMulTranspose.builder().transposeA(transposeA).transposeB(transposeB).build()))
			Return c
		End Function

		''' <summary>
		''' Matrix multiplication/dot product
		''' 
		''' Depending on inputs dimensionality output result might be different.
		''' matrix x matrix = BLAS gemm
		''' vector x matrix = BLAS gemm
		''' vector x vector = BLAS dot
		''' vector x scalar = element-wise mul
		''' scalar x vector = element-wise mul
		''' tensor x tensor = matrix multiplication using the last two dimensions
		''' 
		''' Transpose operations only available where applicable. In the
		''' tensor x tensor case it will be applied only to the last two dimensions.
		''' </summary>
		''' <param name="a"> First tensor </param>
		''' <param name="b"> Second tensor </param>
		''' <param name="result"> result matrix. </param>
		''' <param name="transposeA"> if true: transpose matrix a before mmul </param>
		''' <param name="transposeB"> if true: transpose matrix b before mmul </param>
		''' <param name="transposeResult"> if true: result matrix will be transposed </param>
		''' <returns> result, i.e., result matrix is returned for convenience </returns>
		Public Shared Function matmul(ByVal a As INDArray, ByVal b As INDArray, ByVal result As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal transposeResult As Boolean) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ops.impl.reduce.Mmul op = new org.nd4j.linalg.api.ops.impl.reduce.Mmul(a, b, result, org.nd4j.linalg.api.blas.params.MMulTranspose.builder().transposeA(transposeA).transposeB(transposeB).transposeResult(transposeResult).build());
			Dim op As New Mmul(a, b, result, MMulTranspose.builder().transposeA(transposeA).transposeB(transposeB).transposeResult(transposeResult).build())
			Return exec(op)(0)
		End Function

		''' <summary>
		''' Matrix multiplication/dot product.<br>
		''' 
		''' See <seealso cref="matmul(INDArray, INDArray, INDArray, Boolean, Boolean, Boolean)"/>
		''' </summary>
		Public Shared Function matmul(ByVal a As INDArray, ByVal b As INDArray, ByVal result As INDArray) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ops.impl.reduce.Mmul op = new org.nd4j.linalg.api.ops.impl.reduce.Mmul(a, b, result, null);
			Dim op As New Mmul(a, b, result, Nothing)
			Return exec(op)(0)
		End Function

		''' <summary>
		''' Matrix multiplication/dot product.<br>
		''' 
		''' See <seealso cref="matmul(INDArray, INDArray, INDArray, Boolean, Boolean, Boolean)"/>
		''' </summary>
		Public Shared Function matmul(ByVal a As INDArray, ByVal b As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal transposeResult As Boolean) As INDArray
			Return matmul(a, b, Nothing, transposeA, transposeB, transposeResult)
		End Function

		''' <summary>
		''' Matrix multiplication/dot product
		''' 
		''' See <seealso cref="matmul(INDArray, INDArray, INDArray, Boolean, Boolean, Boolean)"/>
		''' </summary>
		Public Shared Function matmul(ByVal a As INDArray, ByVal b As INDArray) As INDArray
			Return matmul(a,b, Nothing)
		End Function

		''' <summary>
		''' The factory used for creating ndarrays
		''' </summary>
		''' <returns> the factory instance used for creating ndarrays </returns>
		Public Shared Function factory() As NDArrayFactory
			Return INSTANCE
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.cumsum(Integer)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		''' <returns> scalar ndarray. </returns>
		Public Shared Function cumsum(ByVal compute As INDArray) As INDArray
			Return compute.cumsum(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.max(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function max(ByVal compute As INDArray) As INDArray
			Return compute.max(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.min(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function min(ByVal compute As INDArray) As INDArray
			Return compute.min(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.prod(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function prod(ByVal compute As INDArray) As INDArray
			Return compute.prod(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.normmax(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function normmax(ByVal compute As INDArray) As INDArray
			Return compute.normmax(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.norm2(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function norm2(ByVal compute As INDArray) As INDArray
			Return compute.norm2(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.norm1(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function norm1(ByVal compute As INDArray) As INDArray
			Return compute.norm1(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.std(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function std(ByVal compute As INDArray) As INDArray
			Return compute.std(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.var(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function var(ByVal compute As INDArray) As INDArray
			Return compute.var(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.sum(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function sum(ByVal compute As INDArray) As INDArray
			Return compute.sum(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.mean(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function mean(ByVal compute As INDArray) As INDArray
			Return compute.mean(Integer.MaxValue)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.cumsum(Integer)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function cumsum(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.cumsum(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.max(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function max(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.max(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.min(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function min(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.min(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.prod(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function prod(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.prod(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.normmax(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function normmax(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.normmax(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.norm2(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function norm2(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.norm2(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.norm1(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function norm1(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.norm1(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.std(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function std(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.std(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.var(Integer...)"/> with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function var(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.var(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.sum(Integer...)"/>with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function sum(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.sum(dimension)
		End Function

		''' <summary>
		''' See <seealso cref="org.nd4j.linalg.api.ndarray.INDArray.mean(Integer...)"/>with Integer.MAX_VALUE for full array reduction.
		''' </summary>
		Public Shared Function mean(ByVal compute As INDArray, ByVal dimension As Integer) As INDArray
			Return compute.mean(dimension)
		End Function

		''' <summary>
		''' Create a view of a data buffer
		''' Leverages the underlying storage of the buffer with a new view
		''' </summary>
		''' <param name="underlyingBuffer"> the underlying buffer </param>
		''' <param name="offset"> the offset for the view </param>
		''' <returns> the new view of the data buffer </returns>
		Public Shared Function createBuffer(ByVal underlyingBuffer As DataBuffer, ByVal offset As Long, ByVal length As Long) As DataBuffer
			Return DATA_BUFFER_FACTORY_INSTANCE.create(underlyingBuffer, offset, length)
		End Function

		''' <summary>
		''' Create a buffer equal of length prod(shape)
		''' </summary>
		''' <param name="shape"> the shape of the buffer to create </param>
		''' <param name="type">  the opType to create </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal shape() As Integer, ByVal type As DataType, ByVal offset As Long) As DataBuffer
			Dim length As Integer = ArrayUtil.prod(shape)
			Return If(type = DataType.DOUBLE, createBuffer(New Double(length - 1){}, offset), createBuffer(New Single(length - 1){}, offset))
		End Function

		''' <summary>
		''' Creates a buffer of the specified opType and length with the given byte buffer.
		''' 
		''' This will wrap the buffer as a reference (no copy)
		''' if the allocation opType is the same. </summary>
		''' <param name="buffer"> the buffer to create from </param>
		''' <param name="type"> the opType of buffer to create </param>
		''' <param name="length"> the length of the buffer </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal buffer As ByteBuffer, ByVal type As DataType, ByVal length As Integer, ByVal offset As Long) As DataBuffer
			Return DATA_BUFFER_FACTORY_INSTANCE.create(buffer, type, length, offset)
		End Function

		''' <summary>
		''' Create a buffer based on the data opType
		''' </summary>
		''' <param name="data"> the data to create the buffer with </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal data() As SByte, ByVal length As Integer, ByVal offset As Long) As DataBuffer
			Dim ret As DataBuffer
			If dataType() = DataType.DOUBLE Then
				ret = DATA_BUFFER_FACTORY_INSTANCE.createDouble(offset, data, length)
			Else
				ret = DATA_BUFFER_FACTORY_INSTANCE.createFloat(offset, data, length)
			End If
			Return ret
		End Function

		''' <summary>
		''' Creates a buffer of the specified length based on the data opType
		''' </summary>
		''' <param name="length"> the length of te buffer </param>
		''' <returns> the buffer to create </returns>
		Public Shared Function createBuffer(ByVal length As Integer, ByVal offset As Long) As DataBuffer
			Dim ret As DataBuffer
			If dataType() = DataType.FLOAT Then
				ret = DATA_BUFFER_FACTORY_INSTANCE.createFloat(offset, length)
			ElseIf dataType() = DataType.INT Then
				ret = DATA_BUFFER_FACTORY_INSTANCE.createInt(offset, length)
			ElseIf dataType() = DataType.DOUBLE Then
				ret = DATA_BUFFER_FACTORY_INSTANCE.createDouble(offset, length)
			ElseIf dataType() = DataType.HALF Then
				ret = DATA_BUFFER_FACTORY_INSTANCE.createHalf(offset, length)
			Else
				ret = Nothing
			End If

			Return ret
		End Function

		Private Shared Function getIndexerByType(ByVal pointer As Pointer, ByVal dataType As DataType) As Indexer
			Select Case dataType.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64, [LONG]
					Return LongIndexer.create(CType(pointer, LongPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32
					Return UIntIndexer.create(CType(pointer, IntPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Return IntIndexer.create(CType(pointer, IntPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16
					Return UShortIndexer.create(CType(pointer, ShortPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Return ShortIndexer.create(CType(pointer, ShortPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Return ByteIndexer.create(CType(pointer, BytePointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					Return UByteIndexer.create(CType(pointer, BytePointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Return BooleanIndexer.create(CType(pointer, BooleanPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Return FloatIndexer.create(CType(pointer, FloatPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16
					Return Bfloat16Indexer.create(CType(pointer, ShortPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Return HalfIndexer.create(CType(pointer, ShortPointer))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Return DoubleIndexer.create(CType(pointer, DoublePointer))
				Case Else
					Throw New System.NotSupportedException()
			End Select
		End Function

		''' <summary>
		''' Creates a buffer of the specified type and length with the given pointer.
		''' </summary>
		''' <param name="pointer"> pointer to data to create from. </param>
		''' <param name="length"> the length of the buffer </param>
		''' <param name="dataType"> the opType of buffer to create, </param>
		''' <returns> the created buffer </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static DataBuffer createBuffer(@NonNull Pointer pointer, long length, @NonNull DataType dataType)
		Public Shared Function createBuffer(ByVal pointer As Pointer, ByVal length As Long, ByVal dataType As DataType) As DataBuffer
			Dim nPointer As Pointer = getPointer(pointer, dataType)
			Return DATA_BUFFER_FACTORY_INSTANCE.create(nPointer, dataType, length, getIndexerByType(nPointer, dataType))
		End Function

		''' <summary>
		''' Creates a buffer of the specified type and length with the given pointer at the specified device.
		''' (This method is relevant only for a CUDA backend).
		''' </summary>
		''' <param name="pointer">        pointer to data to create from. </param>
		''' <param name="devicePointer">  pointer to device to create in (only implemented in the CUDA backend) </param>
		''' <param name="length">         the length of the buffer </param>
		''' <param name="dataType">       the opType of buffer to create, </param>
		''' <returns>               the created buffer </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static DataBuffer createBuffer(@NonNull Pointer pointer, @NonNull Pointer devicePointer, long length, @NonNull DataType dataType)
		Public Shared Function createBuffer(ByVal pointer As Pointer, ByVal devicePointer As Pointer, ByVal length As Long, ByVal dataType As DataType) As DataBuffer
			Dim nPointer As Pointer = getPointer(pointer, dataType)
			Return DATA_BUFFER_FACTORY_INSTANCE.create(nPointer, devicePointer, dataType, length, getIndexerByType(nPointer, dataType))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static Pointer getPointer(@NonNull Pointer pointer, @NonNull DataType dataType)
		Private Shared Function getPointer(ByVal pointer As Pointer, ByVal dataType As DataType) As Pointer
			Dim nPointer As Pointer
			Select Case dataType.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64, [LONG]
					nPointer = New LongPointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32, INT
					nPointer = New IntPointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16, [SHORT]
					nPointer = New ShortPointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					nPointer = New BytePointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					nPointer = New BytePointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					nPointer = New BooleanPointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16, HALF
					nPointer = New ShortPointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					nPointer = New FloatPointer(pointer)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					nPointer = New DoublePointer(pointer)
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType)
			End Select

			Return nPointer
		End Function

		''' <summary>
		''' Create a buffer based on the data opType
		''' </summary>
		''' <param name="data"> the data to create the buffer with </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal data() As Single, ByVal offset As Long) As DataBuffer
			Return createTypedBuffer(Arrays.CopyOfRange(data, CInt(offset), data.Length), DataType.FLOAT, Nd4j.MemoryManager.CurrentWorkspace)
		End Function

		''' <summary>
		''' Create a buffer based on the data opType
		''' </summary>
		''' <param name="data"> the data to create the buffer with </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal data() As Double, ByVal offset As Long) As DataBuffer
			Return createTypedBuffer(Arrays.CopyOfRange(data, CInt(offset), data.Length), DataType.DOUBLE, Nd4j.MemoryManager.CurrentWorkspace)
		End Function

		''' <summary>
		''' Create a buffer equal of length prod(shape)
		''' </summary>
		''' <param name="shape"> the shape of the buffer to create </param>
		''' <param name="type">  the opType to create </param>
		''' <returns> the created buffer </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static DataBuffer createBuffer(@NonNull int[] shape, @NonNull DataType type)
		Public Shared Function createBuffer(ByVal shape() As Integer, ByVal type As DataType) As DataBuffer
			Return createBuffer(ArrayUtil.toLongArray(shape), type)
		End Function

		''' <summary>
		''' See <seealso cref=" createBuffer(Integer[], DataType)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static DataBuffer createBuffer(@NonNull long[] shape, @NonNull DataType type)
		Public Shared Function createBuffer(ByVal shape() As Long, ByVal type As DataType) As DataBuffer
			Dim length As Long = Shape.lengthOf(shape)

			Select Case type.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createBool(length, True), DATA_BUFFER_FACTORY_INSTANCE.createBool(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createUByte(length, True), DATA_BUFFER_FACTORY_INSTANCE.createUByte(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createUShort(length, True), DATA_BUFFER_FACTORY_INSTANCE.createUShort(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createUInt(length, True), DATA_BUFFER_FACTORY_INSTANCE.createUInt(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createULong(length, True), DATA_BUFFER_FACTORY_INSTANCE.createULong(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createByte(length, True), DATA_BUFFER_FACTORY_INSTANCE.createByte(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createShort(length, True), DATA_BUFFER_FACTORY_INSTANCE.createShort(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createInt(length, True), DATA_BUFFER_FACTORY_INSTANCE.createInt(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createLong(length, True), DATA_BUFFER_FACTORY_INSTANCE.createLong(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createHalf(length, True), DATA_BUFFER_FACTORY_INSTANCE.createHalf(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createBFloat16(length, True), DATA_BUFFER_FACTORY_INSTANCE.createBFloat16(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createFloat(length, True), DATA_BUFFER_FACTORY_INSTANCE.createFloat(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createDouble(length, True), DATA_BUFFER_FACTORY_INSTANCE.createDouble(length, True, Nd4j.MemoryManager.CurrentWorkspace))
				Case Else
					Throw New System.NotSupportedException("Cannot create type: " & type)
			End Select
		End Function

		''' <summary>
		''' Create a buffer equal of length prod(shape). The buffer is 'detached': Not in any memory workspace even if a
		''' workspace is currently open.
		''' </summary>
		''' <param name="shape"> the shape of the buffer to create </param>
		''' <param name="type"> the opType to create </param>
		''' <returns> the created buffer. </returns>
		Public Shared Function createBufferDetached(ByVal shape() As Integer, ByVal type As DataType) As DataBuffer
			Return createBufferDetachedImpl(Shape.lengthOf(shape), type)
		End Function

		''' <summary>
		''' See <seealso cref=" createBufferDetached(Integer[], DataType)"/>
		''' </summary>
		Public Shared Function createBufferDetached(ByVal shape() As Long, ByVal type As DataType) As DataBuffer
			Return createBufferDetachedImpl(Shape.lengthOf(shape), type)
		End Function

		' used by createBufferDetached(long[] DataType) and createBufferDetached(int[] , DataType)
		Private Shared Function createBufferDetachedImpl(ByVal length As Long, ByVal type As DataType) As DataBuffer
			Select Case type.innerEnumValue

				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Return DATA_BUFFER_FACTORY_INSTANCE.createDouble(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Return DATA_BUFFER_FACTORY_INSTANCE.createFloat(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Return DATA_BUFFER_FACTORY_INSTANCE.createHalf(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16
					Return DATA_BUFFER_FACTORY_INSTANCE.createBFloat16(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64
					Return DATA_BUFFER_FACTORY_INSTANCE.createULong(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					Return DATA_BUFFER_FACTORY_INSTANCE.createLong(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32
					Return DATA_BUFFER_FACTORY_INSTANCE.createUInt(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Return DATA_BUFFER_FACTORY_INSTANCE.createInt(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16
					Return DATA_BUFFER_FACTORY_INSTANCE.createUShort(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Return DATA_BUFFER_FACTORY_INSTANCE.createShort(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					Return DATA_BUFFER_FACTORY_INSTANCE.createUByte(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Return DATA_BUFFER_FACTORY_INSTANCE.createByte(length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Return DATA_BUFFER_FACTORY_INSTANCE.createBool(length)
				Case Else
					Throw New System.NotSupportedException("Cannot create type: " & type)
			End Select
		End Function

		''' <summary>
		''' Creates a buffer of the specified opType
		''' and length with the given byte buffer.
		''' 
		''' This will wrap the buffer as a reference (no copy)
		''' if the allocation opType is the same. </summary>
		''' <param name="buffer"> the buffer to create from </param>
		''' <param name="type"> the opType of buffer to create </param>
		''' <param name="length"> the length of the buffer </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal buffer As ByteBuffer, ByVal type As DataType, ByVal length As Integer) As DataBuffer
			Return createBuffer(buffer, type, length, 0)
		End Function


		''' <summary>
		''' Create a buffer equal of length prod(shape)
		''' </summary>
		''' <param name="data"> the shape of the buffer to create </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal data() As Integer) As DataBuffer
			Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createInt(data), DATA_BUFFER_FACTORY_INSTANCE.createInt(data, Nd4j.MemoryManager.CurrentWorkspace))
		End Function

		''' <summary>
		''' Create a buffer equal of length prod(shape)
		''' </summary>
		''' <param name="data"> the shape of the buffer to create </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal data() As Long) As DataBuffer
			Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createLong(data), DATA_BUFFER_FACTORY_INSTANCE.createLong(data, Nd4j.MemoryManager.CurrentWorkspace))
		End Function

		''' <summary>
		''' Create a buffer equal of length prod(shape). This method is NOT affected by workspaces
		''' </summary>
		''' <param name="data">  the shape of the buffer to create </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBufferDetached(ByVal data() As Integer) As DataBuffer
			Return DATA_BUFFER_FACTORY_INSTANCE.createInt(data)
		End Function

		''' <summary>
		''' Create a buffer equal of length prod(shape). This method is NOT affected by workspaces
		''' </summary>
		''' <param name="data"> the shape of the buffer to create </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBufferDetached(ByVal data() As Long) As DataBuffer
			Return DATA_BUFFER_FACTORY_INSTANCE.createLong(data)
		End Function

		''' <summary>
		''' Creates a buffer of the specified length based on the data opType
		''' </summary>
		''' <param name="length"> the length of te buffer </param>
		''' <returns> the buffer to create </returns>
		Public Shared Function createBuffer(ByVal length As Long) As DataBuffer
			Return createBuffer(length, True)
		End Function

		''' <summary>
		''' Create a data buffer
		''' based on a pointer
		''' with the given opType and length </summary>
		''' <param name="pointer"> the pointer to create the buffer for </param>
		''' <param name="type"> the opType of pointer </param>
		''' <param name="length"> the length of the buffer </param>
		''' <param name="indexer"> the indexer to use </param>
		''' <returns> the data buffer based on the given parameters </returns>
		Public Shared Function createBuffer(ByVal pointer As Pointer, ByVal type As DataType, ByVal length As Long, ByVal indexer As Indexer) As DataBuffer
			Return DATA_BUFFER_FACTORY_INSTANCE.create(pointer, type, length, indexer)
		End Function

		''' <summary>
		''' See {@link  #createBuffer(DataType dataType, long length, boolean initialize) with default datatype.
		''' </summary>
		Public Shared Function createBuffer(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return createBuffer(Nd4j.dataType(), length, initialize)
		End Function

		''' <summary>
		''' Create a data buffer based on datatype. </summary>
		''' <param name="dataType"> the type of buffer to create </param>
		''' <param name="length">  the length of the buffer </param>
		''' <param name="initialize">  flag to leave the underlying memory (false) or initialize with zero (true). </param>
		''' <returns> the created buffer. </returns>
		Public Shared Function createBuffer(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.create(dataType, length, initialize), DATA_BUFFER_FACTORY_INSTANCE.create(dataType,length, initialize, Nd4j.MemoryManager.CurrentWorkspace))
		End Function

		''' <summary>
		''' Create a data buffer based on datatype, workspace. </summary>
		''' <param name="dataType"> the type of buffer to create </param>
		''' <param name="length">  the length of the buffer </param>
		''' <param name="initialize">  flag to leave the underlying memory (false) or initialize with zero (true). </param>
		''' <param name="workspace"> workspace to use for buffer creation. </param>
		''' <returns> the created buffer. </returns>
		Public Shared Function createBuffer(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return If(workspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.create(dataType, length, initialize), DATA_BUFFER_FACTORY_INSTANCE.create(dataType,length, initialize, workspace))
		End Function

		''' <summary>
		''' Create a buffer based on the data opType </summary>
		''' <param name="data"> the data to create the buffer with </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBuffer(ByVal data() As Single) As DataBuffer
			Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createFloat(data), DATA_BUFFER_FACTORY_INSTANCE.createFloat(data, Nd4j.MemoryManager.CurrentWorkspace))
		End Function

		''' <summary>
		''' Create a buffer based on underlying array. </summary>
		''' <param name="data"> data to create the buffer with </param>
		''' <returns> the created buffer </returns>
		Public Shared Function createBufferDetached(ByVal data() As Single) As DataBuffer
			Return DATA_BUFFER_FACTORY_INSTANCE.createFloat(data)
		End Function

		''' <summary>
		''' See <seealso cref="createBufferDetached(Single[])"/>
		''' </summary>
		Public Shared Function createBufferDetached(ByVal data() As Double) As DataBuffer
			Return DATA_BUFFER_FACTORY_INSTANCE.createDouble(data)
		End Function

		''' <summary>
		''' See <seealso cref="createBuffer(Single[])"/>
		''' </summary>
		Public Shared Function createBuffer(ByVal data() As Double) As DataBuffer
			Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.createDouble(data), DATA_BUFFER_FACTORY_INSTANCE.createDouble(data, Nd4j.MemoryManager.CurrentWorkspace))
		End Function

		' refactoring of duplicate code.
		Private Shared Function getDataBuffer(ByVal length As Integer, ByVal dataType As DataType) As DataBuffer
			Return If(Nd4j.MemoryManager.CurrentWorkspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.create(dataType, length, False), DATA_BUFFER_FACTORY_INSTANCE.create(dataType, length, False, Nd4j.MemoryManager.CurrentWorkspace))
		End Function

		''' <summary>
		''' Create a buffer based on the data of the underlying java array with the specified type.. </summary>
		''' <param name="data"> underlying java array </param>
		''' <param name="dataType"> specified type. </param>
		''' <returns> created buffer, </returns>
		Public Shared Function createTypedBuffer(ByVal data() As Double, ByVal dataType As DataType) As DataBuffer
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBuffer(Double[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBuffer(ByVal data() As Single, ByVal dataType As DataType) As DataBuffer
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBuffer(Single[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBuffer(ByVal data() As Integer, ByVal dataType As DataType) As DataBuffer
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBuffer(Single[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBuffer(ByVal data() As Long, ByVal dataType As DataType) As DataBuffer
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBuffer(Single[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBuffer(ByVal data() As Short, ByVal dataType As DataType) As DataBuffer
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBuffer(Single[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBuffer(ByVal data() As SByte, ByVal dataType As DataType) As DataBuffer
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBuffer(Single[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBuffer(ByVal data() As Boolean, ByVal dataType As DataType) As DataBuffer
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType)
			buffer.Data = data
			Return buffer
		End Function


		' refactoring of duplicate code.
		Private Shared Function getDataBuffer(ByVal length As Integer, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return If(workspace Is Nothing, DATA_BUFFER_FACTORY_INSTANCE.create(dataType, length, False), DATA_BUFFER_FACTORY_INSTANCE.create(dataType, length, False, workspace))
		End Function

		''' <summary>
		''' Create a buffer based on the data of the underlying java array, specified type and workspace </summary>
		''' <param name="data"> underlying java array </param>
		''' <param name="dataType"> specified type. </param>
		''' <param name="workspace"> specified workspace. </param>
		''' <returns> created buffer, </returns>
		Public Shared Function createTypedBuffer(ByVal data() As Double, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As DataBuffer
			'val buffer = workspace == null ? DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.length, false) : DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.length, false, workspace);
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType, workspace)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBuffer(Double[], DataType, MemoryWorkspace)"/>
		''' </summary>
		Public Shared Function createTypedBuffer(ByVal data() As Single, ByVal dataType As DataType, ByVal workspace As MemoryWorkspace) As DataBuffer
			'val buffer = workspace == null ? DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.length, false) : DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.length, false, workspace);
			Dim buffer As DataBuffer = getDataBuffer(data.Length, dataType, workspace)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		'''  Create am uninitialized  buffer based on the data of the underlying java array and specified type. </summary>
		''' <param name="data"> underlying java array </param>
		''' <param name="dataType"> specified type. </param>
		''' <returns> the created buffer. </returns>
		Public Shared Function createTypedBufferDetached(ByVal data() As Double, ByVal dataType As DataType) As DataBuffer
			Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.Length, False)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBufferDetached(Double[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBufferDetached(ByVal data() As Single, ByVal dataType As DataType) As DataBuffer
			Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.Length, False)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBufferDetached(Double[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBufferDetached(ByVal data() As Integer, ByVal dataType As DataType) As DataBuffer
			Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.Length, False)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBufferDetached(Double[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBufferDetached(ByVal data() As Long, ByVal dataType As DataType) As DataBuffer
			Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.Length, False)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBufferDetached(Double[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBufferDetached(ByVal data() As Short, ByVal dataType As DataType) As DataBuffer
			Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.Length, False)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBufferDetached(Double[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBufferDetached(ByVal data() As SByte, ByVal dataType As DataType) As DataBuffer
			Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.Length, False)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' See <seealso cref="createTypedBufferDetached(Double[], DataType)"/>
		''' </summary>
		Public Shared Function createTypedBufferDetached(ByVal data() As Boolean, ByVal dataType As DataType) As DataBuffer
			Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.create(dataType, data.Length, False)
			buffer.setData(data)
			Return buffer
		End Function

		''' <summary>
		''' Set the factory instance for INDArray creation. </summary>
		''' <param name="factory"> new INDArray factory </param>
		Public Shared WriteOnly Property Factory As NDArrayFactory
			Set(ByVal factory As NDArrayFactory)
				INSTANCE = factory
			End Set
		End Property

		''' <summary>
		''' Returns the ordering of the ndarrays
		''' </summary>
		''' <returns> the ordering of the ndarrays </returns>
		Public Shared Function order() As Char?
			Return factory().order()
		End Function

		''' <summary>
		''' Returns the data opType used for the runtime
		''' </summary>
		''' <returns> the datatype used for the runtime </returns>
		Public Shared Function dataType() As DataType
			Return DataTypeUtil.DtypeFromContext
		End Function

		''' <summary>
		''' DEPRECATED - use <seealso cref="setDefaultDataTypes(DataType, DataType)"/>
		''' This method sets dataType for the current JVM. </summary>
		''' <param name="dtype"> Data type to set </param>
		''' @deprecated use <seealso cref="setDefaultDataTypes(DataType, DataType)"/>. Equivalent to {@code setDefaultDataTypes(dtype, (dtype.isFPType() ? dtype : defaultFloatingPointType()))} 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""setDefaultDataTypes(DataType, DataType)""/>. Equivalent to {@code setDefaultDataTypes(dtype, (dtype.isFPType() ? dtype : defaultFloatingPointType()))}") public static void setDataType(@NonNull DataType dtype)
		<Obsolete("use <seealso cref=""setDefaultDataTypes(DataType, DataType)""/>. Equivalent to {@code setDefaultDataTypes(dtype, (dtype.isFPType() ? dtype : defaultFloatingPointType()))}")>
		Public Shared WriteOnly Property DataType As DataType
			Set(ByVal dtype As DataType)
				setDefaultDataTypes(dtype, (If(dtype.isFPType(), dtype, defaultFloatingPointType())))
			End Set
		End Property

		''' <summary>
		''' Set the default data types.<br>
		''' The default data types are used for array creation methods where no data type is specified.<br>
		''' When the user explicitly provides a datatype (such as in Nd4j.ones(DataType.FLOAT, 1, 10)) these default values
		''' will not be used.<br>
		''' defaultType: used in methods such as Nd4j.ones(1,10) and Nd4j.zeros(10).<br>
		''' defaultFloatingPointType: used internally where a floating point array needs to be created, but no datatype is specified.
		''' defaultFloatingPointType must be one of DOUBLE, FLOAT or HALF
		''' </summary>
		''' <param name="defaultType">              Default datatype for new arrays (used when no type is specified). </param>
		''' <param name="defaultFloatingPointType"> Default datatype for new floating point arrays (used when no type is specified. Must be one of DOUBLE, FLOAT or HALF </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void setDefaultDataTypes(@NonNull DataType defaultType, @NonNull DataType defaultFloatingPointType)
		Public Shared Sub setDefaultDataTypes(ByVal defaultType As DataType, ByVal defaultFloatingPointType As DataType)
			Preconditions.checkArgument(defaultFloatingPointType.isFPType(), "Invalid default floating point type: %s is not a floating point type", defaultFloatingPointType)
			DataTypeUtil.setDTypeForContext(defaultType)
			Nd4j.defaultFloatingPointDataType.set(defaultFloatingPointType)
		End Sub

		''' <summary>
		''' Retrieve the Nd4J backend. </summary>
		''' <returns> the Nd4J backend. </returns>
		Public Shared ReadOnly Property Backend As Nd4jBackend
			Get
				Return backend_Conflict
			End Get
		End Property

		''' <summary>
		''' Retrieve the BLAS wrapper. </summary>
		''' <returns> the BLAS wrapper. </returns>
		Public Shared ReadOnly Property BlasWrapper As BlasWrapper
			Get
				Return BLAS_WRAPPER_INSTANCE
			End Get
		End Property

		''' <summary>
		''' Sort an ndarray along a particular dimension.<br>
		''' Note that the input array is modified in-place.
		''' </summary>
		''' <param name="ndarray">   the ndarray to sort </param>
		''' <param name="dimension"> the dimension to sort </param>
		''' <returns> the indices and the sorted ndarray (the original array, modified in-place) </returns>
		Public Shared Function sortWithIndices(ByVal ndarray As INDArray, ByVal dimension As Integer, ByVal ascending As Boolean) As INDArray()
			Dim indices As INDArray = Nd4j.create(ndarray.shape())
			Dim ret(1) As INDArray

			Dim nV As Long = ndarray.vectorsAlongDimension(dimension)
			For i As Integer = 0 To nV - 1
				Dim vec As INDArray = ndarray.vectorAlongDimension(i, dimension)
				Dim indexVector As INDArray = indices.vectorAlongDimension(i, dimension)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final System.Nullable<Double>[] data = new System.Nullable<Double>[(int) vec.length()];
				Dim data(CInt(vec.length()) - 1) As Double?
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final System.Nullable<Double>[] index = new System.Nullable<Double>[(int) vec.length()];
				Dim index(CInt(vec.length()) - 1) As Double?

				For j As Integer = 0 To vec.length() - 1
					data(j) = vec.getDouble(j)
					index(j) = CDbl(j)
				Next j

	'            
	'             * Inject a comparator that sorts indices relative to
	'             * the actual values in the data.
	'             * This allows us to retain the indices
	'             * and how they were rearranged.
	'             
				Array.Sort(index, New ComparatorAnonymousInnerClass(data))

				If ascending Then
					Dim j As Integer = 0
					Do While j < vec.length()
						vec.putScalar(j, data(CInt(Math.Truncate(index(j).Value))))
						indexVector.putScalar(j, index(j))
						j += 1
					Loop
				Else
					Dim count As Integer = data.Length - 1
					Dim j As Integer = 0
					Do While j < vec.length()
						Dim currCount2 As Integer = count
						count -= 1
						vec.putScalar(j, data(CInt(Math.Truncate(index(currCount2).Value))))
						indexVector.putScalar(j, index(currCount2))
						j += 1
					Loop
				End If


			Next i

			ret(0) = indices
			ret(1) = ndarray

			Return ret
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Double)

			private Double?() data

			public ComparatorAnonymousInnerClass(Double?() data)
			If True Then
				Me.data = data
			End If

			public Integer compare(Double? o1, Double? o2)
			If True Then
				Dim o As Integer = CInt(Math.Truncate(o1.doubleValue()))
				Dim oo2 As Integer = CInt(Math.Truncate(o2.doubleValue()))
				Return data(o).CompareTo(data(oo2))
			End If
		End Class

		''' <summary>
		''' Sort all elements of an array.
		''' 
		''' sorts all elements of an array. For multi dimansional arrays the result depends on the array ordering]
		''' 
		''' Nd4j.factory().setOrder('f');
		''' INDArray x = Nd4j.arange(4).reshape(2,2);
		''' Nd4j.sort(x, true);
		''' gives: [[         0,    2.0000], [    1.0000,    3.0000]]
		''' 
		''' The same ode with .setOrder('c')
		''' [[         0,    1.0000], [    2.0000,    3.0000]]
		''' </summary>
		''' <param name="ndarray"> array to sort </param>
		''' <param name="ascending"> true for ascending, false for descending </param>
		''' <returns> the sorted ndarray </returns>
		public static INDArray sort(INDArray ndarray, Boolean ascending)
		If True Then
			Return NDArrayFactory.sort(ndarray, Not ascending)
		End If

		''' <summary>
		''' Sort an ndarray along a particular dimension<br>
		''' Note that the input array is modified in-place.
		''' </summary>
		''' <param name="ndarray">   the ndarray to sort </param>
		''' <param name="dimension"> the dimension to sort </param>
		''' <returns> the sorted ndarray </returns>
		public static INDArray sort(INDArray ndarray, Integer dimension, Boolean ascending)
		If True Then
			Return NDArrayFactory.sort(ndarray, Not ascending, dimension)
		End If

		''' <summary>
		'''Sort (shuffle) the rows of a 2d array according to the value at a specified column.
		''' Other than the order of the rows, each row is unmodified. Copy operation: original
		''' INDArray is unmodified<br>
		''' So if sorting the following on values of column 2 (ascending):<br>
		''' [a b 2]<br>
		''' [c d 0]<br>
		''' [e f -3]<br>
		''' Then output is<br>
		''' [e f -3]<br>
		''' [c d 0]<br>
		''' [a b 2]<br> </summary>
		''' <param name="in"> 2d array to sort </param>
		''' <param name="colIdx"> The column to sort on </param>
		''' <param name="ascending"> true if smallest-to-largest; false if largest-to-smallest </param>
		''' <returns> the sorted ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("Duplicates") public static INDArray sortRows(final INDArray in, final int colIdx, final boolean ascending)
		public static INDArray sortRows(final INDArray [in], final Integer colIdx, final Boolean ascending)
		If True Then
			If [in].rank() <> 2 Then
				Throw New System.ArgumentException("Cannot sort rows on non-2d matrix")
			End If
			If colIdx < 0 OrElse colIdx >= [in].columns() Then
				Throw New System.ArgumentException("Cannot sort on values in column " & colIdx & ", nCols=" & [in].columns())
			End If

			Dim [out] As INDArray = Nd4j.create([in].dataType(), [in].shape())
			Dim nRows As Integer = [in].rows()
			Dim list As New List(Of Integer)(nRows)
			For i As Integer = 0 To nRows - 1
				list.Add(i)
			Next i
			list.Sort(New ComparatorAnonymousInnerClass2(Me))
			For i As Integer = 0 To nRows - 1
				[out].putRow(i, [in].getRow(list(i)))
			Next i
			Return [out]
		End If

		''' <summary>
		'''Sort (shuffle) the columns of a 2d array according to the value at a specified row.
		''' Other than the order of the columns, each column is unmodified. Copy operation: original
		''' INDArray is unmodified<br>
		''' So if sorting the following on values of row 1 (ascending):<br>
		''' [a b c]<br>
		''' [1 -1 0]<br>
		''' [d e f]<br>
		''' Then output is<br>
		''' [b c a]<br>
		''' [-1 0 1]<br>
		''' [e f d]<br> </summary>
		''' <param name="in"> 2d array to sort </param>
		''' <param name="rowIdx"> The row to sort on </param>
		''' <param name="ascending"> true if smallest-to-largest; false if largest-to-smallest </param>
		''' <returns> the sorted array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("Duplicates") public static INDArray sortColumns(final INDArray in, final int rowIdx, final boolean ascending)
		public static INDArray sortColumns(final INDArray [in], final Integer rowIdx, final Boolean ascending)
		If True Then
			If [in].rank() <> 2 Then
				Throw New System.ArgumentException("Cannot sort columns on non-2d matrix")
			End If
			If rowIdx < 0 OrElse rowIdx >= [in].rows() Then
				Throw New System.ArgumentException("Cannot sort on values in row " & rowIdx & ", nRows=" & [in].rows())
			End If

			Dim [out] As INDArray = Nd4j.create([in].shape())
			Dim nCols As Integer = [in].columns()
			Dim list As New List(Of Integer)(nCols)
			For i As Integer = 0 To nCols - 1
				list.Add(i)
			Next i
			list.Sort(New ComparatorAnonymousInnerClass3(Me))
			For i As Integer = 0 To nCols - 1
				[out].putColumn(i, [in].getColumn(list(i)))
			Next i
			Return [out]
		End If

		''' <summary>
		''' Create an n x (shape)
		''' ndarray where the ndarray is repeated num times
		''' </summary>
		''' <param name="n">   the ndarray to replicate </param>
		''' <param name="num"> the number of copies to repeat </param>
		''' <returns> the repeated ndarray </returns>
		public static INDArray repeat(INDArray n, Integer num)
		If True Then
			Dim list As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To num - 1
				list.Add(n.dup())
			Next i
			Dim nShape() As Long = n.shape()
			Dim shape() As Long = If(n.isColumnVector(), New Long() {n.shape()(0)}, nShape)
			Dim retShape() As Long = Longs.concat(New Long() {num}, shape)
			Return Nd4j.create(list, retShape)
		End If

		''' <summary>
		''' Generate a linearly spaced vector
		''' </summary>
		''' <param name="lower"> upper bound </param>
		''' <param name="num">   number of items in returned vector </param>
		''' <param name="step">  the step (incompatible with <b>upper</b>) </param>
		''' <returns> the linearly spaced vector </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray linspace(@NonNull DataType dtype, long lower, long num, long step)
		public static INDArray linspace( DataType dtype_Conflict, Long lower, Long num, Long [step])
		If True Then
			' for now we'll temporarily keep original impl
			If num = 1 Then
				Return Nd4j.scalar(dtype_Conflict, lower)
			End If

			If dtype_Conflict.isIntType() Then
				Dim upper As Long = lower + num * [step]
				Return linspaceWithCustomOpByRange(lower, upper, num, [step], dtype_Conflict)
			ElseIf dtype_Conflict.isFPType() Then
				Return Nd4j.Executioner.exec(New Linspace(CDbl(lower), num, CDbl([step]), dtype_Conflict))
			Else
				Throw New System.InvalidOperationException("Illegal data type for linspace: " & dtype_Conflict.ToString())
			End If
		End If

		''' <summary>
		''' Generate a linearly spaced vector with default data type
		''' </summary>
		''' <param name="lower"> lower bound </param>
		''' <param name="upper"> upper bound </param>
		''' <param name="num">   number of items in returned vector </param>
		''' <returns> the linearly spaced vector </returns>
		public static INDArray linspace(Long lower, Long upper, Long num)
		If True Then
			Return linspace(lower, upper, num, Nd4j.dataType())
		End If

		''' <summary>
		''' Generate a linearly spaced vector
		''' </summary>
		''' <param name="lower"> lower bound </param>
		''' <param name="upper"> upper bound </param>
		''' <param name="num">   number of items in returned vector </param>
		''' <returns> the linearly spaced vector </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray linspace(long lower, long upper, long num, @NonNull DataType dtype)
		public static INDArray linspace(Long lower, Long upper, Long num, DataType dtype_Conflict)
		If True Then
			' for now we'll temporarily keep original impl
			If lower = upper AndAlso num = 1 Then
				Return Nd4j.scalar(dtype_Conflict, lower)
			End If
			If num = 1 Then
				Return Nd4j.scalar(dtype_Conflict, lower)
			End If
			If dtype_Conflict.isIntType() Then
				Return linspaceWithCustomOp(lower, upper, CInt(Math.Truncate(num)), dtype_Conflict)
			ElseIf dtype_Conflict.isFPType() Then
				Return linspace(CDbl(lower), CDbl(upper), CInt(Math.Truncate(num)), dtype_Conflict)
			Else
				Throw New System.InvalidOperationException("Illegal data type for linspace: " & dtype_Conflict.ToString())
			End If
		End If

		''' <summary>
		''' Generate a linearly spaced 1d vector of the specified datatype
		''' </summary>
		''' <param name="lower"> lower bound </param>
		''' <param name="step"> step between items </param>
		''' <param name="num">   number of resulting items </param>
		''' <returns> the linearly spaced vector </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray linspace(@NonNull DataType dataType, double lower, double step, long num)
		public static INDArray linspace( DataType dataType, Double lower, Double [step], Long num)
		If True Then
			Preconditions.checkState(dataType.isFPType(), "Datatype must be a floating point type for linspace, got %s", dataType)
			If num = 1 Then
				Return Nd4j.scalar(dataType, lower)
			End If
			Return Nd4j.Executioner.exec(New Linspace(lower, num, [step], dataType))
		End If

		''' <summary>
		''' Generate a linearly spaced 1d vector of the specified datatype
		''' </summary>
		''' <param name="lower"> lower bound </param>
		''' <param name="upper"> upper bound </param>
		''' <param name="num">   number of resulting items </param>
		''' <returns> the linearly spaced vector </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray linspace(double lower, double upper, long num, @NonNull DataType dataType)
		public static INDArray linspace(Double lower, Double upper, Long num, DataType dataType)
		If True Then
			Preconditions.checkState(dataType.isFPType(), "Datatype must be a floating point type for linspace, got %s", dataType)
			If num = 1 Then
				Return Nd4j.scalar(dataType, lower)
			End If
			Return Nd4j.Executioner.exec(New Linspace(lower, upper, num, dataType))
		End If

		private static INDArray linspaceWithCustomOp(Long lower, Long upper, Integer num, DataType dataType)
		If True Then
			If num = 1 Then
				Return Nd4j.scalar(dataType, lower)
			End If

			Dim result As INDArray = Nd4j.createUninitialized(dataType, New Long() {num}, Nd4j.order())

			Dim op As val = DynamicCustomOp.builder("lin_space").addInputs(Nd4j.scalar(lower), Nd4j.scalar(upper), Nd4j.scalar(num)).addOutputs(result).build()

			Nd4j.Executioner.execAndReturn(op)
			Return result
		End If

		private static INDArray linspaceWithCustomOpByRange(Long lower, Long upper, Long num, Long [step], DataType dataType)
		If True Then
			If num = 1 Then
				Return Nd4j.scalar(dataType, lower)
			End If

			Dim result As INDArray = Nd4j.createUninitialized(dataType, New Long() {num}, Nd4j.order())

			Dim op As val = DynamicCustomOp.builder("range").addInputs(Nd4j.scalar(lower), Nd4j.scalar(upper), Nd4j.scalar([step])).addOutputs(result).build()

			Nd4j.Executioner.execAndReturn(op)
			Return result
		End If

		''' <summary>
		''' Meshgrid op. Returns a pair of arrays where values are broadcast on a 2d grid.<br>
		''' For example, if x = [1,2,3,4] and y = [5,6,7], then:<br>
		''' out[0] =<br>
		''' [1,2,3,4]<br>
		''' [1,2,3,4]<br>
		''' [1,2,3,4]<br>
		''' <br>
		''' out[1] =<br>
		''' [5,5,5,5]<br>
		''' [6,6,6,6]<br>
		''' [7,7,7,7]<br>
		''' <br>
		''' </summary>
		''' <param name="x"> X array input </param>
		''' <param name="y"> Y array input </param>
		''' <returns> INDArray[] of length 2, shape [y.length, x.length] </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray[] meshgrid(@NonNull INDArray x, @NonNull INDArray y)
		public static INDArray() meshgrid( INDArray x, INDArray y)
		If True Then
			Preconditions.checkArgument(x.isVectorOrScalar(), "X must be a vector")
			Preconditions.checkArgument(y.isVectorOrScalar(), "Y must be a vector")
			If y.dataType() <> x.dataType() Then
				y = y.castTo(x.dataType())
			End If

			Dim xOut As INDArray = Nd4j.createUninitialized(x.dataType(), y.length(), ChrW(x.length()))
			Dim yOut As INDArray = Nd4j.createUninitialized(x.dataType(), y.length(), ChrW(x.length()))

			Dim op As CustomOp = DynamicCustomOp.builder("meshgrid").addInputs(x, y).addOutputs(xOut, yOut).build()
			Nd4j.Executioner.execAndReturn(op)

			Return New INDArray(){xOut, yOut}
		End If


		''' <summary>
		''' Create a long row vector of all of the given ndarrays </summary>
		''' <param name="matrices"> the matrices to create the flattened ndarray for </param>
		''' <returns> the flattened representation of
		''' these ndarrays </returns>
		public static INDArray toFlattened(ICollection(Of INDArray) matrices)
		If True Then
			Return INSTANCE.toFlattened(matrices)
		End If

		''' <summary>
		''' Create a long row vector of all of the given ndarrays </summary>
		''' <param name="order"> the order in which to flatten the matrices </param>
		''' <param name="matrices"> the matrices to create the flattened ndarray for </param>
		''' <returns> the flattened representation of
		''' these ndarrays </returns>
		public static INDArray toFlattened(Char order, ICollection(Of INDArray) matrices)
		If True Then
			Return INSTANCE.toFlattened(order, matrices)
		End If

		''' <summary>
		''' Create a long row vector of all of the given ndarrays </summary>
		''' <param name="matrices"> the matrices to create the flattened ndarray for </param>
		''' <returns> the flattened representation of
		''' these ndarrays </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray toFlattened(@NonNull INDArray... matrices)
		public static INDArray toFlattened( INDArray... matrices)
		If True Then
			Return INSTANCE.toFlattened(matrices)
		End If

		''' <summary>
		''' Create a long row vector of all of the given ndarrays/ </summary>
		''' <param name="order"> order in which to flatten ndarrays </param>
		''' <param name="matrices"> the matrices to create the flattened ndarray for
		''' </param>
		''' <returns> the flattened representation of
		''' these ndarrays </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray toFlattened(char order, @NonNull INDArray... matrices)
		public static INDArray toFlattened(Char order, INDArray... matrices)
		If True Then
			Return INSTANCE.toFlattened(order, matrices)
		End If

		''' <summary>
		''' Create the identity ndarray
		''' </summary>
		''' <param name="n"> the number for the identity </param>
		''' <returns> the identity array </returns>
		public static INDArray eye(Long n)
		If True Then
			Return INSTANCE.eye(n)
		End If

		''' <summary>
		''' Rotate a matrix 90 degrees
		''' </summary>
		''' <param name="toRotate"> the matrix to rotate </param>
		public static void rot90(INDArray toRotate)
		If True Then
			INSTANCE.rot90(toRotate)
		End If

		''' <summary>
		''' Write NDArray to a text file
		''' </summary>
		''' <param name="filePath"> path to write to </param>
		''' <param name="split">    the split separator, defaults to "," </param>
		''' <param name="precision"> digits after the decimal point </param>
		''' @deprecated Precision is no longer used. Split is no longer used.
		''' Defaults to scientific notation with 18 digits after the decimal
		''' Use <seealso cref="writeTxt(INDArray, String)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") public static void writeTxt(INDArray write, String filePath, String split, int precision)
		public static void writeTxt(INDArray write, String filePath, String split, Integer precision)
		If True Then
			writeTxt(write,filePath)
		End If

		''' <summary>
		''' Write NDArray to a text file
		''' </summary>
		''' <param name="write"> array to write </param>
		''' <param name="filePath"> path to write to </param>
		''' <param name="precision"> Precision is no longer used.
		''' @deprecated
		''' Defaults to scientific notation with 18 digits after the decimal
		''' Use <seealso cref="writeTxt(INDArray, String)"/> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") public static void writeTxt(INDArray write, String filePath, int precision)
		public static void writeTxt(INDArray write, String filePath, Integer precision)
		If True Then
			writeTxt(write, filePath)
		End If

		''' <summary>
		''' Write NDArray to a text file
		''' </summary>
		''' <param name="write"> array to write </param>
		''' <param name="filePath"> path to write to </param>
		''' <param name="split"> the split separator, defaults to "," </param>
		''' @deprecated custom col and higher dimension separators are no longer supported; uses ","
		''' Use <seealso cref="writeTxt(INDArray, String)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") public static void writeTxt(INDArray write, String filePath, String split)
		public static void writeTxt(INDArray write, String filePath, String split)
		If True Then
			writeTxt(write,filePath)
		End If

		''' <summary>
		''' Write NDArray to a text file
		''' </summary>
		''' <param name="write"> Array to write </param>
		''' <param name="filePath"> path to write to </param>
		public static void writeTxt(INDArray write, String filePath)
		If True Then
			Try
				Dim toWrite As String = writeStringForArray(write)
				FileUtils.writeStringToFile(New File(filePath), toWrite, DirectCast(Nothing, String), False)
			Catch e As IOException
				Throw New Exception("Error writing output", e)
			End Try
		End If

		private static String writeStringForArray(INDArray write)
		If True Then
			If write.isView() OrElse Not Shape.hasDefaultStridesForShape(write) Then
				write = write.dup()
			End If

			Dim format As String = "0.000000000000000000E0"

			Return "{" & vbLf & """filefrom"": ""dl4j""," & vbLf & """ordering"": """ & write.ordering() & """," & vbLf & """shape"":" & vbTab & java.util.Arrays.toString(write.shape()) & "," & vbLf & """data"":" & vbLf & (New NDArrayStrings(",", format)).format(write, False) & vbLf & "}" & vbLf
		End If



		''' <summary>
		'''Y
		''' Write an ndarray to a writer </summary>
		''' <param name="writer"> the writer to write to </param>
		''' <param name="write"> the ndarray to write </param>
		public static void write(Stream writer, INDArray write) throws IOException
		If True Then
			Dim stream As New DataOutputStream(writer)
			write(write, stream)
			stream.close()
		End If

		''' <summary>
		''' Convert an ndarray to a byte array </summary>
		''' <param name="arr"> the array to convert </param>
		''' <returns> the converted byte array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static byte[] toByteArray(@NonNull INDArray arr) throws IOException
		public static SByte() toByteArray( INDArray arr) throws IOException
		If True Then
			If arr.length() * arr.data().getElementSize() > Integer.MaxValue Then
				Throw New ND4JIllegalStateException("")
			End If

			Dim bos As New MemoryStream(CInt(Math.Truncate(arr.length() * arr.data().getElementSize())))
			Dim dos As New DataOutputStream(bos)
			write(arr, dos)
			Return bos.toByteArray()
		End If

		''' <summary>
		''' Read an ndarray from a byte array </summary>
		''' <param name="arr"> the array to read from </param>
		''' <returns> the deserialized ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray fromByteArray(@NonNull byte[] arr)
		public static INDArray fromByteArray( SByte() arr)
		If True Then
			Dim bis As New MemoryStream(arr)
			Return read(bis)
		End If

		''' <summary>
		''' Read line via input streams
		''' </summary>
		''' <param name="filePath"> the input stream ndarray </param>
		''' <param name="split">    the split separator </param>
		''' <returns> the read txt method </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray readNumpy(@NonNull InputStream filePath, @NonNull String split) throws IOException
		public static INDArray readNumpy( Stream filePath, String split) throws IOException
		If True Then
			Return readNumpy(DataType.FLOAT, filePath, split, StandardCharsets.UTF_8)
		End If

		''' <summary>
		''' Read array from input stream.
		''' </summary>
		''' <param name="dataType"> datatype of array </param>
		''' <param name="filePath"> the input stream </param>
		''' <param name="split">    the split separator </param>
		''' <param name="charset"> the  charset </param>
		''' <returns> the deserialized array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static INDArray readNumpy(@NonNull DataType dataType, @NonNull InputStream filePath, @NonNull String split, @NonNull Charset charset) throws IOException
		public static INDArray readNumpy( DataType dataType, Stream filePath, String split, Charset charset) throws IOException
		If True Then
			Dim reader As New StreamReader(filePath, charset)
			Dim line As String
			Dim data2 As IList(Of Single()) = New List(Of Single())()
			Dim numColumns As Integer = -1
			Dim ret As INDArray
			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				Dim data() As String = line.Trim().Split(split, True)
				If numColumns < 0 Then
					numColumns = data.Length
				Else
					Preconditions.checkState(data.Length = numColumns, "Data has inconsistent number of columns: data length %s, numColumns %s", data.Length, numColumns)
				End If
				data2.Add(readSplit(data))
					line = reader.ReadLine()
			Loop
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim fArr[][] As Single = new Single[data2.Count][0]
			Dim fArr()() As Single = RectangularArrays.RectangularSingleArray(data2.Count, 0)
			For i As Integer = 0 To data2.Count - 1
				fArr(i) = data2(i)
			Next i
			ret = Nd4j.createFromArray(fArr).castTo(dataType)
			Return ret
		End If

		private static Single() readSplit(String() split)
		If True Then
			Dim ret(split.length - 1) As Single
			For i As Integer = 0 To split.length - 1
				Try
					ret(i) = Single.Parse(split(i))
				Catch e As System.FormatException
					If split(i).equalsIgnoreCase("inf") Then
						ret(i) = Single.PositiveInfinity
					ElseIf split(i).equalsIgnoreCase("-inf") Then
						ret(i) = Single.NegativeInfinity
					ElseIf split(i).equalsIgnoreCase("nan") Then
						ret(i) = Float.NaN
					Else
						Throw New Exception(e)
					End If

				End Try
			Next i
			Return ret
		End If

		''' <summary>
		''' Read line via input streams
		''' </summary>
		''' <param name="filePath"> the input stream ndarray </param>
		''' <param name="split">    the split separator </param>
		''' <returns> the read txt method </returns>
		public static INDArray readNumpy(String filePath, String split) throws IOException
		If True Then
			Return readNumpy(DataType.FLOAT, filePath, split)
		End If

		''' <summary>
		''' Read array via input stream.
		''' 
		''' See <seealso cref="readNumpy(DataType, InputStream, String , Charset)"/> using standard UTF-8 encoding
		''' </summary>
		public static INDArray readNumpy(DataType dataType, String filePath, String split) throws IOException
		If True Then
			Using [is] As Stream = New FileStream(filePath, FileMode.Open, FileAccess.Read)
				Return readNumpy(dataType, [is], split, StandardCharsets.UTF_8)
			End Using
		End If

		''' <summary>
		''' Read line via input streams
		''' </summary>
		''' <param name="filePath"> the input stream ndarray </param>
		''' <returns> the read txt method </returns>
		public static INDArray readNumpy(String filePath) throws IOException
		If True Then
			Return readNumpy(DataType.FLOAT, filePath)
		End If

		''' <summary>
		''' Read array.<br>
		''' 
		''' See <seealso cref="readNumpy(DataType, InputStream, String , Charset)"/> with default split and UTF-8 encoding.
		''' </summary>
		public static INDArray readNumpy(DataType dataType, String filePath) throws IOException
		If True Then
			Return readNumpy(dataType, filePath, " ")
		End If

		''' <summary>
		''' Raad an ndarray from an input stream
		''' 
		''' See <seealso cref="read(DataInputStream)"/>
		''' </summary>
		public static INDArray read(Stream reader)
		If True Then
			Return read(New DataInputStream(reader))
		End If

		''' <summary>
		''' Read line via input streams
		''' </summary>
		''' <param name="ndarray"> the input stream ndarray </param>
		''' <returns> NDArray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static INDArray readTxtString(InputStream ndarray)
		public static INDArray readTxtString(Stream ndarray)
		If True Then
			Dim sep As String = ","
	'        
	'         We could dump an ndarray to a file with the tostring (since that is valid json) and use put/get to parse it as json
	'         But here we leverage our information of the tostring method to be more efficient
	'         With our current toString format we use tads along dimension (rank-1,rank-2) to write to the array in two dimensional chunks at a time.
	'         This is more efficient than setting each value at a time with putScalar.
	'         This also means we can read the file one line at a time instead of loading the whole thing into memory
	'        
			Dim newArr As INDArray = Nothing
			Dim reader As New StreamReader(ndarray)
			Dim it As LineIterator = IOUtils.lineIterator(reader)
			Dim format As DecimalFormat = CType(NumberFormat.getInstance(Locale.US), DecimalFormat)
			format.setParseBigDecimal(True)
			Try
				Dim lineNum As Integer = 0
				Dim tensorNum As Integer = 0
				Dim theOrder As Char = "c"c
				Dim rank As Integer = 0
				Dim theShape() As Long = Nothing
				Dim subsetArr() As Double = Nothing
				Do While it.hasNext()
					Dim line As String = it.nextLine()
					lineNum += 1
					line = line.replaceAll("\s", "")
					If line.Equals("") OrElse line.Equals("}") Then
						Continue Do
					End If
					' is it from dl4j?
					If lineNum = 2 Then
						Dim lineArr() As String = line.Split(":", True)
						Dim fileSource As String = lineArr(1).replaceAll("\W", "")
						If Not fileSource.Equals("dl4j") Then
							Throw New System.ArgumentException("Only files written out from Nd4j.writeTxT/writeTxtString can be read with the readTxt/readTxtString methods")
						End If
					End If
					' parse ordering
					If lineNum = 3 Then
						Dim lineArr() As String = line.Split(":", True)
						theOrder = lineArr(1).replaceAll("\W", "").Chars(0)
						Continue Do
					End If
					' parse shape
					If lineNum = 4 Then
						Dim shapeString As String = line.Split(":", True)(1).replace("[", "").replace("],", "")
						If shapeString.Length = 0 Then
							newArr = Nd4j.scalar(Nd4j.defaultFloatingPointType(), 0)
						Else
							Dim shapeArr() As String = shapeString.Split(",", True)
							rank = shapeArr.Length
							theShape = New Long(rank - 1){}
							For i As Integer = 0 To rank - 1
								theShape(i) = Integer.Parse(shapeArr(i))
							Next i
							If theOrder = "f"c AndAlso theShape(rank-1) = 1 Then
								'Hack fix for tad issue with 'f' order and rank-1 dim shape == 1
								newArr = Nd4j.create(Nd4j.defaultFloatingPointType(), theShape, "c"c)
							Else
								newArr = Nd4j.create(Nd4j.defaultFloatingPointType(), theShape, theOrder)
							End If
							subsetArr = New Double(CInt(theShape(rank - 1)) - 1){}
						End If
						Continue Do
					End If
					'parse data
					If lineNum > 5 Then
						Dim entries() As String = line.Replace("\],", "").replaceAll("]", "").replaceAll("\[", "").Split(sep, True)
						If rank = 0 Then
							Try
								'noinspection ConstantConditions
								newArr.addi((format.parse(entries(0))).doubleValue())
							Catch e As ParseException
								log.error("",e)
							End Try
						Else
							Preconditions.checkState(entries.Length = theShape(rank-1), "Invalid number of entries - format does not match expected shape." & "Expected %s values per line, got %s at line %s", theShape(rank-1), entries.Length, lineNum)
							Dim i As Integer = 0
							Do While i < theShape(rank - 1)
								Try
									Dim number As Decimal = CDec(format.parse(entries(i)))
									subsetArr(i) = number.doubleValue()
								Catch e As ParseException
									log.error("",e)
								End Try
								i += 1
							Loop
							Dim subTensor As INDArray = Nd4j.create(subsetArr, New Long(){subsetArr.Length}, Nd4j.defaultFloatingPointType())
							newArr.tensorAlongDimension(tensorNum, rank - 1).addi(subTensor)
							tensorNum += 1
						End If
					End If
				Loop
				'Hack fix for tad issue with 'f' order and rank-1 dim shape == 1
				If theOrder = "f"c AndAlso rank > 1 AndAlso theShape(rank-1) = 1 Then
					newArr = newArr.dup("f"c)
				End If

			Finally
				LineIterator.closeQuietly(it)
			End Try

			If newArr Is Nothing Then
				Throw New System.InvalidOperationException("Cannot parse file: file does not appear to represent a text serialized INDArray file")
			End If

			Return newArr
		End If

		''' <summary>
		''' Read line via input streams
		''' </summary>
		''' <param name="filePath"> the input stream ndarray </param>
		''' <returns> NDArray </returns>
		public static INDArray readTxt(String filePath)
		If True Then
			Dim file As New File(filePath)
			Dim [is] As Stream = Nothing
			Try
				[is] = New FileStream(file, FileMode.Open, FileAccess.Read)
				Return readTxtString([is])
			Catch e As FileNotFoundException
				Throw New Exception(e)
			Finally
				IOUtils.closeQuietly([is])
			End Try
		End If

		private static Integer() toIntArray(Integer length, DataBuffer buffer)
		If True Then
			Dim ret(length - 1) As Integer
			For i As Integer = 0 To length - 1
				ret(i) = buffer.getInt(i)
			Next i
			Return ret
		End If

		''' <summary>
		''' Create array based in data buffer and shape info,
		''' </summary>
		''' <param name="data"> Data buffer. </param>
		''' <param name="shapeInfo"> shape information. </param>
		''' <returns> new INDArray. </returns>
		public static INDArray createArrayFromShapeBuffer(DataBuffer data, DataBuffer shapeInfo)
		If True Then
			Dim jvmShapeInfo As val = shapeInfo.asLong()
			Dim dataType As val = ArrayOptionsHelper.dataType(jvmShapeInfo)
			Dim shape As val = Shape.shape(jvmShapeInfo)
			Dim strides As val = Shape.stridesOf(jvmShapeInfo)
			Dim order As val = Shape.order(jvmShapeInfo)
			Dim result As INDArray = Nd4j.create(data, shape, strides, 0, order, dataType)
			If TypeOf data Is CompressedDataBuffer Then
				result.markAsCompressed(True)
			End If

			Return result
		End If

		''' <summary>
		''' Create array based in data buffer and shape info,
		''' </summary>
		''' <param name="data"> data buffer. </param>
		''' <param name="shapeInfo"> shape information. </param>
		''' <returns> new INDArray. </returns>
		public static INDArray createArrayFromShapeBuffer(DataBuffer data, Pair(Of DataBuffer, Long()) shapeInfo)
		If True Then
			Dim rank As Integer = Shape.rank(shapeInfo.getFirst())
			' removed offset parameter that called a deprecated method which always returns 0.
			Dim result As INDArray = Nd4j.create(data, toIntArray(rank, Shape.shapeOf(shapeInfo.getFirst())), toIntArray(rank, Shape.stride(shapeInfo.getFirst())), 0, Shape.order(shapeInfo.getFirst()))
			If TypeOf data Is CompressedDataBuffer Then
				result.markAsCompressed(True)
			End If

			Return result
		End If

		''' <summary>
		''' Read in an ndarray from a data input stream
		''' </summary>
		''' <param name="dis"> the data input stream to read from </param>
		''' <returns> the ndarray </returns>
		public static INDArray read(DataInputStream dis)
		If True Then
			Dim headerShape As val = BaseDataBuffer.readHeader(dis)

			'noinspection UnnecessaryUnboxing
			Dim shapeInformation As var = Nd4j.createBufferDetached(New Long(){headerShape.getMiddle().longValue()}, headerShape.getRight())
			shapeInformation.read(dis, headerShape.getLeft(), headerShape.getMiddle(), headerShape.getThird())
			Dim type As DataType
			Dim data As DataBuffer = Nothing

			Dim headerData As val = BaseDataBuffer.readHeader(dis)
			Try
				' current version contains dtype in extras
				data = CompressedDataBuffer.readUnknown(dis, headerData.getFirst(), headerData.getMiddle(), headerData.getRight())
				ArrayOptionsHelper.dataType(shapeInformation.asLong())
			Catch e As ND4JUnknownDataTypeException
				' manually setting data type
				type = headerData.getRight()
				Dim extras As Long = ArrayOptionsHelper.setOptionBit(0L, type)
				shapeInformation.put(shapeInformation.length() - 3, extras)
			End Try

			Return createArrayFromShapeBuffer(data, shapeInformation)
		End If

		''' <summary>
		''' Write an ndarray to the specified outputstream
		''' </summary>
		''' <param name="arr">              the array to write </param>
		''' <param name="dataOutputStream"> the data output stream to write to </param>
		public static void write(INDArray arr, DataOutputStream dataOutputStream) throws IOException
		If True Then
			'BaseDataBuffer.write(...) doesn't know about strides etc, so dup (or equiv. strategy) is necessary here
			'Furthermore, because we only want to save the *actual* data for a view (not the full data), the shape info
			' (mainly strides, offset, element-wise stride) may be different in the duped array vs. the view array
			If arr.isView() Then
				arr = arr.dup()
			End If

			arr.shapeInfoDataBuffer().write(dataOutputStream)
			arr.data().write(dataOutputStream)
		End If

		''' <summary>
		''' Save an ndarray to the given file </summary>
		''' <param name="arr"> the array to save </param>
		''' <param name="saveTo"> the file to save to </param>
		public static void saveBinary(INDArray arr, File saveTo) throws IOException
		If True Then
			Dim bos As New BufferedOutputStream(New FileStream(saveTo, FileMode.Create, FileAccess.Write))
			Dim dos As New DataOutputStream(bos)
			Nd4j.write(arr, dos)
			dos.flush()
			dos.close()
			bos.close()
		End If

		''' <summary>
		''' Read a binary ndarray from the given file </summary>
		''' <param name="read"> the nd array to read </param>
		''' <returns> the loaded ndarray </returns>
		public static INDArray readBinary(File read) throws IOException
		If True Then
			Dim bis As New BufferedInputStream(New FileStream(read, FileMode.Open, FileAccess.Read))
			Dim dis As New DataInputStream(bis)
			Dim ret As INDArray = Nd4j.read(dis)
			dis.close()
			Return ret
		End If

		''' <summary>
		''' Clear nans from an ndarray
		''' </summary>
		''' <param name="arr"> the array to clear </param>
		public static void clearNans(INDArray arr)
		If True Then
			Executioner.exec(New ReplaceNans(arr, Nd4j.EPS_THRESHOLD))
		End If

		''' <summary>
		''' Reverses the passed in matrix such that m[0] becomes m[m.length - 1] etc
		''' </summary>
		''' <param name="reverse"> the matrix to reverse </param>
		''' <returns> the reversed matrix </returns>
		public static INDArray reverse(INDArray reverse)
		If True Then
			Return Nd4j.Executioner.exec(New Reverse(reverse))(0)
		End If

		''' <summary>
		''' Create a 1D array of evenly spaced values between {@code begin} (inclusive) and {@code end} (exclusive)
		''' with a step size.
		''' </summary>
		''' <param name="begin"> the begin of the range (inclusive) </param>
		''' <param name="end">   the end of the range (exclusive) </param>
		''' <param name="step"> spacing between values. Default value is 1. </param>
		''' <returns> the 1D range vector </returns>
		public static INDArray arange(Double begin, Double [end], Double [step])
		If True Then
			Return INSTANCE.arange(begin, [end], [step])
		End If

		''' <summary>
		''' Create a 1D array of evenly spaced values between {@code begin} (inclusive) and {@code end} (exclusive)
		''' with a step size of 1
		''' 
		''' See <seealso cref="arange(Double, Double, Double)"/> with step size 1.
		''' </summary>
		public static INDArray arange(Double begin, Double [end])
		If True Then
			Return INSTANCE.arange(begin, [end], 1)
		End If

		''' <summary>
		''' Create a 1D array of evenly spaced values between 0 (inclusive) and {@code end} (exclusive)
		''' with a step size of 1
		''' 
		''' See <seealso cref="arange(Double, Double, Double)"/> with begin = 0 and step size 1.
		''' </summary>
		public static INDArray arange(Double [end])
		If True Then
			Return arange(0, [end])
		End If

		''' <summary>
		''' Copy a to b
		''' </summary>
		''' <param name="a"> the origin matrix </param>
		''' <param name="b"> the destination matrix </param>
		public static void copy(INDArray a, INDArray b)
		If True Then
			INSTANCE.copy(a, b)
		End If

		''' <summary>
		''' Creates a new matrix where the values of the given vector are the diagonal values of
		''' the matrix if a vector is passed in, if a matrix is returns the kth diagonal
		''' in the matrix
		''' </summary>
		''' <param name="x"> the diagonal values </param>
		''' <returns> new matrix </returns>
		public static INDArray diag(INDArray x)
		If True Then
			Dim ret As INDArray
			If x.isVectorOrScalar() OrElse x.isRowVector() OrElse x.isColumnVector() Then
				ret = Nd4j.create(x.dataType(), x.length(), x.length())
				Nd4j.Executioner.execAndReturn(New Diag(x, ret))
			Else
				ret = Nd4j.createUninitialized(x.dataType(), Math.Min(x.size(0), x.size(1)))
				Nd4j.Executioner.execAndReturn(New DiagPart(x,ret))
			End If
			Return ret
		End If

		''' <summary>
		''' This method samples value from Source array to Target, with probabilites provided in Probs argument
		''' </summary>
		''' <param name="source"> source array. </param>
		''' <param name="probs"> array with probabilities. </param>
		''' <param name="target"> destination array. </param>
		''' <param name="rng"> Random number generator. </param>
		''' <returns> the destination (target) array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray choice(@NonNull INDArray source, @NonNull INDArray probs, @NonNull INDArray target, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray choice( INDArray source, INDArray probs, INDArray target, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			If source.length() <> probs.length() Then
				Throw New ND4JIllegalStateException("Nd4j.choice() requires lengths of Source and Probs to be equal")
			End If

			Return Nd4j.Executioner.exec(New Choice(source, probs, target), rng)
		End If

		' @see tag works well here.
		''' <summary>
		''' This method samples value from Source array to Target,the default random number generator.
		''' </summary>
		''' <seealso cref= #choice(INDArray, INDArray, INDArray, org.nd4j.linalg.api.rng.Random) </seealso>
		public static INDArray choice(INDArray source, INDArray probs, INDArray target)
		If True Then
			Return choice(source, probs, target, Nd4j.Random)
		End If

		' @see tag works well here.
		''' <summary>
		''' This method returns new INDArray instance, sampled from Source array with probabilities given in Probs.
		''' </summary>
		''' <param name="numSamples"> number of samples to take. (size of the new NDArray). </param>
		''' <seealso cref= #choice(INDArray, INDArray, int, org.nd4j.linalg.api.rng.Random) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray choice(INDArray source, INDArray probs, int numSamples, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray choice(INDArray source, INDArray probs, Integer numSamples, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			If numSamples < 1 Then
				Throw New ND4JIllegalStateException("Nd4j.choice() numSamples must be positive value")
			End If

			Return choice(source, probs, createUninitialized(source.dataType(), numSamples), rng)
		End If

		' @see tag works well here.
		''' <summary>
		''' This method returns new INDArray instance, sampled from Source array with probabilities given in Probs
		''' using the default random number generator.
		''' </summary>
		''' <seealso cref= #choice(INDArray, INDArray, int, org.nd4j.linalg.api.rng.Random) </seealso>
		public static INDArray choice(INDArray source, INDArray probs, Integer numSamples)
		If True Then
			Return choice(source, probs, numSamples, Nd4j.Random)
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray appendBias(@NonNull INDArray... vectors)
		public static INDArray appendBias( INDArray... vectors)
		If True Then
			Return INSTANCE.appendBias(vectors)
		End If

		'//////////////////// RANDOM ///////////////////////////////

		''' <summary>
		''' Create a random ndarray with values from a uniform distribution over (0, 1) with the given shape
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(@NonNull int... shape)
		public static INDArray rand( Integer... shape)
		If True Then
			Dim ret As INDArray = createUninitialized(shape, order()).castTo(Nd4j.defaultFloatingPointType()) 'INSTANCE.rand(shape, Nd4j.getRandom());
			Return rand(ret)
		End If

		''' <summary>
		''' See <seealso cref="rand(Integer[])"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(@NonNull long... shape)
		public static INDArray rand( Long... shape)
		If True Then
			Dim ret As INDArray = createUninitialized(shape, order()).castTo(Nd4j.defaultFloatingPointType()) 'INSTANCE.rand(shape, Nd4j.getRandom());
			Return rand(ret)
		End If

		''' <summary>
		''' Create a random ndarray with values from a uniform distribution over (0, 1) with the given shape and data type
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(@NonNull DataType dataType, @NonNull long... shape)
		public static INDArray rand( DataType dataType, Long... shape)
		If True Then
			Preconditions.checkArgument(dataType.isFPType(), "Can't create a random array of a non-floating point data type")
			Dim ret As INDArray = createUninitialized(dataType, shape, order()) 'INSTANCE.rand(shape, Nd4j.getRandom());
			Return rand(ret)
		End If

		''' <summary>
		''' Create a random ndarray with the given shape and array order
		''' 
		''' Values are sampled from a uniform distribution over (0, 1)
		''' </summary>
		''' <param name="order"> the order of the ndarray to return </param>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(char order, @NonNull int... shape)
		public static INDArray rand(Char order, Integer... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(shape, order).castTo(Nd4j.defaultFloatingPointType()) 'INSTANCE.rand(order, shape);
			Return rand(ret)
		End If

		''' @deprecated use {@link Nd4j#rand(org.nd4j.linalg.api.buffer.DataType, char, long...)) 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use {@link Nd4j#rand(org.nd4j.linalg.api.buffer.DataType, char, long...))") public static INDArray rand(@NonNull DataType dataType, int[] shape, char order)
		<Obsolete("use {@link Nd4j#rand(org.nd4j.linalg.api.buffer.DataType, char, long...))")>
		public static INDArray rand( DataType dataType, Integer() shape, Char order)
		If True Then
			Return rand(dataType, order, ArrayUtil.toLongArray(shape))
		End If

		''' @deprecated use <seealso cref="org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType, Char, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType, Char, Long...)""/>") public static INDArray rand(@NonNull DataType dataType, char order, @NonNull int... shape)
		<Obsolete("use <seealso cref=""org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.buffer.DataType, Char, Long...)""/>")>
		public static INDArray rand( DataType dataType, Char order, Integer... shape)
		If True Then
			Return rand(dataType, order, ArrayUtil.toLongArray(shape))
		End If

		''' <summary>
		''' Create a random ndarray with the given shape, data type, and array order
		''' 
		''' Values are sampled from a uniform distribution over (0, 1)
		''' </summary>
		''' <param name="order"> the order of the ndarray to return </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="dataType"> the data type of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(@NonNull DataType dataType, char order, @NonNull long... shape)
		public static INDArray rand( DataType dataType, Char order, Long... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(dataType, shape, order)
			Return rand(ret)
		End If


		''' <summary>
		''' Create a random ndarray with the given shape and data type
		''' 
		''' Values are sampled from a uniform distribution over (0, 1)
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="dataType"> the data type of the ndarray </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(@NonNull DataType dataType, @NonNull int... shape)
		public static INDArray rand( DataType dataType, Integer... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(dataType, ArrayUtil.toLongArray(shape), Nd4j.order())
			Return rand(ret)
		End If

		''' <summary>
		''' Create a random ndarray with values from a uniform distribution over (0, 1) with the given shape
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <returns> the random ndarray with the specified shape </returns>
	'    public static INDArray rand(int rows, int columns) {
	'        if (rows < 1 || columns < 1)
	'            throw new ND4JIllegalStateException("Number of rows and columns should be positive for new INDArray");
	'
	'        INDArray ret = createUninitialized(new int[] {rows, columns}, Nd4j.order());
	'        return rand(ret);
	'    }

		''' <summary>
		''' Create a random ndarray with the given shape and output order
		''' 
		''' Values are sampled from a uniform distribution over (0, 1)
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <returns> the random ndarray with the specified shape </returns>
	'    public static INDArray rand(char order, int rows, int columns) {
	'        if (rows < 1 || columns < 1)
	'            throw new ND4JIllegalStateException("Number of rows and columns should be positive for new INDArray");
	'
	'        INDArray ret = createUninitialized(new int[] {rows, columns}, order);//INSTANCE.rand(order, rows, columns);
	'        return rand(ret);
	'    }

		''' <summary>
		''' Create a random ndarray with values from a uniform distribution over (0, 1) with the given shape
		''' using given seed
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="seed">  the  seed to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(long seed, @NonNull long... shape)
		public static INDArray rand(Long seed, Long... shape)
		If True Then
			Dim ret As INDArray = createUninitialized(shape, Nd4j.order()) ';INSTANCE.rand(shape, seed);
			Return rand(ret, seed)
		End If

		''' @deprecated use <seealso cref="Nd4j.rand(Long, Long...)"/> 
		<Obsolete("use <seealso cref=""Nd4j.rand(Long, Long...)""/>")>
		public static INDArray rand(Integer() shape, Long seed)
		If True Then
			Return rand(seed, ArrayUtil.toLongArray(shape)).castTo(Nd4j.defaultFloatingPointType())
		End If


		''' <summary>
		''' Create a random ndarray with values from a uniform distribution over (0, 1) with the given shape
		''' using the given seed
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="seed">    the  seed to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
	'    public static INDArray rand(int rows, int columns, long seed) {
	'        INDArray ret = createUninitialized(new int[] {rows, columns}, Nd4j.order());
	'        return rand(ret, seed);
	'    }

		''' @deprecated use <seealso cref="Nd4j.rand(org.nd4j.linalg.api.rng.Random, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""Nd4j.rand(org.nd4j.linalg.api.rng.Random, Long...)""/>") public static INDArray rand(int[] shape, @NonNull org.nd4j.linalg.api.rng.Random rng)
		<Obsolete("use <seealso cref=""Nd4j.rand(org.nd4j.linalg.api.rng.Random, Long...)""/>")>
		public static INDArray rand(Integer() shape, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Return rand(rng, ArrayUtil.toLongArray(shape)).castTo(Nd4j.defaultFloatingPointType())
		End If

		''' <summary>
		''' Create a random ndarray with the given shape using the given RandomGenerator
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="rng">     the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(@NonNull org.nd4j.linalg.api.rng.Random rng, @NonNull long... shape)
		public static INDArray rand( org.nd4j.linalg.api.rng.Random rng, Long... shape)
		If True Then
			Dim ret As INDArray = createUninitialized(shape, Nd4j.order()).castTo(Nd4j.defaultFloatingPointType()) 'INSTANCE.rand(shape, rng);
			Return rand(ret, rng)
		End If

		''' @deprecated use <seealso cref="Nd4j.rand(org.nd4j.linalg.api.rng.distribution.Distribution, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""Nd4j.rand(org.nd4j.linalg.api.rng.distribution.Distribution, Long...)""/>") public static INDArray rand(int[] shape, @NonNull Distribution dist)
		<Obsolete("use <seealso cref=""Nd4j.rand(org.nd4j.linalg.api.rng.distribution.Distribution, Long...)""/>")>
		public static INDArray rand(Integer() shape, Distribution dist)
		If True Then
			Return rand(dist, ArrayUtil.toLongArray(shape)).castTo(Nd4j.defaultFloatingPointType())
		End If

		''' @deprecated use
		''' <seealso cref="org.nd4j.linalg.factory.Nd4j.rand(org.nd4j.linalg.api.rng.distribution.Distribution, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use") public static INDArray rand(long[] shape, @NonNull Distribution dist)
		<Obsolete("use")>
		public static INDArray rand(Long() shape, Distribution dist)
		If True Then
			Return rand(dist, shape)
		End If

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="dist">  distribution to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(@NonNull Distribution dist, @NonNull long... shape)
		public static INDArray rand( Distribution dist, Long... shape)
		If True Then
			Return dist.sample(shape)
		End If

		''' <summary>
		''' Create a random ndarray with the given shape using the given rng
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="rng">       the random generator to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(int rows, int columns, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray rand(Integer rows, Integer columns, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Dim ret As INDArray = createUninitialized(New Integer() {rows, columns}, order())
			Return rand(ret, rng)
		End If

		''' @deprecated use <seealso cref="Nd4j.rand(Double, Double, org.nd4j.linalg.api.rng.Random, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""Nd4j.rand(Double, Double, org.nd4j.linalg.api.rng.Random, Long...)""/>") public static INDArray rand(int[] shape, double min, double max, @NonNull org.nd4j.linalg.api.rng.Random rng)
		<Obsolete("use <seealso cref=""Nd4j.rand(Double, Double, org.nd4j.linalg.api.rng.Random, Long...)""/>")>
		public static INDArray rand(Integer() shape, Double min, Double max, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Return rand(min, max, rng, ArrayUtil.toLongArray(shape))
		End If

		''' @deprecated use <seealso cref="Nd4j.rand(Double, Double, org.nd4j.linalg.api.rng.Random, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""Nd4j.rand(Double, Double, org.nd4j.linalg.api.rng.Random, Long...)""/>") public static INDArray rand(long[] shape, double min, double max, @NonNull org.nd4j.linalg.api.rng.Random rng)
		<Obsolete("use <seealso cref=""Nd4j.rand(Double, Double, org.nd4j.linalg.api.rng.Random, Long...)""/>")>
		public static INDArray rand(Long() shape, Double min, Double max, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Dim ret As INDArray = createUninitialized(shape, order())
			Return rand(ret, min, max, rng)
		End If

		''' <summary>
		''' Generates a random matrix between min and max
		''' </summary>
		''' <param name="shape"> the number of rows of the matrix </param>
		''' <param name="min">   the minimum number </param>
		''' <param name="max">   the maximum number </param>
		''' <param name="rng">   the rng to use </param>
		''' <returns> a random matrix of the specified shape and range </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(double min, double max, @NonNull org.nd4j.linalg.api.rng.Random rng, @NonNull long... shape)
		public static INDArray rand(Double min, Double max, org.nd4j.linalg.api.rng.Random rng, Long... shape)
		If True Then
			Dim ret As INDArray = createUninitialized(shape, order())
			Return rand(ret, min, max, rng)
		End If

		''' <summary>
		''' Generates a random matrix between min and max
		''' </summary>
		''' <param name="rows">    the number of rows of the matrix </param>
		''' <param name="columns"> the number of columns in the matrix </param>
		''' <param name="min">     the minimum number </param>
		''' <param name="max">     the maximum number </param>
		''' <param name="rng">     the rng to use </param>
		''' <returns> a drandom matrix of the specified shape and range </returns>
	'    public static INDArray rand(int rows, int columns, double min, double max, @NonNull org.nd4j.linalg.api.rng.Random rng) {
	'        INDArray ret = createUninitialized(rows, columns);
	'        return rand(ret, min, max, rng);
	'    }

		''' <summary>
		''' Fill the given ndarray with random numbers drawn from a normal distribution
		''' </summary>
		''' <param name="target">  target array </param>
		''' <returns> the given target array </returns>
		public static INDArray randn(INDArray target)
		If True Then
			Return Executioner.exec(New GaussianDistribution(target), Nd4j.Random)
		End If

		''' <summary>
		''' Create a ndarray of the given shape with values from N(0,1)
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(@NonNull int[] shape)
		public static INDArray randn( Integer() shape)
		If True Then
			Return randn(ArrayUtil.toLongArray(shape))
		End If


		''' <summary>
		''' Create a ndarray of the given shape and data type with values from N(0,1)
		''' </summary>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(@NonNull DataType dataType, @NonNull int[] shape)
		public static INDArray randn( DataType dataType, Integer() shape)
		If True Then
			Return randn(dataType, ArrayUtil.toLongArray(shape))
		End If

		''' <summary>
		''' Create a ndarray of the given shape and data type with values from N(0,1)
		''' </summary>
		''' <param name="dataType"> datatype to use, must be a float type datatype. </param>
		''' <param name="shape"> shape for the new array. </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(@NonNull DataType dataType, @NonNull long... shape)
		public static INDArray randn( DataType dataType, Long... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(dataType, shape, order())
			Return randn(ret)
		End If


		''' <summary>
		''' Create a ndarray of the given shape with values from N(0,1).
		''' Defaults to FLOAT and c-order.
		''' </summary>
		''' <param name="shape"> shape for the new array. </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(@NonNull long... shape)
		public static INDArray randn( Long... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(shape, order())
			Return randn(ret)
		End If

		''' <summary>
		''' Random normal N(0,1) with the specified shape and array order
		''' </summary>
		''' <param name="order"> order of the output ndarray </param>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(char order, @NonNull int... shape)
		public static INDArray randn(Char order, Integer... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(shape, order)
			Return randn(ret)
		End If

		''' <summary>
		''' Random normal N(0,1) with the specified shape and array order
		''' </summary>
		''' <param name="order"> order of the output ndarray </param>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(char order, @NonNull long... shape)
		public static INDArray randn(Char order, Long... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(shape, order)
			Return randn(ret)
		End If


		''' <summary>
		''' Random normal N(0,1) with the specified shape and array order
		''' </summary>
		''' <param name="order"> order of the output ndarray </param>
		''' <param name="shape"> the shape of the ndarray </param>
		''' <param name="dataType"> the data type of the ndarray </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(@NonNull DataType dataType, char order, @NonNull long... shape)
		public static INDArray randn( DataType dataType, Char order, Long... shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(dataType, shape, order)
			Return randn(ret)
		End If

		''' @deprecated use <seealso cref="Nd4j.randn(Long, Long[])"/> 
		<Obsolete("use <seealso cref=""Nd4j.randn(Long, Long[])""/>")>
		public static INDArray randn(Long seed, Integer() shape)
		If True Then
			Return randn(seed, ArrayUtil.toLongArray(shape))
		End If

		''' <summary>
		''' Random normal N(0, 1) using the specified seed
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(long seed, @NonNull long[] shape)
		public static INDArray randn(Long seed, Long() shape)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(shape, order())
			Return randn(ret, seed)
		End If

		''' @deprecated use <seealso cref="Nd4j.randn(org.nd4j.linalg.api.rng.Random, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""Nd4j.randn(org.nd4j.linalg.api.rng.Random, Long...)""/>") public static INDArray randn(int[] shape, @NonNull org.nd4j.linalg.api.rng.Random r)
		<Obsolete("use <seealso cref=""Nd4j.randn(org.nd4j.linalg.api.rng.Random, Long...)""/>")>
		public static INDArray randn(Integer() shape, org.nd4j.linalg.api.rng.Random r)
		If True Then
			Return randn(r, ArrayUtil.toLongArray(shape))
		End If

		''' @deprecated use <seealso cref="Nd4j.randn(org.nd4j.linalg.api.rng.Random, Long...)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("use <seealso cref=""Nd4j.randn(org.nd4j.linalg.api.rng.Random, Long...)""/>") public static INDArray randn(long[] shape, @NonNull org.nd4j.linalg.api.rng.Random r)
		<Obsolete("use <seealso cref=""Nd4j.randn(org.nd4j.linalg.api.rng.Random, Long...)""/>")>
		public static INDArray randn(Long() shape, org.nd4j.linalg.api.rng.Random r)
		If True Then
			Return randn(r, shape)
		End If

		''' <summary>
		''' Random normal using the given rng
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="r">     the random generator to use </param>
		''' <returns> new array with random values </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(@NonNull org.nd4j.linalg.api.rng.Random r, @NonNull long... shape)
		public static INDArray randn( org.nd4j.linalg.api.rng.Random r, Long... shape)
		If True Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final INDArray ret = Nd4j.createUninitialized(shape, order());
			Dim ret As INDArray = Nd4j.createUninitialized(shape, order())
			Return randn(ret, r)
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(double mean, double stddev, INDArray target, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray randn(Double mean, Double stddev, INDArray target, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Return Executioner.exec(New GaussianDistribution(target, mean, stddev), rng)
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(double mean, double stddev, long[] shape, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray randn(Double mean, Double stddev, Long() shape, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Dim target As INDArray = Nd4j.createUninitialized(shape)
			Return Executioner.exec(New GaussianDistribution(target, mean, stddev), rng)
		End If
		''' <summary>
		''' Fill the given ndarray with random numbers drawn from a uniform distribution
		''' </summary>
		''' <param name="target">  target array </param>
		''' <returns> the given target array </returns>
		public static INDArray rand(INDArray target)
		If True Then
			Return Executioner.exec(New UniformDistribution(target), Nd4j.Random)
		End If

		''' <summary>
		''' Fill the given ndarray with random numbers drawn from a uniform distribution
		''' </summary>
		''' <param name="target">  target array </param>
		''' <param name="seed"> the  seed to use </param>
		''' <returns> the given target array </returns>
		public static INDArray rand(INDArray target, Long seed)
		If True Then
			Nd4j.Random.setSeed(seed)
			Return Executioner.exec(New UniformDistribution(target), Nd4j.Random)
		End If

		''' <summary>
		''' Fill the given ndarray with random numbers drawn from a uniform distribution using the given RandomGenerator
		''' </summary>
		''' <param name="target">  target array </param>
		''' <param name="rng">     the random generator to use </param>
		''' <returns> the given target array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(INDArray target, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray rand(INDArray target, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Return Executioner.exec(New UniformDistribution(target), rng)
		End If

		''' <summary>
		''' Fill the given ndarray with random numbers drawn from the given distribution
		''' </summary>
		''' <param name="target">  target array </param>
		''' <param name="dist">  distribution to use </param>
		''' <returns> the random ndarray with the specified shape </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(INDArray target, @NonNull Distribution dist)
		public static INDArray rand(INDArray target, Distribution dist)
		If True Then
			Return dist.sample(target)
		End If

		''' <summary>
		''' Fill the given ndarray with random numbers drawn from a uniform distribution using the given RandomGenerator
		''' </summary>
		''' <param name="target">  target array </param>
		''' <param name="min">   the minimum number </param>
		''' <param name="max">   the maximum number </param>
		''' <param name="rng">     the random generator to use </param>
		''' <returns> the given target array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray rand(INDArray target, double min, double max, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray rand(INDArray target, Double min, Double max, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			If min > max Then
				Throw New System.ArgumentException("the maximum value supplied is smaller than the minimum")
			End If
			Return Executioner.exec(New UniformDistribution(target, min, max), rng)
		End If

		''' <summary>
		''' Fill the given ndarray with random numbers drawn from a normal distribution
		''' </summary>
		''' <param name="target">  target array </param>
		''' <returns> the given target array </returns>
		public static INDArray randn(INDArray target, Long seed)
		If True Then
			Nd4j.Random.setSeed(seed)
			Return Executioner.exec(New GaussianDistribution(target), Nd4j.Random)
		End If

		''' <summary>
		''' Fill the given ndarray with random numbers drawn from a normal distribution utilizing the given random generator
		''' </summary>
		''' <param name="target">  target array </param>
		''' <param name="rng">     the random generator to use </param>
		''' <returns> the given target array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randn(INDArray target, @NonNull org.nd4j.linalg.api.rng.Random rng)
		public static INDArray randn(INDArray target, org.nd4j.linalg.api.rng.Random rng)
		If True Then
			Return Executioner.exec(New GaussianDistribution(target), rng)
		End If

		''' <summary>
		''' Generate a random array according to a binomial distribution with probability p: i.e., values 0 with probability
		''' (1-p) or value 1 with probability p
		''' </summary>
		''' <param name="p">     Probability. Must be in range 0 to 1 </param>
		''' <param name="shape"> Shape of the result array </param>
		''' <returns> Result array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randomBernoulli(double p, @NonNull long... shape)
		public static INDArray randomBernoulli(Double p, Long... shape)
		If True Then
			Return randomBernoulli(p, Nd4j.createUninitialized(shape))
		End If

		''' <summary>
		''' Fill the specified array with values generated according to a binomial distribution with probability p: i.e.,
		''' values 0 with probability (1-p) or value 1 with probability p
		''' </summary>
		''' <param name="p">      Probability. Must be in range 0 to 1 </param>
		''' <param name="target"> Result array to place generated values in </param>
		''' <returns> Result array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randomBernoulli(double p, @NonNull INDArray target)
		public static INDArray randomBernoulli(Double p, INDArray target)
		If True Then
			Preconditions.checkArgument(p >= 0 AndAlso p <= 1.0, "Invalid probability: must be in range 0 to 1, got %s", p)
			Return Nd4j.Executioner.exec(New BernoulliDistribution(target, p))
		End If

		''' <summary>
		''' Generate an array with random values generated according to a binomial distribution with the specified
		''' number of trials and probability
		''' </summary>
		''' <param name="nTrials"> Number of trials. Must be >= 0 </param>
		''' <param name="p">       Probability. Must be in range 0 to 1 </param>
		''' <param name="shape">   Shape of the result array </param>
		''' <returns> Result array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray randomBinomial(int nTrials, double p, @NonNull long... shape)
		public static INDArray randomBinomial(Integer nTrials, Double p, Long... shape)
		If True Then
			Return randomBinomial(nTrials, p, Nd4j.createUninitialized(shape))
		End If

		''' <summary>
		''' Fill the target array with random values generated according to a binomial distribution with the specified
		''' number of trials and probability
		''' </summary>
		''' <param name="nTrials"> Number of trials. Must be >= 0 </param>
		''' <param name="p">       Probability. Must be in range 0 to 1 </param>
		''' <param name="target">  Result array </param>
		''' <returns> Result array </returns>
		public static INDArray randomBinomial(Integer nTrials, Double p, INDArray target)
		If True Then
			Preconditions.checkArgument(p >= 0 AndAlso p <= 1.0, "Invalid probability: must be in range 0 to 1, got %s", p)
			Preconditions.checkArgument(nTrials >= 0, "Number of trials must be positive: got %s", nTrials)
			Return Nd4j.Executioner.exec(New BinomialDistribution(target, nTrials, p))
		End If

		''' <summary>
		''' Exponential distribution: P(x) = lambda * exp(-lambda * x)
		''' </summary>
		''' <param name="lambda"> Must be > 0 </param>
		''' <param name="shape">  Shape of the array to generate </param>
		public static INDArray randomExponential(Double lambda, Long... shape)
		If True Then
			Return randomExponential(lambda, Nd4j.createUninitialized(shape))
		End If

		''' <summary>
		''' Exponential distribution: P(x) = lambda * exp(-lambda * x)
		''' </summary>
		''' <param name="lambda"> Must be > 0 </param>
		''' <param name="target"> Array to hold the result </param>
		public static INDArray randomExponential(Double lambda, INDArray target)
		If True Then
			Preconditions.checkArgument(lambda > 0, "Lambda argument must be >= 0 - got %s", lambda)
			Dim shapeArr As INDArray = Nd4j.createFromArray(target.shape())
			Dim r As New RandomExponential(shapeArr, target, lambda)
			Nd4j.exec(r)
			Return target
		End If

		'//////////////////// CREATE ///////////////////////////////

		''' <summary>
		''' This method returns uninitialized 2D array of rows x columns
		''' 
		''' PLEASE NOTE: memory of underlying array will be NOT initialized, and won't be set to 0.0
		''' </summary>
		''' <param name="rows"> rows </param>
		''' <param name="columns"> columns </param>
		''' <returns> uninitialized 2D array of rows x columns </returns>
	'    public static INDArray createUninitialized(long rows, long columns) {
	'        return createUninitialized(new long[] {rows, columns});
	'    }

		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Single() data)
		If True Then
			Return create(data, order())
		End If

		''' <summary>
		''' Create a vector based on a java boolean array. </summary>
		''' <param name="data"> java boolean array </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Boolean() data)
		If True Then
			Return INSTANCE.create(data, New Long(){data.length}, New Long(){1}, DataType.BOOL, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="list"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public static INDArray create(List<? extends Number> list)
		public static INDArray create(IList(Of Number) list)
		If True Then
			Dim array As INDArray = create(list.size())
			Dim cnt As Integer = 0
			If dataType() = DataType.DOUBLE Then
				For Each element As Number In list
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: array.putScalar(cnt++,element.doubleValue());
					array.putScalar(cnt,element.doubleValue())
						cnt += 1
				Next element
			Else
				For Each element As Number In list
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: array.putScalar(cnt++,element.floatValue());
					array.putScalar(cnt,element.floatValue())
						cnt += 1
				Next element
			End If
			Return array
		End If

		''' <summary>
		''' Create double array based on java double array.
		''' </summary>
		''' <param name="data"> java double array, </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Double() data)
		If True Then
			Return create(data, order())
		End If

		''' <summary>
		''' Create 2D float array based on java 2d float array. </summary>
		''' <param name="data"> java 2d arrau. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Single()() data)
		If True Then
			Return INSTANCE.create(data)
		End If

		''' <summary>
		''' Create 2D float array based on java 2d float array and ordering. </summary>
		''' <param name="data"> java 2d arrau. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Single()() data, Char ordering)
		If True Then
			Return INSTANCE.create(data, ordering)
		End If

		''' <summary>
		''' Create 2D double array based on java 2d double array. and ordering
		''' </summary>
		''' <param name="data"> the data to use </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Double()() data)
		If True Then
			Return INSTANCE.create(data)
		End If

		''' <summary>
		''' Create 2D long array based on java 2d long array. </summary>
		''' <param name="data"> java 2d long array </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Long()() data)
		If True Then
			Dim shape As val = New Long(){data.length, data(0).length}
			Return INSTANCE.create(ArrayUtil.flatten(data), shape, getStrides(shape), DataType.LONG, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Create 2D boolean array based on java 2d boolean array. </summary>
		''' <param name="data"> java 2d boolean array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Boolean()() data)
		If True Then
			Dim shape As val = New Long(){data.length, data(0).length}
			Return INSTANCE.create(ArrayUtil.flatten(data), shape, getStrides(shape), DataType.BOOL, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Create a boolean array with given shape based on java 2d boolean array. </summary>
		''' <param name="data"> java 2d boolean array. </param>
		''' <param name="shape"> desired shape of new array. </param>
		''' <returns> the created ndarray. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(boolean[][] data, @NonNull long... shape)
		public static INDArray create(Boolean()() data, Long... shape)
		If True Then
			Return INSTANCE.create(ArrayUtil.flatten(data), shape, getStrides(shape), DataType.BOOL, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Create a 3D double array based on the 3D java double array. </summary>
		''' <param name="data"> java 3d double array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Double()()() data)
		If True Then
			Return create(ArrayUtil.flatten(data), data.length, data(0).length, data(0)(0).length)
		End If

		''' <summary>
		''' Create a 3D float array based on the 3D java float array. </summary>
		''' <param name="data"> java 3d float array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Single()()() data)
		If True Then
			Return create(ArrayUtil.flatten(data), data.length, data(0).length, data(0)(0).length)
		End If

		''' <summary>
		''' Create 2D double array based on java 2d double array. and ordering
		''' </summary>
		''' <param name="data"> the data to use </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Integer()() data)
		If True Then
			Return createFromArray(data)
		End If

		''' <summary>
		''' create 3D int array based on 3D java int array. </summary>
		''' <param name="data"> java 3D i array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Integer()()() data)
		If True Then
			Return create(ArrayUtil.flatten(data), New Integer() {data.length, data(0).length, data(0)(0).length})
		End If

		''' <summary>
		''' Create 4D double array based on 4D java double array. </summary>
		''' <param name="data"> java 4D double array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Double()()()() data)
		If True Then
			Return create(ArrayUtil.flatten(data), data.length, data(0).length, data(0)(0).length, data(0)(0)(0).length)
		End If

		''' <summary>
		''' Create 4D float array based on 4D java float array. </summary>
		''' <param name="data"> java 4D float array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Single()()()() data)
		If True Then
			Return create(ArrayUtil.flatten(data), data.length, data(0).length, data(0)(0).length, data(0)(0)(0).length)
		End If

		''' <summary>
		''' Create 4D int array based on 4D java int array. </summary>
		''' <param name="data"> java 4D int array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Integer()()()() data)
		If True Then
			Return create(ArrayUtil.flatten(data), New Integer() {data.length, data(0).length, data(0)(0).length, data(0)(0)(0).length})
		End If


		''' <summary>
		''' Create a 2D double array based on a 2D java double array with given ordering. </summary>
		''' <param name="data"> java 2D double array. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray, </returns>
		public static INDArray create(Double()() data, Char ordering)
		If True Then
			Return INSTANCE.create(data, ordering)
		End If

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Integer columns)
		If True Then
			Return create(columns, order())
		End If

		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <param name="order"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Single() data, Char order)
		If True Then
			Return INSTANCE.create(data, order)
		End If

		''' <summary>
		''' Creates a row vector with the data
		''' </summary>
		''' <param name="data"> the columns of the ndarray </param>
		''' <param name="order"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Double() data, Char order)
		If True Then
			Return INSTANCE.create(data, order)
		End If

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' </summary>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="order"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Integer columns, Char order)
		If True Then
			Return INSTANCE.create(New Long() {columns}, Nd4j.getStrides(New Long() {columns}, order), 0, order)
		End If

		''' <summary>
		''' Create a 1D float array in soecified order initialized with zero. </summary>
		''' <param name="columns"> number of elements. </param>
		''' <param name="order"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray zeros(Integer columns, Char order)
		If True Then
			Return Nd4j.create(columns, order)
		End If

		''' <summary>
		''' Create an array of the specified type and shape initialized with values from a java 1d array. </summary>
		''' <param name="data"> java array used for initialisation. Must have at least the number of elements required. </param>
		''' <param name="shape"> desired shape of new array. </param>
		''' <param name="type"> Datatype of the new array. Does not need to match int. data will be converted. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Integer() data, Long() shape, DataType type)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape), type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], DataType)"/>
		''' </summary>
		public static INDArray create(Long() data, Long() shape, DataType type)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape), type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], DataType)"/>
		''' </summary>
		public static INDArray create(Double() data, Long() shape, DataType type)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape), type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], DataType)"/>
		''' </summary>
		public static INDArray create(Single() data, Long() shape, DataType type)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape), type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], DataType)"/>
		''' </summary>
		public static INDArray create(Short() data, Long() shape, DataType type)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape), type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], DataType)"/>
		''' </summary>
		public static INDArray create(SByte() data, Long() shape, DataType type)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape), type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], DataType)"/>
		''' </summary>
		public static INDArray create(Boolean() data, Long() shape, DataType type)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape), type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		'//////////////////////////////////////////////

		''' <summary>
		''' Create an array of the specified type, shape and stride initialized with values from a java 1d array. </summary>
		''' <param name="data"> java array used for initialisation. Must have at least the number of elements required. </param>
		''' <param name="shape"> desired shape of new array. </param>
		''' <param name="strides"> stride, separation of elements in each dimension. </param>
		''' <param name="order"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <param name="type"> Datatype of the new array. Does not need to match int. data will be converted. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Integer() data, Long() shape, Long() strides, Char order, DataType type)
		If True Then
			Return INSTANCE.create(data, shape, strides, order, type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], Long[], Char, DataType)"/>
		''' </summary>
		public static INDArray create(Long() data, Long() shape, Long() strides, Char order, DataType type)
		If True Then
			Return INSTANCE.create(data, shape, strides, order, type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], Long[], Char, DataType)"/>
		''' </summary>
		public static INDArray create(Double() data, Long() shape, Long() strides, Char order, DataType type)
		If True Then
			Return INSTANCE.create(data, shape, strides, order, type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], Long[], Char, DataType)"/>
		''' </summary>
		public static INDArray create(Single() data, Long() shape, Long() strides, Char order, DataType type)
		If True Then
			Return INSTANCE.create(data, shape, strides, order, type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], Long[], Char, DataType)"/>
		''' </summary>
		public static INDArray create(Short() data, Long() shape, Long() strides, Char order, DataType type)
		If True Then
			Return INSTANCE.create(data, shape, strides, order, type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], Long[], Char, DataType)"/>
		''' </summary>
		public static INDArray create(SByte() data, Long() shape, Long() strides, Char order, DataType type)
		If True Then
			Return INSTANCE.create(data, shape, strides, order, type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Long[], Long[], Char, DataType)"/>
		''' </summary>
		public static INDArray create(Boolean() data, Long() shape, Long() strides, Char order, DataType type)
		If True Then
			Return INSTANCE.create(data, shape, strides, order, type, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' This method creates "empty" INDArray with datatype determined by <seealso cref="dataType()"/>
		''' </summary>
		''' <returns> Empty INDArray </returns>
		public static INDArray empty()
		If True Then
			Return empty(Nd4j.dataType())
		End If

		''' <summary>
		''' This method creates "empty" INDArray of the specified datatype
		''' </summary>
		''' <returns> Empty INDArray </returns>
		public static INDArray empty(DataType type)
		If True Then
			If EMPTY_ARRAYS(type.ordinal()) Is Nothing Then
				Using ignored As org.nd4j.linalg.api.memory.MemoryWorkspace = Nd4j.MemoryManager.scopeOutOfWorkspaces()
					Dim ret As val = INSTANCE.empty(type)
					EMPTY_ARRAYS(type.ordinal()) = ret
				End Using
			End If
			Return EMPTY_ARRAYS(type.ordinal())
		End If

		''' <summary>
		''' Create an ndrray with the specified shape
		''' </summary>
		''' <param name="data">  the data to use with tne ndarray </param>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Single() data, Integer() shape)
		If True Then
			If shape.length = 0 AndAlso data.length = 1 Then
				Return scalar(data(0))
			End If

			If shape.length = 1 Then
				If shape(0) <> data.length Then
					Throw New ND4JIllegalStateException("Shape of the new array doesn't match data length")
				End If
			End If
			checkShapeValues(data.length, LongUtils.toLongs(shape))
			Return INSTANCE.create(data, shape)
		End If

		''' <summary>
		''' See <seealso cref="create(Single[], Integer[])"/>
		''' </summary>
		public static INDArray create(Single() data, Long... shape)
		If True Then
			If shape.length = 0 AndAlso data.length = 1 Then
				Return scalar(data(0))
			End If
			commonCheckCreate(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape, Nd4j.order()), DataType.FLOAT, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Single[], Integer[])"/>
		''' </summary>
		public static INDArray create(Double() data, Long... shape)
		If True Then
			If shape.length = 0 AndAlso data.length = 1 Then
				Return scalar(data(0))
			End If
			commonCheckCreate(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape, Nd4j.order()), DataType.DOUBLE, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Create an array of the specified shape initialized with values from a java 1d array.
		''' </summary>
		''' <param name="data">  the data to use with tne ndarray </param>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the created ndarray </returns>
		public static INDArray create(Double() data, Integer() shape)
		If True Then
			commonCheckCreate(data.length, LongUtils.toLongs(shape))
			Dim lshape As val = ArrayUtil.toLongArray(shape)
			Return INSTANCE.create(data, lshape, Nd4j.getStrides(lshape, Nd4j.order()), DataType.DOUBLE, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Create an array.
		''' Use specified shape and ordering initialized with values from a java 1d array starting at offset.
		''' </summary>
		''' <param name="data"> java array used for initialisation. Must have at least the number of elements required. </param>
		''' <param name="shape">  desired shape of new array. </param>
		''' <param name="offset"> the offset of data array used for initialisation. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Double() data, Integer() shape, Long offset, Char ordering)
		If True Then
			commonCheckCreate(data.length, LongUtils.toLongs(shape))
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape, ordering), offset, ordering)
		End If

		private static void commonCheckCreate(Integer dataLength, Long() shape)
		If True Then
			If shape.length= 1 Then
				If shape(0) <> dataLength Then
					Throw New ND4JIllegalStateException("Shape of the new array " & java.util.Arrays.toString(shape) & " doesn't match data length: " & dataLength)
				End If
			End If

			checkShapeValues(dataLength, shape)
		End If

		''' <summary>
		''' See <seealso cref="create(Double[], Integer[], Long, Char )"/>
		''' </summary>
		public static INDArray create(Double() data, Long() shape, Long offset, Char ordering)
		If True Then
			commonCheckCreate(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape, ordering), offset, ordering)
		End If

		''' <summary>
		''' Create an array of the specified type, shape and stride initialized with values from a java 1d array using offset.
		''' </summary>
		''' <param name="data"> java array used for initialisation. Must have at least the number of elements required. </param>
		''' <param name="shape"> desired shape of new array. </param>
		''' <param name="stride"> stride, separation of elements in each dimension. </param>
		''' <param name="offset"> the offset of data array used for initialisation. </param>
		''' <returns> the instance </returns>
		public static INDArray create(Single() data, Integer() shape, Integer() stride, Long offset)
		If True Then
			commonCheckCreate(data.length, LongUtils.toLongs(shape))
			Return INSTANCE.create(data, shape, stride, offset)
		End If

		''' <summary>
		''' Creates an array with the specified shape from a list of arrays.
		''' </summary>
		''' <param name="list"> list of arrays. </param>
		''' <param name="shape"> desired shape of new array. Must match the resulting shape of combining the list. </param>
		''' <returns> the instance </returns>
		public static INDArray create(IList(Of INDArray) list, Integer... shape)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(list, shape)
		End If

		''' <summary>
		''' See <seealso cref="create(List, Integer[])"/>
		''' </summary>
		public static INDArray create(IList(Of INDArray) list, Long... shape)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(list, shape)
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <param name="offset">  the offset of the ndarray </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Integer rows, Integer columns, Integer() stride, Long offset)
		If True Then
			If rows < 1 OrElse columns < 1 Then
				Throw New ND4JIllegalStateException("Number of rows and columns should be positive for new INDArray")
			End If

			Return INSTANCE.create(rows, columns, stride, offset)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer , Integer , Integer[] , Long )"/>
		''' </summary>
		public static INDArray zeros(Integer rows, Integer columns, Integer() stride, Long offset)
		If True Then
			Return create(rows, columns, stride, offset)
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		public static INDArray create(Integer() shape, Integer() stride, Long offset)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(shape, stride, offset)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[] , Integer[] , Long )"/>
		''' </summary>
		public static INDArray zeros(Integer() shape, Integer() stride, Long offset)
		If True Then
			Return create(shape, stride, offset)
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="stride">  the stride for the ndarray </param>
		''' <returns> the instance </returns>
		public static INDArray create(Integer rows, Integer columns, Integer() stride)
		If True Then
			Return create(rows, columns, stride, order())
		End If

		''' <summary>
		''' See <seealso cref="@see .create(Integer, Integer, Integer[], Char)"/>
		''' </summary>
		public static INDArray zeros(Integer rows, Integer columns, Integer() stride)
		If True Then
			Return create(rows, columns, stride, order())
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <returns> the instance </returns>
		public static INDArray create(Integer() shape, Integer() stride)
		If True Then
			Return create(shape, stride, order())
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Integer[])"/>
		''' </summary>
		public static INDArray create(Long() shape, Long() stride)
		If True Then
			Return create(shape, stride, order())
		End If


		''' <summary>
		''' See <seealso cref="create(Integer[], Integer[])"/>
		''' </summary>
		public static INDArray zeros(Integer() shape, Integer() stride)
		If True Then
			Return create(shape, stride)
		End If


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the instance </returns>
		public static INDArray create(Integer... shape)
		If True Then
			Return create(shape, order())
		End If


		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the instance </returns>
		public static INDArray create(Long... shape)
		If True Then
			Return create(shape, order())
		End If

		''' <summary>
		''' Create an array with specified shape and datatype.
		''' </summary>
		''' <param name="type"> Datatype of the new array. </param>
		''' <param name="shape">  desired shape of new array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(DataType type, Long... shape)
		If True Then
			Return create(type, shape, order())
		End If

		''' <summary>
		''' Create an array based on the data buffer with given shape, stride and offset.
		''' </summary>
		''' <param name="data"> data buffer used for initialisation. . Must have at least the number of elements required. </param>
		''' <param name="shape"> desired shape of new array. </param>
		''' <param name="strides"> stride, separation of elements in each dimension. </param>
		''' <param name="offset"> the offset of data array used for initialisation. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(DataBuffer data, Integer() shape, Integer() strides, Long offset)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(data, shape, strides, offset)
		End If

		''' <summary>
		''' See <seealso cref="create(DataBuffer, Integer[], Integer[], Long)"/>
		''' </summary>
		public static INDArray create(DataBuffer data, Long() shape, Long() strides, Long offset)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(data, shape, strides, offset)
		End If

		''' <summary>
		''' See <seealso cref="create(DataBuffer, Integer[], Integer[], Long)"/>. Uses default strides based on shape.
		''' </summary>
		public static INDArray create(DataBuffer data, Integer() shape, Long offset)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(data, shape, getStrides(shape), offset)
		End If

		''' <summary>
		''' See <seealso cref="create(DataBuffer data, Long[], Long[], Long, Long, Char )"/>
		''' </summary>
		public static INDArray create(DataBuffer data, Integer() newShape, Integer() newStride, Long offset, Char ordering)
		If True Then
			checkShapeValues(newShape)
			Return INSTANCE.create(data, newShape, newStride, offset, ordering)
		End If

		''' <summary>
		''' See <seealso cref="create(DataBuffer data, Long[], Long[], Long, Long, Char )"/>
		''' </summary>
		public static INDArray create(DataBuffer data, Long() newShape, Long() newStride, Long offset, Char ordering)
		If True Then
			checkShapeValues(newShape)
			Return INSTANCE.create(data, newShape, newStride, offset, ordering)
		End If

		''' <summary>
		''' Create an array based on the data buffer with given shape, stride and offset.
		''' </summary>
		''' <param name="data"> data buffer used for initialisation. . Must have at least the number of elements required. </param>
		''' <param name="newShape"> desired shape of new array. </param>
		''' <param name="newStride"> stride, separation of elements in each dimension. </param>
		''' <param name="offset"> the offset of data array used for initialisation. </param>
		''' <param name="ews"> element wise stride. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(DataBuffer data, Long() newShape, Long() newStride, Long offset, Long ews, Char ordering)
		If True Then
			checkShapeValues(newShape)
			Return INSTANCE.create(data, newShape, newStride, offset, ews, ordering)
		End If

		''' <summary>
		''' Create an array based on the data buffer with given shape, stride, offset and data type.
		''' </summary>
		''' <param name="data"> data buffer used for initialisation. . Must have at least the number of elements required. </param>
		''' <param name="newShape"> desired shape of new array. </param>
		''' <param name="newStride"> stride, separation of elements in each dimension. </param>
		''' <param name="offset"> the offset of data array used for initialisation. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <param name="dataType"> data type. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(DataBuffer data, Long() newShape, Long() newStride, Long offset, Char ordering, DataType dataType)
		If True Then
			checkShapeValues(newShape)
			Return INSTANCE.create(data, newShape, newStride, offset, ordering, dataType)
		End If

		' This method gets it own javadoc and not a @see because it is used  often.
		''' <summary>
		''' Create an array based on the data buffer with given shape.
		''' </summary>
		''' <param name="data"> data data buffer used for initialisation. . Must have at least the number of elements required. </param>
		''' <param name="shape"> desired shape of new array. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(DataBuffer data, Integer... shape)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(data, shape)
		End If

		''' <summary>
		''' See <seealso cref="create(DataBuffer, Integer[])"/>
		''' </summary>
		public static INDArray create(DataBuffer data, Long... shape)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(data, shape)
		End If

		' This method gets it own javadoc and not a @see because it is used  often.
		''' <summary>
		''' Create an array based on the data buffer.
		''' </summary>
		''' <param name="buffer"> data data buffer used for initialisation. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(DataBuffer buffer)
		If True Then
			Return INSTANCE.create(buffer)
		End If

		''' <summary>
		''' Create an array of given shape and data type. </summary>
		''' <param name="shape"> desired shape of new array. </param>
		''' <param name="dataType"> data type. </param>
		''' <returns>  the created ndarray. </returns>
		public static INDArray create(Integer() shape, DataType dataType)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(shape, dataType, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <seealso cref= #create(int[], DataType) </seealso>
		public static INDArray zeros(Integer() shape, DataType dataType)
		If True Then
			Return create(shape, dataType)
		End If

		' This method gets it own javadoc and not a @see because it is used  often.
		''' <summary>
		''' Create an array withgiven shape and ordering based on a java double array. </summary>
		''' <param name="data"> java array used for initialisation. Must have at least the number of elements required. </param>
		''' <param name="shape"> desired shape of new array. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created ndarray. </returns>
		public static INDArray create(Double() data, Integer() shape, Char ordering)
		If True Then
			commonCheckCreate(data.length, LongUtils.toLongs(shape))
			Dim lshape As val = ArrayUtil.toLongArray(shape)
			Return INSTANCE.create(data, lshape, Nd4j.getStrides(lshape, ordering), ordering, DataType.DOUBLE, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' See <seealso cref="create(Double[], Integer[], Char)"/>
		''' </summary>
		public static INDArray create(Single() data, Integer() shape, Char ordering)
		If True Then
			commonCheckCreate(data.length, LongUtils.toLongs(shape))
			Return INSTANCE.create(data, shape, ordering)
		End If

		''' <summary>
		''' See <seealso cref=" create(Double[], Integer[], Char)"/>
		''' </summary>
		public static INDArray create(Single() data, Long() shape, Char ordering)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape, ordering), ordering, DataType.FLOAT)
		End If

		''' <summary>
		''' See <seealso cref="create(Double[], Integer[], Char)"/>
		''' </summary>
		public static INDArray create(Double() data, Long() shape, Char ordering)
		If True Then
			checkShapeValues(data.length, shape)
			Return INSTANCE.create(data, shape, Nd4j.getStrides(shape, ordering), ordering, DataType.DOUBLE, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="offset"> the offset of the ndarray </param>
		''' <returns> the instance </returns>
		public static INDArray create(Long() shape, Long() stride, Long offset, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(0.0)
			End If

			checkShapeValues(shape)
			Return INSTANCE.create(shape, stride, offset, ordering)
		End If

		''' <summary>
		''' Create a 2D array with given rows, columns, stride and ordering. </summary>
		''' <param name="rows"> number of rows. </param>
		''' <param name="columns"> number of columns </param>
		''' <param name="stride"> stride, separation of elements in each dimension. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created array. </returns>
		public static INDArray create(Integer rows, Integer columns, Integer() stride, Char ordering)
		If True Then
			Dim shape() As Integer = {rows, columns}
			checkShapeValues(shape)
			Return INSTANCE.create(shape, stride, 0, ordering)
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape">  the shape of the ndarray </param>
		''' <param name="stride"> the stride for the ndarray </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the instance </returns>
		public static INDArray create(Integer() shape, Integer() stride, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(Nd4j.dataType(), 0.0)
			End If

			checkShapeValues(shape)
			Return INSTANCE.create(shape, stride, 0, ordering)
		End If

		''' <summary>
		''' See <seealso cref="create(Integer[], Integer[], Char)"/>
		''' </summary>
		public static INDArray create(Long() shape, Long() stride, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(Nd4j.dataType(), 0.0)
			End If

			checkShapeValues(shape)
			Return INSTANCE.create(shape, stride, 0, ordering)
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the instance </returns>
		public static INDArray create(Long rows, Long columns, Char ordering)
		If True Then
			Return create(New Long() {rows, columns}, ordering)
		End If

		''' <summary>
		''' Create a 2D array initialized with zeros.
		''' </summary>
		''' <param name="rows">    the rows of the ndarray </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the instance </returns>
		public static INDArray zeros(Integer rows, Integer columns, Char ordering)
		If True Then
			Return create(New Integer() {rows, columns}, ordering)
		End If

		''' <summary>
		''' Creates an ndarray with the specified shape
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the instance </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(@NonNull int[] shape, char ordering)
		public static INDArray create( Integer() shape, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType(), 0.0)
			End If

			Return INSTANCE.create(shape, ordering)
		End If

		' used  often.
		''' <summary>
		''' Create an array with given shape and ordering.
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(@NonNull long[] shape, char ordering)
		public static INDArray create( Long() shape, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType(), 0.0)
			End If
			'ensure shapes that wind up being scalar end up with the write shape

			checkShapeValues(shape)
			Return INSTANCE.create(shape, ordering)
		End If

		''' <summary>
		''' Create an array with given shape, stride  and ordering.
		''' </summary>
		''' <param name="dataType"> data type. </param>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="strides"> stride, separation of elements in each dimension. </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(DataType dataType, @NonNull long[] shape, long[] strides, char ordering)
		public static INDArray create(DataType dataType, Long() shape, Long() strides, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType, 0.0)
			End If

			checkShapeValues(shape)
			Return INSTANCE.create(dataType, shape, strides, ordering, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		' used often.
		''' <summary>
		''' Create an array with given data type shape and ordering.
		''' </summary>
		''' <param name="dataType"> data type. </param>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(@NonNull DataType dataType, @NonNull long[] shape, char ordering)
		public static INDArray create( DataType dataType, Long() shape, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType, 0.0)
			End If
			'ensure shapes that wind up being scalar end up with the write shape
			checkShapeValues(shape)
			Return INSTANCE.create(dataType, shape, ordering, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Throws exception on negative shape values. </summary>
		''' <param name="shape"> to check </param>
		public static void checkShapeValues(Long... shape)
		If True Then
			For Each e As Long In shape
				If e < 0 Then
					Throw New ND4JIllegalStateException("Invalid shape: Requested INDArray shape " & java.util.Arrays.toString(shape) & " contains dimension size values < 0 (all dimensions must be 0 or more)")
				End If
			Next e
		End If

		' made private as it is only used for internal checks.
		private static void checkShapeValues(Integer... shape)
		If True Then
			checkShapeValues(LongUtils.toLongs(shape))
		End If

		private static void checkShapeValues(Integer length, Long... shape)
		If True Then
			checkShapeValues(shape)
			If ArrayUtil.prodLong(shape) <> length AndAlso Not (length = 1 AndAlso shape.length = 0) Then
				Throw New ND4JIllegalStateException("Shape of the new array " & java.util.Arrays.toString(shape) & " doesn't match data length: " & length & " - prod(shape) must equal the number of values provided")
			End If
		End If


		''' <summary>
		''' Creates an *uninitialized* array with the specified shape and ordering.<br>
		''' <b>NOTE</b>: The underlying memory (DataBuffer) will not be initialized. Don't use this unless you know what you are doing.
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the instance </returns>
		public static INDArray createUninitialized(Integer() shape, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return scalar(dataType(), 0.0)
			End If

			checkShapeValues(shape)
			Return INSTANCE.createUninitialized(shape, ordering)
		End If

		public static INDArray createUninitialized(DataType type, Long... shape)
		If True Then
			Return createUninitialized(type, shape, Nd4j.order())
		End If

		''' <summary>
		''' Creates an *uninitialized* array with the specified data type, shape and ordering.
		''' </summary>
		''' <param name="type"> data type </param>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created array. </returns>
		public static INDArray createUninitialized(DataType type, Long() shape, Char ordering)
		If True Then
			If shape.length = 0 Then
				If type = DataType.UTF8 Then
					Return scalar("")
				End If
				Return scalar(type, 0)
			End If

			checkShapeValues(shape)
			Return INSTANCE.createUninitialized(type, shape, ordering, Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Creates an *uninitialized* array with the specified shape and ordering.
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="ordering"> Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <returns> the created array. </returns>
		public static INDArray createUninitialized(Long() shape, Char ordering)
		If True Then
			If shape.length = 0 Then
				Return scalar(dataType(), 0.0)
			End If

			checkShapeValues(shape)
			Return INSTANCE.createUninitialized(shape, ordering)
		End If

		''' <summary>
		''' See <seealso cref="createUninitialized(Long[])"/>
		''' </summary>
		public static INDArray createUninitialized(Integer... shape)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType(), 0.0)
			End If
			checkShapeValues(shape)
			'ensure shapes that wind up being scalar end up with the write shape
			Return createUninitialized(shape, Nd4j.order())
		End If

		''' <summary>
		''' Creates an *uninitialized* ndarray with the specified shape and default ordering.<br>
		''' <b>NOTE</b>: The underlying memory (DataBuffer) will not be initialized. Don't use this unless you know what you are doing.
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the instance </returns>
		public static INDArray createUninitialized(Long... shape)
		If True Then
			checkShapeValues(shape)
			'ensure shapes that wind up being scalar end up with the write shape
			Return createUninitialized(shape, Nd4j.order())
		End If

		''' <summary>
		''' This method creates an *uninitialized* ndarray of specified length and default ordering.
		''' 
		''' PLEASE NOTE: Do not use this method unless you're 100% sure why you use it.
		''' </summary>
		''' <param name="length"> length of array to create </param>
		''' <returns> the created INDArray </returns>
		public static INDArray createUninitialized(Long length)
		If True Then
			Dim shape() As Long = {length}
			Return INSTANCE.createUninitialized(shape, order())
		End If

		''' <summary>
		''' Create an uninitialized ndArray. Detached from workspace. </summary>
		''' <param name="dataType"> data type. Exceptions will be thrown for UTF8, COMPRESSED and UNKNOWN data types. </param>
		''' <param name="ordering">  Fortran 'f' or C/C++ 'c' ordering. </param>
		''' <param name="shape"> the shape of the array. </param>
		''' <returns> the created detached array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static INDArray createUninitializedDetached(DataType dataType, char ordering, long... shape)
		public static INDArray createUninitializedDetached(DataType dataType, Char ordering, Long... shape)
		If True Then
			Return INSTANCE.createUninitializedDetached(dataType, ordering, shape)
		End If

		''' <summary>
		''' See <seealso cref="createUninitializedDetached(DataType, Char, Long...)"/> with default ordering.
		''' </summary>
		public static INDArray createUninitializedDetached(DataType dataType, Long... shape)
		If True Then
			Return createUninitializedDetached(dataType, order(), shape)
		End If


		'//////////////////// OTHER ///////////////////////////////


		''' <summary>
		''' Creates an array with the specified data tyoe and shape initialized with zero.
		''' </summary>
		''' <param name="dataType"> data type. </param>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the created array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray zeros(DataType dataType, @NonNull long... shape)
		public static INDArray zeros(DataType dataType, Long... shape)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType, 0)
			End If

			Return INSTANCE.create(dataType, shape, Nd4j.order(), Nd4j.MemoryManager.CurrentWorkspace)
		End If

		''' <summary>
		''' Creates an ndarray with the specified value
		''' as the  only value in the ndarray.
		''' Some people may know this as np.full
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="value"> the value to assign </param>
		''' <returns> the created ndarray </returns>
		public static INDArray valueArrayOf(Integer() shape, Double value)
		If True Then
			If shape.length = 0 Then
				Return scalar(value)
			End If

			checkShapeValues(shape)
			Return INSTANCE.valueArrayOf(shape, value)
		End If

		''' <summary>
		''' Creates an ndarray with the specified value as the only value in the FLOAT32 datatype NDArray.
		''' Equivalent to Numpy's np.full
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="value"> the value to assign </param>
		''' <returns> the created ndarray </returns>
		public static INDArray valueArrayOf(Long() shape, Single value)
		If True Then
			Return valueArrayOf(shape, CDbl(value), DataType.FLOAT)
		End If

		''' <summary>
		''' Creates an ndarray with the specified value as the only value in the INTEGER datatype NDArray.
		''' Equivalent to Numpy's np.full
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="value"> the value to assign </param>
		''' <returns> the created ndarray </returns>
		public static INDArray valueArrayOf(Long() shape, Integer value)
		If True Then
			Return valueArrayOf(shape, CDbl(value), DataType.INT)
		End If

		''' <summary>
		''' See <seealso cref="valueArrayOf(Long[], Double, DataType)"/>
		''' </summary>
		public static INDArray valueArrayOf(Long() shape, Double value)
		If True Then
			If shape.length = 0 Then
				Return scalar(value)
			End If

			checkShapeValues(shape)
			Return INSTANCE.valueArrayOf(shape, value)
		End If

		''' <summary>
		''' Creates an ndarray with the specified value
		''' as the  only value in the ndarray.
		''' Some people may know this as np.full
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="value"> the value to assign </param>
		''' <param name="type"> data type </param>
		''' <returns> the created ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("Duplicates") public static INDArray valueArrayOf(long[] shape, double value, DataType type)
		public static INDArray valueArrayOf(Long() shape, Double value, DataType type)
		If True Then
			If shape.length = 0 Then
				Return scalar(type, value)
			End If

			checkShapeValues(shape)

			Dim ret As INDArray = createUninitialized(type, shape)
			ret.assign(value)
			Return ret
		End If

		''' <summary>
		''' See <seealso cref="valueArrayOf(Long[], Double, DataType)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("Duplicates") public static INDArray valueArrayOf(long[] shape, long value, DataType type)
		public static INDArray valueArrayOf(Long() shape, Long value, DataType type)
		If True Then
			If shape.length = 0 Then
				Return scalar(type, value)
			End If

			checkShapeValues(shape)

			Dim ret As INDArray = createUninitialized(type, shape)
			ret.assign(value)
			Return ret
		End If

		''' <summary>
		''' Creates a row vector ndarray with the specified value
		''' as the  only value in the ndarray
		''' 
		''' Some people may know this as np.full
		''' </summary>
		''' <param name="num">   number of columns </param>
		''' <param name="value"> the value to assign </param>
		''' <returns> the created ndarray </returns>
		public static INDArray valueArrayOf(Long num, Double value)
		If True Then
			Return INSTANCE.valueArrayOf(New Long() {num}, value)
		End If

		''' <summary>
		''' Creates a row vector with the specified number of columns
		''' 
		''' Some people may know this as np.full
		''' </summary>
		''' <param name="rows">    the number of rows in the matrix </param>
		''' <param name="columns"> the columns of the ndarray </param>
		''' <param name="value">   the value to assign </param>
		''' <returns> the created ndarray </returns>
		public static INDArray valueArrayOf(Long rows, Long columns, Double value)
		If True Then
			Return INSTANCE.valueArrayOf(rows, columns, value)
		End If

		''' <summary>
		''' Empty like
		''' </summary>
		''' <param name="arr"> the array to create the ones like </param>
		''' <returns> ones in the shape of the given array </returns>
		public static INDArray zerosLike(INDArray arr)
		If True Then
			Return zeros(arr.dataType(), arr.shape())
		End If

		''' <summary>
		''' Ones like
		''' </summary>
		''' <param name="arr"> the array to create the ones like </param>
		''' <returns> ones in the shape of the given array </returns>
		public static INDArray onesLike(INDArray arr)
		If True Then
			Return ones(arr.dataType(), arr.shape())
		End If

		''' <summary>
		''' Creates an array with the specified datatype and shape, with values all set to 1
		''' </summary>
		''' <param name="shape"> Shape fo the array </param>
		''' <returns> the created ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray ones(DataType dataType, @NonNull long... shape)
		public static INDArray ones(DataType dataType, Long... shape)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType, 1.0)
			End If
			Dim ret As INDArray = INSTANCE.createUninitialized(dataType, shape, Nd4j.order(), Nd4j.MemoryManager.CurrentWorkspace)
			ret.assign(1)
			Return ret
		End If

		''' <summary>
		''' Concatenates two matrices horizontally. Matrices must have identical
		''' numbers of rows.
		''' </summary>
		''' <param name="arrs"> the first matrix to concat </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray hstack(@NonNull INDArray... arrs)
		public static INDArray hstack( INDArray... arrs)
		If True Then
			Return INSTANCE.hstack(arrs)
		End If

		''' <summary>
		''' Concatenates two matrices horizontally. Matrices must have identical
		''' numbers of rows.
		''' </summary>
		''' <param name="arrs"> the first matrix to concat </param>
		public static INDArray hstack(ICollection(Of INDArray) arrs)
		If True Then
			Dim arrays() As INDArray = arrs.toArray(New INDArray(){})
			Return INSTANCE.hstack(arrays)
		End If

		''' <summary>
		''' Concatenates two matrices vertically. Matrices must have identical numbers of columns.<br>
		''' Note that for vstack on rank 1 arrays, this is equivalent to <seealso cref="Nd4j.pile(INDArray...)"/>. Example: vstack([3],[3]) -> [2,3]
		''' </summary>
		''' <param name="arrs"> Arrays to vstack </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray vstack(@NonNull INDArray... arrs)
		public static INDArray vstack( INDArray... arrs)
		If True Then
			Preconditions.checkState(arrs IsNot Nothing AndAlso arrs.length > 0, "No input specified to vstack (null or length 0)")
			'noinspection ConstantConditions
			If arrs(0).rank() = 1 Then
				'Edge case: vstack rank 1 arrays - gives rank 2... vstack([3],[3]) -> [2,3]
				Return pile(arrs)
			End If
			Return INSTANCE.vstack(arrs)
		End If

		''' <summary>
		''' Concatenates two matrices vertically. Matrices must have identical numbers of columns.<br>
		''' Note that for vstack on rank 1 arrays, this is equivalent to <seealso cref="Nd4j.pile(INDArray...)"/>. Example: vstack([3],[3]) -> [2,3]
		''' </summary>
		''' <param name="arrs"> Arrays to vstack </param>
		public static INDArray vstack(ICollection(Of INDArray) arrs)
		If True Then
			Dim arrays() As INDArray = arrs.toArray(New INDArray(){})
			Return vstack(arrays)
		End If

		''' <summary>
		''' This method averages input arrays, and returns averaged array.
		''' On top of that, averaged array is propagated to all input arrays
		''' </summary>
		''' <param name="arrays"> arrays to average </param>
		''' <returns> averaged arrays </returns>
		public static INDArray averageAndPropagate(INDArray() arrays)
		If True Then
			Return INSTANCE.average(arrays)
		End If


		''' <summary>
		''' This method averages input arrays, and returns averaged array.
		''' On top of that, averaged array is propagated to all input arrays
		''' </summary>
		''' <param name="arrays"> arrays to average </param>
		''' <returns> averaged arrays </returns>
		public static INDArray averageAndPropagate(ICollection(Of INDArray) arrays)
		If True Then
			Return INSTANCE.average(arrays)
		End If

		''' <summary>
		''' This method averages input arrays, and returns averaged array.
		''' On top of that, averaged array is propagated to all input arrays
		''' </summary>
		''' <param name="arrays"> arrays to average </param>
		''' <returns> averaged arrays </returns>
		public static INDArray averageAndPropagate(INDArray target, ICollection(Of INDArray) arrays)
		If True Then
			Return INSTANCE.average(target, arrays)
		End If

		''' <summary>
		''' Reshapes an ndarray to remove leading 1s </summary>
		''' <param name="toStrip"> the ndarray to newShapeNoCopy </param>
		''' <returns> the reshaped ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings({"unused"}) public static INDArray stripOnes(INDArray toStrip)
		public static INDArray stripOnes(INDArray toStrip)
		If True Then
			If toStrip.isVector() Then
				Return toStrip
			Else
				Dim shape() As Long = Shape.squeeze(toStrip.shape())
				Return toStrip.reshape(shape)
			End If
		End If

		''' <summary>
		''' This method sums given arrays and stores them to a new array
		''' </summary>
		''' <param name="arrays"> array to accumulate </param>
		''' <returns> accumulated array. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray accumulate(@NonNull INDArray... arrays)
		public static INDArray accumulate( INDArray... arrays)
		If True Then
			If arrays Is Nothing OrElse arrays.length = 0 Then
				Throw New ND4JIllegalStateException("Input for accumulation is null or empty")
			End If

			Return accumulate(Nd4j.create(arrays(0).shape(), arrays(0).ordering()), arrays)
		End If

		''' <summary>
		''' This method sums given arrays and stores them to a given target array
		''' </summary>
		''' <param name="target"> result array </param>
		''' <param name="arrays"> arrays to sum </param>
		''' <returns> result array </returns>
		public static INDArray accumulate(INDArray target, ICollection(Of INDArray) arrays)
		If True Then
			Return accumulate(target, arrays.toArray(New INDArray(){}))
		End If

		''' <summary>
		''' This method sums given arrays and stores them to a given target array
		''' </summary>
		''' <param name="target"> result array </param>
		''' <param name="arrays"> arrays to sum </param>
		''' <returns> result array </returns>
		public static INDArray accumulate(INDArray target, INDArray() arrays)
		If True Then
			If arrays Is Nothing OrElse arrays.length = 0 Then
				Return target
			End If
			Return factory().accumulate(target, arrays)
		End If

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some dimension and specified indexes
		''' </summary>
		''' <param name="source"> source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes"> indexes from source array </param>
		''' <returns> result array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pullRows(INDArray source, int sourceDimension, @NonNull int... indexes)
		public static INDArray pullRows(INDArray source, Integer sourceDimension, Integer... indexes)
		If True Then
			Return pullRows(source, sourceDimension, indexes, Nd4j.order())
		End If

		''' <summary>
		''' This method produces concatenated array,
		''' that consist from tensors,
		''' fetched from source array,
		''' against some dimension and specified indexes
		''' </summary>
		''' <param name="source"> source tensor </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes"> indexes from source array </param>
		''' <returns> concatenated array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("Duplicates") public static INDArray pullRows(INDArray source, int sourceDimension, int[] indexes, char order)
		public static INDArray pullRows(INDArray source, Integer sourceDimension, Integer() indexes, Char order)
		If True Then
			If sourceDimension >= source.rank() Then
				Throw New System.InvalidOperationException("Source dimension can't be higher the rank of source tensor")
			End If

			If indexes Is Nothing OrElse indexes.length = 0 Then
				Throw New System.InvalidOperationException("Indexes shouldn't be empty")
			End If

			If order <> "c"c AndAlso order <> "f"c AndAlso order <> "a"c Then
				Throw New System.InvalidOperationException("Unknown order being passed in [" & order & "]")
			End If

			For Each idx As Integer In indexes
				If idx < 0 OrElse idx >= source.shape()(source.rank() - sourceDimension - 1) Then
					Throw New System.InvalidOperationException("Index can't be < 0 and >= " & source.shape()(source.rank() - sourceDimension - 1))
				End If
			Next idx

			Preconditions.checkArgument(source.rank() > 1, "pullRows() can't operate on 0D/1D arrays")
			Return INSTANCE.pullRows(source, sourceDimension, indexes, order)
		End If

		''' <summary>
		''' This method produces concatenated array, that consist from tensors, fetched from source array, against some
		''' dimension and specified indexes.
		''' The concatenated arrays are placed in the specified array.
		''' </summary>
		''' <param name="source"> source tensor </param>
		''' <param name="destination"> Destination tensor (result will be placed here) </param>
		''' <param name="sourceDimension"> dimension of source tensor </param>
		''' <param name="indexes"> indexes from source array </param>
		''' <returns> Destination array with specified tensors </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("Duplicates") public static INDArray pullRows(INDArray source, INDArray destination, int sourceDimension, @NonNull int... indexes)
		public static INDArray pullRows(INDArray source, INDArray destination, Integer sourceDimension, Integer... indexes)
		If True Then
			If sourceDimension >= source.rank() Then
				Throw New System.InvalidOperationException("Source dimension can't be higher the rank of source tensor")
			End If

			If indexes Is Nothing OrElse indexes.length = 0 Then
				Throw New System.InvalidOperationException("Indexes shouldn't be empty")
			End If

			For Each idx As Integer In indexes
				If idx < 0 OrElse idx >= source.shape()(source.rank() - sourceDimension - 1) Then
					Throw New System.InvalidOperationException("Index can't be < 0 and >= " & source.shape()(source.rank() - sourceDimension - 1))
				End If
			Next idx

			Preconditions.checkArgument(source.rank() > 1, "pullRows() can't operate on 0D/1D arrays")

			Return INSTANCE.pullRows(source, destination, sourceDimension, indexes)
		End If

		''' <summary>
		''' Stack a set of N SDVariables of rank X into one rank X+1 variable.
		''' If inputs have shape [a,b,c] then output has shape:<br>
		''' axis = 0: [N,a,b,c]<br>
		''' axis = 1: [a,N,b,c]<br>
		''' axis = 2: [a,b,N,c]<br>
		''' axis = 3: [a,b,c,N]<br>
		''' </summary>
		''' <param name="axis">   Axis to stack on </param>
		''' <param name="values"> Input variables to stack. Must have the same shape for all inputs </param>
		''' <returns> Output array </returns>
		''' <seealso cref= #concat(int, INDArray...) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("ConstantConditions") public static INDArray stack(int axis, @NonNull INDArray... values)
		public static INDArray stack(Integer axis, INDArray... values)
		If True Then
			Preconditions.checkArgument(values IsNot Nothing AndAlso values.length > 0, "No inputs: %s", CType(values, Object()))
			Preconditions.checkState(axis >= -(values(0).rank()+1) AndAlso axis < values(0).rank()+1, "Invalid axis: must be between " & "%s (inclusive) and %s (exclusive) for rank %s input, got %s", -(values(0).rank()+1), values(0).rank()+1, values(0).rank(), axis)

			Dim stack As New Stack(values, Nothing, axis)
			Dim outputArrays() As INDArray = Nd4j.Executioner.allocateOutputArrays(stack)
			stack.addOutputArgument(outputArrays)
			Nd4j.Executioner.execAndReturn(stack)
			Return outputArrays(0)
		End If

		''' <summary>
		''' Concatneate ndarrays along a dimension
		''' </summary>
		''' <param name="dimension"> the dimension to concatneate along </param>
		''' <param name="toConcat">  the ndarrays to concat </param>
		''' <returns> the merged ndarrays with an output shape of
		''' the ndarray shapes save the dimension shape specified
		''' which is then the sum of the sizes along that dimension </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray concat(int dimension, @NonNull INDArray... toConcat)
		public static INDArray concat(Integer dimension, INDArray... toConcat)
		If True Then
			If dimension < 0 Then
				dimension += toConcat(0).rank()
			End If

			Return INSTANCE.concat(dimension, toConcat)
		End If

		''' <summary>
		''' Concatneate ndarrays along a dimension
		''' 
		''' PLEASE NOTE: This method is special for GPU backend, it works on HOST side only.
		''' </summary>
		''' <param name="dimension"> dimension </param>
		''' <param name="toConcat"> arrayts to concatenate </param>
		''' <returns> concatenated arrays. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray specialConcat(int dimension, @NonNull INDArray... toConcat)
		public static INDArray specialConcat(Integer dimension, INDArray... toConcat)
		If True Then
			Return INSTANCE.specialConcat(dimension, toConcat)
		End If

		''' <summary>
		''' Create an ndarray of zeros
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> an ndarray with ones filled in </returns>
		public static INDArray zeros(Integer() shape, Char order)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(shape, order)
		End If

		''' <summary>
		''' See <seealso cref="zeros(Integer[] , Char)"/>
		''' </summary>
		public static INDArray zeros(Long() shape, Char order)
		If True Then
			checkShapeValues(shape)
			Return INSTANCE.create(shape, order)
		End If

		''' <summary>
		''' Create an ndarray of zeros
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> an ndarray with ones filled in </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray zeros(@NonNull int... shape)
		public static INDArray zeros( Integer... shape)
		If True Then
			Return Nd4j.create(shape)
		End If


		''' <summary>
		''' Create an ndarray of zeros
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> an ndarray with ones filled in </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray zeros(@NonNull long... shape)
		public static INDArray zeros( Long... shape)
		If True Then
			Return Nd4j.create(shape)
		End If

		''' <summary>
		''' Create an ndarray of ones
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> an ndarray with ones filled in </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray ones(@NonNull int... shape)
		public static INDArray ones( Integer... shape)
		If True Then
			Return If(shape.length = 0, Nd4j.scalar(dataType(), 1.0), INSTANCE.ones(shape))
		End If


		''' <summary>
		''' See <seealso cref="ones(Integer... shape)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray ones(@NonNull long... shape)
		public static INDArray ones( Long... shape)
		If True Then
			If shape.length = 0 Then
				Return Nd4j.scalar(dataType(), 1.0)
			End If
			checkShapeValues(shape)
			Return INSTANCE.ones(shape)
		End If

		''' <summary>
		''' Create a scalar ndarray with the specified value
		''' </summary>
		''' <param name="value"> the value to initialize the scalar with </param>
		''' <returns> the created ndarray </returns>
		public static INDArray scalar(Number value)
		If True Then
			Return INSTANCE.scalar(value)
		End If

		''' <summary>
		''' Create a scalar ndarray with the specified value and datatype
		''' </summary>
		''' <param name="value"> the value to initialize the scalar with </param>
		''' <returns> the created ndarray </returns>
		public static INDArray scalar(DataType dataType, Number value)
		If True Then
			Dim ws As val = Nd4j.MemoryManager.CurrentWorkspace

			Select Case dataType
				Case [DOUBLE]
					Return INSTANCE.create(New Double() {value.doubleValue()}, New Long() {}, New Long() {}, dataType, ws)
				Case FLOAT, BFLOAT16, HALF
					Return INSTANCE.create(New Single() {value.floatValue()}, New Long() {}, New Long() {}, dataType, ws)
				Case UINT32, INT
					Return INSTANCE.create(New Integer() {value.intValue()}, New Long() {}, New Long() {}, dataType, ws)
				Case UINT64, [LONG]
					Return INSTANCE.create(New Long() {value.longValue()}, New Long() {}, New Long() {}, dataType, ws)
				Case UINT16, [SHORT]
					Return INSTANCE.create(New Short() {value.shortValue()}, New Long() {}, New Long() {}, dataType, ws)
				Case [BYTE]
					Return INSTANCE.create(New SByte() {value.byteValue()}, New Long() {}, New Long() {}, dataType, ws)
				Case UBYTE
					Return INSTANCE.create(New Short() {value.shortValue()}, New Long() {}, New Long() {}, dataType, ws)
				Case BOOL
					Return INSTANCE.create(New SByte() {value.byteValue()}, New Long() {}, New Long() {}, dataType, ws)

				Case Else
					Throw New System.NotSupportedException("Unsupported data type used: " & dataType)
			End Select
		End If

		''' <summary>
		''' Create a scalar nd array with the specified value
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		''' <returns> the scalar nd array </returns>
		public static INDArray scalar(Double value)
		If True Then
			Return scalar(DataType.DOUBLE, value)
		End If

		''' <summary>
		''' Create a scalar NDArray with the specified value and FLOAT datatype
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		''' <returns> the scalar nd array </returns>
		public static INDArray scalar(Single value)
		If True Then
			Return scalar(DataType.FLOAT, value)
		End If

		''' <summary>
		''' Create a scalar NDArray with the specified value and BOOLEAN datatype
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		''' <returns> the scalar nd array </returns>
		public static INDArray scalar(Boolean value)
		If True Then
			Return scalar(DataType.BOOL,If(value, 1, 0))
		End If

		''' <summary>
		''' Create a scalar NDArray with the specified value and INT datatype
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		''' <returns> the scalar nd array </returns>
		public static INDArray scalar(Integer value)
		If True Then
			Return scalar(DataType.INT, value)
		End If

		''' <summary>
		''' Create a scalar NDArray with the specified value and LONG datatype
		''' </summary>
		''' <param name="value"> the value of the scalar </param>
		''' <returns> the scalar nd array </returns>
		public static INDArray scalar(Long value)
		If True Then
			Return scalar(DataType.LONG, value)
		End If

		''' <summary>
		''' Get the strides for the given order and shape
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <param name="order"> the order to getScalar the strides for </param>
		''' <returns> the strides for the given shape and order </returns>
		public static Integer() getStrides(Integer() shape, Char order)
		If True Then
			If order = NDArrayFactory.FORTRAN Then
				Return ArrayUtil.calcStridesFortran(shape)
			End If
			Return ArrayUtil.calcStrides(shape)
		End If

		public static Long() getStrides(Long() shape, Char order)
		If True Then
			If order = NDArrayFactory.FORTRAN Then
				Return ArrayUtil.calcStridesFortran(shape)
			End If
			Return ArrayUtil.calcStrides(shape)
		End If

		''' <summary>
		''' Get the strides based on the shape
		''' and NDArrays.order()
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the strides for the given shape
		''' and order specified by NDArrays.order() </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static int[] getStrides(@NonNull int... shape)
		public static Integer() getStrides( Integer... shape)
		If True Then
			Return getStrides(shape, Nd4j.order())
		End If

		''' <summary>
		''' Get the strides based on the shape
		''' and NDArrays.order()
		''' </summary>
		''' <param name="shape"> the shape of the array </param>
		''' <returns> the strides for the given shape
		''' and order specified by NDArrays.order() </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static long[] getStrides(@NonNull long... shape)
		public static Long() getStrides( Long... shape)
		If True Then
			Return getStrides(shape, Nd4j.order())
		End If

		''' <summary>
		''' An alias for repmat
		''' </summary>
		''' <param name="tile">   the ndarray to tile </param>
		''' <param name="repeat"> the shape to repeat </param>
		''' <returns> the tiled ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray tile(INDArray tile, @NonNull int... repeat)
		public static INDArray tile(INDArray tile, Integer... repeat)
		If True Then
			Return Nd4j.exec(New Tile(New INDArray(){tile}, New INDArray(){}, repeat))(0)
		End If

		''' <summary>
		''' Initializes nd4j
		''' </summary>
		private synchronized void initContext()
		If True Then
			Try
				defaultFloatingPointDataType = New AtomicReference(Of DataType)()
				defaultFloatingPointDataType.set(DataType.FLOAT)
				Dim backend As Nd4jBackend = Nd4jBackend.load()
				initWithBackend(backend)
			Catch e As NoAvailableBackendException
				Throw New Exception(e)
			End Try
		End If

		''' <summary>
		''' Initialize with the specific backend </summary>
		''' <param name="backend"> the backend to initialize with </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings({"unchecked", "Duplicates"}) public void initWithBackend(Nd4jBackend backend)
		public void initWithBackend(Nd4jBackend backend_Conflict)
		If True Then
			VersionCheck.checkVersions()

			Try
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				If System.getProperties().getProperty("backends") IsNot Nothing AndAlso Not System.getProperties().getProperty("backends").contains(backend_Conflict.GetType().FullName) Then
					Return
				End If

				If Not SupportedPlatform Then
					showAttractiveMessage(MessageForUnsupportedPlatform)
					Return
				End If

				Nd4j.backend_Conflict = backend_Conflict
				updateNd4jContext()
				props = Nd4jContext.Instance.Conf
				Dim pp As New PropertyParser(props)

				Dim otherDtype As String = pp.toString(ND4JSystemProperties.DTYPE)
				dtype_Conflict = If(otherDtype.Equals("float", StringComparison.OrdinalIgnoreCase), DataType.FLOAT, If(otherDtype.Equals("half", StringComparison.OrdinalIgnoreCase), DataType.HALF, DataType.DOUBLE))

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				If dtype_Conflict = DataType.HALF AndAlso backend_Conflict.GetType().FullName.Equals("CpuBackend") Then
					showAttractiveMessage(MessageForNativeHalfPrecision)
				End If

				If Nd4j.dataType() <> dtype_Conflict Then
					DataTypeUtil.setDTypeForContext(dtype_Conflict)
				End If

				compressDebug = pp.toBoolean(COMPRESSION_DEBUG)
				Dim ORDER As Char = pp.toChar(ORDER_KEY, NDArrayFactory.C)

				Dim affinityManagerClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(AFFINITY_MANAGER))
				affinityManager_Conflict = System.Activator.CreateInstance(affinityManagerClazz)
				Dim ndArrayFactoryClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(NDARRAY_FACTORY_CLASS))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim convolutionInstanceClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(CONVOLUTION_OPS, GetType(DefaultConvolutionInstance).FullName))
				Dim defaultName As String = pp.toString(DATA_BUFFER_OPS, "org.nd4j.linalg.cpu.nativecpu.buffer.DefaultDataBufferFactory")
				Dim dataBufferFactoryClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(DATA_BUFFER_OPS, defaultName))
				Dim shapeInfoProviderClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(SHAPEINFO_PROVIDER))

				Dim constantProviderClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(CONSTANT_PROVIDER))

				Dim memoryManagerClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(MEMORY_MANAGER))

				allowsOrder = backend_Conflict.allowsOrder()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim rand As String = pp.toString(RANDOM_PROVIDER, GetType(DefaultRandom).FullName)
				Dim randomClazz As Type = ND4JClassLoading.loadClassByName(rand)
				randomFactory_Conflict = New RandomFactory(randomClazz)

				Dim workspaceManagerClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(WORKSPACE_MANAGER))

				Dim blasWrapperClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(BLAS_OPS))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim clazzName As String = pp.toString(DISTRIBUTION, GetType(DefaultDistributionFactory).FullName)
				Dim distributionFactoryClazz As Type = ND4JClassLoading.loadClassByName(clazzName)


				memoryManager_Conflict = System.Activator.CreateInstance(memoryManagerClazz)
				constantHandler_Conflict = System.Activator.CreateInstance(constantProviderClazz)
				shapeInfoProvider_Conflict = System.Activator.CreateInstance(shapeInfoProviderClazz)
				workspaceManager_Conflict = System.Activator.CreateInstance(workspaceManagerClazz)

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim opExecutionerClazz As Type = ND4JClassLoading.loadClassByName(pp.toString(OP_EXECUTIONER, GetType(DefaultOpExecutioner).FullName))

				OP_EXECUTIONER_INSTANCE = System.Activator.CreateInstance(opExecutionerClazz)
				Dim c2 As System.Reflection.ConstructorInfo = ndArrayFactoryClazz.GetConstructor(GetType(DataType), GetType(Char))
				INSTANCE = DirectCast(c2.Invoke(dtype_Conflict, ORDER), NDArrayFactory)
				CONVOLUTION_INSTANCE = System.Activator.CreateInstance(convolutionInstanceClazz)
				BLAS_WRAPPER_INSTANCE = System.Activator.CreateInstance(blasWrapperClazz)
				DATA_BUFFER_FACTORY_INSTANCE = System.Activator.CreateInstance(dataBufferFactoryClazz)

				DISTRIBUTION_FACTORY = System.Activator.CreateInstance(distributionFactoryClazz)

				If Fallback Then
					fallbackMode.set(True)
					showAttractiveMessage(MessageForFallback)
				Else
					fallbackMode.set(False)
				End If

				Dim logInitProperty As String = System.getProperty(ND4JSystemProperties.LOG_INITIALIZATION, "true")
				If Boolean.Parse(logInitProperty) Then
					OP_EXECUTIONER_INSTANCE.printEnvironmentInformation()
				End If

				Dim actions As val = ND4JClassLoading.loadService(GetType(EnvironmentalAction))
				Dim mappedActions As val = New Dictionary(Of String, EnvironmentalAction)()
				For Each a As val In actions
					If Not mappedActions.containsKey(a.targetVariable()) Then
						mappedActions.put(a.targetVariable(), a)
					End If
				Next a

				For Each e As val In mappedActions.keySet()
					Dim action As val = mappedActions.get(e)
					Dim value As val = System.Environment.GetEnvironmentVariable(e)
					If value IsNot Nothing Then
						Try
							action.process(value)
						Catch e2 As Exception
							logger.info("Failed to process env variable [" & e & "], got exception: " & e2.ToString())
						End Try
					End If
				Next e

				backend_Conflict.logBackendInit()
			Catch e As Exception
				Throw New Exception(e)
			End Try

		End If

		private static Boolean SupportedPlatform
		If True Then
			Return (System.getProperty("java.vm.name").equalsIgnoreCase("Dalvik") OrElse System.getProperty("os.arch").ToLower().StartsWith("arm", StringComparison.Ordinal) OrElse System.getProperty("sun.arch.data.model").Equals("64"))
		End If

		private static void showAttractiveMessage(String... strings)
		If True Then
			Console.WriteLine(attract(strings))
		End If

		private static String attract(String... strings)
		If True Then
			Dim delimiter As String = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
			Dim shift As String = "                 "
			Dim sb As StringBuilder = (New StringBuilder()).Append(delimiter).Append(vbLf).Append(vbLf)
			For Each s As String In strings
				sb.Append(shift).Append(s).Append(vbLf)
			Next s
			sb.Append(vbLf).Append(delimiter).Append(vbLf)
			Return sb.ToString()
		End If

		private static String() MessageForUnsupportedPlatform
		If True Then
			Return New String() {"Unfortunately you can't use DL4j/ND4j on 32-bit x86 JVM", "Please, consider running this on 64-bit JVM instead"}
		End If

		private static String() MessageForFallback
		If True Then
			Return New String() {"ND4J_FALLBACK environment variable is detected!", "Performance will be slightly reduced"}
		End If

		private String() MessageForNativeHalfPrecision
		If True Then
			Return New String() {"Half-precision data opType isn't support for nd4j-native", "Please, consider using FLOAT or DOUBLE data opType instead"}
		End If

		private void updateNd4jContext() throws IOException
		If True Then
			Using [is] As Stream = backend_Conflict.ConfigurationResource.InputStream
				Nd4jContext.Instance.updateProperties([is])
			End Using
		End If

		private Boolean Fallback
		If True Then
			Dim fallback As String = System.Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_FALLBACK)
			If fallback Is Nothing Then
				Return False
			End If
			Return (fallback.Equals("true", StringComparison.OrdinalIgnoreCase) OrElse fallback.Equals("1", StringComparison.OrdinalIgnoreCase))
		End If

		''' 
		''' <returns> Shape info provider </returns>
		public static ShapeInfoProvider ShapeInfoProvider
		If True Then
			Return shapeInfoProvider_Conflict
		End If

		''' 
		''' <returns> constant handler </returns>
		public static ConstantHandler ConstantHandler
		If True Then
			Return constantHandler_Conflict
		End If

		''' 
		''' <returns> affinity manager </returns>
		public static AffinityManager AffinityManager
		If True Then
			Return affinityManager_Conflict
		End If

		''' 
		''' <returns> NDArrayFactory </returns>
		public static NDArrayFactory NDArrayFactory
		If True Then
			Return INSTANCE
		End If

		''' <summary>
		''' This method returns BasicNDArrayCompressor instance,
		''' suitable for NDArray compression/decompression
		''' at runtime
		''' </summary>
		''' <returns> BasicNDArrayCompressor instance </returns>
		public static BasicNDArrayCompressor Compressor
		If True Then
			Return BasicNDArrayCompressor.Instance
		End If

		''' <summary>
		''' This method returns backend-specific MemoryManager implementation, for low-level memory management </summary>
		''' <returns> MemoryManager </returns>
		public static MemoryManager MemoryManager
		If True Then
			Return memoryManager_Conflict
		End If

		''' <summary>
		''' This method returns sizeOf(currentDataType), in bytes
		''' </summary>
		''' <returns> number of bytes per element </returns>
		''' @deprecated Use DataType.width() 
		<Obsolete("Use DataType.width()")>
		public static Integer sizeOfDataType()
		If True Then
			Return sizeOfDataType(Nd4j.dataType())
		End If

		''' <summary>
		''' This method returns size of element for specified dataType, in bytes
		''' </summary>
		''' <param name="dtype"> number of bytes per element </param>
		''' <returns> element size </returns>
		public static Integer sizeOfDataType(DataType dtype_Conflict)
		If True Then
			Select Case dtype_Conflict.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE, BOOL, UBYTE
					Return 1
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16, [SHORT], BFLOAT16, HALF
					Return 2
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32, FLOAT, INT
					Return 4
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64, [LONG], [DOUBLE]
					Return 8
				Case Else
					Throw New ND4JIllegalStateException("Unsupported data type: [" & dtype_Conflict & "]")
			End Select
		End If

		''' <summary>
		''' This method enables fallback to safe-mode for specific operations. Use of this method will reduce performance.
		''' Currently supported operations are:
		'''  1) CPU GEMM
		''' 
		''' PLEASE NOTE: Do not use this method, unless you have too.
		''' </summary>
		''' <param name="reallyEnable"> fallback mode </param>
		public static void enableFallbackMode(Boolean reallyEnable)
		If True Then
			fallbackMode.set(reallyEnable)
		End If

		''' <summary>
		''' This method checks, if fallback mode was enabled.
		''' </summary>
		''' <returns> fallback mode </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("BooleanMethodIsAlwaysInverted") public static boolean isFallbackModeEnabled()
		public static Boolean FallbackModeEnabled
		If True Then
			Return fallbackMode.get()
		End If

		''' <summary>
		''' This method returns WorkspaceManager implementation to be used within this JVM process
		''' </summary>
		''' <returns> WorkspaceManager </returns>
		public static MemoryWorkspaceManager WorkspaceManager
		If True Then
			Return workspaceManager_Conflict
		End If

		''' <summary>
		''' This method stacks vertically examples with the same shape, increasing result dimensionality.
		''' I.e. if you provide bunch of 3D tensors, output will be 4D tensor. Alignment is always applied to axis 0.
		''' </summary>
		''' <param name="arrays"> arrays to stack </param>
		''' <returns> stacked arrays </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pile(@NonNull INDArray... arrays)
		public static INDArray pile( INDArray... arrays)
		If True Then
			' if we have vectors as input, it's just vstack use case

			Dim shape() As Long = arrays(0).shape()
			'noinspection deprecation
			Dim newShape() As Long = ArrayUtils.add(shape, 0, 1)

			Dim reshaped As IList(Of INDArray) = New List(Of INDArray)()
			For Each array As INDArray In arrays
				reshaped.Add(array.reshape(array.ordering(), newShape))
			Next array

			Return Nd4j.vstack(reshaped)
		End If

		''' <summary>
		''' This method stacks vertically examples with the same shape, increasing result dimensionality. I.e. if you provide bunch of 3D tensors, output will be 4D tensor. Alignment is always applied to axis 0.
		''' </summary>
		''' <param name="arrays"> arrays to stack </param>
		''' <returns> stacked array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray pile(@NonNull Collection<INDArray> arrays)
		public static INDArray pile( ICollection(Of INDArray) arrays)
		If True Then
			Return pile(arrays.toArray(New INDArray(){}))
		End If

		''' <summary>
		''' This method does the opposite to pile/vstack/hstack - it returns independent TAD copies along given dimensions
		''' </summary>
		''' <param name="tensor"> Array to tear </param>
		''' <param name="dimensions"> dimensions </param>
		''' <returns> Array copies </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray[] tear(INDArray tensor, @NonNull int... dimensions)
		public static INDArray() tear(INDArray tensor, Integer... dimensions)
		If True Then
			If dimensions.length >= tensor.rank() Then
				Throw New ND4JIllegalStateException("Target dimensions number should be less tensor rank")
			End If

			For Each dimension As Integer In dimensions
				If dimension < 0 Then
					Throw New ND4JIllegalStateException("Target dimensions can't have negative values")
				End If
			Next dimension

			Return factory().tear(tensor, dimensions)
		End If

		''' <summary>
		'''   Upper triangle of an array.
		''' 
		''' Return a copy of a matrix with the elements below the `k`-th diagonal
		''' zeroed.
		''' 
		''' Please refer to the documentation for `tril` for further details.
		''' 
		''' See Also
		''' --------
		''' tril : lower triangle of an array
		''' 
		''' Examples
		''' --------
		''' >>> np.triu([[1,2,3],[4,5,6],[7,8,9],[10,11,12]], -1)
		''' array([[ 1,  2,  3],
		''' [ 4,  5,  6],
		''' [ 0,  8,  9],
		''' [ 0,  0, 12]])
		''' 
		''' """
		''' m = asanyarray(m)
		''' mask = tri(*m.shape[-2:], k=k-1, dtype=bool)
		''' 
		''' return where(mask, zeros(1, m.dtype), m)
		''' </summary>
		''' <param name="m"> source array </param>
		''' <param name="k"> to zero below the k-th diagonal </param>
		''' <returns> copy with elements  below the `k`-th diagonal zeroed. </returns>
		public static INDArray triu(INDArray m,Integer k)
		If True Then

	'        
	'         * Find a way to apply choose with an existing condition array.
	'         * (This appears to be the select op in libnd4j)
	'         
			Dim result As INDArray = Nd4j.createUninitialized(m.shape())

			Dim op As val = DynamicCustomOp.builder("triu").addInputs(m).addOutputs(result).addIntegerArguments(k).build()

			Nd4j.Executioner.execAndReturn(op)
			Return result
		End If

		''' <summary>
		''' See <seealso cref="tri(Integer,Integer,Integer)"/> with m = n, k=0.
		''' </summary>
		public static INDArray tri(Integer n)
		If True Then
			Return tri(n,n,0)
		End If

		''' <summary>
		''' See <seealso cref="tri(Integer,Integer,Integer)"/> with m = n.
		''' </summary>
		public static INDArray tri(Integer n,Integer k)
		If True Then
			Return tri(n,n,k)
		End If

		''' <summary>
		''' Like the scipy function tri.
		''' From the scipy documentation:
		'''  An array with ones at and below the given diagonal and zeros elsewhere. </summary>
		''' <param name="n"> number of rows in the array </param>
		''' <param name="m"> number of columns in the array ( can be just equal to n) </param>
		''' <param name="k">    The sub-diagonal at and below which the array is filled.
		''' `k` = 0 is the main diagonal, while `k` < 0 is below it,
		''' and `k` > 0 is above.  The default is 0. </param>
		''' <returns> array with ones at and below the given diagonal and zeros elsewhere </returns>
		public static INDArray tri(Integer n,Integer m,Integer k)
		If True Then
			Dim ret As INDArray = Nd4j.createUninitialized(n, m)
			Dim op As val = DynamicCustomOp.builder("tri").addIntegerArguments(n, m, k).addOutputs(ret).build()

			Nd4j.Executioner.execAndReturn(op)
			Return ret
		End If

		''' <summary>
		''' Similar to numpy.where operation.
		''' Supports two modes of operation:<br>
		''' (a) condition array only is provided: returns N 1d arrays of the indices where "condition" values are non-zero.
		''' Specifically, each output out has shape [numNonZero(condition)], such that in[out[0], ..., out[n-1]] is non-zero<br>
		''' (b) all 3 arrays are provided: returns {@code out[i] = (condition[i] != 0 ? x[i] : y[i])}<br> </summary>
		''' <param name="condition"> Condition array </param>
		''' <param name="x">         X array. If null, y must be null also. </param>
		''' <param name="y">         Y array. If null, x must be null also </param>
		''' <returns> Either the indices where condition is non-zero (if x and y are null), or values from x/y depending on
		''' value of condition </returns>
		public static INDArray() where(INDArray condition, INDArray x, INDArray y)
		If True Then
			Preconditions.checkState((x Is Nothing AndAlso y Is Nothing) OrElse (x IsNot Nothing AndAlso y IsNot Nothing), "Both X and Y must be" & "null, or neither must be null")
			Dim op As DynamicCustomOp.DynamicCustomOpsBuilder = DynamicCustomOp.builder("where_np")
			Dim outShapes As IList(Of LongShapeDescriptor)
			If x Is Nothing Then
				'First case: condition only...
				op.addInputs(condition)
			Else
				If Not x.equalShapes(y) OrElse Not x.equalShapes(condition) Then
					'noinspection ConstantConditions
					Preconditions.throwStateEx("Shapes must be equal: condition=%s, x=%s, y=%s", condition.shape(), x.shape(), y.shape())
				End If
				op.addInputs(condition, x, y)
			End If
			Dim o As DynamicCustomOp = op.build()
			outShapes = Nd4j.Executioner.calculateOutputShape(o)
			Dim outputs(outShapes.Count - 1) As INDArray

			If x Is Nothing AndAlso (outShapes(0) Is Nothing OrElse outShapes(0).getShape().length = 0 OrElse outShapes(0).getShape()(0) = 0) Then
				'Empty: no conditions match
				For i As Integer = 0 To outputs.Length - 1
					outputs(i) = Nd4j.empty()
				Next i
				Return outputs
			End If

			For i As Integer = 0 To outputs.Length - 1
				outputs(i) = Nd4j.create(outShapes(i), False)
			Next i
			op.addOutputs(outputs)

			Nd4j.Executioner.execAndReturn(op.build())
			Return outputs
		End If


		''' <summary>
		''' Write an <seealso cref="INDArray"/> to a <seealso cref="File"/> in Numpy .npy format, which can then be loaded with numpy.load </summary>
		''' <param name="arr"> the array to write in Numpy .npy format </param>
		''' <param name="file"> the file to write to </param>
		''' <exception cref="IOException"> if an error occurs when writing the file </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static void writeAsNumpy(INDArray arr, File file) throws IOException
		public static void writeAsNumpy(INDArray arr, File file) throws IOException
		If True Then
			writeAsNumpy(arr, New FileStream(file, FileMode.Create, FileAccess.Write))
		End If


		''' <summary>
		''' Converts an <seealso cref="INDArray"/> to a numpy struct. </summary>
		''' <param name="arr"> the array to convert </param>
		''' <returns> a pointer to the numpy struct </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static Pointer convertToNumpy(INDArray arr)
		public static Pointer convertToNumpy(INDArray arr)
		If True Then
			Return INSTANCE.convertToNumpy(arr)
		End If


		''' <summary>
		''' Writes an array to an output stream </summary>
		''' <param name="arr"> the array to write </param>
		''' <param name="writeTo"> the output stream to write to </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static void writeAsNumpy(INDArray arr, OutputStream writeTo) throws IOException
		public static void writeAsNumpy(INDArray arr, Stream writeTo) throws IOException
		If True Then
			Using bufferedOutputStream As New BufferedOutputStream(writeTo)
				Dim asNumpy As Pointer = convertToNumpy(arr)
				Dim channel As WritableByteChannel = Channels.newChannel(bufferedOutputStream)

				Dim written As Integer = channel.write(asNumpy.asByteBuffer())
				If written <> asNumpy.capacity() Then
					Throw New System.InvalidOperationException("Not all bytes were written! Original capacity " & asNumpy.capacity() & " but wrote " & written)
				End If

				bufferedOutputStream.flush()
			End Using
		End If


		''' <summary>
		''' Create from an in memory numpy pointer
		''' </summary>
		''' <param name="pointer"> the pointer to the
		'''                numpy array </param>
		''' <returns> an ndarray created from the in memory
		''' numpy pointer </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static INDArray createFromNpyPointer(Pointer pointer)
		public static INDArray createFromNpyPointer(Pointer pointer)
		If True Then
			Return INSTANCE.createFromNpyPointer(pointer)
		End If

		''' <summary>
		''' Create an INDArray from a given Numpy .npy file.
		''' </summary>
		''' <param name="path"> Path to the .npy file to read </param>
		''' <returns> the created ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray readNpy(@NonNull String path)
		public static INDArray readNpy( String path)
		If True Then
			Return readNpy(New File(path))
		End If

		''' <summary>
		''' Create an INDArray from a given Numpy .npy file.
		''' </summary>
		''' <param name="file"> the file to create the ndarray from </param>
		''' <returns> the created ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray readNpy(@NonNull File file)
		public static INDArray readNpy( File file)
		If True Then
			Return createFromNpyFile(file)
		End If

		''' <summary>
		''' Create an INDArray from a given Numpy .npy file.
		''' </summary>
		''' <param name="file"> the file to create the ndarray from </param>
		''' <returns> the created ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray createFromNpyFile(@NonNull File file)
		public static INDArray createFromNpyFile( File file)
		If True Then
			If Not file.exists() Then
				Throw New System.ArgumentException("File [" & file.getAbsolutePath() & "] doesn't exist")
			End If

			Return INSTANCE.createFromNpyFile(file)
		End If

		public static IDictionary(Of String, INDArray) createFromNpzFile(File file) throws Exception
		If True Then
			Return INSTANCE.createFromNpzFile(file)
		End If

		''' <summary>
		''' Create a numpy array based on the passed in input stream </summary>
		''' <param name="is"> the input stream to read </param>
		''' <returns> the loaded ndarray </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unused") public static INDArray createNpyFromInputStream(@NonNull InputStream is) throws IOException
		public static INDArray createNpyFromInputStream( Stream [is]) throws IOException
		If True Then
			Dim content() As SByte = IOUtils.toByteArray([is])
			Return createNpyFromByteArray(content)
		End If


		''' <summary>
		''' Create an <seealso cref="INDArray"/> from the given numpy input.<br>
		''' The numpy input follows the format:
		''' https://docs.scipy.org/doc/numpy-1.14.0/neps/npy-format.html
		''' </summary>
		''' <param name="input"> the input byte array with the npy format </param>
		''' <returns> the equivalent <seealso cref="INDArray"/> </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray createNpyFromByteArray(@NonNull byte[] input)
		public static INDArray createNpyFromByteArray( SByte() input)
		If True Then
			Dim byteBuffer As ByteBuffer = ByteBuffer.allocateDirect(input.length)
			byteBuffer.put(input)
			CType(byteBuffer, Buffer).rewind()
			Dim pointer As New Pointer(byteBuffer)
			Return createFromNpyPointer(pointer)
		End If

		''' <summary>
		''' Converts an <seealso cref="INDArray"/> to a byte array </summary>
		''' <param name="input"> the input array </param>
		''' <returns> the <seealso cref="INDArray"/> as a byte array
		''' with the numpy format.
		''' For more on the format, see: https://docs.scipy.org/doc/numpy-1.14.0/neps/npy-format.html </returns>
		public static SByte() toNpyByteArray(INDArray input)
		If True Then
			Try
				Dim byteArrayOutputStream As New MemoryStream()
				writeAsNumpy(input, byteArrayOutputStream)
				Return byteArrayOutputStream.toByteArray()
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try
		End If


		''' <summary>
		''' Create an <seealso cref="INDArray"/> from a flatbuffers <seealso cref="FlatArray"/> </summary>
		''' <param name="array"> the array to create the <seealso cref="INDArray"/> from </param>
		''' <returns> the created <seealso cref="INDArray"/> </returns>
		public static INDArray createFromFlatArray(FlatArray array)
		If True Then
'JAVA TO VB CONVERTER NOTE: The variable dtype was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim dtype_Conflict As val = array.dtype()
			Dim order As val = array.byteOrder()
			Dim rank As val = CInt(Math.Truncate(array.shape(0)))
			Dim shapeInfo As val = New Long(Shape.shapeInfoLength(rank) - 1){}
			For e As Integer = 0 To shapeInfo.length - 1
				shapeInfo(e) = array.shape(e)
			Next e

			Dim shapeOf As val = Shape.shapeOf(shapeInfo)
			Dim _dtype As DataType = FlatBuffersMapper.getDataTypeFromByte(dtype_Conflict)
			If Shape.isEmpty(shapeInfo) Then
				If Shape.rank(shapeInfo) = 0 Then
					Return Nd4j.empty()
				Else
					Return Nd4j.create(_dtype, shapeOf)
				End If
			End If

			Dim ordering As Char = If(shapeInfo(shapeInfo.length - 1) = 99, "c"c, "f"c)


			Dim stridesOf As val = Shape.stridesOf(shapeInfo)


			Dim _order As val = FlatBuffersMapper.getOrderFromByte(order)
			Dim prod As val = If(rank > 0, ArrayUtil.prod(shapeOf), 1)

			Dim bb As val = array.bufferAsByteBuffer()
			Select Case _dtype.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Dim doubles As val = New Double(prod - 1){}
					Dim db As val = bb.order(_order).asDoubleBuffer()
					For e As Integer = 0 To prod - 1
						doubles(e) = db.get(e)
					Next e

					Return Nd4j.create(doubles, shapeOf, stridesOf, ordering, DataType.DOUBLE)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Dim doubles As val = New Single(prod - 1){}
					Dim fb As val = bb.order(_order).asFloatBuffer()
					For e As Integer = 0 To prod - 1
						doubles(e) = fb.get(e)
					Next e

					Return Nd4j.create(doubles, shapeOf, stridesOf, ordering, DataType.FLOAT)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Dim doubles As val = New Single(prod - 1){}
					Dim sb As val = bb.order(_order).asShortBuffer()
					For e As Integer = 0 To prod - 1
						doubles(e) = HalfIndexer.toFloat(CInt(Math.Truncate(sb.get(e))))
					Next e

					Return Nd4j.create(doubles, shapeOf, stridesOf, ordering, DataType.HALF)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Dim doubles As val = New Integer(prod - 1){}
					Dim sb As val = bb.order(_order).asIntBuffer()
					For e As Integer = 0 To prod - 1
						doubles(e) = sb.get(e)
					Next e

					Return Nd4j.create(doubles, shapeOf, stridesOf, ordering, DataType.INT)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					Dim doubles As val = New Long(prod - 1){}
					Dim sb As val = bb.order(_order).asLongBuffer()
					For e As Integer = 0 To prod - 1
						doubles(e) = sb.get(e)
					Next e

					Return Nd4j.create(doubles, shapeOf, stridesOf, ordering, DataType.LONG)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Dim doubles As val = New Short(prod - 1){}
					Dim sb As val = bb.order(_order).asShortBuffer()
					For e As Integer = 0 To prod - 1
						doubles(e) = sb.get(e)
					Next e

					Return Nd4j.create(doubles, shapeOf, stridesOf, ordering, DataType.SHORT)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Dim bytes As val = New SByte(prod - 1){}
					Dim sb As val = bb.order(_order).asReadOnlyBuffer()
					For e As Integer = 0 To prod - 1
						bytes(e) = sb.get(e + sb.position())
					Next e

					Return Nd4j.create(bytes, shapeOf, stridesOf, ordering, DataType.BYTE)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Dim doubles As val = New Boolean(prod - 1){}
					Dim sb As val = bb.order(_order).asReadOnlyBuffer()
					For e As Integer = 0 To prod - 1
						doubles(e) = sb.get(e + sb.position()) = 1
					Next e

					Return Nd4j.create(doubles, shapeOf, stridesOf, ordering, DataType.BOOL)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UTF8
					Try
						Dim sb As val = bb.order(_order)
						Dim pos As val = sb.position()
						Dim arr As val = New SByte((sb.limit() - pos) - 1){}

						For e As Integer = 0 To arr.length - 1
							arr(e) = sb.get(e + pos)
						Next e

						Dim buffer As val = DATA_BUFFER_FACTORY_INSTANCE.createUtf8Buffer(arr, prod)
						Return Nd4j.create(buffer, shapeOf)
					Catch e As Exception
						Throw New Exception(e)
					End Try
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE, BFLOAT16, UINT16
					Dim arr As INDArray = Nd4j.createUninitialized(_dtype, shapeOf)
					Dim obb As ByteBuffer = bb.order(_order)
					Dim pos As Integer = obb.position()
					Dim bArr((obb.limit() - pos) - 1) As SByte

					For e As Integer = 0 To bArr.Length - 1
						bArr(e) = obb.get(e + pos)
					Next e
					arr.data().asNio().put(bArr)
					Return arr
				Case Else
					Throw New System.NotSupportedException("Unknown datatype: [" & _dtype & "]")
			End Select

		End If

		public static DataType defaultFloatingPointType()
		If True Then
			Return defaultFloatingPointDataType.get()
		End If

		public static Boolean PrecisionBoostAllowed
		If True Then
			Return False
		End If


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray scalar(@NonNull String string)
		public static INDArray scalar( String [string])
		If True Then
			'noinspection RedundantArrayCreation
			Return create(Collections.singletonList([string]), New Long(){})
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(@NonNull String... strings)
		public static INDArray create( String... strings)
		If True Then
			Return create(New List(Of INDArray) From {strings}, New Long(){strings.length}, Nd4j.order())
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(@NonNull Collection<String> strings, long... shape)
		public static INDArray create( ICollection(Of String) strings, Long... shape)
		If True Then
			Return create(strings, shape, Nd4j.order())
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static INDArray create(@NonNull Collection<String> strings, long[] shape, char order)
		public static INDArray create( ICollection(Of String) strings, Long() shape, Char order)
		If True Then
			Return INSTANCE.create(strings, shape, order)
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double... array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			If array.length = 0 Then
				Return Nd4j.empty(DataType.DOUBLE)
			End If
			Dim shape() As Long = {array.length}
			Return create(array, shape, ArrayUtil.calcStrides(shape), "c"c, DataType.DOUBLE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single... array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			If array.length = 0 Then
				Return Nd4j.empty(DataType.FLOAT)
			End If
			Dim shape() As Long = {array.length}
			Return create(array, shape, ArrayUtil.calcStrides(shape), "c"c, DataType.FLOAT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer... array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			If array.length = 0 Then
				Return Nd4j.empty(DataType.INT)
			End If
			Dim shape() As Long = {array.length}
			Return create(array, shape, ArrayUtil.calcStrides(shape), "c"c, DataType.INT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short... array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			If array.length = 0 Then
				Return Nd4j.empty(DataType.SHORT)
			End If
			Dim shape() As Long = {array.length}
			Return create(array, shape, ArrayUtil.calcStrides(shape), "c"c, DataType.SHORT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte... array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			If array.length = 0 Then
				Return Nd4j.empty(DataType.BYTE)
			End If
			Dim shape() As Long = {array.length}
			Return create(array, shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BYTE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long... array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			If array.length = 0 Then
				Return Nd4j.empty(DataType.LONG)
			End If
			Dim shape() As Long = {array.length}
			Return create(array, shape, ArrayUtil.calcStrides(shape), "c"c, DataType.LONG)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean... array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			If array.length = 0 Then
				Return Nd4j.empty(DataType.BOOL)
			End If
			Dim shape() As Long = {array.length}
			Return create(array, shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BOOL)
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 Then
				Return Nd4j.empty(DataType.DOUBLE)
			End If
			Dim shape() As Long = {array.length, array(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.DOUBLE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 Then
				Return Nd4j.empty(DataType.FLOAT)
			End If
			Dim shape() As Long = {array.length, array(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.FLOAT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 Then
				Return Nd4j.empty(DataType.LONG)
			End If
			Dim shape() As Long = {array.length, array(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.LONG)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 Then
				Return Nd4j.empty(DataType.INT)
			End If
			Dim shape() As Long = {array.length, array(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.INT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 Then
				Return Nd4j.empty(DataType.SHORT)
			End If
			Dim shape() As Long = {array.length, array(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.SHORT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 Then
				Return Nd4j.empty(DataType.BYTE)
			End If
			Dim shape() As Long = {array.length, array(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BYTE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 Then
				Return Nd4j.empty(DataType.BOOL)
			End If

			Dim shape() As Long = {array.length, array(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BOOL)
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.DOUBLE)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.DOUBLE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.FLOAT)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.FLOAT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.LONG)
			End If

			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.LONG)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.INT)
			End If

			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.INT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.SHORT)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.SHORT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.BYTE)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BYTE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.BOOL)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BOOL)
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double()()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 OrElse array(0)(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.DOUBLE)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.DOUBLE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single()()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 OrElse array(0)(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.FLOAT)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.FLOAT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long()()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 OrElse array(0)(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.LONG)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.LONG)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer()()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 OrElse array(0)(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.INT)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.INT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short()()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 OrElse array(0)(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.SHORT)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.SHORT)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte()()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 OrElse array(0)(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.BYTE)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BYTE)
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean()()()() array)
		If True Then
			Preconditions.checkNotNull(array, "Cannot create INDArray from null Java array")
			ArrayUtil.assertNotRagged(array)
			If array.length = 0 OrElse array(0).length = 0 OrElse array(0)(0).length = 0 OrElse array(0)(0)(0).length = 0 Then
				Return Nd4j.empty(DataType.BOOL)
			End If
			Dim shape() As Long = {array.length, array(0).length, array(0)(0).length, array(0)(0)(0).length}
			Return create(ArrayUtil.flatten(array), shape, ArrayUtil.calcStrides(shape), "c"c, DataType.BOOL)
		End If

		public static synchronized DeallocatorService DeallocatorService
		If True Then
			If deallocatorService_Conflict Is Nothing Then
				deallocatorService_Conflict = New DeallocatorService()
			End If

			Return deallocatorService_Conflict
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double?() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single?() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer?() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short?() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte?() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long?() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 1D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean?() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double?()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single?()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer?()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short?()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte?()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long?()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 2D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean?()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double?()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single?()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer?()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short?()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte?()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long?()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 3D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean?()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

	'/////////////////

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with DOUBLE data type </returns>
		public static INDArray createFromArray(Double?()()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with FLOAT data type </returns>
		public static INDArray createFromArray(Single?()()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT32 data type </returns>
		public static INDArray createFromArray(Integer?()()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT16 data type </returns>
		public static INDArray createFromArray(Short?()()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT8 data type </returns>
		public static INDArray createFromArray(SByte?()()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with INT64 data type </returns>
		public static INDArray createFromArray(Long?()()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		''' <summary>
		''' This method creates INDArray from provided jvm array </summary>
		''' <param name="array"> jvm array </param>
		''' <returns> 4D INDArray with BOOL data type </returns>
		public static INDArray createFromArray(Boolean?()()()() array)
		If True Then
			Return createFromArray(ArrayUtil.toPrimitives(array))
		End If

		public static Boolean ExperimentalMode
		If True Then
			Return Executioner.ExperimentalMode
		End If

		''' <summary>
		''' Execute the operation and return the result
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		public static INDArray exec(Op op)
		If True Then
			Return Executioner.exec(op)
		End If

		public static INDArray exec(Op op, OpContext context)
		If True Then
			Return Executioner.exec(op, context)
		End If



		''' <summary>
		''' Execute the operation and return the result
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		public static INDArray() exec(CustomOp op)
		If True Then
			Return Executioner.exec(op)
		End If

		''' <summary>
		''' Execute the operation and return the result
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		public static INDArray() exec(CustomOp op, OpContext context)
		If True Then
			Return Executioner.exec(op, context)
		End If


		''' <summary>
		''' This method applies ScatterUpdate op
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated public static void scatterUpdate(org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate.UpdateOp op, @NonNull INDArray array, @NonNull INDArray indices, @NonNull INDArray updates, int... axis)
		<Obsolete>
		public static void scatterUpdate(ScatterUpdate.UpdateOp op, INDArray array, INDArray indices, INDArray updates, Integer... axis)
		If True Then
			Preconditions.checkArgument(indices.dataType() = DataType.INT OrElse indices.dataType() = DataType.LONG, "Indices should have INT data type")
			Preconditions.checkArgument(array.dataType() = updates.dataType(), "Array and updates should have the same data type")
			Executioner.scatterUpdate(op, array, indices, updates, axis)
		End If
	End Class

End Namespace