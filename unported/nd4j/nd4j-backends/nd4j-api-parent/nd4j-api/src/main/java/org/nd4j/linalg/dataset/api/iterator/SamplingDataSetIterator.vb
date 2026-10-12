Imports System
Imports System.Collections.Generic
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.nd4j.linalg.dataset.api.iterator


	''' <summary>
	''' A wrapper for a dataset to sample from.
	''' This will randomly sample from the given dataset.
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class SamplingDataSetIterator
		Implements DataSetIterator

		Private sampleFrom As DataSet
		Private batchSize As Integer
		Private totalNumberSamples As Integer
		Private numTimesSampled As Integer
		Private replace As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

		''' <param name="sampleFrom">         the dataset to sample from </param>
		''' <param name="batchSize">          the batch size to sample </param>
		''' <param name="totalNumberSamples"> the sample size </param>
		Public Sub New(ByVal sampleFrom As DataSet, ByVal batchSize As Integer, ByVal totalNumberSamples As Integer, ByVal replace As Boolean)
			MyBase.New()
			Me.sampleFrom = sampleFrom
			Me.batchSize = batchSize
			Me.totalNumberSamples = totalNumberSamples
			Me.replace = replace
		End Sub


		''' <param name="sampleFrom">         the dataset to sample from </param>
		''' <param name="batchSize">          the batch size to sample </param>
		''' <param name="totalNumberSamples"> the sample size </param>
		Public Sub New(ByVal sampleFrom As DataSet, ByVal batchSize As Integer, ByVal totalNumberSamples As Integer)
			MyBase.New()
			Me.sampleFrom = sampleFrom
			Me.batchSize = batchSize
			Me.totalNumberSamples = totalNumberSamples
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return numTimesSampled < totalNumberSamples
		End Function

		Public Overrides Function [next]() As DataSet
			Dim ret As DataSet = sampleFrom.sample(batchSize, replace)
			numTimesSampled += batchSize

			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(ret)
			End If

			Return ret
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return sampleFrom.numInputs()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return sampleFrom.numOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			'Aleady in memory -> async prefetching doesn't make sense here
			Return False
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			numTimesSampled = 0
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batchSize
		End Function

		''' <summary>
		''' Set a pre processor
		''' </summary>
		''' <param name="preProcessor"> a pre processor to set </param>
		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Dim ret As DataSet = sampleFrom.sample(num)
			numTimesSampled += 1
			Return ret
		End Function
	End Class

End Namespace