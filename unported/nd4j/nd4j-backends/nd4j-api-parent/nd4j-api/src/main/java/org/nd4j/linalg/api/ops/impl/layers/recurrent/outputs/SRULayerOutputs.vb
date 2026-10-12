Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports SDIndex = org.nd4j.autodiff.samediff.SDIndex
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
'ORIGINAL LINE: @Getter public class SRULayerOutputs
	Public Class SRULayerOutputs


		''' <summary>
		''' Current cell output [batchSize, inSize, timeSeriesLength].
		''' </summary>
		Private h As SDVariable

		''' <summary>
		''' Current cell state [batchSize, inSize, timeSeriesLength].
		''' </summary>
		Private c As SDVariable

		Public Sub New(ByVal outputs() As SDVariable)
			Preconditions.checkArgument(outputs.Length = 2, "Must have 2 SRU cell outputs, got %s", outputs.Length)

			h = outputs(0)
			c = outputs(1)
		End Sub

		''' <summary>
		''' Get all outputs returned by the cell.
		''' </summary>
		Public Overridable ReadOnly Property AllOutputs As IList(Of SDVariable)
			Get
				Return New List(Of SDVariable) From {h, c}
			End Get
		End Property

		''' <summary>
		''' Get h, the output of the cell.
		''' 
		''' Has shape [batchSize, inSize, timeSeriesLength].
		''' </summary>
		Public Overridable ReadOnly Property Output As SDVariable
			Get
				Return h
			End Get
		End Property

		''' <summary>
		''' Get c, the state of the cell.
		''' 
		''' Has shape [batchSize, inSize, timeSeriesLength].
		''' </summary>
		Public Overridable ReadOnly Property State As SDVariable
			Get
				Return c
			End Get
		End Property

'JAVA TO VB CONVERTER NOTE: The field lastOutput was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastOutput_Conflict As SDVariable = Nothing

		''' <summary>
		''' Get y, the output of the cell, for the last time step.
		''' 
		''' Has shape [batchSize, inSize].
		''' </summary>
		Public Overridable ReadOnly Property LastOutput As SDVariable
			Get
				If lastOutput_Conflict IsNot Nothing Then
					Return lastOutput_Conflict
				End If
    
				lastOutput_Conflict = Output.get(SDIndex.all(), SDIndex.all(), SDIndex.point(-1))
				Return lastOutput_Conflict
			End Get
		End Property

'JAVA TO VB CONVERTER NOTE: The field lastState was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastState_Conflict As SDVariable = Nothing

		''' <summary>
		''' Get c, the state of the cell, for the last time step.
		''' 
		''' Has shape [batchSize, inSize].
		''' </summary>
		Public Overridable ReadOnly Property LastState As SDVariable
			Get
				If lastState_Conflict IsNot Nothing Then
					Return lastState_Conflict
				End If
    
				lastOutput_Conflict = Output.get(SDIndex.all(), SDIndex.all(), SDIndex.point(-1))
				Return lastState_Conflict
			End Get
		End Property

	End Class

End Namespace