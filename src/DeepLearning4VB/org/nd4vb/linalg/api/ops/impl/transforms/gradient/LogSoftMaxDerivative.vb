Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.gradient

    ''' 
    Public Class LogSoftMaxDerivative
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal gradO As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){[in], gradO})
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal gradO As INDArray, ByVal [out] As INDArray)
            MyBase.New(Nothing, New INDArray(){[in], gradO}, New INDArray(){[out]})
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable, ByVal wrt As SDVariable, ByVal dimension As Integer)
			Me.New(sameDiff, arg, wrt)
			Me.addIArgument(dimension)
		End Sub
#End If

        ''' <summary>
        ''' The opName of this operation
        ''' </summary>
        ''' <returns> the opName of this operation </returns>
        Public Overrides Function opName() As String
            Return "log_softmax_bp"
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentation of op not supported: " & Me.GetType().FullName)
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inTypes IsNot Nothing AndAlso inTypes.Count = 2, "Expected 2 input datatypes for %s, got %s", Me.GetType(), inTypes)
            Return New List(Of DataType) From {inTypes(0)}
        End Function

    End Class

End Namespace
