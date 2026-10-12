Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
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

Namespace org.nd4j.linalg.api.ops.impl.transforms


	Public Class HistogramFixedWidth
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal values As SDVariable, ByVal valuesRange As SDVariable, ByVal numBins As SDVariable)
			MyBase.New(sameDiff,If(numBins Is Nothing, New SDVariable(){values, valuesRange}, New SDVariable()){values, valuesRange, numBins}, False)
		End Sub

		Public Sub New()
			'no-op
		End Sub

		Public Overrides Function opName() As String
			Return "histogram_fixed_width"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "HistogramFixedWidth"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			'No op - just need the inputs
		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			'1 or 2 possible: 2 for TF import (fill with specified value
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 2 OrElse dataTypes.Count = 3), "Expected 2 or 3 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			'TODO MAKE CONFIGURABLE
			Return Collections.singletonList(DataType.INT)
		End Function
	End Class

End Namespace