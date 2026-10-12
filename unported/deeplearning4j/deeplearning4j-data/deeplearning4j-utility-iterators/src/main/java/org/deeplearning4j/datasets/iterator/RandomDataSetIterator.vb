Imports System
Imports System.Collections.Generic
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.datasets.iterator

	<Serializable>
	Public Class RandomDataSetIterator
		Inherits MultiDataSetWrapperIterator

		Public NotInheritable Class Values
			Public Shared ReadOnly RANDOM_UNIFORM As New Values("RANDOM_UNIFORM", InnerEnum.RANDOM_UNIFORM)
			Public Shared ReadOnly RANDOM_NORMAL As New Values("RANDOM_NORMAL", InnerEnum.RANDOM_NORMAL)
			Public Shared ReadOnly ONE_HOT As New Values("ONE_HOT", InnerEnum.ONE_HOT)
			Public Shared ReadOnly ZEROS As New Values("ZEROS", InnerEnum.ZEROS)
			Public Shared ReadOnly ONES As New Values("ONES", InnerEnum.ONES)
			Public Shared ReadOnly BINARY As New Values("BINARY", InnerEnum.BINARY)
			Public Shared ReadOnly INTEGER_0_10 As New Values("INTEGER_0_10", InnerEnum.INTEGER_0_10)
			Public Shared ReadOnly INTEGER_0_100 As New Values("INTEGER_0_100", InnerEnum.INTEGER_0_100)
			Public Shared ReadOnly INTEGER_0_1000 As New Values("INTEGER_0_1000", InnerEnum.INTEGER_0_1000)
			Public Shared ReadOnly INTEGER_0_10000 As New Values("INTEGER_0_10000", InnerEnum.INTEGER_0_10000)
			Public Shared ReadOnly INTEGER_0_100000 As New Values("INTEGER_0_100000", InnerEnum.INTEGER_0_100000)

			Private Shared ReadOnly valueList As New List(Of Values)()

			Shared Sub New()
				valueList.Add(RANDOM_UNIFORM)
				valueList.Add(RANDOM_NORMAL)
				valueList.Add(ONE_HOT)
				valueList.Add(ZEROS)
				valueList.Add(ONES)
				valueList.Add(BINARY)
				valueList.Add(INTEGER_0_10)
				valueList.Add(INTEGER_0_100)
				valueList.Add(INTEGER_0_1000)
				valueList.Add(INTEGER_0_10000)
				valueList.Add(INTEGER_0_100000)
			End Sub

			Public Enum InnerEnum
				RANDOM_UNIFORM
				RANDOM_NORMAL
				ONE_HOT
				ZEROS
				ONES
				BINARY
				INTEGER_0_10
				INTEGER_0_100
				INTEGER_0_1000
				INTEGER_0_10000
				INTEGER_0_100000
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
			Public Function toMdsValues() As RandomMultiDataSetIterator.Values
				Return System.Enum.Parse(GetType(RandomMultiDataSetIterator.Values), Me.ToString())
			End Function

			Public Shared Function values() As Values()
				Return valueList.ToArray()
			End Function

			Public Function ordinal() As Integer
				Return ordinalValue
			End Function

			Public Overrides Function ToString() As String
				Return nameValue
			End Function

			Public Shared Operator =(ByVal one As Values, ByVal two As Values) As Boolean
				Return one.innerEnumValue = two.innerEnumValue
			End Operator

			Public Shared Operator <>(ByVal one As Values, ByVal two As Values) As Boolean
				Return one.innerEnumValue <> two.innerEnumValue
			End Operator

			Public Shared Function valueOf(ByVal name As String) As Values
				For Each enumInstance As Values In Values.valueList
					If enumInstance.nameValue = name Then
						Return enumInstance
					End If
				Next
				Throw New System.ArgumentException(name)
			End Function
		End Class

		''' <param name="numMiniBatches"> Number of minibatches per epoch </param>
		''' <param name="featuresShape">  Features shape </param>
		''' <param name="labelsShape">    Labels shape </param>
		''' <param name="featureValues">  Type of values for the features </param>
		''' <param name="labelValues">    Type of values for the labels </param>
		Public Sub New(ByVal numMiniBatches As Integer, ByVal featuresShape() As Long, ByVal labelsShape() As Long, ByVal featureValues As Values, ByVal labelValues As Values)
			Me.New(numMiniBatches, featuresShape, labelsShape, featureValues, labelValues, Nd4j.order(), Nd4j.order())
		End Sub

		''' <param name="numMiniBatches"> Number of minibatches per epoch </param>
		''' <param name="featuresShape">  Features shape </param>
		''' <param name="labelsShape">    Labels shape </param>
		''' <param name="featureValues">  Type of values for the features </param>
		''' <param name="labelValues">    Type of values for the labels </param>
		''' <param name="featuresOrder">  Array order ('c' or 'f') for the features array </param>
		''' <param name="labelsOrder">    Array order ('c' or 'f') for the labels array </param>
		Public Sub New(ByVal numMiniBatches As Integer, ByVal featuresShape() As Long, ByVal labelsShape() As Long, ByVal featureValues As Values, ByVal labelValues As Values, ByVal featuresOrder As Char, ByVal labelsOrder As Char)
			MyBase.New((New RandomMultiDataSetIterator.Builder(numMiniBatches)).addFeatures(featuresShape, featuresOrder, featureValues.toMdsValues()).addLabels(labelsShape, labelsOrder, labelValues.toMdsValues()).build())
		End Sub

	End Class

End Namespace