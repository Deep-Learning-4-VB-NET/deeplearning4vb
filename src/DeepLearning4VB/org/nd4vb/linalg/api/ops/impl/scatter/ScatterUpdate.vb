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

Namespace org.nd4vb.linalg.api.ops.impl.scatter

    ''' <summary>The custom op <c>scatter_update</c> (Java <c>org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate</c>).</summary>
    Public Class ScatterUpdate
        Inherits DynamicCustomOp

        Public Enum UpdateOp
            ADD
            SUBTRACT
            MULTIPLY
            DIVIDE
            REVERSE_SUBTRACT
            REVERSE_DIVIDE
            ASSIGN
        End Enum

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal ref As SDVariable, ByVal indices As SDVariable, ByVal updates As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){ref, indices, updates}, False)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal ref As INDArray, ByVal indices As INDArray, ByVal update As INDArray)
            MyBase.New(New INDArray(){ref, indices, update}, Nothing)
        End Sub

        Public Overrides Function opName() As String
            Return "scatter_update"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "ScatterUpdate"
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
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
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradOut As IList(Of SDVariable)) As IList(Of SDVariable)
			'3 args: ref, indices, updates
			'For non-modified indices, input gradient (reference) is same as output gradient
			'For modified indices, dL/dref = dL/dOut * dOut/dRef = dL/dOut * d(update)/dRef = 0
			'And for updates, dL/du = dL/dOut * dOut/du = dL/dOut * d(update)/du = dL/dOut -> gather op

			Dim indices As SDVariable = arg(1)
			Dim updates As SDVariable = arg(2)

			Dim ret As IList(Of SDVariable) = New List(Of SDVariable)(3)
			Dim zerosUpdate As SDVariable = sameDiff.zerosLike(updates)
			Dim gradRef As SDVariable = sameDiff.scatterMul(gradOut(0), indices, zerosUpdate) 'TODO optimize
			ret.Add(gradRef) 'Reference array gradient
			ret.Add(sameDiff.zerosLike(arg(1))) 'Indices

			Dim gather As SDVariable = sameDiff.gather(gradOut(0), indices, 0) 'Updates
			ret.Add(gather)

			Return ret
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 3, "Expected exactly 3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
            Preconditions.checkState(inputDataTypes(0) = inputDataTypes(2), "Reference (input 0) and updates (input 2) must have exactly same data types, got %s and %s", inputDataTypes(0), inputDataTypes(2))
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
