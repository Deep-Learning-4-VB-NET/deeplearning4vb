Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ParallelConcat extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class ParallelConcat
		Inherits DynamicCustomOp

		Public Sub New()
			' we know that axis is always 0 for PC
			iArguments.Add(0L)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			' We might want to import everything here? i.e. shape in advance?
		End Sub

		Public Overrides Function opName() As String
			' :)
			Return "ParallelConcat"
		End Function


		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return 1
			End Get
		End Property

		Public Overrides Function tensorflowName() As String
			Return "ParallelConcat"
		End Function

		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Dim ret As IDictionary(Of String, IDictionary(Of String, AttributeAdapter)) = New Dictionary(Of String, IDictionary(Of String, AttributeAdapter))()
			Dim tfMappings As IDictionary(Of String, AttributeAdapter) = New LinkedHashMap(Of String, AttributeAdapter)()

			Return ret
		End Function

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Return ret
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim first As DataType = dataTypes(0)
			Dim i As Integer = 1
			Do While i < dataTypes.Count
				Dim dt As DataType = dataTypes(i)
				Preconditions.checkState(first = dt, "Data types must all be equal: got %s", dataTypes)
				i += 1
			Loop
			Return Collections.singletonList(first)
		End Function
	End Class

End Namespace