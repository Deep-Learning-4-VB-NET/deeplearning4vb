Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.weightinit


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode public abstract class BaseWeightInitScheme implements WeightInitScheme
	Public MustInherit Class BaseWeightInitScheme
		Implements WeightInitScheme

		Public MustOverride Function type() As WeightInit Implements WeightInitScheme.type
'JAVA TO VB CONVERTER NOTE: The field order was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private order_Conflict As Char

		''' <summary>
		''' Initialize with c weight ordering by default
		''' </summary>
		Public Sub New()
			Me.New("c"c)
		End Sub

		Public Sub New(ByVal order As Char)
			Me.order_Conflict = order
		End Sub

		Public MustOverride Function doCreate(ByVal dataType As DataType, ByVal shape() As Long, ByVal paramsView As INDArray) As INDArray

		Public Overridable Function create(ByVal shape() As Long, ByVal paramsView As INDArray) As INDArray Implements WeightInitScheme.create
			Return handleParamsView(doCreate(paramsView.dataType(), shape,paramsView),paramsView)
		End Function

		Public Overridable Function create(ByVal dataType As DataType, ParamArray ByVal shape() As Long) As INDArray Implements WeightInitScheme.create
			Dim ret As INDArray = doCreate(dataType, shape,Nothing)
			Return ret
		End Function

		Public Overridable Function order() As Char Implements WeightInitScheme.order
			Return order_Conflict
		End Function

		Protected Friend Overridable Function handleParamsView(ByVal outputArray As INDArray, ByVal paramView As INDArray) As INDArray
			'minor optimization when the views are the same, just return
			If paramView Is Nothing OrElse paramView Is outputArray Then
				Return outputArray
			End If
			Dim flat As INDArray = Nd4j.toFlattened(order(), outputArray)
			If flat.length() <> paramView.length() Then
				Throw New Exception("ParamView length does not match initialized weights length (view length: " & paramView.length() & ", view shape: " & Arrays.toString(paramView.shape()) & "; flattened length: " & flat.length())
			End If

			paramView.assign(flat)

			Return paramView.reshape(order(), outputArray.shape())
		End Function


	End Class

End Namespace