Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports EmnistDataFetcher = org.deeplearning4j.datasets.fetchers.EmnistDataFetcher
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports BaseDatasetIterator = org.nd4j.linalg.dataset.api.iterator.BaseDatasetIterator

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

Namespace org.deeplearning4j.datasets.iterator.impl


	<Serializable>
	Public Class EmnistDataSetIterator
		Inherits BaseDatasetIterator

		Private Const NUM_COMPLETE_TRAIN As Integer = 697932
		Private Const NUM_COMPLETE_TEST As Integer = 116323

		Private Const NUM_MERGE_TRAIN As Integer = 697932
		Private Const NUM_MERGE_TEST As Integer = 116323

		Private Const NUM_BALANCED_TRAIN As Integer = 112800
		Private Const NUM_BALANCED_TEST As Integer = 18800

		Private Const NUM_DIGITS_TRAIN As Integer = 240000
		Private Const NUM_DIGITS_TEST As Integer = 40000

		Private Const NUM_LETTERS_TRAIN As Integer = 88800
		Private Const NUM_LETTERS_TEST As Integer = 14800

		Private Const NUM_MNIST_TRAIN As Integer = 60000
		Private Const NUM_MNIST_TEST As Integer = 10000

		Private Shared ReadOnly LABELS_COMPLETE() As Char = {ChrW(48), ChrW(49), ChrW(50), ChrW(51), ChrW(52), ChrW(53), ChrW(54), ChrW(55), ChrW(56), ChrW(57), ChrW(65), ChrW(66), ChrW(67), ChrW(68), ChrW(69), ChrW(70), ChrW(71), ChrW(72), ChrW(73), ChrW(74), ChrW(75), ChrW(76), ChrW(77), ChrW(78), ChrW(79), ChrW(80), ChrW(81), ChrW(82), ChrW(83), ChrW(84), ChrW(85), ChrW(86), ChrW(87), ChrW(88), ChrW(89), ChrW(90), ChrW(97), ChrW(98), ChrW(99), ChrW(100), ChrW(101), ChrW(102), ChrW(103), ChrW(104), ChrW(105), ChrW(106), ChrW(107), ChrW(108), ChrW(109), ChrW(110), ChrW(111), ChrW(112), ChrW(113), ChrW(114), ChrW(115), ChrW(116), ChrW(117), ChrW(118), ChrW(119), ChrW(120), ChrW(121), ChrW(122)}

		Private Shared ReadOnly LABELS_MERGE() As Char = {ChrW(48), ChrW(49), ChrW(50), ChrW(51), ChrW(52), ChrW(53), ChrW(54), ChrW(55), ChrW(56), ChrW(57), ChrW(65), ChrW(66), ChrW(67), ChrW(68), ChrW(69), ChrW(70), ChrW(71), ChrW(72), ChrW(73), ChrW(74), ChrW(75), ChrW(76), ChrW(77), ChrW(78), ChrW(79), ChrW(80), ChrW(81), ChrW(82), ChrW(83), ChrW(84), ChrW(85), ChrW(86), ChrW(87), ChrW(88), ChrW(89), ChrW(90), ChrW(97), ChrW(98), ChrW(100), ChrW(101), ChrW(102), ChrW(103), ChrW(104), ChrW(110), ChrW(113), ChrW(114), ChrW(116)}

		Private Shared ReadOnly LABELS_BALANCED() As Char = {ChrW(48), ChrW(49), ChrW(50), ChrW(51), ChrW(52), ChrW(53), ChrW(54), ChrW(55), ChrW(56), ChrW(57), ChrW(65), ChrW(66), ChrW(67), ChrW(68), ChrW(69), ChrW(70), ChrW(71), ChrW(72), ChrW(73), ChrW(74), ChrW(75), ChrW(76), ChrW(77), ChrW(78), ChrW(79), ChrW(80), ChrW(81), ChrW(82), ChrW(83), ChrW(84), ChrW(85), ChrW(86), ChrW(87), ChrW(88), ChrW(89), ChrW(90), ChrW(97), ChrW(98), ChrW(100), ChrW(101), ChrW(102), ChrW(103), ChrW(104), ChrW(110), ChrW(113), ChrW(114), ChrW(116)}

		Private Shared ReadOnly LABELS_DIGITS() As Char = {ChrW(48), ChrW(49), ChrW(50), ChrW(51), ChrW(52), ChrW(53), ChrW(54), ChrW(55), ChrW(56), ChrW(57)}

		Private Shared ReadOnly LABELS_LETTERS() As Char = {ChrW(65), ChrW(66), ChrW(67), ChrW(68), ChrW(69), ChrW(70), ChrW(71), ChrW(72), ChrW(73), ChrW(74), ChrW(75), ChrW(76), ChrW(77), ChrW(78), ChrW(79), ChrW(80), ChrW(81), ChrW(82), ChrW(83), ChrW(84), ChrW(85), ChrW(86), ChrW(87), ChrW(88), ChrW(89), ChrW(90)}

		''' <summary>
		''' EMNIST dataset has multiple different subsets. See <seealso cref="EmnistDataSetIterator"/> Javadoc for details.
		''' </summary>
		Public Enum [Set]
			COMPLETE
			MERGE
			BALANCED
			LETTERS
			DIGITS
			MNIST
		End Enum

		Protected Friend dataSet As [Set]
'JAVA TO VB CONVERTER NOTE: The field numExamples was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shadows batch_Conflict, numExamples_Conflict As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
		Protected Friend Shadows preProcessor_Conflict As DataSetPreProcessor

		''' <summary>
		''' Create an EMNIST iterator with randomly shuffled data based on a random RNG seed
		''' </summary>
		''' <param name="dataSet"> Dataset (subset) to return </param>
		''' <param name="batch">   Batch size </param>
		''' <param name="train">   If true: use training set. If false: use test set </param>
		''' <exception cref="IOException"> If an error occurs when loading/downloading the dataset </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public EmnistDataSetIterator(@Set dataSet, int batch, boolean train) throws java.io.IOException
		Public Sub New(ByVal dataSet As [Set], ByVal batch As Integer, ByVal train As Boolean)
			Me.New(dataSet, batch, train, DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		''' <summary>
		''' Create an EMNIST iterator with randomly shuffled data based on a specified RNG seed
		''' </summary>
		''' <param name="dataSet">   Dataset (subset) to return </param>
		''' <param name="batchSize"> Batch size </param>
		''' <param name="train">     If true: use training set. If false: use test set </param>
		''' <param name="seed">      Random number generator seed </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public EmnistDataSetIterator(@Set dataSet, int batchSize, boolean train, long seed) throws java.io.IOException
		Public Sub New(ByVal dataSet As [Set], ByVal batchSize As Integer, ByVal train As Boolean, ByVal seed As Long)
			Me.New(dataSet, batchSize, False, train, True, seed)
		End Sub

		''' <summary>
		''' Get the specified number of MNIST examples (test or train set), with optional shuffling and binarization.
		''' </summary>
		''' <param name="batch">    Size of each minibatch </param>
		''' <param name="binarize"> whether to binarize the data or not (if false: normalize in range 0 to 1) </param>
		''' <param name="train">    Train vs. test set </param>
		''' <param name="shuffle">  whether to shuffle the examples </param>
		''' <param name="rngSeed">  random number generator seed to use when shuffling examples </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public EmnistDataSetIterator(@Set dataSet, int batch, boolean binarize, boolean train, boolean shuffle, long rngSeed) throws java.io.IOException
		Public Sub New(ByVal dataSet As [Set], ByVal batch As Integer, ByVal binarize As Boolean, ByVal train As Boolean, ByVal shuffle As Boolean, ByVal rngSeed As Long)
			MyBase.New(batch, numExamples(train, dataSet), New EmnistDataFetcher(dataSet, binarize, train, shuffle, rngSeed))
			Me.dataSet = dataSet
		End Sub

		Private Shared Function numExamples(ByVal train As Boolean, ByVal ds As [Set]) As Integer
			If train Then
				Return numExamplesTrain(ds)
			Else
				Return numExamplesTest(ds)
			End If
		End Function

		''' <summary>
		''' Get the number of training examples for the specified subset
		''' </summary>
		''' <param name="dataSet"> Subset to get </param>
		''' <returns> Number of examples for the specified subset </returns>
		Public Shared Function numExamplesTrain(ByVal dataSet As [Set]) As Integer
			Select Case dataSet
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.COMPLETE
					Return NUM_COMPLETE_TRAIN
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.MERGE
					Return NUM_MERGE_TRAIN
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.BALANCED
					Return NUM_BALANCED_TRAIN
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.LETTERS
					Return NUM_LETTERS_TRAIN
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.DIGITS
					Return NUM_DIGITS_TRAIN
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.MNIST
					Return NUM_MNIST_TRAIN
				Case Else
					Throw New System.NotSupportedException("Unknown Set: " & dataSet)
			End Select
		End Function

		''' <summary>
		''' Get the number of test examples for the specified subset
		''' </summary>
		''' <param name="dataSet"> Subset to get </param>
		''' <returns> Number of examples for the specified subset </returns>
		Public Shared Function numExamplesTest(ByVal dataSet As [Set]) As Integer
			Select Case dataSet
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.COMPLETE
					Return NUM_COMPLETE_TEST
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.MERGE
					Return NUM_MERGE_TEST
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.BALANCED
					Return NUM_BALANCED_TEST
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.LETTERS
					Return NUM_LETTERS_TEST
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.DIGITS
					Return NUM_DIGITS_TEST
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.MNIST
					Return NUM_MNIST_TEST
				Case Else
					Throw New System.NotSupportedException("Unknown Set: " & dataSet)
			End Select
		End Function

		''' <summary>
		''' Get the number of labels for the specified subset
		''' </summary>
		''' <param name="dataSet"> Subset to get </param>
		''' <returns> Number of labels for the specified subset </returns>
		Public Shared Function numLabels(ByVal dataSet As [Set]) As Integer
			Select Case dataSet
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.COMPLETE
					Return 62
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.MERGE
					Return 47
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.BALANCED
					Return 47
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.LETTERS
					Return 26
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.DIGITS
					Return 10
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.MNIST
					Return 10
				Case Else
					Throw New System.NotSupportedException("Unknown Set: " & dataSet)
			End Select
		End Function

		''' <summary>
		''' Get the labels as a character array
		''' </summary>
		''' <returns> Labels </returns>
		Public Overridable ReadOnly Property LabelsArrays As Char()
			Get
				Return getLabelsArray(dataSet)
			End Get
		End Property

		''' <summary>
		''' Get the labels as a List<String>
		''' </summary>
		''' <returns> Labels </returns>
		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Return getLabels(dataSet)
			End Get
		End Property

		''' <summary>
		''' Get the label assignments for the given set as a character array.
		''' </summary>
		''' <param name="dataSet"> DataSet to get the label assignment for </param>
		''' <returns> Label assignment and given dataset </returns>
		Public Shared Function getLabelsArray(ByVal dataSet As [Set]) As Char()
			Select Case dataSet
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.COMPLETE
					Return LABELS_COMPLETE
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.MERGE
					Return LABELS_MERGE
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.BALANCED
					Return LABELS_BALANCED
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.LETTERS
					Return LABELS_LETTERS
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.DIGITS, MNIST
					Return LABELS_DIGITS
				Case Else
					Throw New System.NotSupportedException("Unknown Set: " & dataSet)
			End Select
		End Function

		''' <summary>
		''' Get the label assignments for the given set as a List<String>
		''' </summary>
		''' <param name="dataSet"> DataSet to get the label assignment for </param>
		''' <returns> Label assignment and given dataset </returns>
		Public Shared Overloads Function getLabels(ByVal dataSet As [Set]) As IList(Of String)
			Dim c() As Char = getLabelsArray(dataSet)
			Dim l As IList(Of String) = New List(Of String)(c.Length)
			For Each c2 As Char In c
				l.Add(c2.ToString())
			Next c2
			Return l
		End Function

		''' <summary>
		''' Are the labels balanced in the training set (that is: are the number of examples for each label equal?)
		''' </summary>
		''' <param name="dataSet"> Set to get balanced value for </param>
		''' <returns> True if balanced dataset, false otherwise </returns>
		Public Shared Function isBalanced(ByVal dataSet As [Set]) As Boolean
			Select Case dataSet
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.COMPLETE, MERGE, LETTERS
					'Note: EMNIST docs claims letters is balanced, but this is not possible for training set:
					' 88800 examples / 26 classes = 3418.46
					Return False
				Case org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator.Set.BALANCED, DIGITS, MNIST
					Return True
				Case Else
					Throw New System.NotSupportedException("Unknown Set: " & dataSet)
			End Select
		End Function
	End Class

End Namespace