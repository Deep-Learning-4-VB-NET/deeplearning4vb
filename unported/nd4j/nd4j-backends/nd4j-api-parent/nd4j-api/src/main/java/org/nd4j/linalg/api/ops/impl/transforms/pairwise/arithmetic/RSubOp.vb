Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports RSubBpOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.RSubBpOp

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


	Public Class RSubOp
		Inherits BaseDynamicTransformOp

		Public Const OP_NAME As String = "reversesubtract"


		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, args, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			Me.New(sameDiff, New SDVariable(){i_v1, i_v2}, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal inPlace As Boolean)
			Me.New(sameDiff, New SDVariable(){i_v1, i_v2}, inPlace)
		End Sub

		Public Sub New(ByVal first As INDArray, ByVal second As INDArray)
			Me.New(first, second, Nothing)
		End Sub

		Public Sub New(ByVal first As INDArray, ByVal second As INDArray, ByVal result As INDArray)
			Me.New(New INDArray(){first, second}, wrapOrNull(result))
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return OP_NAME
		End Function

		Public Overrides Function onnxName() As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New NoOpNameFoundException("No ONNX op name found for: " & Me.GetType().FullName)
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New RSubBpOp(sameDiff, larg(), rarg(), i_v(0))).outputs()
		End Function

	End Class

End Namespace