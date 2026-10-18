Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports AttributeAdapter = DeepLearning4VB.org.nd4vb.imports.descriptors.properties.AttributeAdapter
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

Namespace org.nd4vb.linalg.api.ops.impl.controlflow.compat

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.controlflow.compat.BaseCompatOp</c>.</summary>
    Public MustInherit Class BaseCompatOp
        Inherits DynamicCustomOp

        Protected Friend frameNameField As String

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable)
			MyBase.New(Nothing, sameDiff, inputs)
		End Sub
#End If

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
                Return frameNameField
            End Get
            Set(ByVal frameName As String)
                Me.frameNameField = frameName
            End Set
        End Property

#If MODEL_IMPORT Then
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
#End If

        Public Overrides Sub addSArgument(ParamArray ByVal args() As String)
            MyBase.addSArgument(args)
            If args IsNot Nothing AndAlso args.Length >= 1 Then
                FrameName = args(0)
            End If
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Function attributeAdaptersForFunction() As IDictionary(Of String, IDictionary(Of String, AttributeAdapter))
			Return MyBase.attributeAdaptersForFunction()
		End Function
#End If

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Throw New System.NotSupportedException("calculateOutputShape() is not supported for control flow ops.")
        End Function

    End Class

End Namespace
