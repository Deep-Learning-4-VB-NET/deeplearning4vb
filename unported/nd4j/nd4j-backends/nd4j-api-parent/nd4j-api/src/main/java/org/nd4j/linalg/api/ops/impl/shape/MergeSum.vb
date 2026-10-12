Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Onnx = onnx.Onnx
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

Namespace org.nd4j.linalg.api.ops.impl.shape


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class MergeSum extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class MergeSum
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ParamArray ByVal inputs() As SDVariable)
			MyBase.New(Nothing, sameDiff, inputs)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "mergesum"
		End Function


	'
	'    @Override
	'    public List<LongShapeDescriptor> calculateOutputShape() {
	'        List<LongShapeDescriptor> ret = new ArrayList<>(1);
	'        ret.add(arg().getShapeDescriptor());
	'        return ret;
	'    }
	'

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			' noop
		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)
			MyBase.initFromOnnx(node, initWith, attributesForNode, graph)
		End Sub

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "AddN"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim gradient As SDVariable = sameDiff.setupFunction(i_v(0))
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)()
			Dim i As Integer = 0
			Do While i < args().Length
				ret.Add(gradient)
				i += 1
			Loop
			Return ret
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim first As DataType = dataTypes(0)
			For i As Integer = 1 To dataTypes.Count - 1
				Dim dt As DataType = dataTypes(i)
				Preconditions.checkState(first = dt, "All inputs must have same datatype - got %s and %s for inputs 0 and %s respectively", first, dt, i)
			Next i
			'Output type is same as input types
			Return Collections.singletonList(first)
		End Function

	End Class

End Namespace