Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeConv3DConfig = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
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

Namespace org.nd4vb.linalg.api.ops.impl.layers.convolution

    ''' <summary>The custom op <c>deconv3d_tf</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv3DTF</c>).</summary>
    Public Class DeConv3DTF
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend config As DeConv3DConfig

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv3DTF(@NonNull SameDiff sameDiff, @NonNull SDVariable shape, @NonNull SDVariable weights, @NonNull SDVariable input, @NonNull DeConv3DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal weights As SDVariable, ByVal input As SDVariable, ByVal config As DeConv3DConfig)
			MyBase.New(sameDiff, New SDVariable(){shape, weights, input})

			Me.config = config
			addArgs()
		End Sub
#End If

        Public Overrides Function iArgs() As Long()
            If iArguments.Count = 0 Then
                addArgs()
            End If

            Return MyBase.iArgs()
        End Function

        Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
            If config Is Nothing AndAlso iArguments.Count > 0 Then
                config = DeConv3DConfig.builder().kD(iArguments(0)).kH(iArguments(1)).kW(iArguments(2)).sD(iArguments(3)).sH(iArguments(4)).sW(iArguments(5)).pD(iArguments(6)).pH(iArguments(7)).pW(iArguments(8)).dD(iArguments(9)).dH(iArguments(10)).dW(iArguments(11)).isSameMode(iArguments(12) = 1).dataFormat(If(iArguments(13) = 1, DeConv3DConfig.NDHWC, DeConv3DConfig.NCDHW)).build()
            End If
            Return config.toProperties()
        End Function

        Private Sub addArgs()
            addIArgument(config.getKD())
            addIArgument(config.getKH())
            addIArgument(config.getKW())
            addIArgument(config.getSD())
            addIArgument(config.getSH())
            addIArgument(config.getSW())
            addIArgument(config.getPD())
            addIArgument(config.getPH())
            addIArgument(config.getPW())
            addIArgument(config.getDD())
            addIArgument(config.getDH())
            addIArgument(config.getDW())
            addIArgument(ArrayUtil.fromBoolean(config.isSameMode()))
            addIArgument(If(config.getDataFormat().Equals(DeConv3DConfig.NCDHW, StringComparison.OrdinalIgnoreCase), 0, 1))
        End Sub

        Public Overrides ReadOnly Property ConfigProperties As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Function configFieldName() As String
            Return "config"
        End Function

        Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
            If config Is Nothing Then
                config = DeConv3DConfig.builder().build()
            End If

            Return config.getValue([property])
        End Function

#If MODEL_IMPORT Then
		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

			Dim aStrides As val = nodeDef.getAttrOrThrow("strides")
			Dim aDilations As val = nodeDef.getAttrOrDefault("dilations", Nothing)
			Dim tfStrides As val = aStrides.getList().getIList()
			Dim tfDilation As val = If(aDilations Is Nothing, Nothing, aDilations.getList().getIList())
			Dim sD, sH, sW, dD, dH, dW As Integer

			Dim aPadding As val = nodeDef.getAttrOrDefault("padding", Nothing)
			Dim paddingMode As String = aPadding.getS().toStringUtf8()

			Dim dataFormat As String = DeConv3DConfig.NDHWC
			If nodeDef.containsAttr("data_format") Then
				Dim attr As val = nodeDef.getAttrOrThrow("data_format")
				dataFormat = attr.getS().toStringUtf8().ToLower()
			End If

			If dataFormat.Equals(DeConv3DConfig.NCDHW, StringComparison.OrdinalIgnoreCase) Then
				sD = tfStrides.get(2).intValue()
				sH = tfStrides.get(3).intValue()
				sW = tfStrides.get(4).intValue()

				dD = If(tfDilation Is Nothing, 1, tfDilation.get(2).intValue())
				dH = If(tfDilation Is Nothing, 1, tfDilation.get(3).intValue())
				dW = If(tfDilation Is Nothing, 1, tfDilation.get(4).intValue())
			Else
				sD = tfStrides.get(1).intValue()
				sH = tfStrides.get(2).intValue()
				sW = tfStrides.get(3).intValue()

				dD = If(tfDilation Is Nothing, 1, tfDilation.get(1).intValue())
				dH = If(tfDilation Is Nothing, 1, tfDilation.get(2).intValue())
				dW = If(tfDilation Is Nothing, 1, tfDilation.get(3).intValue())
			End If

			Dim isSameMode As Boolean = paddingMode.Equals("SAME", StringComparison.OrdinalIgnoreCase)
			Dim conv3DConfig As DeConv3DConfig = DeConv3DConfig.builder().kD(-1).kH(-1).kW(-1).sD(sD).sH(sW).sW(sH).dD(dD).dH(dH).dW(dW).isSameMode(isSameMode).dataFormat(If(dataFormat.Equals(DeConv3DConfig.NCDHW, StringComparison.OrdinalIgnoreCase), DeConv3DConfig.NCDHW, DeConv3DConfig.NDHWC)).build()
			Me.config = conv3DConfig

			addArgs()
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "deconv3d_tf"
        End Function

        Public Overrides Function tensorflowNames() As String()
            Return New String(){"Conv3DBackpropInput", "Conv3DBackpropInputV2"}
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Backprop not yet implemented for " & Me.GetType())
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType) 'inShape, weights, input
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(2))
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType) 'inShape, weights, input
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

        ' ---------------------------------------------------------------- Lombok members

        ''' <summary>Lombok getter of <c>config</c>.</summary>
        Public Overridable Function getConfig() As DeConv3DConfig
            Return config
        End Function

    End Class

End Namespace
