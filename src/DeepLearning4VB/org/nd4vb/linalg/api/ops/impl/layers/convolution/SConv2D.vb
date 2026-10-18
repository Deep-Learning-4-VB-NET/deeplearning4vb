Imports System
Imports System.Collections.Generic
Imports NoOpNameFoundException = DeepLearning4VB.org.nd4vb.imports.NoOpNameFoundException
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Conv2DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.SConv2D</c>.</summary>
    Public Class SConv2D
        Inherits Conv2D

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffSBuilder") public SConv2D(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig conv2DConfig)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal conv2DConfig As Conv2DConfig)
			MyBase.New(sameDiff, inputFunctions, conv2DConfig)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SConv2D(@NonNull SameDiff sameDiff, @NonNull SDVariable layerInput, @NonNull SDVariable depthWeights, org.nd4j.autodiff.samediff.SDVariable pointWeights, org.nd4j.autodiff.samediff.SDVariable bias, @NonNull Conv2DConfig conv2DConfig)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal depthWeights As SDVariable, ByVal pointWeights As SDVariable, ByVal bias As SDVariable, ByVal conv2DConfig As Conv2DConfig)
			Me.New(sameDiff, wrapFilterNull(layerInput, depthWeights, pointWeights, bias), conv2DConfig)
		End Sub
#End If

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As Conv2DConfig)
            MyBase.New(inputs, outputs, config)
        End Sub

        Public Sub New(ByVal layerInput As INDArray, ByVal depthWeights As INDArray, ByVal pointWeights As INDArray, ByVal Conv2DConfig As Conv2DConfig)
            Me.New(wrapFilterNull(layerInput, depthWeights, pointWeights), Nothing, Conv2DConfig)
        End Sub

        Public Sub New(ByVal layerInput As INDArray, ByVal depthWeights As INDArray, ByVal pointWeights As INDArray, ByVal bias As INDArray, ByVal config As Conv2DConfig)
            Me.New(wrapFilterNull(layerInput, depthWeights, pointWeights, bias), Nothing, config)
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "sconv2d"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'Args at libnd4j level: in, gradAtOut, wD, wP, bias
			'Args for SConv2d libnd4j: input, wD, wP, bias
			Dim inputs As IList(Of SDVariable) = New List(Of SDVariable)()
			inputs.Add(arg(0))
			inputs.Add(f1(0))
			Dim args() As SDVariable = Me.args()
			For i As Integer = 1 To args.Length - 1 'Skip input, already added
				inputs.Add(args(i))
			Next i
			Dim conv2DDerivative As SConv2DDerivative = SConv2DDerivative.sDerviativeBuilder().conv2DConfig(config).inputFunctions(CType(inputs, List(Of SDVariable)).ToArray()).sameDiff(sameDiff).build()
			Dim ret As IList(Of SDVariable) = New List(Of SDVariable) From {conv2DDerivative.outputVariables()}
			Return ret
		End Function
#End If

        Public Overrides Function iArgs() As Long()
            If iArguments.Count = 0 Then
                addArgs()
            End If

            Return MyBase.iArgs()
        End Function

        Public Overrides ReadOnly Property ConfigProperties As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Function configFieldName() As String
            Return "config"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Throw New NoOpNameFoundException("No op name found for " & opName())
        End Function

        Public Overrides Function onnxName() As String
            Throw New NoOpNameFoundException("No onnx op opName found for op " & opName())
        End Function

        Public Overrides Function tensorflowName() As String
            Return "separable_conv2d"
        End Function

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
