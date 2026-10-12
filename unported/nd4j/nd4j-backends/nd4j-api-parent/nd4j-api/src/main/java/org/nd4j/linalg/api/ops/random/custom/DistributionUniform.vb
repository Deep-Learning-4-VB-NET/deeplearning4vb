Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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

Namespace org.nd4j.linalg.api.ops.random.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DistributionUniform extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DistributionUniform
		Inherits DynamicCustomOp

		Private min As Double = 0.0
		Private max As Double = 1.0
		Private dataType As DataType

		Public Sub New()
			'
		End Sub

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

		Public Sub New(ByVal shape As INDArray, ByVal [out] As INDArray, ByVal min As Double, ByVal max As Double)
			Me.New(shape, [out], min, max, Nothing)
		End Sub

		Public Sub New(ByVal shape As INDArray, ByVal [out] As INDArray, ByVal min As Double, ByVal max As Double, ByVal dataType As DataType)
			MyBase.New(Nothing, New INDArray(){shape}, New INDArray(){[out]}, Arrays.asList(min, max), DirectCast(Nothing, IList(Of Integer)))
			Me.min = min
			Me.max = max
			Me.dataType = dataType
		End Sub


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

		Protected Friend Overridable Sub addArgs()
			tArguments.Clear()
			addTArgument(min, max)
			If dataType <> Nothing Then
				iArguments.Clear()
				addIArgument(dataType.toInt())
			End If
		End Sub

		Public Overrides Function opName() As String
			Return "randomuniform"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"RandomUniform", "RandomUniformInt"}
		End Function

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing, "Expected input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			'Input data type specifies the shape
			If dataType <> Nothing Then
				Return Collections.singletonList(dataType)
			End If
			Return Collections.singletonList(DataType.FLOAT)
		End Function
	End Class

End Namespace