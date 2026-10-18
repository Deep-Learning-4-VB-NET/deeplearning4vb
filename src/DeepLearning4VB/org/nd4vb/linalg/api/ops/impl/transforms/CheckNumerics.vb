Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
#If MODEL_IMPORT Then
Imports Nd4j = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef
#End If
#If SAMEDIFF OrElse MODEL_IMPORT Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms

    ''' <summary>The custom op <c>check_numerics</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.CheckNumerics</c>).</summary>
    Public Class CheckNumerics
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal message As SDVariable)
			MyBase.New(sd, New SDVariable(){input, message})
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "check_numerics"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "CheckNumerics"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(f1(0))
		End Function
#End If

        Public Overrides Function numOutputArguments() As Integer
            Return 1
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			Dim str As String = attributesForNode("message").getS().toStringUtf8()
			'No "string args" support in libnd4j custom ops -> make it a constant instead
			Dim name As String = nodeDef.getName()
			Dim msg As SDVariable = initWith.constant(name & "/message", Nd4j.scalar(str))
			Dim newInputs As IList(Of String) = New List(Of String)(2)
			CType(newInputs, List(Of String)).AddRange(initWith.getOps().get(name).getInputsToOp())
			newInputs.Add(msg.name())
			initWith.getOps().get(name).setInputsToOp(newInputs)
			initWith.getVariables().get(msg.name()).setInputsForOp(Collections.singletonList(getOwnName()))
		End Sub
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            'input data types may be less than 2 for import, only first one matters anyways
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count <= 2, "Expected 2 datatype in, got %s", inputDataTypes)
            Preconditions.checkState(inputDataTypes(0).isFPType(), "Input datatype must be a floating point type, got %s", inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
