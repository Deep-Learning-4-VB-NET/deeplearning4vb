Imports System
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports org.bytedeco.javacpp
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Deallocatable = org.nd4j.linalg.api.memory.Deallocatable
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseOpContext = org.nd4j.linalg.api.ops.BaseOpContext
Imports ExecutionMode = org.nd4j.linalg.api.ops.ExecutionMode
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports BaseCpuDataBuffer = org.nd4j.linalg.cpu.nativecpu.buffer.BaseCpuDataBuffer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports OpaqueContext = org.nd4j.nativeblas.OpaqueContext
Imports OpaqueRandomGenerator = org.nd4j.nativeblas.OpaqueRandomGenerator

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

Namespace org.nd4j.linalg.cpu.nativecpu.ops

	Public Class CpuOpContext
		Inherits BaseOpContext
		Implements OpContext, Deallocatable

		' we might want to have configurable
		Private nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Private context As OpaqueContext = nativeOps.createGraphContext(1)
		<NonSerialized>
		Private ReadOnly id As Long = Nd4j.DeallocatorService.nextValue()

		Public Sub New()
			Nd4j.DeallocatorService.pickObject(Me)
		End Sub

		Public Overrides Sub close()
			' no-op
		End Sub

		Public Overrides WriteOnly Property IArguments Implements OpContext.setIArguments As Long()
			Set(ByVal arguments() As Long)
				If arguments.Length > 0 Then
					MyBase.IArguments = arguments
					nativeOps.setGraphContextIArguments(context, New LongPointer(arguments), arguments.Length)
				End If
			End Set
		End Property

		Public Overrides WriteOnly Property BArguments Implements OpContext.setBArguments As Boolean()
			Set(ByVal arguments() As Boolean)
				If arguments.Length > 0 Then
					MyBase.BArguments = arguments
					nativeOps.setGraphContextBArguments(context, New BooleanPointer(arguments), arguments.Length)
				End If
			End Set
		End Property

		Public Overrides WriteOnly Property TArguments Implements OpContext.setTArguments As Double()
			Set(ByVal arguments() As Double)
				If arguments.Length > 0 Then
					MyBase.TArguments = arguments
					nativeOps.setGraphContextTArguments(context, New DoublePointer(arguments), arguments.Length)
				End If
			End Set
		End Property

		Public Overrides WriteOnly Property DArguments Implements OpContext.setDArguments As DataType()
			Set(ByVal arguments() As DataType)
				If arguments.Length > 0 Then
					MyBase.DArguments = arguments
					Dim args As val = New Integer(arguments.Length - 1){}
					For e As Integer = 0 To arguments.Length - 1
						args(e) = arguments(e).toInt()
					Next e
    
					nativeOps.setGraphContextDArguments(context, New IntPointer(args), arguments.Length)
				End If
			End Set
		End Property

		Public Overrides Sub setRngStates(ByVal rootState As Long, ByVal nodeState As Long) Implements OpContext.setRngStates
			nativeOps.setRandomGeneratorStates(nativeOps.getGraphContextRandomGenerator(context), rootState, nodeState)
		End Sub

		Public Overrides ReadOnly Property RngStates As Pair(Of Long, Long) Implements OpContext.getRngStates
			Get
				Dim g As OpaqueRandomGenerator = nativeOps.getGraphContextRandomGenerator(context)
				Return Pair.makePair(nativeOps.getRandomGeneratorRootState(g), nativeOps.getRandomGeneratorNodeState(g))
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setInputArray(int index, @NonNull INDArray array)
		Public Overridable Overloads Sub setInputArray(ByVal index As Integer, ByVal array As INDArray)
			'nativeOps.setGraphContextInputArray(context, index, array.isEmpty() ? null : array.data().addressPointer(), array.shapeInfoDataBuffer().addressPointer(), null, null);
			nativeOps.setGraphContextInputBuffer(context, index,If(array.isEmpty(), Nothing, CType(array.data(), BaseCpuDataBuffer).OpaqueDataBuffer), array.shapeInfoDataBuffer().addressPointer(), Nothing)

			MyBase.setInputArray(index, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setOutputArray(int index, @NonNull INDArray array)
		Public Overridable Overloads Sub setOutputArray(ByVal index As Integer, ByVal array As INDArray)
			'nativeOps.setGraphContextOutputArray(context, index, array.isEmpty() ? null : array.data().addressPointer(), array.shapeInfoDataBuffer().addressPointer(), null, null);
			nativeOps.setGraphContextOutputBuffer(context, index,If(array.isEmpty(), Nothing, CType(array.data(), BaseCpuDataBuffer).OpaqueDataBuffer), array.shapeInfoDataBuffer().addressPointer(), Nothing)

			MyBase.setOutputArray(index, array)
		End Sub

		Public Overrides Function contextPointer() As Pointer
			Return context
		End Function

		Public Overrides Sub markInplace(ByVal reallyInplace As Boolean) Implements OpContext.markInplace
			nativeOps.markGraphContextInplace(context, reallyInplace)
		End Sub

		Public Overrides Sub allowHelpers(ByVal reallyAllow As Boolean) Implements OpContext.allowHelpers
			nativeOps.ctxAllowHelpers(context, reallyAllow)
		End Sub

		Public Overrides Sub shapeFunctionOverride(ByVal reallyOverride As Boolean) Implements OpContext.shapeFunctionOverride
			nativeOps.ctxShapeFunctionOverride(context, reallyOverride)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setExecutionMode(@NonNull ExecutionMode mode)
		Public Overridable Overloads WriteOnly Property ExecutionMode Implements OpContext.setExecutionMode As ExecutionMode
			Set(ByVal mode As ExecutionMode)
				MyBase.ExecutionMode = mode
				nativeOps.ctxSetExecutionMode(context, mode.ordinal())
			End Set
		End Property

		Public Overrides Sub purge() Implements OpContext.purge
			MyBase.purge()
			nativeOps.ctxPurge(context)
		End Sub

		Public Overridable ReadOnly Property UniqueId As String Implements Deallocatable.getUniqueId
			Get
				Return "CTX_" & id
			End Get
		End Property

		Public Overridable Function deallocator() As Deallocator
			Return New CpuOpContextDeallocator(Me)
		End Function

		Public Overridable Function targetDevice() As Integer Implements Deallocatable.targetDevice
			Return 0
		End Function
	End Class

End Namespace