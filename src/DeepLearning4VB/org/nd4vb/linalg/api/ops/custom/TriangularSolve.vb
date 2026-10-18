Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

Namespace org.nd4vb.linalg.api.ops.custom

    ''' <summary>The custom op <c>triangular_solve</c> (Java <c>org.nd4j.linalg.api.ops.custom.TriangularSolve</c>).</summary>
    Public Class TriangularSolve
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Public Sub New(ByVal matrix As INDArray, ByVal rhs As INDArray, ByVal lower As Boolean, ByVal adjoint As Boolean)
            addInputArgument(matrix, rhs)
            addBArgument(lower, adjoint)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal lower As SDVariable, ByVal adjoint As SDVariable)
			MyBase.New(sameDiff, New SDVariable() {matrix, rhs, lower, adjoint})
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal lower As Boolean, ByVal adjoint As Boolean)
			MyBase.New(sameDiff, New SDVariable() {matrix, rhs})
			addBArgument(lower, adjoint)
		End Sub
#End If

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
			If attributesForNode.ContainsKey("lower") Then
				addBArgument(attributesForNode("lower").getB())
			End If

			If attributesForNode.ContainsKey("adjoint") Then
				addBArgument(attributesForNode("adjoint").getB())
			End If
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "triangular_solve"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "MatrixTriangularSolve"
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
