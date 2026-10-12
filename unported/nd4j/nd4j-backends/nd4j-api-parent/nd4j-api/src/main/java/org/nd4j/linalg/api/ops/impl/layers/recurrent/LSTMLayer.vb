Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
'ORIGINAL LINE: @NoArgsConstructor public class LSTMLayer extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class LSTMLayer
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
'ORIGINAL LINE: public LSTMLayer(@NonNull SameDiff sameDiff, org.nd4j.autodiff.samediff.SDVariable x, org.nd4j.autodiff.samediff.SDVariable cLast, org.nd4j.autodiff.samediff.SDVariable yLast, org.nd4j.autodiff.samediff.SDVariable maxTSLength, org.nd4j.linalg.api.ops.impl.layers.recurrent.weights.LSTMLayerWeights weights, org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMLayerConfig configuration)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal cLast As SDVariable, ByVal yLast As SDVariable, ByVal maxTSLength As SDVariable, ByVal weights As LSTMLayerWeights, ByVal configuration As LSTMLayerConfig)
			MyBase.New(Nothing, sameDiff, weights.argsWithInputs(x, maxTSLength, cLast, yLast))
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

		Public Sub New(ByVal x As INDArray, ByVal cLast As INDArray, ByVal yLast As INDArray, ByVal maxTSLength As INDArray, ByVal lstmWeights As LSTMLayerWeights, ByVal LSTMLayerConfig As LSTMLayerConfig)
			MyBase.New(Nothing, Nothing, lstmWeights.argsWithInputs(maxTSLength, x, cLast, yLast))
			Me.configuration = LSTMLayerConfig
			Me.weights = lstmWeights
			addIArgument(iArgs())
			addTArgument(tArgs())
			addBArgument(bArgs(weights, maxTSLength, yLast, cLast))

			Preconditions.checkState(Me.configuration.isRetLastH() OrElse Me.configuration.isRetLastC() OrElse Me.configuration.isRetFullSequence(), "You have to specify at least one output you want to return. Use isRetLastC, isRetLast and isRetFullSequence  methods  in LSTMLayerConfig builder to specify them")
		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso 3 <= inputDataTypes.Count AndAlso inputDataTypes.Count <= 8, "Expected amount of inputs to LSTMLayer between 3 inputs minimum (input, Wx, Wr only) or 8 maximum, got %s", inputDataTypes)
			'7 outputs, all of same type as input. Note that input 0 is max sequence length (int64), input 1 is actual input
			Dim dt As DataType = inputDataTypes(1)
			Dim list As New List(Of DataType)()
			If configuration.isRetFullSequence() Then

				list.Add(dt)
			End If

			If configuration.isRetLastC() Then

				list.Add(dt)
			End If
			If configuration.isRetLastH() Then

				list.Add(dt)
			End If

			Preconditions.checkState(dt.isFPType(), "Input type 1 must be a floating point type, got %s", dt)
			Return list
		End Function

		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim i As Integer=0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.nd4j.autodiff.samediff.SDVariable grad0 = this.configuration.isRetFullSequence() ? grads.get(i++): null;
			Dim grad0 As SDVariable = If(Me.configuration.isRetFullSequence(), grads(i), Nothing)
				i += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.nd4j.autodiff.samediff.SDVariable grad1 = this.configuration.isRetLastH() ? grads.get(i++): null;
			Dim grad1 As SDVariable = If(Me.configuration.isRetLastH(), grads(i), Nothing)
				i += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: org.nd4j.autodiff.samediff.SDVariable grad2 = this.configuration.isRetLastC() ? grads.get(i++): null;
			Dim grad2 As SDVariable = If(Me.configuration.isRetLastC(), grads(i), Nothing)
				i += 1

			Return New List(Of SDVariable) From {(New LSTMLayerBp(sameDiff, arg(0), Me.cLast, Me.yLast, Me.maxTSLength, Me.weights, Me.configuration, grad0, grad1,grad2)).outputVariables()}
		End Function


		Public Overrides Function opName() As String
			Return "lstmLayer"
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
    
				Return Booleans.countTrue(configuration.isRetFullSequence(), configuration.isRetLastH(), configuration.isRetLastC())
			End Get
		End Property




	End Class



End Namespace