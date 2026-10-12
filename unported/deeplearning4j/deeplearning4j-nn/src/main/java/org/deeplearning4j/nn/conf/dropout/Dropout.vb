Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports HelperUtils = org.deeplearning4j.nn.layers.HelperUtils
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports DropOutInverted = org.nd4j.linalg.api.ops.random.impl.DropOutInverted
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
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
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"mask", "helper", "helperCountFail", "initializedHelper"}) @EqualsAndHashCode(exclude = {"mask", "helper", "helperCountFail", "initializedHelper"}) @Slf4j public class Dropout implements IDropout
	<Serializable>
	Public Class Dropout
		Implements IDropout

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected boolean helperAllowFallback = true;
'JAVA TO VB CONVERTER NOTE: The field helperAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend helperAllowFallback_Conflict As Boolean = True

		Private p As Double
		Private pSchedule As ISchedule
		<NonSerialized>
		Private mask As INDArray
		<NonSerialized>
		Private helper As DropoutHelper
		Private initializedHelper As Boolean = False

		Private helperCountFail As Integer = 0

		Public Const CUDNN_DROPOUT_HELPER_CLASS_NAME As String = "org.deeplearning4j.cuda.dropout.CudnnDropoutHelper"

		''' <param name="activationRetainProbability"> Probability of retaining an activation - see <seealso cref="Dropout"/> javadoc </param>
		Public Sub New(ByVal activationRetainProbability As Double)
			Me.New(activationRetainProbability, Nothing)
			If activationRetainProbability < 0.0 Then
				Throw New System.ArgumentException("Activation retain probability must be > 0. Got: " & activationRetainProbability)
			End If
			If activationRetainProbability = 0.0 Then
				Throw New System.ArgumentException("Invalid probability value: Dropout with 0.0 probability of retaining " & "activations is not supported")
			End If
		End Sub

		''' <param name="activationRetainProbabilitySchedule"> Schedule for probability of retaining an activation - see <seealso cref="Dropout"/> javadoc </param>
		Public Sub New(ByVal activationRetainProbabilitySchedule As ISchedule)
			Me.New(Double.NaN, activationRetainProbabilitySchedule)
		End Sub

		''' <summary>
		''' When using a helper (CuDNN or MKLDNN in some cases) and an error is encountered, should fallback to the non-helper implementation be allowed?
		''' If set to false, an exception in the helper will be propagated back to the user. If false, the built-in
		''' (non-helper) implementation for Dropout will be used
		''' </summary>
		''' <param name="allowFallback"> Whether fallback to non-helper implementation should be used </param>
		Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As Dropout
			Me.setHelperAllowFallback(allowFallback)
			Return Me
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected Dropout(@JsonProperty("p") double activationRetainProbability, @JsonProperty("pSchedule") org.nd4j.linalg.schedule.ISchedule activationRetainProbabilitySchedule)
		Protected Friend Sub New(ByVal activationRetainProbability As Double, ByVal activationRetainProbabilitySchedule As ISchedule)
			Me.p = activationRetainProbability
			Me.pSchedule = activationRetainProbabilitySchedule
		End Sub

		''' <summary>
		''' Initialize the CuDNN dropout helper, if possible
		''' </summary>
		Protected Friend Overridable Sub initializeHelper(ByVal dataType As DataType)
			helper = HelperUtils.createHelper(CUDNN_DROPOUT_HELPER_CLASS_NAME, "", GetType(DropoutHelper), "dropout-helper", dataType)

			initializedHelper = helper IsNot Nothing
		End Sub

		Public Overridable Function applyDropout(ByVal inputActivations As INDArray, ByVal output As INDArray, ByVal iteration As Integer, ByVal epoch As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IDropout.applyDropout
			Preconditions.checkState(output.dataType().isFPType(), "Output array must be a floating point type, got %s for array of shape %ndShape", output.dataType(), output)
			Dim currP As Double
			If pSchedule IsNot Nothing Then
				currP = pSchedule.valueAt(iteration, epoch)
			Else
				currP = p
			End If

			If Not initializedHelper Then
				initializeHelper(output.dataType())
			End If

			If helper IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not isHelperAllowFallback()) Then
				Dim helperWorked As Boolean = False
				Try
					helper.applyDropout(inputActivations, output, p)
					helperWorked = True
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If isHelperAllowFallback() Then
						helperCountFail += 1
						log.warn("CuDNN execution failed - falling back on built-in implementation",e)
					Else
						Throw New Exception("Error during Dropout CuDNN helper forward pass - helperAllowFallback() is set to false", e)
					End If
				End Try

				If helperWorked Then
					Return output
				End If
			End If

			Dim inputCast As INDArray = inputActivations
			If inputCast IsNot output AndAlso inputCast.dataType() <> output.dataType() Then
				inputCast = inputCast.castTo(output.dataType())
			End If

			mask = workspaceMgr.createUninitialized(ArrayType.INPUT, output.dataType(), output.shape(), output.ordering()).assign(1.0)
			Nd4j.Executioner.exec(New DropOutInverted(mask, mask, currP))
			Nd4j.Executioner.exec(New MulOp(inputCast, mask, output))
			Return output
		End Function

		Public Overridable Function backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As INDArray Implements IDropout.backprop
			If helper IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not isHelperAllowFallback()) Then
				Dim helperWorked As Boolean = False
				Try
					helper.backprop(gradAtOutput, gradAtInput)
					helperWorked = True
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If isHelperAllowFallback() Then
						helperCountFail += 1
						log.warn("CuDNN execution failed - falling back on built-in implementation",e)
					Else
						Throw New Exception("Error during Dropout CuDNN helper backprop - helperAllowFallback() is set to false", e)
					End If
				End Try

				If helperWorked Then
					Return gradAtInput
				End If
			End If

			Preconditions.checkState(mask IsNot Nothing, "Cannot perform backprop: Dropout mask array is absent (already cleared?)")
			'dL/dx = dL/dz * dz/dx, with z=0 or x/p
			'Mask already contains either 0 or 1/p, so just muli
			Dim m As INDArray = mask
			If m.dataType() <> gradAtInput.dataType() Then
				m = m.castTo(gradAtInput.dataType())
			End If
			Nd4j.Executioner.exec(New MulOp(gradAtOutput, m, gradAtInput))
			mask = Nothing
			Return gradAtInput
		End Function

		Public Overridable Sub clear() Implements IDropout.clear
			mask = Nothing
		End Sub

		Public Overridable Function clone() As Dropout
			Return New Dropout(p,If(pSchedule Is Nothing, Nothing, pSchedule.clone()))
		End Function
	End Class

End Namespace