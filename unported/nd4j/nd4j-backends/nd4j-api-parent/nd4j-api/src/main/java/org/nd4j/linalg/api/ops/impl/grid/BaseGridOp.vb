Imports System.Collections.Generic
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseOp = org.nd4j.linalg.api.ops.BaseOp
Imports GridOp = org.nd4j.linalg.api.ops.GridOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports GridDescriptor = org.nd4j.linalg.api.ops.grid.GridDescriptor
Imports GridPointers = org.nd4j.linalg.api.ops.grid.GridPointers
Imports OpDescriptor = org.nd4j.linalg.api.ops.grid.OpDescriptor

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

Namespace org.nd4j.linalg.api.ops.impl.grid


	Public MustInherit Class BaseGridOp
		Inherits BaseOp
		Implements GridOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs As Object()
		Protected Friend queuedOps As IList(Of OpDescriptor) = New List(Of OpDescriptor)()
		Protected Friend grid As IList(Of GridPointers) = New List(Of GridPointers)()

		Public Sub New()

		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			' no-op
		End Sub

		Protected Friend Sub New(ParamArray ByVal ops() As Op)
			grid = New List(Of GridPointers)(ops.Length)
			For Each op As Op In ops
				queuedOps.Add(New OpDescriptor(op, Nothing))
				grid.Add(Nothing)
			Next op
		End Sub

		Protected Friend Sub New(ParamArray ByVal descriptors() As OpDescriptor)
			For Each op As OpDescriptor In descriptors
				queuedOps.Add(op)
				grid.Add(Nothing)
			Next op
		End Sub

		Protected Friend Sub New(ParamArray ByVal pointers() As GridPointers)
			For Each ptr As GridPointers In pointers
				grid.Add(ptr)
			Next ptr
		End Sub

		Protected Friend Sub New(ByVal ops As IList(Of Op))
			Me.New(CType(ops, List(Of Op)).ToArray())
		End Sub


		Public Overridable ReadOnly Property GridDescriptor As GridDescriptor Implements GridOp.getGridDescriptor
			Get
				Dim descriptor As New GridDescriptor()
				descriptor.setGridDepth(grid.Count)
				descriptor.setGridPointers(grid)
				Return descriptor
			End Get
		End Property


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function
	End Class

End Namespace