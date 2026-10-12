Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
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

Namespace org.nd4j.linalg.api.ops.impl.scatter



	Public Class ScatterMin
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal ref As SDVariable, ByVal indices As SDVariable, ByVal updates As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){ref, indices, updates}, False)
		End Sub

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ScatterMin(@NonNull INDArray ref, @NonNull INDArray indices, @NonNull INDArray update)
		Public Sub New(ByVal ref As INDArray, ByVal indices As INDArray, ByVal update As INDArray)
			MyBase.New(New INDArray(){ref, indices, update}, Nothing)
		End Sub

		Public Overrides Function opName() As String
			Return "scatter_min"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "ScatterMin"
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			TFGraphMapper.initFunctionFromProperties(nodeDef.getOp(), Me, attributesForNode, nodeDef, graph)

			If nodeDef.containsAttr("use_locking") Then
				If nodeDef.getAttrOrThrow("use_locking").getB() = True Then
					bArguments.Add(True)
				Else
					bArguments.Add(False)
				End If
			Else
				bArguments.Add(False)
			End If
		End Sub

		Public Overrides Function doDiff(ByVal gradOut As IList(Of SDVariable)) As IList(Of SDVariable)
			'3 args: ref, indices, updates
			'For non-modified indices, input gradient (reference) is same as output gradient
			'For modified indices, dL/dref = dL/dOut if(ref[index[i],j] == min) or 0 otherwise
			'And for updates, dL/du = dL/dOut if(update[i,j]==min) or 0 otherwise

			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)(3)
			Dim notModified As SDVariable = arg(0).eq(outputVariable()).castTo(arg(0).dataType()) '0 if modified, 1 otherwise
			Dim refGrad As SDVariable = gradOut(0).mul(notModified)

			Dim gatherOut As SDVariable = sameDiff.gather(outputVariable(), arg(1), 0)
			Dim gatherGrad As SDVariable = sameDiff.gather(gradOut(0), arg(1), 0)
			Dim outIsUpdate As SDVariable = gatherOut.eq(arg(2)).castTo(arg(2).dataType())
			Dim updateGrad As SDVariable = gatherGrad.mul(outIsUpdate)

			Return New List(Of SDVariable) From {refGrad, sameDiff.zerosLike(arg(1)), updateGrad}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 3, "Expected exactly 3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Preconditions.checkState(inputDataTypes(0) = inputDataTypes(2), "Reference (input 0) and updates (input 2) must have exactly same data types, got %s and %s", inputDataTypes(0), inputDataTypes(2))
			Return Collections.singletonList(inputDataTypes(0))
		End Function

	End Class

End Namespace