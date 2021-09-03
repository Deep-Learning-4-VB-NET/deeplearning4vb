Imports System
Imports System.Collections.Generic
Imports System.IO
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports SplitTestAndTrain = org.nd4j.linalg.dataset.SplitTestAndTrain
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

Namespace org.nd4j.linalg.dataset.api


	Public Interface DataSet
		Inherits IEnumerable(Of org.nd4j.linalg.dataset.DataSet)


		Function getRange(ByVal from As Integer, ByVal [to] As Integer) As DataSet

		''' <summary>
		''' Load the contents of the DataSet from the specified InputStream. The current contents of the DataSet (if any) will be replaced.<br>
		''' The InputStream should contain a DataSet that has been serialized with <seealso cref="save(OutputStream)"/>
		''' </summary>
		''' <param name="from"> InputStream to load the DataSet from </param>
		Sub load(ByVal from As Stream)

		''' <summary>
		''' Load the contents of the DataSet from the specified File. The current contents of the DataSet (if any) will be replaced.<br>
		''' The InputStream should contain a DataSet that has been serialized with <seealso cref="save(File)"/>
		''' </summary>
		''' <param name="from"> File to load the DataSet from </param>
		Sub load(ByVal from As File)

		''' <summary>
		''' Write the contents of this DataSet to the specified OutputStream
		''' </summary>
		''' <param name="to"> OutputStream to save the DataSet to </param>
		Sub save(ByVal [to] As Stream)

		''' <summary>
		''' Save this DataSet to a file. Can be loaded again using <seealso cref=""/>
		''' </summary>
		''' <param name="to">    File to sa </param>
		Sub save(ByVal [to] As File)

		<Obsolete>
		Function iterateWithMiniBatches() As DataSetIterator

		Function id() As String

		''' <summary>
		''' Returns the features array for the DataSet
		''' </summary>
		''' <returns> features array </returns>
		Property Features As INDArray


		''' <summary>
		''' Calculate and return a count of each label, by index.
		''' Assumes labels are a one-hot INDArray, for classification
		''' </summary>
		''' <returns> Map of countsn </returns>
		Function labelCounts() As IDictionary(Of Integer, Double)

		''' <summary>
		''' Create a copy of the DataSet
		''' </summary>
		''' <returns> Copy of the DataSet </returns>
		Function copy() As org.nd4j.linalg.dataset.DataSet

		Function reshape(ByVal rows As Integer, ByVal cols As Integer) As org.nd4j.linalg.dataset.DataSet

		''' <summary>
		''' Multiply the features by a scalar
		''' </summary>
		Sub multiplyBy(ByVal num As Double)

		''' <summary>
		''' Divide the features by a scalar
		''' </summary>
		Sub divideBy(ByVal num As Integer)

		''' <summary>
		''' Shuffle the order of the rows in the DataSet. Note that this generally won't make any difference in practice
		''' unless the DataSet is later split.
		''' </summary>
		Sub shuffle()

		Sub squishToRange(ByVal min As Double, ByVal max As Double)

		Sub scaleMinAndMax(ByVal min As Double, ByVal max As Double)

		Sub scale()

		Sub addFeatureVector(ByVal toAdd As INDArray)

		Sub addFeatureVector(ByVal feature As INDArray, ByVal example As Integer)

		''' <summary>
		''' Normalize this DataSet to mean 0, stdev 1 per input.
		''' This calculates statistics based on the values in a single DataSet only.
		''' For normalization over multiple DataSet objects, use <seealso cref="org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize"/>
		''' </summary>
		Sub normalize()

		Sub binarize()

		Sub binarize(ByVal cutoff As Double)

		''' @deprecated Use <seealso cref="org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize""/>")>
		Sub normalizeZeroMeanZeroUnitVariance()

		''' <summary>
		''' Number of input values - i.e., size of the features INDArray per example
		''' </summary>
		Function numInputs() As Integer

		Sub validate()

		Function outcome() As Integer

		WriteOnly Property NewNumberOfLabels As Integer

		Sub setOutcome(ByVal example As Integer, ByVal label As Integer)

		Function get(ByVal i As Integer) As org.nd4j.linalg.dataset.DataSet

		Function get(ByVal i() As Integer) As org.nd4j.linalg.dataset.DataSet

		Function batchBy(ByVal num As Integer) As IList(Of org.nd4j.linalg.dataset.DataSet)

		Function filterBy(ByVal labels() As Integer) As org.nd4j.linalg.dataset.DataSet

		Sub filterAndStrip(ByVal labels() As Integer)

		''' @deprecated prefer <seealso cref="batchBy(Integer)"/> 
		<Obsolete("prefer <seealso cref=""batchBy(Integer)""/>")>
		Function dataSetBatches(ByVal num As Integer) As IList(Of org.nd4j.linalg.dataset.DataSet)

		Function sortAndBatchByNumLabels() As IList(Of org.nd4j.linalg.dataset.DataSet)

		Function batchByNumLabels() As IList(Of org.nd4j.linalg.dataset.DataSet)

		''' <summary>
		''' Extract each example in the DataSet into its own DataSet object, and return all of them as a list </summary>
		''' <returns> List of DataSet objects, each with 1 example only </returns>
		Function asList() As IList(Of org.nd4j.linalg.dataset.DataSet)

		Function splitTestAndTrain(ByVal numHoldout As Integer, ByVal rnd As Random) As SplitTestAndTrain

		Function splitTestAndTrain(ByVal numHoldout As Integer) As SplitTestAndTrain

		Property Labels As INDArray


		Sub sortByLabel()

		Sub addRow(ByVal d As org.nd4j.linalg.dataset.DataSet, ByVal i As Integer)

		Function exampleSums() As INDArray

		Function exampleMaxs() As INDArray

		Function exampleMeans() As INDArray

		Function sample(ByVal numSamples As Integer) As org.nd4j.linalg.dataset.DataSet

		Function sample(ByVal numSamples As Integer, ByVal rng As Random) As org.nd4j.linalg.dataset.DataSet

		Function sample(ByVal numSamples As Integer, ByVal withReplacement As Boolean) As org.nd4j.linalg.dataset.DataSet

		Function sample(ByVal numSamples As Integer, ByVal rng As Random, ByVal withReplacement As Boolean) As org.nd4j.linalg.dataset.DataSet

		Sub roundToTheNearest(ByVal roundTo As Integer)

		''' <summary>
		''' Returns the number of outcomes (size of the labels array for each example)
		''' </summary>
		Function numOutcomes() As Integer

		''' <summary>
		''' Number of examples in the DataSet
		''' </summary>
		Function numExamples() As Integer

		<Obsolete>
		Property LabelNames As IList(Of String)

		ReadOnly Property LabelNamesList As IList(Of String)

		Function getLabelName(ByVal idx As Integer) As String

		Function getLabelNames(ByVal idxs As INDArray) As IList(Of String)


		Property ColumnNames As IList(Of String)


		''' <summary>
		''' SplitV the DataSet into two DataSets randomly </summary>
		''' <param name="fractionTrain">    Fraction (in range 0 to 1) of examples to be returned in the training DataSet object </param>
		Function splitTestAndTrain(ByVal fractionTrain As Double) As SplitTestAndTrain

		Overrides Function iterator() As IEnumerator(Of org.nd4j.linalg.dataset.DataSet)

		''' <summary>
		''' Input mask array: a mask array for input, where each value is in {0,1} in order to specify whether an input is
		''' actually present or not. Typically used for situations such as RNNs with variable length inputs
		''' </summary>
		''' <returns> Input mask array </returns>
		Property FeaturesMaskArray As INDArray


		''' <summary>
		''' Labels (output) mask array: a mask array for input, where each value is in {0,1} in order to specify whether an
		''' output is actually present or not. Typically used for situations such as RNNs with variable length inputs or many-
		''' to-one situations.
		''' </summary>
		''' <returns> Labels (output) mask array </returns>
		Property LabelsMaskArray As INDArray


		''' <summary>
		''' Whether the labels or input (features) mask arrays are present for this DataSet
		''' </summary>
		Function hasMaskArrays() As Boolean

		''' <summary>
		''' Set the metadata for this DataSet<br>
		''' By convention: the metadata can be any serializable object, one per example in the DataSet
		''' </summary>
		''' <param name="exampleMetaData"> Example metadata to set </param>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void setExampleMetaData(java.util.List<? extends java.io.Serializable> exampleMetaData);
		Property ExampleMetaData(Of T1 As Serializable) As IList(Of T1)

		''' <summary>
		''' Get the example metadata, or null if no metadata has been set<br>
		''' Note: this method results in an unchecked cast - care should be taken when using this!
		''' </summary>
		''' <param name="metaDataType"> Class of the metadata (used for opType information) </param>
		''' @param <T>          Type of metadata </param>
		''' <returns> List of metadata objects </returns>
		 Function getExampleMetaData(Of T As Serializable)(ByVal metaDataType As Type(Of T)) As IList(Of T)


		''' <summary>
		''' This method returns memory used by this DataSet
		''' @return
		''' </summary>
		ReadOnly Property MemoryFootprint As Long

		''' <summary>
		''' This method migrates this DataSet into current Workspace (if any)
		''' </summary>
		Sub migrate()

		''' <summary>
		''' This method detaches this DataSet from current Workspace (if any)
		''' </summary>
		Sub detach()

		''' <returns> true if the DataSet object is empty (no features, labels, or masks) </returns>
		ReadOnly Property Empty As Boolean

		Function toMultiDataSet() As MultiDataSet
	End Interface

End Namespace