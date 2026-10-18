Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Conv1DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig
Imports PaddingMode = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.PaddingMode
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>The custom op <c>conv1d</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.Conv1D</c>).</summary>
    Public Class Conv1D
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend config As Conv1DConfig
        Private Const INVALID_CONFIGURATION As String = "Invalid Conv1D configuration : s = %s p = %s "

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Conv1D(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, @NonNull Conv1DConfig conv1DConfig)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal conv1DConfig As Conv1DConfig)
			Me.New(sameDiff, wrapFilterNull(input, weights, bias), conv1DConfig)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sameDiffBuilder") public Conv1D(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal config As Conv1DConfig)
			MyBase.New(sameDiff, inputFunctions)
			initConfig(config)
		End Sub
#End If

        Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As Conv1DConfig)
            MyBase.New(inputs, outputs)

            initConfig(config)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal output As INDArray, ByVal config As Conv1DConfig)
            Me.New(wrapFilterNull(input, weights, bias), wrapOrNull(output), config)
        End Sub

        Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal config As Conv1DConfig)
            Me.New(input, weights, bias, Nothing, config)
        End Sub

        Private Sub initConfig(ByVal config As Conv1DConfig)
            Me.config = config
            Preconditions.checkState(config.getS() >= 1 AndAlso config.getP() >= 0, INVALID_CONFIGURATION, config.getS(), config.getP())
            addArgs()
        End Sub

        Protected Friend Overridable Sub addArgs()
            If config Is Nothing Then
                config = Conv1DConfig.builder().build()
            End If

            addIArgument(config.getK(), config.getS(), config.getP(), config.getD(), CInt(config.getPaddingMode()), ArrayUtil.fromBoolean(config.isNWC()))
        End Sub

        Public Overrides Function iArgs() As Long()
            If iArguments.Count = 0 Then
                addArgs()
            End If

            Return MyBase.iArgs()
        End Function

        Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
            If config Is Nothing AndAlso iArguments.Count > 0 Then
                config = Conv1DConfig.builder().k(iArguments(0)).s(iArguments(1)).p(iArguments(2)).d(iArguments(3)).paddingMode(System.Enum.GetValues(GetType(PaddingMode))(CInt(iArguments(4)))).dataFormat(If(iArguments(5) = 1, Conv1DConfig.NCW, Conv1DConfig.NWC)).build()
            End If

            Return config.getValue([property])
        End Function

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            Return config.toProperties()
        End Function

        Public Overrides ReadOnly Property ConfigProperties As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Function configFieldName() As String
            Return "config"
        End Function

        Public Overrides Function opName() As String
            Return "conv1d"
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

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim args As IList(Of SDVariable) = New List(Of SDVariable)()
			Collections.addAll(args, Me.args())
			args.Add(grads(0))

			Dim gradFn As New Conv1DDerivative(sameDiff, CType(args, List(Of SDVariable)).ToArray(), config)
			Return New List(Of SDVariable) From {gradFn.outputVariables()}
		End Function
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>config</c>.</summary>
        Public Overridable Function getConfig() As Conv1DConfig
            Return config
        End Function

    End Class

End Namespace
