Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>meshgrid</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.MeshGrid</c>).</summary>
    Public Class MeshGrid
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		''' 
		''' <param name="sd"> </param>
		''' <param name="cartesian"> If true: broadcast dimensions for first two dimensions are swapped </param>
		''' <param name="inputs"> </param>
		Public Sub New(ByVal sd As SameDiff, ByVal cartesian As Boolean, ParamArray ByVal inputs() As SDVariable)
			MyBase.New(Nothing, sd, inputs, False)
			addIArgument(If(cartesian, 1, 0))
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal inputs() As SDVariable, ByVal cartesian As Boolean)
			Me.New(sd, cartesian, inputs)
		End Sub
#End If

        Public Sub New(ByVal inputs() As INDArray, ByVal cartesian As Boolean)
            MyBase.New(inputs, Nothing)
            addIArgument(If(cartesian, 1, 0))
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "meshgrid"
        End Function

#If SAMEDIFF Then
		Public Overridable Overloads Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim args() As SDVariable = Me.args()
			Dim [out] As IList(Of SDVariable) = New List(Of SDVariable)(args.Length)
			For i As Integer = 0 To args.Length - 1
				Dim dims(args.Length - 2) As Integer
				Dim x As Integer=0
				For j As Integer = 0 To args.Length - 1
					If i = j Then
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: dims[x++] = j;
					dims(x) = j
						x += 1
				Next j
				[out].Add(gradients(i).sum(dims))
			Next i
			Return [out]
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return args().Length
			End Get
		End Property
#Else
        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return inputArguments().Count
            End Get
        End Property
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Same output types as input types
            Return dataTypes
        End Function

    End Class

End Namespace
