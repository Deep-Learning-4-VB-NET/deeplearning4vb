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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.outputs.LSTMCellOutputs</c>.</summary>
    Public Class LSTMCellOutputs

#If SAMEDIFF Then
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
#End If

        ' ---------------------------------------------------------------- Lombok members

#If SAMEDIFF Then
        ''' <summary>Lombok getter of <c>i</c>.</summary>
        Public Overridable Function getI() As SDVariable
            Return i
        End Function

        ''' <summary>Lombok getter of <c>c</c>.</summary>
        Public Overridable Function getC() As SDVariable
            Return c
        End Function

        ''' <summary>Lombok getter of <c>f</c>.</summary>
        Public Overridable Function getF() As SDVariable
            Return f
        End Function

        ''' <summary>Lombok getter of <c>o</c>.</summary>
        Public Overridable Function getO() As SDVariable
            Return o
        End Function

        ''' <summary>Lombok getter of <c>z</c>.</summary>
        Public Overridable Function getZ() As SDVariable
            Return z
        End Function

        ''' <summary>Lombok getter of <c>h</c>.</summary>
        Public Overridable Function getH() As SDVariable
            Return h
        End Function

        ''' <summary>Lombok getter of <c>y</c>.</summary>
        Public Overridable Function getY() As SDVariable
            Return y
        End Function
#End If

    End Class

End Namespace
