Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports GRUCell = org.nd4j.linalg.api.ops.impl.layers.recurrent.GRUCell

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent.outputs

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class GRUCellOutputs
	Public Class GRUCellOutputs

		''' <summary>
		''' Reset gate output [batchSize, numUnits].
		''' </summary>
		Private r As SDVariable

		''' <summary>
		''' Update gate output [batchSize, numUnits].
		''' </summary>
		Private u As SDVariable

		''' <summary>
		''' Cell gate output [batchSize, numUnits].
		''' </summary>
		Private c As SDVariable

		''' <summary>
		''' Current cell output [batchSize, numUnits].
		''' </summary>
		Private h As SDVariable

		Public Sub New(ByVal outputs() As SDVariable)
			Preconditions.checkArgument(outputs.Length = 4, "Must have 4 GRU cell outputs, got %s", outputs.Length)

			r = outputs(0)
			u = outputs(1)
			c = outputs(2)
			h = outputs(3)
		End Sub

		''' <summary>
		''' Get all outputs returned by the cell.
		''' </summary>
		Public Overridable ReadOnly Property AllOutputs As IList(Of SDVariable)
			Get
				Return New List(Of SDVariable) From {r, u, c, h}
			End Get
		End Property

		''' <summary>
		''' Get h, the output of the cell.
		''' 
		''' Has shape [batchSize, numUnits].
		''' </summary>
		Public Overridable ReadOnly Property Output As SDVariable
			Get
				Return h
			End Get
		End Property

	End Class

End Namespace