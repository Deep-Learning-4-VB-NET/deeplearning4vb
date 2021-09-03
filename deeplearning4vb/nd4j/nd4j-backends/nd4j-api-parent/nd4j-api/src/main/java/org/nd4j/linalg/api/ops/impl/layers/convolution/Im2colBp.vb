Imports System.Collections.Generic
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



	Public Class Im2colBp
		Inherits DynamicCustomOp

		Protected Friend conv2DConfig As Conv2DConfig

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i2cInput As SDVariable, ByVal gradAtOutput As SDVariable, ByVal conv2DConfig As Conv2DConfig)
			MyBase.New(Nothing, sameDiff,New SDVariable(){i2cInput, gradAtOutput})
			Me.conv2DConfig = conv2DConfig
			addArgs()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal config As Conv2DConfig)
			MyBase.New(Nothing, sd, New SDVariable(){input})
			Me.conv2DConfig = config
			addArgs()
		End Sub

		Public Sub New()
		End Sub

		Protected Friend Overridable Sub addArgs()
			addIArgument(conv2DConfig.getKH())
			addIArgument(conv2DConfig.getKW())
			addIArgument(conv2DConfig.getSH())
			addIArgument(conv2DConfig.getSW())
			addIArgument(conv2DConfig.getPH())
			addIArgument(conv2DConfig.getPW())
			addIArgument(conv2DConfig.getDH())
			addIArgument(conv2DConfig.getDW())
			addIArgument(ArrayUtil.fromBoolean(conv2DConfig.isSameMode()))
		End Sub


		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Return conv2DConfig.toProperties()
		End Function

		Public Overrides Function opName() As String
			Return "im2col_bp"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentiation not supported for this op: " & Me.GetType().Name)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected 2 input data types for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace