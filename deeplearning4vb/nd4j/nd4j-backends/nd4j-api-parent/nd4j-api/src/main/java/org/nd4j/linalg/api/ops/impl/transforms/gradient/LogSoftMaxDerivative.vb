Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.gradient



	''' 
	Public Class LogSoftMaxDerivative
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal gradO As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){[in], gradO})
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal gradO As INDArray, ByVal [out] As INDArray)
			MyBase.New(Nothing, New INDArray(){[in], gradO}, New INDArray(){[out]})
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal wrt As SDVariable, ByVal dimension As Integer)
			Me.New(sameDiff, arg, wrt)
			Me.addIArgument(dimension)
		End Sub

		''' <summary>
		''' The opName of this operation
		''' </summary>
		''' <returns> the opName of this operation </returns>
		Public Overrides Function opName() As String
			Return "log_softmax_bp"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function



		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentation of op not supported: " & Me.GetType().FullName)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inTypes IsNot Nothing AndAlso inTypes.Count = 2, "Expected 2 input datatypes for %s, got %s", Me.GetType(), inTypes)
			Return Collections.singletonList(inTypes(0))
		End Function
	End Class

End Namespace