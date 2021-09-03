Imports System
Imports System.Collections.Generic
Imports System.IO
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


	Public Interface MultiDataSet

		''' <summary>
		''' Number of arrays of features/input data in the MultiDataSet
		''' </summary>
		Function numFeatureArrays() As Integer

		''' <summary>
		''' Number of arrays of label/output data in the MultiDataSet
		''' </summary>
		Function numLabelsArrays() As Integer

		''' <summary>
		''' Get all of the input features, as an array of INDArrays
		''' </summary>
		Property Features As INDArray()

		''' <summary>
		''' Get a single feature/input array
		''' </summary>
		Function getFeatures(ByVal index As Integer) As INDArray


		''' <summary>
		''' Set a single features array (by index) for the MultiDataSet
		''' </summary>
		Sub setFeatures(ByVal idx As Integer, ByVal features As INDArray)

		''' <summary>
		''' Get all of the labels, as an array of INDArrays
		''' </summary>
		Property Labels As INDArray()

		''' <summary>
		''' Get a single label/output array
		''' </summary>
		Function getLabels(ByVal index As Integer) As INDArray


		''' <summary>
		''' Set a single labels array (by index) for the MultiDataSet
		''' </summary>
		Sub setLabels(ByVal idx As Integer, ByVal labels As INDArray)

		''' <summary>
		''' Whether there are any mask arrays (features or labels) present for this MultiDataSet
		''' </summary>
		Function hasMaskArrays() As Boolean

		''' <summary>
		''' Get the feature mask arrays. May return null if no feature mask arrays are present; otherwise,
		''' any entry in the array may be null if no mask array is present for that particular feature
		''' </summary>
		Property FeaturesMaskArrays As INDArray()

		''' <summary>
		''' Get the specified feature mask array. Returns null if no feature mask is present for that particular feature/input
		''' </summary>
		Function getFeaturesMaskArray(ByVal index As Integer) As INDArray


		''' <summary>
		''' Set a single feature mask array by index
		''' </summary>
		Sub setFeaturesMaskArray(ByVal idx As Integer, ByVal maskArray As INDArray)

		''' <summary>
		''' Get the labels mask arrays. May return null if no labels mask arrays are present; otherwise,
		''' any entry in the array may be null if no mask array is present for that particular label
		''' </summary>
		ReadOnly Property LabelsMaskArrays As INDArray()

		''' <summary>
		''' Get the specified label mask array. Returns null if no label mask is present for that particular feature/input
		''' </summary>
		Function getLabelsMaskArray(ByVal index As Integer) As INDArray

		''' <summary>
		''' Set the labels mask arrays
		''' </summary>
		WriteOnly Property LabelsMaskArray As INDArray()

		''' <summary>
		''' Set a single labels mask array by index
		''' </summary>
		Sub setLabelsMaskArray(ByVal idx As Integer, ByVal labelsMaskArray As INDArray)

		''' <summary>
		''' Save this MultiDataSet to the specified stream. Stream will be closed after saving.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void save(OutputStream to) throws IOException;
		Sub save(ByVal [to] As Stream)

		''' <summary>
		''' Save this MultiDataSet to the specified file
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void save(File to) throws IOException;
		Sub save(ByVal [to] As File)

		''' <summary>
		''' Load the contents of this MultiDataSet from the specified stream. Stream will be closed after loading.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void load(InputStream from) throws IOException;
		Sub load(ByVal from As Stream)

		''' <summary>
		''' Load the contents of this MultiDataSet from the specified file
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void load(File from) throws IOException;
		Sub load(ByVal from As File)

		''' <summary>
		''' SplitV the MultiDataSet into a list of individual examples.
		''' </summary>
		''' <returns> List of MultiDataSets, each with 1 example </returns>
		Function asList() As IList(Of MultiDataSet)

		''' <summary>
		''' Clone the dataset
		''' </summary>
		''' <returns> a clone of the dataset </returns>
		Function copy() As MultiDataSet

		''' <summary>
		''' Set the metadata for this MultiDataSet<br>
		''' By convention: the metadata can be any serializable object, one per example in the MultiDataSet
		''' </summary>
		''' <param name="exampleMetaData"> Example metadata to set </param>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void setExampleMetaData(java.util.List<? extends Serializable> exampleMetaData);
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
		''' This method returns memory amount occupied by this MultiDataSet.
		''' </summary>
		''' <returns> value in bytes </returns>
		ReadOnly Property MemoryFootprint As Long

		''' <summary>
		''' This method migrates this MultiDataSet into current Workspace (if any)
		''' </summary>
		Sub migrate()

		''' <summary>
		''' This method detaches this MultiDataSet from current Workspace (if any)
		''' </summary>
		Sub detach()

		''' <returns> True if the MultiDataSet is empty (all features/labels arrays are empty) </returns>
		ReadOnly Property Empty As Boolean

		''' <summary>
		''' Shuffle the order of the examples in the MultiDataSet. Note that this generally won't make any difference in
		''' practice unless the MultiDataSet is later split.
		''' </summary>
		Sub shuffle()
	End Interface

End Namespace