Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports LSTMBlockCell = org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlockCell

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
'ORIGINAL LINE: @Getter public class LSTMCellOutputs
	Public Class LSTMCellOutputs

		''' <summary>
		''' Output - input modulation gate activations [batchSize, numUnits].
		''' </summary>
		Private i As SDVariable

		''' <summary>
		''' Activations, cell state (pre tanh) [batchSize, numUnits].
		''' </summary>
		Private c As SDVariable

		''' <summary>
		''' Output - forget gate activations [batchSize, numUnits].
		''' </summary>
		Private f As SDVariable

		''' <summary>
		''' Output - output gate activations [batchSize, numUnits].
		''' </summary>
		Private o As SDVariable

		''' <summary>
		''' Output - input gate activations [batchSize, numUnits].
		''' </summary>
		Private z As SDVariable

		''' <summary>
		''' Cell state, post tanh [batchSize, numUnits].
		''' </summary>
		Private h As SDVariable

		''' <summary>
		''' Current cell output [batchSize, numUnits].
		''' </summary>
		Private y As SDVariable

		Public Sub New(ByVal outputs() As SDVariable)
			Preconditions.checkArgument(outputs.Length = 7, "Must have 7 LSTM cell outputs, got %s", outputs.Length)

			i = outputs(0)
			c = outputs(1)
			f = outputs(2)
			o = outputs(3)
			z = outputs(4)
			h = outputs(5)
			y = outputs(6)
		End Sub

		''' <summary>
		''' Get all outputs returned by the cell.
		''' </summary>
		Public Overridable ReadOnly Property AllOutputs As IList(Of SDVariable)
			Get
				Return New List(Of SDVariable) From {i, c, f, o, z, h, y}
			End Get
		End Property

		''' <summary>
		''' Get y, the output of the cell.
		''' 
		''' Has shape [batchSize, numUnits].
		''' </summary>
		Public Overridable ReadOnly Property Output As SDVariable
			Get
				Return y
			End Get
		End Property

		''' <summary>
		''' Get c, the cell's state.
		''' 
		''' Has shape [batchSize, numUnits].
		''' </summary>
		Public Overridable ReadOnly Property State As SDVariable
			Get
				Return c
			End Get
		End Property
	End Class

End Namespace