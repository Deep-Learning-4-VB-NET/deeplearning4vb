Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports org.nd4j.imports.descriptors.properties.adapters
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


	Public Class Dilation2D
		Inherits DynamicCustomOp

		Protected Friend isSameMode As Boolean

		' rates
		Protected Friend r0, r1, r2, r3 As Integer

		' strides
		Protected Friend s0, s1, s2, s3 As Integer


		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal df As SDVariable, ByVal weights As SDVariable, ByVal strides() As Integer, ByVal rates() As Integer, ByVal isSameMode As Boolean)
			Me.New(sameDiff, New SDVariable(){df, weights}, strides, rates, isSameMode, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputAndWeights() As SDVariable, ByVal strides() As Integer, ByVal rates() As Integer, ByVal isSameMode As Boolean, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, inputAndWeights, inPlace)
			Preconditions.checkArgument(rates.Length = 4, "Dilation rate length must be 4, got an array with length %s with values %s", rates.Length, rates)
			Preconditions.checkArgument(strides.Length = 4, "Dilation strides length must be 4, got an array with length %s with values %s", strides.Length, strides)

			r0 = rates(0)
			r1 = rates(1)
			r2 = rates(2)
			r3 = rates(3)
			s0 = strides(0)
			s1 = strides(1)
			s2 = strides(2)
			s3 = strides(3)
			Me.isSameMode = isSameMode

			addArgs()

		End Sub

		Public Sub New(ByVal inputArrays() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(Nothing, inputArrays, outputs)

		End Sub

		Public Sub New(ByVal df As INDArray, ByVal weights As INDArray, ByVal strides() As Integer, ByVal rates() As Integer, ByVal isSameMode As Boolean)
			addInputArgument(df, weights)

			If rates.Length < 4 Then
				Throw New System.ArgumentException("Dilation rate length must be 4.")
			End If
			If strides.Length < 4 Then
				Throw New System.ArgumentException("Strides length must be 4.")
			End If

			r0 = rates(0)
			r1 = rates(1)
			r2 = rates(2)
			r3 = rates(3)
			s0 = strides(0)
			s1 = strides(1)
			s2 = strides(2)
			s3 = strides(3)
			Me.isSameMode = isSameMode

			addArgs()
		End Sub

		Protected Friend Overridable Sub addArgs()
			addIArgument(If(isSameMode, 1, 0), r0, r1, r2, r3, s0, s1, s2, s3)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode,nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim sameMode As val = PropertyMapping.builder().tfAttrName("padding").propertyNames(New String(){"isSameMode"}).build()

			Dim ratesMapping As val = PropertyMapping.builder().tfAttrName("rates").propertyNames(New String(){"r0", "r1", "r2", "r3"}).build()

			Dim stridesMapping As val = PropertyMapping.builder().tfAttrName("strides").propertyNames(New String(){"s0", "s1", "s2", "s3"}).build()

			map("isSameMode") = sameMode

			map("r0") = ratesMapping
			map("r1") = ratesMapping
			map("r2") = ratesMapping
			map("r3") = ratesMapping

			map("s0") = stridesMapping
			map("s1") = stridesMapping
			map("s2") = stridesMapping
			map("s3") = stridesMapping

			Try
				ret(onnxName()) = map
			Catch e As NoOpNameFoundException
				'ignore, we dont care about onnx for this set of ops
			End Try


			Try
				ret(tensorflowName()) = map
			Catch e As NoOpNameFoundException
				Throw New Exception(e)
			End Try

			Return ret
		End Function

		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Dim ret As IDictionary(Of String, IDictionary(Of String, AttributeAdapter)) = New Dictionary(Of String, IDictionary(Of String, AttributeAdapter))()
			Dim tfMappings As IDictionary(Of String, AttributeAdapter) = New LinkedHashMap(Of String, AttributeAdapter)()
			Dim fields As val = DifferentialFunctionClassHolder.Instance.getFieldsForFunction(Me)


			tfMappings("r0") = New IntArrayIntIndexAdapter(0)
			tfMappings("r1") = New IntArrayIntIndexAdapter(1)
			tfMappings("r2") = New IntArrayIntIndexAdapter(2)
			tfMappings("r3") = New IntArrayIntIndexAdapter(3)

			tfMappings("s0") = New IntArrayIntIndexAdapter(0)
			tfMappings("s1") = New IntArrayIntIndexAdapter(1)
			tfMappings("s2") = New IntArrayIntIndexAdapter(2)
			tfMappings("s3") = New IntArrayIntIndexAdapter(3)

			tfMappings("isSameMode") = New StringEqualsAdapter("SAME")

			' Onnx doesn't have this op i think?
			Dim onnxMappings As IDictionary(Of String, AttributeAdapter) = New Dictionary(Of String, AttributeAdapter)()
			onnxMappings("isSameMode") = New StringEqualsAdapter("SAME")

			ret(tensorflowName()) = tfMappings
			ret(onnxName()) = onnxMappings
			Return ret
		End Function


		Public Overrides Function opName() As String
			Return "dilation2d"
		End Function

		Public Overrides Function onnxName() As String
			Return "Dilation_2D"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Dilation2D"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType) 'Input and weights, optional rates/strides
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count >= 2 AndAlso inputDataTypes.Count <= 4, "Expected 2 to 4 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace