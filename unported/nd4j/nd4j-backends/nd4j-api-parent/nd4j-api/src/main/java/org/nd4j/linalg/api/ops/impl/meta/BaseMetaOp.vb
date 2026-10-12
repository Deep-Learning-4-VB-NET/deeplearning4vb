Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MetaOp = org.nd4j.linalg.api.ops.MetaOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports GridPointers = org.nd4j.linalg.api.ops.grid.GridPointers
Imports OpDescriptor = org.nd4j.linalg.api.ops.grid.OpDescriptor
Imports BaseGridOp = org.nd4j.linalg.api.ops.impl.grid.BaseGridOp

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

Namespace org.nd4j.linalg.api.ops.impl.meta

	Public MustInherit Class BaseMetaOp
		Inherits BaseGridOp
		Implements MetaOp

		Public Overrides MustOverride WriteOnly Property ExtraArgs As Object()

		Public Sub New()

		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			MyBase.New(x, y)
		End Sub

		Protected Friend Sub New(ByVal opA As Op, ByVal opB As Op)
			MyBase.New(opA, opB)
		End Sub

		Public Overridable ReadOnly Property FirstOpDescriptor As OpDescriptor Implements MetaOp.getFirstOpDescriptor
			Get
				Return queuedOps(0)
			End Get
		End Property

		Public Overridable ReadOnly Property SecondOpDescriptor As OpDescriptor Implements MetaOp.getSecondOpDescriptor
			Get
				Return queuedOps(1)
			End Get
		End Property

		Protected Friend Sub New(ByVal opA As OpDescriptor, ByVal opB As OpDescriptor)
			MyBase.New(opA, opB)
		End Sub

		Protected Friend Sub New(ByVal opA As GridPointers, ByVal opB As GridPointers)
			MyBase.New(opA, opB)
		End Sub

		Public Overridable ReadOnly Property FirstOp As Op
			Get
				Return FirstOpDescriptor.getOp()
			End Get
		End Property

		Public Overridable ReadOnly Property SecondOp As Op
			Get
				Return SecondOpDescriptor.getOp()
			End Get
		End Property

		Public Overridable WriteOnly Property FirstPointers Implements MetaOp.setFirstPointers As GridPointers
			Set(ByVal pointers As GridPointers)
				grid(0) = pointers
			End Set
		End Property

		Public Overridable WriteOnly Property SecondPointers Implements MetaOp.setSecondPointers As GridPointers
			Set(ByVal pointers As GridPointers)
				grid(1) = pointers
			End Set
		End Property
	End Class

End Namespace