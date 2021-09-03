Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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
Namespace org.nd4j.linalg.api.ops.custom


	Public Class Polygamma
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Polygamma(@NonNull INDArray n, @NonNull INDArray x)
		Public Sub New(ByVal n As INDArray, ByVal x As INDArray)
			Preconditions.checkArgument(n.shape() <> x.shape(), "Polygamma: n and x must have the same shapes")
			addInputArgument(n,x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Polygamma(@NonNull INDArray n, @NonNull INDArray x, org.nd4j.linalg.api.ndarray.INDArray output)
		Public Sub New(ByVal n As INDArray, ByVal x As INDArray, ByVal output As INDArray)
			Me.New(n,x)
			If output IsNot Nothing Then
				addOutputArgument(output)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Polygamma(@NonNull SameDiff sameDiff, @NonNull SDVariable n, @NonNull SDVariable x)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal n As SDVariable, ByVal x As SDVariable)
			MyBase.New("", sameDiff, New SDVariable(){n, x})
		End Sub

		Public Overrides Function opName() As String
			Return "polygamma"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Polygamma"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace