Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayStatistics = org.nd4j.linalg.api.ndarray.INDArrayStatistics
Imports org.nd4j.linalg.api.ops
Imports Aggregate = org.nd4j.linalg.api.ops.aggregates.Aggregate
Imports org.nd4j.linalg.api.ops.aggregates
Imports ScatterUpdate = org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports Random = org.nd4j.linalg.api.rng.Random
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports TadPack = org.nd4j.linalg.api.shape.TadPack
Imports TADManager = org.nd4j.linalg.cache.TADManager
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig

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

Namespace org.nd4j.linalg.api.ops.executioner


	Public Interface OpExecutioner

		' in case of adding new executioner - list it here
		Friend Enum ExecutionerType
			NATIVE_CPU
			CUDA
		End Enum

		Friend Enum ProfilingMode
			DISABLED
			NAN_PANIC
			INF_PANIC
			ANY_PANIC
			OPERATIONS
			METHODS
			ALL
			SCOPE_PANIC
			BANDWIDTH
		End Enum

		''' <summary>
		''' This method returns true if verbose mode is enabled, false otherwise
		''' @return
		''' </summary>
		ReadOnly Property Verbose As Boolean

		''' <summary>
		''' This method returns true if debug mode is enabled, false otherwise
		''' @return
		''' </summary>
		ReadOnly Property Debug As Boolean


		''' <summary>
		''' This method returns type for this executioner instance
		''' @return
		''' </summary>
		Function type() As ExecutionerType


		''' <summary>
		''' This method returns opName of the last invoked op
		''' 
		''' @return
		''' </summary>
		ReadOnly Property LastOp As String


		''' <summary>
		''' Execute the operation
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		Function exec(ByVal op As Op) As INDArray

		''' <summary>
		''' Execute the operation
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		Function exec(ByVal op As Op, ByVal opContext As OpContext) As INDArray

		''' <summary>
		'''Execute a TransformOp and return the result </summary>
		''' <param name="op"> the operation to execute </param>
		Function execAndReturn(ByVal op As TransformOp) As TransformOp

		''' <summary>
		''' Execute and return the result from an accumulation
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		''' <returns> the accumulated result </returns>
		Function execAndReturn(ByVal op As ReduceOp) As ReduceOp

		''' <summary>
		''' Execute and return the result from an accumulation
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		''' <returns> the accumulated result </returns>
		Function execAndReturn(ByVal op As Variance) As Variance

		''' <summary>
		'''Execute and return the result from an index accumulation </summary>
		''' <param name="op"> the index accumulation operation to execute </param>
		''' <returns> the accumulated index </returns>
		Function execAndReturn(ByVal op As IndexAccumulation) As IndexAccumulation

		''' <summary>
		'''Execute and return the result from a scalar op
		''' </summary>
		''' <param name="op"> the operation to execute </param>
		''' <returns> the accumulated result </returns>
		Function execAndReturn(ByVal op As ScalarOp) As ScalarOp

		''' <summary>
		''' Execute and return the result from a vector op </summary>
		''' <param name="op"> </param>
		Function execAndReturn(ByVal op As BroadcastOp) As BroadcastOp

		''' <summary>
		''' Execute a reduceOp, possibly along one or more dimensions </summary>
		''' <param name="reduceOp"> the reduceOp </param>
		''' <returns> the reduceOp op </returns>
		Function exec(ByVal reduceOp As ReduceOp) As INDArray

		''' <summary>
		''' Execute a broadcast op, possibly along one or more dimensions </summary>
		''' <param name="broadcast"> the accumulation </param>
		''' <returns> the broadcast op </returns>
		Function exec(ByVal broadcast As BroadcastOp) As INDArray

		''' <summary>
		''' Execute ScalarOp </summary>
		''' <param name="broadcast">
		''' @return </param>
		Function exec(ByVal broadcast As ScalarOp) As INDArray

		''' <summary>
		''' Execute an variance accumulation op, possibly along one or more dimensions </summary>
		''' <param name="accumulation"> the accumulation </param>
		''' <returns> the accmulation op </returns>
		Function exec(ByVal accumulation As Variance) As INDArray


		''' <summary>
		''' Execute an index accumulation along one or more dimensions </summary>
		''' <param name="indexAccum"> the index accumulation operation </param>
		''' <returns> result </returns>
		Function exec(ByVal indexAccum As IndexAccumulation) As INDArray



		''' 
		''' <summary>
		''' Execute and return  a result
		''' ndarray from the given op </summary>
		''' <param name="op"> the operation to execute </param>
		''' <returns> the result from the operation </returns>
		Function execAndReturn(ByVal op As Op) As Op

		''' <summary>
		''' Execute MetaOp
		''' </summary>
		''' <param name="op"> </param>
		Sub exec(ByVal op As MetaOp)

		''' <summary>
		''' Execute GridOp </summary>
		''' <param name="op"> </param>
		Sub exec(ByVal op As GridOp)

		''' 
		''' <param name="op"> </param>
		Sub exec(ByVal op As Aggregate)

		''' <summary>
		''' This method executes previously built batch
		''' </summary>
		''' <param name="batch"> </param>
		 Sub exec(Of T As Aggregate)(ByVal batch As Batch(Of T))

		''' <summary>
		''' This method takes arbitrary sized list of aggregates,
		''' and packs them into batches
		''' </summary>
		''' <param name="batch"> </param>
		Sub exec(ByVal batch As IList(Of Aggregate))

		''' <summary>
		''' This method executes specified RandomOp using default RNG available via Nd4j.getRandom()
		''' </summary>
		''' <param name="op"> </param>
		Function exec(ByVal op As RandomOp) As INDArray

		''' <summary>
		''' This method executes specific RandomOp against specified RNG
		''' </summary>
		''' <param name="op"> </param>
		''' <param name="rng"> </param>
		Function exec(ByVal op As RandomOp, ByVal rng As Random) As INDArray

		''' <summary>
		''' This method return set of key/value and
		''' key/key/value objects,
		''' describing current environment
		''' 
		''' @return
		''' </summary>
		ReadOnly Property EnvironmentInformation As Properties

		''' <summary>
		''' This method specifies desired profiling mode
		''' </summary>
		''' <param name="mode"> </param>
		<Obsolete>
		Property ProfilingMode As ProfilingMode

		''' <summary>
		''' This method stores specified configuration.
		''' </summary>
		''' <param name="config"> </param>
		WriteOnly Property ProfilingConfig As ProfilerConfig



		''' <summary>
		''' This method returns TADManager instance used for this OpExecutioner
		''' 
		''' @return
		''' </summary>
		ReadOnly Property TADManager As TADManager


		''' <summary>
		''' This method prints out environmental information returned by getEnvironmentInformation() method
		''' </summary>
		Sub printEnvironmentInformation()

		''' <summary>
		''' This method ensures all operations that supposed to be executed at this moment, are executed.
		''' </summary>
		Sub push()

		''' <summary>
		''' This method ensures all operations that supposed to be executed at this moment, are executed and finished.
		''' </summary>
		Sub commit()

		''' <summary>
		''' This method encodes array as thresholds, updating input array at the same time
		''' </summary>
		''' <param name="input"> </param>
		''' <returns> encoded array is returned </returns>
		Function thresholdEncode(ByVal input As INDArray, ByVal threshold As Double) As INDArray


		''' <summary>
		''' This method encodes array as thresholds, updating input array at the same time
		''' </summary>
		''' <param name="input"> </param>
		''' <returns> encoded array is returned </returns>
		Function thresholdEncode(ByVal input As INDArray, ByVal threshold As Double, ByVal boundary As Integer?) As INDArray

		''' <summary>
		''' This method decodes thresholds array, and puts it into target array
		''' </summary>
		''' <param name="encoded"> </param>
		''' <param name="target"> </param>
		''' <returns> target is returned </returns>
		Function thresholdDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray

		''' <summary>
		''' This method returns number of elements affected by encoder </summary>
		''' <param name="indArray"> </param>
		''' <param name="target"> </param>
		''' <param name="threshold">
		''' @return </param>
		Function bitmapEncode(ByVal indArray As INDArray, ByVal target As INDArray, ByVal threshold As Double) As Long

		''' 
		''' <param name="indArray"> </param>
		''' <param name="threshold">
		''' @return </param>
		Function bitmapEncode(ByVal indArray As INDArray, ByVal threshold As Double) As INDArray

		''' 
		''' <param name="encoded"> </param>
		''' <param name="target">
		''' @return </param>
		Function bitmapDecode(ByVal encoded As INDArray, ByVal target As INDArray) As INDArray

		''' <summary>
		''' This method returns names of all custom operations available in current backend, and their number of input/output arguments
		''' @return
		''' </summary>
		ReadOnly Property CustomOperations As IDictionary(Of String, CustomOpDescriptor)

		''' <summary>
		''' This method executes given CustomOp
		''' 
		''' PLEASE NOTE: You're responsible for input/output validation </summary>
		''' <param name="op"> </param>
		Function execAndReturn(ByVal op As CustomOp) As CustomOp

		Function exec(ByVal op As CustomOp) As INDArray()

		''' <summary>
		''' This method executes op with given context </summary>
		''' <param name="op"> </param>
		''' <param name="context"> </param>
		''' <returns> method returns output arrays defined within context </returns>
		Function exec(ByVal op As CustomOp, ByVal context As OpContext) As INDArray()

		Function calculateOutputShape(ByVal op As CustomOp) As IList(Of LongShapeDescriptor)

		Function calculateOutputShape(ByVal op As CustomOp, ByVal opContext As OpContext) As IList(Of LongShapeDescriptor)

		''' <summary>
		''' Equivalent to calli
		''' </summary>
		Function allocateOutputArrays(ByVal op As CustomOp) As INDArray()


		Sub enableDebugMode(ByVal reallyEnable As Boolean)

		Sub enableVerboseMode(ByVal reallyEnable As Boolean)

		ReadOnly Property ExperimentalMode As Boolean

		Sub registerGraph(ByVal id As Long, ByVal graph As Pointer)

		Function executeGraph(ByVal id As Long, ByVal map As IDictionary(Of String, INDArray), ByVal reverseMap As IDictionary(Of String, Integer)) As IDictionary(Of String, INDArray)

		Sub forgetGraph(ByVal id As Long)

		''' <summary>
		''' This method allows to set desired number of elements per thread, for performance optimization purposes.
		''' I.e. if array contains 2048 elements, and threshold is set to 1024, 2 threads will be used for given op execution.
		''' 
		''' Default value: 1024
		''' </summary>
		''' <param name="threshold"> </param>
		WriteOnly Property ElementsThreshold As Integer

		''' <summary>
		''' This method allows to set desired number of sub-arrays per thread, for performance optimization purposes.
		''' I.e. if matrix has shape of 64 x 128, and threshold is set to 8, each thread will be processing 8 sub-arrays (sure, if you have 8 core cpu).
		''' If your cpu has, say, 4, cores, only 4 threads will be spawned, and each will process 16 sub-arrays
		''' 
		''' Default value: 8 </summary>
		''' <param name="threshold"> </param>
		WriteOnly Property TadThreshold As Integer

		''' <summary>
		''' This method extracts String from Utf8Buffer </summary>
		''' <param name="buffer"> </param>
		''' <param name="index">
		''' @return </param>
		Function getString(ByVal buffer As DataBuffer, ByVal index As Long) As String

		''' <summary>
		''' Temporary hook </summary>
		''' <param name="op"> </param>
		''' <param name="array"> </param>
		''' <param name="indices"> </param>
		''' <param name="updates"> </param>
		''' <param name="axis"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated void scatterUpdate(org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate.UpdateOp op, @NonNull INDArray array, @NonNull INDArray indices, @NonNull INDArray updates, int[] axis);
		<Obsolete>
		Sub scatterUpdate(ByVal op As ScatterUpdate.UpdateOp, ByVal array As INDArray, ByVal indices As INDArray, ByVal updates As INDArray, ByVal axis() As Integer)

		''' <summary>
		''' This method returns OpContext which can be used (and reused) to execute custom ops
		''' @return
		''' </summary>
		Function buildContext() As OpContext

		''' 
		''' <param name="array"> </param>
		Function inspectArray(ByVal array As INDArray) As INDArrayStatistics


		''' <summary>
		''' This method returns shapeInfo DataBuffer
		''' </summary>
		''' <param name="shape"> </param>
		''' <param name="stride"> </param>
		''' <param name="elementWiseStride"> </param>
		''' <param name="order"> </param>
		''' <param name="dtype">
		''' @return </param>
		Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal empty As Boolean) As DataBuffer

		Function createShapeInfo(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dtype As DataType, ByVal extra As Long) As DataBuffer

		''' <summary>
		''' This method returns host/device tad buffers
		''' </summary>
		Function tadShapeInfoAndOffsets(ByVal array As INDArray, ByVal dimension() As Integer) As TadPack

		''' <summary>
		''' This method returns constant buffer for the given jvm array </summary>
		''' <param name="values">
		''' @return </param>
		Function createConstantBuffer(ByVal values() As Long, ByVal desiredType As DataType) As DataBuffer
		Function createConstantBuffer(ByVal values() As Integer, ByVal desiredType As DataType) As DataBuffer
		Function createConstantBuffer(ByVal values() As Single, ByVal desiredType As DataType) As DataBuffer
		Function createConstantBuffer(ByVal values() As Double, ByVal desiredType As DataType) As DataBuffer


	End Interface

End Namespace