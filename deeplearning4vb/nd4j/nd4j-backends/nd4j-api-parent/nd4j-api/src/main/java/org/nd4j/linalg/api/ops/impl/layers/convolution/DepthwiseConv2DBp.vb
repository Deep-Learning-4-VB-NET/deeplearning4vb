Imports System
Imports System.Collections.Generic
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
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
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class DepthwiseConv2DBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DepthwiseConv2DBp
		Inherits DynamicCustomOp

		Protected Friend config As Conv2DConfig


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DepthwiseConv2DBp(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, @NonNull SDVariable gradO, @NonNull Conv2DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal gradO As SDVariable, ByVal config As Conv2DConfig)
			MyBase.New(sameDiff, wrapFilterNull(input, weights, bias, gradO))
			Me.config = config
			addArgs()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DepthwiseConv2DBp(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, @NonNull SDVariable gradO, @NonNull Conv2DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal gradO As SDVariable, ByVal config As Conv2DConfig)
			MyBase.New(sameDiff, wrapFilterNull(input, weights, gradO))
			Me.config = config
			addArgs()

		End Sub


		Public Overrides Function iArgs() As Long()
			If iArguments.Count = 0 Then
				addArgs()
			End If

			Return MyBase.iArgs()
		End Function

		Protected Friend Overridable Sub addArgs()
			addIArgument(config.getKH(), config.getKW(), config.getSH(), config.getSW(), config.getPH(), config.getPW(), config.getDH(), config.getDW(), ArrayUtil.fromBoolean(config.isSameMode()),If(config.getDataFormat().equalsIgnoreCase(Conv2DConfig.NCHW), 0, 1))

		End Sub

		Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			If config Is Nothing Then
				config = Conv2DConfig.builder().build()
			End If

			Try
				Dim t As val = config.getValue([property])
				Return t
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			If config Is Nothing AndAlso iArguments.Count > 0 Then
				config = Conv2DConfig.builder().kH(iArguments(0)).kW(iArguments(1)).sH(iArguments(2)).sW(iArguments(3)).pH(iArguments(4)).pW(iArguments(5)).dH(iArguments(6)).dW(iArguments(7)).isSameMode(iArguments(8) = 1).dataFormat(If(iArguments(9) = 1, Conv2DConfig.NHWC_Conflict, Conv2DConfig.NCHW)).build()
			End If
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
			Return "depthwise_conv2d_bp"
		End Function


		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Dim list As IList(Of DataType) = New List(Of DataType)()
			Dim i As Integer=0
			Do While i<n-1
				list.Add(inputDataTypes(0))
				i += 1
			Loop
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return list
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				Return If(args().Length = 4, 3, 2)
			End Get
		End Property
	End Class

End Namespace