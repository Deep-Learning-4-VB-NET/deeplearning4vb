Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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
'ORIGINAL LINE: @Slf4j public class VariableMultiTimeseriesGenerator implements org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
	<Serializable>
	Public Class VariableMultiTimeseriesGenerator
		Implements MultiDataSetIterator

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


		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
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

			Return New org.nd4j.linalg.dataset.MultiDataSet(New INDArray() {features}, New INDArray() {labels}, New INDArray() {fMasks}, New INDArray() {lMasks})
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				' no-op
			End Set
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			isFirst = True
			counter.set(0)
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return counter.get() < limit
		End Function

		Public Overrides Function [next]() As MultiDataSet
			Return [next](batchSize)
		End Function

		Public Overrides Sub remove()

		End Sub
	End Class

End Namespace