Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Conv2DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
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

    ''' <summary>The custom op <c>col2im</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.Col2Im</c>).</summary>
    Public Class Col2Im
        Inherits DynamicCustomOp

        Protected Friend conv2DConfig As Conv2DConfig

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "builder") public Col2Im(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ndarray.INDArray[] inputArrays, org.nd4j.linalg.api.ndarray.INDArray[] outputs, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig conv2DConfig)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal inputArrays() As INDArray, ByVal outputs() As INDArray, ByVal conv2DConfig As Conv2DConfig)
			MyBase.New(Nothing,inputArrays,outputs)
			If sameDiff IsNot Nothing Then
				Me.sameDiff = sameDiff
			End If

			Me.conv2DConfig = conv2DConfig

			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Col2Im(@NonNull SameDiff sd, @NonNull SDVariable input, @NonNull Conv2DConfig config)
		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal config As Conv2DConfig)
			MyBase.New(Nothing, sd, New SDVariable(){input})
			Me.conv2DConfig = config
			addArgs()
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Sub New(ByVal [in] As INDArray, ByVal conv2DConfig As Conv2DConfig)
            MyBase.New("col2Im",[in],Nothing,Nothing,Nothing)
            Me.conv2DConfig = conv2DConfig
        End Sub

        Protected Friend Overridable Sub addArgs()
            addIArgument(conv2DConfig.getSH())
            addIArgument(conv2DConfig.getSW())
            addIArgument(conv2DConfig.getPH())
            addIArgument(conv2DConfig.getPW())
            addIArgument(conv2DConfig.getKH())
            addIArgument(conv2DConfig.getKW())
            addIArgument(conv2DConfig.getDH())
            addIArgument(conv2DConfig.getDW())
            addIArgument(ArrayUtil.fromBoolean(conv2DConfig.isSameMode()))

        End Sub

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            Return conv2DConfig.toProperties()
        End Function

        Public Overrides Function opName() As String
            Return "col2im"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Unable to run derivative op on col2im")
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input data type for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>conv2DConfig</c>.</summary>
        Public Overridable Function getConv2DConfig() As Conv2DConfig
            Return conv2DConfig
        End Function

    End Class

End Namespace
