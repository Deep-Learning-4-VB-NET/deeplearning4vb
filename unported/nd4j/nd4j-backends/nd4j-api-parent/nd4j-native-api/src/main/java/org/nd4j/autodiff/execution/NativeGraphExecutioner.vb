Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports ExecutionMode = org.nd4j.autodiff.execution.conf.ExecutionMode
Imports ExecutorConfiguration = org.nd4j.autodiff.execution.conf.ExecutorConfiguration
Imports OutputMode = org.nd4j.autodiff.execution.conf.OutputMode
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports FlatArray = org.nd4j.graph.FlatArray
Imports FlatResult = org.nd4j.graph.FlatResult
Imports FlatVariable = org.nd4j.graph.FlatVariable
Imports OpType = org.nd4j.graph.OpType
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports OpaqueResultWrapper = org.nd4j.nativeblas.OpaqueResultWrapper

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

Namespace org.nd4j.autodiff.execution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NativeGraphExecutioner implements GraphExecutioner
	Public Class NativeGraphExecutioner
		Implements GraphExecutioner

		Public Overridable ReadOnly Property ExecutionerType As Type Implements GraphExecutioner.getExecutionerType
			Get
				Return Type.LOCAL
			End Get
		End Property


		''' <summary>
		''' This method executes given graph and returns results
		''' 
		''' PLEASE NOTE: Default configuration is used
		''' </summary>
		''' <param name="sd">
		''' @return </param>
		Public Overridable Function executeGraph(ByVal sd As SameDiff) As INDArray() Implements GraphExecutioner.executeGraph
			Return executeGraph(sd, ExecutorConfiguration.builder().outputMode(OutputMode.IMPLICIT).executionMode(ExecutionMode.SEQUENTIAL).profilingMode(OpExecutioner.ProfilingMode.DISABLED).build())
		End Function

		Public Overridable Function reuseGraph(ByVal graph As SameDiff, ByVal inputs As IDictionary(Of Integer, INDArray)) As INDArray() Implements GraphExecutioner.reuseGraph
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function convertToFlatBuffers(ByVal sd As SameDiff, ByVal configuration As ExecutorConfiguration) As ByteBuffer Implements GraphExecutioner.convertToFlatBuffers
			Return sd.asFlatBuffers(configuration, True)
		End Function

		''' <summary>
		''' This method executes given graph and returns results
		''' </summary>
		''' <param name="sd">
		''' @return </param>
		Public Overridable Function executeGraph(ByVal sd As SameDiff, ByVal configuration As ExecutorConfiguration) As INDArray() Implements GraphExecutioner.executeGraph

			Dim buffer As ByteBuffer = convertToFlatBuffers(sd, configuration)

			Dim bPtr As New BytePointer(buffer)

			log.info("Buffer length: {}", buffer.limit())

			Dim nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
			Dim res As OpaqueResultWrapper = nativeOps.executeFlatGraph(Nothing, bPtr)
			If res Is Nothing Then
				Throw New ND4JIllegalStateException("Graph execution failed")
			End If

			Dim pagedPointer As New PagedPointer(nativeOps.getResultWrapperPointer(res), nativeOps.getResultWrapperSize(res))
			Dim fr As FlatResult = FlatResult.getRootAsFlatResult(pagedPointer.asBytePointer().asByteBuffer())

			log.info("VarMap: {}", sd.variableMap())

			Dim results(fr.variablesLength() - 1) As INDArray

			Dim e As Integer = 0
			Do While e < fr.variablesLength()
				Dim var As FlatVariable = fr.variables(e)
				Dim varName As String = var.name()
	'            log.info("Var received: id: [{}:{}/<{}>];", var.id().first(), var.id().second(), var.name());
				Dim ndarray As FlatArray = var.ndarray()

				Dim val As INDArray = Nd4j.createFromFlatArray(ndarray)
				results(e) = val

				If var.name() IsNot Nothing AndAlso sd.variableMap().ContainsKey(var.name()) Then
					If sd.getVariable(varName).getVariableType() <> VariableType.ARRAY Then
						sd.associateArrayWithVariable(val, sd.variableMap()(var.name()))
					End If
				Else
					If sd.variableMap()(var.name()) IsNot Nothing Then
						sd.associateArrayWithVariable(val,sd.getVariable(var.name()))
					Else
						log.warn("Unknown variable received: [{}]", var.name())
					End If
				End If
				e += 1
			Loop

			' now we need to release native memory
			nativeOps.deleteResultWrapper(res)

			Return results
		End Function



		Public Shared Function getOpNum(ByVal name As String, ByVal type As Op.Type) As Long
			If type = Op.Type.CUSTOM Then
				Return Nd4j.Executioner.getCustomOperations()(name.ToLower()).getHash()
			Else
				Try
					Dim op As DifferentialFunction = DifferentialFunctionClassHolder.Instance.getInstance(name)
					Return op.opNum()
				Catch e As Exception
					Throw New Exception("Could not find op number for operation: [" & name & "]",e)
				End Try
			End If
		End Function

		Public Shared Function getFlatOpType(ByVal type As Op.Type) As SByte
			Select Case type
				Case Op.Type.SCALAR
					Return OpType.SCALAR
				Case Op.Type.BROADCAST
					Return OpType.BROADCAST
				Case Op.Type.TRANSFORM_FLOAT
					Return OpType.TRANSFORM_FLOAT
				Case Op.Type.TRANSFORM_SAME
					Return OpType.TRANSFORM_SAME
				Case Op.Type.TRANSFORM_STRICT
					Return OpType.TRANSFORM_STRICT
				Case Op.Type.TRANSFORM_BOOL
					Return OpType.TRANSFORM_BOOL
				Case Op.Type.REDUCE_FLOAT
					Return OpType.REDUCE_FLOAT
				Case Op.Type.REDUCE_BOOL
					Return OpType.REDUCE_BOOL
				Case Op.Type.REDUCE_SAME
					Return OpType.REDUCE_SAME
				Case Op.Type.INDEXREDUCE
					Return OpType.INDEX_REDUCE
				Case Op.Type.CUSTOM
					Return OpType.CUSTOM
				Case Else
					Throw New System.NotSupportedException("Unknown op type passed in: " & type)
			End Select
		End Function

		''' <summary>
		''' This method executes
		''' </summary>
		''' <param name="id"> </param>
		''' <param name="variables">
		''' @return </param>
		Public Overridable Function executeGraph(ByVal id As Integer, ParamArray ByVal variables() As SDVariable) As INDArray() Implements GraphExecutioner.executeGraph
			Return New INDArray(){}
		End Function

		''' <summary>
		''' This method stores given graph for future execution
		''' </summary>
		''' <param name="graph">
		''' @return </param>
		Public Overridable Function registerGraph(ByVal graph As SameDiff) As Integer Implements GraphExecutioner.registerGraph
			Return 0
		End Function


		Public Overridable Function importProto(ByVal file As File) As INDArray() Implements GraphExecutioner.importProto
			' TODO: to be implemented
			Throw New System.NotSupportedException("Not implemented yet")
		End Function
	End Class

End Namespace