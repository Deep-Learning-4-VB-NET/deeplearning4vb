Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.impl.controlflow.compat


	Public MustInherit Class BaseCompatOp
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER NOTE: The field frameName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend frameName_Conflict As String

		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable)
			MyBase.New(Nothing, sameDiff, inputs)
		End Sub

		Public Sub New(ParamArray ByVal inputs() As INDArray)
			addInputArgument(inputs)
		End Sub

		Public Sub New()

		End Sub

		Public Overridable Property FrameName As String
			Get
				If numSArguments() > 0 Then
					Return getSArgument(0)
				End If
				Return frameName_Conflict
			End Get
			Set(ByVal frameName As String)
				Me.frameName_Conflict = frameName
			End Set
		End Property


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode,nodeDef, graph)
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim frameNameMapping As val = PropertyMapping.builder().tfAttrName("frame_name").onnxAttrName("frame_name").propertyNames(New String(){"frameName"}).build()

			map("frameName") = frameNameMapping

			Try
				ret(onnxName()) = map
			Catch e As NoOpNameFoundException
				'ignore, we dont care about onnx for this set of ops
			End Try


			Try
				ret(tensorflowName()) = map
			Catch e As NoOpNameFoundException
				'ignore
			End Try

			Return ret
		End Function


		Public Overrides Sub addSArgument(ParamArray ByVal args() As String)
			MyBase.addSArgument(args)
			If args IsNot Nothing AndAlso args.Length >= 1 Then
				FrameName = args(0)
			End If
		End Sub

		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Return MyBase.attributeAdaptersForFunction()
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Throw New System.NotSupportedException("calculateOutputShape() is not supported for control flow ops.")
		End Function
	End Class

End Namespace