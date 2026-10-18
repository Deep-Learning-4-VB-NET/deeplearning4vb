Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports Conv3DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.Conv3DDerivative</c>.</summary>
    Public Class Conv3DDerivative
        Inherits Conv3D

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "derivativeBuilder") public Conv3DDerivative(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig conv3DConfig)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal conv3DConfig As Conv3DConfig)
			MyBase.New(sameDiff, inputFunctions, conv3DConfig)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "conv3dnew_bp"
        End Function

        Public Overrides Function tensorflowName() As String
            Throw New NoOpNameFoundException("No tensorflow op name found for conv3d derivative")
        End Function

        Public Overrides Function tensorflowNames() As String()
            Throw New NoOpNameFoundException("No tensorflow op name found for conv3d derivative")
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op name found for conv3d derivative")
        End Function

        Public Overrides Function onnxNames() As String()
            Throw New NoOpNameFoundException("No onnx op name found for conv3d derivative")
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Unable to differentiate from a derivative op")
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				'Fwd inputs: input, weight, optional bias
				'Bwd inputs: input, input grad, weight, optional bias
				If args().Length = 4 Then
					Return 3 'Includes bias
				Else
					Return 2 'No bias - only input + weight grads
				End If
			End Get
		End Property
#Else
        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                'Fwd inputs: input, weight, optional bias
                'Bwd inputs: input, input grad, weight, optional bias
                If inputArguments().Count = 4 Then
                    Return 3 'Includes bias
                Else
                    Return 2 'No bias - only input + weight grads
                End If
            End Get
        End Property
#End If

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length 'Original inputs + gradient at
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Dim [out] As IList(Of DataType) = New List(Of DataType)(n-1)
			Dim i As Integer=0
			Do While i<n-1
				[out].Add(inputDataTypes(i))
				i += 1
			Loop
			Return [out]
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
