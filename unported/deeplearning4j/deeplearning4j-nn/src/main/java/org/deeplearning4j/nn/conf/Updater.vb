Imports System.Collections.Generic
Imports org.nd4j.linalg.learning.config

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

Namespace org.deeplearning4j.nn.conf

	''' 
	''' <summary>
	''' All the possible different updaters
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public NotInheritable Class Updater
		Public Shared ReadOnly SGD As New Updater("SGD", InnerEnum.SGD)
		Public Shared ReadOnly ADAM As New Updater("ADAM", InnerEnum.ADAM)
		Public Shared ReadOnly ADAMAX As New Updater("ADAMAX", InnerEnum.ADAMAX)
		Public Shared ReadOnly ADADELTA As New Updater("ADADELTA", InnerEnum.ADADELTA)
		Public Shared ReadOnly NESTEROVS As New Updater("NESTEROVS", InnerEnum.NESTEROVS)
		Public Shared ReadOnly NADAM As New Updater("NADAM", InnerEnum.NADAM)
		Public Shared ReadOnly ADAGRAD As New Updater("ADAGRAD", InnerEnum.ADAGRAD)
		Public Shared ReadOnly RMSPROP As New Updater("RMSPROP", InnerEnum.RMSPROP)
		Public Shared ReadOnly NONE As New Updater("NONE", InnerEnum.NONE)
		<System.Obsolete>
		Public Shared ReadOnly CUSTOM As New Updater("CUSTOM", InnerEnum.CUSTOM)

		Private Shared ReadOnly valueList As New List(Of Updater)()

		Shared Sub New()
			valueList.Add(SGD)
			valueList.Add(ADAM)
			valueList.Add(ADAMAX)
			valueList.Add(ADADELTA)
			valueList.Add(NESTEROVS)
			valueList.Add(NADAM)
			valueList.Add(ADAGRAD)
			valueList.Add(RMSPROP)
			valueList.Add(NONE)
			valueList.Add(CUSTOM)
		End Sub

		Public Enum InnerEnum
			SGD
			ADAM
			ADAMAX
			ADADELTA
			NESTEROVS
			NADAM
			ADAGRAD
			RMSPROP
			NONE
			CUSTOM
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


		Public Shared ReadOnly public IUpdater getIUpdaterWithDefaultConfig() {
			switch(Me) { case SGD: Return New Sgd(); case ADAM: Return New Adam(); case ADAMAX: Return New AdaMax(); case ADADELTA: Return New AdaDelta(); case NESTEROVS: Return New Nesterovs(); case NADAM: Return New Nadam(); case ADAGRAD: Return New AdaGrad(); case RMSPROP: Return New RmsProp(); case NONE: Return New NoOp(); case CUSTOM: default: throw New UnsupportedOperationException("Unknown or not supported updater: " & Me); }
		}
		As New Updater("public IUpdater getIUpdaterWithDefaultConfig() { switch(Me) { case SGD: Return New Sgd(); case ADAM: Return New Adam(); case ADAMAX: Return New AdaMax(); case ADADELTA: Return New AdaDelta(); case NESTEROVS: Return New Nesterovs(); case NADAM: Return New Nadam(); case ADAGRAD: Return New AdaGrad(); case RMSPROP: Return New RmsProp(); case NONE: Return New NoOp(); case CUSTOM: default: throw New UnsupportedOperationException("Unknown or not supported updater: " + Me); } }", InnerEnum.public IUpdater getIUpdaterWithDefaultConfig() {
			switch(Me) { case SGD: Return New Sgd(); case ADAM: Return New Adam(); case ADAMAX: Return New AdaMax(); case ADADELTA: Return New AdaDelta(); case NESTEROVS: Return New Nesterovs(); case NADAM: Return New Nadam(); case ADAGRAD: Return New AdaGrad(); case RMSPROP: Return New RmsProp(); case NONE: Return New NoOp(); case CUSTOM: default: throw New UnsupportedOperationException("Unknown or not supported updater: " & Me); }
		})

		Public Shared Function values() As Updater()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As Updater, ByVal two As Updater) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As Updater, ByVal two As Updater) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As Updater
			For Each enumInstance As Updater In Updater.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace