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
Imports Conv1DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig
Imports PaddingMode = org.nd4j.linalg.api.ops.impl.layers.convolution.config.PaddingMode
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
'ORIGINAL LINE: @Slf4j @Getter @NoArgsConstructor public class Conv1DDerivative extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Conv1DDerivative
		Inherits DynamicCustomOp

		Protected Friend config As Conv1DConfig
		Private Const INVALID_CONFIGURATION As String = "Invalid Conv1D configuration : s = %s p = %s "

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Conv1DDerivative(@NonNull SameDiff sameDiff, @NonNull SDVariable[] inputs, @NonNull Conv1DConfig config)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputs() As SDVariable, ByVal config As Conv1DConfig)
			MyBase.New(sameDiff, inputs)
			initConfig(config)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Conv1DDerivative(@NonNull SameDiff sd, @NonNull SDVariable input, @NonNull SDVariable weights, org.nd4j.autodiff.samediff.SDVariable bias, org.nd4j.autodiff.samediff.SDVariable gradOut, @NonNull Conv1DConfig config)
		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable, ByVal gradOut As SDVariable, ByVal config As Conv1DConfig)
			Me.New(sd, wrapFilterNull(input, weights, bias, gradOut), config)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal config As Conv1DConfig)
			MyBase.New(inputs, outputs)

			initConfig(config)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Conv1DDerivative(@NonNull INDArray input, @NonNull INDArray weights, org.nd4j.linalg.api.ndarray.INDArray bias, @NonNull INDArray gradOut, org.nd4j.linalg.api.ndarray.INDArray output, @NonNull Conv1DConfig config)
		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal gradOut As INDArray, ByVal output As INDArray, ByVal config As Conv1DConfig)
			Me.New(wrapFilterNull(input, weights, bias, gradOut), wrapOrNull(output), config)
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

			addIArgument(config.getK(), config.getS(), config.getP(), config.getD(), config.getPaddingMode().ordinal(), ArrayUtil.fromBoolean(config.NWC))
		End Sub

		Public Overrides Function iArgs() As Long()
			If iArguments.Count = 0 Then
				addArgs()
			End If

			Return MyBase.iArgs()
		End Function

		Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			If config Is Nothing AndAlso iArguments.Count > 0 Then
				config = Conv1DConfig.builder().k(iArguments(0)).s(iArguments(1)).p(iArguments(2)).d(iArguments(3)).paddingMode(System.Enum.GetValues(GetType(PaddingMode))(iArguments(4).intValue())).dataFormat(If(iArguments(5) = 1, Conv1DConfig.NCW, Conv1DConfig.NWC_Conflict)).build()
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
			Return "conv1d_bp"
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				If args().Length = 4 Then
					Return 3 'Includes bias
				Else
					Return 2 'No bias - only input + weight grads
				End If
			End Get
		End Property

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return inputDataTypes.GetRange(0, inputDataTypes.Count - 1) 'All except gradient input variable
		End Function
	End Class

End Namespace