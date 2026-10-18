Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports AttributeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>ParallelConcat</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.ParallelConcat</c>).</summary>
    Public Class ParallelConcat
        Inherits DynamicCustomOp

        Public Sub New()
            ' we know that axis is always 0 for PC
            iArguments.Add(0L)
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			' We might want to import everything here? i.e. shape in advance?
		End Sub
#End If

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

#If MODEL_IMPORT Then
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
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Dim first As DataType = dataTypes(0)
            Dim i As Integer = 1
            Do While i < dataTypes.Count
                Dim dt As DataType = dataTypes(i)
                Preconditions.checkState(first = dt, "Data types must all be equal: got %s", dataTypes)
                i += 1
            Loop
            Return New List(Of DataType) From {first}
        End Function

    End Class

End Namespace
