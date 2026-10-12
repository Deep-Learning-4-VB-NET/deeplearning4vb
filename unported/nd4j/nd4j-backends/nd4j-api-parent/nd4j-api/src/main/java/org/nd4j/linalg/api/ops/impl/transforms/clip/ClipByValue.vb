Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Onnx = onnx.Onnx
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.clip


	Public Class ClipByValue
		Inherits DynamicCustomOp

		Private clipValueMin As Double
		Private clipValueMax As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ClipByValue(@NonNull INDArray input, double clipValueMin, double clipValueMax)
		Public Sub New(ByVal input As INDArray, ByVal clipValueMin As Double, ByVal clipValueMax As Double)
			MyBase.New(Nothing, New INDArray(){input}, Nothing)
			Me.clipValueMin = clipValueMin
			Me.clipValueMax = clipValueMax
			addTArgument(clipValueMin, clipValueMax)
		End Sub

		Public Sub New()

		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal clipValueMin As Double, ByVal clipValueMax As Double, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){x})
			Me.clipValueMin = clipValueMin
			Me.clipValueMax = clipValueMax
			Me.inplaceCall = inPlace
			addTArgument(clipValueMin, clipValueMax)
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal clipValueMin As Double, ByVal clipValueMax As Double)
			MyBase.New(Nothing, sameDiff, New SDVariable(){x})
			Me.clipValueMin = clipValueMin
			Me.clipValueMax = clipValueMax
			addTArgument(clipValueMin, clipValueMax)
		End Sub

		Public Overrides Function opName() As String
			Return "ClipByValue"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub


		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'dOut/dIn is 0 if clipped, 1 otherwise
			Dim notClippedLower As SDVariable = sameDiff.gt(arg(), clipValueMin).castTo(arg().dataType())
			Dim notClippedUpper As SDVariable = sameDiff.lt(arg(), clipValueMax).castTo(arg().dataType())
			Dim ret As SDVariable = notClippedLower.mul(notClippedUpper).mul(grad(0))
			Return Collections.singletonList(ret)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count > 0, "Expected at least 1 input datatype for %s, got %s", Me.GetType(), inputDataTypes)
			'get the final data type (sometimes model import passes in 2 dummy data types that aren't relevant)
			Return New List(Of DataType) From {inputDataTypes(inputDataTypes.Count - 1)}
		End Function
	End Class

End Namespace