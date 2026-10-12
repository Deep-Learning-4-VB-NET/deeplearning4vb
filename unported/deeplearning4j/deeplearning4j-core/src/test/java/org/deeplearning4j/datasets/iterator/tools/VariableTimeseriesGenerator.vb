Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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

Namespace org.deeplearning4j.datasets.iterator.tools


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class VariableTimeseriesGenerator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class VariableTimeseriesGenerator
		Implements DataSetIterator

		Protected Friend rng As Random
		Protected Friend batchSize As Integer
		Protected Friend values As Integer
		Protected Friend minTS, maxTS As Integer
		Protected Friend limit As Integer
		Protected Friend firstMaxima As Integer = 0
		Protected Friend isFirst As Boolean = True

		Protected Friend counter As New AtomicInteger(0)

		Public Sub New(ByVal seed As Long, ByVal numBatches As Integer, ByVal batchSize As Integer, ByVal values As Integer, ByVal timestepsMin As Integer, ByVal timestepsMax As Integer)
			Me.New(seed, numBatches, batchSize, values, timestepsMin, timestepsMax, 0)
		End Sub

		Public Sub New(ByVal seed As Long, ByVal numBatches As Integer, ByVal batchSize As Integer, ByVal values As Integer, ByVal timestepsMin As Integer, ByVal timestepsMax As Integer, ByVal firstMaxima As Integer)
			Me.rng = New Random(seed)
			Me.values = values
			Me.batchSize = batchSize
			Me.limit = numBatches
			Me.maxTS = timestepsMax
			Me.minTS = timestepsMin
			Me.firstMaxima = firstMaxima

			If timestepsMax < timestepsMin Then
				Throw New DL4JInvalidConfigException("timestepsMin should be <= timestepsMax")
			End If
		End Sub


		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Dim localMaxima As Integer = If(isFirst AndAlso firstMaxima > 0, firstMaxima, If(minTS = maxTS, minTS, rng.Next(maxTS - minTS) + minTS))

	'        if (isFirst)
	'            log.info("Local maxima: {}", localMaxima);

			isFirst = False


			Dim shapeFeatures() As Integer = {batchSize, values, localMaxima}
			Dim shapeLabels() As Integer = {batchSize, 10}
			Dim shapeFMasks() As Integer = {batchSize, localMaxima}
			Dim shapeLMasks() As Integer = {batchSize, 10}
			'log.info("Allocating dataset seqnum: {}", counter.get());
			Dim features As INDArray = Nd4j.createUninitialized(shapeFeatures).assign(counter.get())
			Dim labels As INDArray = Nd4j.createUninitialized(shapeLabels).assign(counter.get() + 0.25)
			Dim fMasks As INDArray = Nd4j.createUninitialized(shapeFMasks).assign(counter.get() + 0.50)
			Dim lMasks As INDArray = Nd4j.createUninitialized(shapeLMasks).assign(counter.get() + 0.75)


			counter.getAndIncrement()

			Return New DataSet(features, labels, fMasks, lMasks)
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				' no-op
			End Set
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			isFirst = True
			counter.set(0)
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return counter.get() < limit
		End Function

		Public Overrides Function [next]() As DataSet
			Return [next](batchSize)
		End Function

		Public Overrides Sub remove()

		End Sub

		''' <summary>
		''' Input columns for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return 0
		End Function

		''' <summary>
		''' The number of labels for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return 0
		End Function

		''' <summary>
		''' Batch size
		''' 
		''' @return
		''' </summary>
		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return 0
		End Function

		''' <summary>
		''' Get dataset iterator record reader labels
		''' </summary>
		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property
	End Class

End Namespace