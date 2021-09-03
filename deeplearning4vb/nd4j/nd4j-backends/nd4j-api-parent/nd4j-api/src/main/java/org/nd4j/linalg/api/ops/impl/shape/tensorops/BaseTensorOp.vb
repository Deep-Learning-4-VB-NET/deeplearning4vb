Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
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

Namespace org.nd4j.linalg.api.ops.impl.shape.tensorops


	Public MustInherit Class BaseTensorOp
		Inherits DynamicCustomOp

		Public Sub New(ByVal name As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(name, sameDiff, args)
		End Sub
		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(Nothing, sameDiff, args)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim inputOne As val = nodeDef.getInput(1)
			Dim varFor As val = initWith.getVariable(inputOne)
			Dim nodeWithIndex As val = TFGraphMapper.getNodeWithNameFromGraph(graph,inputOne)
			Dim var As val = TFGraphMapper.getArrayFrom(nodeWithIndex,graph)
			If var IsNot Nothing Then
				Dim idx As val = var.getInt(0)
				addIArgument(idx)
			End If
		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation not yet implemented for " & Me.GetType().FullName)
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op name found for " & opName())
		End Function

		Public Overrides Function ToString() As String
			Return opName()
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Throw New System.NotSupportedException("calculateOutputShape() is not supported for tensor ops.")
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				'1 output in allay cases - sometimes just a dummy output, however
				Return 1
			End Get
		End Property

	End Class

End Namespace