Imports System
Imports System.Collections.Generic
#If SAMEDIFF Then
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.outputs.GRUCellOutputs</c>.</summary>
    Public Class GRUCellOutputs

#If SAMEDIFF Then
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
#End If

        ' ---------------------------------------------------------------- Lombok members

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>r</c>.</summary>
        Public Overridable Function getR() As SDVariable
            Return r
        End Function

        ''' <summary>Lombok getter of <c>u</c>.</summary>
        Public Overridable Function getU() As SDVariable
            Return u
        End Function

        ''' <summary>Lombok getter of <c>c</c>.</summary>
        Public Overridable Function getC() As SDVariable
            Return c
        End Function

        ''' <summary>Lombok getter of <c>h</c>.</summary>
        Public Overridable Function getH() As SDVariable
            Return h
        End Function
#End If

    End Class

End Namespace
