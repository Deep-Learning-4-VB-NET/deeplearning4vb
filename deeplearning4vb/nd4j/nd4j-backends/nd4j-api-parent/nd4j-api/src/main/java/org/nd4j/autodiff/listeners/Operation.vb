Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff

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

Namespace org.nd4j.autodiff.listeners


	Public NotInheritable Class Operation
		''' <summary>
		''' The training operation: <seealso cref="SameDiff.fit()"/> methods training step (everything except validation).
		''' </summary>
		Public Shared ReadOnly TRAINING As New Operation("TRAINING", InnerEnum.TRAINING)
		''' <summary>
		''' The training validation operation: the validation step during <seealso cref="SameDiff.fit()"/> methods.
		''' </summary>
		Public Shared ReadOnly TRAINING_VALIDATION As New Operation("TRAINING_VALIDATION", InnerEnum.TRAINING_VALIDATION)
		''' <summary>
		''' Inference operations: <seealso cref="SameDiff.output()"/>, <seealso cref="SameDiff.batchOutput()"/> and <seealso cref="SameDiff.exec(Map, String...)"/> ()} methods,
		''' as well as <seealso cref="SameDiff.execBackwards(Map, Operation, String...)"/> methods.
		''' </summary>
		Public Shared ReadOnly INFERENCE As New Operation("INFERENCE", InnerEnum.INFERENCE)
		''' <summary>
		''' Evaluation operations: <seealso cref="SameDiff.evaluate()"/> methods.
		''' </summary>
		Public Shared ReadOnly EVALUATION As New Operation("EVALUATION", InnerEnum.EVALUATION)

		Private Shared ReadOnly valueList As New List(Of Operation)()

		Shared Sub New()
			valueList.Add(TRAINING)
			valueList.Add(TRAINING_VALIDATION)
			valueList.Add(INFERENCE)
			valueList.Add(EVALUATION)
		End Sub

		Public Enum InnerEnum
			TRAINING
			TRAINING_VALIDATION
			INFERENCE
			EVALUATION
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

		Public ReadOnly Property TrainingPhase As Boolean
			Get
				Return Me = TRAINING OrElse Me = TRAINING_VALIDATION
			End Get
		End Property

		Public ReadOnly Property Validation As Boolean
			Get
				Return Me = TRAINING_VALIDATION
			End Get
		End Property


		Public Shared Function values() As Operation()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As Operation, ByVal two As Operation) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As Operation, ByVal two As Operation) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As Operation
			For Each enumInstance As Operation In Operation.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace