Imports System.Collections.Generic
Imports NamespaceOps = org.nd4j.codegen.api.NamespaceOps
Imports org.nd4j.codegen.ops

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.codegen

	Public NotInheritable Class [Namespace]
		Public Shared ReadOnly BITWISE As New [Namespace]("BITWISE", InnerEnum.BITWISE)
		Public Shared ReadOnly NEURALNETWORK As New [Namespace]("NEURALNETWORK", InnerEnum.NEURALNETWORK)
		Public Shared ReadOnly RANDOM As New [Namespace]("RANDOM", InnerEnum.RANDOM)
		Public Shared ReadOnly IMAGE As New [Namespace]("IMAGE", InnerEnum.IMAGE)
		Public Shared ReadOnly CNN As New [Namespace]("CNN", InnerEnum.CNN)
		Public Shared ReadOnly RNN As New [Namespace]("RNN", InnerEnum.RNN)
		Public Shared ReadOnly MATH As New [Namespace]("MATH", InnerEnum.MATH)
		Public Shared ReadOnly BASE As New [Namespace]("BASE", InnerEnum.BASE)
		Public Shared ReadOnly LOSS As New [Namespace]("LOSS", InnerEnum.LOSS)
		Public Shared ReadOnly LINALG As New [Namespace]("LINALG", InnerEnum.LINALG)

		Private Shared ReadOnly valueList As New List(Of [Namespace])()

		Shared Sub New()
			valueList.Add(BITWISE)
			valueList.Add(NEURALNETWORK)
			valueList.Add(RANDOM)
			valueList.Add(IMAGE)
			valueList.Add(CNN)
			valueList.Add(RNN)
			valueList.Add(MATH)
			valueList.Add(BASE)
			valueList.Add(LOSS)
			valueList.Add(LINALG)
		End Sub

		Public Enum InnerEnum
			BITWISE
			NEURALNETWORK
			RANDOM
			IMAGE
			CNN
			RNN
			MATH
			BASE
			LOSS
			LINALG
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


		Public Shared Function fromString(ByVal [in] As String) As [Namespace]
			Select Case [in].ToLower()
				Case "bitwise"
					Return BITWISE
				Case "nn", "neuralnetwork"
					Return NEURALNETWORK
				Case "random"
					Return RANDOM
				Case "math"
					Return MATH
				Case "image"
					Return IMAGE
				Case "cnn"
					Return CNN
				Case "rnn"
					Return RNN
				Case "base"
					Return BASE
				Case "loss"
					Return LOSS
				Case "linalg"
					Return LINALG
				Case Else
					Return Nothing
			End Select
		End Function

		Public Function javaClassName() As String
			Select Case Me
				Case BITWISE
					Return "NDBitwise"
				Case NEURALNETWORK
					Return "NDNN"
				Case RANDOM
					Return "NDRandom"
				Case IMAGE
					Return "NDImage"
				Case CNN
					Return "NDCNN"
				Case RNN
					Return "NDRNN"
				Case MATH
					Return "NDMath"
				Case BASE
					Return "NDBase"
				Case LOSS
					Return "NDLoss"
				Case LINALG
					Return "NDLinalg"
			End Select
			Throw New System.InvalidOperationException("No java class name defined for: " & Me)
		End Function

		Public Function javaSameDiffClassName() As String
			Select Case Me
				Case BITWISE
					Return "SDBitwise"
				Case NEURALNETWORK
					Return "SDNN"
				Case RANDOM
					Return "SDRandom"
				Case IMAGE
					Return "SDImage"
				Case CNN
					Return "SDCNN"
				Case RNN
					Return "SDRNN"
				Case MATH
					Return "SDMath"
				Case BASE
					Return "SDBaseOps"
				Case LOSS
					Return "SDLoss"
	'            case VALIDATION:
	'                return "SDValidation";
				Case LINALG
					Return "SDLinalg"
			End Select
			Throw New System.InvalidOperationException("No java SameDiff class name defined for: " & Me)
		End Function

		Public ReadOnly Property Namespace As org.nd4j.codegen.api.NamespaceOps
			Get
				Select Case Me
					Case BITWISE
						Return BitwiseKt.Bitwise()
					Case RANDOM
						Return RandomKt.Random()
					Case MATH
						Return MathKt.Math()
					Case IMAGE
						Return ImageKt.SDImage()
					Case CNN
						Return CNNKt.SDCNN()
					Case RNN
						Return RNNKt.SDRNN()
					Case NEURALNETWORK
						Return NeuralNetworkKt.NN()
					Case BASE
						Return SDBaseOpsKt.SDBaseOps()
					Case LOSS
						Return SDLossKt.SDLoss()
					Case LINALG
						Return LinalgKt.Linalg()
				End Select
				Throw New System.InvalidOperationException("No namespace definition available for: " & Me)
			End Get
		End Property

		Public Shared Function values() As [Namespace]()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As [Namespace], ByVal two As [Namespace]) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As [Namespace], ByVal two As [Namespace]) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As [Namespace]
			For Each enumInstance As [Namespace] In [Namespace].valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace