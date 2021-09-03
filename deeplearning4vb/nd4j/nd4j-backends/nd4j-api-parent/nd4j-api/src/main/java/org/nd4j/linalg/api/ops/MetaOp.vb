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

Namespace org.nd4j.linalg.api.ops

	Public Interface MetaOp
		Inherits GridOp

		''' 
		''' <summary>
		''' @return
		''' </summary>
		ReadOnly Property FirstOp As Op

		ReadOnly Property SecondOp As Op

		ReadOnly Property FirstOpDescriptor As OpDescriptor

		ReadOnly Property SecondOpDescriptor As OpDescriptor

		WriteOnly Property FirstPointers As GridPointers

		WriteOnly Property SecondPointers As GridPointers
	End Interface

End Namespace