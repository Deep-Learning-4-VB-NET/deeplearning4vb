Imports System
Imports System.Collections.Generic
#If SAMEDIFF Then
Imports SDIndex = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDIndex
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent.outputs

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.outputs.SRULayerOutputs</c>.</summary>
    Public Class SRULayerOutputs

#If SAMEDIFF Then
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
#End If

        ' ---------------------------------------------------------------- Lombok members

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>h</c>.</summary>
        Public Overridable Function getH() As SDVariable
            Return h
        End Function

        ''' <summary>Lombok getter of <c>c</c>.</summary>
        Public Overridable Function getC() As SDVariable
            Return c
        End Function

        ''' <summary>Lombok getter of <c>lastOutput</c>.</summary>
        Public Overridable Function getLastOutput() As SDVariable
            Return lastOutput_Conflict
        End Function

        ''' <summary>Lombok getter of <c>lastState</c>.</summary>
        Public Overridable Function getLastState() As SDVariable
            Return lastState_Conflict
        End Function
#End If

    End Class

End Namespace
