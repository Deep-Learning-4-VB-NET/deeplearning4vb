Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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
'ORIGINAL LINE: @Slf4j public class DeConv3DDerivative extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DeConv3DDerivative
		Inherits DynamicCustomOp

		Protected Friend config As DeConv3DConfig

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeConv3DDerivative(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, org.nd4j.autodiff.samediff.SDVariable grad, org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal grad As SDVariable, ByVal config As DeConv3DConfig)
			MyBase.New(sameDiff, toArr(input, weights, bias, grad))
			Me.config = config
			addArgs()
		End Sub

		Private Shared Function toArr(ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal grad As SDVariable) As SDVariable()
			If bias IsNot Nothing Then
				Return New SDVariable(){input, weights, bias, grad}
			Else
				Return New SDVariable(){input, weights, grad}
			End If
		End Function

		Public Overrides Function opName() As String
			Return "deconv3d_bp"
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


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No op name found for backwards.")
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No op name found for backwards")
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Gradient of DeConv3DDerivative not supported.")

		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				'Inputs: in, weights, optional bias, gradOut                      3 req, 1 optional
				'Outputs: gradAtInput, gradW, optional gradB                      2 req, 1 optional
				Dim args() As SDVariable = Me.args()
				Return args.Length - 1
			End Get
		End Property

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
	End Class

End Namespace