Imports System
Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

Namespace org.deeplearning4j.nn.api



	Public Interface Classifier
		Inherits Model



		''' <summary>
		''' Sets the input and labels and returns a score for the prediction
		''' wrt true labels </summary>
		''' <param name="data"> the data to score </param>
		''' <returns> the score for the given input,label pairs </returns>
		Function f1Score(ByVal data As DataSet) As Double

		''' <summary>
		''' Returns the f1 score for the given examples.
		''' Think of this to be like a percentage right.
		''' The higher the number the more it got right.
		''' This is on a scale from 0 to 1. </summary>
		''' <param name="examples"> te the examples to classify (one example in each row) </param>
		''' <param name="labels"> the true labels </param>
		''' <returns> the scores for each ndarray </returns>
		Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double

		''' <summary>
		''' Returns the number of possible labels </summary>
		''' <returns> the number of possible labels for this classifier </returns>
		''' @deprecated Will be removed in a future release 
		<Obsolete("Will be removed in a future release")>
		Function numLabels() As Integer

		''' <summary>
		''' Train the model based on the datasetiterator </summary>
		''' <param name="iter"> the iterator to train on </param>
		Sub fit(ByVal iter As DataSetIterator)

		''' <summary>
		''' Takes in a list of examples
		''' For each row, returns a label </summary>
		''' <param name="examples"> the examples to classify (one example in each row) </param>
		''' <returns> the labels for each example </returns>
		Function predict(ByVal examples As INDArray) As Integer()

		''' <summary>
		''' Takes in a DataSet of examples
		''' For each row, returns a label </summary>
		''' <param name="dataSet"> the examples to classify </param>
		''' <returns> the labels for each example </returns>
		Function predict(ByVal dataSet As DataSet) As IList(Of String)


		''' <summary>
		''' Fit the model </summary>
		''' <param name="examples"> the examples to classify (one example in each row) </param>
		''' <param name="labels"> the example labels(a binary outcome matrix) </param>
		Sub fit(ByVal examples As INDArray, ByVal labels As INDArray)

		''' <summary>
		''' Fit the model </summary>
		''' <param name="data"> the data to train on </param>
		Sub fit(ByVal data As DataSet)



		''' <summary>
		''' Fit the model </summary>
		''' <param name="examples"> the examples to classify (one example in each row) </param>
		''' <param name="labels"> the labels for each example (the number of labels must match
		'''               the number of rows in the example </param>
		Sub fit(ByVal examples As INDArray, ByVal labels() As Integer)



	End Interface

End Namespace