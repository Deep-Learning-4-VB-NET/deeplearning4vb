Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.api.ops.impl.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class NormalizeMoments extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class NormalizeMoments
		Inherits DynamicCustomOp

		Private shift As Double = 0.0 ' reporting for duty

		Public Sub New(ByVal sameDiff As SameDiff, ByVal counts As SDVariable, ByVal means As SDVariable, ByVal variances As SDVariable)
			Me.New(sameDiff, counts, means, variances, 0.0)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal counts As SDVariable, ByVal means As SDVariable, ByVal variances As SDVariable, ByVal shift As Double)
			MyBase.New(Nothing, sameDiff, New SDVariable() {counts, means, variances}, False)
			Me.shift = shift
			addArgs()
		End Sub

		Public Sub New(ByVal counts As INDArray, ByVal means As INDArray, ByVal variances As INDArray, ByVal shift As Double)
			MyBase.New(Nothing, New INDArray(){counts, means, variances}, Nothing)
			Me.shift = shift
			addArgs()
		End Sub

		Public Sub New(ByVal counts As INDArray, ByVal ssSum As INDArray, ByVal ssSqSum As INDArray, ByVal outMean As INDArray, ByVal outVar As INDArray)
			MyBase.New(Nothing, New INDArray(){counts, ssSum, ssSqSum}, New INDArray(){outMean, outVar}, New List(Of Double)(), New List(Of Integer)())

			addArgs()
		End Sub

		Private Sub addArgs()
			addTArgument(shift)
		End Sub

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)
			addArgs()
		End Sub

		Public Overrides Function opName() As String
			Return "normalize_moments"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 3, "Expected 3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			'Count, mean_ss, variance_ss
			If inputDataTypes(1).isFPType() Then
				Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(0)}
			End If
			Return New List(Of DataType) From {Nd4j.defaultFloatingPointType(), Nd4j.defaultFloatingPointType()}
		End Function

	End Class

End Namespace