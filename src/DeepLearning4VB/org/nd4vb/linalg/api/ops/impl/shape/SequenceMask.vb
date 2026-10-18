Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
#End If
#If MODEL_IMPORT Then
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>sequence_mask</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.SequenceMask</c>).</summary>
    Public Class SequenceMask
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Const DEFAULT_DTYPE As DataType = DataType.BOOL

        Private maxLen As Integer
        Private is_static_maxlen As Boolean = False
        Private dataTypeField As DataType

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal maxLen As SDVariable, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, maxLen}, False)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal maxLen As Integer, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, False)
			Me.maxLen = maxLen
			Me.is_static_maxlen = True
			addIArgument(maxLen)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input}, False)
			Me.dataType = dataType
			addDArgument(dataType)
		End Sub
#End If

        Public Sub New(ByVal input As INDArray, ByVal maxLen As Integer, ByVal dataType As DataType)
            addInputArgument(input)
            addIArgument(maxLen)
            Me.dataTypeField = dataType
            addDArgument(dataType)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal dataType As DataType)
            Me.New(input, CType(Nothing, INDArray), dataType)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal maxLength As INDArray, ByVal dataType As DataType)
            MyBase.New(wrapFilterNull(input, maxLength), Nothing)
            Me.dataTypeField = dataType
            addDArgument(dataType)
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim targetNode As val = TFGraphMapper.getNodeWithNameFromGraph(graph, nodeDef.getInput(1))
'JAVA TO VB CONVERTER NOTE: The variable maxlen was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim maxlen_Conflict As val = TFGraphMapper.getNDArrayFromTensor(targetNode)
			If maxlen_Conflict Is Nothing Then
				' No 2nd input
				Me.is_static_maxlen = True
			End If
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			If is_static_maxlen Then
				addIArgument(Me.maxLen)
			End If

		End Sub
		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim attrs As IDictionary(Of String, PropertyMapping) = New LinkedHashMap(Of String, PropertyMapping)()
			If is_static_maxlen Then
				Dim maxLen As val = PropertyMapping.builder().propertyNames(New String(){"maxLen"}).tfAttrName("maxlen").build()
				attrs("maxLen") = maxLen
			End If
			ret(tensorflowName()) = attrs
			Return ret
		End Function
#End If

        Public Overrides Function opName() As String
            Return "sequence_mask"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'Input is integer indices
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim args() As SDVariable = Me.args()
			Preconditions.checkState(dataTypes.Count = args.Length, "Expected list with exactly %s datatypes for %s, got %s", args.Length, Me.GetType(), dataTypes)
			'Output type is same as input by default
			Return Collections.singletonList(If(dataType = Nothing, DEFAULT_DTYPE, dataType))
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
