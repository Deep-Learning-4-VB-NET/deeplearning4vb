Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
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
    Public Class GradientBackwardsMarker
        Inherits DynamicCustomOp

        Public Const OP_NAME As String = "gradientbackwards"

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){i_v1, i_v2})
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Overrides Function opNum() As Integer
            Return 0
        End Function

        ''' <summary>
        ''' The opName of this operation
        ''' </summary>
        ''' <returns> the opName of this operation </returns>
        Public Overrides Function opName() As String
            Return OP_NAME
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return i_v
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            'Dummy variable
            Return New List(Of DataType) From {DataType.FLOAT}
        End Function

        Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
            Throw New System.NotSupportedException("calculateOutputShape() is not supported for control flow ops.")
        End Function

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return 1
            End Get
        End Property

    End Class

End Namespace
