Imports System.Collections.Generic
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.layers


	Public Class ExternalErrorsFunction
		Inherits DynamicCustomOp

		Public Const OP_NAME As String = "ExternalErrorsFn"

		Private Shared ReadOnly OUT_SHAPE As IList(Of LongShapeDescriptor) = Collections.singletonList(LongShapeDescriptor.fromShape(New Long(){}, Nd4j.dataType()))

		Private gradients As IDictionary(Of String, INDArray)
		Private gradVariables As IDictionary(Of String, SDVariable)
		Private [out] As SDVariable
		Private id As String


		Public Sub New(ByVal sd As SameDiff, ByVal inputs As IList(Of SDVariable), ByVal gradients As IDictionary(Of String, INDArray))
			MyBase.New(sd, CType(inputs, List(Of SDVariable)).ToArray())
			If gradients Is Nothing Then
				gradients = New Dictionary(Of String, INDArray)()
			End If
			Me.gradients = gradients
			Me.id = System.Guid.randomUUID().ToString()
		End Sub

		Public Sub New()
		End Sub

		Public Overridable ReadOnly Property GradPlaceholderName As String
			Get
				Return arg().name() & "-grad"
			End Get
		End Property

		Public Overrides Function outputVariables(ByVal baseName As String) As SDVariable()
			If [out] Is Nothing Then
				If id Is Nothing Then
					Me.id = System.Guid.randomUUID().ToString()
				End If
				Dim name As String = "dummyOutput-" & id
				If sameDiff.hasVariable(name) Then
					[out] = sameDiff.getVariable(name)
				Else
					[out] = sameDiff.zero(name, Nd4j.dataType(), 1)
					sameDiff.getOps().get(getOwnName()).setOutputsOfOp(Collections.singletonList([out].name()))
					sameDiff.getVariables().get(name).setOutputOfOp(getOwnName())
				End If
			End If
			Return New SDVariable(){[out]}
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim [out] As IList(Of SDVariable) = New List(Of SDVariable)()
			If gradVariables Is Nothing Then
				gradVariables = New Dictionary(Of String, SDVariable)()
				For Each arg As SDVariable In args()
					Dim gradArr As INDArray = gradients(arg.name())
					Dim grad As SDVariable
					Dim dt As DataType = arg.dataType()
					Dim n As String = GradPlaceholderName
					If gradArr IsNot Nothing Then
						Dim shape() As Long = CType(gradArr.shape().Clone(), Long())
						shape(0) = -1
						grad = sameDiff.var(n, VariableType.PLACEHOLDER, Nothing, dt, shape)
					Else
						grad = sameDiff.var(n, VariableType.PLACEHOLDER, Nothing, dt)
					End If
					gradVariables(arg.name()) = grad
					[out].Add(grad)
				Next arg
			End If
			Return [out]
		End Function


		Public Overridable Sub updateBeforeExecution()
			Preconditions.checkState(gradVariables IsNot Nothing, "Variables list is null - doDiff has not been called?")

			'Update external gradients ready for execution
			For Each e As KeyValuePair(Of String, SDVariable) In gradVariables.SetOfKeyValuePairs()
				Dim extGradArray As INDArray = gradients(e.Key)
				If extGradArray Is Nothing Then
					Throw New System.InvalidOperationException("Cannot execute SameDiff instance with external errors: external gradient " & "for variable " & e.Key & " has not been defined")
				End If
				gradVariables(e.Key).setArray(extGradArray)
			Next e
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("Not supported: " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("Not supported: " & opName())
		End Function

		Public Overrides Function opName() As String
			Return OP_NAME
		End Function

		Public Overrides Function ToString() As String
			Return "ExternalErrorsFunction(" & (If(gradVariables IsNot Nothing, gradVariables.Keys, "")) & ")"
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return OUT_SHAPE
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Return OUT_SHAPE
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.LOGIC
		End Function
	End Class

End Namespace