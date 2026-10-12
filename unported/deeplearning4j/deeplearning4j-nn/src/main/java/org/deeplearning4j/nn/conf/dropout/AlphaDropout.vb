Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports ToString = lombok.ToString
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.conf.dropout

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"lastPValue","alphaPrime","a","b", "mask"}) @ToString(exclude = {"lastPValue","alphaPrime","a","b"}) @JsonIgnoreProperties({"lastPValue", "alphaPrime", "a", "b", "mask"}) public class AlphaDropout implements IDropout
	<Serializable>
	Public Class AlphaDropout
		Implements IDropout

		Public Const DEFAULT_ALPHA As Double = 1.6732632423543772
		Public Const DEFAULT_LAMBDA As Double = 1.0507009873554804


		Private ReadOnly p As Double
		Private ReadOnly pSchedule As ISchedule
		Private ReadOnly alpha As Double
		Private ReadOnly lambda As Double

		<NonSerialized>
		Private lastPValue As Double
		Private alphaPrime As Double
'JAVA TO VB CONVERTER NOTE: The field a was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private a_Conflict As Double
'JAVA TO VB CONVERTER NOTE: The field b was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private b_Conflict As Double

		<NonSerialized>
		Private mask As INDArray

		''' <param name="activationRetainProbability"> Probability of retaining an activation. See <seealso cref="AlphaDropout"/> javadoc </param>
		Public Sub New(ByVal activationRetainProbability As Double)
			Me.New(activationRetainProbability, Nothing, DEFAULT_ALPHA, DEFAULT_LAMBDA)
			If activationRetainProbability < 0.0 Then
				Throw New System.ArgumentException("Activation retain probability must be > 0. Got: " & activationRetainProbability)
			End If
			If activationRetainProbability = 0.0 Then
				Throw New System.ArgumentException("Invalid probability value: Dropout with 0.0 probability of retaining " & "activations is not supported")
			End If
		End Sub

		''' <param name="activationRetainProbabilitySchedule"> Schedule for the probability of retaining an activation. See
		'''  <seealso cref="AlphaDropout"/> javadoc </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AlphaDropout(@NonNull ISchedule activationRetainProbabilitySchedule)
		Public Sub New(ByVal activationRetainProbabilitySchedule As ISchedule)
			Me.New(Double.NaN, activationRetainProbabilitySchedule, DEFAULT_ALPHA, DEFAULT_LAMBDA)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected AlphaDropout(@JsonProperty("p")double activationRetainProbability, @JsonProperty("pSchedule") org.nd4j.linalg.schedule.ISchedule activationRetainProbabilitySchedule, @JsonProperty("alpha") double alpha, @JsonProperty("lambda") double lambda)
		Protected Friend Sub New(ByVal activationRetainProbability As Double, ByVal activationRetainProbabilitySchedule As ISchedule, ByVal alpha As Double, ByVal lambda As Double)
			Me.p = activationRetainProbability
			Me.pSchedule = activationRetainProbabilitySchedule
			Me.alpha = alpha
			Me.lambda = lambda

			Me.alphaPrime = -lambda * alpha
			If activationRetainProbabilitySchedule Is Nothing Then
				Me.lastPValue = p
				Me.a_Conflict = a(p)
				Me.b_Conflict = b(p)
			End If
		End Sub

		Public Overridable Function applyDropout(ByVal inputActivations As INDArray, ByVal output As INDArray, ByVal iteration As Integer, ByVal epoch As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IDropout.applyDropout
			'https://arxiv.org/pdf/1706.02515.pdf pg6
			' "...we propose “alpha dropout”, that randomly sets inputs to α'"
			' "The affine transformation a(xd + α'(1−d))+b allows to determine parameters a and b such that mean and
			' variance are kept to their values"

			Dim pValue As Double
			If pSchedule IsNot Nothing Then
				pValue = pSchedule.valueAt(iteration, epoch)
			Else
				pValue = p
			End If

			If pValue <> lastPValue Then
				a_Conflict = a(pValue)
				b_Conflict = b(pValue)
			End If
			lastPValue = pValue

			mask = workspaceMgr.createUninitialized(ArrayType.INPUT, output.dataType(), output.shape(), output.ordering())
			Nd4j.Executioner.exec(New BernoulliDistribution(mask, pValue))

			'a * (x * d + alphaPrime * (1-d)) + b
			Dim inverseMask As INDArray = mask.rsub(1.0)
			Dim aPOneMinusD As INDArray = inverseMask.muli(alphaPrime)
			Nd4j.Executioner.exec(New MulOp(inputActivations, mask, output)) 'out = x * d
			output.addi(aPOneMinusD).muli(a_Conflict).addi(b_Conflict)

			'Nd4j.getExecutioner().exec(new AlphaDropOut(inputActivations, output, p, a, alphaPrime, b));
			Return output
		End Function

		Public Overridable Function backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As INDArray Implements IDropout.backprop
			Preconditions.checkState(mask IsNot Nothing, "Cannot perform backprop: Dropout mask array is absent (already cleared?)")
			'dL/dIn = dL/dOut * dOut/dIn
			' dOut/dIn = 0 if dropped (d=0), or a otherwise (d=1)
			mask.muli(a_Conflict)
			Nd4j.Executioner.exec(New MulOp(gradAtOutput, mask, gradAtInput))
			mask = Nothing
			Return gradAtInput
		End Function

		Public Overridable Sub clear() Implements IDropout.clear
			mask = Nothing
		End Sub

		Public Overridable Function clone() As AlphaDropout
			Return New AlphaDropout(p,If(pSchedule Is Nothing, Nothing, pSchedule.clone()), alpha, lambda)
		End Function

		Public Overridable Function a(ByVal p As Double) As Double
			Return 1.0 / Math.Sqrt(p + alphaPrime*alphaPrime * p * (1-p))
		End Function

		Public Overridable Function b(ByVal p As Double) As Double
			Return -a(p) * (1-p)*alphaPrime
		End Function
	End Class

End Namespace