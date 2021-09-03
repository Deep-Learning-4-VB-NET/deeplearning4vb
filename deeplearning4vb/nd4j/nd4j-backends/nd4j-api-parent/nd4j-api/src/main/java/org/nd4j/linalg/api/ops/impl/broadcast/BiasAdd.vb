Imports System
Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
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

Namespace org.nd4j.linalg.api.ops.impl.broadcast


	''' <summary>
	''' Bias addition gradient operation.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class BiasAdd extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class BiasAdd
		Inherits DynamicCustomOp

		Protected Friend nchw As Boolean = True

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal bias As SDVariable, ByVal nchw As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, bias}, False)
			bArguments.Clear()
			bArguments.Add(nchw)
			Me.nchw = nchw
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BiasAdd(@NonNull INDArray input, @NonNull INDArray bias, boolean nchw)
		Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal nchw As Boolean)
			Me.New(input, bias, Nothing, nchw)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BiasAdd(@NonNull INDArray input, @NonNull INDArray bias, org.nd4j.linalg.api.ndarray.INDArray output, boolean nchw)
		Public Sub New(ByVal input As INDArray, ByVal bias As INDArray, ByVal output As INDArray, ByVal nchw As Boolean)
			MyBase.New(New INDArray(){input, bias}, wrapOrNull(output))
			bArguments.Clear()
			bArguments.Add(nchw)
			Me.nchw = nchw
		End Sub

		Public Overrides Function opName() As String
			Return "biasadd"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			MyBase.initFromTensorFlow(nodeDef, initWith, attributesForNode, graph)
			If attributesForNode.ContainsKey("data_format") Then
				nchw = "NCHW".Equals(attributesForNode("data_format").getS().toStringUtf8(), StringComparison.OrdinalIgnoreCase)
			Else
				nchw = False 'TF default is NHWC
			End If
			bArguments.Clear()
			bArguments.Add(nchw)
		End Sub

		Public Overrides Function onnxName() As String
			Return "BiasAdd"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"BiasAdd", "BiasAddV1"}
		End Function

		Public Overrides Function doDiff(ByVal gradient As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New BiasAddGrad(sameDiff, arg(0), arg(1), gradient(0), nchw)).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected 2 input data types for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace