Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports ModBpOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.ModBpOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic


	Public Class ModOp
		Inherits BaseDynamicTransformOp

		Public Const OP_NAME As String = "mod"

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ModOp(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable y)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){x, y}, False)
		End Sub

		Public Sub New(ByVal first As INDArray, ByVal second As INDArray, ByVal result As INDArray)
			Me.New(New INDArray(){first, second},If(result Is Nothing, Nothing, New INDArray()){result})
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ModOp(@NonNull INDArray x, @NonNull INDArray y)
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			Me.New(New INDArray(){x, y}, Nothing)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

		Public Overrides Function opName() As String
			Return OP_NAME
		End Function

		Public Overrides Function onnxName() As String
			Return "Mod"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Mod"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New ModBpOp(sameDiff, larg(), rarg(), i_v(0))).outputs()
		End Function
	End Class

End Namespace