Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives

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
	Public Class RandomMultiDataSetIterator
		Implements MultiDataSetIterator

		Public Enum Values
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
		Private ReadOnly numMiniBatches As Integer
		Private ReadOnly features As IList(Of Triple(Of Long(), Char, Values))
		Private ReadOnly labels As IList(Of Triple(Of Long(), Char, Values))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor preProcessor;
		Private preProcessor As MultiDataSetPreProcessor

		Private position As Integer

		''' <param name="numMiniBatches"> Number of minibatches per epoch </param>
		''' <param name="features">       Each triple in the list specifies the shape, array order and type of values for the features arrays </param>
		''' <param name="labels">         Each triple in the list specifies the shape, array order and type of values for the labels arrays </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomMultiDataSetIterator(int numMiniBatches, @NonNull List<org.nd4j.common.primitives.Triple<long[], Char, Values>> features, @NonNull List<org.nd4j.common.primitives.Triple<long[], Char, Values>> labels)
		Public Sub New(ByVal numMiniBatches As Integer, ByVal features As IList(Of Triple(Of Long(), Char, Values)), ByVal labels As IList(Of Triple(Of Long(), Char, Values)))
			Preconditions.checkArgument(numMiniBatches > 0, "Number of minibatches must be positive: got %s", numMiniBatches)
			Preconditions.checkArgument(features.Count > 0, "No features defined")
			Preconditions.checkArgument(labels.Count > 0, "No labels defined")

			Me.numMiniBatches = numMiniBatches
			Me.features = features
			Me.labels = labels
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As MultiDataSet Implements MultiDataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return [next]()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			position = 0
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return position < numMiniBatches
		End Function

		Public Overrides Function [next]() As MultiDataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If
			Dim f(features.Count - 1) As INDArray
			Dim l(labels.Count - 1) As INDArray

			For i As Integer = 0 To f.Length - 1
				Dim t As Triple(Of Long(), Char, Values) = features(i)
				f(i) = generate(t.getFirst(), t.getSecond(), t.getThird())
			Next i

			For i As Integer = 0 To l.Length - 1
				Dim t As Triple(Of Long(), Char, Values) = labels(i)
				l(i) = generate(t.getFirst(), t.getSecond(), t.getThird())
			Next i

			position += 1
			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(f,l)
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(mds)
			End If
			Return mds
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Class Builder

			Friend numMiniBatches As Integer
			Friend features As IList(Of Triple(Of Long(), Char, Values)) = New List(Of Triple(Of Long(), Char, Values))()
			Friend labels As IList(Of Triple(Of Long(), Char, Values)) = New List(Of Triple(Of Long(), Char, Values))()

			''' <param name="numMiniBatches"> Number of minibatches per epoch </param>
			Public Sub New(ByVal numMiniBatches As Integer)
				Me.numMiniBatches = numMiniBatches
			End Sub

			''' <summary>
			''' Add a new features array to the iterator </summary>
			''' <param name="shape">  Shape of the features </param>
			''' <param name="values"> Values to fill the array with </param>
			Public Overridable Function addFeatures(ByVal shape() As Long, ByVal values As Values) As Builder
				Return addFeatures(shape, "c"c, values)
			End Function

			''' <summary>
			''' Add a new features array to the iterator </summary>
			''' <param name="shape">  Shape of the features </param>
			''' <param name="order">  Order ('c' or 'f') for the array </param>
			''' <param name="values"> Values to fill the array with </param>
			Public Overridable Function addFeatures(ByVal shape() As Long, ByVal order As Char, ByVal values As Values) As Builder
				features.Add(New Triple(Of Long(), Char, Values)(shape, order, values))
				Return Me
			End Function

			''' <summary>
			''' Add a new labels array to the iterator </summary>
			''' <param name="shape">  Shape of the features </param>
			''' <param name="values"> Values to fill the array with </param>
			Public Overridable Function addLabels(ByVal shape() As Long, ByVal values As Values) As Builder
				Return addLabels(shape, "c"c, values)
			End Function

			''' <summary>
			''' Add a new labels array to the iterator </summary>
			''' <param name="shape">  Shape of the features </param>
			''' <param name="order">  Order ('c' or 'f') for the array </param>
			''' <param name="values"> Values to fill the array with </param>
			Public Overridable Function addLabels(ByVal shape() As Long, ByVal order As Char, ByVal values As Values) As Builder
				labels.Add(New Triple(Of Long(), Char, Values)(shape, order, values))
				Return Me
			End Function

			Public Overridable Function build() As RandomMultiDataSetIterator
				Return New RandomMultiDataSetIterator(numMiniBatches, features, labels)
			End Function
		End Class

		''' <summary>
		''' Generate a random array with the specified shape </summary>
		''' <param name="shape">  Shape of the array </param>
		''' <param name="values"> Values to fill the array with </param>
		''' <returns> Random array of specified shape + contents </returns>
		Public Shared Function generate(ByVal shape() As Long, ByVal values As Values) As INDArray
			Return generate(shape, Nd4j.order(), values)
		End Function

		''' <summary>
		''' Generate a random array with the specified shape and order </summary>
		''' <param name="shape">  Shape of the array </param>
		''' <param name="order">  Order of array ('c' or 'f') </param>
		''' <param name="values"> Values to fill the array with </param>
		''' <returns> Random array of specified shape + contents </returns>
		Public Shared Function generate(ByVal shape() As Long, ByVal order As Char, ByVal values As Values) As INDArray
			Select Case values
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.RANDOM_UNIFORM
					Return Nd4j.rand(Nd4j.createUninitialized(shape,order))
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.RANDOM_NORMAL
					Return Nd4j.randn(Nd4j.createUninitialized(shape,order))
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.ONE_HOT
					Dim r As New Random(CInt(Nd4j.Random.nextLong()))
					Dim [out] As INDArray = Nd4j.create(shape,order)
					If shape.Length = 1 Then
						[out].putScalar(r.Next(CInt(shape(0))), 1.0)
					ElseIf shape.Length = 2 Then
						Dim i As Integer=0
						Do While i<shape(0)
							[out].putScalar(i, r.Next(CInt(shape(1))), 1.0)
							i += 1
						Loop
					ElseIf shape.Length = 3 Then
						Dim i As Integer=0
						Do While i<shape(0)
							Dim j As Integer=0
							Do While j<shape(2)
								[out].putScalar(i, r.Next(CInt(shape(1))), j, 1.0)
								j += 1
							Loop
							i += 1
						Loop
					ElseIf shape.Length = 4 Then
						Dim i As Integer=0
						Do While i<shape(0)
							Dim j As Integer=0
							Do While j<shape(2)
								Dim k As Integer=0
								Do While k<shape(3)
									[out].putScalar(i, r.Next(CInt(shape(1))), j, k, 1.0)
									k += 1
								Loop
								j += 1
							Loop
							i += 1
						Loop
					ElseIf shape.Length = 5 Then
						Dim i As Integer=0
						Do While i<shape(0)
							Dim j As Integer=0
							Do While j<shape(2)
								Dim k As Integer=0
								Do While k<shape(3)
									Dim l As Integer=0
									Do While l<shape(4)
										[out].putScalar(New Integer(){i, r.Next(CInt(shape(1))), j, k, l}, 1.0)
										l += 1
									Loop
									k += 1
								Loop
								j += 1
							Loop
							i += 1
						Loop
					Else
						Throw New Exception("Not supported: rank 6+ arrays. Shape: " & java.util.Arrays.toString(shape))
					End If
					Return [out]
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.ZEROS
					Return Nd4j.create(shape,order)
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.ONES
					Return Nd4j.createUninitialized(shape,order).assign(1.0)
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.BINARY
					Return Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(shape, order), 0.5))
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.INTEGER_0_10
					Return Transforms.floor(Nd4j.rand(shape).muli(10), False)
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.INTEGER_0_100
					Return Transforms.floor(Nd4j.rand(shape).muli(100), False)
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.INTEGER_0_1000
					Return Transforms.floor(Nd4j.rand(shape).muli(1000), False)
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.INTEGER_0_10000
					Return Transforms.floor(Nd4j.rand(shape).muli(10000), False)
				Case org.deeplearning4j.datasets.iterator.RandomMultiDataSetIterator.Values.INTEGER_0_100000
					Return Transforms.floor(Nd4j.rand(shape).muli(100000), False)
				Case Else
					Throw New Exception("Unknown enum value: " & values)

			End Select
		End Function

	End Class

End Namespace