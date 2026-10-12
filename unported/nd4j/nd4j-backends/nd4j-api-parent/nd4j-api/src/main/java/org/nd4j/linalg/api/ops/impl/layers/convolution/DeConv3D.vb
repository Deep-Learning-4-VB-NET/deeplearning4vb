Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports DeConv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class DeConv3D extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DeConv3D
		Inherits DynamicCustomOp

		Protected Friend config As DeConv3DConfig

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv3D(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, @NonNull DeConv3DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal config As DeConv3DConfig)
			MyBase.New(sameDiff, toArr(input, weights, bias))
			Me.config = config
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv3D(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, @NonNull DeConv3DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal config As DeConv3DConfig)
			MyBase.New(sameDiff, toArr(input, weights, Nothing))
			Me.config = config
			addArgs()
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As DeConv3DConfig)
			MyBase.New(inputs, outputs)

			Me.config = config
			addArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv3D(@NonNull INDArray input, @NonNull INDArray weights, org.nd4j.linalg.api.ndarray.INDArray bias, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull DeConv3DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal output As INDArray, ByVal config As DeConv3DConfig)
			Me.New(wrapFilterNull(input, weights, bias), wrapOrNull(output), config)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal config As DeConv3DConfig)
			Me.New(input, weights, bias, Nothing, config)
		End Sub

		Private Shared Function toArr(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable) As SDVariable()
			If bias IsNot Nothing Then
				Return New SDVariable(){input, weights, bias}
			Else
				Return New SDVariable(){input, weights}
			End If
		End Function

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


		Public Overrides Function opName() As String
			Return "deconv3d"
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim bias As SDVariable = If(args().Length > 2, arg(2), Nothing)
			Return (New DeConv3DDerivative(sameDiff, arg(0), arg(1), bias, f1(0), config)).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class
End Namespace