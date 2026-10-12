Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.weights


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class WeightInitIdentity implements IWeightInit
	<Serializable>
	Public Class WeightInitIdentity
		Implements IWeightInit

		Private scale As Double?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WeightInitIdentity(@JsonProperty("scale") System.Nullable<Double> scale)
		Public Sub New(ByVal scale As Double?)
			Me.scale = scale
		End Sub


		Public Overridable Function init(ByVal fanIn As Double, ByVal fanOut As Double, ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray Implements IWeightInit.init
			If shape(0) <> shape(1) Then
				Throw New System.InvalidOperationException("Cannot use IDENTITY init with parameters of shape " & Arrays.toString(shape) & ": weights must be a square matrix for identity")
			End If
			Select Case shape.Length
				Case 2
				   Return setIdentity2D(shape, order, paramView)
				Case 3, 4, 5
					Return setIdentityConv(shape, order, paramView)
					Case Else
						Throw New System.InvalidOperationException("Identity mapping for " & shape.Length & " dimensions not defined!")
			End Select
		End Function

		Private Function setIdentity2D(ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray
			Dim ret As INDArray
			If order = Nd4j.order() Then
				ret = Nd4j.eye(shape(0))
			Else
				ret = Nd4j.createUninitialized(shape, order).assign(Nd4j.eye(shape(0)))
			End If

			If scale IsNot Nothing Then
				ret.muli(scale)
			End If

			Dim flat As INDArray = Nd4j.toFlattened(order, ret)
			paramView.assign(flat)
			Return paramView.reshape(order, shape)
		End Function

		''' <summary>
		''' Set identity mapping for convolution layers. When viewed as an NxM matrix of kernel tensors,
		''' identity mapping is when parameters is a diagonal matrix of identity kernels. </summary>
		''' <param name="shape"> Shape of parameters </param>
		''' <param name="order"> Order of parameters </param>
		''' <param name="paramView"> View of parameters </param>
		''' <returns> A reshaped view of paramView which results in identity mapping when used in convolution layers </returns>
		Private Function setIdentityConv(ByVal shape() As Long, ByVal order As Char, ByVal paramView As INDArray) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.indexing.INDArrayIndex[] indArrayIndices = new org.nd4j.linalg.indexing.INDArrayIndex[shape.length];
			Dim indArrayIndices(shape.Length - 1) As INDArrayIndex
			Dim i As Integer = 2
			Do While i < shape.Length
				If shape(i) Mod 2 = 0 Then
					Throw New System.InvalidOperationException("Cannot use IDENTITY init with parameters of shape " & Arrays.toString(shape) & "! Must have odd sized kernels!")
				End If
				indArrayIndices(i) = NDArrayIndex.point(shape(i) \ 2)
				i += 1
			Loop

			paramView.assign(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray params =paramView.reshape(order, shape);
			Dim params As INDArray =paramView.reshape(order, shape)
			i = 0
			Do While i < shape(0)
				indArrayIndices(0) = NDArrayIndex.point(i)
				indArrayIndices(1) = NDArrayIndex.point(i)
				params.put(indArrayIndices, Nd4j.ones(1))
				i += 1
			Loop
			If scale IsNot Nothing Then
				params.muli(scale)
			End If
			Return params
		End Function
	End Class

End Namespace