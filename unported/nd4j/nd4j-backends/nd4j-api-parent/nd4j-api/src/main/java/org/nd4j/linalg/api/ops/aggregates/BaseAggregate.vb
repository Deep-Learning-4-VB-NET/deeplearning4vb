Imports System.Collections.Generic
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.ops.aggregates


	Public MustInherit Class BaseAggregate
		Implements Aggregate

		Public MustOverride ReadOnly Property ThreadsPerInstance As Integer Implements Aggregate.getThreadsPerInstance
		Public MustOverride ReadOnly Property SharedMemorySize As Integer Implements Aggregate.getSharedMemorySize
		Public MustOverride Function maxRealArguments() As Integer Implements Aggregate.maxRealArguments
		Public MustOverride Function maxIndexArguments() As Integer Implements Aggregate.maxIndexArguments
		Public MustOverride Function maxIntArraySize() As Integer Implements Aggregate.maxIntArraySize
		Public MustOverride Function maxIntArrays() As Integer Implements Aggregate.maxIntArrays
		Public MustOverride Function maxShapes() As Integer Implements Aggregate.maxShapes
		Public MustOverride Function maxArguments() As Integer Implements Aggregate.maxArguments
		Public MustOverride Function opNum() As Integer Implements Aggregate.opNum
		Public MustOverride Function name() As String Implements Aggregate.name
		Protected Friend arguments As IList(Of INDArray) = New List(Of INDArray)()
		Protected Friend shapes As IList(Of DataBuffer) = New List(Of DataBuffer)()
		Protected Friend intArrayArguments As IList(Of Integer()) = New List(Of Integer())()
		Protected Friend indexingArguments As IList(Of Integer) = New List(Of Integer)()
		Protected Friend realArguments As IList(Of Number) = New List(Of Number)()

'JAVA TO VB CONVERTER NOTE: The field finalResult was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend finalResult_Conflict As Number = 0.0

		Public Overridable ReadOnly Property Arguments As IList(Of INDArray) Implements Aggregate.getArguments
			Get
				Return arguments
			End Get
		End Property

		Public Overridable Property FinalResult As Number Implements Aggregate.getFinalResult
			Get
				Return finalResult_Conflict
			End Get
			Set(ByVal result As Number)
				Me.finalResult_Conflict = result
			End Set
		End Property


		Public Overridable ReadOnly Property Shapes As IList(Of DataBuffer) Implements Aggregate.getShapes
			Get
				Return shapes
			End Get
		End Property

		Public Overridable ReadOnly Property IndexingArguments As IList(Of Integer) Implements Aggregate.getIndexingArguments
			Get
				Return indexingArguments
			End Get
		End Property

		Public Overridable ReadOnly Property RealArguments As IList(Of Number) Implements Aggregate.getRealArguments
			Get
				Return realArguments
			End Get
		End Property

		Public Overridable ReadOnly Property IntArrayArguments As IList(Of Integer()) Implements Aggregate.getIntArrayArguments
			Get
				Return intArrayArguments
			End Get
		End Property

		Public Overridable ReadOnly Property RequiredBatchMemorySize As Long Implements Aggregate.getRequiredBatchMemorySize
			Get
				Dim result As Long = maxIntArrays() * maxIntArraySize() * 4
				result += maxArguments() * 8 ' pointers
				result += maxShapes() * 8 ' pointers
				result += maxIndexArguments() * 4
				result += maxRealArguments() * (If(Nd4j.dataType() = DataType.DOUBLE, 8, If(Nd4j.dataType() = DataType.FLOAT, 4, 2)))
				result += 5 * 4 ' numArgs
    
				Return result * Batch.getBatchLimit()
			End Get
		End Property
	End Class

End Namespace