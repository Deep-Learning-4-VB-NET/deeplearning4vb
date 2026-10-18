Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports DifferentialFunctionClassHolder = DeepLearning4VB.org.nd4vb.imports.converters.DifferentialFunctionClassHolder
Imports AttributeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.PropertyMapping
Imports DataTypeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.adapters.DataTypeAdapter
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If FLATBUFFERS Then
Imports FlatBuffersMapper = DeepLearning4VB.org.nd4vb.autodiff.samediff.serde.FlatBuffersMapper
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.dtype

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.transforms.dtype.Cast</c>.</summary>
    Public Class Cast
        Inherits BaseDynamicTransformOp

        Private typeDst As DataType

        Public Sub New()
            '
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Cast(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable arg, @NonNull DataType dst)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal dst As DataType)
			MyBase.New(sameDiff, New SDVariable() {arg}, False)

			Me.typeDst = dst
			addArgs()
		End Sub
#End If

        Public Sub New(ByVal arg As INDArray, ByVal dataType As DataType)
            MyBase.New(New INDArray(){arg}, Nothing)
            Me.typeDst = dataType
            addArgs()
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub
#End If

#If FLATBUFFERS Then
		Protected Friend Overridable Sub addArgs()
			addIArgument(FlatBuffersMapper.getDataTypeAsByte(typeDst))
		End Sub
#Else
        Protected Friend Overridable Sub addArgs()
            ' FlatBuffersMapper.getDataTypeAsByte(typeDst): the FlatBuffers DType numbers are libnd4j's type numbers
            addIArgument(typeDst.toInt())
        End Sub
#End If

#If MODEL_IMPORT Then
		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Dim ret As IDictionary(Of String, IDictionary(Of String, AttributeAdapter)) = New LinkedHashMap(Of String, IDictionary(Of String, AttributeAdapter))()
			Dim tfAdapters As IDictionary(Of String, AttributeAdapter) = New LinkedHashMap(Of String, AttributeAdapter)()

			Dim fields As val = DifferentialFunctionClassHolder.Instance.getFieldsForFunction(Me)

			tfAdapters("typeDst") = New DataTypeAdapter()

			ret(tensorflowName()) = tfAdapters
			Return ret
		End Function

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim map As IDictionary(Of String, PropertyMapping) = New Dictionary(Of String, PropertyMapping)()

			Dim dstMapping As val = PropertyMapping.builder().tfAttrName("DstT").propertyNames(New String(){"typeDst"}).build()

			For Each propertyMapping As val In New PropertyMapping() {dstMapping}
				For Each keys As val In propertyMapping.getPropertyNames()
					map(keys) = propertyMapping
				Next keys
			Next propertyMapping

			ret(tensorflowName()) = map

			Return ret
		End Function
#End If

        Public Overrides Sub setValueFor(ByVal target As System.Reflection.FieldInfo, ByVal value As Object)
            'This is a hack around a property mapping issue - TF datatype DT_DOUBLE return attribute.getType() of DT_DOUBLE which doesn't make sense
            If value Is Nothing OrElse TypeOf value Is String OrElse TypeOf value Is DataType Then
                MyBase.setValueFor(target, value)
            End If
        End Sub

        Public Overrides Function opName() As String
            Return "cast"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "Cast"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'If input is numerical: reverse cast. Otherwise 0
			If arg().dataType().isFPType() Then
				Return Collections.singletonList(i_v(0).castTo(arg().dataType()))
			Else
				Return Collections.singletonList(sameDiff.zerosLike(arg()))
			End If
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'All scalar ops: output type is same as input type
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got input %s", Me.GetType(), dataTypes)
            Return New List(Of DataType) From {typeDst}
        End Function

    End Class

End Namespace
