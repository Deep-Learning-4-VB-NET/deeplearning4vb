﻿Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig

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
'ORIGINAL LINE: @Slf4j public class SConv2DDerivative extends SConv2D
	Public Class SConv2DDerivative
		Inherits SConv2D

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder(builderMethodName = "sDerviativeBuilder") public SConv2DDerivative(org.nd4j.autodiff.samediff.SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable[] inputFunctions, org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig conv2DConfig)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal inputFunctions() As SDVariable, ByVal conv2DConfig As Conv2DConfig)
			MyBase.New(sameDiff, inputFunctions, conv2DConfig)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "sconv2d_bp"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Throw New NoOpNameFoundException("No op name found for backwards")
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No op name found for backwards.")
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No op name found for backwards")
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Unable to take derivative of derivative.")
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				'Inputs: in, gradAtOutput, weightsDepth, optional weightsPoint, optional weightsBias       3 req, 2 optional
				'Outputs: gradAtInput, gradWD, optional gradWP, optional gradB                             2 req, 2 optional
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