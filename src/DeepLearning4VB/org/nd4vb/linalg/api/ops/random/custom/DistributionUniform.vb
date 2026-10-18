Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
#End If
#If MODEL_IMPORT Then
Imports TFGraphMapper = DeepLearning4VB.org.nd4vb.imports.graphmapper.tf.TFGraphMapper
Imports ND4JIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
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

Namespace org.nd4vb.linalg.api.ops.random.custom

    ''' <summary>The custom op <c>randomuniform</c> (Java <c>org.nd4j.linalg.api.ops.random.custom.DistributionUniform</c>).</summary>
    Public Class DistributionUniform
        Inherits DynamicCustomOp

        Private min As Double = 0.0
        Private max As Double = 1.0
        Private dataTypeField As DataType?

        Public Sub New()
            '
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal shape As SDVariable, ByVal min As Double, ByVal max As Double)
			Me.New(sd, shape, min, max, Nothing)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal shape As SDVariable, ByVal min As Double, ByVal max As Double, ByVal dataType As DataType)
			MyBase.New(Nothing, sd, New SDVariable(){shape})
			Preconditions.checkState(min <= max, "Minimum (%s) must be <= max (%s)", min, max)
			Preconditions.checkState(dataType = Nothing OrElse dataType.isNumerical(), "Only numerical datatypes can be used with DistributionUniform - rquested output datatype: %s", dataType)
			Me.dataType = dataType
			Me.min = min
			Me.max = max
			addArgs()
		End Sub
#End If

        Public Sub New(ByVal shape As INDArray, ByVal [out] As INDArray, ByVal min As Double, ByVal max As Double)
            Me.New(shape, [out], min, max, Nothing)
        End Sub

        Public Sub New(ByVal shape As INDArray, ByVal [out] As INDArray, ByVal min As Double, ByVal max As Double, ByVal dataType As DataType)
            MyBase.New(Nothing, New INDArray(){shape}, New INDArray(){[out]}, Arrays.asList(min, max), DirectCast(Nothing, IList(Of Integer)))
            Me.min = min
            Me.max = max
            Me.dataTypeField = dataType
        End Sub

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim vDtype As AttrValue = attributesForNode("dtype")
			Dim vTout As AttrValue = attributesForNode("Tout")
			If vDtype Is Nothing AndAlso vTout Is Nothing Then
				Throw New ND4JIllegalStateException("Unable to find output data type for node " & nodeDef.getName())
			End If
			Dim v As AttrValue = If(vDtype Is Nothing, vTout, vDtype)
			dataType = TFGraphMapper.convertType(v.getType())
			addIArgument(dataType.toInt())
			addTArgument(0.0, 1.0) 'TF version is hardcoded 0 to 1
		End Sub
#End If

        Protected Friend Overridable Sub addArgs()
            tArguments.Clear()
            addTArgument(min, max)
            If dataTypeField.HasValue Then
                iArguments.Clear()
                addIArgument(dataTypeField.Value.toInt())
            End If
        End Sub

        Public Overrides Function opName() As String
            Return "randomuniform"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"RandomUniform", "RandomUniformInt"}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing, "Expected input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
            'Input data type specifies the shape
            If dataTypeField.HasValue Then
                Return New List(Of DataType) From {dataTypeField}
            End If
            Return New List(Of DataType) From {DataType.FLOAT}
        End Function

    End Class

End Namespace
