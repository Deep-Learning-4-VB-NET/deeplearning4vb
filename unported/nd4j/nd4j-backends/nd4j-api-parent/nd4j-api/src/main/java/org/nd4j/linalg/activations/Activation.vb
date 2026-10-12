Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports org.nd4j.linalg.activations.impl

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

Namespace org.nd4j.linalg.activations

	Public NotInheritable Class Activation
		Public Shared ReadOnly CUBE As New Activation("CUBE", InnerEnum.CUBE)
		Public Shared ReadOnly ELU As New Activation("ELU", InnerEnum.ELU)
		Public Shared ReadOnly HARDSIGMOID As New Activation("HARDSIGMOID", InnerEnum.HARDSIGMOID)
		Public Shared ReadOnly HARDTANH As New Activation("HARDTANH", InnerEnum.HARDTANH)
		Public Shared ReadOnly IDENTITY As New Activation("IDENTITY", InnerEnum.IDENTITY)
		Public Shared ReadOnly LEAKYRELU As New Activation("LEAKYRELU", InnerEnum.LEAKYRELU)
		Public Shared ReadOnly RATIONALTANH As New Activation("RATIONALTANH", InnerEnum.RATIONALTANH)
		Public Shared ReadOnly RELU As New Activation("RELU", InnerEnum.RELU)
		Public Shared ReadOnly RELU6 As New Activation("RELU6", InnerEnum.RELU6)
		Public Shared ReadOnly RRELU As New Activation("RRELU", InnerEnum.RRELU)
		Public Shared ReadOnly SIGMOID As New Activation("SIGMOID", InnerEnum.SIGMOID)
		Public Shared ReadOnly SOFTMAX As New Activation("SOFTMAX", InnerEnum.SOFTMAX)
		Public Shared ReadOnly SOFTPLUS As New Activation("SOFTPLUS", InnerEnum.SOFTPLUS)
		Public Shared ReadOnly SOFTSIGN As New Activation("SOFTSIGN", InnerEnum.SOFTSIGN)
		Public Shared ReadOnly TANH As New Activation("TANH", InnerEnum.TANH)
		Public Shared ReadOnly RECTIFIEDTANH As New Activation("RECTIFIEDTANH", InnerEnum.RECTIFIEDTANH)
		Public Shared ReadOnly SELU As New Activation("SELU", InnerEnum.SELU)
		Public Shared ReadOnly SWISH As New Activation("SWISH", InnerEnum.SWISH)
		Public Shared ReadOnly THRESHOLDEDRELU As New Activation("THRESHOLDEDRELU", InnerEnum.THRESHOLDEDRELU)
		Public Shared ReadOnly GELU As New Activation("GELU", InnerEnum.GELU)
		Public Shared ReadOnly MISH As New Activation("MISH", InnerEnum.MISH)

		Private Shared ReadOnly valueList As New List(Of Activation)()

		Shared Sub New()
			valueList.Add(CUBE)
			valueList.Add(ELU)
			valueList.Add(HARDSIGMOID)
			valueList.Add(HARDTANH)
			valueList.Add(IDENTITY)
			valueList.Add(LEAKYRELU)
			valueList.Add(RATIONALTANH)
			valueList.Add(RELU)
			valueList.Add(RELU6)
			valueList.Add(RRELU)
			valueList.Add(SIGMOID)
			valueList.Add(SOFTMAX)
			valueList.Add(SOFTPLUS)
			valueList.Add(SOFTSIGN)
			valueList.Add(TANH)
			valueList.Add(RECTIFIEDTANH)
			valueList.Add(SELU)
			valueList.Add(SWISH)
			valueList.Add(THRESHOLDEDRELU)
			valueList.Add(GELU)
			valueList.Add(MISH)
		End Sub

		Public Enum InnerEnum
			CUBE
			ELU
			HARDSIGMOID
			HARDTANH
			IDENTITY
			LEAKYRELU
			RATIONALTANH
			RELU
			RELU6
			RRELU
			SIGMOID
			SOFTMAX
			SOFTPLUS
			SOFTSIGN
			TANH
			RECTIFIEDTANH
			SELU
			SWISH
			THRESHOLDEDRELU
			GELU
			MISH
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
			nameValue = name
			ordinalValue = nextOrdinal
			nextOrdinal += 1
			innerEnumValue = thisInnerEnumValue
		End Sub

		''' <summary>
		''' Creates an instance of the activation function
		''' </summary>
		''' <returns> an instance of the activation function </returns>
		Public ReadOnly Property ActivationFunction As IActivation
			Get
				Select Case Me
					Case CUBE
						Return New ActivationCube()
					Case ELU
						Return New ActivationELU()
					Case HARDSIGMOID
						Return New ActivationHardSigmoid()
					Case HARDTANH
						Return New ActivationHardTanH()
					Case IDENTITY
						Return New ActivationIdentity()
					Case LEAKYRELU
						Return New ActivationLReLU()
					Case RATIONALTANH
						Return New ActivationRationalTanh()
					Case RECTIFIEDTANH
						Return New ActivationRectifiedTanh()
					Case RELU
						Return New ActivationReLU()
					Case RELU6
						Return New ActivationReLU6()
					Case SELU
						Return New ActivationSELU()
					Case SWISH
						Return New ActivationSwish()
					Case RRELU
						Return New ActivationRReLU()
					Case SIGMOID
						Return New ActivationSigmoid()
					Case SOFTMAX
						Return New ActivationSoftmax()
					Case SOFTPLUS
						Return New ActivationSoftPlus()
					Case SOFTSIGN
						Return New ActivationSoftSign()
					Case TANH
						Return New ActivationTanH()
					Case THRESHOLDEDRELU
						Return New ActivationThresholdedReLU()
					Case GELU
						Return New ActivationGELU()
					Case MISH
						Return New ActivationMish()
					Case Else
						Throw New System.NotSupportedException("Unknown or not supported activation function: " & Me)
				End Select
			End Get
		End Property

		''' <summary>
		''' Returns the activation function enum value
		''' </summary>
		''' <param name="name"> the case-insensitive opName of the activation function </param>
		''' <returns> the activation function enum value </returns>
		Public Shared Function fromString(ByVal name As String) As Activation
			Return Activation.valueOf(name.ToUpper())
		End Function

		''' <summary>
		''' Get the Activation as a SameDiff variable
		''' </summary>
		''' <param name="sd">    SameDiff instance </param>
		''' <param name="input"> Input variable to apply the activation function to </param>
		''' <returns> SDVariable: output after applying the activation function </returns>
		''' <seealso cref= #asSameDiff(SameDiff, SDVariable) </seealso>
		Public Function asSameDiff(ByVal sd As org.nd4j.autodiff.samediff.SameDiff, ByVal input As org.nd4j.autodiff.samediff.SDVariable) As org.nd4j.autodiff.samediff.SDVariable
			Return asSameDiff(Nothing, sd, input)
		End Function

		''' <summary>
		''' Get the Activation as a SameDiff variable
		''' </summary>
		''' <param name="variableName"> Variable name </param>
		''' <param name="sd">           SameDiff instance </param>
		''' <param name="input">        Input variable to apply the activation function to </param>
		''' <returns> SDVariable: output after applying the activation function </returns>
		Public Function asSameDiff(ByVal variableName As String, ByVal sd As org.nd4j.autodiff.samediff.SameDiff, ByVal input As org.nd4j.autodiff.samediff.SDVariable) As org.nd4j.autodiff.samediff.SDVariable
			Select Case Me
				Case CUBE
					Return sd.math().pow(variableName, input, 3.0)
				Case ELU
					Return sd.nn().elu(variableName, input)
				Case HARDTANH
					Return sd.nn().hardTanh(variableName, input)
				Case IDENTITY
					Return sd.identity(variableName, input)
				Case LEAKYRELU
					Return sd.nn().leakyRelu(variableName, input, 0.0)
				Case RELU
					Return sd.nn().relu(variableName, input, 0.0)
				Case SIGMOID
					Return sd.nn().sigmoid(variableName, input)
				Case SOFTMAX
					Return sd.nn().softmax(variableName, input)
				Case SOFTPLUS
					Return sd.nn().softplus(variableName, input)
				Case SOFTSIGN
					Return sd.nn().softsign(variableName, input)
				Case TANH
					Return sd.math().tanh(variableName, input)
				Case GELU
					Return sd.nn().gelu(variableName, input)
				Case Else
					Throw New System.NotSupportedException("Activation function not yet supported: " & Me)
			End Select
		End Function

		Public Shared Function values() As Activation()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As Activation, ByVal two As Activation) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As Activation, ByVal two As Activation) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As Activation
			For Each enumInstance As Activation In Activation.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace