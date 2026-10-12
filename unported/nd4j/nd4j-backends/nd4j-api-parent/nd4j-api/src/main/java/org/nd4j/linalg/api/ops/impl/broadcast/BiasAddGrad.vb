Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
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

Namespace org.nd4j.linalg.api.ops.impl.broadcast


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class BiasAddGrad extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class BiasAddGrad
		Inherits DynamicCustomOp

		Protected Friend nchw As Boolean = True

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal bias As SDVariable, ByVal gradient As SDVariable, ByVal nchw As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){input, bias, gradient})
			Me.nchw = nchw
			addBArgument(nchw)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BiasAddGrad(@NonNull INDArray input, @NonNull INDArray bias, @NonNull INDArray gradient, org.nd4j.linalg.api.ndarray.INDArray output)
		Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal gradient As INDArray, ByVal output As INDArray)
			MyBase.New(New INDArray(){input, bias, gradient}, wrapOrNull(output))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BiasAddGrad(@NonNull INDArray input, @NonNull INDArray bias, @NonNull INDArray gradient, boolean nchw)
		Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal gradient As INDArray, ByVal nchw As Boolean)
			addInputArgument(input, bias, gradient)
			Me.nchw = nchw
			addBArgument(nchw)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BiasAddGrad(@NonNull INDArray input, @NonNull INDArray bias, @NonNull INDArray gradient)
		Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal gradient As INDArray)
			Me.New(input, bias, gradient, False)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 0
		End Function

		Public Overrides Function opName() As String
			Return "biasadd_bp"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentiation not supported for op " & Me.GetType().Name)
		End Function

		Public Overrides Function onnxName() As String
			Return "BiasAddGrad"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 3, "Expected 3 input data types for %s, got %s", Me.GetType(), inputDataTypes)
			Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(1)}
		End Function
	End Class

End Namespace