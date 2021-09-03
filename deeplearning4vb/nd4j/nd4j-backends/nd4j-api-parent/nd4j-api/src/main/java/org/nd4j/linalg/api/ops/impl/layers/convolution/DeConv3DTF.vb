Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports DeConv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.impl.layers.convolution



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class DeConv3DTF extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DeConv3DTF
		Inherits DynamicCustomOp

		Protected Friend config As DeConv3DConfig

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv3DTF(@NonNull SameDiff sameDiff, @NonNull SDVariable shape, @NonNull SDVariable weights, @NonNull SDVariable input, @NonNull DeConv3DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal weights As SDVariable, ByVal input As SDVariable, ByVal config As DeConv3DConfig)
			MyBase.New(sameDiff, New SDVariable(){shape, weights, input})

			Me.config = config
			addArgs()
		End Sub

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
			addIArgument(If(config.getDataFormat().equalsIgnoreCase(DeConv3DConfig.NCDHW), 0, 1))
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

		Public Overrides Function opName() As String
			Return "deconv3d_tf"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"Conv3DBackpropInput", "Conv3DBackpropInputV2"}
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Backprop not yet implemented for " & Me.GetType())
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType) 'inShape, weights, input
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(2))
		End Function
	End Class

End Namespace