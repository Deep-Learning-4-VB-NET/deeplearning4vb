Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LSTMLayerConfig = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig
Imports LSTMLayerWeights = org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights
Imports Booleans = org.nd4j.shade.guava.primitives.Booleans

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
Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class LSTMLayerBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class LSTMLayerBp
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig configuration;
		Private configuration As LSTMLayerConfig

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights weights;
		Private weights As LSTMLayerWeights

		Private cLast As SDVariable
		Private yLast As SDVariable
		Private maxTSLength As SDVariable


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LSTMLayerBp(@NonNull SameDiff sameDiff, @NonNull SDVariable x, org.nd4j.autodiff.samediff.SDVariable cLast, org.nd4j.autodiff.samediff.SDVariable yLast, org.nd4j.autodiff.samediff.SDVariable maxTSLength, @NonNull LSTMLayerWeights weights, @NonNull LSTMLayerConfig configuration, org.nd4j.autodiff.samediff.SDVariable dLdh, org.nd4j.autodiff.samediff.SDVariable dLdhL, org.nd4j.autodiff.samediff.SDVariable dLdcL)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal maxTSLength As SDVariable, ByVal weights As LSTMLayerWeights, ByVal configuration As LSTMLayerConfig, ByVal dLdh As SDVariable, ByVal dLdhL As SDVariable, ByVal dLdcL As SDVariable)
			MyBase.New("lstmLayer_bp", sameDiff, wrapFilterNull(x, weights.getWeights(), weights.getRWeights(), weights.getBias(), maxTSLength, yLast, cLast, weights.getPeepholeWeights(), dLdh, dLdhL, dLdcL))
			Me.configuration = configuration
			Me.weights = weights
			Me.cLast = cLast
			Me.yLast = yLast
			Me.maxTSLength = maxTSLength
			addIArgument(iArgs())
			addTArgument(tArgs())
			addBArgument(bArgs(weights, maxTSLength, yLast, cLast))


			Preconditions.checkState(Me.configuration.isRetLastH() OrElse Me.configuration.isRetLastC() OrElse Me.configuration.isRetFullSequence(), "You have to specify at least one output you want to return. Use isRetLastC, isRetLast and isRetFullSequence  methods  in LSTMLayerConfig builder to specify them")


		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)

			Dim dt As DataType = inputDataTypes(1)
			Preconditions.checkState(dt.isFPType(), "Input type 1 must be a floating point type, got %s", dt)
			Dim list As New List(Of DataType)()
			list.Add(dt) ' dLdx
			list.Add(dt) ' dLdWx
			list.Add(dt) ' dLdWr

			If Me.weights.hasBias() Then
				list.Add(dt)
			End If ' dLdb

			If Me.maxTSLength IsNot Nothing Then
				list.Add(dt)
			End If ' dLdSl
			If Me.yLast IsNot Nothing Then
				list.Add(dt)
			End If 'dLdhI
			If Me.cLast IsNot Nothing Then
				list.Add(dt)
			End If ' dLdcI
			If Me.weights.hasPH() Then
				list.Add(dt)
			End If ' dLdWp

			Return list
		End Function


		Public Overrides Function opName() As String
			Return "lstmLayer_bp"
		End Function

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Return configuration.toProperties(True, True)
		End Function


		Public Overrides Function iArgs() As Long()
			Return New Long(){ configuration.getLstmdataformat().ordinal(), configuration.getDirectionMode().ordinal(), configuration.getGateAct().ordinal(), configuration.getOutAct().ordinal(), configuration.getCellAct().ordinal() }
		End Function

		Public Overrides Function tArgs() As Double()
			Return New Double(){Me.configuration.getCellClip()} ' T_ARG(0)
		End Function


		Protected Friend Overridable Overloads Function bArgs(Of T)(ByVal weights As LSTMLayerWeights, ByVal maxTSLength As T, ByVal yLast As T, ByVal cLast As T) As Boolean()
			Return New Boolean(){ weights.hasBias(), maxTSLength IsNot Nothing, yLast IsNot Nothing, cLast IsNot Nothing, weights.hasPH(), configuration.isRetFullSequence(), configuration.isRetLastH(), configuration.isRetLastC() }

		End Function

		Public Overrides ReadOnly Property ConfigProperties As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function configFieldName() As String
			Return "configuration"
		End Function


		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
    
				Return Booleans.countTrue(True, True, True, weights.hasBias(), Me.maxTSLength IsNot Nothing, Me.yLast IsNot Nothing, Me.cLast IsNot Nothing, weights.hasPH())
			End Get
		End Property


	End Class



End Namespace