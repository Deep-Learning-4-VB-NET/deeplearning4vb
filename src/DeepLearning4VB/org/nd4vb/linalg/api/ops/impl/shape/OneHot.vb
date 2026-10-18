Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
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

    ''' <summary>The custom op <c>onehot</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.OneHot</c>).</summary>
    Public Class OneHot
        Inherits DynamicCustomOp

        Public Const DEFAULT_DTYPE As DataType = DataType.FLOAT

        Private depth As Long
        Private jaxis As Long = -1
        Private [on] As Double
        Private off As Double
        Private outputType As DataType?

        Public Sub New()

        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal indices As SDVariable, ByVal depth As Integer)
			Me.New(sameDiff, indices, depth, -1, 1, 0, DEFAULT_DTYPE)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal indices As SDVariable, ByVal depth As Integer, ByVal axis As Integer, ByVal [on] As Double, ByVal off As Double, ByVal dataType As DataType)
			MyBase.New(Nothing, sameDiff, New SDVariable() {indices}, False)
			Me.depth = depth
			Me.jaxis = axis
			Me.on = [on]
			Me.off = off
			addArgs()
			Me.outputType = dataType
		End Sub
#End If

        Public Sub New(ByVal indices As INDArray, ByVal output As INDArray, ByVal depth As Integer)
            Me.New(indices, output, depth, -1, 1, 0)
        End Sub

        Public Sub New(ByVal indices As INDArray, ByVal depth As Integer)
            Me.New(indices, CType(Nothing, INDArray), depth, 0, 1.0, 0.0)
        End Sub

        Public Sub New(ByVal indices As INDArray, ByVal output As INDArray, ByVal depth As Integer, ByVal axis As Integer, ByVal [on] As Double, ByVal off As Double)
            MyBase.New(Nothing, indices, output, Nothing, Nothing)
            Me.depth = depth
            Me.jaxis = axis
            Me.on = [on]
            Me.off = off
            addArgs()
        End Sub

        Public Sub New(ByVal indices As INDArray, ByVal depth As Integer, ByVal axis As Integer, ByVal [on] As Double, ByVal off As Double)
            Me.New(indices, CType(Nothing, INDArray), depth, axis, [on], off)
        End Sub

        Public Sub New(ByVal indices As INDArray, ByVal depth As Integer, ByVal axis As Integer, ByVal [on] As Double, ByVal off As Double, ByVal dataType As DataType)
            Me.New(indices, CType(Nothing, INDArray), depth, axis, [on], off)
            Me.outputType = dataType
            If outputType.HasValue Then
                addDArgument(outputType)
            End If
        End Sub

        Protected Friend Overridable Sub addArgs()
            addIArgument(jaxis)
            addIArgument(depth)
            addTArgument([on])
            addTArgument(off)

            If outputType.HasValue Then
                addDArgument(outputType)
            End If
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
			If attributesForNode.ContainsKey("T") Then
				outputType = TFGraphMapper.convertType(attributesForNode("T").getType())
			End If
		End Sub

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Dim ret As IDictionary(Of String, IDictionary(Of String, PropertyMapping)) = New Dictionary(Of String, IDictionary(Of String, PropertyMapping))()
			Dim attrs As IDictionary(Of String, PropertyMapping) = New LinkedHashMap(Of String, PropertyMapping)()

			Dim depth As val = PropertyMapping.builder().propertyNames(New String(){"depth"}).tfInputPosition(1).build()
			attrs("depth") = depth

			Dim [on] As val = PropertyMapping.builder().propertyNames(New String(){"on"}).tfInputPosition(2).build()
			attrs("on") = [on]

			Dim off As val = PropertyMapping.builder().propertyNames(New String(){"off"}).tfInputPosition(3).build()
			attrs("off") = off

			Dim axis As val = PropertyMapping.builder().propertyNames(New String() {"jaxis"}).tfAttrName("axis").build()
			attrs("jaxis") = axis

			ret(tensorflowName()) = attrs
			Return ret
		End Function
#End If

        Public Overrides Function tensorflowName() As String
            Return "OneHot"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx name found for " & opName())
        End Function

        Public Overrides Function opName() As String
            Return "onehot"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes.Count >= 1 AndAlso dataTypes.Count <= 4, "Expected list with 1 to 4 datatypes for %s, got %s", Me.GetType(), dataTypes)
            If outputType.HasValue Then
                Return New List(Of DataType) From {outputType}
            Else
                Return New List(Of DataType) From {DEFAULT_DTYPE}
            End If
        End Function

    End Class

End Namespace
